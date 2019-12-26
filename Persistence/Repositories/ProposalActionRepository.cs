using System.Collections.Generic;
using System;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;

namespace Persistence.Repositories
{
	public class ProposalActionRepository : Repository<ProposalAction>, IProposalActionRepository
	{



		public List<ProposalAction> GetProposalActionsWithApprovers (int proposalID)
		{
			List<ProposalAction> results = new List<ProposalAction>();

			string actionsQuery = "SELECT * FROM ProposalActions WHERE IsDeleted=0 AND ProposalID=@ProposalID";
			string actionApproversQuery = "SELECT * FROM ProposalActionsApprovers WHERE IsDeleted=0 AND ProposalID=@ProposalID AND ActionID=@ActionID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<ProposalAction>(actionsQuery, new
				{
					ProposalID = proposalID
				}).ToList();


				foreach (var action in results)
				{

					action.Approvers = conn.Query<ProposalActionApprover>(actionApproversQuery, new
					{
						ProposalID = proposalID,
						ActionID = action.Id
					}).ToList();

				}


				conn.Close();
			}

			return results;
		}


		public List<ProposalAction> GetAllOpenActionItems ()
		{
			List<ProposalAction> results = new List<ProposalAction>();

			//string actionsQuery = "SELECT * FROM ProposalActions WHERE IsDeleted=0 AND isClosed=0";
			string actionsQuery = @"SELECT PA.*
									FROM ProposalActions As PA
									LEFT JOIN Proposals As P ON PA.ProposalID=P.id
									WHERE P.isDeleted=0 AND PA.IsDeleted=0 AND PA.isClosed=0 AND P.OAStatus NOT IN @InvalidStatus";

			string actionApproversQuery = "SELECT * FROM ProposalActionsApprovers WHERE IsDeleted=0 AND ActionID=@ActionID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<ProposalAction>(actionsQuery, new
				{
					InvalidStatus = new int[] {
								(int)StaticData.OverallStatus.INVALID,
								(int)StaticData.OverallStatus.CANCELED,
								//(int)StaticData.OverallStatus.COST_AVOIDANCE,
								(int)StaticData.OverallStatus.EXISTING_PROJECT,
								(int)StaticData.OverallStatus.DUPLICATE_ENTRY
								}
				}).ToList();


				foreach (var action in results)
				{

					action.Approvers = conn.Query<ProposalActionApprover>(actionApproversQuery, new
					{
						ActionID = action.Id
					}).ToList();

				}


				conn.Close();
			}

			return results;
		}

		public ProposalAction GetProposalActionWithApprovers (int proposalID, int actionID)
		{

			ProposalAction results = new ProposalAction();

			string actionsQuery = "SELECT * FROM ProposalActions WHERE IsDeleted=0 AND ProposalID=@ProposalID AND id=@ActionID";
			string actionApproversQuery = "SELECT * FROM ProposalActionsApprovers WHERE IsDeleted=0 AND ProposalID=@ProposalID AND ActionID=@ActionID";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.QuerySingleOrDefault<ProposalAction>(actionsQuery, new
				{
					ProposalID = proposalID,
					ActionID = actionID
				});

				if (results != null)
				{
					results.Approvers = conn.Query<ProposalActionApprover>(actionApproversQuery, new
					{
						ProposalID = proposalID,
						ActionID = results.Id
					}).ToList();
				}


				conn.Close();
			}

			return results;


		}


		public int UpdateProposalActionOwnerResponse (ProposalAction action)
		{
			string query = @"UPDATE ProposalActions SET isClosed=@isClosed, OwnerRemarks=@OwnerRemarks, 
										DateResponse=CURRENT_TIMESTAMP 
								WHERE id=@ActionID AND ProposalID=@ProposalID AND OwnerFFID=@OwnerFFID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					isClosed = action.IsClosed,
					OwnerRemarks = action.OwnerRemarks,
					ActionID = action.Id,
					ProposalID = action.ProposalID,
					OwnerFFID = action.OwnerFFID
				});

				conn.Close();

			}

			return rowsUpdated;

		}

	}
}
