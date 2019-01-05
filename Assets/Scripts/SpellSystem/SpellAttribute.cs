using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SpellSystem.SpellAttribute;

namespace SpellSystem {
  public enum SpellAttribute {
    Fire, Ice, Volt, //Damage types
    Power, Stability, //Max and min damage
    Efficiency //TP cost
  }

  public static class SpellAttributeExtensions {
    public static bool IsElemental(this SpellAttribute attr) {
      return attr == Fire || attr == Ice || attr == Volt;
    }
  }
}
