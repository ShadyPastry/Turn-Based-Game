using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {
  [SerializeField] private Player player;
  [SerializeField] private int rows;
  [SerializeField] private int cols;

  //These should be set in the inspector with prefabs
  [SerializeField] private MinimapButton minimapButtonPrefab;
  [SerializeField] private GameObject youAreHere;
  [SerializeField] private GameObject playerTarget;

  private MinimapButton[][] buttons;

  private void Awake() {
    //Replace prefab with an instantiation of said prefab
    youAreHere = Instantiate(youAreHere);
    playerTarget = Instantiate(playerTarget);

    buttons = new MinimapButton[rows][];
    for (int r = rows-1; r >= 0; r--) {
      buttons[r] = new MinimapButton[cols];
      for (int c = 0; c < cols; c++) {
        MinimapButton b = Instantiate(minimapButtonPrefab);
        buttons[r][c] = b;
        b.transform.SetParent(gameObject.transform, false);
        if (r == rows/2 && c == cols/2) {
          youAreHere.transform.SetParent(b.transform, false);
        }

        b.button.onClick.AddListener(() => {
          player.SetTarget(b.Row, b.Col);
          playerTarget.SetActive(true);
          playerTarget.transform.SetParent(b.transform, false);
        });
      }
    }
  }

  private void Start() {
    StartCoroutine(WaitForPlayer());
  }

  private IEnumerator WaitForPlayer() {
    yield return new WaitUntil(() => player.ListenPositionChanged(OnPlayerPositionChanged));
    OnPlayerPositionChanged();
  }

  private void OnPlayerPositionChanged() {
    player.ClearTarget();
    playerTarget.SetActive(false);

    int startRow = player.Row - rows / 2, startCol = player.Col - cols / 2;
    int endRow = startRow + rows, endCol = startCol + cols;
    for (int r = startRow; r < endRow; r++) {
      for (int c = startCol; c < endCol; c++) {
        buttons[r-startRow][c-startCol].SetPosition(r, c);
      }
    }
  }
}
