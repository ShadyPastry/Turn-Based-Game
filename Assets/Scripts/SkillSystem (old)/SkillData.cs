using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem {
  public class SkillData : MonoBehaviour {

    //
    //Graphics
    //

#pragma warning disable 0649
    [SerializeField] private GameObject _fireballGfx;
    public static GameObject FireballGfx { get { return S._fireballGfx; } }

    [SerializeField] private GameObject _sparkGfx;
    public static GameObject SparkGfx { get { return S._sparkGfx; } }
#pragma warning restore 0649


    //
    //Other
    //

    private static SkillData S { get; set; }

    private void Awake() {
      if (S != null) {
        Destroy(gameObject);
        return;
      }
      S = this;
    }
  }
}