using Repositories;
using Persistence.Repositories;

namespace ESavingsFactory
{
	public class Factory
	{
		public static IProposalRepository ProposalFactory ()
		{
			return new ProposalRepository();
		}

		public static ICurrentImgRepository CurrentImgFactory ()
		{
			return new CurrentImgRepository();
		}


		public static IProposalImgRepository ProposalImgFactory ()
		{
			return new ProposalImgRepository();
		}

		public static ISupportingDocRepository SupportingDocFactory ()
		{
			return new SupportingDocRepository();
		}

		public static IOperatorUserRepository OperatorFactory ()
		{
			return new OperatorUserRepository();
		}

		public static ICostAnalystRepository CostAnalystFactory ()
		{
			return new CostAnalystRepository();
		}

		public static IFinanceApproverRepository FinanceApproverFactory ()
		{
			return new FinanceApproverRepository();
		}

		public static ICostAnalystDeptCodesRepository CostAnalystDeptCodesFactory ()
		{
			return new CostAnalystDeptCodesRepository();
		}

		public static IProposalCostAnalystRepository ProposalCostAnalystRepository ()
		{
			return new ProposalCostAnalystRepository();
		}


		public static IProposalFinanceApprovalRepository ProposalFinanceApprovalRepository ()
		{
			return new ProposalFinanceApprovalRepository();
		}

		public static IProposalManagerVerifierRepository ProposalManagerVerifierRepository ()
		{
			return new ProposalManagerVerifierRepository();
		}


		public static IProposalActionRepository ProposalActionRepository ()
		{
			return new ProposalActionRepository();
		}

		public static IProposalActionApproverRepository ProposalActionApproverRepository ()
		{
			return new ProposalActionApproverRepository();
		}


		public static ICostSiteInChargeRepository CostSiteInChargeRepository ()
		{
			return new CostSiteInChargeRepository();
		}

		public static IFinanceCategory FinanceCategoryRepository ()
		{
			return new FinanceCategoryRepository();
		}

		public static IProposalStatusLogRepository ProposalStatusLogRepository ()
		{
			return new ProposalStatusLogRepository();
		}
	}
}
