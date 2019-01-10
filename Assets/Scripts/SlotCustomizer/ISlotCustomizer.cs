using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public interface ISlotCustomizer<TSlot, TSlotElement, TSlotStatuses>
    where TSlot : Slot<TSlotElement, TSlotStatuses>
    where TSlotStatuses : System.Enum {
    //The number of ranks and files in the slot customizer
    int Ranks { get; }
    int Files { get; }

    //How many slots SlotElements are in the slot customizer
    int Count { get; }

    //Return whether the given position exists within the slot customizer
    bool PositionIsLegal(int rank, int file);

    //Return how many files are available in the given rank
    int AvailableFilesInRank(int rank);

    //Returns the Slot at the given position (or default if there isn't one)
    TSlot Get(int rank, int file);

    //Adds item at the given position, REPLACING any SlotElement already there
    //Returns false iff item couldn't be added
    bool Add(TSlotElement element, int rank, int file);

    //Removes the SlotElement at the given position, CLEARING any associated SlotStatuses
    //Returns true iff a SlotElement was removed
    bool Remove(int rank, int file);

    //Adds the specified flags to the given position
    void AddStatusFlags(TSlotStatuses flagsToAdd, int rank, int file);

    //Removes the specified flags from the given position
    void RemoveStatusFlags(TSlotStatuses flagsToRemove, int rank, int file);
  }
}
