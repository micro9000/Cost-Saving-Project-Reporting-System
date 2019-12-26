using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class ProposalActionApproverRepository : Repository<ProposalActionApprover>, IProposalActionApproverRepository
	{
		public int UpdateActionsApproverVerification (ProposalActionApprover verification)
		{
			string query = @"UPDATE ProposalActionsApprovers SET Remarks=@Remarks, 
								ApprovalStatus=@ApprovalStatus, DateResponse=CURRENT_TIMESTAMP
							WHERE ProposalID=@ProposalID AND ActionID=@ActionID AND ApproverFFID=@ApproverFFID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					ProposalID = verification.ProposalID,
					ActionID = verification.ActionID,
					ApproverFFID = verification.ApproverFFID,
					Remarks = verification.Remarks,
					ApprovalStatus = verification.ApprovalStatus
				});

				conn.Close();

			}


			return rowsUpdated;
		}


		public List<ProposalActionApprover> GetProposalActionApprovers (int proposalID)
		{
			List<ProposalActionApprover> results = new List<ProposalActionApprover>();
			string query = "SELECT * FROM ProposalActionsApprovers WHERE IsDeleted=0 AND ProposalID=@ProposalID";


			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<ProposalActionApprover>(query, new
				{
					ProposalID = proposalID
				}).ToList();

				conn.Close();
			}

			return results;
		}

	}
}
