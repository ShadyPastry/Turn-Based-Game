using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavigationSystem {
  public class Map : IMap<ITile> {
    private ITile[][] map { get; set; }
    public int Rows { get; }
    public int Cols { get; }

    public Map(ITile[][] map) {
      this.map = new ITile[map.Length][];

      int? maxCol = null;
      for (int r = 0; r < map.Length; r++) {
        ITile[] row = map[r];
        this.map[r] = new ITile[row.Length];
        foreach (ITile tile in row) {
          this.map[tile.Row][tile.Col] = row[tile.Col];
          if (maxCol == null || tile.Col > maxCol) {
            maxCol = tile.Col;
          }
        }
      }
      if (maxCol == null) {
        throw new System.Exception("Invalid map provided; must have at least 1 row and column");
      }

      Rows = this.map.Length;
      Cols = maxCol.Value + 1;
    }

    public int ColumnsInRow(int row) {
      return map[row].Length;
    }

    public ITile CheckMap(int row, int col) {
      if (0 <= row && row < map.Length) {
        if (0 <= col && col < map[row].Length) {
          return map[row][col];
        }
      }

      return null;
    }

    public IEnumerator<ITile[]> GetEnumerator() {
      foreach (ITile[] row in map) {
        yield return row;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      yield return GetEnumerator();
    }

    //private class Graph {
    //  public struct Node {
    //    public readonly int row, col;
    //    public readonly int elevation;

    //    public Node(Tile tile) {
    //      row = tile.Row;
    //      col = tile.Col;
    //      elevation = tile.Elevation;
    //    }

    //    public override bool Equals(object obj) {
    //      if (!(obj is Node)) { return false; }
    //      Node node = (Node)obj;
    //      return node.row == row && node.col == col && node.elevation == elevation;
    //    }

    //    public override int GetHashCode() {
    //      int hash = 13;
    //      hash = (hash * 7) + row.GetHashCode();
    //      hash = (hash * 7) + col.GetHashCode();
    //      hash = (hash * 7) + elevation.GetHashCode();
    //      return hash;
    //    }
    //  }

    //  private readonly Map map;
    //  private readonly Node[][] nodeMap;

    //  public Graph(Map map) {
    //    this.map = map;
    //    nodeMap = new Node[map.map.Length][];
    //    for (int r = 0; r < map.map.Length; r++) {
    //      nodeMap[r] = new Node[map.map[r].Length];
    //      for (int c = 0; c < nodeMap[r].Length; c++) {
    //        nodeMap[r][c] = new Node(map.map[r][c]);
    //      }
    //    }
    //  }

    //  public List<Direction> ShortestPath(Controller controller, Node start, Node destination) {
    //    throw new System.NotImplementedException();
    //  }
    //}
  }
}