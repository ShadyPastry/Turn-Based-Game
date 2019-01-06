using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageSystem {
  [System.Flags]
  public enum DamageTypes {
    Fire = 1 << 0,
    Ice = 1 << 1,
    Volt = 1 << 2,
    Cut = 1 << 3,
    Stab = 1 << 4,
    Bash = 1 << 5
  }
  public static class DamageTypesExtensions {
    public static bool IsElemental(this DamageTypes damageType) {
      return damageType.HasFlag(DamageTypes.Fire | DamageTypes.Ice | DamageTypes.Volt);
    }
    public static bool IsPhysical(this DamageTypes damageType) {
      return damageType.HasFlag(DamageTypes.Cut | DamageTypes.Stab | DamageTypes.Bash);
    }
  }

  public static class Damage {
    private static Vector3 skinWidth = new Vector3(0.1f, 0.1f, 0.1f);

    public delegate bool IsImmune(IDamageable victim);
    public static readonly IsImmune noImmunity = v => false;

    public static void DamageTargets(ICollection<IDamageable> targets, DamageTypes damageTypes, int baseDamage, IsImmune immunityCheck) {
      foreach (IDamageable target in targets) {
        if (target != null && !immunityCheck(target)) {
          target.DamageHp(damageTypes, baseDamage);
        }
      }
    }

    public static void DamageTargets(ICollection<Collider> targets, DamageTypes damageTypes, int baseDamage, IsImmune immunityCheck) {
      foreach (Collider hit in targets) {
        IDamageable target = hit.transform.gameObject.GetComponent<IDamageable>();
        if (target != null && !immunityCheck(target)) {
          target.DamageHp(damageTypes, baseDamage);
        }
      }
    }

    //Damage within a box
    private static void DamageBox(DamageTypes damageTypes, int baseDamage, Vector3 center, Vector3 halfExtents, IsImmune immunityCheck) {
      Collider[] hits = Physics.OverlapBox(center, halfExtents + skinWidth);
      DamageTargets(hits, damageTypes, baseDamage, immunityCheck);
    }

    //Damage whoever is touching tile (includes the sides of tile)
    public static void DamageTouching(DamageTypes damageTypes, int baseDamage, GameTile tile, IsImmune immunityCheck) {
      DamageBox(damageTypes, baseDamage, tile.transform.position, tile.transform.localScale / 2, immunityCheck);
    }

    //On top of tile, envision a box with height verticalReach
    //Damage whoever is within that box
    public static void DamageAbove(DamageTypes damageTypes, int baseDamage, GameTile tile, float verticalReach, IsImmune immunityCheck) {
      Vector3 tileHalfExtents = tile.transform.localScale / 2;

      Vector3 center = tile.transform.position + new Vector3(0, tileHalfExtents.y + verticalReach / 2, 0);
      Vector3 halfExtents = tileHalfExtents;
      halfExtents.y = verticalReach / 2;
      DamageBox(damageTypes, baseDamage, center, halfExtents, immunityCheck);
    }
  }
}