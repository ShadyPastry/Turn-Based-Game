using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public class RuneSlot : Slot<Rune, RuneSlotStatuses> {
    public RuneSlot(Rune rune, RuneSlotStatuses slotStatuses, int rank, int file)
      : base(rune, slotStatuses, rank, file) {
    }
  }
}
