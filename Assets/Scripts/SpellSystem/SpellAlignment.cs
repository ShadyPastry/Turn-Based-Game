using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellAlignment {
  public enum AlignmentType { Order, Chaos }

  public const int PureChaos = -1;
  public const int PureOrder = 1001;

  public static bool IsValidValue(int alignment) {
    return PureChaos <= alignment && alignment <= PureOrder && alignment != 500;
  }

  public static int OrderTier(int alignment) {
    if (!IsValidValue(alignment)) {
      throw new System.ArgumentException("Invalid alignment value");
    }

    if (alignment == PureOrder) {
      return 11;
    } else if (alignment == PureChaos) {
      return 0;
    } else {
      //0:99 = 1, 100:199 = 2, ... 400:499 = 5
      //501:600 = 6, 601:700 = 7, ..., 901:1000 = 10
      return 1 + (alignment < 500 ? alignment : (alignment - 1)) / 100;
    }
  }

  public static int ChaosTier(int alignment) {
    return 11 - OrderTier(alignment);
  }
}
