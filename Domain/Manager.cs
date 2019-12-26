using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("Managers")]
	public class Manager
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


		private string cid;

		public string CID
		{
			get
			{
				return cid;
			}
			set
			{
				cid = value;
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

		private string email;

		public string Email
		{
			get
			{
				return email;
			}
			set
			{
				email = value;
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

	public class ManagerMapper : ClassMapper<Manager>
	{
		public ManagerMapper ()
		{
			Table("Managers");
			AutoMap();
		}
	}
}
