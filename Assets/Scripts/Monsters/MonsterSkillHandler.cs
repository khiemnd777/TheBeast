using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSkillHandler : MonoBehaviour
{
	public Transform host;
	public List<MonsterSkill> skills;
	[System.NonSerialized]
	public bool accessExecutingSkill;
	[System.NonSerialized]
	public MonsterSkill executingSkill;
	[System.NonSerialized]
	public bool isPassiveFendingOff;
	Player2 _player;

	void Start ()
	{
		_player = FindObjectOfType<Player2> ();
	}

	void Update ()
	{
		if (isPassiveFendingOff) return;
		ExecuteSkillsBaseDistance ();
	}

	void ExecuteSkillsBaseDistance ()
	{
		var distanceFromTarget = Utilities.DistanceFromTarget (_player.transform.position, host.transform.position);
		var executeableList = new List<MonsterSkill> ();
		foreach (var skill in skills)
		{
			if (Mathf.Clamp (distanceFromTarget, skill.minDistanceExecuting, skill.maxDistanceExecuting) != distanceFromTarget)
			{
				skill.OnOutOfRange ();
				continue;
			}
			if (skills.Any (x => x.beExecuting))
			{
				return;
			}
			executeableList.Add (skill);
		}
		if (!executeableList.Any ()) return;
		executingSkill = executeableList.ElementAt (Random.Range (0, executeableList.Count));
		if (!executingSkill) return;
		executingSkill.StartExecutingSkill ();
	}
}
