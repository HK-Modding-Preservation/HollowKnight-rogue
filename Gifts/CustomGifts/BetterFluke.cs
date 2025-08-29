
namespace rogue;
internal class BetterFluke : CustomGift
{
    public BetterFluke() : base(Giftname.custom_better_fluke, 1, "witches_eye")
    {
        name = "custom_better_fluke_name";
        desc = "custom_better_fluke_desc";
    }

    protected override void _GetGift()
    {
        On.SpellFluke.DoDamage += StopBurst;
    }

    private void StopBurst(On.SpellFluke.orig_DoDamage orig, SpellFluke self, GameObject obj, int upwardRecursionAmount, bool burst)
    {
        orig(self, obj, upwardRecursionAmount, false);
    }

    protected override void _RemoveGift()
    {
        On.SpellFluke.DoDamage -= StopBurst;
    }
}