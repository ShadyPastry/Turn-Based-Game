using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnSystem;
using static DamageSystem.DamageTypesExtensions;
using SkillSystem;
using NavigationSystem.Directions;

public sealed class Player : MonoBehaviour, ITurnTaker, DamageSystem.IDamageable, SkillSystem.ISkillUser {

  //Maximum and current HP
  [SerializeField] private int _maxHp; private int _currentHp;
  public int MaxHp { get { return _maxHp; } private set { _maxHp = value; statChanged.Invoke(); } }
  public int CurrentHp { get { return _currentHp; } private set { _currentHp = value; statChanged.Invoke(); } }

  //Maximum and current TP
  [SerializeField] private int _maxTp; private int _currentTp;
  public int MaxTp { get { return _maxTp; } private set { _maxTp = value; statChanged.Invoke(); } }
  public int CurrentTp { get { return _currentTp; } private set { _currentTp = value; statChanged.Invoke(); } }

  [SerializeField] private int _strength = 7;
  public int Str { get { return _strength; } private set { _strength = value; statChanged.Invoke(); } }

  [SerializeField] private int _vitality = 7;
  public int Vit { get { return _vitality; } private set { _vitality = value; statChanged.Invoke(); } }

  [SerializeField] private int _focus = 7;
  public int Foc { get { return _focus; } private set { _focus = value; statChanged.Invoke(); } }

  [SerializeField] private int _spirit = 7;
  public int Spi { get { return _spirit; } private set { _spirit = value; statChanged.Invoke(); } }

  //Affects how frequently you get a turn
  [SerializeField] private int _spd = 5;
  public int Spd { get { return _spd; } private set { _spd = value; statChanged.Invoke(); } }

  public int TurnSpeed { get { return Spd; } }

  //public float Climb { get { return (Str + 2 * Vit) / 99.0f; } }
  //public float Jump { get { return 3 * Str / 99.0f; } }

  //Controller
  [SerializeField] private Vector2Int _initialRowColCoords;
  private TurnTakerController controller;
  public Direction? Climbing { get { return controller.Climbing; } }
  public bool IsClimbing { get { return controller.IsClimbing; } }
  public int Row { get { return controller.Row; } }
  public int Col { get { return controller.Col; } }
  public float Elevation { get { return controller.Elevation; } }

  private void Start() {
    InitSkills();
    GameManager.RegisterForTurn(this);
    //statChanged.AddListener(() => controller.ChangeStats(Climb, Jump));

    controller = new TurnTakerController(transform, GameManager.GameMap, _initialRowColCoords.x, _initialRowColCoords.y);

    CurrentHp = MaxHp;
    CurrentTp = MaxTp;

    StartCoroutine(MoveRoutine());
  }

  private void Update() {
    QueueKeyPress();
    //if (Input.GetKeyDown(KeyCode.Space)) DamageHp(Damage.Type.FIRE, 999);
  }

  KeyCode? queuedKeyPress;
  private void QueueKeyPress() {
    if (Input.GetKeyDown(KeyCode.RightArrow)) {
      queuedKeyPress = KeyCode.RightArrow;
    } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
      queuedKeyPress = KeyCode.LeftArrow;
    } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
      queuedKeyPress = KeyCode.UpArrow;
    } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
      queuedKeyPress = KeyCode.DownArrow;
    }
  }

  private IEnumerator MoveRoutine() {
    while (true) {
      if (isMyTurn) {
        Direction? direction = null;
        if (Input.GetKey(KeyCode.RightArrow) || queuedKeyPress == KeyCode.RightArrow) {
          direction = Direction.EAST;
        } else if (Input.GetKey(KeyCode.LeftArrow) || queuedKeyPress == KeyCode.LeftArrow) {
          direction = Direction.WEST;
        } else if (Input.GetKey(KeyCode.UpArrow) || queuedKeyPress == KeyCode.UpArrow) {
          direction = Direction.NORTH;
        } else if (Input.GetKey(KeyCode.DownArrow) || queuedKeyPress == KeyCode.DownArrow) {
          direction = Direction.SOUTH;
        }

        queuedKeyPress = null;

        if (direction.HasValue) {
          actionSetter = () => {
            turnActions.Clear();
            Step moveAction = controller.Move(direction.Value, ref actionFailedInfo);
            if (moveAction != null) {
              turnActions.Add(moveAction);
            }
          };

          ConfirmAction();
        }
      }

      yield return new WaitForSeconds(0.1f);
    }
  }


  //
  //Combat
  //

  private int experience = 0;
  public void AddExperience(int baseExp) {
    GameManager.PublishMessage("Got " + baseExp + " experience!");

    //Max level is 10
    if (experience / 100 > 5) {
      GameManager.PublishMessage("But you already reached the maximum level...  Guess you win!");
      GameManager.ResetGame(true);
      return;
    }

    int oldExperience = experience;
    baseExp = Mathf.Max(0, baseExp);
    experience += baseExp;

    //Level up every 100 experience
    for (int i = 0; i < (experience / 100) - (oldExperience / 100); i++) {
      GameManager.PublishMessage("Leveled up!  New level is " + experience / 100);
      CurrentHp = MaxHp;
      CurrentTp = MaxTp;
      Str += Random.Range(0, 3);
      Vit += Random.Range(0, 3);
      Foc += Random.Range(0, 3);
      Spi += Random.Range(0, 3);
      Spd += Random.Range(0, 3);
    }
  }

  public void DamageHp(DamageSystem.DamageTypes damageType, int baseDamage) {
    int finalDamage = baseDamage;

    bool isElemental = damageType.IsElemental(), isPhysical = damageType.IsPhysical();
    if (isElemental && isPhysical) {
      finalDamage -= Mathf.Min(Spi, Vit);
    } else if (isElemental) {
      finalDamage -= Spi;
    } else if (isPhysical) {
      finalDamage -= Vit;
    }

    if (finalDamage < 10) {
      finalDamage = Random.Range(1, 10);
    }

    CurrentHp = Mathf.Max(CurrentHp - finalDamage, 0);
    if (CurrentHp == 0) {
      StartCoroutine(OnDeath());
    } else {
      SoundManager.PlaySound(SoundManager.Sound.DAMAGE);
    }
  }

  private IEnumerator OnDeath() {
    GameManager.UnregisterForTurn(this);
    yield return SoundManager.PlaySoundAndWait(SoundManager.Sound.DEATH);
    GameManager.ResetGame(false);
  }

  public void DamageTp(int baseDamage) {
    CurrentTp = Mathf.Max(CurrentTp - baseDamage, 0);
  }

  public ITargetable Target { get; private set; }
  public void SetTarget(int targetRow, int targetCol) {
    Target = GameManager.GameMap.CheckMap(targetRow, targetCol);
  }
  public void ClearTarget() {
    Target = null;
  }


  //
  //Taking a turn
  //

  private readonly List<Step> turnActions = new List<Step>();
  private bool isMyTurn = false;

  //Waits until Player successfully confirms its action via ConfirmAction
  public IEnumerator PrepareOnTurn() {
    isMyTurn = true;
    yield return new WaitUntil(() => actionConfirmed);
  }

  //Attempts to confirm an action
  public void ConfirmAction() {
    actionSetter();
    if (turnActions.Count > 0) {
      actionConfirmed = true;
      isMyTurn = false;
    } else {
      if (actionFailedInfo != null) {
        GameManager.PublishMessage("Action failed: " + actionFailedInfo);
        actionFailedInfo = null;
      } else {
        GameManager.PublishMessage("Action failed");
      }
    }
  }
  private bool actionConfirmed = false;

  //Whenever I get a turn, perform Player.action immediately and then register for a new turn.
  public Turn OnTurn() {
    var result = new Turn(turnActions, GameManager.standardTurnTime);
    turnActions.Clear();
    actionConfirmed = false;
    return result;
  }


  //
  //Action Setters
  //

  private delegate void ActionSetter();
  private ActionSetter actionSetter;
  private string actionFailedInfo;

  public void UsePassTurn() {
    actionSetter = () => {
      turnActions.Clear();
      turnActions.Add(new Pass());
    };
  }
  private class Pass : Step {
    public override int ExpectedTime { get; } = 0;
    public override int ExpectedTimeStdDev { get; } = 0;
    public override int MinTimeRequiredToStart { get; } = 0;
    public override bool CheckActionPrerequisites(ref string failureMessage) { return true; }
    protected override IEnumerator StepBehavior() { yield return null; }
  }

  private Skill<Fireball> fireball;
  private Skill<Spark> spark;
  private Skill<NormalSlash> normalSlash;
  private void InitSkills() {
    fireball = new Skill<Fireball>(this, 10);
    spark = new Skill<Spark>(this, 10);
    normalSlash = new Skill<NormalSlash>(this, 10);
  }

  public void UseFireball() {
    actionSetter = () => {
      turnActions.Clear();
      Fireball step = fireball.Use();
      if (step.CheckActionPrerequisites(ref actionFailedInfo)) {
        turnActions.Add(step);
      }
    };
  }

  public void UseSpark() {
    actionSetter = () => {
      turnActions.Clear();
      Spark step = spark.Use();
      if (step.CheckActionPrerequisites(ref actionFailedInfo)) {
        turnActions.Add(step);
      }
    };
  }

  public void UseNormalSlash() {
    actionSetter = () => {
      turnActions.Clear();
      NormalSlash step = normalSlash.Use();
      if (step.CheckActionPrerequisites(ref actionFailedInfo)) {
        turnActions.Add(step);
      }
    };
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
