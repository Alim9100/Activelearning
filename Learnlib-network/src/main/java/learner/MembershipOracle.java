package learner;

import java.net.Socket;

public class MembershipOracle extends WordProcessor {
	private static final long serialVersionUID = 9079445565706621341L;
	
	public MembershipOracle(Socket sock, Logger sqllog) {
		super(sock, sqllog, false);  // This oracle should not retrieve from cache, so set false.
	}
}
