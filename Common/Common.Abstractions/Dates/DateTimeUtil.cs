using Common.Abstractions.TimeZones;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Common.Abstractions.Dates
{
	public static class DateTimeUtil
	{
		private static readonly Dictionary<string, string> customDateTimes = new Dictionary<string, string>();

		public enum DateTimeFormat
		{
			DateNumeric,
			Date,
			DateShort,
			DateTimeNumeric,
			DateTimeNumericWithoutSeconds,
			DateTime,
			DateTimeShort,
			DateTimeWithoutSeconds,
			DateTimeWithDay,
			DateTimeWithDayWithoutSeconds,
			TimeWithoutSeconds,
			Time,
			Iso,
			Sortable,
			Rfc1123,
			UniversalSortable,
			Default,
			SortableDate,
			SortableTime,
			SpssDateTime,
			SasDateTime,
			SasDate,
			TimeWithMilli
		}

		public static string FormatDateTime(DateTimeOffset dateTime, string formatString, string textForEmptyDate = "")
		{
			//TODO current info configurable?
			DateTimeFormatInfo formatInfo = DateTimeFormatInfo.CurrentInfo;
			if (dateTime == DateTime.MinValue)
			{
				return textForEmptyDate;
			}
			return dateTime.ToString(formatString, formatInfo);
		}

		public static string FormatDateTime(DateTimeOffset dateTime, DateTimeFormat format = DateTimeFormat.Default,
			DateTimeFormatInfo formatInfo = null, string textForEmptyDate = "")
		{
			if (formatInfo == null)
			{
				//todo format not based off the machine?
				formatInfo = DateTimeFormatInfo.CurrentInfo;
			}
			if (format == DateTimeFormat.Default)
			{
				if (dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0)
				{
					format = DateTimeFormat.DateShort;
				}
				else
				{
					format = DateTimeFormat.DateTimeShort;
				}
			}
			if (dateTime == DateTime.MinValue)
			{
				return textForEmptyDate;
			}
			return dateTime.ToString(DateTimeUtil.GetDateTimeFormat(format, formatInfo));
		}

		public static string GetDateTimeFormat(DateTimeFormat format, DateTimeFormatInfo formatInfo = null)
		{
			if (formatInfo == null)
			{
				//todo format not based off the machine?
				formatInfo = DateTimeFormatInfo.CurrentInfo;
			}

			string formatHash = (int)format + formatInfo.LongDatePattern;

			string dateTime;
			lock (DateTimeUtil.customDateTimes)
			{

				if (DateTimeUtil.customDateTimes.ContainsKey(formatHash))
				{
					return DateTimeUtil.customDateTimes[formatHash];
				}

				switch (format)
				{
					case DateTimeFormat.DateTimeWithoutSeconds:
						dateTime = DateTimeUtil.RemoveDayOfWeek(formatInfo.ShortDatePattern);
						dateTime += " " + formatInfo.ShortTimePattern;
						dateTime = dateTime.Replace("MMMM", "MMM");
						break;

					case DateTimeFormat.DateShort:
						dateTime = DateTimeUtil.RemoveDayOfWeek(formatInfo.LongDatePattern);
						dateTime = dateTime.Replace("MMMM", "MMM");
						break;

					case DateTimeFormat.DateTime:
						dateTime = DateTimeUtil.RemoveDayOfWeek(formatInfo.FullDateTimePattern);
						dateTime = dateTime.Replace("MMMM", "MMM");
						break;

					case DateTimeFormat.Default:
					case DateTimeFormat.DateTimeShort:
						dateTime = DateTimeUtil.GetDateTimeFormat(DateTimeFormat.DateShort, formatInfo) + " " +
									formatInfo.ShortTimePattern;
						break;

					case DateTimeFormat.DateNumeric:
						dateTime = "d";
						break;

					case DateTimeFormat.DateTimeNumeric:
						dateTime = "G";
						break;

					case DateTimeFormat.DateTimeNumericWithoutSeconds:
						dateTime = "g";
						break;

					case DateTimeFormat.Date:
						dateTime = "D";
						break;

					case DateTimeFormat.TimeWithoutSeconds:
						dateTime = "t";
						break;

					case DateTimeFormat.Time:
						dateTime = "T";
						break;

					case DateTimeFormat.Iso:
						dateTime = "o";
						break;

					case DateTimeFormat.Sortable:
						dateTime = "s";
						break;

					case DateTimeFormat.SortableDate:
						dateTime = "yyyy-MM-dd";
						break;

					case DateTimeFormat.SortableTime:
						dateTime = "HH:mm:ss";
						break;

					case DateTimeFormat.TimeWithMilli:
						dateTime = "HH:mm:ss.fff";
						break;

					case DateTimeFormat.Rfc1123:
						dateTime = "r";
						break;

					case DateTimeFormat.UniversalSortable:
						dateTime = "u";
						break;

					case DateTimeFormat.DateTimeWithDay:
						dateTime = "F";
						break;

					case DateTimeFormat.DateTimeWithDayWithoutSeconds:
						dateTime = "f";
						break;

					case DateTimeFormat.SpssDateTime:
						dateTime = "dd/MM/yyyy HH:mm:ss";
						break;

					case DateTimeFormat.SasDate:
						dateTime = "yyyyMMdd";
						break;

					case DateTimeFormat.SasDateTime:
						dateTime = "yyyy-MM-dd HH:mm:ss";
						break;

					default:
						throw new ArgumentException("No date time format found");
				}
				DateTimeUtil.customDateTimes.Add(formatHash, dateTime);
			}
			return dateTime;
		}

		private static string RemoveDayOfWeek(string dateTime)
		{
			if (!dateTime.StartsWith("dddd"))
			{
				return dateTime;
			}
			int index = dateTime.IndexOf(" ", StringComparison.Ordinal) + 1;
			return dateTime.Substring(index);
		}

		public static DateTime TruncateToSeconds(this DateTime dateTime)
		{
			return dateTime.Truncate(TimeSpan.FromSeconds(1));
		}

		public static DateTime TruncateToMinutes(this DateTime dateTime)
		{
			return dateTime.Truncate(TimeSpan.FromMinutes(1));
		}

		public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
		{
			if (timeSpan == TimeSpan.Zero)
			{
				return dateTime;
			}
			return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
		}

		public static DateTime CalculateDateFromUnits(DateType type, int timeUnits, DateTime baseDate, PlatformTimeZone timeZone, bool? weekdayOnly = false)
		{
			DsDateTime dsBaseDate = DsDateTime.FromDateTime(baseDate, timeZone);
			return DateTimeUtil.CalculateDateFromUnits(type, timeUnits, dsBaseDate, weekdayOnly).ToDateTimeOffset(timeZone).UtcDateTime;
		}

		public static DsDateTime CalculateDateFromUnits(DateType type, int timeUnits, DsDateTime baseDate, bool? weekdayOnly = false)
		{
			switch (type)
			{
				case DateType.Minutes:
					baseDate = baseDate.AddMinutes(timeUnits);
					break;
				case DateType.Hours:
					baseDate = baseDate.AddHours(timeUnits);
					break;
				case DateType.Days:
					baseDate = baseDate.AddDays(timeUnits);
					break;
				case DateType.Weeks:
					baseDate = baseDate.AddDays(7 * timeUnits);
					break;
				case DateType.Months:
					baseDate = baseDate.AddMonths(timeUnits);
					break;
				case DateType.Years:
					baseDate = baseDate.AddYears(timeUnits);
					break;
				default:
					throw new Exception();
			}

			if (!weekdayOnly.HasValue || !weekdayOnly.Value)
			{
				return baseDate;
			}

			switch (baseDate.GetDayOfWeek())
			{
				case DayOfWeek.Saturday:
				case DayOfWeek.Sunday:
					return baseDate.RoundToDayOfWeek(DayOfWeek.Monday);
				default:
					return baseDate;
			}
		}

		public static DateTimeOffset ConvertToTime(this DateTimeOffset dateTime, bool truncateToMinutes = false)
		{
			return truncateToMinutes
				? new DateTimeOffset(1, 1, 1, dateTime.Hour, dateTime.Minute, 0, dateTime.Offset)
				: new DateTimeOffset(1, 1, 1, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Offset);
		}

		public static DateTimeOffset RoundToDayOfWeek(this DateTimeOffset dt, DayOfWeek dayOfWeek, bool onlyDate = true, bool roundDown = false)
		{
			int diff;
			if (roundDown)
			{
				diff = dt.DayOfWeek - dayOfWeek;
			}
			else
			{
				diff = dayOfWeek - dt.DayOfWeek;
			}
			if (diff < 0)
			{
				diff += 7;
			}
			if (onlyDate)
			{
				dt = new DateTimeOffset(dt.Date, dt.Offset);
			}
			if (roundDown)
			{
				diff = -diff;
			}
			return dt.AddDays(diff);
		}

		public static DateTime RoundToDayOfWeek(this DateTime dt, DayOfWeek dayOfWeek, bool onlyDate = true, bool roundDown = false)
		{
			return new DateTimeOffset(dt).RoundToDayOfWeek(dayOfWeek, onlyDate, roundDown).DateTime;
		}

		public static bool AreEqualWithSpanOffset(DateTime dateTime, DateTime dateTimeToOffset, int spanValue, DateType timeUnit)
		{
			DateTime nowParticipantWithQueryOffset;
			switch (timeUnit)
			{
				case DateType.Minutes:
					nowParticipantWithQueryOffset = dateTimeToOffset.AddMinutes(spanValue);
					break;
				case DateType.Hours:
					nowParticipantWithQueryOffset = dateTimeToOffset.AddHours(spanValue);
					break;
				case DateType.Days:
					nowParticipantWithQueryOffset = dateTimeToOffset.AddDays(spanValue);
					break;
				case DateType.Weeks:
					nowParticipantWithQueryOffset = dateTimeToOffset.AddDays(spanValue * 7);
					break;
				case DateType.Months:
					nowParticipantWithQueryOffset = dateTimeToOffset.AddMonths(spanValue);
					break;
				case DateType.Years:
					nowParticipantWithQueryOffset = dateTimeToOffset.AddYears(spanValue);
					break;
				default:
					throw new Exception();
			}

			TimeSpan queryOffset = (nowParticipantWithQueryOffset - dateTimeToOffset).Duration();
			if (queryOffset < TimeSpan.FromHours(1))
			{
				return dateTime.TruncateToMinutes() == nowParticipantWithQueryOffset.TruncateToMinutes();
			}
			else if (queryOffset < TimeSpan.FromDays(1))
			{
				return dateTime.Truncate(TimeSpan.FromHours(1)) == nowParticipantWithQueryOffset.Truncate(TimeSpan.FromHours(1));
			}
			else
			{
				return dateTime.Truncate(TimeSpan.FromDays(1)) == nowParticipantWithQueryOffset.Truncate(TimeSpan.FromDays(1));
			}

		}

		public static string Format(this DsDate d, string format = "MMMM d, yyyy")
		{
			return d.ToDateTime()
				.Format(format);
		}

		public static string Format(this DsTime t, string format = "h:mm tt")
		{
			var d = new DsDate(1, 1, 2000);
			return new DsDateTime(d, t)
				.ToDateTimeOffset()
				.ToString(format);
		}

		public static string Format(this DateTime dt, PlatformTimeZone tz, string format = "MMMM d, yyyy h:mm tt")
		{
			return new DateTimeOffset(dt)
				.ToOffset(tz.GetOffset(dt))
				.ToString(format);
		}

		public static string Format(this DateTimeOffset dto, string format = "MMMM d, yyyy h:mm tt")
		{
			return dto
				.ToString(format);
		}
		private static DateTime epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static decimal ToUnixTimestamp(this DateTime dateTime)
		{
			return (decimal)DateTime.UtcNow.Subtract(epochDateTime).TotalSeconds;
		}
	}

	public enum DateType
	{
		Minutes = 0,
		Hours = 1,
		Days = 2,
		Weeks = 3,
		Months = 4,
		Years = 5
	}
}
