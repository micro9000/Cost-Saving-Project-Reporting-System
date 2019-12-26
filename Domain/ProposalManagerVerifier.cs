using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("ProposalManagerVerifiers")]
	public class ProposalManagerVerifier
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

		private string manFFID;

		public string ManFFID
		{
			get
			{
				return manFFID;
			}
			set
			{
				manFFID = value;
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
	}

	public class ProposalManagerVerifierMapper : ClassMapper<ProposalManagerVerifier>
	{
		public ProposalManagerVerifierMapper ()
		{
			Table("ProposalManagerVerifiers");
			Map(p => p.DateAssign).Ignore();
			Map(p => p.FullName).Ignore();
			Map(p => p.Email).Ignore();
			AutoMap();
		}
	}
}
