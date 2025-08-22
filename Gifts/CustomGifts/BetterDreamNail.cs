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
        var fsm = HeroController.instance.gameObject.LocateMyFSM("Dream Nail");
        fsm.GetAction<SendMessage>("Take Control", 2).Enabled = false;
        // fsm.RemoveTransition("Set Charge Start", "CANCEL");
        // fsm.RemoveTransition("Set Charge", "CANCEL");
        // fsm.RemoveTransition("Set Antic", "CANCEL");

    }

    protected override void _RemoveGift()
    {
        var fsm = HeroController.instance.gameObject.LocateMyFSM("Dream Nail");
        fsm.GetAction<SendMessage>("Take Control", 2).Enabled = true;
        // fsm.AddTransition("Set Charge Start", "CANCEL", "Set Recover");
        // fsm.AddTransition("Set Charge", "CANCEL", "Set Recover");
        // fsm.AddTransition("Set Antic", "CANCEL", "Set Recover");
    }
}