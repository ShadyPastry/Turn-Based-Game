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
    //Alignment
    //

    private int _alignment; //To be set ONLY by Alignment
    private int alignmentModifiers;

    //Affects OrderTier, ChaosTier, and the weighting of orderPower and chaosPower in SpellPage.Power()
    public int Alignment {
      get { return Mathf.Min(SpellAlignment.PureChaos, Mathf.Max(SpellAlignment.PureOrder, _alignment + alignmentModifiers)); }
    }

    //Affects maximum number of active ranks
    public int OrderTier => SpellAlignment.OrderTier(Alignment);

    //Affects maximum number of link slots
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

      runeModifiers = new List<IRuneModifier>();
      RuneModifiers = runeModifiers.AsReadOnly();

      _alignment = 501 - 2 * Random.Range(0, 1); //Either 499 or 501, with equal probability
    }


    //
    //Runes
    //

    private readonly Dictionary<SpellAttribute, int> energies;
    public IReadOnlyDictionary<SpellAttribute, int> Energies { get; }

    private readonly List<IRuneModifier> runeModifiers;
    public IReadOnlyList<IRuneModifier> RuneModifiers { get; }

    public void AddRune(Rune rune) {
      //Base energy from rune
      foreach (RuneEnergy runeEnergy in rune.Energies) {
        energies[runeEnergy.attr] += runeEnergy.energy;
      }

      //Energy bonuses from any modifiers
      foreach (IRuneModifier runeModifier in runeModifiers) {
        foreach (RuneEnergy runeEnergy in runeModifier.RuneEnergyBonuses(rune)) {
          energies[runeEnergy.attr] += runeEnergy.energy;
        }
      }

      alignmentModifiers += rune.AlignmentChange;
    }

    public void AddRuneModifier(IRuneModifier modifier) {
      throw new System.NotImplementedException();
    }

    public void RemoveRuneModifier(IRuneModifier modifier) {
      throw new System.NotImplementedException();
    }


    //
    //Spellpages
    //

    public void AddSpellpage(int pageNumber, Spellpage spellpage) {
      throw new System.NotImplementedException();
    }

    public void RemoveSpellpage(int pageNumber) {
      throw new System.NotImplementedException();
    }

    public Spellpage ViewSpellpage(int pageNumber) {
      throw new System.NotImplementedException();
    }

    public void CastSpell(int pageNumber) {
      Spellpage spellpage = ViewSpellpage(pageNumber);
      int spellPower = spellpage.ComputeSpellPower(Alignment);
      throw new System.NotImplementedException();
    }
  }
}