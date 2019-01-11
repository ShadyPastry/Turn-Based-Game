using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionPanel : MonoBehaviour {

#pragma warning disable 0649
  [SerializeField] private Player player;
#pragma warning restore 0649
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
