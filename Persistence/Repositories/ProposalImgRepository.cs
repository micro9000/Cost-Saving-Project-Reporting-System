using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class ProposalImgRepository : Repository<ProposalImg>, IProposalImgRepository
	{


		public int InsertProposalImgs (IEnumerable<ProposalImg> entities)
		{
			string query = "INSERT INTO ProposalImgs(ProposalID, ServerFileName, OrigFileName) VALUES (@ProposalID, @ServerFileName, @OrigFileName)";

			var rowsInserted = AddRange(entities, query);

			return rowsInserted;
		}

		public List<ProposalImg> GetProposalImgs (int proposalID)
		{

			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};
			pgMain.Predicates.Add(Predicates.Field<ProposalImg>(e => e.ProposalId, Operator.Eq, proposalID));
			pgMain.Predicates.Add(Predicates.Field<ProposalImg>(e => e.IsDeleted, Operator.Eq, 0));

			List<ProposalImg> results = new List<ProposalImg>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<ProposalImg>(pgMain).ToList();
				conn.Close();
			}

			return results;
		}

		public int DeleteProposalImg (int imgID, int proposalID)
		{
			string query = "UPDATE ProposalImgs SET IsDeleted=1 WHERE id=@ImgID AND ProposalID=@ProposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					ImgID = imgID,
					ProposalID = proposalID
				});

				conn.Close();
			}

			return rowsUpdated;
		}
	}
}
