//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace SpellSystem {

//  public interface IRune<T> where T : System.Enum {
//    IReadOnlyList<IRuneEnergy<T>> Energies { get; }
//  }

//  public interface IRuneEnergy<T> where T : System.Enum {
//    T Attribute { get; }
//    int Energy { get; }
//  }

//  public interface ISpellbook<T> where T : System.Enum {

//    IReadOnlyDictionary<T, int> Energies { get; }
//    void AddRune(IRune<T> rune);

//    IReadOnlyList<ISpellpage<T>> Spellpages { get; }
//    void SetSpellpage(int pageNumber, ISpellpage<T> spellpage);
//    bool ClearSpellpage(int pageNumber);

//    bool CanActivateSpellpage(int pageNumber);
//    void ActivateSpellpage(int pageNumber);
//    void DectivateSpellpage(int pageNumber);

//    bool CanCast(int pageNumber);
//    void Cast(int pageNumber);
//  }

//  public interface ISpellpage<T> where T : System.Enum {

//    bool CanAddRune(int rank, int file);
//    void AddRune(IRune<T> rune, int rank, int file);
//    bool RemoveRune(int rank, int file);
//    IRune<T> ViewRune(int rank, int file);

//    int ComputeRunePower(int rank, int file);
//    int ComputeSpellPower();
//  }
//}
