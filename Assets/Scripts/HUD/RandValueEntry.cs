using System.Collections.Generic;

public class RandValueEntry
{
	public List<ValueEntry> Entries { get; set; } = new List<ValueEntry>();
	public List<ValueEntry> PosEntries { get; set; } = new List<ValueEntry>();
	public ValueEntry Target { get; set; }
	public ValueEntry PosTarget { get; set; }
	public List<int> UsedRandomIndices { get; set; } = new List<int>();
}