
namespace rogue;
internal class BetterFluke : CustomGift
{
    public BetterFluke() : base(Giftname.custom_better_fluke, 4, "witches_eye")
    {
        name = "不灭吸虫";
        desc = "击中敌人后吸虫不消失";
        weight = 0.5f;
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