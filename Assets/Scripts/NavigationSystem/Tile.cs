namespace NavigationSystem {
  public class Tile : ITile {
    public static readonly int elevationSegments = 10; //1 unit of elevation is divided into this many segments

    public int Row { get; }
    public int Col { get; }
    public int Elevation { get; }

    public Tile(int row, int col, int elevation) {
      Row = row;
      Col = col;
      Elevation = elevationSegments * elevation;
    }
  }
}