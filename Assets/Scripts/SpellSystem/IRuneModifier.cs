using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  //Modifies the energy adjustments made to an ISpellbook when ISpellbook.AddRune() is called
  public interface IRuneModifier {
    //An ADDITIVE modifier that can add or remove additional RuneEnergy based on rune
    IEnumerable<RuneEnergy> RuneEnergyBonuses(Rune rune);
  }
}