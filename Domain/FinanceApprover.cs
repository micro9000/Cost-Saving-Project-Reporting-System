using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("FinanceApprovers")]
	public class FinanceApprover
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


		private int isMaster;

		public int IsMaster
		{
			get
			{
				return isMaster;
			}
			set
			{
				isMaster = value;
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

	public class FinanceApproverMapper : ClassMapper<FinanceApprover>
	{
		public FinanceApproverMapper ()
		{
			Table("FinanceApprovers");
			AutoMap();
		}
	}
}
