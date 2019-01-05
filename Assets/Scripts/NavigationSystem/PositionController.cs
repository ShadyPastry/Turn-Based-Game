using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NavigationSystem.Directions;

namespace NavigationSystem {
  //TODO: Support falling
  public class PositionController<M, T> : IController<M, T> 
    where M : IMap<T>
    where T : ITile {

    public IMap<T> Map => map;
    private readonly M map;

    //TODO: Allow these to be customized
    //private int fallIncrement = 5 * Tile.elevationSegments;
    private int climbIncrement = Tile.elevationSegments;

    //When on top of a tile's edge, horizontal climbing has many edge cases to consider (pun not intended)
    //We handle them by avoiding them.  ^.-
    private static readonly int ledgeOffset = 2; //Must be between 1 and Tile.elevationSegments

    //Position information
    private T _positionedAbove;
    public T PositionedAbove { get { return _positionedAbove; } private set { _positionedAbove = value; positionChanged.Invoke(); } }
    public int Row => PositionedAbove.Row;
    public int Col => PositionedAbove.Col;
    private int _elevation;
    public int Elevation { get { return _elevation; } private set { _elevation = value; positionChanged.Invoke(); } }

    //Position changed event
    private UnityEvent positionChanged = new UnityEvent();
    public void ListenPositionChanged(UnityAction onEvent) { positionChanged.AddListener(onEvent); }

    //Climb state
    public Direction? Climbing { get; private set; } //We are climbing a tile N/S/E/W of positionedAbove or nothing at all

    //Constructor
    public PositionController(M map, int initialRow, int initialCol) {
      this.map = map;

      PositionedAbove = map.CheckMap(initialRow, initialCol);
      if (PositionedAbove == null) {
        throw new System.ArgumentException("The specified initial position is invalid");
      }

      Elevation = PositionedAbove.Elevation;
      Climbing = null;
    }

    //Move in a direction
    public MoveInfo<T> PerformMove(Direction direction) {
      MoveInfo<T>? info = this.CheckMove(direction);
      if (info == null) {
        throw new System.Exception("Attempting to perform an illegal move");
      }

      //Update state
      PositionedAbove = map.CheckMap(info.Value.finalPos.positionedAbove.Row, info.Value.finalPos.positionedAbove.Col);
      Elevation = info.Value.finalPos.elevation;
      Climbing = info.Value.finalPos.climbing;

      return info.Value;
    }

    //Get information pertaining to moving in a direction
    public MoveInfo<T>? CheckMove(Direction direction, ref string failureInfo) {
      //Legality check
      if (this.IsClimbing()) {
        if (Climbing.Value.IsOrthogonal(direction) && !CanClimbHorizontally(direction, ref failureInfo)) {
          return null;
        }
      } else {
        T movingTowards = map.CheckMap(PositionedAbove.Row, PositionedAbove.Col, direction);
        if (movingTowards == null) {
          failureInfo = "Moving out of bounds";
          return null;
        }
      }

      if (this.IsClimbing()) {
        return Climbing.Value.IsOrthogonal(direction) ? ClimbHorizontally(direction)
                                                      : ClimbVertically(direction);
      } else {
        T movingTowards = map.CheckMap(PositionedAbove.Row, PositionedAbove.Col, direction);
        float elevationDifference = movingTowards.Elevation - Elevation;
        if (Mathf.Abs(elevationDifference) > ledgeOffset) {
          return StartClimbing(direction, movingTowards);
        } else {
          return Walk(direction);
        }
      }
    }

    private bool CanClimbHorizontally(Direction direction, ref string failureInfo) {
      T currentlyClimbing = map.CheckMap(PositionedAbove.Row, PositionedAbove.Col, Climbing.Value);
      if (currentlyClimbing == null) {
        throw new System.Exception("Illegal state: Climbing a non-existent Tile");
      }

      T adjacentTile = map.CheckMap(PositionedAbove.Row, PositionedAbove.Col, direction);
      if (adjacentTile == null) {
        failureInfo = "Cannot climb past the edge of the map";
        return false;
      }

      T diagonalTile = map.CheckMap(adjacentTile.Row, adjacentTile.Col, Climbing.Value);
      if (diagonalTile == null) {
        throw new System.Exception("Illegal map: Non-rectangular maps are not supported");
      }

      return true;
    }

    private MoveInfo<T> ClimbHorizontally(Direction direction) {
      T currentlyClimbing = map.CheckMap(PositionedAbove.Row, PositionedAbove.Col, Climbing.Value);
      T adjacentTile = map.CheckMap(PositionedAbove.Row, PositionedAbove.Col, direction);
      T diagonalTile = map.CheckMap(adjacentTile.Row, adjacentTile.Col, Climbing.Value);

      MoveType moveType;
      T willBeClimbing, willBePositionedAbove;
      if (adjacentTile.Elevation >= Elevation) {
        //adjacentTile is higher than me, so I climb onto its face
        willBeClimbing = adjacentTile;
        willBePositionedAbove = PositionedAbove;
        moveType = MoveType.ClimbAroundInnerEdge;

      } else if (diagonalTile.Elevation >= Elevation) {
        //adjacentTile is lower than me, and diagonalTile is higher than me, so I climb onto diagonalTile's face
        willBeClimbing = diagonalTile;
        willBePositionedAbove = adjacentTile;
        moveType = MoveType.ClimbSideways;

      } else {
        //adjacentTile is below me, and diagonalTile is below me, so I climb around currentlyClimbing
        willBeClimbing = currentlyClimbing;
        willBePositionedAbove = diagonalTile;
        moveType = MoveType.ClimbAroundOuterEdge;
      }

      Direction? newClimbing = willBeClimbing.RelativePosition(willBePositionedAbove);

      PositionInfo<T> finalPos = new PositionInfo<T>(willBePositionedAbove, Elevation, newClimbing);
      return new MoveInfo<T>(this.CurrentPosition(), finalPos, moveType);
    }

    private MoveInfo<T> ClimbVertically(Direction direction) {
      T beingClimbed = map.CheckMap(PositionedAbove.Row, PositionedAbove.Col, direction);

      PositionInfo<T> finalPos;
      MoveType moveType;
      if (direction == Climbing) {
        //Climbing upwards
        if (Elevation >= beingClimbed.Elevation - ledgeOffset) {
          finalPos = new PositionInfo<T>(beingClimbed, beingClimbed.Elevation, null);
          moveType = MoveType.FinishClimbUpwards;
        } else {
          int newElevation = Mathf.Min(Elevation + climbIncrement, beingClimbed.Elevation - ledgeOffset);
          finalPos = new PositionInfo<T>(PositionedAbove, newElevation, Climbing);
          moveType = MoveType.ClimbUpwards;
        }

      } else if (direction.Opposite() == Climbing) {
        //Climbing downwards
        if (Elevation <= PositionedAbove.Elevation + ledgeOffset) {
          finalPos = new PositionInfo<T>(PositionedAbove, PositionedAbove.Elevation, null);
          moveType = MoveType.FinishClimbDownwards;
        } else {
          int newElevation = Mathf.Max(Elevation - climbIncrement, PositionedAbove.Elevation + ledgeOffset);
          finalPos = new PositionInfo<T>(PositionedAbove, newElevation, Climbing);
          moveType = MoveType.ClimbDownwards;
        }

      } else {
        //Non-vertical climb
        throw new System.Exception("ClimbVertically called for a non-vertical climb");
      }

      return new MoveInfo<T>(this.CurrentPosition(), finalPos, moveType);
    }

    private MoveInfo<T> StartClimbing(Direction direction, T movingTowards) {
      Direction? newClimbing;
      int newElevation;
      T newPositionedAbove;

      MoveType moveType;
      float elevationDifference = movingTowards.Elevation - Elevation;
      if (elevationDifference < 0) {
        //Starting a downwards climb (interpreted as the end of an upwards climb)
        newPositionedAbove = movingTowards;
        newElevation = Elevation - ledgeOffset;
        newClimbing = direction.Opposite();
        moveType = MoveType.StartClimbDownwards;
      } else {
        //Starting an upwards climb
        newPositionedAbove = PositionedAbove;
        newElevation = Elevation + ledgeOffset;
        newClimbing = direction;
        moveType = MoveType.StartClimbUpwards;
      }

      PositionInfo<T> finalPos = new PositionInfo<T>(newPositionedAbove, newElevation, newClimbing);
      return new MoveInfo<T>(this.CurrentPosition(), finalPos, moveType);
    }

    private MoveInfo<T> Walk(Direction direction) {
      T newPositionedAbove = map.CheckMap(PositionedAbove.Row, PositionedAbove.Col, direction);
      if (newPositionedAbove == null) {
        throw new System.Exception("Illegal walk destination");
      }

      PositionInfo<T> finalPos = new PositionInfo<T>(newPositionedAbove, newPositionedAbove.Elevation, null);
      return new MoveInfo<T>(this.CurrentPosition(), finalPos, MoveType.Walk);
    }
  }
}