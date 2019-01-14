using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public class UnstableRune : Rune {
    public override string Name { get; }
    public override int AlignmentChange { get; } = 10;

    public UnstableRune(uint id, SpellAttribute type, string name) : base(id, EnergiesFromType(type)) {
      Name = name;
    }

    //Helper function whose purpose is to make the constructor is more readable
    private static List<RuneEnergy> EnergiesFromType(SpellAttribute type) {
      if (!type.IsElemental()) {
        throw new System.Exception("type.IsElemental() must be true");
      }
      return new List<RuneEnergy>() {
        new RuneEnergy(type, 10),
        new RuneEnergy(SpellAttribute.Power, 10),
        new RuneEnergy(SpellAttribute.Stability, -10)
      };
    }
  }
}