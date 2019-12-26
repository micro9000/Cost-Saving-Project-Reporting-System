using System.Collections.Generic;
using Domain;
using System;

namespace Repositories
{
	public interface IFinanceCategory : IRepository<FinanceCategory>
	{
		List<FinanceCategory> GetAllCategories ();
	}
}
