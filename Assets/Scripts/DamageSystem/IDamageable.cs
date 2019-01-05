namespace DamageSystem {
  public interface IDamageable {
    int CurrentHp { get; }
    int MaxHp { get; }
    void DamageHp(DamageTypes damageType, int baseDamage);

    int CurrentTp { get; }
    int MaxTp { get; }
    void DamageTp(int baseDamage);

    //TODO: DamageTypes resistances
  }
}