using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnSystem;

public class EnemySpawner : MonoBehaviour, ITurnTaker {

  [SerializeField] private Enemy enemyPrefab;
  private int numEnemiesToSpawn = 3;

  private List<Enemy> spawnedEnemies = new List<Enemy>();

  public int TurnSpeed { get; } = 5;

  private void Start() {
    GameManager.RegisterForTurn(this);
  }

  private bool CanAct() {
    return numEnemiesToSpawn > 0;
  }

  public Turn OnTurn() {
    var steps = new List<Step>();
    if (CanAct()) {
      steps.Add(SpawnEnemy());
    }

    return new Turn(steps, 0);
  }

  public IEnumerator PrepareOnTurn() {
    //Could do other things.  Play a sound/animation, publish a message, etc.
    yield return null;
  }

  private Step SpawnEnemy() {
    return new SpawnEnemyStep(this);
  }
  private class SpawnEnemyStep : Step {
    public override int ExpectedTime { get; } = 0;
    public override int ExpectedTimeStdDev { get; } = 0;
    public override int MinTimeRequiredToStart { get; } = 0;
    public override bool CheckActionPrerequisites(ref string failureMessage) { return true; }

    private readonly EnemySpawner enemySpawner;

    public SpawnEnemyStep(EnemySpawner enemySpawner) {
      this.enemySpawner = enemySpawner;
    }

    protected override IEnumerator StepBehavior() {
      enemySpawner.numEnemiesToSpawn--;
      Enemy enemy = Instantiate(enemySpawner.enemyPrefab);
      enemySpawner.spawnedEnemies.Add(enemy);
      enemy.ListenStatChanged(() => enemySpawner.OnEnemyStatChanged(enemy));

      GameManager.PublishMessage("Enemy spawned!");

      yield return null;
    }
  }

  private void OnEnemyStatChanged(Enemy enemy) {
    if (enemy.CurrentHp == 0 && spawnedEnemies.Contains(enemy)) {
      spawnedEnemies.Remove(enemy);
      GameManager.PublishMessage("You killed an enemy!");
      if (spawnedEnemies.Count == 0 && numEnemiesToSpawn == 0) {
        GameManager.PublishMessage("Enemy wave cleared!");
        numEnemiesToSpawn = Random.Range(2, 10);
      }
    }
  }
}
