using DistributedDPX.Admin.UserManagement.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement.Infrastructure
{
	public class UserManagementContextSeed
	{
		public async Task SeedAsync(UserManagementContext context, IWebHostEnvironment env, 
			IOptions<UserManagementSettings> settings, ILogger<UserManagementContextSeed> logger)
		{
			AsyncRetryPolicy policy = CreatePolicy(logger, nameof(UserManagementContextSeed));

			await policy.ExecuteAsync(async () =>
			{
				var useCustomizationData = settings.Value.UseCustomizationData;
				var contentRootPath = env.ContentRootPath;
				var picturePath = env.WebRootPath;

				if (!context.Users.Any())
				{
					List<User> users = GetUsers();
					await context.Users.AddRangeAsync(users);

					List<UserGroup> groups = GetUserGroups();
					await context.UserGroups.AddRangeAsync(groups);

					List<AccessRole> roles = GetAccessRoles();
					await context.AccessRoles.AddRangeAsync(roles);

					User supportUser = users[0];
					User testUser = users[1];

					UserGroup administratorsGroup = groups[0];
					UserGroup testersGroup = groups[1];

					AccessRole powerUserRole = roles[0];
					AccessRole testerRole = roles[1];

					var supportUserRole = new UserAccessRole
					{
						UserId = supportUser.Id,
						User = supportUser,
						AccessRoleId = powerUserRole.Id,
						AccessRole = powerUserRole
					};

					var testUserRole = new UserAccessRole
					{
						UserId = testUser.Id,
						User = testUser,
						AccessRoleId = testerRole.Id,
						AccessRole = testerRole
					};

					await context.UserAccessRoles.AddRangeAsync(supportUserRole, testUserRole);

					var supportUserGroup = new UserUserGroup
					{
						UserId = supportUser.Id,
						User = supportUser,
						UserGroupId = administratorsGroup.Id,
						UserGroup = administratorsGroup
					};

					var testUserGroup = new UserUserGroup
					{
						UserId = testUser.Id,
						User = testUser,
						UserGroupId = testersGroup.Id,
						UserGroup = testersGroup
					};

					await context.UserUserGroups.AddRangeAsync(supportUserGroup, testUserGroup);

					await context.SaveChangesAsync();
				}
			});
		}

		private static List<AccessRole> GetAccessRoles()
		{
			return new List<AccessRole>
			{
				new AccessRole
				{
					Id = Guid.NewGuid(),
					Name = "Power User",
					ParticipantWorkflows = new RolePermissions
					{
						View = true,
						Add = true,
						Delete = true,
						Modify = true,
						Export = true
					},
					Participants = new RolePermissions
					{
						View = true,
						Add = true,
						Delete = true,
						Modify = true,
						Export = true
					},
					SubmissionData = new RolePermissions
					{
						View = true,
						Add = true,
						Delete = true,
						Modify = true,
						Export = true
					},
					TestParticipantsAndData = new RolePermissions
					{
						View = true,
						Add = false,
						Delete = false,
						Modify = false,
						Export = false
					}
				},
				new AccessRole
				{
					Id = Guid.NewGuid(),
					Name = "Tester",
					ParticipantWorkflows = new RolePermissions
					{
						View = false,
						Add = false,
						Delete = false,
						Modify = false,
						Export = false
					},
					Participants = new RolePermissions
					{
						View = false,
						Add = false,
						Delete = false,
						Modify = false,
						Export = false
					},
					SubmissionData = new RolePermissions
					{
						View = false,
						Add = false,
						Delete = false,
						Modify = false,
						Export = false
					},
					TestParticipantsAndData = new RolePermissions
					{
						View = true,
						Add = true,
						Delete = true,
						Modify = true,
						Export = true
					}
				}
			};
		}

		private static List<User> GetUsers()
		{
			return new List<User>
			{
				new User
				{
					Id = Guid.NewGuid(),
					UserName = "support@scisolutions.com",
					FirstName = "SCI Support",
					Enabled = true,
					Locked = false,
					CreatedDate = DateTime.UtcNow,
					Type = UserType.PlatformAdmin,
					Campaigns = new List<string>(),
					IsAdmin = true,
					EmptySiteAccess = true,
					AllSitesAccess = true,
					AllCampaignsAccess = true
				},
				new User
				{
					Id = Guid.NewGuid(),
					UserName = "test_user@scisolutions.com",
					FirstName = "Test User",
					Enabled = true,
					Locked = false,
					CreatedDate = DateTime.UtcNow,
					Type = UserType.EndUser,
					Campaigns = new List<string>(),
					IsAdmin = false,
					EmptySiteAccess = false,
					AllSitesAccess = false,
					AllCampaignsAccess = false
				}
			};
		}

		private static List<UserGroup> GetUserGroups()
		{
			return new List<UserGroup>
			{
				new UserGroup
				{
					Id = Guid.NewGuid(),
					Name = "Administrators"
				},
				new UserGroup
				{
					Id = Guid.NewGuid(),
					Name = "Testers"
				}
			};
		}


		private AsyncRetryPolicy CreatePolicy(ILogger<UserManagementContextSeed> logger, string prefix, int retries = 3)
		{
			return Policy.Handle<SqlException>().
				WaitAndRetryAsync(
					retryCount: retries,
					sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
					onRetry: (exception, timeSpan, retry, ctx) =>
					{
						logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
					}
				);
		}
	}
}
