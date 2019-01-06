using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  //TODO: Add/remove and view ISpellbookModifier objects
  //TODO: Add/remove, activate/deactivate, and view SpellPage objects
  //  Should activate/deactivate be part of SpellSystem?  Or is it something specific to the main-game?
  /**
   * A Spellbook maintains "energy" amounts for each SpellAttribute
   * These amounts are changed by adding Runes via AddRune()
   * 
   * ISpellbookModifers influence the energy-changes from adding Runes
   * ISpellbookModifiers can be added and removed from Spellbook
   */
  public class Spellbook : MonoBehaviour {

    private readonly Dictionary<SpellAttribute, int> energies;
    public IReadOnlyDictionary<SpellAttribute, int> Energies { get; }

    private readonly List<Rune> runes; //Still not sure what the purpose of this is
    public IReadOnlyList<Rune> Runes { get; }

    private readonly List<ISpellbookModifier> modifiers;
    //private readonly ISpellUser owner;

    //TODO: Should take in an ISpellUser or something so we can modify TP, look at the caster's state, etc?  Or should that be up to the client?
    public Spellbook() {
      //this.spellUser = spellUser;
      energies = new Dictionary<SpellAttribute, int>();
      Energies = new System.Collections.ObjectModel.ReadOnlyDictionary<SpellAttribute, int>(energies);

      runes = new List<Rune>();
      Runes = runes.AsReadOnly();

      modifiers = new List<ISpellbookModifier>();
      Reset();
    }

    public void AddRune(Rune rune) {
      //Base energy from rune
      foreach (RuneEnergy runeEnergy in rune.Energies) {
        energies[runeEnergy.attr] += runeEnergy.energy;
      }

      //Energy bonuses from any modifiers
      foreach (ISpellbookModifier modifier in modifiers) {
        foreach (RuneEnergy runeEnergy in modifier.RuneEnergyBonuses(rune)) {
          energies[runeEnergy.attr] += runeEnergy.energy;
        }
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