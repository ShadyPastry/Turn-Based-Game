using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  //Ideally, there would be a base version using generics, and RuneSlotCustomizer would use it via composition
  //Sadly, there is no way to have a flag-enum type constraint
  internal class RuneSlotCustomizer : ISlotCustomizer<RuneSlot, Rune, RuneSlotStatuses> {

    private readonly IReadOnlyList<int> availableFilesInRank;
    private readonly SortedList<int, Dictionary<int, RuneSlot>> elements;

    public delegate bool SlotFilter(Slot<Rune, RuneSlotStatuses> slot);
    public IEnumerable<RuneSlot> GetSlots(SlotFilter filterBy) {
      for (int rank = 1; Ranks < Ranks; rank++) {
        foreach (var slot in elements[rank].Values) {
          if (filterBy(slot)) {
            yield return slot;
          }
        }
      }
    }

    //
    //ISlotCustomizer API
    //

    public int Ranks { get; }
    public int Files { get; }
    public int Count { get; private set; }

    //SlotCustomizer will have availableFilesInRank.Length ranks
    //  Ranks are ONE-INDEXED
    //Rank k (ONE-INDEXED) can contain up to availableFilesInRank[k-1] SlotElements, each associated with a file
    //  Files are ONE-INDEXED
    public RuneSlotCustomizer(int[] availableFilesInRank, int maxFiles) {
      Ranks = availableFilesInRank.Length;
      Files = maxFiles;
      this.availableFilesInRank = new List<int>(availableFilesInRank).AsReadOnly(); //Defensive copy

      elements = new SortedList<int, Dictionary<int, RuneSlot>>(Ranks);
      for (int rank = 1; rank < Ranks; rank++) {
        elements.Add(rank, new Dictionary<int, RuneSlot>());
      }
    }

    public RuneSlot Get(int rank, int file) {
      if (!PositionIsLegal(rank, file)) {
        throw new System.ArgumentException("Illegal rank/file");
      }

      return elements[rank].TryGetValue(file, out RuneSlot result) ? result : null;
    }

    public bool Add(Rune item, int rank, int file) {
      if (!PositionIsLegal(rank, file)) {
        throw new System.ArgumentException("Illegal rank/file");
      }
      RuneSlotStatuses slotStatus;
      if (elements[rank].TryGetValue(file, out RuneSlot oldSlot)) {
        slotStatus = oldSlot.status;
        Remove(rank, file);
      } else {
        slotStatus = default;
      }

      //Couldn't add the item because there's no more space
      if (elements[rank].Count == AvailableFilesInRank(rank)) {
        return false;
      }

      elements[rank].Add(file, new RuneSlot(item, slotStatus, rank, file));
      Count += 1;
      return true;
    }

    public bool Remove(int rank, int file) {
      if (!PositionIsLegal(rank, file)) {
        throw new System.ArgumentException("Illegal rank/file");
      }

      if (elements[rank].Remove(file)) {
        Count -= 1;
        return true;
      }
      return false;
    }

    public void AddStatusFlags(RuneSlotStatuses flagsToSet, int rank, int file) {
      if (!PositionIsLegal(rank, file)) {
        throw new System.ArgumentException("Illegal rank/file");
      }

      if (elements[rank].TryGetValue(file, out RuneSlot oldSlot)) {
        elements[rank][file] = new RuneSlot(oldSlot.element, oldSlot.status | flagsToSet, rank, file);
      }
    }

    public void RemoveStatusFlags(RuneSlotStatuses flagsToRemove, int rank, int file) {
      if (!PositionIsLegal(rank, file)) {
        throw new System.ArgumentException("Illegal rank/file");
      }

      if (elements[rank].TryGetValue(file, out RuneSlot oldSlot)) {
        elements[rank][file] = new RuneSlot(oldSlot.element, oldSlot.status & ~flagsToRemove, rank, file);
      }
    }

    public RuneSlotStatuses SlotStatus(int rank, int file) {
      if (!PositionIsLegal(rank, file)) {
        throw new System.ArgumentException("Illegal rank/file");
      }

      return elements[rank].TryGetValue(file, out RuneSlot result) ? result.status : default;
    }

    public int AvailableFilesInRank(int rank) {
      if (rank < 1 || rank > Ranks) {
        throw new System.ArgumentException("Illegal rank");
      }
      return availableFilesInRank[rank];
    }

    public bool PositionIsLegal(int rank, int file) {
      if (rank < 1 || rank > Ranks || file < 1 || file > AvailableFilesInRank(rank)) {
        return false;
      }
      return true;
    }
  }
}
