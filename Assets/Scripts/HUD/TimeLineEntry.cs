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
	public float Time = 0f;
	public TimeLineEntryType Type;
	public string Mechanic;
	public List<object> Parameters;
}