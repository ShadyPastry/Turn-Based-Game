using System.Collections;
using System.Collections.Generic;

namespace TurnSystem {
  public class TurnManager {
    //Stores metadata about an ITurnTaker's turn priority
    private class TurnTakerInfo {
      public readonly ITurnTaker turnTaker;
      public int Priority { get { return (additionalPriority + 1) * turnTaker.TurnSpeed; } }
      private int additionalPriority;

      public TurnTakerInfo(ITurnTaker turnTaker) {
        this.turnTaker = turnTaker;
        additionalPriority = 0;
      }
      public void IncreasePriority() { additionalPriority += 1; }
      public void ResetPriority() { additionalPriority = 0; }
    }
    private readonly List<TurnTakerInfo> turnInfo = new List<TurnTakerInfo>();

    public bool IsEmpty { get { return turnInfo.Count == 0; } }

    public void Register(ITurnTaker turnTaker) {
      turnInfo.Add(new TurnTakerInfo(turnTaker));
    }

    public void Unregister(ITurnTaker turnTaker) {
      turnInfo.RemoveAll(info => info.turnTaker == turnTaker);
    }

    public void Peek(int n) {
      var turnInfoCopy = new List<TurnTakerInfo>(turnInfo);
      List<TurnTakerInfo> result = new List<TurnTakerInfo>();
      for (int i = 0; i < n; i++) {
        result.Add(GetFastestAndReorder(turnInfoCopy));
      }

      //Should return a list of wrapped ITurnTaker objects, I suppose?
      throw new System.NotImplementedException();
    }

    //Get the TurnTakerInfo with the highest priority and increase the priority of those that remain
    private TurnTakerInfo GetFastestAndReorder(List<TurnTakerInfo> turnInfo) {
      //Find and store the TurnTakerInfo with the highest priority
      int fastestIndex = 0;
      for (int i = 0; i < turnInfo.Count; i++) {
        if (turnInfo[i].Priority > turnInfo[fastestIndex].Priority) { fastestIndex = i; }
      }
      var result = turnInfo[fastestIndex];

      //Adjust the priority of all the TurnTakerInfos
      for (int i = 0; i < turnInfo.Count; i++) {
        if (i == fastestIndex) { turnInfo[i].ResetPriority(); } else {
          turnInfo[i].IncreasePriority();
        }
      }

      return result;
    }

    public IEnumerator PerformNextTurn() {
      ITurnTaker nextTurnTaker = GetFastestAndReorder(turnInfo).turnTaker;

      yield return nextTurnTaker.PrepareOnTurn();
      Turn turn = nextTurnTaker.OnTurn();

      int timeTaken = 0;
      foreach (Step step in turn.steps) {

        //Perform the next step if enough time remains
        if (step.MinTimeRequiredToStart + timeTaken > turn.maxTime) { break; }
        yield return step.Action();
        timeTaken += step.TimeTaken();

        foreach (var reaction in reactions) {
          //TODO: Add some sort of indicator that what's occurring is a reaction
          yield return reaction.Action();
        }
        reactions.Clear();
      }
    }

    private readonly List<Step> reactions = new List<Step>();
    public void AddReaction(Step reaction) {
      reactions.Add(reaction);
    }
  }
}
