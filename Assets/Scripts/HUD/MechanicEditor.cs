using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MechanicEditor : MonoBehaviour
{
	private InputField _nameField;

	private Lazy<MainTimeLineBehaviour> _timeLine = new Lazy<MainTimeLineBehaviour>(() => FindObjectOfType<MainTimeLineBehaviour>());
	private Lazy<SubElementBehaviour> _subElement = new Lazy<SubElementBehaviour>(() => FindObjectOfType<SubElementBehaviour>());

	private Mechanic _mechanic;

	private void Rename(string newName)
	{
		//TODO validate
		_subElement.Value.Rename(_mechanic.Name, newName);
		_timeLine.Value.Rename(_mechanic.Name, newName);
	}

	private void Update()
	{
	}

	internal void Load(string mechanicName)
	{
		_nameField = GetComponentInChildren<InputField>();
		_nameField.onEndEdit.AddListener((s) => Rename(s));

		_nameField.SetTextWithoutNotify(mechanicName);
		_mechanic = _timeLine.Value.Mechanics[mechanicName];

		GetComponentInChildren<TimeLineBehaviour>().SetEntries(_mechanic.TimeLine.TimeLineEntries);
	}
}