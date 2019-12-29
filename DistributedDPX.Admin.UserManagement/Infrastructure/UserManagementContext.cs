using DistributedDPX.Admin.UserManagement.Entities;
using DistributedDPX.Admin.UserManagement.Entities.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Infrastructure
{
	public class UserManagementContext : DbContext
	{
		public UserManagementContext(DbContextOptions<UserManagementContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<UserGroup> UserGroups { get; set; }
		public DbSet<AccessRole> AccessRoles { get; set; }
		public DbSet<UserUserGroup> UserUserGroups { get; set; }
		public DbSet<UserAccessRole> UserAccessRoles { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.ApplyConfiguration(new UserEntityTypeConfiguration());
			builder.ApplyConfiguration(new UserGroupEntityTypeConfiguration());
			builder.ApplyConfiguration(new AccessRoleEntityTypeConfiguration());
			builder.ApplyConfiguration(new UserUserGroupEntityTypeConfiguration());
			builder.ApplyConfiguration(new UserAccessRoleEntityTypeConfiguration());
		}
	}
}
