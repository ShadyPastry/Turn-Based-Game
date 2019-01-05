using System.Collections;
using UnityEngine;
using DamageSystem;
using SkillSystem;

public class Spark : SkillStep {

  //Constant across all instances
  private static GameObject gfxPrefab = SkillData.SparkGfx;

  //May change as the skill levels up
  public int TpCost { get; private set; }

  //TODO: Account for SkillLevel
  public override int ExpectedTime { get; } = 200;
  public override int ExpectedTimeStdDev { get; } = 20;
  public override int MinTimeRequiredToStart { get; } = 200;

  public override bool CheckActionPrerequisites(ref string failureMessage) {
    if (Owner.CurrentTp < TpCost) {
      failureMessage = "Insufficient TP";
      return false;
    }

    if (Owner.Target == null) {
      failureMessage = "No target selected";
      return false;
    }

    GameTile targetTile = GameManager.GameMap.CheckMap(Owner.Target.Row, Owner.Target.Row);
    if (targetTile == null) {
      failureMessage = "ERROR: Target has an invalid position";
      return false;
    }

    return true;
  }

  protected override IEnumerator SkillEffect() {
    Owner.DamageTp(TpCost);

    GameObject sparks = Object.Instantiate(gfxPrefab);
    GameTile targetTile = GameManager.GameMap.CheckMap(Owner.Target.Row, Owner.Target.Col);

    sparks.transform.position = targetTile.transform.position + new Vector3(0, targetTile.transform.lossyScale.y / 2, 0);
    //yield return SoundManager.PlaySoundAndWait(SoundManager.Sound.VOLT);

    Damage.DamageAbove(DamageTypes.Volt, Owner.Foc, targetTile, 1f, Damage.noImmunity);
    yield return null;
  }
}
