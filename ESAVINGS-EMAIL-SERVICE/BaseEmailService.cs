using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using SendEmailServiceInterfaces;
using Domain;
using ESavingsFactory;


namespace ESAVINGS_EMAIL_SERVICE
{
	public class BaseEmailService
	{
		public List<Proposal> GetProposalAdditionalInfo (List<Proposal> proposals)
		{
			//var departments = Factory.CostAnalystDeptCodesFactory().GetDepartments();

			for (int i = 0 ; i <proposals.Count ; i++)
			{
				var proposal = proposals[i];
				proposal.CurrentImgs = Factory.CurrentImgFactory().GetProposalCurrentImgs(proposal.Id);
				proposal.ProposalImgs = Factory.ProposalImgFactory().GetProposalImgs(proposal.Id);
				proposal.OAStatusIndicator = StaticData.GetOverallStatusStr(proposal.OAStatus);

				//var deptTmp = departments.Where(d => d.DeptCode == proposal.EmpDeptCode).FirstOrDefault();
				var dept = Factory.CostAnalystDeptCodesFactory().GetDepartment(proposal.AreaDeptBeneficiary);
				proposal.DeptName = dept != null ? dept.DeptName : "";


				proposal.ExpectedStartDateStr = proposal.ExpectedStartDate.ToString("yyyy-MM-dd");


				proposals[i] = proposal;
			}

			return proposals;
		}
	}
}
