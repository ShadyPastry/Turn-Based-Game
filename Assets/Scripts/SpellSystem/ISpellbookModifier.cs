using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public interface ISpellbookModifier {
    //An ADDITIVE modifier that awards additional RuneEnergy based on rune
    IEnumerable<RuneEnergy> RuneEnergyBonuses(Rune rune);
  }
}