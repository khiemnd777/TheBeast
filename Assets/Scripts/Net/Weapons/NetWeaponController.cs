using System;
using Net.Socket;
using UnityEngine;

namespace Net
{
  [RequireComponent (typeof (NetIdentity))]
  public class NetWeaponController : MonoBehaviour
  {
    /// <summary>
    /// The weapon that the player holds on hand.
    /// </summary>
    public NetWeapon weapon;
    /// <summary>
    /// The holder holds the weapon.
    /// </summary>
    public Transform holder;
    Transform projectilePoint;
    Settings settings;
    NetIdentity netIdentity;
    Player player;
    NetBulletList netBulletList;
    SocketNetworkManager socketNetworkManager;
    ISocketWrapper socket;
    Cooldown _holdTriggerCooldown;
    bool holdTrigger;

    void Start ()
    {
      settings = Settings.instance;
      netIdentity = GetComponent<NetIdentity> ();
      player = GetComponent<Player> ();
      netBulletList = NetBulletList.instance;
      projectilePoint = weapon.projectilePoint;
      if (weapon)
      {
        weapon.netIdentity = netIdentity;
        weapon.player = player;
      }
      socket = SocketNetworkManagerCache.socket;
      _holdTriggerCooldown = new Cooldown (HoldTrigger);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update ()
    {
      if (netIdentity.isLocal)
      {
        if (weapon)
        {
          holdTrigger = Input.GetButton ("Fire1");
          _holdTriggerCooldown.Count (weapon.interval);
        }
      }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate ()
    {
      if (holdTrigger)
      {
        _holdTriggerCooldown.Execute ();
      }
    }

    /// <summary>
    /// Hold the trigger of the guns.
    /// </summary>
    public void HoldTrigger ()
    {
      if (weapon)
      {
        weapon.HoldTrigger ();
        if (netIdentity.isLocal)
        {
          socket.Emit (Constants.EVENT_SERVER_WEAPON_TRIGGER, new NetWeaponTriggerJSON { id = netIdentity.id });
        }
      }
    }

    /// <summary>
    /// Removes the bullet by the others.
    /// </summary>
    /// <param name="id"></param>
    public void RemoveBulletByOther (int id)
    {
      netBulletList.Remove (id);
    }
  }
}
