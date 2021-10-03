using System.Collections;
using System.Linq;
using Net;
using UnityEngine;

public class ScoreList : MonoBehaviour
{
  [SerializeField]
  float _interval;

  [SerializeField]
  ScoreItem[] scores;

  NetObjectList _netObjectList;

  // Start is called before the first frame update
  void Start()
  {
    _netObjectList = NetObjectList.instance;

    // Init items of list to empty string.
    for (var inx = 0; inx < scores.Length; inx++)
    {
      scores[inx].Reset();
    }
    if (Settings.instance.isClient)
    {
      StartCoroutine("DisplayScores", _interval);
    }
  }

  IEnumerator DisplayScores(float interval)
  {
    while (gameObject)
    {
      yield return new WaitForSeconds(interval);
      var highScores = _netObjectList.All(x =>
      {
        var netScore = x.Value?.GetComponent<NetScore>();
        return netScore && netScore.score > 0;
      }, o => o.OrderByDescending(x => x.Value.GetComponent<NetScore>().score));
      
      var topScores = highScores.Take(7).ToList();

      for (var inx = 0; inx < topScores.Count(); inx++)
      {
        if (inx <= scores.Length - 1)
        {
          var scoreItem = scores[inx];
          var topScore = topScores[inx];
          if (topScore)
          {
            var player = topScore.GetComponent<Player>();
            if (player)
            {
              scoreItem.UpdateNickname(player.netName);
            }
            var score = topScore.GetComponent<NetScore>();
            if (score)
            {
              scoreItem.UpdateScore(score.score);
            }
          }
        }
      }
    }
  }
}
