public enum ParameterType
{
	NUM,
	STRING,
	POS, //  keep as last, parenting types start after pos
	NUM_COLLECT,
	STRING_COLLECT,
	POS_COLLECT,
	ANY // special
}

public static class ParameterTypeExtension
{
	public static bool Equality(this ParameterType p0, ParameterType p1)
	{
		return p1 == ParameterType.ANY || p0 == p1;
	}
}

public class ValueEntry : Entry
{
	public object Value;
	public string Label = "";

	public override bool IsParentingType()
	{
		return Type > ParameterType.POS;
	}
}