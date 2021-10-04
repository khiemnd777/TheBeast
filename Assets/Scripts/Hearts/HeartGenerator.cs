using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Net;
using Net.Socket;
using UnityEngine;

public class HeartGenerator : MonoBehaviour
{
  public ProbabilityHeartField[] probabilityHeartField;

  Settings _settings;

  Probability<ProbabilityHeartField> _probability;

  void Start()
  {
    _settings = Settings.instance;

    if (!_settings.isServer) return;

    // Init probability
    var hearts = probabilityHeartField.Select(x => x).ToArray();
    var percents = probabilityHeartField.Select(x => x.percent).ToArray();
    _probability = new Probability<ProbabilityHeartField>();
    _probability.Initialize(hearts, percents);
  }

  public DroppedHeart Generate(Vector3 position, Quaternion rotation, float generatedRadius = 0)
  {
    if (!_settings.isServer) return null;
    var field = _probability.GetValueInProbability();
    var spawnPosition = position + Random.insideUnitSphere * generatedRadius;
    var heart = NetIdentity.InstantiateServerAndEverywhere(field.prefabName, field.heart, new Vector3(spawnPosition.x, 0, spawnPosition.z), rotation, null, null, true);
    Debug.Log($"{field.prefabName}: {heart.id}");
    return heart;
  }
}

[System.Serializable]
public struct ProbabilityHeartField
{
  public string prefabName;
  public DroppedHeart heart;
  public float percent;
}
