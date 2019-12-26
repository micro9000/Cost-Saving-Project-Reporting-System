using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("ProposalCostAnalysts")]
	public class ProposalCostAnalyst
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

		private int costAnalystID;

		public int CostAnalystID
		{
			get
			{
				return costAnalystID;
			}
			set
			{
				costAnalystID = value;
			}
		}

		private string remarks = "na";

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

		private int isVerify;

		public int IsVerify
		{
			get
			{
				return isVerify;
			}
			set
			{
				isVerify = value;
			}
		}



		private DateTime dateAssign;

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

		private DateTime verifyDate = DateTime.MinValue;

		public DateTime VerifyDate
		{
			get
			{
				return verifyDate;
			}
			set
			{
				verifyDate = value;
			}
		}

		private int isDeleted;

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

		private CostAnalyst costAnalystInfo;

		public CostAnalyst CostAnalystInfo
		{
			get
			{
				return costAnalystInfo;
			}
			set
			{
				costAnalystInfo = value;
			}
		}
	}

	public class ProposalCostAnalystMapper : ClassMapper<ProposalCostAnalyst>
	{
		public ProposalCostAnalystMapper ()
		{
			Table("ProposalCostAnalysts");
			Map(p => p.DateAssign).Ignore();
			Map(p => p.CostAnalystInfo).Ignore();
			AutoMap();
		}
	}

}
