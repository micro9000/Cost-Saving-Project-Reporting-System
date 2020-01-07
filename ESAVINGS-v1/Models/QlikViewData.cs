using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESAVINGS_v1.Models
{
	public class QlikViewData
	{
		private int financeCategoryId;

		public int FinanceCategoryId
		{
			get
			{
				return financeCategoryId;
			}
			set
			{
				financeCategoryId = value;
			}
		}


		private DateTime originalDueDate;

		public DateTime OriginalDueDate
		{
			get
			{
				return originalDueDate;
			}
			set
			{
				originalDueDate = value;
			}
		}

		private DateTime currentDueDate;

		public DateTime CurrentDueDate
		{
			get
			{
				return currentDueDate;
			}
			set
			{
				currentDueDate = value;
			}
		}

		private DateTime plannedProjectStartDate;

		public DateTime PlannedProjectStartDate
		{
			get
			{
				return plannedProjectStartDate;
			}
			set
			{
				plannedProjectStartDate = value;
			}
		}


		private DateTime plannedSavingStartDate;

		public DateTime PlannedSavingStartDate
		{
			get
			{
				return plannedSavingStartDate;
			}
			set
			{
				plannedSavingStartDate = value;
			}
		}

		private DateTime actualCompletionDate;

		public DateTime ActualCompletionDate
		{
			get
			{
				return actualCompletionDate;
			}
			set
			{
				actualCompletionDate = value;
			}
		}

		private int globalFunnelStatusIndicator;

		public int GlobalFunnelStatusIndicator
		{
			get
			{
				return globalFunnelStatusIndicator;
			}
			set
			{
				globalFunnelStatusIndicator = value;
			}
		}



	}
}