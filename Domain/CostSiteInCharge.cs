using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("CostSiteIncharges")]
	public class CostSiteInCharge
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


		private string ffID;

		public string FFID
		{
			get
			{
				return ffID;
			}
			set
			{
				ffID = value;
			}
		}


		private string fullName;

		public string FullName
		{
			get
			{
				return fullName;
			}
			set
			{
				fullName = value;
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


	public class CostSiteInChargeMapper : ClassMapper<CostSiteInCharge>
	{
		public CostSiteInChargeMapper ()
		{
			Table("CostSiteIncharges");
			AutoMap();
		}
	}
}
