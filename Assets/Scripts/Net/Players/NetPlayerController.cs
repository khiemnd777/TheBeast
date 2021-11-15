using UnityEngine;

namespace Net
{
  [RequireComponent(typeof(NetIdentity), typeof(NetTransform))]
  public class NetPlayerController : MonoBehaviour
  {
    public event System.Action OnSprint;

    [SerializeField]
    Player _player;

    public float initSpeed = 2f;
    [Range(0f, 1f)]
    public float sprintSpeed = 1f;
    [Range(0f, 1f)]
    public float walkSpeed = .5f;

    public Locker sprintLocker = new Locker();

    NetIdentity _netIdentity;
    NetTransform _netTransform;
    MovingCalculator _movingCalculator;
    SpeedCalculator _speedCalculator;
    Transform _cachedTransform;


    [Space]
    [SerializeField]
    CuriousGenerator _curiousGenerator;

    [SerializeField]
    CuriousListener _curiousListener;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
      _netIdentity = GetComponent<NetIdentity>();
      _netTransform = GetComponent<NetTransform>();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
      _cachedTransform = transform;

      if (_netIdentity.isLocal)
      {
        _curiousListener.curiousIdentity = _netIdentity.clientId;
        _curiousGenerator.curiousIdentity = _netIdentity.clientId;

        // Moving calculator
        _movingCalculator = new MovingCalculator(Point.FromVector3(_cachedTransform.position));
        _movingCalculator.onMoving += () =>
        {
          // Generate the footstep
          _curiousGenerator.Generate(_curiousGenerator.curiousIdentity);
          // if (_speedCalculator.speedType == SpeedType.Sprint)
          // {
          //   // Generate the footstep
          //   _curiousGenerator.Generate(_curiousGenerator.curiousIdentity);
          // }
        };

        // Speed calculator
        _speedCalculator = new SpeedCalculator();
        _speedCalculator
          .SetSpeedValues(initSpeed, sprintSpeed, walkSpeed)
          .SetSpeedType(SpeedType.Sprint);

        // Emit this event after the footstep generated.
        _curiousGenerator.onAfterGenerate += () =>
        {
          _netIdentity.EmitMessage("curious_generate", new GeneratedCuriosityJson
          {
            identity = _curiousGenerator.curiousIdentity
          });
        };

        // Listen to the curiosity.
        StartCoroutine(_curiousListener.Listen());
      }
      if (_netIdentity.isClient)
      {
        _netIdentity.onMessageReceived += (eventName, eventMessage) =>
        {
          if (eventName == "curious_generate")
          {
            // Generate the footstep
            var data = Utility.Deserialize<GeneratedCuriosityJson>(eventMessage);
            _curiousGenerator.Generate(data.identity);
          }
        };
      }
    }

    void Update()
    {
      if (_netIdentity.isLocal)
      {
        if (!sprintLocker.IsLocked() && Input.GetKey(KeyCode.LeftShift))
        {
          _speedCalculator.SetSpeedType(SpeedType.Sprint);
          if (OnSprint != null)
          {
            OnSprint();
          }
        }
        else
        {
          _speedCalculator.SetSpeedType(SpeedType.Walk);
        }
        if (_player.locker.IsLocked())
        {
          _speedCalculator.StopImmediately();
        }
        else
        {
          _speedCalculator.StartImmediately();
        }
      }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
      if (_netIdentity.isLocal)
      {
        var horizontalDirection = GetHorizontalDirection();
        var verticalDirection = GetVerticalDirection();
        _movingCalculator.Calculate(horizontalDirection, verticalDirection);
        _speedCalculator.Calculate(_player.gunWeightIncrement);
        if (!_player.locker.IsLocked())
        {
          _netTransform.Velocity(_movingCalculator.direction, _speedCalculator.speed);
        }
      }
    }

    /// <summary>
    /// Get horizontal direction (x-axis).
    /// </summary>
    /// <returns></returns>
    float GetHorizontalDirection()
    {
      return Input.GetAxisRaw("Horizontal");
    }

    /// <summary>
    /// Get vertical direction (y-axis).
    /// </summary>
    /// <returns></returns>
    float GetVerticalDirection()
    {
      return Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// Rotation
    /// </summary>
    /// <param name="rotation"></param>
    public void Rotate(Quaternion rotation)
    {
      _netTransform.Rotate(rotation);
    }
  }
}
