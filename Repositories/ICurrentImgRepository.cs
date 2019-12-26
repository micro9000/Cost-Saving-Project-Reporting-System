using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface ICurrentImgRepository : IRepository<CurrentImg>
	{
		int InsertProposalCurrentImgs (IEnumerable<CurrentImg> entities);

		int DeleteProposalCurrentImg (int imgID, int proposalID);

		List<CurrentImg> GetProposalCurrentImgs (int proposalID);
	}
}
