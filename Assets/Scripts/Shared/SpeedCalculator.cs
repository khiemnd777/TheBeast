public enum SpeedType
{
  Walk,
  Sprint
}

public class SpeedCalculator
{
  float _initSpeed;
  float _sprintSpeed;
  float _walkSpeed;
  float _speed;
  SpeedType _speedType;

  /// <summary>
  /// The speed that is being gotten by speed-type
  /// </summary>
  /// <value></value>
  public float speed
  {
    get
    {
      return _speed;
    }
  }

  public SpeedCalculator ()
  {

  }

  public SpeedCalculator (float initSpeed) : this (initSpeed, initSpeed, initSpeed)
  {

  }

  public SpeedCalculator (float initSpeed, float walkSpeed) : this (initSpeed, initSpeed, walkSpeed)
  {

  }

  public SpeedCalculator (float initSpeed, float sprintSpeed, float walkSpeed)
  {
    SetSpeeds (initSpeed, sprintSpeed, walkSpeed);
  }

  /// <summary>
  /// Set the values of speed such as init-speed.
  /// </summary>
  /// <param name="initSpeed"></param>
  /// <returns></returns>
  public SpeedCalculator SetSpeeds (float initSpeed)
  {
    return SetSpeeds (initSpeed, initSpeed, initSpeed);
  }

  /// <summary>
  /// Set the values of speed such as init-speed, walk-speed.
  /// </summary>
  /// <param name="initSpeed"></param>
  /// <param name="walkSpeed"></param>
  /// <returns></returns>
  public SpeedCalculator SetSpeeds (float initSpeed, float walkSpeed)
  {
    return SetSpeeds (initSpeed, initSpeed, walkSpeed);
  }

  /// <summary>
  /// Set the values of speed such as init-speed, sprint-speed, walk-speed.
  /// </summary>
  /// <param name="initSpeed"></param>
  /// <param name="sprintSpeed"></param>
  /// <param name="walkSpeed"></param>
  /// <returns></returns>
  public SpeedCalculator SetSpeeds (float initSpeed, float sprintSpeed, float walkSpeed)
  {
    _initSpeed = initSpeed;
    _sprintSpeed = sprintSpeed;
    _walkSpeed = walkSpeed;
    _speedType = SpeedType.Sprint;
    return this;
  }

  /// <summary>
  /// The calculation of speed by speed-type and init-speed.
  /// </summary>
  public void Calculate ()
  {
    switch (_speedType)
    {
      case SpeedType.Sprint:
        {
          _speed = _initSpeed * _sprintSpeed;
        }
        break;
      case SpeedType.Walk:
        {
          _speed = _initSpeed * _walkSpeed;
        }
        break;
      default:
        {

        }
        break;
    }
  }

  /// <summary>
  /// The calculation of speed by speed-type and init-speed.
  /// </summary>
  /// <param name="speedType"></param>
  public void Calculate (SpeedType speedType)
  {
    SetSpeedType (speedType);
    Calculate ();
  }

  /// <summary>
  /// Set the speed-type's value.
  /// </summary>
  /// <param name="speedType"></param>
  public void SetSpeedType (SpeedType speedType)
  {
    _speedType = speedType;
  }
}
