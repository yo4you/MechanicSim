﻿public class TimeLineEntryRandom : TimeLineEntryHud
{
	internal override void SetEntryData(TimeLineEntry entry, TimeLineBehaviour timeLineBehaviour)
	{
		base.SetEntryData(entry, timeLineBehaviour);
		ConstructEmptyEntry(entry);
	}
}