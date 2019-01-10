using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  //TODO: Add/remove and view ISpellbookModifier objects
  //TODO: Add/remove, activate/deactivate, and view SpellPage objects
  /**
   * A Spellbook maintains "energy" amounts for each SpellAttribute
   * These amounts are changed by adding Runes via AddRune()
   * 
   * IRuneModifers influence the energy-changes from adding Runes
   * IRuneModifiers can be added and removed from Spellbook
   */
  public class Spellbook : MonoBehaviour {

    //
    //Alignment (affects Spellbook.CastSpell())
    //

    //Affects OrderTier, ChaosTier, and the weighting of orderPower and chaosPower in SpellPage.Power()
    private int alignment;
    public int Alignment {
      get { return Mathf.Min(SpellAlignment.MaxValue, Mathf.Max(SpellAlignment.MinValue, alignment)); }
    }

    //Affects maximum number of active ranks
    public int OrderTier => SpellAlignment.OrderTier(Alignment);

    //Affects maximum number of links
    public int ChaosTier => SpellAlignment.ChaosTier(Alignment);


    //
    //Constructor
    //

    public Spellbook() {
      energies = new Dictionary<SpellAttribute, int>();
      Energies = new System.Collections.ObjectModel.ReadOnlyDictionary<SpellAttribute, int>(energies);
      foreach (SpellAttribute energy in energies.Keys) {
        energies[energy] = 0;
      }

      runeModifiers = new List<ICastRuneModifier>();
      RuneModifiers = runeModifiers.AsReadOnly();

      availableRunes = new HashSet<Rune>();
      AvailableRunes = availableRunes; //Can still be cast back to a HashSet and modified
    }


    //
    //Runes
    //

    private readonly HashSet<Rune> availableRunes;
    public IReadOnlyCollection<Rune> AvailableRunes { get; }
    public int MaxAvailableRunes { get; } = 5;

    private readonly Dictionary<SpellAttribute, int> energies;
    public IReadOnlyDictionary<SpellAttribute, int> Energies { get; }

    private readonly List<ICastRuneModifier> runeModifiers;
    public IReadOnlyList<ICastRuneModifier> RuneModifiers { get; }

    public bool AddRune(Rune rune) {
      if (availableRunes.Count == MaxAvailableRunes) {
        return false;
      }
      return availableRunes.Add(rune);
    }

    public bool RemoveRune(Rune rune) {
      return availableRunes.Remove(rune);
    }

    public bool CastRune(Rune rune) {
      //Relies on the fact that the only instances of Rune are those exposed by the class
      if (!availableRunes.Contains(rune)) {
        return false;
      }

      //Base energy from rune
      foreach (RuneEnergy runeEnergy in rune.Energies) {
        energies[runeEnergy.attr] += runeEnergy.energy;
      }

      //Energy bonuses from any modifiers
      foreach (ICastRuneModifier runeModifier in runeModifiers) {
        foreach (RuneEnergy runeEnergy in runeModifier.RuneEnergyBonuses(rune)) {
          energies[runeEnergy.attr] += runeEnergy.energy;
        }
      }

      alignment += rune.AlignmentChange;
      return true;
    }

    public void AddCastRuneModifier(ICastRuneModifier modifier) {
      throw new System.NotImplementedException();
    }

    public void RemoveCastRuneModifier(ICastRuneModifier modifier) {
      throw new System.NotImplementedException();
    }


    //
    //Spellpages
    //

    public Spellpage AddSpellpage(int pageNumber) {
      throw new System.NotImplementedException();
    }

    public void RemoveSpellpage(int pageNumber) {
      throw new System.NotImplementedException();
    }

    public Spellpage ViewSpellpage(int pageNumber) {
      throw new System.NotImplementedException();
    }

    public bool CanCastSpell(int pageNumber) {
      throw new System.NotImplementedException();
    }

    public void CastSpell(int pageNumber) {
      if (!CanCastSpell(pageNumber)) {
        throw new System.Exception("Cannot cast spell");
      }

      Spellpage spellpage = ViewSpellpage(pageNumber);
      throw new System.NotImplementedException();
    }
  }
}