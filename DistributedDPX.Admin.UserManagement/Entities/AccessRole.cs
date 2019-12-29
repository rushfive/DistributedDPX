using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Entities
{
	public class AccessRole
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public RolePermissions ParticipantWorkflows { get; set; }
		public RolePermissions Participants { get; set; }
		public RolePermissions SubmissionData { get; set; }
		public RolePermissions TestParticipantsAndData { get; set; }

		public IList<UserAccessRole> UserAccessRoles { get; set; }
	}

	public class RolePermissions
	{
		public bool View { get; set; }

		public bool Add { get; set; }

		public bool Delete { get; set; }

		public bool Modify { get; set; }

		public bool Export { get; set; }

		public string Serialize()
		{
			var tokens = new List<string>
			{
				$"V{(View ? "1" : "0")}",
				$"A{(Add ? "1" : "0")}",
				$"D{(Delete ? "1" : "0")}",
				$"M{(Modify ? "1" : "0")}",
				$"E{(Export ? "1" : "0")}"
			};

			return string.Join('|', tokens);
		}

		public static RolePermissions Deserialize(string serialized)
		{
			var permissions = new RolePermissions();
			var tokens = serialized.Split('|');

			foreach(var t in tokens)
			{
				var prop = t[0];
				bool value = t[1] == '1' ? true : false;

				switch (prop)
				{
					case 'V':
						permissions.View = value;
						break;
					case 'A':
						permissions.Add = value;
						break;
					case 'D':
						permissions.Delete = value;
						break;
					case 'M':
						permissions.Modify = value;
						break;
					case 'E':
						permissions.Export= value;
						break;
					default:
						throw new InvalidOperationException($"'{prop}' doesn't match a property for RolePermissions.");
				}
			}

			return permissions;
		}
	}
}
