using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavigationSystem;

public class ElevationCompass : MonoBehaviour {

#pragma warning disable 0649
  [SerializeField] private Player player;
  [SerializeField] private UnityEngine.UI.Text center;
  [SerializeField] private UnityEngine.UI.Text north;
  [SerializeField] private UnityEngine.UI.Text south;
  [SerializeField] private UnityEngine.UI.Text east;
  [SerializeField] private UnityEngine.UI.Text west;

  [SerializeField] private UnityEngine.UI.Text northwest;
  [SerializeField] private UnityEngine.UI.Text northeast;
  [SerializeField] private UnityEngine.UI.Text southwest;
  [SerializeField] private UnityEngine.UI.Text southeast;
#pragma warning restore 0649

  private const string intFormat = "+#; -#; 0"; //For floats"+#.0;-#.0; 0.0";

  private void Start() {
    StartCoroutine(WaitForPlayer());
  }

  private IEnumerator WaitForPlayer() {
    yield return new WaitUntil(() => player.ListenPositionChanged(OnPlayerPositionChanged));
    OnPlayerPositionChanged();
  }

  private void OnPlayerPositionChanged() {
    GameTile northTile = GameManager.GameMap.CheckMap(player.Row + 1, player.Col);
    GameTile southTile = GameManager.GameMap.CheckMap(player.Row - 1, player.Col);
    GameTile eastTile = GameManager.GameMap.CheckMap(player.Row, player.Col + 1);
    GameTile westTile = GameManager.GameMap.CheckMap(player.Row, player.Col - 1);

    GameTile nwTile = GameManager.GameMap.CheckMap(player.Row + 1, player.Col - 1);
    GameTile neTile = GameManager.GameMap.CheckMap(player.Row + 1, player.Col + 1);
    GameTile swTile = GameManager.GameMap.CheckMap(player.Row - 1, player.Col - 1);
    GameTile seTile = GameManager.GameMap.CheckMap(player.Row - 1, player.Col + 1);

    center.text = player.Elevation.ToString(intFormat) + "\n" + GameManager.GameMap.CheckMap(player.Row, player.Col).Elevation.ToString(intFormat);
    north.text = northTile == null ? "" : northTile.Elevation.ToString(intFormat);
    south.text = southTile == null ? "" : southTile.Elevation.ToString(intFormat);
    east.text = eastTile == null ? "" : eastTile.Elevation.ToString(intFormat);
    west.text = westTile == null ? "" : westTile.Elevation.ToString(intFormat);

    northwest.text = nwTile == null ? "" : nwTile.Elevation.ToString(intFormat);
    northeast.text = neTile == null ? "" : neTile.Elevation.ToString(intFormat);
    southwest.text = swTile == null ? "" : swTile.Elevation.ToString(intFormat);
    southeast.text = seTile == null ? "" : seTile.Elevation.ToString(intFormat);
  }
}
