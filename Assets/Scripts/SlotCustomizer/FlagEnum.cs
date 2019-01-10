public class FlagEnum<T> where T : System.Enum {
  //Test cases:
  //Debug.Log(SpellSystem.FlagEnum<SpellSystem.RuneSlotStatuses>.Xor(SpellSystem.RuneSlotStatuses.Link, SpellSystem.RuneSlotStatuses.None));

  private static System.Type underlyingType = System.Enum.GetUnderlyingType(typeof(T));

  private FlagEnum() { }

  static FlagEnum() {
    bool isFlagsEnum = System.Attribute.IsDefined(typeof(T), typeof(System.FlagsAttribute));
    if (!isFlagsEnum) {
      throw new System.Exception("Provided enum must have Flags attribute");
    }
  }

  public static bool HasFlag(T enumValue, T flag) {
    ulong _enumValue = UlongFromEnum(enumValue);
    ulong _flag = UlongFromEnum(flag);
    return (_enumValue & _flag) == _flag;
  }

  public static T Or(T flags1, T flags2) {
    return (T)System.Convert.ChangeType(UlongFromEnum(flags1) | UlongFromEnum(flags2), underlyingType);
  }

  public static T And(T flags1, T flags2) {
    return (T)System.Convert.ChangeType(UlongFromEnum(flags1) & UlongFromEnum(flags2), underlyingType);
  }

  public static T Not(T flags) {
    return (T)System.Convert.ChangeType(~UlongFromEnum(flags), underlyingType);
  }

  public static T Xor(T flags1, T flags2) {
    return (T)System.Convert.ChangeType(UlongFromEnum(flags1) ^ UlongFromEnum(flags2), underlyingType);
  }

  private static ulong UlongFromEnum(T enumValue) {
    return System.Convert.ToUInt64(enumValue);
  }
}