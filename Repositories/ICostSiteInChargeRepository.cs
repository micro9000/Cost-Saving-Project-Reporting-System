using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface ICostSiteInChargeRepository : IRepository<CostSiteInCharge>
	{
		CostSiteInCharge GetProposalCostSiteInCharge (int proposalID);

		int UpdateProposalCostSiteInChargeVerification (CostSiteInCharge verification);

	}
}
