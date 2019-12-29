using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Entities.Configurations
{
	public class UserAccessRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserAccessRole>
	{
		public void Configure(EntityTypeBuilder<UserAccessRole> builder)
		{
			builder.HasKey(uar => new { uar.UserId, uar.AccessRoleId });

			builder.HasOne(uar => uar.User)
				.WithMany(uar => uar.UserAccessRoles)
				.HasForeignKey(uar => uar.UserId);

			builder.HasOne(uar => uar.AccessRole)
				.WithMany(uar => uar.UserAccessRoles)
				.HasForeignKey(uar => uar.AccessRoleId);
		}
	}
}
