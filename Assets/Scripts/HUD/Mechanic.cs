using System;
using System.Collections.Generic;

public class Mechanic
{
	public string Name;
	public MechanicTimeLine TimeLine;
	public Dictionary<string, Type> ParameterTypes = default;
	public string FuncCall = default;
}