using NavigationSystem.Directions;

namespace NavigationSystem {
  public interface ITile {
    int Row { get; }
    int Col { get; }
    int Elevation { get; }
  }

  public static class ITileExtensions {
    public static Direction? RelativePosition(this ITile finalTile, ITile startingTile) {
      return DirectionExtensions.RelativeDirection(startingTile.Row, startingTile.Col, finalTile.Row, finalTile.Col);
    }
  }
}