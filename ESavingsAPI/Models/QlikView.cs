using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESavingsAPI.Models
{
	public class QlikView
	{
		public string site
		{
			get;
			set;
		}

		public string projectTitle
		{
			get;
			set;
		}

		public string benefitType
		{
			get;
			set;
		}

		public decimal amount
		{
			get;
			set;
		}

		public decimal monthlyDollarAmount
		{
			get;
			set;
		}

		public int numberOfMonthsToBeActive
		{
			get;
			set;
		}

		public string projectStatus
		{
			get;
			set;
		}

		public int rank
		{
			get;
			set;
		}

		public string financeCategory
		{
			get;
			set;
		}

		public string notes
		{
			get;
			set;
		}

		public string funnelStatus
		{
			get;
			set;
		}

		public string description
		{
			get;
			set;
		}

		public string functionalArea
		{
			get;
			set;
		}

		public string owner
		{
			get;
			set;
		}

		public string originalDueDate
		{
			get;
			set;
		}

		public string currentDueDate
		{
			get;
			set;
		}

		public string plannedProjectStartDate
		{
			get;
			set;
		}

		public string plannedSavingStartDate
		{
			get;
			set;
		}

		public string actualCompletionDate
		{
			get;
			set;
		}

		public decimal actualAmount
		{
			get;
			set;
		}
	}
}