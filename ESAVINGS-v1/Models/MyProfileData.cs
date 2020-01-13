using Domain;
using System.Collections.Generic;

namespace ESAVINGS_v1.Models
{
	public class MyProfileData
	{
		private List<ProposalByOAStatus> proposalNumberOfSubmissionsByStatus;

		public List<ProposalByOAStatus> ProposalNumberOfSubmissionsByStatus
		{
			get
			{
				return proposalNumberOfSubmissionsByStatus;
			}
			set
			{
				proposalNumberOfSubmissionsByStatus = value;
			}
		}


		private List<Proposal> assignedProjects;

		public List<Proposal> AssignedProjects
		{
			get
			{
				return assignedProjects;
			}
			set
			{
				assignedProjects = value;
			}
		}



		private List<Proposal> activeProposals;

		public List<Proposal> ActiveProposals
		{
			get
			{
				return activeProposals;
			}
			set
			{
				activeProposals = value;
			}
		}



		private List<Proposal> forApprovalProposals;

		public List<Proposal> ForApprovalProposals
		{
			get
			{
				return forApprovalProposals;
			}
			set
			{
				forApprovalProposals = value;
			}
		}


		private List<Proposal> currentUserActionItems;

		public List<Proposal> CurrentUserActionItems
		{
			get
			{
				return currentUserActionItems;
			}
			set
			{
				currentUserActionItems = value;
			}
		}



		private List<Proposal> archivedProposals;

		public List<Proposal> ArchivedProposals
		{
			get
			{
				return archivedProposals;
			}
			set
			{
				archivedProposals = value;
			}
		}
	}
}