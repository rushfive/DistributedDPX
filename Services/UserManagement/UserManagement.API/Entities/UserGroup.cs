using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API.Entities
{
	[Table("UserGroup")]
	public class UserGroup
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public IList<UserUserGroup> Users { get; set; }
		//public List<User> Users { get; set; } = new List<User>();
	}
}
