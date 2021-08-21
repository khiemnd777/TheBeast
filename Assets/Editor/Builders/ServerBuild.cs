using UnityEditor;

class Builder
{
  static void ServerBuild()
  {
    string[] scenes = { "Assets/_Scenes/2.unity" };
    BuildPipeline.BuildPlayer(new BuildPlayerOptions{
      scenes = scenes,
      target = BuildTarget.StandaloneWindows,
      locationPathName = @"C:\Works\project_game_code_blue\build\Server1\Server1.exe",
      options = BuildOptions.EnableHeadlessMode
    });
  }
}