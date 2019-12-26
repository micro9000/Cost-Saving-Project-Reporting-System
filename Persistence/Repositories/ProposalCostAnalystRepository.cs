using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class ProposalCostAnalystRepository : Repository<ProposalCostAnalyst>, IProposalCostAnalystRepository
	{

		public List<ProposalCostAnalyst> GetProposalCostAnalystInfoAndVerificationInfo (int proposalID)
		{
			string query = @"SELECT PCA.*, CA.*
							FROM ProposalCostAnalysts AS PCA
							JOIN CostAnalysts AS CA ON CA.Id=PCA.CostAnalystID
							WHERE PCA.isDeleted=0 AND CA.IsDeleted=0 AND PCA.ProposalID=@ProposalID";

			List<ProposalCostAnalyst> results = new List<ProposalCostAnalyst>();

			var p = new
			{
				ProposalID = proposalID
			};

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<ProposalCostAnalyst, CostAnalyst, ProposalCostAnalyst>(query,
					(PCA, CA) =>
					{
						PCA.CostAnalystInfo = CA;
						return PCA;
					}
					, p).ToList();

				conn.Close();

			}
			return results;
		}

		public ProposalCostAnalyst GetProposalCostAnalystVerificationResults (int proposalID, int costAnalystID)
		{
			string sql = "SELECT * FROM ProposalCostAnalysts WHERE isDeleted=0 AND ProposalID=@ProposalID AND CostAnalystID=@CostAnalystID";

			var results = new ProposalCostAnalyst();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.QuerySingleOrDefault<ProposalCostAnalyst>(sql, new
				{
					ProposalID = proposalID,
					CostAnalystID = costAnalystID
				});

				conn.Close();
			}

			return results;
		}


		public int UpdateProposalCostAnalystVerification (int proposalID, int costAnalystID, string remarks, int isVerify)
		{
			int rowsUpdated = 0;

			string query = @"UPDATE ProposalCostAnalysts 
							SET remarks=@remarks, isVerify=@isVerify, verifyDate=CURRENT_TIMESTAMP
							WHERE ProposalID=@proposalID AND CostAnalystID=@costAnalystID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					remarks = remarks,
					isVerify = isVerify,
					proposalID = proposalID,
					costAnalystID = costAnalystID
				});
			}

			return rowsUpdated;
		}


		public int DeleteProposalCostAnalyst (int proposalID, int costAnalystID, string remarks)
		{
			int rowsUpdated = 0;

			string query = @"UPDATE ProposalCostAnalysts 
							SET isDeleted=1, remarks=@remarks, verifyDate=CURRENT_TIMESTAMP
							WHERE ProposalID=@proposalID AND CostAnalystID=@costAnalystID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					remarks = remarks,
					proposalID = proposalID,
					costAnalystID = costAnalystID
				});
			}

			return rowsUpdated;
		}
	}
}
