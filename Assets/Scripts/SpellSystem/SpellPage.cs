using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public class Spellpage {


    //
    //Future additions brainstorm
    //
    /**
     * Preferred alignment based on the runes in the spell
     * 
     */


    //
    //Constructor
    //

    public Spellpage() {
      runeSlots = new SortedSet<RuneSlot>[Ranks];
      for (int r = 0; r < Ranks; r++) {
        runeSlots[r] = new SortedSet<RuneSlot>();
      }

      links = new SortedSet<LinkInfo>();
    }


    //
    //Power computations
    //


    //Index represents OrderTier
    //If OrderTier is too low, some ranks will be ignored
    private static readonly int[] bestAccessibleRank = new int[] {
      8, 6, 6, 6, 5, 5, 5, 5, 4, 4, 4, 1
    };

    //Index represents ChaosTier
    //If ChaosTier is too low, some links will be ignored
    private static readonly int[] maxLinkSlots = new int[] {
      0, 2, 2, 2, 4, 4, 4, 4, 7, 7, 7, 10
    };

    public int ComputeSpellPower(int alignment) {
      throw new System.NotImplementedException();
    }

    //Computes the power of the Rune at the specified position with the given alignment
    public int ComputeRunePower(int alignment, int rank, int file) {
      int a = 50 - alignment / 10;
      int chaosMultiplier = alignment == SpellAlignment.PureChaos ? 250 : a == SpellAlignment.PureOrder ? 0 : 100 + (a * a * a / 1250);
      int orderMultiplier = alignment == SpellAlignment.PureOrder ? 250 : a == SpellAlignment.PureChaos ? 0 : 200 - chaosMultiplier;

      int baseOrderPower = OrderPower(alignment, rank, file);
      int baseChaosPower = ChaosPower(alignment, rank, file);

      int totalPower = 0;
      totalPower += (int)(baseOrderPower * (orderMultiplier / 100f));
      totalPower += (int)(baseChaosPower * (chaosMultiplier / 100f));
      return totalPower;
    }

    //Returns the power of the Rune at the specified position as determined by its subordinates
    public int OrderPower(int alignment, int rank, int file) {
      //R1 is a subordinate of R2 iff...
      //  R1.rank - R2.rank > 0
      //    Lower LHS means more power
      //  Mathf.Abs(R1.file - R2.file) < 4 - ((OrderTier + 1) / 3)
      //    Lower LHS means more power
      //    Lower RHS also means more power
      throw new System.NotImplementedException();
    }

    //Returns the power of the Rune at the specified position as determined by its links
    public int ChaosPower(int alignment, int rank, int file) {
      //Every rune in a link slot is linked to every other rune in a link slot
      //TODO: Give Rune an "IdealLinkDistance" field; the power of a link depends on that
      throw new System.NotImplementedException();
    }


    //
    //Rune storage
    //

    //TODO: Add a rank 0 with special behavior?

    //Index represents rank
    private static readonly int[] maxActiveFilesInRank = new int[] {
      1, 2, 2, 4, 4, 6, 8, 10 //Sums to 37
    };

    //Rank 1: 1 active file,   requires OrderTier >= 11
    //Rank 2: 2 active files,  requires OrderTier >= 10
    //Rank 3: 2 active files,  requires OrderTier >= 8
    //Rank 4: 4 active files,  requires OrderTier >= 4
    //Rank 5: 4 active files,  requires OrderTier >= 1
    //Rank 6: 6 active files,  requires OrderTier >= 1
    //Rank 7: 8 active files,  requires OrderTier >= 1
    //Rank 8: 10 active files, requires OrderTier >= 0
    private readonly SortedSet<RuneSlot>[] runeSlots;
    private struct RuneSlot : IComparer<RuneSlot> {
      public readonly Rune rune;
      public readonly int file;
      public RuneSlot(Rune rune, int file) {
        this.rune = rune;
        this.file = file;
      }

      public int Compare(RuneSlot x, RuneSlot y) {
        return x.file.CompareTo(y.file);
      }

      public override bool Equals(object obj) {
        if (!(obj is RuneSlot)) {
          return false;
        }
        RuneSlot other = (RuneSlot)obj;
        return other.file == file;
      }

      public override int GetHashCode() {
        return file.GetHashCode();
      }
    }

    //A set representing all rune slots marked as links, sorted by their priority
    private readonly SortedSet<LinkInfo> links;
    private struct LinkInfo : IComparer<LinkInfo> {
      public readonly int rank;
      public readonly int file;
      public readonly int priority;
      public LinkInfo(int rank, int file, int priority = 0) {
        this.rank = rank;
        this.file = file;
        this.priority = priority;
      }

      public int Compare(LinkInfo x, LinkInfo y) {
        return x.priority.CompareTo(y.priority);
      }

      //Depends only on rank and file
      public override bool Equals(object obj) {
        if (!(obj is LinkInfo)) {
          return false;
        }
        LinkInfo other = (LinkInfo)obj;
        return other.rank == rank && other.file == file;
      }

      //Depends only on rank and file
      public override int GetHashCode() {
        int result = 13;
        result = result * 7 + rank.GetHashCode();
        result = result * 7 + file.GetHashCode();
        return result;
      }
    }

    public int Ranks { get; } = maxActiveFilesInRank.Length;
    public int Files { get; } = 10;

    //Adds rune at the given position, replacing any Rune already there
    //Returns false if no more runes can be added to the rank
    public bool AddRune(Rune rune, int rank, int file) {
      RemoveRune(rank, file);
      if (runeSlots[rank].Count == maxActiveFilesInRank[rank]) {
        return false;
      }
      return runeSlots[rank].Add(new RuneSlot(rune, file));
    }

    //Removes the Rune at the given position
    public void RemoveRune(int rank, int file) {
      //Recall that equality/comparison of RuneSlot objects is dependent ONLY on RuneSlot.file, NOT on RuneSlot.rune
      runeSlots[rank].Remove(new RuneSlot(null, file));
    }

    //Returns the Rune at the given position (or null if there isn't one)
    public Rune ViewRune(int rank, int file) {
      foreach (RuneSlot slot in runeSlots[rank]) {
        if (slot.file == file) {
          return slot.rune;
        }
      }

      return null;
    }

    //Marks a position as being a link slot
    public void ActivateLinkSlot(int rank, int file) {
      links.Add(new LinkInfo(rank, file));
    }

    //Marks a position as not being a link slot
    public void DeactivateLinkSlot(int rank, int file) {
      links.Remove(new LinkInfo(rank, file));
    }

    //Sets the link priority of the given position
    public void SetLinkPriority(int rank, int file, int priority) {
      links.Add(new LinkInfo(rank, file, priority));
    }

    //Returns the link priority of the given position
    public int ViewLinkPriority(int rank, int file) {
      foreach (LinkInfo linkInfo in links) {
        if (linkInfo.rank == rank && linkInfo.file == file) {
          return linkInfo.priority;
        }
      }

      throw new System.Exception("Link not found");
    }
  }
}