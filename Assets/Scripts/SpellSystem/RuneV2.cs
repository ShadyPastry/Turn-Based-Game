using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem.Refactoring {
  public sealed class Rune {
    public System.Collections.ObjectModel.ReadOnlyCollection<RuneEnergy> Energies { get; }

    public readonly int AlignmentChange;

    //Not sure which, if any, of these I want
    //public readonly int IdealRank;
    //public readonly int IdealLinkDistance;
    //public readonly int IdealLinkCount;

    internal Rune(List<RuneEnergy> energies) {
      Energies = new System.Collections.ObjectModel.ReadOnlyCollection<RuneEnergy>(new List<RuneEnergy>(energies));
    }
  }

  public sealed class SpellAttribute {
    public readonly int id;
    public readonly string name;
  }

  public struct RuneEnergy {
    public readonly SpellAttribute attr;
    public readonly int energy;
    public RuneEnergy(SpellAttribute attr, int energy) {
      this.attr = attr;
      this.energy = energy;
    }
  }
}
