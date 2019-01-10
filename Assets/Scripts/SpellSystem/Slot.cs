using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  public class Slot<Element, Statuses> where Statuses : System.Enum {
    public readonly Element element;
    public readonly Statuses status;
    public readonly int rank, file;
    public Slot(Element element, Statuses status, int rank, int file) {
      this.element = element;
      this.status = status;
      this.rank = rank;
      this.file = file;
    }
  }
}
