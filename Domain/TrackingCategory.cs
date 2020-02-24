using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;
using System.Globalization;

namespace Domain
{
	[Table("TrackingCategories")]
	public class TrackingCategory
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


		private int isDeleted = 0;

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
	public class TrackingCategoryMapper : ClassMapper<TrackingCategory>
	{
		public TrackingCategoryMapper ()
		{
			Table("TrackingCategories");
			//Map(p => p.ProjectTypeIndicator).Ignore();
			AutoMap();
		}
	}
}

