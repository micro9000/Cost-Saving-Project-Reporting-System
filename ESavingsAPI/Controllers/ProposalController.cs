using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Domain;
using ESavingsFactory;

namespace ESavingsAPI.Controllers
{
	public class ProposalController : ApiController
	{
		public List<Proposal> Get ()
		{
			List<Proposal> proposals = Factory.ProposalFactory().GetAllProposals().ToList();

			for (int i = 0 ; i <proposals.Count ; i++)
			{
				var proposal = proposals[i];

				//proposal.CurrentImgs = Factory.CurrentImgFactory().GetProposalCurrentImgs(proposal.Id);
				//proposal.ProposalImgs = Factory.ProposalImgFactory().GetProposalImgs(proposal.Id);
				//proposal.OAStatusIndicator = StaticData.GetOverallStatusStr(proposal.OAStatus);
				//proposal.ProjectTypeIndicator = StaticData.GetProjectTypeStr(proposal.ProjectType);
				////proposal.SiteIndicator = ConfigurationManager.AppSettings["onsemi_site"];

				////proposal.SiteBaseURL = Request.Url.GetLeftPart(UriPartial.Authority) +""+ Url.Content("~");
				////proposal.CurrentImgsPath = Url.Content(ConfigurationManager.AppSettings["dir_for_upload_current_imgs"]);
				////proposal.ProposalImgsPath = Url.Content(ConfigurationManager.AppSettings["dir_for_upload_proposal_imgs"]);

				////var deptTmp = departments.Where(d => d.DeptCode == proposal.EmpDeptCode).FirstOrDefault();
				//var dept = Factory.CostAnalystDeptCodesFactory().GetDepartment(proposal.AreaDeptBeneficiary);
				//proposal.DeptName = dept != null ? dept.DeptName : "";

				//proposal.ExpectedStartDateStr = proposal.ExpectedStartDate.ToString("yyyy-MM-dd");

				proposal.ProjectTitle = WebUtility.HtmlDecode(proposal.ProjectTitle);
				proposal.CurrentDescription = WebUtility.HtmlDecode(proposal.CurrentDescription);
				proposal.ProposalDescription = WebUtility.HtmlDecode(proposal.ProposalDescription);
				proposal.Remarks = WebUtility.HtmlDecode(proposal.Remarks);

				proposals[i] = proposal;
			}

			return proposals;
		}
	}
}
