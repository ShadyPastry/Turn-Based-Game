using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavigationSystem;

public class GameMap : IMap<GameTile> {
  private readonly Map<GameTile> tileMap;

  public int Rows { get { return tileMap.Rows; } }
  public int Cols { get { return tileMap.Cols; } }

  public GameMap(TextAsset mapText, Dictionary<string, GameObject> tilePrefabs) {
    tileMap = new Map<GameTile>(GenerateMap(mapText, tilePrefabs));
  }

  public int ColumnsInRow(int row) {
    return tileMap.ColumnsInRow(row);
  }

  public GameTile CheckMap(int row, int col) {
    return tileMap.CheckMap(row, col);
  }

  public IEnumerator<GameTile[]> GetEnumerator() {
    return tileMap.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return tileMap.GetEnumerator();
  }

  private static GameTile[][] GenerateMap(TextAsset mapText, Dictionary<string, GameObject> tilePrefabs) {
    GameObject tileHolder = new GameObject("Tile Holder");
    GameTile[][] result;
    string[] lines = mapText.text.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
    result = new GameTile[lines.Length][];
    for (int r = 0; r < lines.Length; r++) {
      string[] entries = lines[lines.Length - 1 - r].Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries); //Splits by whitespace
      result[r] = new GameTile[entries.Length];
      for (int c = 0; c < entries.Length; c++) {
        result[r][c] = ParseTile(entries[c], r, c, tilePrefabs);
        result[r][c].transform.parent = tileHolder.transform;
      }
    }

    return result;
  }

  private static GameTile ParseTile(string entry, int row, int col, Dictionary<string, GameObject> tilePrefabs) {
    GameTile result = Object.Instantiate(tilePrefabs[entry]).AddComponent<GameTile>();

    Tile tile;
    if (row == 0 && col == 0) tile = new Tile(row, col, 2);
    else if (row == 1 && col == 0) tile = new Tile(row, col, 3);
    else if (row == 0 && col == 1) tile = new Tile(row, col, 3);
    else if (row == 1 && col == 1) tile = new Tile(row, col, 4);

    else if (row == 1 && col == 2) tile = new Tile(row, col, 4);
    else if (row == 1 && col == 3) tile = new Tile(row, col, 2);
    else if (row == 0 && col == 2) tile = new Tile(row, col, 2);
    else if (row == 0 && col == 3) tile = new Tile(row, col, 4);

    else tile = new Tile(row, col, Random.Range(1, 5));

    result.Initialize(tile);
    return result;
  }

  ////Work in progress; incomplete and not currently used
  //private static Tile ParseTile2(string entry, int row, int col, Dictionary<string, Tile> tilePrefabs) {
  //  string[] tokens = entry.Split(',');

  //  //Token 0 represents the kind of tile to instantiate
  //  string tilePrefabKey = tokens[0];
  //  Tile result = Object.Instantiate(tilePrefabs[tilePrefabKey]).GetComponent<Tile>();

  //  //Tokens 1 and 2 represent the minimum and maximum elevation for the tile
  //  int minElevation = Tile.elevationScale*int.Parse(tokens[1]);
  //  int maxElevation = Tile.elevationScale*int.Parse(tokens[2]);
  //  result.Initialize(row, col, Random.Range(minElevation, maxElevation));

  //  return result;
  //}
}