using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class SupportingDocRepository : Repository<SupportingDoc>, ISupportingDocRepository
	{


		public int InsertProposalSupportingDocs (IEnumerable<SupportingDoc> entities)
		{
			string query = "INSERT INTO SupportingDocs(ProposalID, ServerFileName, OrigFileName, attachedBy) VALUES (@ProposalID, @ServerFileName, @OrigFileName, @AttachedBy)";

			var rowsInserted = AddRange(entities, query);

			return rowsInserted;
		}


		public List<SupportingDoc> GetProposalSupportingDocs (int proposalID)
		{
			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};
			pgMain.Predicates.Add(Predicates.Field<SupportingDoc>(e => e.ProposalId, Operator.Eq, proposalID));
			pgMain.Predicates.Add(Predicates.Field<SupportingDoc>(e => e.IsDeleted, Operator.Eq, 0));

			List<SupportingDoc> results = new List<SupportingDoc>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<SupportingDoc>(pgMain).ToList();
				conn.Close();
			}
			return results;
		}

		public int DeleteProposalSupportingDoc (int fileID, int proposalID)
		{
			string query = "UPDATE SupportingDocs SET IsDeleted=1 WHERE id=@fileID AND ProposalID=@ProposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					fileID = fileID,
					ProposalID = proposalID
				});

				conn.Close();
			}

			return rowsUpdated;
		}
	}
}
