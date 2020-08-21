public abstract class Entry
{
	public ParameterType Type;

	public Entry ParentEntry = null;

	/// <summary>
	/// Time refers to the place in the time line this entry is associated with
	/// Value entries don't have a functional time however they're still sorted by Time
	/// so that values can appear in the same order as they're used in the time line
	/// </summary>
	public float Time = 0f;

	public abstract bool IsParentingType();
}