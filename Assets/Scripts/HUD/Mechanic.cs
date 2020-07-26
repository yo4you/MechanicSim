using System;
using System.Collections.Generic;

public class Mechanic
{
	public string Name;
	public MechanicTimeLine TimeLine;
	public Dictionary<string, Type> Parameters = default;
	public string FuncCall = default;
}