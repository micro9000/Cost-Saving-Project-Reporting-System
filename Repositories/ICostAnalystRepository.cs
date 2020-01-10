using System.Collections.Generic;
using Domain;

namespace Repositories
{
	public interface ICostAnalystRepository : IRepository<CostAnalyst>
	{
		CostAnalyst GetInfoByFFID (string ffID);
		List<CostAnalyst> GetInfoByDeptCode (string deptCode);
		List<CostAnalyst> GetAllCostAnalyst ();
	}
}
