using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Domain;
using ESavingsFactory;
using System.Configuration;
using ESavingsAPI.Models;

namespace ESavingsAPI.Controllers
{

	public class ProposalsController : ApiController
	{
		public List<QlikView> Get ()
		{
			List<Proposal> proposals = Factory.ProposalFactory().GetAllProposals().ToList();

			var financeCategories = Loaders.GetAllFinanceCategory();

			var departments = Loaders.GetAllDepartments();

			string site = ConfigurationManager.AppSettings["onsemi_site_code"];

			var results = (from p in proposals
						   select new QlikView()
						   {
							   site = site,
							   projectTitle = WebUtility.HtmlDecode(p.ProjectTitle),
							   benefitType = p.ProjectTypeIndicator,
							   monthlyDollarAmount = p.DollarImpact,
							   numberOfMonthsToBeActive = p.NumberOfMonthsToBeActive,
							   amount = (p.DollarImpact * p.NumberOfMonthsToBeActive),
							   projectStatus = p.OAStatusIndicator,
							   rank = 0,
							   financeCategory = financeCategories[p.FinanceCategoryID],
							   notes = WebUtility.HtmlDecode(p.ProposalDescription),
							   funnelStatus = p.FunnelStatusIndicator,
							   description = p.Remarks,
							   functionalArea = departments[p.AreaDeptBeneficiary],
							   owner = (p.ProjectOwnerName == null) ? p.SubmittedBy : p.ProjectOwnerName,
							   originalDueDate = p.OriginalDueDate == (new DateTime(0001, 01, 01)) ? "" : p.OriginalDueDate.ToString("yyyy-MM-dd"),
							   currentDueDate = p.CurrentDueDate == (new DateTime(0001, 01, 01)) ? "" : p.CurrentDueDate.ToString("yyyy-MM-dd"),
							   plannedProjectStartDate = p.PlannedProjectStartDate == (new DateTime(0001, 01, 01)) ? "" : p.PlannedProjectStartDate.ToString("yyyy-MM-dd"),
							   plannedSavingStartDate = p.PlannedSavingsStartDate == (new DateTime(0001, 01, 01)) ? "" : p.PlannedSavingsStartDate.ToString("yyyy-MM-dd"),
							   actualCompletionDate = p.ActualCompletionDate == (new DateTime(0001, 01, 01)) ? "" : p.ActualCompletionDate.ToString("yyyy-MM-dd"),
							   actualAmount = 0
						   }).OrderByDescending(p => p.amount).ToList();

			return results;
		}
	}
}
