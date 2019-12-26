using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface ISupportingDocRepository : IRepository<SupportingDoc>
	{
		int InsertProposalSupportingDocs (IEnumerable<SupportingDoc> entities);

		int DeleteProposalSupportingDoc (int fileID, int proposalID);

		List<SupportingDoc> GetProposalSupportingDocs (int proposalID);
	}
}
