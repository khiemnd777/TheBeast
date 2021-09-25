using UnityEngine;
using UnityEngine.UI;

public class ScoreItem : MonoBehaviour
{
  [SerializeField]
  Text _nicknameText;

  [SerializeField]
  Text _scoreText;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void Reset()
  {
    _nicknameText.text = "";
    _scoreText.text = "";
  }


  public void UpdateScore(int score)
  {
    _scoreText.text = score.ToString();
  }

  public void UpdateNickname(string nickname)
  {
    _nicknameText.text = nickname;
  }
}
