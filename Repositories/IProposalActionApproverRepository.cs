using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface IProposalActionApproverRepository : IRepository<ProposalActionApprover>
	{
		int UpdateActionsApproverVerification (ProposalActionApprover verification);

		List<ProposalActionApprover> GetProposalActionApprovers (int proposalID);
	}
}
