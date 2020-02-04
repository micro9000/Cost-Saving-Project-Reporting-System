using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace ESAVINGS_v1.Models
{
	public class UserSession
	{
		public string ffid
		{
			get;
			set;
		}
		public string fullName
		{
			get;
			set;
		}
		public string email
		{
			get;
			set;
		}

		public string department
		{
			get;
			set;
		}

		public StaticData.UserTypes type
		{
			get;
			set;
		}

		public bool isMaster
		{
			get;
			set;
		}

		public bool isMultiPermission
		{
			get;
			set;
		}

		public string mgrFFID
		{
			get;
			set;
		}

		private string _password = "";
		public string password
		{
			get
			{
				return this._password;
			}

			set
			{
				this._password = value;
			}
		}

		private string costAnalystID;

		public string CostAnalystID
		{
			get
			{
				return costAnalystID;
			}
			set
			{
				costAnalystID = value;
			}
		}

		private string financeID;

		public string FinanceID
		{
			get
			{
				return financeID;
			}
			set
			{
				financeID = value;
			}
		}

		private bool isDL;

		public bool IsDL
		{
			get
			{
				return isDL;
			}
			set
			{
				isDL = value;
			}
		}


	}
}