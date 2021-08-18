using System;
using System.Collections;
using System.Linq;
using Net.Socket;
using UnityEngine;

namespace Net
{
  public class NetRegistrar : MonoBehaviour
  {
    public event Action<int, int> onPlayerCount;
    [SerializeField]
    NetIdentityItem[] netIdentifierPrefabs;
    NetworkManager _networkManager;
    ISocketWrapper socket;
    NetObjectList netObjectList;
    CameraController cameraController;
    DotSightController dotSightController;
    OtherPlayerLoadingCounter otherPlayerCounter;
    Settings _settings;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
      _settings = Settings.instance;
      _networkManager = NetworkManagerCache.networkManager;
      socket = NetworkManagerCache.socket;
      netObjectList = NetObjectList.instance;
      otherPlayerCounter = new OtherPlayerLoadingCounter();
      dotSightController = FindObjectOfType<DotSightController>();
      cameraController = FindObjectOfType<CameraController>();
      InitEvents();
    }

    object initClientObj = new object();
    void InitEvents()
    {
      // This will be fired after the connection was completely connected.
      _networkManager.onConnected += () =>
      {
        print("Initializing other players...");
        socket.Emit(Constants.EVENT_SERVER_LOAD_PLAYERS);
      };
      _networkManager.onReceiveMessageJson += (NetMessageJSON dataJson) =>
      {
        if (netObjectList.Exists(dataJson.id))
        {
          var netObj = netObjectList.Find(dataJson.id);
          if (netObj)
          {
            netObj.OnReceiveMessage(dataJson.eventName, dataJson.message);
          }
        }
        else
        {
          // Generate the object at the client if it does not exist.
          if (_settings.isClient)
          {
            lock (initClientObj)
            {
              if (!netObjectList.Exists(dataJson.id))
              {
                Debug.Log($"Instantiate the object of client {dataJson.clientId}");
                var position = Point.FromArray(dataJson.position);
                var rotation = Utility.AnglesArrayToQuaternion(dataJson.rotation);
                var netObj = CreateAtTheClientSide(dataJson.prefabName, dataJson.clientId, dataJson.netName, dataJson.id, dataJson.position, dataJson.rotation, dataJson.life, dataJson.maxLife);
                netObj.OnReceiveMessage(dataJson.eventName, dataJson.message);
                StartCoroutine(OnAfterInitNetObjectWithMessage(netObj, dataJson.eventName, dataJson.message));
              }
              else
              {
                var netObj = netObjectList.Find(dataJson.id);
                if (netObj)
                {
                  StartCoroutine(OnAfterInitNetObjectWithMessage(netObj, dataJson.eventName, dataJson.message));
                }
              }
            }
          }
        }
      };
      _networkManager.onBroadcastCloneEverywhereJson += (NetCloneJSON dataJson) =>
      {
        CloneEverywhere(
          dataJson.prefabName,
          dataJson.clientId,
          dataJson.netName,
          dataJson.position,
          dataJson.rotation,
          dataJson.life,
          dataJson.maxLife
        );
      };
      if (_settings.isServer)
      {
        _networkManager.onServerRegister += (NetRegisterJSON netRegisterJson) =>
        {
          Debug.Log($"Register {netRegisterJson.prefabName}'s '{netRegisterJson.netName}' for client-id '{netRegisterJson.clientId}'");
          CreateAtTheServerSide(netRegisterJson.prefabName, netRegisterJson.clientId, netRegisterJson.netName);
        };
      }
      else
      {
        _networkManager.onClientRegisterFinished += (NetObjectJSON netObjJson) =>
        {
          CreateAtTheClientSide(netObjJson.prefabName, netObjJson.clientId, netObjJson.netName, netObjJson.id, netObjJson.position, netObjJson.rotation, netObjJson.life, netObjJson.maxLife);
        };
      }
    }

    IEnumerator OnAfterInitNetObjectWithMessage(NetIdentity netObj, string eventName, string message)
    {
      yield return null;
      netObj.OnReceiveMessage(eventName, message);
    }

    public void Register(string prefabName, string netName)
    {
      if (_settings.isServer) return;
      print("Registering...");
      socket.Emit(Constants.EVENT_REGISTER, new NetRegisterJSON(_networkManager.clientId.ToString(), prefabName, netName));
    }

    /// <summary>
    /// Create net player by prefab.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="life"></param>
    public NetIdentity CreateLocally(string prefabName, string clientId, string netName, int id, float[] position, float[] rotation, float life, float maxLife)
    {
      if (netObjectList.Exists(id)) return null;
      var prefab = netIdentifierPrefabs.FirstOrDefault(x => x.name == prefabName);
      if (prefab.netIdentityPrefab)
      {
        var netIdentifierPrefab = prefab.netIdentityPrefab;
        var netId = Instantiate<NetIdentity>(
          netIdentifierPrefab,
          Utility.PositionArrayToVector3(Vector3.zero, position),
          Utility.AnglesArrayToQuaternion(rotation));
        netId.clientId = clientId;
        netId.prefabName = prefabName;
        netId.Init(id, netName);
        netId.life = life;
        netId.maxLife = maxLife;
        netObjectList.Store(netId);
        return netId;
      }
      return null;
    }

    public NetIdentity CreateClientOther(string prefabName, string clientId, string netName, int id, float[] position, float[] rotation, float life, float maxLife)
    {
      if (netObjectList.Exists(id)) return null;
      var prefab = netIdentifierPrefabs.FirstOrDefault(x => x.name == prefabName);
      if (prefab.netIdentityPrefab)
      {
        var netIdentifierPrefab = prefab.netIdentityPrefab;
        var netId = Instantiate<NetIdentity>(
          netIdentifierPrefab,
          Utility.PositionArrayToVector3(Vector3.zero, position),
          Utility.AnglesArrayToQuaternion(rotation));
        netId.SetNetIdAtClientSide(id);
        netId.clientId = clientId;
        netId.prefabName = prefabName;
        netId.InitOther(id, netName);
        netId.life = life;
        netId.maxLife = maxLife;
        netObjectList.Store(netId);
        return netId;
      }
      return null;
    }

    public void CloneEverywhere(string prefabName, string clientId, string netName, float[] position, float[] rotation, float life, float maxLife)
    {
      if (clientId != _networkManager.clientId.ToString())
      {
        var prefab = netIdentifierPrefabs.FirstOrDefault(x => x.name == prefabName);
        if (prefab.netIdentityPrefab)
        {
          var netIdentifierPrefab = prefab.netIdentityPrefab;
          var netIdentityIns = Instantiate<NetIdentity>(
            netIdentifierPrefab,
            Utility.PositionArrayToVector3(Vector3.zero, position),
            Utility.AnglesArrayToQuaternion(rotation));
          if (_settings.isServer)
          {
            netIdentityIns.InitServer(netIdentityIns.GetInstanceID(), netName);
          }
          if (_settings.isClient)
          {
            netIdentityIns.InitOther(netIdentityIns.GetInstanceID(), netName);
          }
          netIdentityIns.life = life;
          netIdentityIns.maxLife = maxLife;
        }
      }
    }

    void CreateAtTheServerSide(string prefabName, string clientId, string netName)
    {
      if (!_settings.isServer) return;
      var prefab = netIdentifierPrefabs.FirstOrDefault(x => x.name == prefabName);
      if (prefab.netIdentityPrefab)
      {
        var netIdentifierPrefab = prefab.netIdentityPrefab;
        var netId = netObjectList.Create(netIdentifierPrefab, netName, Vector3.zero, Quaternion.identity);
        netId.clientId = clientId;
        netId.prefabName = prefabName;
        netId.type = "player";
        netObjectList.Store(netId);
        Debug.Log($"The object '{netName}' '{netId.id}' has been instantiated.");
        socket.Emit(Constants.EVENT_SERVER_REGISTER_FINISHED,
          new NetObjectJSON(
            clientId,
            netId.id,
            prefabName,
            netName,
            netId.life,
            netId.maxLife,
            Point.FromVector3(netId.transform.position),
            netId.transform.rotation
          )
        );
      }
    }

    NetIdentity CreateAtTheClientSide(string prefabName, string clientId, string netName, int id, float[] position, float[] rotation, float life, float maxLife)
    {
      if (!_settings.isClient) return null;
      var isLocalPlayer = clientId.Equals(_networkManager.clientId.ToString());
      if (isLocalPlayer)
      {
        return CreateLocally(prefabName, clientId, netName, id, position, rotation, life, maxLife);
      }
      return CreateClientOther(prefabName, clientId, netName, id, position, rotation, life, maxLife);
    }
  }
}
