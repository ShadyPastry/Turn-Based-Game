using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NavigationSystem;
using NavigationSystem.Directions;

//Integrates NavigationSystem with Unity's Transform object
public class TransformController : IController<GameMap, GameTile> {
  private readonly PositionController<GameMap, GameTile> controller;
  private readonly Transform transform;

  public IMap<GameTile> Map => controller.Map;
  public int Row => controller.Row;
  public int Col => controller.Col;
  public int Elevation => controller.Elevation;
  public Direction? Climbing => controller.Climbing;

  public void ListenPositionChanged(UnityAction onEvent) { controller.ListenPositionChanged(onEvent); }

  public TransformController(Transform transform, GameMap map, int initialRow, int initialCol) {
    this.transform = transform;
    controller = new PositionController<GameMap, GameTile>(map, GameTile.elevationScale, initialRow, initialCol);

    GameTile initialPositionedAbove = controller.CurrentPosition().positionedAbove;
    Vector3 initialPosition = initialPositionedAbove.WorldPosition(controller.CurrentPosition().elevation);
    initialPosition.y += transform.lossyScale.y / 2;
    transform.position = initialPosition;
  }

  public MoveInfo<GameTile>? CheckMove(Direction direction, ref string failureInfo) {
    return controller.CheckMove(direction, ref failureInfo);
  }

  //TODO: Rather than fixed time, maybe fixed speed would be preferable?  Or support both, let caller choose.
  //Move in a direction.  First updates Controller's state and then animates the move.
  public IEnumerator PerformMove(Direction direction) {
    MoveInfo<GameTile> info = controller.PerformMove(direction);

    List<Vector3> waypoints = ComputeAnimationWaypoints(direction, info);

    //Calculate the speed we should move at so that the animation takes only moveAnimationTime seconds
    waypoints.Insert(0, transform.position);
    float totalMoveDistance = 0f;
    for (int i = 1; i < waypoints.Count; i++) {
      totalMoveDistance += (waypoints[i] - waypoints[i - 1]).magnitude;
    }
    float speed = totalMoveDistance / 0.25f;

    foreach (var waypoint in waypoints) {
      while (transform.position != waypoint) {
        transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
        yield return null;
      }
    }
  }

  private List<Vector3> ComputeAnimationWaypoints(Direction moveDirection, MoveInfo<GameTile> moveInfo) {
    List<Vector3> result = new List<Vector3>();

    //Climbing around an edge is the only time we need an intermediate waypoint for animation
    if (moveInfo.moveType == MoveType.ClimbAroundOuterEdge) {
      Vector3 outerEdge = moveInfo.initialPos.positionedAbove.WorldPosition(Elevation);
      outerEdge.y += transform.lossyScale.y / 2;
      moveInfo.initialPos.positionedAbove.SlideXzToEdge(ref outerEdge, moveInfo.initialPos.climbing.Value, transform.lossyScale / 2);
      moveInfo.initialPos.positionedAbove.SlideXzToEdge(ref outerEdge, moveDirection, -transform.lossyScale / 2);
      result.Add(outerEdge);

    } else if (moveInfo.moveType == MoveType.ClimbAroundInnerEdge) {
      Vector3 innerEdge = moveInfo.initialPos.positionedAbove.WorldPosition(Elevation);
      innerEdge.y += transform.lossyScale.y / 2;
      moveInfo.initialPos.positionedAbove.SlideXzToEdge(ref innerEdge, moveInfo.initialPos.climbing.Value, transform.lossyScale / 2);
      moveInfo.initialPos.positionedAbove.SlideXzToEdge(ref innerEdge, moveDirection, transform.lossyScale / 2);
      result.Add(innerEdge);
    }

    GameTile finalPositionedAbove = moveInfo.finalPos.positionedAbove;
    Vector3 finalPosition = finalPositionedAbove.WorldPosition(moveInfo.finalPos.elevation);
    if (moveInfo.finalPos.climbing != null) {
      finalPositionedAbove.SlideXzToEdge(ref finalPosition, moveInfo.finalPos.climbing.Value, transform.lossyScale / 2);
    }
    finalPosition.y += transform.lossyScale.y / 2;
    result.Add(finalPosition);
    return result;
  }

  MoveInfo<GameTile> IController<GameMap, GameTile>.PerformMove(Direction direction) {
    return controller.PerformMove(direction);
  }
}