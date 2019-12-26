using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("ProposalActionsApprovers")]
	public class ProposalActionApprover
	{
		private int id;
		[Dapper.Contrib.Extensions.Key]
		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		private int proposalID;

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

		private int actionID;

		public int ActionID
		{
			get
			{
				return actionID;
			}
			set
			{
				actionID = value;
			}
		}


		private string approverFFID;

		public string ApproverFFID
		{
			get
			{
				return approverFFID;
			}
			set
			{
				approverFFID = value;
			}
		}

		private string approverFullname;

		public string ApproverFullname
		{
			get
			{
				return approverFullname;
			}
			set
			{
				approverFullname = value;
			}
		}


		private string remarks;

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

		private int approvalStatus;

		public int ApprovalStatus
		{
			get
			{
				return approvalStatus;
			}
			set
			{
				approvalStatus = value;
			}
		}

		private DateTime dateResponse;

		public DateTime DateResponse
		{
			get
			{
				return dateResponse;
			}
			set
			{
				dateResponse = value;
			}
		}


		private int isDeleted = 0;

		public int IsDeleted
		{
			get
			{
				return isDeleted;
			}
			set
			{
				isDeleted = value;
			}
		}

	}

	public class ProposalActionApproverMapper : ClassMapper<ProposalActionApprover>
	{
		public ProposalActionApproverMapper ()
		{
			Table("ProposalActionsApprovers");
			AutoMap();
		}
	}
}
