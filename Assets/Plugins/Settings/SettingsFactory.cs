using System.Runtime.InteropServices;

public class SettingsFactory
{
  [DllImport("__Internal")]
  public static extern void Play();

  [DllImport("__Internal")]
  public static extern void PlayerDead();

  [DllImport("__Internal")]
  public static extern void Fullscreen();

  [DllImport("__Internal")]
  public static extern void Windowscreen();
}