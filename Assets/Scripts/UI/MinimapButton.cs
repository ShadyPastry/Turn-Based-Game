using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapButton : MonoBehaviour {

  public UnityEngine.UI.Button button;
  public int Row { get; private set; }
  public int Col { get; private set; }

  private Color noTileColor = new Color(0f, 0f, 0f, 0.8f);

  private void Awake() {
    button = GetComponent<UnityEngine.UI.Button>();
  }

  public void SetPosition(int row, int col) {
    Row = row;
    Col = col;

    GameTile t = GameManager.GameMap.CheckMap(Row, Col);
    if (t == null) {
      button.image.color = noTileColor;
    }
    else {
      button.image.color = t.GetComponent<Renderer>().material.color;
    }
  }
}
