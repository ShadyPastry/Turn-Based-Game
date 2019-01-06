using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Refactoring {

  //Usage looks like this
  namespace MainGameExample {

    //Typically a game would only have one magic system
    public enum MainGameSpellAttributes { }

    //This seems like a case where inheritance is okay.  There's a lot of fixed base functionality
    public class MainGameSpellSystemManager : SpellSystemManager<MainGameSpellAttributes> {
      public MainGameSpellSystemManager(List<RuneBuildingInfo<MainGameSpellAttributes>> runes) : base(runes) { }
    }

    //Under this model, Spellbook and SpellPage would also need type parameters
    //I think interfaces are preferable here if we eventually want these objects to inherit from MonoBehaviour
    //  e.g. if we want to associate graphics with them
    //  I would prefer
    //    GameSpellbook : MonoBehaviour, ISpellbook (implementing ISpellbook via a private ISpellbook object)
    //  as opposed to
    //    GameSpellbook : MonoBehaviour (requiring a public SpellSystem.Spellbook object inside of it)
  }

  public interface ISpellbookModifier<T> where T : System.Enum {
    //An ADDITIVE modifier that awards additional RuneEnergy based on rune
    IEnumerable<RuneEnergy<T>> RuneEnergyBonuses(Rune<T> rune);
  }

  public interface ISpellbook<T> where T : System.Enum {
    int Alignment { get; }
    int OrderTier { get; }
    int ChaosTier { get; }

    IReadOnlyList<ISpellpage<T>> Spellpages { get; }

    IReadOnlyDictionary<T, int> Energies { get; }
    IReadOnlyList<ISpellbookModifier<T>> AddRuneModifers { get; }

    void AddRune(Rune<T> rune);
    void Reset();

    void AddSpellpage(ISpellpage<T> spellpage);
    void InsertSpellpage(int pageNumber, ISpellpage<T> spellpage);
    bool RemoveSpellpage(int pageNumber);

    bool CanCast(int pageNumber);
    void Cast(int pageNumber);
    void ActivateSpellpage(int pageNumber);
    void DectivateSpellpage(int pageNumber);
  }

  public interface ISpellpage<T> where T : System.Enum {
    int MinRank { get; }
    int MaxActiveRank { get; }
    int MaxRank { get; }

    int MaxActiveLinks { get; }

    bool CheckPositionValidity(int rank, int file);
    int FilesInRank(int rank);

    void AddRune(Rune<T> rune, int rank, int file);
    bool RemoveRune(int rank, int file);

    void MarkAsLinkSlot(int rank, int file, int priority);
    bool UnmarkAsLinkSlot(int rank, int file);

    Rune<T> ViewRune(int rank, int file);
    int ComputeRunePower(int rank, int file);

    int ComputePower();
    int ComputeBaseActivationCost();
  }

  //Intended usage is as follows
  //  Client defines an enum representing spell attributes
  //  Client defines a fixed set of runes corresponding to those attributes
  //  Client creates an instance of SpellSystemManager to hold that information, probably exposing it via static methods in a singleton
  public class SpellSystemManager<T> where T : System.Enum {

    //Runes are accessible by the index they had in the List<RuneInfo> provided to the constructor
    public IReadOnlyList<Rune<T>> Runes { get; }

    public SpellSystemManager(List<RuneBuildingInfo<T>> runes) {
      List<Rune<T>> runeObjects = new List<Rune<T>>();
      foreach (RuneBuildingInfo<T> info in runes) {
        runeObjects.Add(new Rune<T>(info));
      }
      Runes = runeObjects.AsReadOnly();
    }
  }

  public sealed class Rune<T> where T : System.Enum {
    public IReadOnlyList<RuneEnergy<T>> Energies { get; }

    public int AlignmentChange { get; }

    //Not sure which, if any, of these I want
    //public int IdealRank { get; }
    //public int IdealLinkDistance { get; }
    //public int IdealLinkCount { get; }

    internal Rune(RuneBuildingInfo<T> info) : 
      this(new List<RuneEnergy<T>>(info.energies), info.alignmentChange) {
    }

    internal Rune(List<RuneEnergy<T>> energies, int alignmentChange) {
      Energies = new List<RuneEnergy<T>>(energies).AsReadOnly();
      AlignmentChange = alignmentChange;
    }
  }

  public struct RuneBuildingInfo<T> where T : System.Enum {
    public List<RuneEnergy<T>> energies;
    public int alignmentChange;
  }

  public struct RuneEnergy<T> where T : System.Enum {
    public readonly T attr;
    public readonly int energy;
    public RuneEnergy(T attr, int energy) {
      this.attr = attr;
      this.energy = energy;
    }
  }
}
