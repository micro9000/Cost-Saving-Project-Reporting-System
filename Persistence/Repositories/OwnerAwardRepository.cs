using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class OwnerAwardRepository : Repository<OwnerAward>, IOwnerAwardRepository
	{

		public List<OwnerAward> GetAllAwardsByUser (string userFFID)
		{
			List<OwnerAward> results = new List<OwnerAward>();

			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};
			pgMain.Predicates.Add(Predicates.Field<OwnerAward>(e => e.UserFFID, Operator.Eq, userFFID));
			pgMain.Predicates.Add(Predicates.Field<OwnerAward>(e => e.IsDeleted, Operator.Eq, 0));

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<OwnerAward>(pgMain).ToList();
				conn.Close();
			}

			return results;
		}

		public List<OwnerAward> GetAllAwardsByProject (int proposalID)
		{
			List<OwnerAward> results = new List<OwnerAward>();

			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};
			pgMain.Predicates.Add(Predicates.Field<OwnerAward>(e => e.ProposalID, Operator.Eq, proposalID));
			pgMain.Predicates.Add(Predicates.Field<OwnerAward>(e => e.IsDeleted, Operator.Eq, 0));

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<OwnerAward>(pgMain).ToList();
				conn.Close();
			}

			return results;
		}
	}
}
