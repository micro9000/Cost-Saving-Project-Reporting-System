using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface IProposalImgRepository : IRepository<ProposalImg>
	{
		int InsertProposalImgs (IEnumerable<ProposalImg> entities);

		int DeleteProposalImg (int imgId, int proposalID);

		List<ProposalImg> GetProposalImgs (int proposalID);
	}
}
