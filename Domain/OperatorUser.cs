using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	public class OperatorUser
	{
		private int id;

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

		private string password;

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value;
			}
		}


	}


	public class OperatorUserMapper : ClassMapper<OperatorUser>
	{
		public OperatorUserMapper ()
		{
			Table("Operators");
			AutoMap();
		}
	}
}
