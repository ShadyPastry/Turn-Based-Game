using NavigationSystem.Directions;

namespace NavigationSystem {
  public interface IController<M, T> 
    where M : IMap<T> 
    where T : ITile {

    IMap<T> Map { get; }
    int Row { get; }
    int Col { get; }
    int Elevation { get; }
    Direction? Climbing { get; }

    MoveInfo<T>? CheckMove(Direction direction, ref string failureInfo);
    MoveInfo<T> PerformMove(Direction direction);
  }

  public static class IControllerExtensions {
    public static bool IsClimbing<M, T>(this IController<M, T> controller)
      where M : IMap<T>
      where T : ITile {
      return controller.Climbing.HasValue;
    }

    public static PositionInfo<T> CurrentPosition<M, T>(this IController<M, T> controller)
      where M : IMap<T>
      where T : ITile {
      return new PositionInfo<T>(controller.Map.CheckMap(controller.Row, controller.Col), controller.Elevation, controller.Climbing);
    }

    public static MoveInfo<T>? CheckMove<M, T>(this IController<M, T> controller, Direction direction)
      where M : IMap<T>
      where T : ITile {
      string _ignore = null;
      return controller.CheckMove(direction, ref _ignore);
    }
  }

  public enum MoveType {
    Walk,
    StartClimbUpwards, StartClimbDownwards,
    ClimbUpwards, ClimbDownwards,
    FinishClimbUpwards, FinishClimbDownwards,
    ClimbSideways, ClimbAroundOuterEdge, ClimbAroundInnerEdge
  }

  public struct MoveInfo<T> where T : ITile {
    public readonly PositionInfo<T> initialPos, finalPos;
    public readonly MoveType moveType;

    internal MoveInfo(PositionInfo<T> initialPos, PositionInfo<T> finalPos, MoveType moveType) {
      this.initialPos = initialPos;
      this.finalPos = finalPos;
      this.moveType = moveType;
    }
  }

  public struct PositionInfo<T> where T : ITile {
    public readonly T positionedAbove;
    public readonly int elevation;
    public readonly Direction? climbing;

    internal PositionInfo(T positionedAbove, int elevation, Direction? climbing) {
      this.positionedAbove = positionedAbove;
      this.elevation = elevation;
      this.climbing = climbing;
    }
  }
}
