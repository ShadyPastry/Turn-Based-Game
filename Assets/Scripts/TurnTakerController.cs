using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavigationSystem;
using NavigationSystem.Directions;
using TurnSystem;

//Integrates NavigationSystem, TurnSystem, and Unity's Transform object
public class TurnTakerController : IController<GameMap, GameTile> {
  private readonly TransformController controller;

  public IMap<GameTile> Map => controller.Map;
  public int Row => controller.Row;
  public int Col => controller.Col;
  public int Elevation => controller.Elevation;
  public Direction? Climbing => controller.Climbing;
  public bool IsClimbing => controller.IsClimbing();

  public TurnTakerController(Transform transform, GameMap map, int initialRow, int initialCol) {
    controller = new TransformController(transform, map, initialRow, initialCol);
  }

  public void ListenPositionChanged(UnityEngine.Events.UnityAction onEvent) {
    controller.ListenPositionChanged(onEvent);
  }

  public Step Move(Direction direction, ref string failureInfo) {
    Step result = new MoveStep(controller, direction);
    if (result.CheckActionPrerequisites(ref failureInfo)) {
      return result;
    } else {
      return null;
    }
  }
  public Step Move(Direction direction) {
    string _ignore = null;
    return Move(direction, ref _ignore);
  }

  public MoveInfo<GameTile>? CheckMove(Direction direction, ref string failureInfo) {
    return controller.CheckMove(direction, ref failureInfo);
  }

  public MoveInfo<GameTile> PerformMove(Direction direction) {
    return ((IController<GameMap, GameTile>)controller).PerformMove(direction);
  }

  private class MoveStep : Step {
    public override int ExpectedTime { get; }
    public override int ExpectedTimeStdDev { get; }
    public override int MinTimeRequiredToStart { get; }

    private readonly TransformController positionController;
    private readonly Direction direction;

    public MoveStep(TransformController positionController, Direction direction) {
      this.positionController = positionController;
      this.direction = direction;
    }

    public override bool CheckActionPrerequisites(ref string failureMessage) {
      return positionController.CheckMove(direction, ref failureMessage) != null;
    }

    protected override IEnumerator StepBehavior() {
      yield return positionController.PerformMove(direction);
    }
  }
}
