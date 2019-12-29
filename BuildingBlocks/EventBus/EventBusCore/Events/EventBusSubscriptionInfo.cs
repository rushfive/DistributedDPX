using System;
using System.Collections.Generic;
using System.Text;

namespace DistributedDPX.BuildingBlocks.EventBus.EventBusCore.Events
{
	public class EventBusSubscriptionInfo
	{
		public bool IsDynamic { get; }
		public Type HandlerType { get; }

		private EventBusSubscriptionInfo(bool isDynamic, Type handlerType)
		{
			IsDynamic = isDynamic;
			HandlerType = handlerType;
		}

		public static EventBusSubscriptionInfo Dynamic(Type handlerType)
		{
			return new EventBusSubscriptionInfo(true, handlerType);
		}
		public static EventBusSubscriptionInfo Typed(Type handlerType)
		{
			return new EventBusSubscriptionInfo(false, handlerType);
		}
	}
}
