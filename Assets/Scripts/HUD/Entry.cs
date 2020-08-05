public abstract class Entry
{
	public int Type;

	public Entry ParentEntry = null;
	public float Time = 0f;

	public abstract bool IsParentingType();
}