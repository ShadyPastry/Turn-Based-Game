using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem {
  public interface ISkillUser : DamageSystem.IDamageable {
    ITargetable Target { get; }

    int Row { get; }
    int Col { get; }
    float Elevation { get; }
    bool IsClimbing { get; }

    int Str { get; }
    int Vit { get; }
    int Foc { get; }
    int Spi { get; }
    int Spd { get; }

    //void ConsumeTp(int baseCost);
  }
}
