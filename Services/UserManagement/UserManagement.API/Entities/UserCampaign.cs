using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API.Entities
{
	[Table("UserCampaign")]
	public class UserCampaign
	{
		public Guid UserId { get; set; }
		public User User { get; set; }
		public string CampaignId { get; set; }
	}
}
