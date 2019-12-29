using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Entities.Configurations
{
	public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(u => u.Id);

			builder.Property(u => u.UserName)
				.IsRequired();

			builder.Property(u => u.FirstName)
				.IsRequired();

			builder.Property(u => u.LastName)
				.IsRequired(false);

			builder.Property(u => u.MobilePhone)
				.IsRequired(false);

			builder.Property(u => u.Language)
				.IsRequired(false);

			builder.Property(u => u.Enabled)
				.IsRequired();

			builder.Property(u => u.Locked)
				.IsRequired();

			builder.Property(u => u.CreatedDate)
				.IsRequired();

			builder.Property(u => u.Type)
				.IsRequired()
				.HasConversion(
					v => v.ToString(),
					v => Enum.Parse<UserType>(v));

			builder.Property(u => u.Campaigns)
				.IsRequired()
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<List<string>>(v));

			builder.Property(u => u.IsAdmin)
				.IsRequired();

			builder.Property(u => u.EmptySiteAccess)
				.IsRequired();

			builder.Property(u => u.AllSitesAccess)
				.IsRequired();

			builder.Property(u => u.AllCampaignsAccess)
				.IsRequired();
		}
	}
}
