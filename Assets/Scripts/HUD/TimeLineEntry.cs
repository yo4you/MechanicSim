using System;
using System.Collections.Generic;

public enum TimeLineEntryType
{
	Time,
	Random,
	IfElse,
	Distribute,
}

public class TimeLineEntry
{
	public TimeLineEntry ParentEntry;
	public float Time = 0f;
	public TimeLineEntryType Type;
	public string Mechanic;
	public Dictionary<string, object> Parameters = new Dictionary<string, object>();
}