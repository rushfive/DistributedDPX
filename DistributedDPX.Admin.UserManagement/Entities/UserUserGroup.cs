using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Entities
{
	public class UserUserGroup
	{
		public Guid UserId { get; set; }
		public User User { get; set; }
		public Guid UserGroupId { get; set; }
		public UserGroup UserGroup { get; set; }
	}
}
