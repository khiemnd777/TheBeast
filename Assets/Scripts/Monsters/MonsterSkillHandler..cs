using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillHandler : MonoBehaviour
{
	public MonsterZero host;
	public AnimationClip defaultAnim;
	[Header ("Fang Bomb")]
	public MonsterZeroFangBomb fangBombPrefab;
	public Transform projectileBomb;
	public AnimationClip fangBombAnim;
	public float timeFangBombLaunching;
	public float timeBetweenLaunchFangBomb;
	public float minDistanceExecuteFangBomb;
	public float maxDistanceExecuteFangBomb;
	[Header ("Bite")]
	public AnimationClip biteAnim;
	public float timeBetweenBites;
	public float maxDistanceExecuteBite;
	public float distanceBite;

	bool _beInFangBombSkill;
	bool _beInBiteSkill;
	bool _accessExecutingSkill;
	public bool accessExecutingSkill
	{
		get { return _accessExecutingSkill; }
	}
	Player2 _player;

	void Start ()
	{
		_player = FindObjectOfType<Player2> ();
	}

	void Update ()
	{
		DistanceForExecutingSkill ();
	}

	void DistanceForExecutingSkill ()
	{
		var distanceFromTarget = Utilities.DistanceFromTarget (_player.transform.position, host.transform.position);
		if (maxDistanceExecuteBite >= distanceFromTarget)
		{
			if (_beInFangBombSkill)
			{
				_accessExecutingSkill = false;
				return;
			}
			if (_accessExecutingSkill) return;
			_accessExecutingSkill = true;
			StartCoroutine (BiteSkill ());
			return;
		}
		else if (minDistanceExecuteFangBomb <= distanceFromTarget && distanceFromTarget <= maxDistanceExecuteFangBomb)
		{
			if (_beInBiteSkill)
			{
				_accessExecutingSkill = false;
				return;
			}
			if (_accessExecutingSkill) return;
			_accessExecutingSkill = true;
			StartCoroutine (FangBombSkill ());
			return;
		}
		_accessExecutingSkill = false;
	}

	IEnumerator BiteSkill ()
	{
		if (_beInBiteSkill) yield break;
		_beInBiteSkill = true;
		var tBetweenAct = 1f;
		while (true)
		{
			while (tBetweenAct <= 1f)
			{
				tBetweenAct += Time.deltaTime / timeBetweenBites;
				yield return null;
			}
			tBetweenAct = 0f;
			host.StopMoving ();
			host.animator.Play (biteAnim.name, 0, 0);
			yield return new WaitForSeconds (biteAnim.length);
			host.animator.Play (defaultAnim.name, 0, 0);
			if (!_accessExecutingSkill) break;
		}
		_beInBiteSkill = false;
		host.animator.Play (defaultAnim.name, 0, 0);
		host.KeepMoving ();
	}

	IEnumerator FangBombSkill ()
	{
		if (_beInFangBombSkill) yield break;
		_beInFangBombSkill = true;
		var tBetweenAct = 1f;
		while (true)
		{
			host.animator.Play (fangBombAnim.name, 0, 0);
			while (tBetweenAct <= 1f)
			{
				tBetweenAct += Time.deltaTime / timeBetweenLaunchFangBomb;
				yield return null;
			}
			tBetweenAct = 0f;
			host.StopMoving ();
			var fangBombIns = Instantiate<MonsterZeroFangBomb> (fangBombPrefab, projectileBomb.position, Quaternion.identity);
			fangBombIns.projectile = projectileBomb;
			// Launching...
			var tLaunching = 0f;
			while (tLaunching <= 1f)
			{
				tLaunching += Time.deltaTime / timeFangBombLaunching;
				yield return null;
			}
			fangBombIns.Launch ();
			if (!_accessExecutingSkill) break;
		}
		_beInFangBombSkill = false;
		host.animator.Play (defaultAnim.name, 0, 0);
		host.KeepMoving ();
	}

	void OnDrawGizmos ()
	{
		Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuteBite);
		Gizmos.DrawWireSphere (host.transform.position, minDistanceExecuteFangBomb);
		Gizmos.DrawWireSphere (host.transform.position, maxDistanceExecuteFangBomb);
	}
}
