using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterZeroSkillHandler : MonoBehaviour
{
	public MonsterZero host;
	public MonsterZeroFangBomb fangBombPrefab;
	public Transform projectileBomb;
	public bool beInASkill;
	public float timeBetweenAct;
	MonsterSkill _skillBeInAct;
	float _tBetweenAct = 1;

	void Update ()
	{
		if (!beInASkill)
		{
			_tBetweenAct += Time.deltaTime / timeBetweenAct;
			if (_tBetweenAct <= 1f) return;
			_tBetweenAct = 0f;
			var fangBombIns = Instantiate<MonsterZeroFangBomb> (fangBombPrefab, projectileBomb.position, Quaternion.identity);
			fangBombIns.projectile = projectileBomb;
			_skillBeInAct = fangBombIns;
			beInASkill = true;
		}
		else
		{
			if (_skillBeInAct.launched)
			{
				beInASkill = false;
			}
		}
	}
}
