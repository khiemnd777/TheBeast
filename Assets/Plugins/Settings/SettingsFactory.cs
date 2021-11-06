using System.Runtime.InteropServices;

public class SettingsFactory
{
  [DllImport("__Internal")]
  public static extern void Play();

  [DllImport("__Internal")]
  public static extern void PlayerDead();
}