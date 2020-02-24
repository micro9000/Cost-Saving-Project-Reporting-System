using System.Collections.Generic;
using Repositories;
using Domain;
using Dapper;
using DapperExtensions;
using System.Linq;
using System;
using System.Globalization;

namespace Persistence.Repositories
{
	public class ProposalRepository : Repository<Proposal>, IProposalRepository
	{

		/*
		 * 
		 * 
		 * 
		 * Kindly use this manual Predicates
		 * https: //github.com/tmsmith/Dapper-Extensions/wiki/Predicates
		 * 
		 * 
		 * */

		public Proposal GetProposalDetailsByID (int proposalID)
		{
			//Proposal proposalDetails = new Proposal();

			//proposalDetails = this.Get(proposalID);

			//proposalDetails = (proposalDetails.IsDeleted == 0) ? proposalDetails : new Proposal();

			//return proposalDetails;

			string query = "SELECT * FROM Proposals WHERE IsDeleted=0 AND id=@Id";

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				var result = conn.QuerySingleOrDefault<Proposal>(query, new
				{
					Id = proposalID
				});
				conn.Close();

				return result;
			}

		}


		public List<Proposal> GetProposalByStatus (int status)
		{
			// If you have a question on this syntax, please refer to Predicate manual --> see the link at the top of this class
			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};
			pgMain.Predicates.Add(Predicates.Field<Proposal>(p => p.OAStatus, Operator.Eq, status));
			pgMain.Predicates.Add(Predicates.Field<Proposal>(p => p.IsDeleted, Operator.Eq, 0));

			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<Proposal>(pgMain).OrderByDescending(p => p.Id).ToList();
				conn.Close();
			}
			return results;
		}


		public List<Proposal> GetArchivedProposalByUser (string userFFID)
		{
			// If you have a question on this syntax, please refer to Predicate manual --> see the link at the top of this class
			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};

			pgMain.Predicates.Add(Predicates.Field<Proposal>(p => p.EmpFFID, Operator.Eq, userFFID));
			pgMain.Predicates.Add(Predicates.Field<Proposal>(p => p.IsDeleted, Operator.Eq, 0));

			var pgNested = new PredicateGroup
			{
				Operator = GroupOperator.Or,
				Predicates = new List<IPredicate>()
			};

			pgNested.Predicates.Add(Predicates.Field<Proposal>(p => p.OAStatus, Operator.Eq, (int)StaticData.OverallStatus.COST_FUNNEL_EVALUATING));
			pgNested.Predicates.Add(Predicates.Field<Proposal>(p => p.OAStatus, Operator.Eq, (int)StaticData.OverallStatus.INVALID));
			//pgNested.Predicates.Add(Predicates.Field<Proposal>(p => p.OAStatus, Operator.Eq, (int)StaticData.OverallStatus.COST_AVOIDANCE));
			pgNested.Predicates.Add(Predicates.Field<Proposal>(p => p.OAStatus, Operator.Eq, (int)StaticData.OverallStatus.EXISTING_PROJECT));
			pgNested.Predicates.Add(Predicates.Field<Proposal>(p => p.OAStatus, Operator.Eq, (int)StaticData.OverallStatus.DUPLICATE_ENTRY));
			pgNested.Predicates.Add(Predicates.Field<Proposal>(p => p.OAStatus, Operator.Eq, (int)StaticData.OverallStatus.REALIZATION));

			pgMain.Predicates.Add(pgNested);

			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<Proposal>(pgMain).OrderByDescending(p => p.Id).ToList();
				conn.Close();
			}
			return results;
		}

		public List<Proposal> SearchProposal (Proposal proposal)
		{
			// If you have a question on this syntax, please refer to Predicate manual --> see the link at the top of this class
			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};

			var pga = new PredicateGroup
			{
				Operator = GroupOperator.Or,
				Predicates = new List<IPredicate>()
			};

			if (!string.IsNullOrEmpty(proposal.ProposalTicket))
			{
				pga.Predicates.Add(Predicates.Field<Proposal>(f => f.ProposalTicket, Operator.Like, "%" + proposal.ProposalTicket + "%"));
			}

			if (!string.IsNullOrEmpty(proposal.ProjectTitle))
			{
				pga.Predicates.Add(Predicates.Field<Proposal>(f => f.ProjectTitle, Operator.Like, "%" + proposal.ProjectTitle + "%"));
			}

			if (!string.IsNullOrEmpty(proposal.CurrentDescription))
			{
				pga.Predicates.Add(Predicates.Field<Proposal>(f => f.CurrentDescription, Operator.Like, "%" + proposal.CurrentDescription + "%"));
			}

			if (!string.IsNullOrEmpty(proposal.ProposalDescription))
			{
				pga.Predicates.Add(Predicates.Field<Proposal>(f => f.ProposalDescription, Operator.Like, "%" + proposal.ProposalDescription + "%"));
			}

			if (!string.IsNullOrEmpty(proposal.Remarks))
			{
				pga.Predicates.Add(Predicates.Field<Proposal>(f => f.Remarks, Operator.Like, "%" + proposal.Remarks + "%"));
			}


			if (!string.IsNullOrEmpty(proposal.SubmittedBy))
			{
				pga.Predicates.Add(Predicates.Field<Proposal>(f => f.SubmittedBy, Operator.Like, "%" + proposal.SubmittedBy + "%"));
			}

			if (!string.IsNullOrEmpty(proposal.EmpDeptCode))
			{
				pga.Predicates.Add(Predicates.Field<Proposal>(f => f.EmpDeptCode, Operator.Like, "%" + proposal.EmpDeptCode + "%"));
			}

			if (proposal.OAStatus >= 0)
			{
				pga.Predicates.Add(Predicates.Field<Proposal>(f => f.OAStatus, Operator.Eq, proposal.OAStatus));
			}

			if (proposal.IsBPI == 1)
			{
				pga.Predicates.Add(Predicates.Field<Proposal>(f => f.IsBPI, Operator.Eq, proposal.IsBPI));
			}

			pgMain.Predicates.Add(pga);

			pgMain.Predicates.Add(Predicates.Field<Proposal>(f => f.IsDeleted, Operator.Eq, 0));


			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<Proposal>(pgMain).OrderByDescending(p => p.Id).ToList();

				conn.Close();
			}

			return results;
		}


		public List<Proposal> SearchProposal (Proposal proposal, string startDate, string endDate)
		{
			string query = @"SELECT * FROM Proposals 
							WHERE isDeleted=0 AND (ProposalTicket LIKE '%@ProposalTicket%' OR 
									ProjectTitle LIKE '%@ProjectTitle%' OR 
									CurrentDescription LIKE '%@CurrentDescription%' OR 
									ProposalDescription LIKE '%@ProposalDescription%' OR
									remarks LIKE '%@Remarks%' OR OAStatus=@OAStatus OR
									submittedBy LIKE '%SubmittedBy%' OR EmpDeptCode=@EmpDeptCode) AND submittedDate BETWEEN @StartDate AND @EndDate ";

			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<Proposal>(query, new
				{
					ProposalTicket = proposal.ProposalTicket,
					ProjectTitle = proposal.ProjectTitle,
					CurrentDescription = proposal.CurrentDescription,
					ProposalDescription = proposal.ProposalDescription,
					Remarks = proposal.Remarks,
					OAStatus = proposal.OAStatus,
					SubmittedBy = proposal.SubmittedBy,
					EmpDeptCode = proposal.EmpDeptCode,
					StartDate = startDate,
					EndDate = endDate
				}).OrderByDescending(p => p.Id).ToList();

				conn.Close();
			}
			return results;
		}


		public List<Proposal> SearchProposalByKeywordAndOrStatusAndOrDepts (string projectType, string keywordStr, string startDate = "", string endDate = "", string[] statusList = null, string[] deptList = null, int isBPI = 0)
		{
			// If you have a question on this syntax, please refer to Predicate manual --> see the link at the top of this class
			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};


			var pga = new PredicateGroup
			{
				Operator = GroupOperator.Or,
				Predicates = new List<IPredicate>()
			};

			pga.Predicates.Add(Predicates.Field<Proposal>(f => f.ProposalTicket, Operator.Like, "%" + keywordStr + "%"));
			pga.Predicates.Add(Predicates.Field<Proposal>(f => f.ProjectTitle, Operator.Like, "%" + keywordStr + "%"));
			pga.Predicates.Add(Predicates.Field<Proposal>(f => f.CurrentDescription, Operator.Like, "%" + keywordStr + "%"));
			pga.Predicates.Add(Predicates.Field<Proposal>(f => f.ProposalDescription, Operator.Like, "%" + keywordStr + "%"));
			pga.Predicates.Add(Predicates.Field<Proposal>(f => f.Remarks, Operator.Like, "%" + keywordStr + "%"));
			pga.Predicates.Add(Predicates.Field<Proposal>(f => f.SubmittedBy, Operator.Like, "%" + keywordStr + "%"));
			pga.Predicates.Add(Predicates.Field<Proposal>(f => f.EmpDeptCode, Operator.Like, "%" + keywordStr + "%"));

			pgMain.Predicates.Add(pga);

			if (isBPI == 1)
			{
				pgMain.Predicates.Add(Predicates.Field<Proposal>(f => f.IsBPI, Operator.Eq, 1));
			}

			if (projectType != "ALL")
			{
				int projectTypeInt = 0;
				if (int.TryParse(projectType, out projectTypeInt))
				{
					pgMain.Predicates.Add(Predicates.Field<Proposal>(f => f.ProjectType, Operator.Eq, projectTypeInt));
				}
			}


			if (startDate != "" && endDate != "")
			{
				DateTime startingDate, endingDate;
				DateTime.TryParse(startDate, out startingDate);
				DateTime.TryParse(endDate, out endingDate);

				pgMain.Predicates.Add(Predicates.Field<Proposal>(f => f.SubmittedDate, Operator.Ge, startingDate));
				pgMain.Predicates.Add(Predicates.Field<Proposal>(f => f.SubmittedDate, Operator.Le, endingDate));
			}


			if (statusList != null)
			{
				if (statusList.Length > 0)
				{
					var pgb = new PredicateGroup
					{
						Operator = GroupOperator.Or,
						Predicates = new List<IPredicate>()
					};

					if (statusList[0] == "0") // means all
					{
						foreach (int status in StaticData.GetOverallStatusStringList())
						{
							pgb.Predicates.Add(Predicates.Field<Proposal>(f => f.OAStatus, Operator.Eq, status));
						}
					}
					else
					{
						foreach (string status in statusList)
						{
							if (status != "")
								pgb.Predicates.Add(Predicates.Field<Proposal>(f => f.OAStatus, Operator.Eq, status));
						}
					}
					pgMain.Predicates.Add(pgb);


				}
			}

			if (deptList != null)
			{
				if (deptList.Length > 0)
				{
					var pgc = new PredicateGroup
					{
						Operator = GroupOperator.Or,
						Predicates = new List<IPredicate>()
					};

					foreach (string deptCode in deptList)
					{
						if (deptCode != "")
							pgc.Predicates.Add(Predicates.Field<Proposal>(f => f.EmpDeptCode, Operator.Eq, deptCode));
					}
					pgMain.Predicates.Add(pgc);
				}
			}

			pgMain.Predicates.Add(Predicates.Field<Proposal>(f => f.IsDeleted, Operator.Eq, 0));


			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<Proposal>(pgMain).OrderByDescending(p => p.Id).ToList();

				conn.Close();
			}

			return results;
		}



		public List<Proposal> GetAllProposals ()
		{
			// If you have a question on this syntax, please refer to Predicate manual --> see the link at the top of this class
			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};

			pgMain.Predicates.Add(Predicates.Field<Proposal>(f => f.IsDeleted, Operator.Eq, 0));


			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<Proposal>(pgMain).OrderByDescending(p => p.Id).ToList();

				conn.Close();
			}

			return results;
		}


		public List<Proposal> GetAllBPIProposals ()
		{
			// If you have a question on this syntax, please refer to Predicate manual --> see the link at the top of this class
			var pgMain = new PredicateGroup
			{
				Operator = GroupOperator.And,
				Predicates = new List<IPredicate>()
			};

			pgMain.Predicates.Add(Predicates.Field<Proposal>(f => f.IsDeleted, Operator.Eq, 0));
			pgMain.Predicates.Add(Predicates.Field<Proposal>(f => f.IsBPI, Operator.Eq, 1));

			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.GetList<Proposal>(pgMain).OrderByDescending(p => p.Id).ToList();

				conn.Close();
			}

			return results;
		}


		//		public List<Proposal> SearchProposalByKeywordAndOrStatusAndOrDeptsWithActionItems (string keywordStr, string startDate = "", string endDate = "", string[] statusList = null, string[] deptList = null)
		//		{
		//			string query = @"SELECT *
		//							FROM Proposals AS P
		//							JOIN ProposalActions AS PA ON P.id = PA.ProposalID
		//							WHERE P.OAStatus <> @InvalidStatus AND P.isDeleted=0 AND PA.isDeleted=0 ";

		//			if (keywordStr != "")
		//			{
		//				query += @"AND (ProposalTicket=@searchStr OR ProjectTitle=@searchStr OR CurrentDescription=@searchStr
		//							OR ProposalDescription=@searchStr OR Remarks=@searchStr OR Remarks=@searchStr OR SubmittedBy=@searchStr
		//							OR EmpDeptCode=@searchStr OR empFFID=@searchStr) ";
		//			}

		//			if (startDate != "" && endDate != "")
		//			{
		//				query += "AND submittedDate BETWEEN @startDate AND @endDate";
		//			}


		//			string[] 

		//			if (statusList != null)
		//			{
		//				if (statusList.Length > 0)
		//				{
		//					var pgb = new PredicateGroup
		//					{
		//						Operator = GroupOperator.Or,
		//						Predicates = new List<IPredicate>()
		//					};

		//					if (statusList[0] == "0") // means all
		//					{
		//						foreach (int status in StaticData.GetOverallStatusStringArray())
		//						{
		//							pgb.Predicates.Add(Predicates.Field<Proposal>(f => f.OAStatus, Operator.Eq, status));
		//						}
		//					}
		//					else
		//					{
		//						foreach (string status in statusList)
		//						{
		//							if (status != "")
		//								pgb.Predicates.Add(Predicates.Field<Proposal>(f => f.OAStatus, Operator.Eq, status));
		//						}
		//					}
		//					pgMain.Predicates.Add(pgb);


		//				}
		//			}

		//			List<Proposal> results = new List<Proposal>();

		//			var p = new
		//			{
		//				InvalidStatus = (int)StaticData.OverallStatus.INVALID,
		//				OwnerFFID = ownerFFID
		//			};


		//			using (var conn = new WrappedDbConnection(GetOpenConnection()))
		//			{
		//				results = conn.Query<Proposal, ProposalAction, Proposal>(query,
		//						(P, PA) =>
		//						{
		//							P.NeededActions = new List<ProposalAction>();
		//							P.NeededActions.Add(PA);
		//							return P;
		//						}, p).ToList();

		//				conn.Close();

		//			}
		//			return results;
		//		}

		//public Proposal GetProposalByTicket (string ticket)
		//{
		//	var pg = new PredicateGroup { Operator = GroupOperator.Or, Predicates = new List<IPredicate>() };
		//	pg.Predicates.Add(Predicates.Field<Proposal>(f => f.ProposalTicket, Operator.Like, ticket));
		//	pg.Predicates.Add(Predicates.Field<Proposal>(f => f.IsDeleted, Operator.Eq, 0));

		//	using (var conn = new WrappedDbConnection(GetOpenConnection()))
		//	{
		//		var results = conn.Get<Proposal>(pg);
		//		return results;
		//	}
		//}

		public List<Proposal> GetProposalByDateSubmittedRange (string startDate, string endDate)
		{
			string query = "SELECT * FROM Proposals WHERE isDeleted=0 AND submittedDate BETWEEN @StartDate AND @EndDate";

			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<Proposal>(query, new
				{
					StartDate = startDate,
					EndDate = endDate
				}).OrderByDescending(p => p.Id).ToList();

				conn.Close();

			}
			return results;
		}


		public List<ProposalByOAStatus> GetProposalNumberOfSubmissionsByStatus ()
		{
			string query = "SELECT OAStatus, COUNT(Id) as Submissions FROM Proposals WHERE isDeleted=0 GROUP BY OAStatus";

			List<ProposalByOAStatus> results = new List<ProposalByOAStatus>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<ProposalByOAStatus>(query).ToList();

				conn.Close();

			}
			return results;
		}

		public List<Proposal> GetAssignedProjectsByUser (string userFFID)
		{
			string query = @"SELECT * FROM Proposals 
							WHERE isDeleted=0 AND projectOwnerFFID=@userFFID AND OAStatus IN @status";

			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<Proposal>(query, new
				{
					status = new int[] { (int)StaticData.OverallStatus.PROJECT_PROPOSAL,
										(int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS,
										(int)StaticData.OverallStatus.COST_FUNNEL_IDENTIFIED,
										(int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS
										},
					userFFID = userFFID,
				}).OrderByDescending(p => p.Id).ToList();

				conn.Close();

			}
			return results;
		}

		public List<Proposal> GetActiveProposalsByUser (string userFFID)
		{
			string query = @"SELECT * FROM Proposals 
							WHERE isDeleted=0 AND empFFID=@userFFID AND OAStatus IN @status";

			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<Proposal>(query, new
				{
					status = new int[] { (int)StaticData.OverallStatus.PROJECT_PROPOSAL,
										(int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS,
										(int)StaticData.OverallStatus.COST_FUNNEL_IDENTIFIED,
										(int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS
										},
					userFFID = userFFID,
				}).OrderByDescending(p => p.Id).ToList();

				conn.Close();

			}
			return results;
		}


		public List<Proposal> GetProposalByCostAnalystApprovalPending (int costAnalystID = 0)
		{
			string query = @"SELECT P.*
							FROM Proposals As P
							JOIN ProposalCostAnalysts As PCA ON P.id = PCA.ProposalID
							WHERE P.isDeleted=0 AND PCA.isDeleted=0 AND
								(PCA.isVerify=0 OR PCA.isVerify=3) AND 
								P.OAStatus IN @status";

			if (costAnalystID > 0)
			{
				query = @"SELECT P.*
							FROM Proposals As P
							JOIN ProposalCostAnalysts As PCA ON P.id = PCA.ProposalID
							WHERE P.isDeleted=0 AND PCA.isDeleted=0 AND
								(PCA.isVerify=0 OR PCA.isVerify=3) AND PCA.CostAnalystID=@costAnalystID AND 
								P.OAStatus IN @status";
			}

			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<Proposal>(query, new
				{
					status = new int[] { (int)StaticData.OverallStatus.PROJECT_PROPOSAL,
										(int)StaticData.OverallStatus.COST_ANALYST_REVIEW_IN_PROGRESS},
					costAnalystID = costAnalystID
				}).ToList();

				conn.Close();

			}
			return results;
		}



		public List<Proposal> GetProposalByFinanceApprovalPending (int financeID = 0)
		{
			string query = @"SELECT P.*
								FROM Proposals As P
								JOIN ProposalFinanceApprovals As PFA ON P.id = PFA.ProposalID
								WHERE P.isDeleted=0 AND PFA.isDeleted=0 AND
								P.OAStatus IN @status";

			//(PFA.isVerify=0 OR PFA.isVerify=3) AND 

			if (financeID > 0)
			{
				query = @"SELECT P.*
								FROM Proposals As P
								JOIN ProposalFinanceApprovals As PFA ON P.id = PFA.ProposalID
								WHERE P.isDeleted=0 AND PFA.isDeleted=0 AND PFA.FinanceID=@FinanceID AND
								P.OAStatus IN @status";
			}

			List<Proposal> results = new List<Proposal>();

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<Proposal>(query, new
				{
					status = new int[] { (int)StaticData.OverallStatus.COST_FUNNEL_IDENTIFIED,
										(int)StaticData.OverallStatus.FINANCE_REVIEW_IN_PROGRESS,
										(int)StaticData.OverallStatus.COST_FUNNEL_EVALUATING
									},
					FinanceID = financeID
				}).ToList();

				conn.Close();

			}
			return results;
		}


		//		public List<Proposal> GetProposalByManagerApprovalPending (string mgrFFID = "")
		//		{
		//			string query = @"SELECT P.*
		//							FROM Proposals As P
		//							JOIN ProposalManagerVerifiers As PMV ON P.id = PMV.ProposalID
		//							WHERE P.isDeleted=0 AND PMV.isDeleted=0 AND
		//								(PMV.isVerify=0 OR PMV.isVerify=2) AND
		//								P.OAStatus <> @INVALID AND P.OAStatus=@PENDING_MGR_REVIEW";

		//			if (mgrFFID != "")
		//			{
		//				query = @"SELECT P.*
		//							FROM Proposals As P
		//							JOIN ProposalManagerVerifiers As PMV ON P.id = PMV.ProposalID
		//							WHERE P.isDeleted=0 AND PMV.isDeleted=0 AND
		//								(PMV.isVerify=0 OR PMV.isVerify=2) AND PMV.manFFID=@mgrFFID AND
		//								P.OAStatus <> @INVALID AND P.OAStatus=@PENDING_MGR_REVIEW";
		//			}


		//			List<Proposal> results = new List<Proposal>();

		//			using (var conn = new WrappedDbConnection(GetOpenConnection()))
		//			{
		//				results = conn.Query<Proposal>(query, new
		//				{
		//					INVALID = (int)StaticData.OverallStatus.INVALID,
		//					PENDING_MGR_REVIEW = (int)StaticData.OverallStatus.PENDING_MGR_REVIEW,
		//					mgrFFID = mgrFFID
		//				}).ToList();

		//				conn.Close();

		//			}
		//			return results;
		//		}



		public List<Proposal> GetProposalThatCurrentUserHasActionItem (string ownerFFID)
		{
			string query = @"SELECT *
							FROM Proposals AS P
							JOIN ProposalActions AS PA ON P.id = PA.ProposalID
							WHERE P.OAStatus NOT IN @InvalidStatus AND P.isDeleted=0 AND PA.isDeleted=0 AND PA.isClosed=0 AND PA.OwnerFFID=@OwnerFFID";

			List<Proposal> results = new List<Proposal>();

			var p = new
			{
				InvalidStatus = new int[] {
								(int)StaticData.OverallStatus.INVALID,
								(int)StaticData.OverallStatus.CANCELED,
								//(int)StaticData.OverallStatus.COST_AVOIDANCE,
								(int)StaticData.OverallStatus.EXISTING_PROJECT,
								(int)StaticData.OverallStatus.DUPLICATE_ENTRY
								},
				OwnerFFID = ownerFFID
			};


			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				results = conn.Query<Proposal, ProposalAction, Proposal>(query,
						(P, PA) =>
						{
							P.NeededActions = new List<ProposalAction>();
							P.NeededActions.Add(PA);
							return P;
						}, p).ToList();

				conn.Close();

			}
			return results;
		}



		public int UpdateProposalStatus (int status, int proposalID)
		{
			string query = "UPDATE Proposals SET OAStatus=@status WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					status = status,
					proposalID = proposalID
				});

				conn.Close();

			}


			return rowsUpdated;
		}

		public int UpdateProposalDetails (Proposal proposal)
		{
			string query = @"UPDATE Proposals 
							SET projectType=@ProjectType,
								ProjectTitle=@ProjectTitle,
								CurrentDescription=@CurrentDescription,
								ProposalDescription=@ProposalDescription,
								remarks=@remarks,
								areaDeptBeneficiary=@AreaDeptBeneficiary,
								expectedStartDate=@ExpectedStartDate,
								numberOfMonthsToBeActive=@NumberOfMonthsToBeActive,
								dollarImpact=@DollarImpact
							WHERE id=@Id";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, proposal);

				conn.Close();

			}

			return rowsUpdated;
		}

		public int UpdateProposalDollarImpact (decimal dollarImpact, int proposalID)
		{
			string query = "UPDATE Proposals SET DollarImpact=@dollarImpact WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					dollarImpact = dollarImpact,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}


		public int UpdateProposalProjectType (int projectType, int proposalID)
		{
			string query = "UPDATE Proposals SET projectType=@projectType WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					projectType = projectType,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}


		public int UpdateProposalNummberOfMonthsToBeActive (int numberOfMonths, int proposalID)
		{
			string query = "UPDATE Proposals SET numberOfMonthsToBeActive=@numberOfMonths WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					numberOfMonths = numberOfMonths,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}


		public int UpdateProposalExpectedStartDate (DateTime expectedStartDate, int proposalID)
		{
			string query = "UPDATE Proposals SET expectedStartDate=@expectedStartDate WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					expectedStartDate = expectedStartDate,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}


		public int MarkProposalAsBPI (int status, int proposalID)
		{
			string query = "UPDATE Proposals SET isBPI=@status WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					status = status,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}



		public int UpdateProposalFinanceCategory (int financeCategoryID, int proposalID)
		{
			string query = "UPDATE Proposals SET financeCategoryID=@financeCategoryID WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					financeCategoryID = financeCategoryID,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}


		public int UpdateProposalFunnelStatus (int funnelStatus, int proposalID)
		{
			string query = "UPDATE Proposals SET funnelStatus=@funnelStatus WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					funnelStatus = funnelStatus,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}


		public int UpdateProposalActualCompletionDate (DateTime actualCompletionDate, int proposalID)
		{
			string query = "UPDATE Proposals SET actualCompletionDate=@actualCompletionDate WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					actualCompletionDate = actualCompletionDate,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}

		public int UpdateProposalActualAmount (decimal actualAmount, int proposalID)
		{
			string query = "UPDATE Proposals SET actualAmount=@actualAmount WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					actualAmount = actualAmount,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}

		public int UpdateProposalTrackingCategory (int trackingCategoryID, int proposalID)
		{
			string query = "UPDATE Proposals SET trackingCategoryID=@trackingCategoryID WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					trackingCategoryID = trackingCategoryID,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}

		public int UpdateProposalOriginalDueDate (DateTime origDueDate, int proposalID)
		{
			string query = "UPDATE Proposals SET originalDueDate=@originalDueDate WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					originalDueDate = origDueDate,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}

		public int UpdateProposalCurrentDueDate (DateTime currentDueDate, int proposalID)
		{
			string query = "UPDATE Proposals SET currentDueDate=@currentDueDate WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					currentDueDate = currentDueDate,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}

		public int UpdateProposalPlannedProjectStartDate (DateTime plannedProjectStartDate, int proposalID)
		{
			string query = "UPDATE Proposals SET plannedProjectStartDate=@plannedProjectStartDate WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					plannedProjectStartDate = plannedProjectStartDate,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}

		public int UpdateProposalPlannedSavingStartDate (DateTime plannedSavingStartDate, int proposalID)
		{
			string query = "UPDATE Proposals SET plannedSavingsStartDate=@plannedSavingsStartDate WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					plannedSavingsStartDate = plannedSavingStartDate,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}

		public int AssignProjectOwner (int proposalID, string empFFID, string empFullname, string remarks)
		{
			string query = "UPDATE Proposals SET projectOwnerName=@empFullname, projectOwnerFFID=@empFFID, projectOwnerRemarks=@remarks WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					empFFID = empFFID,
					empFullname = empFullname,
					remarks = remarks,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}


		public int UpdateProjectOwnerRemarks (int proposalID, string remarks)
		{
			string query = "UPDATE Proposals SET projectOwnerRemarks=@remarks WHERE id=@proposalID";

			int rowsUpdated = 0;

			using (var conn = new WrappedDbConnection(GetOpenConnection()))
			{
				rowsUpdated = conn.Execute(query, new
				{
					remarks = remarks,
					proposalID = proposalID
				});

				conn.Close();

			}
			return rowsUpdated;
		}
	}
}
