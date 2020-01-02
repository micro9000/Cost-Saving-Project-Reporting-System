using System.Collections.Generic;
using Domain;
namespace Repositories
{
	public interface IProposalStatusLogRepository : IRepository<ProposalStatusLog>
	{
		List<ProposalStatusLog> GetAllStatusLogByProposal (int proposalID);
	}
}
