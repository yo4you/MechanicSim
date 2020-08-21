using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class ScriptableValueManager
{
	private const int _playerCount = 8;
	private bool _distributing;
	public List<ValueEntry> Values { get; internal set; }

	public Dictionary<ValueEntry, string> _entityValues = new Dictionary<ValueEntry, string>();

	private string[] _playerTypes = new string[_playerCount]
	{
		"tanks",
		"tanks",
		"healers",
		"healers",
		"dps",
		"dps",
		"dps",
		"dps"
	};

	private ValueEntry _bossPos;
	private string _bossLabel = "boss";
	private Dictionary<string, RandValueEntry> _randomGroups;

	public ScriptableValueManager()
	{
		Values = new List<ValueEntry>();
		ValueEntry parentSpecifiedVal = null;
		ValueEntry parentString = null;
		ValueEntry parentPos = null;
		_randomGroups = new Dictionary<string, RandValueEntry>();
		for (int i = 0; i < _playerCount; i++)
		{
			var typelabel = _playerTypes[i];
			var prevtypeLabel = i == 0 ? "" : _playerTypes[i - 1];
			var entityLabel = "player" + i;

			var newSpecifiedVal = new ValueEntry()
			{
				Label = typelabel,
				ParentEntry = prevtypeLabel == typelabel ? parentSpecifiedVal : null,
				Type = _playerTypes.Count(j => j == typelabel) == 1 ? ParameterType.POS : ParameterType.POS_COLLECT,
				Value = new Vector2()
			};
			parentSpecifiedVal = newSpecifiedVal;
			var newVal = new ValueEntry()
			{
				Label = "players",
				Value = entityLabel,
				ParentEntry = parentString,
				Type = ParameterType.STRING_COLLECT
			};

			parentString = newVal;
			var newValPos = new ValueEntry()
			{
				Label = "posplayers",
				Value = new Vector2(),
				ParentEntry = parentPos,
				Type = ParameterType.POS_COLLECT
			};
			parentPos = newValPos;

			if (!_randomGroups.ContainsKey(typelabel))
			{
				_randomGroups.Add(typelabel, new RandValueEntry());
			}
			_randomGroups[typelabel].Entries.Add(newVal);
			_randomGroups[typelabel].PosEntries.Add(newValPos);

			Values.Add(newVal);
			Values.Add(newValPos);
			Values.Add(newSpecifiedVal);
			_entityValues.Add(newValPos, entityLabel);
			_entityValues.Add(newSpecifiedVal, entityLabel);
		}

		var boss = new ValueEntry()
		{
			Label = "boss",
			Value = _bossLabel,
			Type = ParameterType.STRING
		};
		_bossPos = new ValueEntry()
		{
			Label = "bosspos",
			Value = new Vector2(),
			Type = ParameterType.POS
		};

		Values.Add(boss);
		Values.Add(_bossPos);
		_entityValues.Add(_bossPos, _bossLabel);

		foreach (var rand in _randomGroups)
		{
			rand.Value.Target = new ValueEntry()
			{
				Label = "rand" + rand.Key,
				Type = ParameterType.STRING
			};
			rand.Value.PosTarget = new ValueEntry()
			{
				Label = "rand" + rand.Key + "pos",
				Type = ParameterType.POS
			};
			Values.Add(rand.Value.Target);
			Values.Add(rand.Value.PosTarget);
		}
	}

	public void StartDistribute()
	{
		_distributing = true;
	}

	public void StopDistribute()
	{
		_distributing = false;
		foreach (var randEntry in _randomGroups)
		{
			randEntry.Value.UsedRandomIndices = new List<int>();
		}
	}

	public void Read()
	{
		foreach (var randEntry in _randomGroups)
		{
			int rand;
			if (randEntry.Value.UsedRandomIndices.Count > randEntry.Value.Entries.Count)
			{
				List<int> unpicked = new List<int>();
				for (int i = 0; i < randEntry.Value.Entries.Count; i++)
				{
					unpicked.Add(i);
				}
				unpicked = unpicked.Except(randEntry.Value.UsedRandomIndices).ToList();

				rand = unpicked[Random.Range(0, unpicked.Count)];
			}
			else
			{
				rand = Random.Range(0, randEntry.Value.Entries.Count);
			}
			if (_distributing)
			{
				randEntry.Value.UsedRandomIndices.Add(rand);
			}
			_randomGroups[randEntry.Key].Target = randEntry.Value.Entries[rand];
			_randomGroups[randEntry.Key].PosTarget = randEntry.Value.PosEntries[rand];
		}
		//foreach (var playerpos in )
		//{
		//	Vector2 newpos = new Vector2(); // @TODO read from player
		//	playerpos.Value = newpos;
		//}
	}
}