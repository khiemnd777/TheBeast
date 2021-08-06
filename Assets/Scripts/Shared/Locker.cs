
using System.Linq;
using System.Collections.Generic;

public class Locker
{
  IDictionary<string, bool> _lockControlList;

  public Locker()
  {
    _lockControlList = new Dictionary<string, bool>();
  }

  public void RegisterLock(string name)
  {
    if (_lockControlList.ContainsKey(name)) return;
    _lockControlList.Add(name, false);
  }

  public void Lock(string name)
  {
    if (!_lockControlList.ContainsKey(name)) return;
    _lockControlList[name] = true;
  }

  public void Unlock(string name)
  {
    if (!_lockControlList.ContainsKey(name)) return;
    _lockControlList[name] = false;
  }

  public bool IsLocked()
  {
    return _lockControlList.Values.Any(locked => locked);
  }
}
