using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("CostAnalystsDeptCodes")]
	public class CostAnalystDeptCode
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


		private string deptCode;

		public string DeptCode
		{
			get
			{
				return deptCode;
			}
			set
			{
				deptCode = value;
			}
		}

		private string deptName;

		public string DeptName
		{
			get
			{
				return deptName;
			}
			set
			{
				deptName = value;
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

	public class CostAnalystDeptCodeMapper : ClassMapper<CostAnalystDeptCode>
	{
		public CostAnalystDeptCodeMapper ()
		{
			Table("CostAnalystsDeptCodes");
			AutoMap();
		}
	}

}
