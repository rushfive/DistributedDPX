using System;
using System.Collections.Generic;
using System.Text;

namespace DistributedDPX.BuildingBlocks.EventBus.EventBusCore
{
	public class EventBusSettings
	{
		public string Connection { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public int? RetryCount { get; set; }
	}
}
