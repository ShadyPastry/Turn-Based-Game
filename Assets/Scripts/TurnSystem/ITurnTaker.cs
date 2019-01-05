using System.Collections;

namespace TurnSystem {
  public interface ITurnTaker {
    int TurnSpeed { get; }
    IEnumerator PrepareOnTurn();
    Turn OnTurn();
  }
}
