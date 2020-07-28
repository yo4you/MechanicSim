using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainTimeLineBehaviour : MonoBehaviour
{
	[SerializeField]
	private Text _title = default;

	private string _titlePrefix;
	private FightData _fightData;
	private Dictionary<string, Mechanic> _mechanics = new Dictionary<string, Mechanic>();
	private TimeLineBehaviour _hudTimeLine;
	public Dictionary<string, Mechanic> Mechanics { get => _mechanics; }

	private void Start()
	{
		_titlePrefix = _title.text;

		_hudTimeLine = GetComponentInChildren<TimeLineBehaviour>();
		// just for debug
		LoadMechanic("Test0");
	}

	private void LoadMechanic(string name)
	{
		_title.text = _titlePrefix + name;
		// TODO read json file based on name
		_fightData = new FightData();
		//
		void addmech(Mechanic mech) => Mechanics.Add(mech.Name, mech);
		_fightData.Mechanics.ForEach(addmech);
		BaseMechanics.Mechanics.Value.ForEach(addmech);

		_hudTimeLine.SetEntries(_fightData.MechanicTimeLine.TimeLineEntries);
	}
}