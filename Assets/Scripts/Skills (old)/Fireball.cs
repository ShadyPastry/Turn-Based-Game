using System.Collections;
using UnityEngine;
using DamageSystem;
using SkillSystem;

public class Fireball : SkillStep {

  //Constant across all instances
  private static GameObject gfxPrefab = SkillData.FireballGfx;

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
    //TODO: Account for SkillLevel

    Owner.DamageTp(TpCost);

    GameObject fireball = Object.Instantiate(gfxPrefab);
    GameTile targetTile = GameManager.GameMap.CheckMap(Owner.Target.Row, Owner.Target.Col);

    fireball.transform.position = targetTile.transform.position + new Vector3(0, targetTile.transform.lossyScale.y/2, 0);
    yield return SoundManager.PlaySoundAndWait(SoundManager.Sound.FIRE);

    Damage.DamageAbove(DamageTypes.Fire, Owner.Foc, targetTile, 1f, Damage.noImmunity);
  }
}
