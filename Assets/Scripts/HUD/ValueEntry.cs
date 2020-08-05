public enum ParameterType
{
	NUM,
	STRING,
	POS,
	NUM_COLLECT,
	STRING_COLLECT,
	POS_COLLECT
}

public class ValueEntry : Entry
{
	public object Value;
	public string Label;

	public override bool IsParentingType()
	{
		return false;
	}
}