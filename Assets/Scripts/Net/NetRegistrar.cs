using System;
using System.Linq;
using Net.Socket;
using UnityEngine;

namespace Net
{
  public class NetRegistrar : MonoBehaviour
  {
    public event Action<int, int> onPlayerCount;
    public event Action onLocalPlayerRegistered;
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

    void InitEvents()
    {
      // This will be fired after the connection was completely connected.
      _networkManager.onConnected += () =>
      {
        print("Initializing other players...");
        socket.Emit(Constants.EVENT_SERVER_LOAD_PLAYERS);
      };
      if (_settings.isServer)
      {
        _networkManager.onServerRegister += (NetRegisterJSON netRegisterJson) =>
        {
          print($"Register {netRegisterJson.prefabName}'s '{netRegisterJson.netName}' for client-id '{netRegisterJson.clientId}'");
          CreateAtTheServerSide(netRegisterJson.prefabName, netRegisterJson.clientId, netRegisterJson.netName);
        };
      }
      else
      {
        _networkManager.onClientRegisterFinished += (NetObjectJSON netObjJson) =>
        {
          // Detects the local player at the client-side.
          if (netObjJson.clientId.Equals(_networkManager.clientId.ToString()))
          {
            if (onLocalPlayerRegistered != null)
            {
              onLocalPlayerRegistered();
            }
          }
          CreateAtTheClientSide(netObjJson.prefabName, netObjJson.clientId, netObjJson.netName, netObjJson.id, netObjJson.position, netObjJson.rotation, netObjJson.life, netObjJson.maxLife);
        };
      }
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
    void Create(string prefabName, string clientId, string netName, int id, float[] position, float[] rotation, float life, float maxLife)
    {
      if (netObjectList.Exists(id)) return;
      var prefab = netIdentifierPrefabs.FirstOrDefault(x => x.name == prefabName);
      if (prefab.netIdentityPrefab)
      {
        var netIdentifierPrefab = prefab.netIdentityPrefab;
        var netIdentityIns = Instantiate<NetIdentity>(
          netIdentifierPrefab,
          Utility.PositionArrayToVector3(Vector3.zero, position),
          Utility.AnglesArrayToQuaternion(rotation));
        netIdentityIns.Init(id, netName);
        netObjectList.Store(netIdentityIns);
        netIdentityIns.life = life;
        netIdentityIns.maxLife = maxLife;
      }
    }

    /// <summary>
    /// Create other net player by prefab.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    void CreateOther(string prefabName, int id, string name, float hp)
    {
      if (netObjectList.Exists(id)) return;
      var prefab = netIdentifierPrefabs.FirstOrDefault(x => x.name == prefabName);
      if (prefab.netIdentityPrefab)
      {
        var netIdentifierPrefab = prefab.netIdentityPrefab;
        var netIdentityIns = Instantiate<NetIdentity>(netIdentifierPrefab, Vector3.zero, Quaternion.identity);
        netIdentityIns.InitOther(id, name);
        netObjectList.Store(netIdentityIns);
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

    void CreateAtTheClientSide(string prefabName, string clientId, string netName, int id, float[] position, float[] rotation, float life, float maxLife)
    {
      if (!_settings.isClient) return;
      var isLocalPlayer = clientId.Equals(_networkManager.clientId.ToString());
      if (isLocalPlayer)
      {
        Create(prefabName, clientId, netName, id, position, rotation, life, maxLife);
        return;
      }
      var prefab = netIdentifierPrefabs.FirstOrDefault(x => x.name == prefabName);
      if (prefab.netIdentityPrefab)
      {
        var netIdentifierPrefab = prefab.netIdentityPrefab;
        var netId = netObjectList.Create(
          netIdentifierPrefab,
          id,
          netName,
          life,
          maxLife,
          Utility.PositionArrayToVector3(Vector3.zero, position),
          Utility.AnglesArrayToQuaternion(rotation),
          false
        );
        netObjectList.Store(netId);
      }
    }
  }
}
