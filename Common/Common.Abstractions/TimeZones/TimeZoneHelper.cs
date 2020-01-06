using Common.Abstractions.Dates;
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Abstractions.TimeZones
{
	public interface ITimeZoneHelper
	{
		string GetCanonicalId(string id);
		string GetAbbreviation(DateTimeOffset dateTime);
		TimeSpan GetOffset(DateTimeOffset dateTime);
		TimeSpan GetOffset(DsDateTime dateTime);
	}

	public class NodaTimeZoneHelper : ITimeZoneHelper
	{
		private DateTimeZone timeZone { get; }

		public NodaTimeZoneHelper(DateTimeZone timeZone)
		{
			this.timeZone = timeZone;
		}

		public string GetCanonicalId(string id)
		{
			if (!TzdbDateTimeZoneSource.Default.CanonicalIdMap.TryGetValue(id, out string canonicalId))
			{
				return id;
			}
			return canonicalId;
		}

		public string GetAbbreviation(DateTimeOffset dateTime)
		{
			return this.timeZone.GetZoneInterval(Instant.FromDateTimeOffset(dateTime)).Name;
		}

		public TimeSpan GetOffset(DateTimeOffset dateTime)
		{
			return this.timeZone.GetUtcOffset(Instant.FromDateTimeOffset(dateTime)).ToTimeSpan();
		}

		public TimeSpan GetOffset(DsDateTime dateTime)
		{
			if (this.timeZone.MinOffset == this.timeZone.MaxOffset)
			{
				return this.timeZone.MinOffset.ToTimeSpan();
			}
			DateTimeOffset min = GetDateTime(this.timeZone.MinOffset);
			DateTimeOffset max = GetDateTime(this.timeZone.MaxOffset);
			if (max < min)
			{
				DateTimeOffset temp = min;
				min = max;
				max = temp;
			}
			List<ZoneInterval> zones = this.timeZone
				.GetZoneIntervals(Instant.FromDateTimeOffset(min), Instant.FromDateTimeOffset(max))
				.ToList();
			if (!zones.Any())
			{
				//??TODO
				return this.timeZone.MinOffset.ToTimeSpan();
			}
			if (zones.Count == 1)
			{
				return zones.First().WallOffset.ToTimeSpan();
			}
			//??TODO its just an hour difference, not sure what to do, better early than late
#if DEBUG
			throw new Exception($"Multiple time zone intervals found for date + time: {dateTime}");
#else
			return zones.First().WallOffset.ToTimeSpan();
#endif

			DateTimeOffset GetDateTime(Offset offset)
			{
				TimeSpan offsetTimeSpan = offset.ToTimeSpan();
				//Make sure any weird offset is only to minutes
				if (offsetTimeSpan.Seconds != 0)
				{
					offsetTimeSpan = offsetTimeSpan.Subtract(TimeSpan.FromSeconds(offsetTimeSpan.Seconds));
				}
				if (offsetTimeSpan.Milliseconds != 0)
				{
					offsetTimeSpan = offsetTimeSpan.Subtract(TimeSpan.FromMilliseconds(offsetTimeSpan.Milliseconds));
				}
				return new DateTimeOffset(dateTime.Date.Year, dateTime.Date.Month, dateTime.Date.Day, dateTime.Time.Hour, dateTime.Time.Minutes, dateTime.Time.Seconds, offsetTimeSpan);
			}
		}
	}
}
