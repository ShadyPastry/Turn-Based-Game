using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public interface ISpellbookObserver {
    void OnAlignmentChanged(int newAlignment);
    void OnOrderTierChanged(int newOrderTier);
    void OnChaosTierChanged(int newChaosTier);

    //newEnergies is sorted by enum-order and contains entries for SpellAttributes with 0 energy
    void OnEnergiesChanged(IReadOnlyDictionary<SpellAttribute, int> newEnergies);

    void OnMaxStoredRunesChanged(int newMaxStoredRunes);

    void OnRuneStored(Rune added);
    void OnRuneRemoved(Rune removed);
    void OnRuneCast(Rune cast);

    void OnCastRuneModifierAdded(ICastRuneModifier modifier);
    void OnCastRuneModifierRemoved(ICastRuneModifier modifier);

    void OnSpellpageAdded(int pageNumber);
    void OnSpellpageRemoved(int pageNumber);
    void OnSpellCast(int pageNumber);
  }

  //Exists primarily for organizational purposes
  //If I ever wanted to reuse it, I'd probably need to break it into multiple interfaces
  internal interface ISpellbook {
    //Alignment
    int Alignment { get; }
    int OrderTier { get; }
    int ChaosTier { get; }

    //Energy storage
    IReadOnlyDictionary<SpellAttribute, int> Energies { get; }

    //Rune storage
    IReadOnlyList<Rune> StoredRunes { get; }
    int MaxStoredRunes { get; }
    bool SetMaxStoredRunes(int newMaxStoredRunes);

    //Attempt to add or remove a rune
    bool StoreRune(Rune rune);
    bool RemoveRune(Rune rune);

    //Attempt to cast the rune at AvailableRunes[runeIndex]
    bool CastRune(int runeIndex);

    //Add and remove cast rune modifiers
    IReadOnlyList<ICastRuneModifier> CastRuneModifiers { get; }
    bool AddCastRuneModifier(ICastRuneModifier modifier);
    bool RemoveCastRuneModifier(ICastRuneModifier modifier);

    //Looking mighty similar to ISlotCustomizer (minus the status flags for slots)...

    //The highest page number in the spellbook
    int MaxPageNumber { get; }

    //Add, remove, and get spell pages
    bool AddSpellpage(int pageNumber);
    bool RemoveSpellpage(int pageNumber);
    Spellpage GetSpellpage(int pageNumber);

    //Return whether the spell can be cast
    bool CanCastSpell(int pageNumber);

    //Attempt to cast the spell, which will remove energy from Energies
    //Returns true if successful, false if not
    //It is up to the caller to define and invoke any effects
    bool CastSpell(int pageNumber);

    //Activate and deactivate spellpages
    bool ActivateSpellpage(int pageNumber);
    bool DeactivateSpellpage(int pageNumber);
  }

  /**
   * A Spellbook maintains "energy" amounts for each SpellAttribute
   * These amounts are changed by adding Runes via AddRune()
   * 
   * IRuneModifers influence the energy-changes from adding Runes
   * IRuneModifiers can be added and removed from Spellbook
   */
  public class Spellbook : ISpellbook {

    public Spellbook() {
      energies = new SortedDictionary<SpellAttribute, int>(Comparer<SpellAttribute>.Create((x, y) => x.CompareTo(y)));
      Energies = new System.Collections.ObjectModel.ReadOnlyDictionary<SpellAttribute, int>(energies);
      foreach (SpellAttribute energy in energies.Keys) {
        energies[energy] = 0;
      }

      castRuneModifiers = new List<ICastRuneModifier>();
      CastRuneModifiers = castRuneModifiers.AsReadOnly();

      storedRunes = new List<Rune>();
      StoredRunes = storedRunes.AsReadOnly();

      spellpages = new Spellpage[MaxPageNumber + 1]; //+1 is because API is one-indexed; note that we "waste" the zeroth index
    }


    //
    //Alignment (affects Spellbook.CastSpell())
    //

    //Affects OrderTier, ChaosTier, and the weighting of orderPower and chaosPower in SpellPage.Power()
    private int _alignment;
    public int Alignment {
      get {
        return Mathf.Min(SpellAlignment.MaxValue, Mathf.Max(SpellAlignment.MinValue, _alignment));
      }

      private set {
        int oldAlignment = Alignment, oldOrderTier = OrderTier, oldChaosTier = ChaosTier;
        _alignment = value;
        int newAlignment = Alignment, newOrderTier = OrderTier, newChaosTier = ChaosTier;

        if (newAlignment != oldAlignment) {
          observers.ForEach(o => o.OnAlignmentChanged(newAlignment));
          if (newOrderTier != oldOrderTier) {
            observers.ForEach(o => o.OnOrderTierChanged(newOrderTier));
          }
          if (newChaosTier != oldChaosTier) {
            observers.ForEach(o => o.OnChaosTierChanged(newChaosTier));
          }
        }
      }
    }

    //Affects maximum number of active ranks
    public int OrderTier => SpellAlignment.OrderTier(Alignment);

    //Affects maximum number of links
    public int ChaosTier => SpellAlignment.ChaosTier(Alignment);


    //
    //Runes
    //

    private readonly List<Rune> storedRunes;
    public IReadOnlyList<Rune> StoredRunes { get; }
    public int MaxStoredRunes { get; private set; } = 5;

    private readonly IDictionary<SpellAttribute, int> energies;
    public IReadOnlyDictionary<SpellAttribute, int> Energies { get; }

    private readonly List<ICastRuneModifier> castRuneModifiers;
    public IReadOnlyList<ICastRuneModifier> CastRuneModifiers { get; }

    //Attempt to change the maximum number of runes that the Spellbook can store
    //Fails, returning false, if the Spellbook contains more runes than the new value for MaxAvailableRunes
    //Returns true iff MaxStoredRunes changes to a new value
    public bool SetMaxStoredRunes(int newMaxStoredRunes) {
      if (StoredRunes.Count >= newMaxStoredRunes) {
        return false;
      }

      MaxStoredRunes = newMaxStoredRunes;
      observers.ForEach(o => o.OnMaxStoredRunesChanged(MaxStoredRunes));
      return true;
    }

    public bool StoreRune(Rune rune) {
      if (storedRunes.Count == MaxStoredRunes || storedRunes.Contains(rune)) {
        return false;
      }
      storedRunes.Add(rune);
      observers.ForEach(o => o.OnRuneStored(rune));
      return true;
    }

    public bool RemoveRune(Rune rune) {
      bool success = storedRunes.Remove(rune);
      if (success) {
        observers.ForEach(o => o.OnRuneRemoved(rune));
      }

      return success;
    }

    public bool CastRune(int runeIndex) {
      //Relies on the fact that the only instances of Rune are those exposed by the class
      Rune rune = StoredRunes[runeIndex];
      if (!storedRunes.Contains(rune)) {
        return false;
      }

      bool energiesChanged = false;

      //Base energy from rune
      foreach (RuneEnergy runeEnergy in rune.Energies) {
        energiesChanged = energiesChanged || runeEnergy.energy != 0;
        energies[runeEnergy.attr] += runeEnergy.energy;
      }

      //Energy bonuses from any modifiers
      foreach (ICastRuneModifier runeModifier in castRuneModifiers) {
        foreach (RuneEnergy runeEnergy in runeModifier.RuneEnergyBonuses(rune)) {
          energiesChanged = energiesChanged || runeEnergy.energy != 0;
          energies[runeEnergy.attr] += runeEnergy.energy;
        }
      }

      Alignment += rune.AlignmentChange;
      observers.ForEach(o => o.OnRuneCast(rune));
      if (energiesChanged) {
        observers.ForEach(o => o.OnEnergiesChanged(Energies));
      }
      return true;
    }

    public bool AddCastRuneModifier(ICastRuneModifier modifier) {
      observers.ForEach(o => o.OnCastRuneModifierAdded(modifier));
      throw new System.NotImplementedException();
    }

    public bool RemoveCastRuneModifier(ICastRuneModifier modifier) {
      observers.ForEach(o => o.OnCastRuneModifierRemoved(modifier));
      throw new System.NotImplementedException();
    }


    //
    //Spellpages
    //

    public int MaxPageNumber { get; } = 5;

    private Spellpage[] spellpages;

    public bool ActivateSpellpage(int pageNumber) {
      if (!IsLegalPageNumber(pageNumber)) {
        throw new System.Exception("Illegal page number");
      }

      throw new System.NotImplementedException();
    }

    public bool DeactivateSpellpage(int pageNumber) {
      if (!IsLegalPageNumber(pageNumber)) {
        throw new System.Exception("Illegal page number");
      }

      throw new System.NotImplementedException();
    }

    public bool AddSpellpage(int pageNumber) {
      if (!IsLegalPageNumber(pageNumber)) {
        throw new System.Exception("Illegal page number");
      }

      if (spellpages[pageNumber] != null) {
        //Must first remove the existing Spellpage
        return false;
      }

      spellpages[pageNumber] = new Spellpage(this);
      observers.ForEach(o => o.OnSpellpageAdded(pageNumber));
      return true;
    }

    public bool RemoveSpellpage(int pageNumber) {
      if (!IsLegalPageNumber(pageNumber)) {
        throw new System.Exception("Illegal page number");
      }

      if (spellpages[pageNumber] == null) {
        //There's nothing to remove!
        return false;
      }

      spellpages[pageNumber] = null;
      observers.ForEach(o => o.OnSpellpageRemoved(pageNumber));
      return true;
    }

    public Spellpage GetSpellpage(int pageNumber) {
      if (!IsLegalPageNumber(pageNumber)) {
        throw new System.Exception("Illegal page number");
      }

      return spellpages[pageNumber];
    }

    public bool CanCastSpell(int pageNumber) {
      if (!IsLegalPageNumber(pageNumber)) {
        throw new System.Exception("Illegal page number");
      }

      //TODO: Check Energies against energy requirements

      throw new System.NotImplementedException();
    }

    public bool CastSpell(int pageNumber) {
      if (!IsLegalPageNumber(pageNumber)) {
        throw new System.Exception("Illegal page number");
      }

      if (!CanCastSpell(pageNumber)) {
        return false;
      }

      Spellpage spellpage = GetSpellpage(pageNumber);

      //TODO: Adjust Energies based on energy requirements

      observers.ForEach(o => o.OnSpellCast(pageNumber));
      observers.ForEach(o => o.OnEnergiesChanged(Energies));  //Remember, casting a spell consumes energy.
      return true;
    }

    private bool IsLegalPageNumber(int pageNumber) {
      return 1 <= pageNumber && pageNumber <= MaxPageNumber;
    }


    //
    //Observers
    //

    private HashSet<ISpellbookObserver> observers = new HashSet<ISpellbookObserver>();
    public bool AddObserver(ISpellbookObserver observer) {
      if (observer == null) {
        throw new System.ArgumentNullException();
      }
      return observers.Add(observer);
    }
    public bool RemoveObserver(ISpellbookObserver observer) {
      return observers.Remove(observer);
    }

    public void RefreshObserver(ISpellbookObserver observer) {
      observer.OnAlignmentChanged(Alignment);
      observer.OnOrderTierChanged(OrderTier);
      observer.OnChaosTierChanged(ChaosTier);

      observer.OnEnergiesChanged(Energies);

      observer.OnMaxStoredRunesChanged(MaxStoredRunes);

      foreach (Rune rune in StoredRunes) {
        observer.OnRuneStored(rune);
      }

      foreach (ICastRuneModifier modifier in CastRuneModifiers) {
        observer.OnCastRuneModifierAdded(modifier);
      }

      //foreach (Spellpage spellpage in ...) {
      //  OnSpellpageAdded(int pageNumber);
      //}
    }
  }
}