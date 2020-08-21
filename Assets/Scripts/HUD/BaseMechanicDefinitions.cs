using UnityEngine;

public static partial class BaseMechanicDefinitions
{
	public static void AOECircle(Vector2 position, float radius, float warnTime)
	{
	}

	public static void AOEDonut(Vector2 position, float inRadius, float outRadius, float warnTime)
	{
	}

	public static void AOERect(Vector2 corner1, Vector2 corner2, float warnTime)
	{
	}

	public static void AOELine(Vector2 position, float angle, float width, float warnTime)
	{
	}

	public static void Gaze(Vector2 pos, float warnTime)
	{
	}

	public static void StackMarker(Vector2 pos, float people, float warnTime)
	{
	}

	public static void FlareMarker(Vector2 pos, float halvingRadius, float warnTime)
	{
	}

	public static void ApplyStatus(string entity, string status)
	{
	}

	public static void Tether(Vector2 pos1, Vector2 pos2, string effectData)
	{
	}

	public static void SpawnMob(Vector2 position, string mob)
	{
	}

	public static void Speak(string line, float uptime)
	{
	}

	public static void StoreClosestPlayer(RefrenceType<string> name, Vector2 pos, float skip)
	{
	}

	public static void StoreFurthestPlayer(RefrenceType<string> name, Vector2 pos, float skip)
	{
	}

	public static void StoreNum(RefrenceType<float> newnum, float number)
	{
	}

	public static void Negate(RefrenceType<float> name)
	{
	}

	public static void StoreAngle(RefrenceType<float> angle, Vector2 unitVector)
	{
	}

	public static void Sum(RefrenceType<float> sum, float summand)
	{
	}

	public static void MultNum(RefrenceType<float> product, float multiplicant)
	{
	}

	public static void StoreDistance(RefrenceType<float> dist, Vector2 pos1, Vector2 pos2)
	{
	}

	public static void DotProduct(RefrenceType<float> product, Vector2 vec1, Vector2 vec2)
	{
	}

	public static void StorePos(RefrenceType<Vector2> name, Vector2 pos)
	{
	}

	public static void StoreDiffrencePos(RefrenceType<Vector2> diffrence, Vector2 minuend, Vector2 subtrahend)
	{
	}

	public static void SumPos(RefrenceType<Vector2> sum, Vector2 summand)
	{
	}

	public static void Normalize(RefrenceType<Vector2> unitVector)
	{
	}

	public static void Scale(RefrenceType<Vector2> vector, float scalar)
	{
	}

	public static void Flip(RefrenceType<Vector2> vector)
	{
	}

	public static void StoreUnitVector(RefrenceType<Vector2> unitVector, float angle)
	{
	}

	public static void StoreString(RefrenceType<string> name, string text)
	{
	}
}