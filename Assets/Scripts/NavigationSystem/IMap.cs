using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavigationSystem.Directions;

namespace NavigationSystem {
  public interface IMap<T> : IEnumerable<T[]> where T : ITile {
    int Rows { get; }
    int Cols { get; }
    int ColumnsInRow(int row);

    T CheckMap(int row, int col);
  }

  public static class IMapExtensions {
    public static T CheckMap<T>(this IMap<T> map, int row, int col, Direction direction) where T : ITile {
      switch (direction) {
        case Direction.NORTH:
          return map.CheckMap(row + 1, col);
        case Direction.EAST:
          return map.CheckMap(row, col + 1);
        case Direction.SOUTH:
          return map.CheckMap(row - 1, col);
        case Direction.WEST:
          return map.CheckMap(row, col - 1);
        default:
          throw new System.Exception("Missing switch branch");
      }
    }
  }
}