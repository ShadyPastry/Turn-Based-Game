using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public abstract partial class Rune {
    private class BasicRune : Rune {
      public override int AlignmentChange { get; } = 0;

      public BasicRune(SpellAttribute type) 
        : base(new List<RuneEnergy>() { new RuneEnergy(type, 10) }) {
      }
    }
  }
}