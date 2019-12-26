using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;

namespace Domain
{
	[Table("FinanceCategories")]
	public class FinanceCategory
	{
		private int id;
		[Dapper.Contrib.Extensions.Key]
		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}


		private string category;

		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
			}
		}


		private int isDeleted;

		public int IsDeleted
		{
			get
			{
				return isDeleted;
			}
			set
			{
				isDeleted = value;
			}
		}


	}


	public class FinanceCategoryMapper : ClassMapper<FinanceCategory>
	{
		public FinanceCategoryMapper ()
		{
			Table("FinanceCategories");
			AutoMap();
		}
	}
}
