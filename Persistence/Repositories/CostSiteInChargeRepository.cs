using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class CostSiteInChargeRepository : Repository<CostSiteInCharge>, ICostSiteInChargeRepository
	{
		public CostSiteInCharge GetProposalCostSiteInCharge (int proposalID)
		{
			string sql = "SELECT * FROM CostSiteIncharges WHERE isDeleted=0 AND ProposalID = @ProposalID ORDER BY id DESC LIMIT 1";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var result = conn.QuerySingleOrDefault<CostSiteInCharge>(sql, new
				{
					ProposalID = proposalID
				});

				conn.Close();

				return result;
			}
		}

		public int UpdateProposalCostSiteInChargeVerification (CostSiteInCharge verification)
		{
			int rowsUpdated = 0;

			string query = @"UPDATE CostSiteIncharges 
							SET remarks=@remarks, ApprovalStatus=@ApprovalStatus, DateResponse=CURRENT_TIMESTAMP
							WHERE ProposalID=@proposalID AND Id=@Id AND FFID=@FFID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, verification);
			}

			return rowsUpdated;
		}
	}
}
