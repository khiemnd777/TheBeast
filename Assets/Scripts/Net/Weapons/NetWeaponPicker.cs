using UnityEngine;

namespace Net
{
  public class NetWeaponPicker : MonoBehaviour
  {
    public NetWeapon weaponPrefab;
    public Transform display;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public virtual void Start ()
    {

    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    public virtual void Update ()
    {
      MoveDisplayBySineWave ();
    }

    void MoveDisplayBySineWave ()
    {
      var displayLocalPosition = display.localPosition;
      var sineWave = MathUtility.SineWave (0.09f, 5, Time.time);
      display.localPosition = new Vector3 (displayLocalPosition.x, sineWave, displayLocalPosition.y);
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter (Collider other)
    {
      if (other.tag == "Player")
      {
        var netWeaponController = other.GetComponent<NetWeaponController> ();
        if (netWeaponController && !netWeaponController.weapon)
        {
          var player = other.GetComponent<Player> ();
          var holder = netWeaponController.holder;
          var insWeapon = Instantiate<NetWeapon> (weaponPrefab, holder.position, holder.rotation, holder);
          insWeapon.player = player;
          netWeaponController.weapon = insWeapon;
          Destroy (gameObject);
        }
      }
    }
  }
}
