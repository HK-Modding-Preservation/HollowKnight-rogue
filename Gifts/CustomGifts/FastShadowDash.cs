

namespace rogue;

internal class FastShadowDash : CustomGift
{
    internal FastShadowDash() : base(Giftname.custom_fast_shadow_dash, 4, "abyss_tendril")
    {
        weight = 0.5f;
        price = 200;
        name = "custom_fast_shadow_dash_name";
        desc = "custom_fast_shadow_dash_desc";
    }

    protected override void _GetGift()
    {
        On.HeroController.HeroDash += SetShadowDash;
    }



    protected override void _RemoveGift()
    {
        On.HeroController.HeroDash -= SetShadowDash;
    }

    private void SetShadowDash(On.HeroController.orig_HeroDash orig, HeroController self)
    {
        orig(self);
        ReflectionHelper.SetField(self, "shadowDashTimer", -0.2f);
    }
}