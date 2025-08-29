

namespace rogue;

internal class EquipCharmEveryWhere : CustomGift
{
    public EquipCharmEveryWhere() : base(Giftname.custom_equip_charm_everywhere, 1, "witches_eye")
    {
        name = "custom_equip_charm_everywhere_name";
        desc = "custom_equip_charm_everywhere_desc";
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