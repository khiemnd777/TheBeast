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

    [SerializeField]
    Spawner _spawner;

    NetworkManager _networkManager;
    ISocketWrapper socket;
    NetObjectList netObjectList;
    CameraController cameraController;
    DotSightController dotSightController;
    OtherPlayerLoadingCounter otherPlayerCounter;
    LocalPlayerManager _localPlayerManager;
    Settings _settings;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
      _settings = Settings.instance;
      _networkManager = NetworkManagerCache.networkManager;
      _localPlayerManager = FindObjectOfType<LocalPlayerManager>();
      socket = NetworkManagerCache.socket;
      netObjectList = NetObjectList.instance;
      otherPlayerCounter = new OtherPlayerLoadingCounter();
      dotSightController = FindObjectOfType<DotSightController>();
      cameraController = FindObjectOfType<CameraController>();
      InitEvents();
    }

    static object initClientObj = new object();
    void InitEvents()
    {
      // This will be fired after the connection was completely connected.
      _networkManager.onClientConnected += () =>
      {
        // print("Initializing other players...");
        // socket.Emit(Constants.EVENT_SERVER_LOAD_PLAYERS);
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
                if (dataJson.life < 0) return;
                Debug.Log($"Instantiate the object of client {dataJson.clientId}");
                var position = Point.FromArray(dataJson.position);
                var rotation = Utility.AnglesArrayToQuaternion(dataJson.rotation);
                var netObj = CreateAtTheClientSide(dataJson.prefabName, dataJson.clientId, dataJson.netName, dataJson.id, dataJson.position, dataJson.rotation, dataJson.life, dataJson.maxLife, dataJson.score);
                Debug.Log($"1: Show log if player respawn after dead by event [{dataJson.eventName}] for object {netObj.netName}");
                StartCoroutine(OnAfterInitNetObjectWithMessage(netObj, dataJson.eventName, dataJson.message));
              }
              else
              {
                var netObj = netObjectList.Find(dataJson.id);
                if (netObj)
                {
                  Debug.Log($"2: Show log if player respawn after dead by event {dataJson.eventName}");
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
          dataJson.maxLife,
          dataJson.lifetime,
          dataJson.other
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
          CreateAtTheClientSide(netObjJson.prefabName, netObjJson.clientId, netObjJson.netName, netObjJson.id, netObjJson.position, netObjJson.rotation, netObjJson.life, netObjJson.maxLife, 0);
        };
        _networkManager.onScoreBroadcast += (ScoreJson data) =>
        {
          Debug.Log($"Hey! I scored");
          var player = netObjectList.Find(data.playerNetId);
          var score = player.GetComponent<NetScore>();
          if (score)
          {
            score.ClientScore(data.score);
          }
        };
        _networkManager.onBroadcastServerCloneEverywhereJson += (NetServerCloneJSON dataJson) =>
        {
          ServerCloneToClientEverywhere(
            dataJson.prefabName,
            dataJson.netId,
            dataJson.clientId,
            dataJson.netName,
            dataJson.position,
            dataJson.rotation,
            dataJson.life,
            dataJson.maxLife,
            dataJson.lifetime,
            dataJson.other,
            dataJson.stored
          );
        };
      }
    }

    IEnumerator OnAfterInitNetObjectWithMessage(NetIdentity netObj, string eventName, string message)
    {
      yield return null;
      netObj?.OnReceiveMessage(eventName, message);
    }

    public void Register(string prefabName, string netName)
    {
      if (_settings.isServer) return;
      print("Registering...");
      socket.Emit(Constants.EVENT_REGISTER, new NetRegisterJSON(_networkManager.clientId, prefabName, netName));
    }

    public void Disenroll(NetIdentity netObj)
    {
      netObjectList.Remove(netObj);
    }

    /// <summary>
    /// Create net player by prefab.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="life"></param>
    public NetIdentity CreateLocally(string prefabName, string clientId, string netName, int id, float[] position, float[] rotation, float life, float maxLife, int score)
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
        // Score
        var netScore = netId.GetComponent<NetScore>();
        if (netScore)
        {
          netScore.score = score;
        }
        netObjectList.Store(netId);
        _localPlayerManager.SetLocalPlayer(netId);
        return netId;
      }
      return null;
    }

    public NetIdentity CreateClientOther(string prefabName, string clientId, string netName, int id, float[] position, float[] rotation, float life, float maxLife, int score)
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
        // Score
        var netScore = netId.GetComponent<NetScore>();
        if (netScore)
        {
          netScore.score = score;
        }
        netObjectList.Store(netId);
        return netId;
      }
      return null;
    }

    public void CloneEverywhere(string prefabName, string clientId, string netName, float[] position, float[] rotation, float life, float maxLife, float lifetime, string otherMessage)
    {
      if (clientId != _networkManager.clientId)
      {
        var prefab = netIdentifierPrefabs.FirstOrDefault(x => x.name == prefabName);
        if (prefab.netIdentityPrefab)
        {
          var netIdentifierPrefab = prefab.netIdentityPrefab;
          var netIdentityIns = Instantiate<NetIdentity>(
            netIdentifierPrefab,
            Utility.PositionArrayToVector3(Vector3.zero, position),
            Utility.AnglesArrayToQuaternion(rotation));
          netIdentityIns.OnCloneMessage(otherMessage);
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
          if (lifetime > 0f)
          {
            Destroy(netIdentityIns.gameObject, lifetime);
          }
        }
      }
    }

    public void ServerCloneToClientEverywhere(string prefabName, int netId, string clientId, string netName, float[] position, float[] rotation, float life, float maxLife, float lifetime, string otherMessage, bool stored)
    {
      if (!_settings.isClient) return;
      if (clientId != _networkManager.clientId)
      {
        var prefab = netIdentifierPrefabs.FirstOrDefault(x => x.name == prefabName);
        if (prefab.netIdentityPrefab)
        {
          var netIdentifierPrefab = prefab.netIdentityPrefab;
          var netIdentityIns = Instantiate<NetIdentity>(
            netIdentifierPrefab,
            Utility.PositionArrayToVector3(Vector3.zero, position),
            Utility.AnglesArrayToQuaternion(rotation));
          Debug.Log($"{prefabName}: {netId}");
          netIdentityIns.InitOther(netId, netName);
          netIdentityIns.OnCloneMessage(otherMessage);
          netIdentityIns.life = life;
          netIdentityIns.maxLife = maxLife;
          if (stored)
          {
            netObjectList.Store(netIdentityIns);
          }
          if (lifetime > 0f)
          {
            Destroy(netIdentityIns.gameObject, lifetime);
          }
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
        var spawnPosition = _spawner.GetPosition();
        Debug.Log($"spawnPosition: {spawnPosition}");
        var netId = netObjectList.Create(netIdentifierPrefab, netName, spawnPosition, Quaternion.identity);
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
            Point.FromVector3(spawnPosition),
            netId.transform.rotation
          )
        );
      }
    }

    NetIdentity CreateAtTheClientSide(string prefabName, string clientId, string netName, int id, float[] position, float[] rotation, float life, float maxLife, int score)
    {
      if (!_settings.isClient) return null;
      var isLocalPlayer = clientId.Equals(_networkManager.clientId);
      if (isLocalPlayer)
      {
        var locally = CreateLocally(prefabName, clientId, netName, id, position, rotation, life, maxLife, score);
        if (locally)
        {
          socket.Emit(Constants.EVENT_LOCALLY_REGISTER_FINISHED,
          new NetObjectJSON(
            clientId,
            locally.id,
            prefabName,
            netName,
            locally.life,
            locally.maxLife,
            Point.FromVector3(locally.transform.position),
            locally.transform.rotation
          )
        );
        }
        return locally;
      }
      return CreateClientOther(prefabName, clientId, netName, id, position, rotation, life, maxLife, score);
    }
  }
}
