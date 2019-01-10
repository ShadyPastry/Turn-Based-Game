using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public class Spellpage : ISlotCustomizer<RuneSlot, Rune, RuneSlotStatuses> {
    /** 
     * FUTURE ADDITIONS BRAINSTORM
     * Preferred alignment based on the runes in the spell
     */

    private static readonly int[] availableFilesInRank = new int[] {
      1, 2, 2, 4,
      4, 6, 8, 10 //Sums to 37
    };

    private readonly Spellbook spellbook;

    //ISlotCustomizer API is implemented via this
    private readonly RuneSlotCustomizer runeSlotCustomizer = 
      new RuneSlotCustomizer(availableFilesInRank, availableFilesInRank[availableFilesInRank.Length - 1]);

    public Spellpage(Spellbook spellbook) {
      this.spellbook = spellbook;
    }


    //
    //Power computations
    //

    //Computes the power of the Rune at the specified position with the given alignment
    public int ComputeRunePower(int rank, int file) {
      int orderTier = spellbook.OrderTier;

      int chaosMultiplier, orderMultiplier;
      if (orderTier == SpellAlignment.MaxTier) {
        chaosMultiplier = 0;
        orderMultiplier = 250;

      } else if (orderTier == SpellAlignment.MinTier) {
        chaosMultiplier = 250;
        orderMultiplier = 0;

      } else {
        int x;
        //Falls within interval [0, 499] U [501, 1000]
        x = spellbook.Alignment < 500 ? spellbook.Alignment : spellbook.Alignment + 1;

        //Falls within interval [-50, -1] U [1, 50]
        x = 50 - spellbook.Alignment / 10;

        //Equivalent to chaosMultiplier = 100 + 100 * (1 - (spellbook.Alignment/10) / 50)^3
        chaosMultiplier = 100 + (x * x * x / 1250);
        orderMultiplier = 200 - chaosMultiplier;
      }

      int totalPower = 0;
      totalPower += (int)(OrderPower(rank, file) * (orderMultiplier / 100f));
      totalPower += (int)(ChaosPower(rank, file) * (chaosMultiplier / 100f));
      return totalPower;
    }

    //Returns the power of the Rune at the specified position as determined by its subordinates
    public int OrderPower(int rank, int file) {
      Slot<Rune, RuneSlotStatuses> slot = Get(rank, file);
      if (slot == null) {
        return 0;
      }
      int result = 0;

      //Iterate over all other runes
      for (int r = 1; r <= BestAccessibleRank(spellbook.OrderTier); r++) {
        for (int f = 1; f <= AvailableFilesInRank(r); f++) {
          Slot<Rune, RuneSlotStatuses> otherSlot = Get(r, f);
          if (r == f || otherSlot == null) {
            continue;
          }

          int rankDifference = -(rank - r); //Negated because the "superior" ranks are LOWER, not higher
          int fileDifference = Mathf.Abs(file - f);

          //other is a subordinate of rune iff rune has a superior rank
          //If other is NOT a subordinate of rune, then it contributes no power
          if (rankDifference <= 0) {
            continue;
          }

          int distance = rankDifference + fileDifference;
          int maxDistance = 6;

          //More power is awarded when subordinates are closer to rune
          int distPoints = 1 + Mathf.Max(1, maxDistance - distance); //Falls within interval [2, 6]
          result += distPoints * distPoints; //Adds either 4, 9, 16, 25, or 36
        }
      }
      return result;
    }

    //Returns the power of the Rune at the specified position as determined by its links
    public int ChaosPower(int rank, int file) {
      int result = 0;
      List<Slot<Rune, RuneSlotStatuses>> links = SelectRandomLinks(MaxLinks(spellbook.ChaosTier));

      //Iterate over linked runes
      foreach (var slot in links) {
        Slot<Rune, RuneSlotStatuses> otherSlot = Get(slot.rank, slot.file);
        if ((slot.rank == rank && slot.file == file) || otherSlot == null) {
          continue;
        }
      }
      return result;
    }


    //
    //Rune storage
    //

    //TODO: Add a rank 0 with special behavior?

    //Index represents ChaosTier
    private readonly List<int> maxLinks = new List<int> {
      0, 2, 2, 2,
      4, 4, 4, 4,
      7, 7, 7, 10
    };

    //Index represents OrderTier
    private readonly List<int> bestAccessibleRank = new List<int> {
      8, 6, 6, 6,
      5, 5, 5, 5,
      4, 4, 4, 1
    };

    //As ChaosTier increases, more links can contribute to power
    //Among the RuneSlots marked as links, MaxLinks(chaosTier) will be randomly selected
    //  RuneSlots in inaccessible ranks will still be inaccessible
    public int MaxLinks(int chaosTier) {
      return maxLinks[chaosTier];
    }

    //As OrderTier increases, better ranks can contribute to power
    //If OrderTier is too low, said ranks will be ignored when computing power
    public int BestAccessibleRank(int orderTier) {
      return bestAccessibleRank[orderTier];
    }

    //Returns true iff the slot at the given position will contribute to power
    public bool SlotIsActive(int rank, int file) {
      if (!PositionIsLegal(rank, file)) {
        throw new System.ArgumentException("Illegal rank/file");
      }

      return rank <= BestAccessibleRank(spellbook.OrderTier);
    }

    //Returns true iff the slot at the given position is a link slot
    public bool SlotIsLinkSlot(int rank, int file) {
      return (SlotStatus(rank, file) & RuneSlotStatuses.Link) == RuneSlotStatuses.Link;
    }

    //Selects a random, size k subset of link slots
    //  If k >= the number of link slots, returns all link slots
    //Randomness of the order is NOT guaranteed
    private List<Slot<Rune, RuneSlotStatuses>> SelectRandomLinks(int k) {
      var result = new List<Slot<Rune, RuneSlotStatuses>>();
      var linkSlots =
        new List<Slot<Rune, RuneSlotStatuses>>(runeSlotCustomizer.GetSlots(slot => (slot.status & RuneSlotStatuses.Link) == RuneSlotStatuses.Link));
      int n = linkSlots.Count;

      //If asking for more than available, just return everything
      //Otherwise, add k random slots to result
      if (k >= n) {
        return linkSlots;

      } else if (k > 0) {
        foreach (var slot in linkSlots) {
          int x = Random.Range(1, n + 1); //Random number from interval [1, n]
          if (x <= k) { //Occurs with probability k/n
            result.Add(slot);
            k -= 1;
            if (result.Count == k) break;
          }
          n -= 1;
        }
      }

      return result;
    }


    //
    //ISlotCustomizer API (implemented via runeSlotCustomizer)
    //

    public int Ranks => runeSlotCustomizer.Ranks;
    public int Files => runeSlotCustomizer.Files;
    public int Count => runeSlotCustomizer.Count;

    public bool PositionIsLegal(int rank, int file) {
      return runeSlotCustomizer.PositionIsLegal(rank, file);
    }

    public int AvailableFilesInRank(int rank) {
      return runeSlotCustomizer.AvailableFilesInRank(rank);
    }

    public RuneSlot Get(int rank, int file) {
      return runeSlotCustomizer.Get(rank, file);
    }

    public bool Add(Rune item, int rank, int file) {
      return runeSlotCustomizer.Add(item, rank, file);
    }

    public bool Remove(int rank, int file) {
      return runeSlotCustomizer.Remove(rank, file);
    }

    public RuneSlotStatuses SlotStatus(int rank, int file) {
      return runeSlotCustomizer.SlotStatus(rank, file);
    }

    public void AddStatusFlags(RuneSlotStatuses flagsToAdd, int rank, int file) {
      runeSlotCustomizer.AddStatusFlags(flagsToAdd, rank, file);
    }

    public void RemoveStatusFlags(RuneSlotStatuses flagsToRemove, int rank, int file) {
      runeSlotCustomizer.RemoveStatusFlags(flagsToRemove, rank, file);
    }
  }
}