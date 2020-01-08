using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DapperExtensions.Mapper;
using System.Globalization;

namespace Domain
{
	[Table("OwnerAwards")]
	public class OwnerAward
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


		private int proposalID;

		public int ProposalID
		{
			get
			{
				return proposalID;
			}
			set
			{
				proposalID = value;
			}
		}


		private string userFFID;

		public string UserFFID
		{
			get
			{
				return userFFID;
			}
			set
			{
				userFFID = value;
			}
		}

		private string userName;

		public string Username
		{
			get
			{
				return userName;
			}
			set
			{
				userName = value;
			}
		}


		private int oaStatus;

		public int OAStatus
		{
			get
			{
				return oaStatus;
			}
			set
			{
				oaStatus = value;
			}
		}

		private StaticData.RewardType rewardType;

		public StaticData.RewardType RewardType
		{
			get
			{
				return rewardType;
			}
			set
			{
				rewardType = value;
			}
		}

		public string RewardTypeStr
		{
			get
			{
				return StaticData.GetRewardTypeStr((int)this.RewardType);
			}
		}

		private decimal rewardCash;

		public decimal RewardCash
		{
			get
			{
				return rewardCash;
			}
			set
			{
				rewardCash = value;
			}
		}

		private string claimTo;

		public string ClaimTo
		{
			get
			{
				return claimTo;
			}
			set
			{
				claimTo = value;
			}
		}


		private int isAwarded;

		public int IsAwarded
		{
			get
			{
				return isAwarded;
			}
			set
			{
				isAwarded = value;
			}
		}



		private DateTime dateAwarded = new DateTime(0001, 01, 01);

		public DateTime DateAwarded
		{
			get
			{
				return dateAwarded;
			}
			set
			{
				dateAwarded = value;
			}
		}

		private string awardedBy;

		public string AwardedBy
		{
			get
			{
				return awardedBy;
			}
			set
			{
				awardedBy = value;
			}
		}

		private string remarks;

		public string Remarks
		{
			get
			{
				return remarks;
			}
			set
			{
				remarks = value;
			}
		}


		private DateTime createdAt = DateTime.UtcNow;

		public DateTime CreatedAt
		{
			get
			{
				return createdAt;
			}
			set
			{
				createdAt = value;
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

	public class OwnerAwardMapper : ClassMapper<OwnerAward>
	{
		public OwnerAwardMapper ()
		{
			Table("OwnerAwards");
			//Map(p => p.ProjectTypeIndicator).Ignore();
			AutoMap();
		}
	}
}
