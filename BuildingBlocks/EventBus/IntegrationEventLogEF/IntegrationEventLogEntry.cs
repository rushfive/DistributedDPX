using DistributedDPX.BuildingBlocks.EventBus.EventBusCore.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DistributedDPX.BuildingBlocks.EventBus.IntegrationEventLogEF
{
	[Table("IntegrationEventLog")]
	public class IntegrationEventLogEntry
	{
		private IntegrationEventLogEntry() { }
		public IntegrationEventLogEntry(IntegrationEvent @event, Guid transactionId)
		{
			EventId = @event.Id;
			CreationTime = @event.CreationDate;
			EventTypeName = @event.GetType().FullName;
			Content = JsonConvert.SerializeObject(@event);
			State = EventState.NotPublished;
			TimesSent = 0;
			TransactionId = transactionId.ToString();
		}
		public Guid EventId { get; private set; }
		public string EventTypeName { get; private set; }
		[NotMapped]
		public string EventTypeShortName => EventTypeName.Split('.')?.Last();
		[NotMapped]
		public IntegrationEvent IntegrationEvent { get; private set; }
		public EventState State { get; set; }
		public int TimesSent { get; set; }
		public DateTime CreationTime { get; private set; }
		public string Content { get; private set; }
		public string TransactionId { get; private set; }

		public IntegrationEventLogEntry DeserializeJsonContent(Type type)
		{
			IntegrationEvent = JsonConvert.DeserializeObject(Content, type) as IntegrationEvent;
			return this;
		}
	}

	public enum EventState
	{
		NotPublished = 0,
		InProgress = 1,
		Published = 2,
		PublishedFailed = 3
	}
}
