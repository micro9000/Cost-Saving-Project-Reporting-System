using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("CostAnalysts")]
	public class CostAnalyst
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


		private string area;

		public string Area
		{
			get
			{
				return area;
			}
			set
			{
				area = value;
			}
		}

		private IList<CostAnalystDeptCode> deptCodes;

		public IList<CostAnalystDeptCode> DeptCodes
		{
			get
			{
				return deptCodes;
			}
			set
			{
				deptCodes = value;
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

	public class CostAnalystMapper : ClassMapper<CostAnalyst>
	{
		public CostAnalystMapper ()
		{
			Table("CostAnalysts");
			Map(p => p.DeptCodes).Ignore();
			AutoMap();
		}
	}

}
