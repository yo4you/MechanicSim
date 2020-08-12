using System.Text.RegularExpressions;

public static class TimeStampUtil
{
	public static float FromTimeStamp(string time)
	{
		if (float.TryParse(time, out float res))
		{
			return res;
		}
		var groups = Regex.Match(time, @"(\d*):(\d*)").Groups;
		if (groups.Count == 3)
		{
			return (int.Parse(groups[1].ToString()) * 60) + int.Parse(groups[2].ToString());
		}

		return 0f;
	}

	public static string ToTimeStamp(float time)
	{
		string ToTwoPlaces(int digit)
		{
			var outp = "";
			outp += digit / 10;
			outp += digit % 10;
			return outp;
		}
		return ToTwoPlaces((int)(time / 60f)) + ":" + ToTwoPlaces((int)(time % 60f));
	}
}