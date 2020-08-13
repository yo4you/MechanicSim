using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public Dictionary<string, Mechanic> CustomMechanics { get; } = new Dictionary<string, Mechanic>();
	public List<ValueEntry> RefrenceValues { get; private set; } = new List<ValueEntry>();

	private void Start()
	{
		_titlePrefix = _title.text;
		_hudTimeLine = GetComponentInChildren<TimeLineBehaviour>();
		// just for debug
		LoadMechanic("Test0");
	}

	public void Rename(string prevName, string newName)
	{
		var renamedMechanic = CustomMechanics[prevName];
		renamedMechanic.Name = newName;
		CustomMechanics.Remove(prevName);
		CustomMechanics.Add(newName, renamedMechanic);

		CombineMechanicDictionaries();
		_hudTimeLine.Rename(prevName, newName);
	}

	public void Redraw()
	{
		_hudTimeLine.Redraw();
	}

	private void LoadMechanic(string name)
	{
		_title.text = _titlePrefix + name;
		// TODO read json file based on name
		_fightData = new FightData();
		//

		_fightData.Mechanics.ForEach((i) => CustomMechanics.Add(i.Name, i));
		CombineMechanicDictionaries();

		_hudTimeLine.SetEntries(_fightData.MechanicTimeLine.TimeLineEntries);
		RefrenceValues = _fightData.ValueStores;
	}

	private void CombineMechanicDictionaries()
	{
		_mechanics = new Dictionary<string, Mechanic>();
		BaseMechanics.Mechanics.Value.ForEach(AddMechanicToDict);
		CustomMechanics.Values.ToList().ForEach(AddMechanicToDict);
	}

	private void AddMechanicToDict(Mechanic mech) => Mechanics.Add(mech.Name, mech);

	public void AddMechanic(Mechanic mech)
	{
		Mechanics.Add(mech.Name, mech);
		CustomMechanics.Add(mech.Name, mech);
	}
}