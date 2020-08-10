public enum ParameterType
{
	NUM,
	STRING,
	POS, //  keep as last, parenting types start after pos
	NUM_COLLECT,
	STRING_COLLECT,
	POS_COLLECT
}

public class ValueEntry : Entry
{
	public object Value;
	public string Label = "";

	public override bool IsParentingType()
	{
		return Type > (int)ParameterType.POS;
	}
}