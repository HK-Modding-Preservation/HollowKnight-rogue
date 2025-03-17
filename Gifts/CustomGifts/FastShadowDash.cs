

namespace rogue;
internal class FastShadowDash : CustomGift
{
    internal FastShadowDash() : base(Giftname.custom_fast_shadow_dash, 4, "abyss_tendril")
    {
        weight = 0.5f;
        price = 200;
        name = "全程黑冲";
        desc = "每次黑冲都是黑冲";
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