using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class CurrentImgRepository : Repository<CurrentImg>, ICurrentImgRepository
	{

		public int InsertProposalCurrentImgs (IEnumerable<CurrentImg> entities)
		{
			string query = "INSERT INTO CurrentImgs(ProposalID, ServerFileName, OrigFileName) VALUES (@ProposalID, @ServerFileName, @OrigFileName)";

			var rowsInserted = AddRange(entities, query);

			return rowsInserted;
		}

		public List<CurrentImg> GetProposalCurrentImgs (int proposalID)
		{
			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};
			pgMain.Predicates.Add(Predicates.Field<CurrentImg>(e => e.ProposalId, Operator.Eq, proposalID));
			pgMain.Predicates.Add(Predicates.Field<CurrentImg>(e => e.IsDeleted, Operator.Eq, 0));

			List<CurrentImg> results = new List<CurrentImg>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<CurrentImg>(pgMain).ToList();
				conn.Close();
			}
			return results;

		}

		public int DeleteProposalCurrentImg (int imgID, int proposalID)
		{
			string query = "UPDATE CurrentImgs SET IsDeleted=1 WHERE id=@ImgID AND ProposalID=@ProposalID";

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
