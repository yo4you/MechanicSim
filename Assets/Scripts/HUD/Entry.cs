public abstract class Entry
{
	public ParameterType Type;

	public Entry ParentEntry = null;
	public float Time = 0f;

	public abstract bool IsParentingType();
}