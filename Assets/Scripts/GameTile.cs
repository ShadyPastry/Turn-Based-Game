using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavigationSystem;
using NavigationSystem.Directions;

public class GameTile : MonoBehaviour, SkillSystem.ITargetable, ITile {
  public static readonly int elevationScale = 10; //The argument "elevation" in Initialize() is multiplied by this
  public static readonly float width = 3, height = 3;

  public int Row { get; private set; }
  public int Col { get; private set; }
  public int Elevation { get; private set; }
  private bool isInitialized = false;

  private void Start() {
    transform.localScale = new Vector3(width, Elevation / elevationScale, height);
    transform.position = new Vector3(Col * width, ((float)Elevation) / 2 / elevationScale, Row * height);

    Renderer renderer = GetComponent<Renderer>();
    Color color = renderer.material.color;
    renderer.material.color = renderer.material.color.Randomize(0.9f, 1.1f);

    if (!isInitialized) {
      throw new System.Exception("Must invoke GameTile.Initialize() when initializing");
    }
  }

  public void Initialize(int row, int col, int elevation) {
    if (isInitialized) {
      throw new System.Exception("GameTile has already been initialized");
    }

    Row = row;
    Col = col;
    Elevation = elevation * elevationScale;
    isInitialized = true;
  }

  public static Vector3 WorldPosition(int row, int col, int elevation) {
    return new Vector3(col * width, ((float)elevation) / elevationScale, row * height);
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