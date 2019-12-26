using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESAVINGS_v1.Controllers
{
	public class ManagerApprovalNeedData
	{
		private int proposalID;
		[Required(ErrorMessage = "Proposal ID is required")]
		public int ProposalID
		{
			get
			{
				return proposalID;
			}
			set
			{
				proposalID = value;
			}
		}


		private int isVerified;

		[Required(ErrorMessage = "Status is required")]
		public int IsVerified
		{
			get
			{
				return isVerified;
			}
			set
			{
				isVerified = value;
			}
		}


		private string remarks;

		[Required(ErrorMessage = "Remarks is required")]
		public string Remarks
		{
			get
			{
				return remarks;
			}
			set
			{
				remarks = value;
			}
		}
	}
}