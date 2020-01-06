using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.Entities;
using UserManagement.API.Entities.Configurations;

namespace UserManagement.API.Infrastructure
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
			builder.ApplyConfiguration(new UserCampaignEntityTypeConfiguration()); 
		}
	}

	public class UserManagementContextDesignFactory : IDesignTimeDbContextFactory<UserManagementContext>
	{
		public UserManagementContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<UserManagementContext>()
				.UseSqlServer("Server=.;Initial Catalog=UserManagementDb;Integrated Security=true");

			return new UserManagementContext(optionsBuilder.Options);
		}
	}
}
