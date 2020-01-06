using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API.Entities.Configurations
{
	public class UserGroupEntityTypeConfiguration : IEntityTypeConfiguration<UserGroup>
	{
		public void Configure(EntityTypeBuilder<UserGroup> builder)
		{
			builder.HasKey(u => u.Id);

			builder.Property(u => u.Name)
				.IsRequired();
		}
	}
}
