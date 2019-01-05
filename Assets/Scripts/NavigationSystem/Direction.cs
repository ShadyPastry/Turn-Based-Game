using UnityEngine;

namespace NavigationSystem.Directions {
  public enum Direction { NORTH, EAST, SOUTH, WEST } //WARNING: DO NOT EXTEND OR REORDER; EXTENSIONS DEPEND ON THIS
  public static class DirectionExtensions {
    public static bool IsOrthogonal(this Direction dir1, Direction dir2) {
      int d1 = (int)dir1, d2 = (int)dir2;
      return (d1 + d2) % 2 == 1;
    }

    public static Direction RotateClockwise(this Direction direction) {
      return (Direction)((int)direction + 1);
    }

    public static Direction Opposite(this Direction direction) {
      return (Direction)(((int)direction + 2) % 4);
    }

    public static int Sign(this Direction direction) {
      return 1 - 2 * ((int)direction / 2);
    }

    public static bool IsNorthOrSouth(this Direction direction) {
      return direction == Direction.NORTH || direction == Direction.SOUTH;
    }

    //Ignores y-value of the input vectors
    public static Direction? RelativeDirection(Vector3 startPosition, Vector3 targetPosition) {
      Vector3 direction = targetPosition - startPosition;
      direction.y = 0;
      if (direction == Vector3.zero) {
        return null;
      }
      if (Vector3.Angle(direction, Vector3.forward) <= 45.0) {
        return Direction.NORTH;
      } else if (Vector3.Angle(direction, Vector3.right) <= 45.0) {
        return Direction.EAST;
      } else if (Vector3.Angle(direction, Vector3.back) <= 45.0) {
        return Direction.SOUTH;
      } else {
        return Direction.WEST;
      }
    }

    public static Direction? RelativeDirection(int row0, int col0, int rowf, int colf) {
      if (rowf > row0) {
        return Direction.NORTH;
      } else if (rowf < row0) {
        return Direction.SOUTH;
      } else if (colf > col0) {
        return Direction.EAST;
      } else if (colf < col0) {
        return Direction.WEST;
      } else {
        return null;
      }
    }
  }
}