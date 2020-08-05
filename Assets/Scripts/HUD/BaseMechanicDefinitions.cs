using UnityEngine;

public static partial class BaseMechanicDefinitions
{
	public static void SpawnAOECircle(Vector2 position, float radius, float warnTime)
	{
	}

	public static void SpawnAOEDonut(Vector2 position, float inRadius, float outRadius, float warnTime)
	{
	}

	public static void SpawnAOERect(Vector2 corner1, Vector2 corner2, float warnTime)
	{
	}

	public static void Gaze(Vector2 pos, float warnTime)
	{
	}

	public static void Speak(string line, float uptime)
	{
	}

	public static void SpawnAd(Vector2 position, string ad)
	{
	}

	public static void StorePos(string name, Vector2 position)
	{
	}

	public static void StoreNum(string name, float number)
	{
	}

	public static void StoreString(string name, string theString)
	{
	}

	public static float GetNum(string name)
	{
		return -1f;
	}

	public static string GetString(string name)
	{
		return "";
	}

	public static Vector2 GetPos(string name)
	{
		return new Vector2();
	}
}