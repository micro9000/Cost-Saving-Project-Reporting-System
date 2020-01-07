using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESavingsFactory;
using System.Configuration;
using ESAVINGS_v1.Models;
using Domain;
using System.Globalization;
using System.Threading;
//using System.Reflection;

namespace ESAVINGS_v1.Controllers
{
	public class HomeController : BaseController
	{
		//
		// GET: /Home/

		public ActionResult Index ()
		{

			CultureInfo[] ci = CultureInfo.GetCultures(CultureTypes.AllCultures);

			List<string> cultures = new List<string>();

			foreach (CultureInfo c in ci)
			{
				cultures.Add(c.Name);
			}
			ViewBag.cultures = cultures;


			this.ViewBagUserData();
			this.ViewBagUserTypes();
			this.ViewBagProjectTypes();
			ViewBag.ProjectTypes = StaticData.GetProjectTypeWithLabel();

			ViewBag.dirCurrentImgs = ConfigurationManager.AppSettings["dir_for_upload_current_imgs"];
			ViewBag.dirProposalImgs = ConfigurationManager.AppSettings["dir_for_upload_proposal_imgs"];

			ViewBag.DeptNames = (from d in Factory.CostAnalystDeptCodesFactory().GetDepartments()
								 select d.DeptName).ToList().Distinct();

			ViewBag.DepartmentListTmp = Factory.CostAnalystDeptCodesFactory().GetDepartments();


			ViewBag.statusWithLabel = StaticData.GetOverallStatusWithLabel();

			return View();
		}




		public ActionResult BPIs ()
		{
			this.ViewBagUserData();
			this.ViewBagUserTypes();
			return View();
		}

		public JsonResult testingJson ()
		{
			Response.AppendHeader("Access-Control-Allow-Origin", "*");
			List<Proposal> results = Factory.ProposalFactory().GetAllProposals().ToList();
			return Json(results, JsonRequestBehavior.AllowGet);
		}


		public ActionResult HowTo ()
		{
			this.ViewBagUserData();
			this.ViewBagUserTypes();
			return View();
		}


		public ActionResult MyProfile ()
		{
			MyProfileData profileData = new MyProfileData();

			if (IsUserSuccessfullyLoggedIn() == false)
			{
				return RedirectToAction("UserLogin", "User");
			}
			this.ViewBagUserData();
			this.ViewBagUserTypes();



			var proposalNumberOfSubmissionsByStatus = Factory.ProposalFactory().GetProposalNumberOfSubmissionsByStatus();
			for (int i = 0 ; i<proposalNumberOfSubmissionsByStatus.Count ; i++)
			{
				proposalNumberOfSubmissionsByStatus[i].OAStatusIndicator = StaticData.GetOverallStatusStr(proposalNumberOfSubmissionsByStatus[i].OAStatus);
			}
			profileData.ProposalNumberOfSubmissionsByStatus = proposalNumberOfSubmissionsByStatus;




			profileData.ActiveProposals = this.GetProposalAdditionalInfo(Factory.ProposalFactory().GetActiveProposalsByUser(this.UserFFID));



			List<Proposal> forApprovalProposals = new List<Proposal>();

			if (this.UserType == ((int)StaticData.UserTypes.CostAnalyst).ToString())
			{// COST ANALYST
				if (this.IsUserMaster == true)
				{
					forApprovalProposals = Factory.ProposalFactory().GetProposalByCostAnalystApprovalPending();
				}
				else
				{
					forApprovalProposals = Factory.ProposalFactory().GetProposalByCostAnalystApprovalPending(int.Parse(this.UserCostAnalystID));
				}

			}
			else if (this.UserType == ((int)StaticData.UserTypes.Finance).ToString())
			{ // FINANCE
				if (this.IsUserMaster == true)
				{
					forApprovalProposals = Factory.ProposalFactory().GetProposalByFinanceApprovalPending();
				}
				else
				{
					forApprovalProposals = Factory.ProposalFactory().GetProposalByFinanceApprovalPending(int.Parse(this.UserFinanceID));
				}
			}
			//else if (this.UserType == ((int)StaticData.UserTypes.Manager).ToString())
			//{// MANAGER
			//	forApprovalProposals = Factory.ProposalFactory().GetProposalByManagerApprovalPending(this.UserFFID);
			//}
			//else if (this.UserType == ((int)StaticData.UserTypes.CostSiteInCharge).ToString())
			//{// COST SITE INCHARGE
			//	forApprovalProposals = Factory.ProposalFactory().GetProposalByStatus((int)StaticData.OverallStatus.ACTION_SITE_COST_INCHARGE_REVIEW);
			//}

			forApprovalProposals = this.GetProposalAdditionalInfo(forApprovalProposals);
			profileData.ForApprovalProposals = forApprovalProposals;


			profileData.CurrentUserActionItems = Factory.ProposalFactory().GetProposalThatCurrentUserHasActionItem(this.UserFFID);


			profileData.ArchivedProposals = this.GetProposalAdditionalInfo(Factory.ProposalFactory().GetArchivedProposalByUser(this.UserFFID));


			ViewBag.ProjectTypes = StaticData.GetProjectTypeWithLabel();

			return View(profileData);
		}

		//public JsonResult testing ()
		//{
		//	if (this.IsUserIsFinanceApproverOnTheCurrentProposal(32))
		//	{
		//		return Json("YES", JsonRequestBehavior.AllowGet);
		//	}
		//	return Json(this.UserFinanceID, JsonRequestBehavior.AllowGet);
		//}

		public ActionResult Proposal (int proposalID = 0)
		{

			//if (this.IsCostSiteInChargeExistsInAD() == false)
			//{
			//	return RedirectToAction("CostSiteInChargeChecking", "Home");
			//}


			this.ViewBagUserData();
			this.ViewBagUserTypes();

			ViewBag.dirCurrentImgs = ConfigurationManager.AppSettings["dir_for_upload_current_imgs"];
			ViewBag.dirProposalImgs = ConfigurationManager.AppSettings["dir_for_upload_proposal_imgs"];

			if (IsUserSuccessfullyLoggedIn() == false)
			{
				return RedirectToAction("UserLogin", "User");
			}

			ViewBag.proposalID = proposalID;
			Proposal proposalDetails = new Proposal();


			if (proposalID != 0)
			{

				proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalID);

				if (proposalDetails == null)
				{
					return RedirectToAction("NotFound404", "Home");
				}
				else
				{

					if (proposalDetails.EmpFFID == this.UserFFID || 
						this.IsUserIsCostAnalystOnTheCurrentProposal(proposalDetails.AreaDeptBeneficiary) ||
						this.IsUserIsFinanceApproverOnTheCurrentProposal(proposalDetails.Id) ||
						this.IsUserMaster == true)
					{
						proposalDetails.CurrentImgs = Factory.CurrentImgFactory().GetProposalCurrentImgs(proposalID);

						proposalDetails.ProposalImgs = Factory.ProposalImgFactory().GetProposalImgs(proposalID);

						proposalDetails.SupportingDocs = Factory.SupportingDocFactory().GetProposalSupportingDocs(proposalID);
					}
					else
					{
						return RedirectToAction("NotFound404", "Home");
					}
				}
			}

			var departmentGroupList = (from d in Factory.CostAnalystDeptCodesFactory().GetDepartments()
									   orderby d.DeptName
									   select d.DeptName).ToList().Distinct();

			ViewBag.DepartmentList = departmentGroupList;
			ViewBag.DepartmentListTmp = Factory.CostAnalystDeptCodesFactory().GetDepartments();

			ViewBag.ProjectTypes = StaticData.GetProjectTypeWithLabel();

			//Factory.CostAnalystDeptCodesFactory().GetDepartments();

			return View(proposalDetails);
		}


		public ActionResult Details (int proposalID=0)
		{

			this.ViewBagUserData();
			this.ViewBagUserTypes();
			this.ViewBagOverallStatus();
			ViewBag.ProjectTypes = StaticData.GetProjectTypeWithLabel();
			ViewBag.GlobalFunnelStatus = StaticData.GetGlobalFunnelStatusWithLabel();
			ViewBag.proposalID = proposalID;

			Proposal proposalDetails = new Proposal();

			if (proposalID == 0)
			{
				return RedirectToAction("NotFound404", "Home");
			}
			else
			{


				ViewBag.dirCurrentImgs = ConfigurationManager.AppSettings["dir_for_upload_current_imgs"];
				ViewBag.dirProposalImgs = ConfigurationManager.AppSettings["dir_for_upload_proposal_imgs"];

				proposalDetails = Factory.ProposalFactory().GetProposalDetailsByID(proposalID);


				if (proposalDetails != null)
				{
					ViewBag.UserIsCostAnalystOnCurrentProposal = this.IsUserIsCostAnalystOnTheCurrentProposal(proposalDetails.AreaDeptBeneficiary);
					ViewBag.UserIsFinanceApprovalOnCurrentProposal = this.IsUserIsFinanceApproverOnTheCurrentProposal(proposalDetails.Id);
					ViewBag.UserIsManagerVerifierOnCurrentProposal = this.isUserIsManagerVerifierOnTheCurrentProposal(proposalID);


					proposalDetails.CurrentImgs = Factory.CurrentImgFactory().GetProposalCurrentImgs(proposalID);

					proposalDetails.ProposalImgs = Factory.ProposalImgFactory().GetProposalImgs(proposalID);

					proposalDetails.SupportingDocs = Factory.SupportingDocFactory().GetProposalSupportingDocs(proposalID);

					proposalDetails.OAStatusIndicator = StaticData.GetOverallStatusStr(proposalDetails.OAStatus);

					proposalDetails.ProjectTypeIndicator = StaticData.GetProjectTypeStr(proposalDetails.ProjectType);

					proposalDetails.CostAnalysts = Factory.ProposalCostAnalystRepository().GetProposalCostAnalystInfoAndVerificationInfo(proposalID);

					//proposalDetails.Managers = this.GetProposalManagers(proposalID);

					proposalDetails.NeededActions = Factory.ProposalActionRepository().GetProposalActionsWithApprovers(proposalID);

					//proposalDetails.CostSiteInCharge = Factory.CostSiteInChargeRepository().GetProposalCostSiteInCharge(proposalID);

					proposalDetails.FinanceApproval = Factory.ProposalFinanceApprovalRepository().GetProposalFinanceInfoAndVerificationInfo(proposalID);

					var dept = Factory.CostAnalystDeptCodesFactory().GetDepartment(proposalDetails.AreaDeptBeneficiary);
					proposalDetails.AreaDeptBeneficiary += " - " + dept.DeptName;

					proposalDetails.SiteBaseURL = Request.Url.GetLeftPart(UriPartial.Authority) +""+ Url.Content("~");
					proposalDetails.CurrentImgsPath = Url.Content(ConfigurationManager.AppSettings["dir_for_upload_current_imgs"]);
					proposalDetails.ProposalImgsPath = Url.Content(ConfigurationManager.AppSettings["dir_for_upload_proposal_imgs"]);

					proposalDetails.FunnelStatusIndicator = ViewBag.GlobalFunnelStatus[proposalDetails.FunnelStatus];

					var financeCategories = this.GetAllFinanceCategoryById();
					proposalDetails.FinanceCategory = financeCategories[proposalDetails.FinanceCategoryID].Category;

					if (IsUserSuccessfullyLoggedIn() == true)
					{
						ViewBag.FinanceList = Factory.FinanceApproverFactory().GetAll().Where(f => f.IsDeleted == 0);
						ViewBag.CostAnalystList = Factory.CostAnalystFactory().GetAll().Where(ca => ca.IsDeleted == 0 && ca.IsMaster == 0);

						ViewBag.FinanceCategories = Factory.FinanceCategoryRepository().GetAllCategories();

					}

				}
				else
				{
					return RedirectToAction("NotFound404", "Home");
				}



			}

			return View(proposalDetails);
		}


		public ActionResult NotFound404 ()
		{
			return View();
		}

		public ActionResult CostSiteInChargeChecking ()
		{
			return View();
		}


	}
}
