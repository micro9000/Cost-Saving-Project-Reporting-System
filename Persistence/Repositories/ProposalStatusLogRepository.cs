using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class ProposalStatusLogRepository : Repository<ProposalStatusLog>, IProposalStatusLogRepository
	{
		public List<ProposalStatusLog> GetAllStatusLogByProposal (int proposalID)
		{
			List<ProposalStatusLog> results = new List<ProposalStatusLog>();

			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};
			pgMain.Predicates.Add(Predicates.Field<ProposalImg>(e => e.ProposalId, Operator.Eq, proposalID));
			pgMain.Predicates.Add(Predicates.Field<ProposalImg>(e => e.IsDeleted, Operator.Eq, 0));

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<ProposalStatusLog>(pgMain).ToList();
				conn.Close();
			}

			return results;
		}
	}
}
