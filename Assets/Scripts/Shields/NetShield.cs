using System.Collections;
using Net;
using UnityEngine;

public class NetShield : MonoBehaviour
{
  public float hitback;
  protected NetShieldHolder holder;
  [System.NonSerialized]
  public bool anyAction;
  public RuntimeAnimatorController shieldAnimatorController;
  protected Player player;
  protected NetIdentity netIdentity;

  public virtual IEnumerator HoldTrigger()
  {
    yield break;
  }

  public virtual IEnumerator ReleaseTrigger()
  {
    yield break;
  }

  public virtual void TakeShieldAsReverse()
  {
    player.animator.runtimeAnimatorController = shieldAnimatorController;
  }

  public virtual void TakeShieldUpAsCover()
  {
    player.animator.runtimeAnimatorController = shieldAnimatorController;
  }

  public virtual void TakeShieldDown()
  {
    // Destroy (gameObject);
  }

  public virtual void Start()
  {

  }

  public virtual void Update()
  {

  }

  public virtual void FixedUpdate()
  {

  }

  public virtual Vector3 GetDirection()
  {
    var direction = Utility.GetDirection(transform, Vector3.back);
    return direction * holder.transform.localScale.z;
  }

  public Vector3 GetNormalDirection()
  {
    var direction = GetDirection();
    direction.Normalize();
    return direction;
  }

  public void SetPlayer(Player player)
  {
    this.player = player;
  }

  public void SetNetIdentity(NetIdentity netIdentity)
  {
    this.netIdentity = netIdentity;
  }
}
