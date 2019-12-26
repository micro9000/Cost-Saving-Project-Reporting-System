using System.Collections.Generic;
using Domain;


namespace Repositories
{
	public interface ICostAnalystDeptCodesRepository : IRepository<CostAnalystDeptCode>
	{
		List<CostAnalystDeptCode> GetDeptCodesByCostAnalystID (int costAnalystID);

		List<CostAnalystDeptCode> GetDepartments ();

		CostAnalystDeptCode GetDepartment (string deptCodeOrName);
	}
}
