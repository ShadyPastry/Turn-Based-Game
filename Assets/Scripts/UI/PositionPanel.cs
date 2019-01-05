using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPanel : MonoBehaviour {

  [SerializeField] private Player player;
  private UnityEngine.UI.Text positionText;

  private void Awake() {
    positionText = GetComponent<UnityEngine.UI.Text>();
  }

  private void Start() {
    StartCoroutine(WaitForPlayer());
  }

  private IEnumerator WaitForPlayer() {
    yield return new WaitUntil(() => player.ListenPositionChanged(OnPlayerPositionChanged));
    OnPlayerPositionChanged();
  }

  private void OnPlayerPositionChanged() {
    float alt = GameManager.GameMap.CheckMap(player.Row, player.Col).Elevation;
    positionText.text = "" +
      "Row  " + player.Row + "\n" +
      "Col  " + player.Col + "\n" +
      "Alt " + alt.ToString("+#.0;-#.0; 0.0");
  }
}
