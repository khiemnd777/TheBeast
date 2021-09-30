using System;
using System.Collections.Generic;
using System.Linq;
using Net.Socket;
using UnityEngine;

namespace Net
{
  public abstract class BaseNetList<T> : MonoBehaviour where T : BaseNetIdentity
  {
    static object lockList = new object();
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
      lock (lockList)
      {
        list.Add(gamePlayer.id, gamePlayer);
      }
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
      lock (lockList)
      {
        list.Remove(id);
      }
    }

    /// <summary>
    /// Remove a player that exists on the list.
    /// </summary>
    /// <param name="id"></param>
    public virtual void Remove(T obj)
    {
      Remove(obj.id);
    }

    /// <summary>
    /// Clear all of the players on the list.
    /// </summary>
    public virtual void Clear()
    {
      lock (lockList)
      {
        list.Clear();
      }
    }

    /// <summary>
    /// Get all objects on the list.
    /// </summary>
    /// <returns></returns>
    public virtual List<T> All()
    {
      return list.Select(x => x.Value).ToList();
    }

    public virtual IEnumerable<T> All(Func<KeyValuePair<int, T>, bool> predicate, Func<IEnumerable<KeyValuePair<int, T>>, IOrderedEnumerable<KeyValuePair<int, T>>> order = null)
    {
      var query = list.Where(predicate);
      if (order != null)
      {
        query = order.Invoke(query);
      }
      return query.Select(x => x.Value);
    }

    public virtual int Count()
    {
      return list.Count;
    }
  }
}
