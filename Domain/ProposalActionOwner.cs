using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("ProposalActionOwners")]
	public class ProposalActionOwner
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


		private int empFFID;

		public int EmpFFID
		{
			get
			{
				return empFFID;
			}
			set
			{
				empFFID = value;
			}
		}


		private int empFullName;

		public int EmpFullName
		{
			get
			{
				return empFullName;
			}
			set
			{
				empFullName = value;
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

		private DateTime dateResponse = DateTime.MinValue;

		public DateTime DateReponse
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

	}
}
