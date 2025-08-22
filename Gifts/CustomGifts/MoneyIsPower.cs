
namespace rogue;
internal class MoneyIsPower : CustomGift
{
    public MoneyIsPower() : base(Giftname.custom_money_is_power, 4, "witches_eye")
    {
        weight = 0.5f;
        price = 200;
        name = "custom_money_is_power_name";
        desc = "custom_money_is_power_desc";
    }

    protected override void _GetGift()
    {
        On.HealthManager.TakeDamage += AddDamage;
    }

    private void AddDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        int geo = PlayerData.instance.geo;
        int more_damage = geo / 140;
        if (hitInstance.DamageDealt > 0) hitInstance.DamageDealt += more_damage;
        orig(self, hitInstance);
    }

    protected override void _RemoveGift()
    {
        On.HealthManager.TakeDamage -= AddDamage;
    }
}