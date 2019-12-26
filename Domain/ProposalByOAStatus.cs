using DapperExtensions.Mapper;

namespace Domain
{
	public class ProposalByOAStatus
	{
		public int OAStatus
		{
			get;
			set;
		}

		public string OAStatusIndicator
		{
			get;
			set;
		}


		public int Submissions
		{
			get;
			set;
		}

	}


	public class ProposalByOAStatusMapper : ClassMapper<ProposalByOAStatus>
	{
		public ProposalByOAStatusMapper ()
		{
			Map(p => p.OAStatusIndicator).Ignore();
			AutoMap();
		}
	}
}
