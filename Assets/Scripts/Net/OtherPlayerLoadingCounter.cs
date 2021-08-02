using System;

public class OtherPlayerLoadingCounter
{
  /// <summary>
  /// This will be fired when the loading was finished.
  /// </summary>
  public event Action onFinishedLoading;

  int _count;

  /// <summary>
  /// Total of loaded player
  /// </summary>
  /// <value></value>
  public int count
  {
    get
    {
      return _count;
    }
  }

  /// <summary>
  /// This is called proactively to increase the number of counter.
  /// Then, it will invoke the finished loading event when finished.
  /// </summary>
  /// <param name="total"></param>
  public void Count (int total, Action<int> onAfterCount = null)
  {
    ++_count;
    if (onAfterCount != null)
    {
      onAfterCount (_count);
    }
    if (_count == total)
    {
      _count = 0;
      if (onFinishedLoading != null)
      {
        onFinishedLoading ();
      }
    }
  }
}
