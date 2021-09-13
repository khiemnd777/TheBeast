using UnityEngine;

public class NetRifle : NetGun
{
	[SerializeField]
	Animator _fireAnim;

  public override void OnTriggerEffect()
  {
    EjectShell ();
		flashAnim.Play ("Gun Flash", 0, 0);
		_fireAnim.Play ("Rifle Fire", 0, 0);
    audioSource.Play();
		base.OnTriggerEffect();
  }
}
