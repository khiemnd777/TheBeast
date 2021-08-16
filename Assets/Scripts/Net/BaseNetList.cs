using System;
using System.Collections.Generic;
using System.Linq;
using Net.Socket;
using UnityEngine;

namespace Net
{
  public abstract class BaseNetList<T> : MonoBehaviour where T : BaseNetIdentity
  {
    protected Settings settings;
    protected ISocketWrapper socket;
    protected Dictionary<int, T> list = new Dictionary<int, T>();

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    protected virtual void Start()
    {
      settings = Settings.instance;
      socket = NetworkManagerCache.socket;
    }

    /// <summary>
    /// Store the player was created before by the prefab into the list.
    /// </summary>
    /// <param name="gamePlayer"></param>
    public virtual void Store(T gamePlayer)
    {
      list.Add(gamePlayer.id, gamePlayer);
    }

    /// <summary>
    /// Find the player through an identity.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual T Find(int id)
    {
      if (!Exists(id)) return default(T);
      return list[id];
    }

    /// <summary>
    /// Check whether any player that exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public virtual bool Exists(int id)
    {
      return list.Keys.Any(key => key == id);
    }

    /// <summary>
    /// Remove a player that exists on the list.
    /// </summary>
    /// <param name="id"></param>
    public virtual void Remove(int id)
    {
      list.Remove(id);
    }

    /// <summary>
    /// Clear all of the players on the list.
    /// </summary>
    public virtual void Clear()
    {
      list.Clear();
    }

    /// <summary>
    /// Get all objects on the list.
    /// </summary>
    /// <returns></returns>
    public virtual List<T> All()
    {
      return list.Select(x => x.Value).ToList();
    }

    public virtual List<T> All(Func<KeyValuePair<int, T>, bool> predicate)
    {
      return list.Where(predicate).Select(x => x.Value).ToList();
    }

    public virtual int Count()
    {
      return list.Count;
    }
  }
}
