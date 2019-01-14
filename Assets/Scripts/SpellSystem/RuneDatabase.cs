using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellSystem {
  //This is basically a dictionary, not an actual database, but I wasn't sure what else to call it
  public class RuneDatabase<T> where T : IRune {
    private readonly Dictionary<uint, RuneManagerEntry> runes = new Dictionary<uint, RuneManagerEntry>();

    //Associate rune with runeId.  Throws an exception if runeId is already taken
    public void Add(T rune, uint runeId) {
      runes.Add(runeId, new RuneManagerEntry(rune, runeId));
    }

    //Return the rune associated with the id
    public T Get(uint id) {
      return runes[id].rune;
    }

    //Returns whether id is already used
    public bool ContainsId(uint id) {
      return runes.ContainsKey(id);
    }

    //An (Id : uint, rune : Rune) pair
    //Implements Equals and GetHashCode, which both look ONLY at Id
    private struct RuneManagerEntry {
      public uint Id { get; }
      public readonly T rune;

      public RuneManagerEntry(T rune, uint runeId) {
        this.rune = rune;
        Id = runeId;
      }

      //Equality is determined exclusively by Id
      public override bool Equals(object obj) {
        if (!(obj is RuneManagerEntry)) {
          return false;
        }
        RuneManagerEntry other = (RuneManagerEntry)obj;
        return other.Id == Id;
      }

      //Hash code is determined exclusively by Id
      public override int GetHashCode() {
        return Id.GetHashCode();
      }
    }
  }
}