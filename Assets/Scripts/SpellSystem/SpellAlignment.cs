namespace SpellSystem {
  public static class SpellAlignment {
    public const int MinValue = 0;
    public const int MaxValue = 999;
    public const int MinTier = 0;
    public const int MaxTier = 11;

    public static int OrderTier(int alignment) {
      if (alignment < MinValue) {
        return 0;
      } else if (alignment > MaxValue) {
        return 11;
      } else {
        //0:99 = 1, 100:199 = 2, ... 900:999 = 10
        return 1 + alignment / 100;
      }
    }

    public static int ChaosTier(int alignment) {
      return 11 - OrderTier(alignment);
    }
  }
}