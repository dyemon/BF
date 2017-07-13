using System;

public class DateTimeUtill {
	
	public static DateTime ConvertFromUnixTimestamp(long  timestamp) {
		DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		return origin.AddSeconds(timestamp);
	}

	public static long ConvertToUnixTimestamp(DateTime date) {
		DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		TimeSpan diff = date - origin;
		return (long)Math.Floor(diff.TotalSeconds);
	}

	public static string FormatMinutes(int minutes) {
		int hour = (int)(minutes / 60);
		int min = minutes % 60;
		return string.Format("{0}:{1}", ((hour > 9) ? hour.ToString() : "0" + hour), ((min > 9) ? min.ToString() : "0" + min));
	}

	public static bool IsYesterday(long now, long check) {
		DateTime nowdt = ConvertFromUnixTimestamp(now);
		DateTime checkdt = ConvertFromUnixTimestamp(check);
		return nowdt.Date.AddDays(-1) == checkdt.Date;
	}

	public static bool IsToday(long now, long check) {
		DateTime nowdt = ConvertFromUnixTimestamp(now);
		DateTime checkdt = ConvertFromUnixTimestamp(check);
		return nowdt.Date == checkdt.Date;
	}
}
