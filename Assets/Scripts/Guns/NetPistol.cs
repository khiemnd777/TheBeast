public class NetPistol : NetGun
{
  public override void OnTriggerEffect()
  {
    EjectShell();
    if (flashAnim)
    {
      flashAnim.Play("Gun Flash", 0, 0);
    }
    audioSource.Play();
    base.OnTriggerEffect();
  }
}
