using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API.Entities.Configurations
{
	public class UserUserGroupEntityTypeConfiguration : IEntityTypeConfiguration<UserUserGroup>
	{
		public void Configure(EntityTypeBuilder<UserUserGroup> builder)
		{
			builder.HasKey(uar => new { uar.UserId, uar.UserGroupId });

			builder.HasOne(uug => uug.User)
				.WithMany(u => u.UserGroups)
				.HasForeignKey(uug => uug.UserId);

			builder.HasOne(uug => uug.UserGroup)
				.WithMany(ug => ug.Users)
				.HasForeignKey(uug => uug.UserGroupId);
		}
	}
}
