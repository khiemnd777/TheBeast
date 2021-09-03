using System;
using Net.Socket;
using UnityEngine;

namespace Net
{
  [RequireComponent(typeof(NetIdentity))]
  public class NetTransform : MonoBehaviour
  {
    [SerializeField]
    bool _useLocalPosition;

    [SerializeField]
    Transform _targetRotation;
    NetIdentity _netIdentity;
    NetworkManager _networkManager;
    ISocketWrapper _socket;
    Rigidbody _rb;
    Transform _cachedTransform;
    Vector3 _lastPosition;
    Quaternion _lastRotation;

    [SerializeField]
    Player _player;

    public bool canSendTranslateMessage
    {
      get
      {
        return _cachedTransform.position != _lastPosition;
      }
    }

    public bool canSendRotationMessage
    {
      get
      {
        return _cachedTransform.rotation != _lastRotation;
      }
    }

    void Start()
    {
      _networkManager = NetworkManagerCache.networkManager;
      _socket = NetworkManagerCache.socket;
      _rb = GetComponent<Rigidbody>();
      _cachedTransform = transform;
      _netIdentity = GetComponent<NetIdentity>();
      _netIdentity.onMessageReceived += (eventName, message) =>
      {
        if (eventName == Constants.EVENT_OBJECT_TRANSFORM)
        {
          if (_netIdentity.isServer && !_player.lifeEnd)
          {
            var netObj = TranslateAndRotateByMessage(message);
            _netIdentity.EmitMessage("server_object_transform", netObj);
          }
        }
        if (eventName == "server_object_transform")
        {
          if (!_netIdentity.isLocal && _netIdentity.isClient)
          {
            TranslateAndRotateByMessage(message);
          }
        }
      };
    }

    NetTransformJSON TranslateAndRotateByMessage(string message)
    {
      var netObj = NetTransformJSON.Deserialize(message);
      var point = Point.FromArray(netObj.position);
      Translate(point);
      var rotation = Utility.AnglesArrayToQuaternion(netObj.rotation);
      Rotate(rotation);
      return netObj;
    }

    /// <summary>
    /// Moves the transform in the direction and distance of translation.
    /// </summary>
    /// <param name="point"></param>
    public void Translate(Point point)
    {
      if (_useLocalPosition)
      {
        _cachedTransform.localPosition = Point.ToVector3(_cachedTransform.localPosition, point);
      }
      else
      {
        _cachedTransform.position = Point.ToVector3(_cachedTransform.position, point);
      }
      if (_netIdentity.isLocal)
      {
        EmitTransformEvent();
      }
      if (_useLocalPosition)
      {
        _lastPosition = _cachedTransform.position;
      }
      else
      {
        _lastPosition = _cachedTransform.localPosition;
      }
    }

    /// <summary>
    /// Set the velocity vector of the rigidbody.
    /// </summary>
    /// <param name="velocity"></param>
    public void Velocity(Point velocity)
    {
      if (!_rb) return;
      _rb.velocity = Point.ToVector3(_rb.velocity, velocity);
      if (_netIdentity.isLocal)
      {
        EmitTransformEvent();
      }
      _lastPosition = _cachedTransform.position;
    }

    /// <summary>
    /// Set the velocity vector by multiplication of direction and speed.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void Velocity(Point direction, float speed)
    {
      var velocity = direction * speed;
      Velocity(velocity);
    }

    /// <summary>
    /// Rotation
    /// </summary>
    /// <param name="rotation"></param>
    public void Rotate(Quaternion rotation)
    {
      if (!_targetRotation) return;
      var destinationAngles = rotation.eulerAngles;
      var realQuaternion = Quaternion.Euler(new Vector3(destinationAngles.x, destinationAngles.y, destinationAngles.z));
      _targetRotation.rotation = realQuaternion;
      if (_netIdentity.isLocal)
      {
        EmitTransformEvent();
      }
      _lastRotation = realQuaternion;
    }

    void EmitTransformEvent()
    {
      if (_player.lifeEnd) return;
      if (!canSendTranslateMessage && !canSendRotationMessage) return;
      var point = Point.FromVector3(_cachedTransform.position);
      var rotation = !_targetRotation ? Quaternion.identity : _targetRotation.rotation;
      var netTransformJson = new NetTransformJSON(point, rotation);
      _netIdentity.EmitMessage(Constants.EVENT_OBJECT_TRANSFORM, netTransformJson, true);
    }
  }
}
