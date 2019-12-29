using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Entities
{
	public class UserAccessRole
	{
		public Guid UserId { get; set; }
		public User User { get; set; }
		public Guid AccessRoleId { get; set; }
		public AccessRole AccessRole { get; set; }
	}
}
