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
	public Dictionary<string, ParameterData> Parameters = new Dictionary<string, ParameterData>();

	public override bool IsParentingType()
	{
		return Type != (int)TimeLineEntryType.Time;
	}
}