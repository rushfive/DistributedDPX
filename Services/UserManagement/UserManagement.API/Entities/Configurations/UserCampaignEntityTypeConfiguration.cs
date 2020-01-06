using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API.Entities.Configurations
{
	public class UserCampaignEntityTypeConfiguration : IEntityTypeConfiguration<UserCampaign>
	{
		public void Configure(EntityTypeBuilder<UserCampaign> builder)
		{
			builder.HasKey(uc => new { uc.UserId, uc.CampaignId });

			builder.HasOne(uc => uc.User)
				.WithMany(u => u.UserCampaigns)
				.HasForeignKey(uc => uc.UserId);
		}
	}
}
