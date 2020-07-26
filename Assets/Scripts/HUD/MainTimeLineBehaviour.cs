using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainTimeLineBehaviour : MonoBehaviour
{

    [SerializeField]
	Text _title = default;
	private string _titlePrefix;
	private FightData _fightData;
    private List<Mechanic> _mechanics = new List<Mechanic>() ;
	private TimeLineBehaviour _hudTimeLine;

	void Start()
    {
        _titlePrefix =  _title.text;
        
        _hudTimeLine = GetComponentInChildren<TimeLineBehaviour>();
		// just for debug
		LoadMechanic("Test0");
	}

	private void LoadMechanic(string name)
	{
        _title.text = _titlePrefix + name;
        // TODO read json file based on name
        _fightData = new FightData();
        _mechanics.AddRange(_fightData.Mechanics);
        _mechanics.AddRange(BaseMechanics.Mechanics.Value);

        _hudTimeLine.SetEntries(_fightData.MechanicTimeLine.TimeLineEntries);
    }

	void Update()
    {
        
    }
}
