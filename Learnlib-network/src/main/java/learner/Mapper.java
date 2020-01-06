package learner;

import java.net.Socket;
import java.util.Arrays;
import java.util.List;
import java.util.Optional;

import de.ls5.jlearn.interfaces.Alphabet;
import de.ls5.jlearn.shared.AlphabetImpl;
import de.ls5.jlearn.shared.SymbolImpl;

public class Mapper extends WordProcessor {
	private static final long serialVersionUID = 16164788928002812L;
	public static List<String> fullAlphabet = Arrays.asList(
			new String [] { 
					"DNS",
				    "CONNECT",
				     "SENDMSG",
				     "SENDFILE",
				     } );
	
	private MembershipOracle memberOracle;

	public Mapper(Socket sock, Logger sqllog) {
		super(sock, sqllog, true);  // This mapper should retrieve from cache, so set true.
	}
	
	
	
	public void setMembershipOracle(MembershipOracle memberOracle) {
		// Sets the membership oracle on this object
		this.memberOracle = memberOracle;
	}
	
	public MembershipOracle getMembershipOracle() {
		// Retrieves this object's membership oracle
		return memberOracle;
	}

	
	public Alphabet generateInputAlphabet(List<String> suppliedInputStrings) {
//		Optional<String> hasInvInput = suppliedInputStrings.stream().filter(str -> !fullAlphabet.contains(str)).findAny();
//		if (hasInvInput.isPresent()) {
//			System.out.println("Input " + hasInvInput.get() + " not in full alphabet");
//			System.exit(0);
//		}
		Alphabet result = new AlphabetImpl();
		suppliedInputStrings.forEach(action -> 
		result.addSymbol(new SymbolImpl(action)));
		return result;
	}
}

