using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavigationSystem;
using NavigationSystem.Directions;

public class GameTile : MonoBehaviour, SkillSystem.ITargetable, ITile {
  public static readonly float width = 3, height = 3;

  public int Row { get { return tile.Row; } }
  public int Col { get { return tile.Col; } }
  public int Elevation { get { return tile.Elevation; } }

  private Tile tile;

  private void Start() {
    transform.localScale = new Vector3(width, Elevation / Tile.elevationSegments, height);
    transform.position = new Vector3(Col * width, ((float)Elevation) / 2 / Tile.elevationSegments, Row * height);

    Renderer renderer = GetComponent<Renderer>();
    Color color = renderer.material.color;
    renderer.material.color = renderer.material.color.Randomize(0.9f, 1.1f);

    if (tile == null) {
      throw new System.Exception("Must invoke GameTile.Initialize() when initializing");
    }
  }

  public void Initialize(Tile tile) {
    if (this.tile != null) {
      throw new System.Exception("GameTile has already been initialized");
    }
    if (tile == null) {
      throw new System.Exception("Argument 'tile' cannot be null");
    }
    this.tile = tile;
  }

  public static Vector3 WorldPosition(int row, int col, int elevation) {
    return new Vector3(col * width, ((float)elevation) / Tile.elevationSegments, row * height);
  }

  public Vector3 WorldPosition(int elevation) {
    return WorldPosition(Row, Col, elevation);
  }

  private float GetEdgeCoord(Direction direction, float inwardsMargin) {
    switch (direction) {
      case Direction.NORTH:
        return transform.position.z + height / 2 - inwardsMargin;
      case Direction.EAST:
        return transform.position.x + width / 2 - inwardsMargin;
      case Direction.SOUTH:
        return transform.position.z - height / 2 + inwardsMargin;
      case Direction.WEST:
        return transform.position.x - width / 2 + inwardsMargin;
      default:
        throw new System.Exception("Missing switch branch");
    }
  }

  public void SlideXzToEdge(ref Vector3 position, Direction slideDirection, Vector3 inwardsMargin) {
    if (slideDirection.IsNorthOrSouth()) {
      position.z = GetEdgeCoord(slideDirection, inwardsMargin.z);
    } else {
      position.x = GetEdgeCoord(slideDirection, inwardsMargin.x);
    }
  }
}