using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Entities
{
	[Table("UserUserGroup")]
	public class UserUserGroup
	{
		public Guid UserId { get; set; }
		public User User { get; set; }
		public Guid UserGroupId { get; set; }
		public UserGroup UserGroup { get; set; }
	}
}
