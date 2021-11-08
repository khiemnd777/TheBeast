using System.Runtime.InteropServices;

public class OpenUrlWindowFactory
{
  [DllImport("__Internal")]
  public static extern void OpenUrlWindow(string link);
}