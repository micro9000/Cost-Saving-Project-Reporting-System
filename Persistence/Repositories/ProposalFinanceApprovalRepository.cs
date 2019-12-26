using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class ProposalFinanceApprovalRepository : Repository<ProposalFinanceApproval>, IProposalFinanceApprovalRepository
	{
		public List<ProposalFinanceApproval> GetProposalFinanceInfoAndVerificationInfo (int proposalID)
		{
			string query = @"SELECT PFA.*, FA.*
							FROM ProposalFinanceApprovals AS PFA
							JOIN FinanceApprovers AS FA ON FA.Id=PFA.FinanceID
							WHERE PFA.isDeleted=0 AND FA.IsDeleted=0 AND PFA.ProposalID=@ProposalID";

			List<ProposalFinanceApproval> results = new List<ProposalFinanceApproval>();

			var p = new
			{
				ProposalID = proposalID
			};

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<ProposalFinanceApproval, FinanceApprover, ProposalFinanceApproval>(query,
					(PFA, FA) =>
					{
						PFA.FinanceApproverInfo = FA;
						return PFA;
					}
					, p).ToList();

				conn.Close();

			}
			return results;
		}

		public ProposalFinanceApproval GetProposalFinanceInfoVerificationResults (int proposalID, int financeID)
		{
			string sql = "SELECT * FROM ProposalFinanceApprovals WHERE isDeleted=0 AND ProposalID=@ProposalID AND FinanceID=@FinanceID";

			var results = new ProposalFinanceApproval();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.QuerySingleOrDefault<ProposalFinanceApproval>(sql, new
				{
					ProposalID = proposalID,
					FinanceID = financeID
				});

				conn.Close();
			}

			return results;
		}

		public int UpdateProposalFinanceInfoVerification (int proposalID, int financeID, string remarks, int isVerify)
		{
			int rowsUpdated = 0;

			string query = @"UPDATE ProposalFinanceApprovals 
							SET remarks=@remarks, isVerify=@isVerify, verifyDate=CURRENT_TIMESTAMP
							WHERE ProposalID=@proposalID AND FinanceID=@FinanceID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					remarks = remarks,
					isVerify = isVerify,
					proposalID = proposalID,
					FinanceID = financeID
				});
			}

			return rowsUpdated;
		}

		public int ReassignProposalFinanceApproval (int proposalID, int newfinanceID, int lastFinanceID)
		{
			int rowsUpdated = 0;

			string query = @"UPDATE ProposalFinanceApprovals 
							SET remarks='na', isVerify=0, verifyDate='2000-01-01 01:01:01', FinanceID=@FinanceID
							WHERE ProposalID=@proposalID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					proposalID = proposalID,
					FinanceID = newfinanceID
				});
			}

			return rowsUpdated;
		}


		public int DeleteProposalFinance (int proposalID, int financeID, string remarks)
		{
			int rowsUpdated = 0;

			string query = @"UPDATE ProposalFinanceApprovals 
							SET isDeleted=1, remarks=@remarks, verifyDate=CURRENT_TIMESTAMP
							WHERE ProposalID=@proposalID AND FinanceID=@financeID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					proposalID = proposalID,
					FinanceID = financeID,
					remarks = remarks
				});
			}

			return rowsUpdated;
		}

	}
}
