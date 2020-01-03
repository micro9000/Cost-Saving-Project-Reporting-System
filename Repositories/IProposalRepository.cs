using System.Collections.Generic;
using Domain;
using System;

namespace Repositories
{
	public interface IProposalRepository : IRepository<Proposal>
	{
		int UpdateProposalDetails (Proposal proposal);

		int MarkProposalAsBPI (int status, int proposalID);

		int UpdateProposalStatus (int status, int proposalID);

		int UpdateProposalDollarImpact (decimal dollarImpact, int proposalID);

		int UpdateProposalProjectType (int projectType, int projectID);

		int UpdateProposalNummberOfMonthsToBeActive (int numberOfMonths, int proposalID);

		int UpdateProposalExpectedStartDate (DateTime expectedStartDate, int proposalID);

		int UpdateProposalOriginalDueDate (DateTime origDueDate, int proposalID);

		int UpdateProposalCurrentDueDate (DateTime currentDueDate, int proposalID);

		int UpdateProposalPlannedProjectStartDate (DateTime plannedProjectStartDate, int proposalID);

		int UpdateProposalPlannedSavingStartDate (DateTime plannedSavingStartDate, int proposalID);

		int UpdateProposalActualCompletionDate (DateTime actualCompletionDate, int proposalID);

		int UpdateProposalFinanceCategory (int financeCategoryID, int proposalID);

		int UpdateProposalFunnelStatus (int funnelStatus, int proposalID);

		List<Proposal> SearchProposal (Proposal proposal);

		List<Proposal> SearchProposal (Proposal proposal, string startDate, string endDate);

		List<Proposal> GetAllProposals ();

		List<Proposal> GetAllBPIProposals ();

		List<Proposal> SearchProposalByKeywordAndOrStatusAndOrDepts (string projectType, string keywordStr, string startDate = "", string endDate = "", string[] statusList = null, string[] deptList = null, int isBPI = 2);

		List<Proposal> GetProposalByStatus (int status);

		List<Proposal> GetArchivedProposalByUser (string userFFID);

		List<Proposal> GetProposalByDateSubmittedRange (string startDate, string endDate);

		List<ProposalByOAStatus> GetProposalNumberOfSubmissionsByStatus ();

		List<Proposal> GetActiveProposalsByUser (string userFFID);

		List<Proposal> GetProposalByCostAnalystApprovalPending (int costAnalystID = 0);

		List<Proposal> GetProposalByFinanceApprovalPending (int financeID = 0);

		//List<Proposal> GetProposalByManagerApprovalPending (string mgrFFID = "");

		List<Proposal> GetProposalThatCurrentUserHasActionItem (string ownerFFID);

		//Proposal GetProposalByTicket (string ticket);

		Proposal GetProposalDetailsByID (int proposalID);
	}
}
