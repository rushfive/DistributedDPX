using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Entities
{
	[Table("User")]
	public class User
	{
		public Guid Id { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string MobilePhone { get; set; }
		public string Language { get; set; }
		public bool Enabled { get; set; }
		public bool Locked { get; set; }
		public DateTime CreatedDate { get; set; }

		public UserType Type { get; set; }
		//public List<AccessRole> AccessRoles { get; set; }
		public List<string> Campaigns { get; set; }
		public bool IsAdmin { get; set; }
		public bool EmptySiteAccess { get; set; }
		public bool AllSitesAccess { get; set; }
		public bool AllCampaignsAccess { get; set; }

		public IList<UserAccessRole> UserAccessRoles { get; set; }
		public IList<UserUserGroup> UserGroups { get; set; }
		//public IList<UserCampaign> UserCampaigns { get; set; }


		public string Name => $"{this.FirstName} {this.LastName}".Trim();
	}

	public enum UserType
	{
		EndUser = 0,
		UserAdmin = 1,
		PlatformAdmin = 2
	}
}
