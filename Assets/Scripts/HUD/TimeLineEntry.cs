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
	public float Time;
	public TimeLineEntryType Type;
	public Mechanic Mechanic;
	public List<object> Parameters; 
}