using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  //Runes are used by Spellbook and SpellPage
  //When added to Spellbook, they change Spellbook.Energies and Spellbook.Alignment
  //See SpellPage for details on how Runes affect SpellPage
  public abstract partial class Rune {
    //If you want to be able to "level up" Runes, then add a Rune-specific "RuneUpgrade : ISpellbookModifier, IUpgradeable" to Spellbook

    public static readonly Rune Heat = new BasicRune(SpellAttribute.Fire);
    public static readonly Rune Chill = new BasicRune(SpellAttribute.Ice);
    public static readonly Rune Shock = new BasicRune(SpellAttribute.Volt);
    public static readonly Rune Empower = new BasicRune(SpellAttribute.Power);
    public static readonly Rune Stabilize = new BasicRune(SpellAttribute.Stability);
    public static readonly Rune Refactor = new BasicRune(SpellAttribute.Efficiency);

    public static readonly Rune Flare = new UnstableRune(SpellAttribute.Fire);
    public static readonly Rune Flurry = new UnstableRune(SpellAttribute.Ice);
    public static readonly Rune Surge = new UnstableRune(SpellAttribute.Volt);

    //Guaranteed never to change
    public System.Collections.ObjectModel.ReadOnlyCollection<RuneEnergy> Energies { get; }

    private Rune(List<RuneEnergy> energies) {
      Energies = new System.Collections.ObjectModel.ReadOnlyCollection<RuneEnergy>(new List<RuneEnergy>(energies));
    }

    public abstract int AlignmentChange { get; }

    //public abstract int IdealRank { get; }
    //public abstract int IdealLinkDistance { get; }
    //public abstract int IdealLinkCount { get; }
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