using System.Collections.Generic;
using Domain;


namespace Repositories
{
	public interface IProposalManagerVerifierRepository : IRepository<ProposalManagerVerifier>
	{
		IList<ProposalManagerVerifier> GetProposalManagerVerifiers (int proposalID);

		ProposalManagerVerifier GetProposalManagerVerificationResults (int proposalID, string manFFID);

		int UpdateProposalManagerVerification (int proposalID, string manFFID, string remarks, int isVerify);

	}
}
