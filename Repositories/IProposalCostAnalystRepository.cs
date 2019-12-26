using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface IProposalCostAnalystRepository : IRepository<ProposalCostAnalyst>
	{
		List<ProposalCostAnalyst> GetProposalCostAnalystInfoAndVerificationInfo (int proposalID);

		ProposalCostAnalyst GetProposalCostAnalystVerificationResults (int proposalID, int costAnalystID);

		int UpdateProposalCostAnalystVerification (int proposalID, int costAnalystID, string remarks, int isVerify);

		int DeleteProposalCostAnalyst (int proposalID, int costAnalystID, string remarks);
	}
}
