using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API.Entities
{
	[Table("UserAccessRole")]
	public class UserAccessRole
	{
		public Guid UserId { get; set; }
		public User User { get; set; }
		public Guid AccessRoleId { get; set; }
		public AccessRole AccessRole { get; set; }
	}
}
