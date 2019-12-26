using DapperExtensions.Mapper;

namespace Domain
{
	public class ProposalImg
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

	public class ProposalImgMapper : ClassMapper<ProposalImg>
	{
		public ProposalImgMapper ()
		{
			Table("ProposalImgs");
			AutoMap();
		}
	}
}
