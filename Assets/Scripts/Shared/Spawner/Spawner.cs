using UnityEngine;

public class Spawner : MonoBehaviour
{
  public float radius;
  public SpawnPoint[] spawnPoints;

  float _currentRadius;

  void Update()
  {
    if (spawnPoints != null)
    {
      if (_currentRadius != radius)
      {
        foreach (var spawnPoint in spawnPoints)
        {
          spawnPoint.radius = radius;
        }
      }
      _currentRadius = radius;
    }
  }

  public SpawnPoint RandomSpawnPoint()
  {
    if (spawnPoints != null)
    {
      var index = Random.Range(0, spawnPoints.Length - 1);
      return spawnPoints[index];
    }
    return null;
  }

  public Vector3 GetPosition()
  {
    var spawnPoint = RandomSpawnPoint();
    if (spawnPoint != null)
    {
      return spawnPoint.GetPosition();
    }
    return Vector3.zero;
  }
}
