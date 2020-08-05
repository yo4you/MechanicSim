using System;
using System.Collections.Generic;

public enum TimeLineEntryType
{
	Time,
	Random,
	IfElse,
	Distribute,
}

public class TimeLineEntry : Entry
{
	public string Mechanic;
	public Dictionary<string, object> Parameters = new Dictionary<string, object>();

	public override bool IsParentingType()
	{
		return Type != (int)TimeLineEntryType.Time;
	}
}