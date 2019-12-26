using System.Collections.Generic;
using Domain;


namespace Repositories
{
	public interface IProposalActionRepository : IRepository<ProposalAction>
	{
		List<ProposalAction> GetProposalActionsWithApprovers (int proposalID);

		List<ProposalAction> GetAllOpenActionItems ();

		ProposalAction GetProposalActionWithApprovers (int proposalID, int actionID);

		int UpdateProposalActionOwnerResponse (ProposalAction action);

	}
}
