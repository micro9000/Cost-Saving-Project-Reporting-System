using DapperExtensions.Mapper;

namespace Domain
{
	public class SupportingDoc
	{
		private int id;
		[Dapper.Contrib.Extensions.Key]
		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		private int proposalId;

		public int ProposalId
		{
			get
			{
				return proposalId;
			}
			set
			{
				proposalId = value;
			}
		}

		private string serverFileName;

		public string ServerFileName
		{
			get
			{
				return serverFileName;
			}
			set
			{
				serverFileName = value;
			}
		}

		private string origFileName;

		public string OrigFileName
		{
			get
			{
				return origFileName;
			}
			set
			{
				origFileName = value;
			}
		}

		private string attachedBy;

		public string AttachedBy
		{
			get
			{
				return attachedBy;
			}
			set
			{
				attachedBy = value;
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


	public class SupportingDocMapper : ClassMapper<SupportingDoc>
	{
		public SupportingDocMapper ()
		{
			Table("SupportingDocs");
			AutoMap();
		}
	}
}
