using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillHandler : MonoBehaviour
{
	public Transform host;
	public List<MonsterSkill> skills;
	[System.NonSerialized]
	public bool accessExecutingSkill;
	Player2 _player;

	void Start ()
	{
		_player = FindObjectOfType<Player2> ();
	}

	void Update ()
	{
		ExecuteSkillsBaseDistance ();
	}

	void ExecuteSkillsBaseDistance ()
	{
		var distanceFromTarget = Utilities.DistanceFromTarget (_player.transform.position, host.transform.position);
		foreach (var skill in skills)
		{
			if (Mathf.Clamp (distanceFromTarget, skill.minDistanceExecuting, skill.maxDistanceExecuting) != distanceFromTarget) continue;
			if (skill.beExecuting)
			{
				accessExecutingSkill = false;
				return;
			}
			if (accessExecutingSkill) return;
			accessExecutingSkill = true;
			StartCoroutine (skill.Execute ());
		}
	}
}
