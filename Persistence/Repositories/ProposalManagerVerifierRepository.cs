using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class ProposalManagerVerifierRepository : Repository<ProposalManagerVerifier>, IProposalManagerVerifierRepository
	{
		public IList<ProposalManagerVerifier> GetProposalManagerVerifiers (int proposalID)
		{

			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};

			pgMain.Predicates.Add(Predicates.Field<ProposalManagerVerifier>(e => e.ProposalID, Operator.Eq, proposalID));
			pgMain.Predicates.Add(Predicates.Field<ProposalManagerVerifier>(e => e.IsDeleted, Operator.Eq, 0));


			List<ProposalManagerVerifier> results = new List<ProposalManagerVerifier>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<ProposalManagerVerifier>(pgMain).ToList();
				conn.Close();
			}

			return results;
		}


		public ProposalManagerVerifier GetProposalManagerVerificationResults (int proposalID, string manFFID)
		{
			string sql = "SELECT * FROM ProposalManagerVerifiers WHERE isDeleted=0 AND ProposalID=@ProposalID AND manFFID=@manFFID";

			var results = new ProposalManagerVerifier();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.QuerySingleOrDefault<ProposalManagerVerifier>(sql, new
				{
					ProposalID = proposalID,
					manFFID = manFFID
				});

				conn.Close();
			}

			return results;
		}

		public int UpdateProposalManagerVerification (int proposalID, string manFFID, string remarks, int isVerify)
		{
			int rowsUpdated = 0;

			string query = @"UPDATE ProposalManagerVerifiers 
							SET remarks=@remarks, isVerify=@isVerify, verifyDate=CURRENT_TIMESTAMP
							WHERE ProposalID=@proposalID AND manFFID=@manFFID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					remarks = remarks,
					isVerify = isVerify,
					proposalID = proposalID,
					manFFID = manFFID
				});
			}

			return rowsUpdated;

		}
	}
}
