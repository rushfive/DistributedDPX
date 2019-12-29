using DistributedDPX.BuildingBlocks.EventBus.EventBusCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedDPX.Admin.UserManagement
{
	public class UserManagementSettings
	{
		public SerilogSettings Serilog { get; set; }
		public InsightsSettings ApplicationInsights { get; set; }
		public EventBusSettings EventBus { get; set; }
		public string SubscriptionClientName { get; set; }
		public int GRPC_PORT { get; set; }
		public int PORT { get; set; }
		public string SqlServerConnectionString { get; set; }
		public string PathBase { get; set; }
	}

	public class SerilogSettings
	{
		public string SeqServerUrl { get; set; }
		public string LogstashUrl { get; set; }
		public MinLevel MinimumLevel { get; set; }


		public class MinLevel
		{
			public string Default { get; set; }
			public OverrideLevels Override { get; set; }
		}

		public class OverrideLevels
		{
			public string Microsoft { get; set; }
			public string DistributedDPX { get; set; }
			public string System { get; set; }
		}
	}

	public class InsightsSettings
	{
		public string InstrumentationKey { get; set; }
	}
}
