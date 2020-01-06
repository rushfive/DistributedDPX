//using DistributedDPX.Admin.UserManagement.Infrastructure;
//using DistributedDPX.BuildingBlocks.EventBus.EventBusCore.Abstractions;
//using DistributedDPX.BuildingBlocks.EventBus.EventBusCore.Events;
//using DistributedDPX.BuildingBlocks.EventBus.IntegrationEventLogEF.Services;
//using DistributedDPX.BuildingBlocks.EventBus.IntegrationEventLogEF.Utilities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Threading.Tasks;

//namespace UserManagement.API.IntegrationEvents
//{
//	public interface IUserManagementEventService
//	{
//		Task SaveEventAndContextChangesAsync(IntegrationEvent @event);
//		Task PublishThroughEventBusAsync(IntegrationEvent @event);
//	}

//	public class UserManagementEventService : IUserManagementEventService
//	{
//		private readonly ILogger<UserManagementEventService> _logger;
//		private readonly IEventBus _eventBus;
//		private readonly UserManagementContext _dbContext;
//		private readonly Func<DbConnection, IIntegrationEventLogService> _eventLogServiceFactory;
//		private readonly IIntegrationEventLogService _eventLogService;

//		public UserManagementEventService(
//			ILogger<UserManagementEventService> logger,
//			IEventBus eventBus,
//			UserManagementContext dbContext,
//			Func<DbConnection, IIntegrationEventLogService> eventLogServiceFactory)
//		{
//			_logger = logger;
//			_dbContext = dbContext;
//			_eventLogServiceFactory = eventLogServiceFactory;
//			_eventBus = eventBus;
//			_eventLogService = _eventLogServiceFactory(_dbContext.Database.GetDbConnection());
//		}

		

//		public async Task SaveEventAndContextChangesAsync(IntegrationEvent @event)
//		{
//			_logger.LogInformation("----- CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}", @event.Id);

//			//Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
//			//See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
//			await ResilientTransaction.New(_dbContext).ExecuteAsync(async () =>
//			{
//				// Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
//				await _dbContext.SaveChangesAsync();
//				await _eventLogService.SaveEventAsync(@event, _dbContext.Database.CurrentTransaction);
//			});
//		}

//		public async Task PublishThroughEventBusAsync(IntegrationEvent @event)
//		{
//			try
//			{
//				_logger.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

//				await _eventLogService.MarkEventAsInProgressAsync(@event.Id);
//				_eventBus.Publish(@event);
//				await _eventLogService.MarkEventAsPublishedAsync(@event.Id);
//			}
//			catch (Exception ex)
//			{
//				_logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
//				await _eventLogService.MarkEventAsFailedAsync(@event.Id);
//			}
//		}
//	}
//}
