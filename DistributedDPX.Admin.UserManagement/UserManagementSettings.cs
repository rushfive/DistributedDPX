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
		public string EventBusConnection { get; set; }
		public int EventBusRetryCount { get; set; }
		public int GRPC_PORT { get; set; }
		public int PORT { get; set; }
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
