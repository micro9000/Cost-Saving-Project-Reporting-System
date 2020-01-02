using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("ProposalStatusLogs")]
	public class ProposalStatusLog
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

		private int oaStatus;

		public int OAStatus
		{
			get
			{
				return oaStatus;
			}
			set
			{
				oaStatus = value;
			}
		}


		private string approverFFID;

		public string ApproverFFID
		{
			get
			{
				return approverFFID;
			}
			set
			{
				approverFFID = value;
			}
		}

		private string approverName;

		public string ApproverName
		{
			get
			{
				return approverName;
			}
			set
			{
				approverName = value;
			}
		}


		private DateTime dateChanged = DateTime.MinValue;

		public DateTime DateChanged
		{
			get
			{
				return dateChanged;
			}
			set
			{
				dateChanged = value;
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

	public class ProposalStatusLogMapper : ClassMapper<ProposalStatusLog>
	{
		public ProposalStatusLogMapper ()
		{
			Table("ProposalStatusLogs");
			AutoMap();
		}
	}
}
