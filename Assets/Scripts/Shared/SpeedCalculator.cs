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
  bool _stopFlag;
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

  public SpeedType speedType { get { return _speedType; } }

  public SpeedCalculator()
  {

  }

  public SpeedCalculator(float initSpeed) : this(initSpeed, initSpeed, initSpeed)
  {

  }

  public SpeedCalculator(float initSpeed, float walkSpeed) : this(initSpeed, initSpeed, walkSpeed)
  {

  }

  public SpeedCalculator(float initSpeed, float sprintSpeed, float walkSpeed)
  {
    SetSpeedValues(initSpeed, sprintSpeed, walkSpeed);
  }

  /// <summary>
  /// Set the values of speed such as init-speed.
  /// </summary>
  /// <param name="initSpeed"></param>
  /// <returns></returns>
  public SpeedCalculator SetInitSpeed(float initSpeed)
  {
    return SetSpeedValues(initSpeed, initSpeed, initSpeed);
  }

  /// <summary>
  /// Set the values of speed such as init-speed, sprint-speed, walk-speed.
  /// </summary>
  /// <param name="initSpeed"></param>
  /// <param name="sprintSpeed"></param>
  /// <param name="walkSpeed"></param>
  /// <returns></returns>
  public SpeedCalculator SetSpeedValues(float initSpeed, float sprintSpeed, float walkSpeed)
  {
    _initSpeed = initSpeed;
    _sprintSpeed = sprintSpeed;
    _walkSpeed = walkSpeed;
    return this;
  }

  /// <summary>
  /// The calculation of speed by speed-type and init-speed.
  /// </summary>
  public void Calculate()
  {
    switch (_speedType)
    {
      case SpeedType.Sprint:
        {
          _speed = _stopFlag ? 0 : _initSpeed * _sprintSpeed;
        }
        break;
      case SpeedType.Walk:
        {
          _speed = _stopFlag ? 0 : _initSpeed * _walkSpeed;
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
  public void Calculate(SpeedType speedType)
  {
    SetSpeedType(speedType);
    Calculate();
  }

  /// <summary>
  /// Set the speed-type's value.
  /// </summary>
  /// <param name="speedType"></param>
  public SpeedCalculator SetSpeedType(SpeedType speedType)
  {
    _speedType = speedType;
    return this;
  }

  public void StopImmediately()
  {
    _stopFlag = true;
  }
  public void StartImmediately()
  {
    _stopFlag = false;
  }
}
