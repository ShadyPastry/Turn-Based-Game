using System.Collections;

namespace TurnSystem {
  //A Step represents an atomic action in a turn of a turn-based game
  //  IMPORTANT: Step.Action() may only be called once; repeat calls will throw an exception
  //
  //Step.Action() takes time to complete; the time taken can vary, but can be accessed via TimeTaken()
  //  IMPORTANT: Action() must first be completed, otherwise TimeTaken() will throw an exception
  public abstract class Step {
    //This information should be available to the player as they're deciding what steps to take on their turn
    public abstract int ExpectedTime { get; }
    public abstract int ExpectedTimeStdDev { get; }
    public abstract int MinTimeRequiredToStart { get; }

    private bool actionCalled = false;
    private bool actionCompleted = false;

    public Step() {
      if (ExpectedTime - 3 * ExpectedTimeStdDev < 0) {
        throw new System.Exception("ExpectedTime - 3*ExpectedTimeStdDev cannot be less than 0");
      }
    }

    //The action to be performed this step
    //Can only be called once, subsequent attempts will throw an exception
    public IEnumerator Action() {
      if (actionCalled) {
        throw new System.Exception("Step objects cannot be reused");
      }

      actionCalled = true;
      yield return StepBehavior();
      actionCompleted = true;
    }
    public abstract bool CheckActionPrerequisites(ref string failureMessage);
    protected abstract IEnumerator StepBehavior();

    //How long it took for Action() to be completed
    //Action() must first be completed, otherwise an exception will be thrown
    public int TimeTaken() {
      if (!actionCompleted) {
        throw new System.Exception(actionCalled ? "Although Action() has been called, it has not been completed" : "Action() must first be completed");
      }

      float variation = RandomFromDistribution.RandomRangeNormalDistribution(
        -3 * ExpectedTimeStdDev,
        3 * ExpectedTimeStdDev,
        RandomFromDistribution.ConfidenceLevel_e._998);
      return ExpectedTime + (int)variation;
    }
  }
}
