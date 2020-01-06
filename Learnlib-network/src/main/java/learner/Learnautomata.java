package learner;

import de.ls5.jlearn.algorithms.packs.ObservationPack;
import de.ls5.jlearn.interfaces.Automaton;
import de.ls5.jlearn.interfaces.EquivalenceOracleOutput;
import de.ls5.jlearn.interfaces.EquivalenceOracle;
import de.ls5.jlearn.interfaces.Learner;
import de.ls5.jlearn.interfaces.Word;
import de.ls5.jlearn.logging.LearnLog;
import de.ls5.jlearn.logging.LogLevel;
import de.ls5.jlearn.logging.PrintStreamLoggingAppender;
import de.ls5.jlearn.util.DotUtil;
import learner.Configuration.Config;
import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.PrintStream;
import java.net.Socket;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.EnumMap;
import java.util.List;
import java.util.Properties;
import java.util.stream.Collectors;
import learner.Main;
import learner.Main.EquType;

public class Learnautomata {
	public static Automaton learn(Config config) throws Exception {
		if (!new File(Main.outDir).exists()) {
			new File(Main.outDir).mkdirs();
		}
		// All output goes to file
		Main.statisticsFileStream = new PrintStream(new FileOutputStream(Main.statisticsFile, false));
		PrintStream fileStream = new PrintStream(new FileOutputStream(Main.outputFile, false));
		System.setOut(fileStream);
		//Main.statisticsFileStream.println(yannakakisRandomCmd); // Check if eclipse
															// is not being dumb
															// and is running
															// old commands
		//statisticsFileStream.println(yannakakisExhaustiveCmd);
		// Set up a connection
		//
		Socket sock = new Socket("localhost", 5059);
		System.out.println("Created a shared socket on local port " + sock.getLocalPort());
		sock.setTcpNoDelay(true);

		LearnLog.addAppender(
				new PrintStreamLoggingAppender(LogLevel.DEBUG, new PrintStream(new FileOutputStream(Main.learnLog, false))));

		// Some debug info
		System.out.println("Started the main process");
		System.out.println("Maximum number of traces: " + Main.maxNumTraces);
		System.out.println("Timestamp: " + Main.timestamp);
		System.out.println("Configuration: " + config);

		// if cache not present, make a copy of an empty db
		if (!new File(Main.dbFile).exists()) {
			Path emptyDbPath = Paths.get(Main.emptyDbFile);
			Files.copy(emptyDbPath, new FileOutputStream(Main.dbFile));
		}

		// Set up database
		Logger sqllog = new Logger(Main.dbFile, config.sutName);

		// And mapper
		Mapper mapper = new Mapper(sock, sqllog);

		// Create learnlib objects: membershipOracle, EquivalenceOracles and
		// Learner
		MembershipOracle memberOracle = new MembershipOracle(sock, sqllog);
		mapper.setMembershipOracle(memberOracle);

		// Set up the eqOracleMap (equType -> equOracle)
		EnumMap<EquType, EquivalenceOracle> equOracleMap = new EnumMap<EquType, EquivalenceOracle>(EquType.class);
		for (EquType equType : config.equOracleTypes) {
			EquivalenceOracle eqOracle = null;
			switch (equType) {
			case RANDOM:
				//eqOracle = new YannakakisEquivalenceOracle(yannakakisRandomCmd, config.maxNumTests);
				break;
			case EXHAUSTIVE:
				//eqOracle = new YannakakisEquivalenceOracle(yannakakisExhaustiveCmd);
				break;
			case WORDS:
				eqOracle = new WordsEquivalenceOracle(config.testWords);
				break;
			case CONFORMANCE:
				//eqOracle = new ConformanceEquivalenceOracle(config.nusmvCmd, config.specFile);
				break;
			}
			eqOracle.setOracle(mapper);
			equOracleMap.put(equType, eqOracle);
		}

		Learner learner = null;

		// Set up the learner
		learner = new ObservationPack();
		learner.setOracle(memberOracle);
		learner.setAlphabet(mapper.generateInputAlphabet(config.alphabet));

		long start = System.currentTimeMillis();
		boolean done = false;
		int memQueries = 0, testQueries = 0;

		// Repeat until a hypothesis has been formed for which no counter
		// example can be found
		for (Main.hypCounter = 0; !done; Main.hypCounter++) {
			Automaton hyp = null;

			// Learn
			System.out.println("starting learning");
			learner.learn();
			System.out.println("done learning");

			// Print some stats
			Main.statisticsFileStream
					.println("Hypothesis " + Main.hypCounter + " after: " + (System.currentTimeMillis() - start) + "ms");
			Main.statisticsFileStream.println("Membership: " + (memberOracle.getNumQueries() - memQueries));
			memQueries = memberOracle.getNumQueries();
			// memberOracle.resetNumQueries();

			// Retrieve hypothesis and write to dot file
			hyp = learner.getResult();
			DotUtil.writeDot(hyp, new File(Main.hypFile(Main.hypCounter)));

			EquivalenceOracleOutput equivOutput = null;

			for (EquType equOracleType : config.equOracleTypes) {
				EquivalenceOracle eqOracle = equOracleMap.get(equOracleType);
				System.out.println("starting " + equOracleType.name() + " equivalence query");
				// mapper.setRetrieveFromCache(false);
				equivOutput = eqOracle.findCounterExample(hyp);
				if (equivOutput != null)
					break;
				// mapper.setRetrieveFromCache(true);
				System.out.println("done " + equOracleType.name() + " equivalence query");

				Main.statisticsFileStream
						.println(equOracleType.name() + " Equivalence: " + (mapper.getNumQueries() - testQueries));
				testQueries = mapper.getNumQueries();
				// mapper.resetNumQueries();
			}

			// Check for a counterexample
			if (equivOutput == null) {
				// No counterexample: close socket and done.
				System.out.println("No counterexample found; done!");
				SutWrapper sutWrapper = new SutWrapper(sock);
				String str=sutWrapper.Sendcompletelearning();
				String s=new String("FINISHLEARNING");
				if(!str.equals(s)){
					Main.newinputs=new ArrayList<String>();
					String[] newinputs=str.split(";");
					for(int i=0;i<newinputs.length;i++) {
						Main.newinputs.add((newinputs[i]));
					}
					sutWrapper.reset();
					Main.finishedliearning=false;
				}
				else {
				Main.finishedliearning=true;
				sock.close();
				}
				done = true;
				
			} else {
				// There is a counter example, send it to learnlib.
				Word counterExample = equivOutput.getCounterExample();

				Main.statisticsFileStream.println("Counter Example: " + counterExample.toString());
				System.out.println("Sending Counter Example to LearnLib.");
				System.out.println("Counter Example: " + counterExample.toString());
				learner.addCounterExample(counterExample, equivOutput.getOracleOutput());
			}
		}

		// End of learning, update some stats
		long end = System.currentTimeMillis();
		Main.statisticsFileStream.println("Total mem Queries: " + memQueries);
		Main.statisticsFileStream.println("Total test Queries: " + testQueries);
		Main.statisticsFileStream.println("Timestamp: " + Main.timestamp + ".");
		Main.statisticsFileStream.println("Running time: " + (end - start) + "ms.");
		Main.statisticsFileStream.close();

		// Get result
		Automaton learnedModel = learner.getResult();

		return learnedModel;
	}

}
