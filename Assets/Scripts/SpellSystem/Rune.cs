using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  //Runes are used by Spellbook and SpellPage
  //When added to Spellbook, they change Spellbook.Energies and Spellbook.Alignment
  //See SpellPage for details on how Runes affect SpellPage
  public abstract class Rune : IRune {
    //If you want to be able to "level up" Runes, then add a Rune-specific "RuneUpgrade : ISpellbookModifier, IUpgradeable" to Spellbook

    //public static readonly Rune Heat = new BasicRune(0, SpellAttribute.Fire, "Heat");
    //public static readonly Rune Chill = new BasicRune(1, SpellAttribute.Ice, "Chill");
    //public static readonly Rune Shock = new BasicRune(2, SpellAttribute.Volt, "Shock");
    //public static readonly Rune Empower = new BasicRune(3, SpellAttribute.Power, "Empower");
    //public static readonly Rune Stabilize = new BasicRune(4, SpellAttribute.Stability, "Stabilize");
    //public static readonly Rune Refactor = new BasicRune(5, SpellAttribute.Efficiency, "Refactor");

    //public static readonly Rune Flare = new UnstableRune(6, SpellAttribute.Fire, "Flare");
    //public static readonly Rune Flurry = new UnstableRune(7, SpellAttribute.Ice, "Flurry");
    //public static readonly Rune Surge = new UnstableRune(8, SpellAttribute.Volt, "Surge");

    public uint Id { get; }
    public abstract string Name { get; }
    public System.Collections.ObjectModel.ReadOnlyCollection<RuneEnergy> Energies { get; }

    public Rune(uint id, List<RuneEnergy> energies) {
      Id = id;
      Energies = new System.Collections.ObjectModel.ReadOnlyCollection<RuneEnergy>(new List<RuneEnergy>(energies));
    }

    public abstract int AlignmentChange { get; }
  }

  public struct RuneEnergy {
    public readonly SpellAttribute attr;
    public readonly int energy;
    public RuneEnergy(SpellAttribute attr, int energy) {
      this.attr = attr;
      this.energy = energy;
    }

    public override bool Equals(object obj) {
      if (!(obj is RuneEnergy)) {
        return false;
      }
      RuneEnergy other = (RuneEnergy)obj;
      return other.attr == attr && other.energy == energy;
    }

    public override int GetHashCode() {
      int hash = 13;
      hash = hash * 7 + attr.GetHashCode();
      hash = hash * 7 + energy.GetHashCode();
      return hash;
    }
  }
}