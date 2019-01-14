using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public abstract class BasicRune : Rune {
    public override string Name { get; }
    public override int AlignmentChange { get; } = 0;

    public BasicRune(uint id, SpellAttribute type, string name) 
      : base(id, new List<RuneEnergy>() { new RuneEnergy(type, 10) }) {
      Name = name;
    }
  }
}