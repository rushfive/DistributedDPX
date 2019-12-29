using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Entities.Configurations
{
	public class AccessRoleEntityTypeConfiguration : IEntityTypeConfiguration<AccessRole>
	{
		public void Configure(EntityTypeBuilder<AccessRole> builder)
		{
			builder.HasKey(ar => ar.Id);

			builder.Property(u => u.Name)
				.IsRequired();

			builder.Property(ar => ar.ParticipantWorkflows)
				.IsRequired()
				.HasConversion(
					v => v.Serialize(),
					v => RolePermissions.Deserialize(v));

			builder.Property(ar => ar.Participants)
				.IsRequired()
				.HasConversion(
					v => v.Serialize(),
					v => RolePermissions.Deserialize(v));

			builder.Property(ar => ar.SubmissionData)
				.IsRequired()
				.HasConversion(
					v => v.Serialize(),
					v => RolePermissions.Deserialize(v));

			builder.Property(ar => ar.TestParticipantsAndData)
				.IsRequired()
				.HasConversion(
					v => v.Serialize(),
					v => RolePermissions.Deserialize(v));
		}
	}
}
