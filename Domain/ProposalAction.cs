using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("ProposalActions")]
	public class ProposalAction
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

		private string neededAction;

		public string NeededAction
		{
			get
			{
				return neededAction;
			}
			set
			{
				neededAction = value;
			}
		}

		private string ownerFFID;

		public string OwnerFFID
		{
			get
			{
				return ownerFFID;
			}
			set
			{
				ownerFFID = value;
			}
		}

		private string ownerFullname;

		public string OwnerFullname
		{
			get
			{
				return ownerFullname;
			}
			set
			{
				ownerFullname = value;
			}
		}

		private string ownerRemarks;

		public string OwnerRemarks
		{
			get
			{
				return ownerRemarks;
			}
			set
			{
				ownerRemarks = value;
			}
		}

		private DateTime dateAssign = DateTime.UtcNow;

		public DateTime DateAssign
		{
			get
			{
				return dateAssign;
			}
			set
			{
				dateAssign = value;
			}
		}


		private DateTime dateResponse = DateTime.MinValue;

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


		private int isClosed;

		public int IsClosed
		{
			get
			{
				return isClosed;
			}
			set
			{
				isClosed = value;
			}
		}


		private List<ProposalActionApprover> approvers;

		public List<ProposalActionApprover> Approvers
		{
			get
			{
				return approvers;
			}
			set
			{
				approvers = value;
			}
		}


	}


	public class ProposalActionMapper : ClassMapper<ProposalAction>
	{
		public ProposalActionMapper ()
		{
			Table("ProposalActions");
			Map(p => p.Approvers).Ignore();
			AutoMap();
		}
	}
}
