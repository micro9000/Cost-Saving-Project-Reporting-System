using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface IFinanceApproverRepository : IRepository<FinanceApprover>
	{
		FinanceApprover GetInfoByFFID (string ffID);
	}
}
