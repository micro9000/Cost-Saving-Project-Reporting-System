using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface IProposalFinanceApprovalRepository : IRepository<ProposalFinanceApproval>
	{

		int ReassignProposalFinanceApproval (int proposalID, int newfinanceID, int lastFinanceID);

		List<ProposalFinanceApproval> GetProposalFinanceInfoAndVerificationInfo (int proposalID);

		ProposalFinanceApproval GetProposalFinanceInfoVerificationResults (int proposalID, int financeID);

		int UpdateProposalFinanceInfoVerification (int proposalID, int financeID, string remarks, int isVerify);

		int DeleteProposalFinance (int proposalID, int financeID, string remarks);

	}
}
