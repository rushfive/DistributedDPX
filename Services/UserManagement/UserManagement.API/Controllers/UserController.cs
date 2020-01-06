using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UserManagement.API.Entities;
using UserManagement.API.Infrastructure;
using UserManagement.API.Models;

namespace UserManagement.API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly UserManagementContext _dbContext;

		public UserController(
			UserManagementContext dbContext)
		{
			_dbContext = dbContext;
		}


		[HttpGet]
		[Route("Users")]
		[ProducesResponseType(typeof(PaginatedItemsModel<User>), (int)HttpStatusCode.OK)]
		public async Task<IActionResult> Users([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
		{
			long totalCount = await _dbContext.Users.LongCountAsync();

			List<User> users = await _dbContext.Users
				.OrderBy(u => u.FirstName)
				.Skip(pageSize * pageIndex)
				.Take(pageSize)
				.ToListAsync();

			var model = new PaginatedItemsModel<User>(pageIndex, pageSize, totalCount, users);

			return Ok(model);
		}
	}
}
