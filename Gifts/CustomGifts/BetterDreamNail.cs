namespace rogue;
internal class BetterDreamNail : CustomGift
{
    internal BetterDreamNail() : base(Giftname.custom_better_dream_nail, 4, "dream_tree")
    {
        weight = 0.5f;
        price = 200;
        name = "低语之根";
        desc = "即使在挥舞梦之钉时，你也是自由的";
    }

    protected override void _GetGift()
    {
        HeroController.instance.gameObject.LocateMyFSM("Dream Nail").GetAction<SendMessage>("Take Control", 2).Enabled = false;

    }

    protected override void _RemoveGift()
    {
        HeroController.instance.gameObject.LocateMyFSM("Dream Nail").GetAction<SendMessage>("Take Control", 2).Enabled = true;
    }
}