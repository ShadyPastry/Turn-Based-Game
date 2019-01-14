using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public interface IRune {
    //Does not change over the lifetime of the object
    uint Id { get; }

    string Name { get; }

    System.Collections.ObjectModel.ReadOnlyCollection<RuneEnergy> Energies { get; }
    int AlignmentChange { get; }
    //int IdealRank { get; }
    //int IdealLinkDistance { get; }
    //int IdealLinkCount { get; }
  }
}
