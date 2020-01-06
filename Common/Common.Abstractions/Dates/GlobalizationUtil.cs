using Common.Abstractions.Languages;
using Common.Abstractions.TimeZones;
using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Abstractions.Dates
{
	

	public static class GlobalizationUtil
	{
		private static object cacheLockObject { get; } = new object();
		private static List<PlatformTimeZone> platformTimeZonesCache { get; set; }
		private static ConcurrentDictionary<string, PlatformTimeZone> timeZonesMapCache { get; } = new ConcurrentDictionary<string, PlatformTimeZone>(StringComparer.OrdinalIgnoreCase);

		//public static List<StateAbbreviation> GetUsStates()
		//{
		//	return EnumerableExtensions.GetList<StateAbbreviation>();
		//}

		public static PlatformTimeZone GetUtcTimeZone()
		{
			return new PlatformTimeZone("UTC", new NodaTimeZoneHelper(DateTimeZone.Utc));
		}

		public static PlatformTimeZone GetTimeZone(string timeZoneId, bool throwIfNotFound = true)
		{
			PlatformTimeZone timeZone = GlobalizationUtil.timeZonesMapCache.GetOrAdd(timeZoneId, GlobalizationUtil.GetTimeZoneInternal);
			if (throwIfNotFound && timeZone == null)
			{
				throw new Exception($"Timezone '{timeZoneId}' not found.");
			}
			return timeZone;
		}

		private static PlatformTimeZone GetTimeZoneInternal(string timeZoneId)
		{
			DateTimeZone timeZone = null;
			if (!string.IsNullOrWhiteSpace(timeZoneId))
			{
				timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneId);
			}
			if (timeZone == null)
			{
				return null;
			}
			return new PlatformTimeZone(timeZone.Id, new NodaTimeZoneHelper(timeZone));
		}

		public static List<PlatformTimeZone> GetTimeZones()
		{
			lock (GlobalizationUtil.cacheLockObject)
			{
				return GlobalizationUtil.platformTimeZonesCache ?? (GlobalizationUtil.platformTimeZonesCache =
							DateTimeZoneProviders.Tzdb.Ids
								//Only keep canonical ids, no aliases
								.Where(id => TzdbDateTimeZoneSource.Default.Aliases.Contains(id))
								.Select(id => GlobalizationUtil.GetTimeZone(id))
								.OrderBy(t => t.GetCurrentOffset())
								.ToList());
			}
		}

		private static List<PlatformLanguage> orderedLanguages;
		public static List<PlatformLanguage> GetLanguages()
		{
			GlobalizationUtil.SetLanguageCache();

			return GlobalizationUtil.orderedLanguages;
		}

		private static Dictionary<string, PlatformLanguage> languageDictionary;
		public static bool TryGetLanguage(string code, out PlatformLanguage language)
		{
			GlobalizationUtil.SetLanguageCache();

			return GlobalizationUtil.languageDictionary.TryGetValue(code, out language);
		}

		private static void SetLanguageCache()
		{
			lock (GlobalizationUtil.cacheLockObject)
			{
				if (GlobalizationUtil.orderedLanguages == null)
				{
					GlobalizationUtil.orderedLanguages = LanguageStore.Get
						.Select(kv => new PlatformLanguage(kv.Key, kv.Value))
						.OrderBy(l => l.Label, StringComparer.OrdinalIgnoreCase)
						.ToList();
				}
				if (GlobalizationUtil.languageDictionary == null)
				{
					GlobalizationUtil.languageDictionary = GlobalizationUtil.orderedLanguages
						.ToDictionary(l => l.Code, l => l);
				}
			}
		}
	}
}
