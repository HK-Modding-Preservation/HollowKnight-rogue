

namespace rogue;
internal class EquipCharmEveryWhere : CustomGift
{
    public EquipCharmEveryWhere() : base(Giftname.custom_equip_charm_everywhere, 4, "throne")
    {
        name = "王座";
        desc = "欲践王座，必担其责\n\n随处可换护符，但收到双倍伤害";
        weight = 0.5f;
    }
    protected override void _GetGift()
    {
        CharmHelper.can_equip_everywhere = true;
        ModHooks.TakeDamageHook += OnTakeDamage;
    }

    private int OnTakeDamage(ref int hazardType, int damage)
    {
        return damage * 2;
    }

    protected override void _RemoveGift()
    {
        CharmHelper.can_equip_everywhere = false;
        ModHooks.TakeDamageHook -= OnTakeDamage;
    }
}