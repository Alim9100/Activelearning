package learner;

import java.io.BufferedReader;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.util.ArrayList;

public class SutWrapper {
	DataOutputStream sockout=null;
	DataInputStream sockin=null;
	private ArrayList<String> inputs;

	public SutWrapper(Socket sock) {
		try {
			// Create socket out (no buffering) and in 
			sockout = new DataOutputStream(sock.getOutputStream());
			sockin = new DataInputStream(sock.getInputStream());
			inputs=new ArrayList<String>();
		} 
		catch (IOException e) {
			e.printStackTrace();
		}
	}

	public String sendInput(String input) {
		try {	
			// Send input to SUT
			byte[] buf=input.getBytes();
			sockout.write(buf);
			inputs.add(input);
			//Read output from mapper
			byte[] message = new byte[1024];
		     sockin.read(message); // read the message
		     String str=new String(message,"UTF-8");
		     //System.out.println(str);
			return str;
		} 
		catch (IOException e) {
			e.printStackTrace();
			return null;
		}
	}
	
	public void reset() throws IOException {
		String str="reset";
		byte[] buf=str.getBytes();
		sockout.write(buf);
		// Check if reset succeeded. Note: this is also needed because not receiving after reset will immediately continue
		// to sending Input, allowing the possibility for the client to receive "reset INPUT" in one string. Reading in between
		// will force a break since reading is blocking.
		try {
			byte[] message = new byte[1024];
		     int k=sockin.read(message); // read the message
		     String line=new String(message,0,k);
		     String resetok=new String("resetok");

			if (!line.equals(resetok)) {
				throw new IOException("Reset did not succeed");
			}
		} catch (IOException e) {
			System.out.println("RESET NOT OK");
			e.printStackTrace();
			System.exit(0);
		}
		
		inputs = new ArrayList<String>();
	}
	public String Sendcompletelearning() throws IOException {
		String str="LEARNINGCOMPLETE";
		byte[] buf=str.getBytes();
		sockout.write(buf);
			byte[] message = new byte[1024];
		     int k=sockin.read(message); // read the message
		     String line=new String(message,0,k);
		     return line;

	}
}

