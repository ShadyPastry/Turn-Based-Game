using System.Collections.Generic;

namespace TurnSystem {
  //A Turn is a series of Steps, with a cap on how much time that series of steps can take
  public class Turn {
    public readonly List<Step> steps;
    public readonly int maxTime;

    public Turn(List<Step> steps, int maxTime) {
      this.steps = new List<Step>(steps);
      this.maxTime = maxTime;
    }
  }
}
