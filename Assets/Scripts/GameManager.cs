using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnSystem;
using NavigationSystem;

public class GameManager : MonoBehaviour {

  public static readonly int standardTurnTime = 600;

  [SerializeField] private UnityEngine.UI.Text gameOverText;
  [SerializeField] private Chatbox chatbox;
  [SerializeField] private TextAsset[] levels;
  [SerializeField] private GameObject grassPrefab;
  [SerializeField] private GameObject icePrefab;

  private static GameManager S { get; set; }
  private static TurnManager turns;

  private static bool autoMakeNextMove = true;
  private static bool makeNextMove = true;

  public static GameMap GameMap { get; private set; }

  private enum State { OVER, PLAYING }
  private static State state;

  private void Awake() {
    S = this;
    state = State.PLAYING;
    turns = new TurnManager();

    Dictionary<string, GameObject> tilePrefabs = new Dictionary<string, GameObject>() {
      { "g", grassPrefab },
      { "i", icePrefab }
    };

    GameMap = new GameMap(levels[0], tilePrefabs);
    PublishMessage("Level 0 is simple.  Enemies will spawn; kill them and level up!");
    PublishMessage("You win if you reach level 5");
  }

  private void Start() {
    StartCoroutine(TurnRoutine());
    SoundManager.ToggleBgm(true);
  }

  public static void ResetGame(bool isWin) {
    state = State.OVER;
    S.gameOverText.gameObject.SetActive(true);
    S.gameOverText.text = isWin ? "^_^ You Win ^_^\nPress 'R' to restart" : "v_v You Lose v_v\nPress 'R' to restart";
    S.StartCoroutine(ResetGameRoutine());
  }
  private static IEnumerator ResetGameRoutine() {
    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));
    UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
  }

  public static void PublishMessage(string message) { S.chatbox.AddMessage(message); }

  //Turn progression
  public static void ToggleAutoMakeNextMove(bool setTrue) { autoMakeNextMove = setTrue; }
  public static void MakeNextMove() { makeNextMove = true; }

  //Registering for turns
  public static void RegisterForTurn(ITurnTaker turnTaker) { turns.Register(turnTaker); }
  public static void UnregisterForTurn(ITurnTaker turnTaker) { turns.Unregister(turnTaker); }
  public static void RegisterReaction(Step reaction) { turns.AddReaction(reaction); }

  private static IEnumerator TurnRoutine() {
    while (true) {
      yield return new WaitUntil(() => state == State.PLAYING && (makeNextMove || autoMakeNextMove) && !turns.IsEmpty);
      makeNextMove = false;
      yield return turns.PerformNextTurn();
    }
  }
}
