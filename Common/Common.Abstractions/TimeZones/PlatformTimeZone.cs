using Common.Abstractions.Dates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Abstractions.TimeZones
{
	public class PlatformTimeZone
	{
		public string Id { get; set; }
		private ITimeZoneHelper timeZoneOffsetAccessor { get; }

		public PlatformTimeZone(string id, ITimeZoneHelper timeZoneOffsetAccessor)
		{
			this.timeZoneOffsetAccessor = timeZoneOffsetAccessor;
			this.Id = this.timeZoneOffsetAccessor.GetCanonicalId(id);
		}

		private PlatformTimeZone(string id)
		{
			this.Id = id;
		}

		public PlatformTimeZone() { }

		public TimeSpan GetCurrentOffset()
		{
			return this.GetOffset(DateTimeOffset.UtcNow);
		}

		public string GetCurrentAbbreviation()
		{
			return this.timeZoneOffsetAccessor.GetAbbreviation(DateTimeOffset.UtcNow);
		}

		public TimeSpan GetOffset(DateTime dateTime)
		{
			//Always assume UTC
			dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
			return this.GetOffset(new DateTimeOffset(dateTime, TimeSpan.Zero));
		}

		public string GetAbbreviation(DateTimeOffset dateTime)
		{
			return this.timeZoneOffsetAccessor.GetAbbreviation(dateTime);
		}

		public TimeSpan GetOffset(DateTimeOffset dateTime)
		{
			return this.timeZoneOffsetAccessor.GetOffset(dateTime);
		}

		public TimeSpan GetOffset(DsDateTime dateTime)
		{
			return this.timeZoneOffsetAccessor.GetOffset(dateTime);
		}

		public override string ToString()
		{
			TimeSpan currentOffset = this.GetCurrentOffset();
			string hourOffset = currentOffset.Hours.ToString("+00;-00");
			string minutesOffset = currentOffset.Minutes.ToString("00");
			return $"({hourOffset}:{minutesOffset}) {this.Id}";
		}

		//public static PlatformTimeZone GetSystemTimeZone(LocalizationSettings localizationSettings)
		//{
		//	return new PlatformTimeZone(localizationSettings?.TimeZone?.Id);
		//}
	}
}
