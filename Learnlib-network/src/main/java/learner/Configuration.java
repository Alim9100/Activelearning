package learner;
import java.io.BufferedReader;
///////////configuration of oracle is not set
import java.io.File;
import java.io.FileInputStream;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Properties;
import java.util.stream.Collectors;
import learner.Main;
import learner.Main.EquType;

public class Configuration {
	public static class Config {
		public final Integer maxNumTests;
		public final List<String> alphabet;
		public final List<String> testWords;
		public String specFile;
		public final String sutName;
		private String nusmvCmd;
		public final List<EquType> equOracleTypes;
        //config alphabet and oracle
		public Config(String file) throws IOException {
			Properties propFile = new Properties();
			if (!new File(file).exists())
				System.out.println("The property file " + propFile + " doesn't exist. Loading default configurations.");
			propFile.load(new FileInputStream(file));
			this.sutName = (String) propFile.getOrDefault("name", "aSut");
			String equOraclesString = (String) propFile.getOrDefault("eqOracle", "RANDOM");
			equOracleTypes = new ArrayList<EquType>();
			for (String equOracleStr : equOraclesString.split(";")) {
				EquType equOracle = EquType.valueOf(equOracleStr.trim().toUpperCase());
				if (equOracle == null)
					throw new RuntimeException(
							"Invalid equ oracle" + equOracleStr + ". Select from " + Arrays.toString(EquType.values()));
				equOracleTypes.add(equOracle);
			}
			String[] inputs = propFile.get("alphabet").toString().split(";");
			alphabet = Arrays.stream(inputs).map(str -> str.toUpperCase()).filter(str -> !str.startsWith("!"))
					.collect(Collectors.<String>toList());


			// efficient computing... not
			maxNumTests = Integer.valueOf((String) propFile.getOrDefault("maxNumTests", String.valueOf(Main.maxNumTraces)));
			testWords = new ArrayList<>();
			//////config eqoracle
			
			  if (equOracleTypes.contains(EquType.WORDS)) { // getting test words from old stats file
				  if (new File(Main.oldStatsFile).exists()) { 
				  BufferedReader reader= new BufferedReader(new FileReader(Main.oldStatsFile));
				  String str;
				  while ((str =reader.readLine()) != null) 
					  if (str.startsWith("Counter Example:"))
						  testWords.add(str.replace("Counter Example:", "").trim()); }
			  
			  } if (equOracleTypes.contains(EquType.CONFORMANCE)) {
				  String specFileName =requireGet(EquType.CONFORMANCE, "specification", propFile); 
				  specFile ="input/" + specFileName; nusmvCmd = requireGet(EquType.CONFORMANCE,"nusmvCmd", propFile); 
			  }
			 
		}

		public String toString() {
			return "Equ Oracles: " + this.equOracleTypes + "; MaxNumTests: " + maxNumTests;
		}

		public String requireGet(EquType type, String param, Properties propFile) {
			String paramVal = (String) propFile.getProperty(param);
			if (paramVal == null)
				throw new RuntimeException("Parameter " + param + " must be specified for Equ Oracle " + type);
			return paramVal;
		}
		
	}
}
