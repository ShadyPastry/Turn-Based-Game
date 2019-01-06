using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {

  //TODO: Make alignment-related stuff a part of Spellbook.
  //  Maybe SpellPage can have a preferred alignment based on the runes it contains
  public class SpellPage {
    public static int PureOrder { get; } = 1001;
    public static int PureChaos { get; } = -1;
    private int _alignment; //To be set ONLY by Alignment
    private int alignmentModifiers;

    //Affects OrderTier, ChaosTier, and the weighting of orderPower and chaosPower in SpellPage.Power()
    public int Alignment {
      get { return Mathf.Min(PureChaos, Mathf.Max(PureOrder, _alignment + alignmentModifiers)); }
      private set {
        if (value < 0) {
          _alignment = PureChaos;
        } else if (value > 1000) {
          _alignment = PureOrder;
        } else if (value == 500) {
          _alignment = _alignment < 500 ? 501 : 499;
        } else {
          _alignment = value;
        }
      }
    }

    //Affects maximum number of active ranks
    public int OrderTier { get {
        if (Alignment == PureOrder) {
          return 11;
        } else if (Alignment == PureChaos) {
          return 0;
        } else if (0 <= Alignment && Alignment <= 1000 && Alignment != 500) {
          //Alignment must be in the interval [0, 1000], but it cannot be 500
          //0:99 = 1, 100:199 = 2, ... 400:499 = 5
          //501:600 = 6, 601:700 = 7, ..., 901:1000 = 10
          return 1 + (Alignment < 500 ? Alignment : (Alignment - 1)) / 100;
        } else {
          throw new System.Exception(string.Format("Alignment has an illegal value, {0}", Alignment));
        }
      }
    }
    
    //Affects maximum number of link slots
    public int ChaosTier { get { return 11 - OrderTier; } }

    public int MaxActiveRank { get { return maxActiveRank[OrderTier]; } }
    public int MaxLinkSlots { get { return maxLinkSlots[ChaosTier]; } }

    //TODO: Add a rank 0 with special behavior?

    //TODO: Does this need to be reversed?
    //Mapping from OrderTier to the maximum number of active ranks allowed
    private static readonly int[] maxActiveRank = new int[] { //Index represents OrderTier
      1,
      4, 4, 4,
      5, 5, 5, 5,
      6, 6, 6,
      8
    };

    //TODO: Should these values be reworked?
    //Mapping from ChaosTier to the maximum number of active files allowed
    private static readonly int[] maxLinkSlots = new int[] { //Index represents ChaosTier
      1,
      2, 2, 2,
      4, 4, 4, 4,
      7, 7, 7,
      10
    };

    private Rune[][] runeSlots = new Rune[][] {
      new Rune[1], //Requires OrderTier >= 11
      new Rune[2], //Requires OrderTier >= 10
      new Rune[2], //Requires OrderTier >= 8
      new Rune[4], //Requires OrderTier >= 4
      new Rune[4], //Requires OrderTier >= 1
      new Rune[6], //Requires OrderTier >= 1
      new Rune[8], //Requires OrderTier >= 1
      new Rune[10] //Requires OrderTier >= 0
    };
    //private struct RuneSlotInfo {
    //  public readonly Rune rune;
    //  public readonly int? linkPriority; //A null value indicates that the slot isn't a LinkSlot
    //}

    public SpellPage() {
      Alignment = 501 - 2*Random.Range(0, 1); //Either 499 or 501, with equal probability
    }

    private bool CheckPositionValidity(int rank, int file) {
      return rank >= 0 && rank < runeSlots.Length && file >= 0 && file < runeSlots[rank].Length;
    }

    //Adds rune to the SpellPage at the specified position.  A pre-existing Rune will be overwritten
    public void AddRune(Rune rune, int rank, int file) {
      if (!CheckPositionValidity(rank, file)) {
        throw new System.ArgumentOutOfRangeException("Invalid position");
      }
      RemoveRune(rank, file);
      runeSlots[rank][file] = rune;
      alignmentModifiers += rune.AlignmentChange;
    }

    //Removes the rune at the specified position
    public bool RemoveRune(int rank, int file) {
      if (!CheckPositionValidity(rank, file)) {
        throw new System.ArgumentOutOfRangeException("Invalid position");
      }
      Rune toRemove = runeSlots[rank][file];
      if (toRemove == null) {
        return false;
      }
      alignmentModifiers -= toRemove.AlignmentChange;
      return true;
    }

    public bool ActivateLinkSlot(int rank, int file, int priority) {
      throw new System.NotImplementedException();
    }

    public bool DeactivateLinkSlot(int rank, int file) {
      throw new System.NotImplementedException();
    }

    //Returns the rune at the given position, or null if there isn't one
    public Rune ViewRune(int rank, int file) {
      if (!CheckPositionValidity(rank, file)) {
        throw new System.ArgumentOutOfRangeException("Invalid position");
      }
      return runeSlots[rank][file];
    }

    //Computes the power of the Rune at the specified position
    public int Power(int rank, int file) {
      int a = 50 - Alignment / 10;
      int chaosMultiplier = Alignment == PureChaos ? 250 : a == PureOrder ? 0 : 100 + (a * a * a / 1250);
      int orderMultiplier = Alignment == PureOrder ? 250 : a == PureChaos ? 0 : 200 - chaosMultiplier;

      int baseOrderPower = BaseOrderPower(rank, file);
      int baseChaosPower = BaseChaosPower(rank, file);

      int totalPower = 0;
      totalPower += (int)(baseOrderPower * (orderMultiplier / 100f));
      totalPower += (int)(baseChaosPower * (chaosMultiplier / 100f));
      return totalPower;
    }

    //Returns the power of the Rune at the specified position as determined by its subordinates
    private int BaseOrderPower(int rank, int file) {
      //R1 is a subordinate of R2 iff...
      //  R1.rank - R2.rank > 0
      //    Lower LHS means more power
      //  Mathf.Abs(R1.file - R2.file) < 4 - ((OrderTier + 1) / 3)
      //    Lower LHS means more power
      //    Lower RHS also means more power
      throw new System.NotImplementedException();
    }

    //Returns the power of the Rune at the specified position as determined by its links
    private int BaseChaosPower(int rank, int file) {
      //Every rune in a link slot is linked to every other rune in a link slot
      //TODO: Give Rune an "IdealLinkDistance" field; the power of a link depends on that
      throw new System.NotImplementedException();
    }
  }
}