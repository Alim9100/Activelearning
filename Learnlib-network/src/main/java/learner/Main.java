package learner;

import java.awt.Desktop;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PrintStream;
import java.io.Writer;
import java.lang.management.ManagementFactory;
import java.net.Socket;
import java.nio.channels.FileChannel;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardCopyOption;
import java.util.ArrayList;
import java.util.List;
import java.util.logging.Level;

import de.ls5.jlearn.interfaces.Automaton;
import de.ls5.jlearn.util.DotUtil;
import learner.Configuration.Config;
import util.FileManager;
import util.SoundUtils;

public class Main {
	public static boolean finishedliearning=true;
	public static int learningphase=1;
	public static List<String> newinputs=null;
	public static final int maxNumTraces = 10000;
	public static final long timestamp = System.currentTimeMillis();
	//output
	public static final String outDir = "output/";
	public static final String outputFile = outDir + "out.txt";
	public static final String statisticsFile = outDir + "statistics.txt";
	public static final String learnedModelFile = outDir + "learnresult.dot";
	public static final String dbFile = outDir + "querylog.db";
	public static final String learnLog = outDir + "learnLog.txt";
	//input
	public static final String emptyDbFile = "input/querylog-empty.db";
	public static  String configFile = "input/config.prop";
	public static final String oldStatsFile = "input/statistics.txt";
	//variable
	public static int hypCounter = 0;
	public static PrintStream statisticsFileStream;
	//
	public static final String hypFile(int hypNum) {
		return outDir + "hypothesis-" + hypNum + ".dot";
	}
	
	//enum
	public static enum EquType {
		RANDOM, EXHAUSTIVE, WORDS, CONFORMANCE
	}
	//Main
	public static void main(String[] args) throws Exception {
		newinputs=new ArrayList<String>();
		final Config config = new Config(Main.configFile);
		Runtime.getRuntime().addShutdownHook(new Thread(new Runnable() {
			public void run() {
				try {
					copyToExpFolder(config.sutName, timestamp);
					
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		}));
		Automaton learnedModel = Learnautomata.learn(config);
		SoundUtils.success();
		File modelFile = new File(learnedModelFile);
		// Write final dot
		DotUtil.writeDot(learnedModel, modelFile);

		// Open dot file
		Desktop.getDesktop().open(modelFile);
		//////////////////////////////////////////////
//		Socket sock = new Socket("localhost", 5059);
//		System.out.println("Created a shared socket on local port " + sock.getLocalPort());
//		//sock.setTcpNoDelay(true);
//		SutWrapper sutWrapper = new SutWrapper(sock);
//		String str=sutWrapper.Sendcompletelearning();
//		String s=new String("FINISHLEARNING");
//		if(!str.equals(s)){
//			Main.newinputs=new ArrayList<String>();
//			String[] newinputs=str.split(";");
//			for(int i=0;i<newinputs.length;i++) {
//				Main.newinputs.add((newinputs[i]));
//			}
//		}
		///////////////
		if(finishedliearning==false) {
			//copyToExpFolder(config.sutName, timestamp);
			Copyperiviousphasefiles();
			Modifyconfigurationfile();
			Deleteoutputdir();
			learningphase++;
			main(args);
		}
		/////////////////////////////////////////////

	}
	public static void copyToExpFolder(String sutName, long timestamp) throws Exception {
		String expFolder = outDir + sutName + timestamp + "/";
		String[] hypFiles = new String[hypCounter + 1];
		for (int i = 0; i <= hypCounter; i++) {
			hypFiles[i] = hypFile(i);
		}

		// the files to carry to the exp folder
		copyToExpFolder(expFolder, hypFiles);
		for (String hypFile : hypFiles) {
			new File(hypFile).delete(); // we delete hyp files
		}
		copyToExpFolder(expFolder, outputFile);
		copyToExpFolder(expFolder, statisticsFile);
		copyToExpFolder(expFolder, dbFile);
		copyToExpFolder(expFolder, configFile);
		copyToExpFolder(expFolder, learnedModelFile);
		copyToExpFolder(expFolder, learnLog);
	}
	public static void copyToExpFolder(String expFolder, String... files) throws Exception {
		for (String file : files) {
			FileManager.copyFromTo(file, expFolder + new File(file).getName());
		}

	}
	/////////////////////////////////////////////////////////////////////////////
	//**************************************************************************
	private static void Copyperiviousphasefiles() throws IOException {
		File file = new File("Learninginfo-phase"+Integer.toString(learningphase));
        if (!file.exists()) {
            file.mkdir();
        }
        File sourceFolder = new File("output");
        File destinationFolder = new File("Learninginfo-phase"+Integer.toString(learningphase));
        copyFolder(sourceFolder, destinationFolder);
        sourceFolder = new File("input");
        copyFolder(sourceFolder, destinationFolder);
        
	}
    private static void copyFolder(File sourceFolder, File destinationFolder) throws IOException
    {
        if (sourceFolder.isDirectory()) 
        {
            if (!destinationFolder.exists()) 
            {
                destinationFolder.mkdir();
                System.out.println("Directory created :: " + destinationFolder);
            }
            String files[] = sourceFolder.list();
            for (String file : files) 
            {
                File srcFile = new File(sourceFolder, file);
                File destFile = new File(destinationFolder, file);
                copyFolder(srcFile, destFile);
            }
        }
        else
        { 
            Files.copy(sourceFolder.toPath(), destinationFolder.toPath(), StandardCopyOption.REPLACE_EXISTING);
            System.out.println("File copied :: " + destinationFolder);
        }
    }
    private static void Modifyconfigurationfile() {
    	  try {
    	        String searchText = "1";
    	        Path p = Paths.get(configFile);
    	        Path tempFile = Files.createTempFile(p.getParent(), "usersTemp", ".txt");
    	        try (BufferedReader reader = Files.newBufferedReader(p);
    	                BufferedWriter writer = Files.newBufferedWriter(tempFile)) {
    	            String line;

    	            // copy everything until the id is found
    	            while ((line = reader.readLine()) != null) {
    	            	if(!line.equals("")) {
    	                writer.write(line);
    	                writer.newLine();
    	            	}
    	            }
    	            for (String input : newinputs) {
    	            	writer.write(input+";\\");
    	            	 writer.newLine();
					}
    	            writer.close();
    	        }
    	        // copy new file & delete temporary file
//    	        File srcFile = new File(p.toAbsolutePath().toString());
//                File destFile = new File(tempFile.toAbsolutePath().toString());
//                copyFolder(destFile, srcFile);
    	        configFile=tempFile.toAbsolutePath().toString();
    	        
			/* Files.copy(tempFile, p, StandardCopyOption.REPLACE_EXISTING); */
    	        //Files.delete(tempFile);
    	    } catch (IOException ex) {
    	        //Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
    	    }
    }
    private static void Deleteoutputdir() {

		File index = new File(outDir);
		String[]entries = index.list();
		for(String s: entries){
		    File currentFile = new File(index.getPath(),s);
		    currentFile.delete();
		}
	}
}
