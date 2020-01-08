using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using ESavingsFactory;
using System.Configuration;
using ESavingsAPI.Models;

namespace ESavingsAPI.Controllers
{
	public static class Loaders
	{
		public static Dictionary<int, string> GetAllFinanceCategory ()
		{

			// var financeCategories = Factory.FinanceCategoryRepository().GetAllCategories();
			// financeCategories.Where(f => f.Id == proposal.FinanceCategoryID).Select(f => f.Category).ToString(),

			var newCategories = new Dictionary<int, string>();

			var categories = Factory.FinanceCategoryRepository().GetAllCategories();

			foreach (var cat in categories)
			{
				newCategories.Add(cat.Id, cat.Category);
			}

			return newCategories;
		}

		public static Dictionary<string, string> GetAllDepartments ()
		{
			var departments = Factory.CostAnalystDeptCodesFactory().GetDepartments();
			Dictionary<string, string> deptResults = new Dictionary<string, string>();

			foreach (var dept in departments)
			{
				if (!deptResults.ContainsKey(dept.DeptCode))
				{
					deptResults.Add(dept.DeptCode, dept.DeptName);
				}
			}

			return deptResults;
		}

	}
}