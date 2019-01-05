using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageSystem;
using SkillSystem;

public class NormalSlash : SkillStep {

  //TODO: Account for SkillLevel
  public override int ExpectedTime { get; } = 200;
  public override int ExpectedTimeStdDev { get; } = 20;
  public override int MinTimeRequiredToStart { get; } = 200;

  public override bool CheckActionPrerequisites(ref string failureInfo) {
    GameTile targetTile = GameManager.GameMap.CheckMap(Owner.Row, Owner.Col);
    if (targetTile == null) {
      failureInfo = "ERROR: User is out of bounds!";
      return false;
    }

    if (Owner.IsClimbing) {
      failureInfo = "Cannot use Slash while climbing";
      return false;
    }

    return true;
  }

  protected override IEnumerator SkillEffect() {
    GameTile targetTile = GameManager.GameMap.CheckMap(Owner.Row, Owner.Col);
    yield return SoundManager.PlaySoundAndWait(SoundManager.Sound.CUT);
    Damage.DamageAbove(DamageTypes.Cut, 5, targetTile, 1f, v => v.Equals(Owner));
  }
}
