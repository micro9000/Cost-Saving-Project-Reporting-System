using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
	public class Validator
	{
		private int id;

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

		private string ffID;

		public string FFID
		{
			get
			{
				return ffID;
			}
			set
			{
				ffID = value;
			}
		}

		private string validatorType;

		public string ValidatorType
		{
			get
			{
				return validatorType;
			}
			set
			{
				validatorType = value;
			}
		}
	}
}
