using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("masterlist")]
	public class SOBManagers
	{
		[Dapper.Contrib.Extensions.Key]
		public int id
		{
			get;
			set;
		}


		public string userCID
		{
			get;
			set;
		}

		public string lastName
		{
			get;
			set;
		}

		public string firstName
		{
			get;
			set;
		}

		public string email
		{
			get;
			set;
		}

		public string deptCode
		{
			get;
			set;
		}

		public string department_head
		{
			get;
			set;
		}

		public string remarks
		{
			get;
			set;
		}

		public string ffId
		{
			get;
			set;
		}
	}

	public class SOBManagersMapper : ClassMapper<SOBManagers>
	{
		public SOBManagersMapper ()
		{
			Table("masterlist");
			AutoMap();
		}
	}
}
