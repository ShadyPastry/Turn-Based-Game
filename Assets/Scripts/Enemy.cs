using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnSystem;
using NavigationSystem.Directions;

//TODO: Improve pathfinding (have Controller expose MoveInfo)
public class Enemy : MonoBehaviour, ITurnTaker, DamageSystem.IDamageable {

  public Vector2Int? Target { get; private set; }

  //Maximum and current HP
  [SerializeField] private int _maxHp; private int _currentHp;
  public int MaxHp { get { return _maxHp; } private set { _maxHp = value; statChanged.Invoke(); } }
  public int CurrentHp { get { return _currentHp; } private set { _currentHp = value; statChanged.Invoke(); } }

  //Maximum and current TP
  [SerializeField] private int _maxTp; private int _currentTp;
  public int MaxTp { get { return _maxTp; } private set { _maxTp = value; statChanged.Invoke(); } }
  public int CurrentTp { get { return _currentTp; } private set { _currentTp = value; statChanged.Invoke(); } }

  public int TurnSpeed { get; private set; } = 7;

  private TurnTakerController controller;
  public bool IsClimbing { get { return controller.IsClimbing; } }

  private Player player;

  private void Awake() {
    CurrentHp = MaxHp;
    CurrentTp = MaxTp;
    Target = null;
    player = FindObjectOfType<Player>();
  }

  private void Start() {
    GameManager.RegisterForTurn(this);
    int r = Random.Range(0, GameManager.GameMap.Rows);
    int c = Random.Range(0, GameManager.GameMap.ColumnsInRow(r));
    controller = new TurnTakerController(transform, GameManager.GameMap, r, c);
  }

  //private void OnPreRender() {
  //  controller.ToggleMoveAnimation(false);
  //}

  //private void OnWillRenderObject() {
  //  controller.ToggleMoveAnimation(true);
  //}

  public bool CanAct() {
    return CurrentHp > 0;
  }

  public Turn OnTurn() {
    var steps = new List<Step>();
    if (CurrentHp > 0) {
      //Direction? direction = DirectionExtensions.RelativeDirection(controller.Row, controller.Col, player.Row, player.Col);
      Direction? direction = DirectionExtensions.RelativeDirection(transform.position, player.transform.position);
      if (direction == null) {
        if (controller.Elevation > player.Elevation && player.IsClimbing && player.Climbing == controller.Climbing) {
          direction = controller.Climbing.Value.Opposite();
        } else {
          direction = player.Climbing;
        }
      }

      if (direction != null) {
        Step moveStep = controller.Move(direction.Value);
        if (moveStep != null) {
          steps.Add(moveStep);
        }
      }

      if (!controller.IsClimbing) steps.Add(AttackMyTile());
    }

    return new Turn(steps, GameManager.standardTurnTime);
  }

  private Step AttackMyTile() {
    return new NormalSlash(this);
  }
  private class NormalSlash : Step {
    private Enemy enemy;
    public override int ExpectedTime { get; } = 200;
    public override int ExpectedTimeStdDev { get; } = 20;
    public override int MinTimeRequiredToStart { get; } = 200;
    public NormalSlash(Enemy enemy) {
      this.enemy = enemy;
    }
    public override bool CheckActionPrerequisites(ref string failureMessage) {
      return !enemy.controller.IsClimbing;
    }
    protected override IEnumerator StepBehavior() {
      DamageSystem.Damage.DamageAbove(DamageSystem.DamageTypes.Cut, 5, GameManager.GameMap.CheckMap(enemy.controller.Row, enemy.controller.Col), 1f, v => !v.Equals(enemy.player));
      yield return null;
    }
  }

  public IEnumerator PrepareOnTurn() {
    //Could do other things.  Play a sound/animation, publish a message, etc.
    yield return null;
  }

  public void DamageHp(DamageSystem.DamageTypes damageType, int baseDamage) {
    int finalDamage = baseDamage;
    CurrentHp = Mathf.Max(CurrentHp - finalDamage, 0);
    if (CurrentHp == 0) {
      GameManager.UnregisterForTurn(this);
      player.AddExperience(Random.Range(50, 75));
      Destroy(gameObject);
    }
  }

  public void DamageTp(int baseDamage) {
    CurrentTp = Mathf.Max(CurrentTp - baseDamage, 0);
  }


  //
  //Published events
  //

  //For use by the UI
  //The actual current/max HP can be read via Player's public properties
  private UnityEngine.Events.UnityEvent statChanged = new UnityEngine.Events.UnityEvent();
  public void ListenStatChanged(UnityEngine.Events.UnityAction onEvent) { statChanged.AddListener(onEvent); }

  //For use by the UI
  //The actual row/col/elevation can be read via Player's public properties, which in turn read from Player.controller
  public bool ListenPositionChanged(UnityEngine.Events.UnityAction onEvent) {
    if (controller == null) return false;
    controller.ListenPositionChanged(onEvent);
    return true;
  }
}
