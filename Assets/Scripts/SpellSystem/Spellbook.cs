using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  //TODO: Add, view, and remove ISpellbookModifier objects
  //TODO: Add, activate, deactivate, and remove SpellPage objects
  public class Spellbook : MonoBehaviour {

    private readonly Dictionary<SpellAttribute, int> energies;
    private readonly List<Rune> runes; //Still not sure what the purpose of this is
    private readonly List<ISpellbookModifier> modifiers;
    //private readonly ISpellUser owner;

    //TODO: Takes in an ISpellUser or something so we can modify TP, look at the caster's state, etc.
    public Spellbook() {
      //this.spellUser = spellUser;
      energies = new Dictionary<SpellAttribute, int>();
      runes = new List<Rune>();
      modifiers = new List<ISpellbookModifier>();
      Reset();
    }

    public Dictionary<SpellAttribute, int> Energies { get { return new Dictionary<SpellAttribute, int>(energies); } }

    public void AddRune(Rune rune) {
      //Bonuses from any modifiers
      foreach (ISpellbookModifier modifier in modifiers) {
        foreach (RuneEnergy runeEnergy in modifier.RuneEnergyBonuses(rune)) {
          energies[runeEnergy.attr] += runeEnergy.energy;
        }
      }

      //Base energy from rune
      foreach (RuneEnergy runeEnergy in rune.Energies) {
        energies[runeEnergy.attr] += runeEnergy.energy;
      }

      runes.Add(rune);
    }

    public void Reset() {
      foreach (SpellAttribute energy in energies.Keys) {
        energies[energy] = 0;
      }
      runes.Clear();
    }
  }
}