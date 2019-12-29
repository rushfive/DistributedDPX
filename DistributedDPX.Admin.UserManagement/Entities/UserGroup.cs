using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Entities
{
	public class UserGroup
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public IList<UserUserGroup> Users { get; set; }
		//public List<User> Users { get; set; } = new List<User>();
	}
}
