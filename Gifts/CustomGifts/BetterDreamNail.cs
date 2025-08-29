namespace rogue;

internal class BetterDreamNail : CustomGift
{
    internal BetterDreamNail() : base(Giftname.custom_better_dream_nail, 3, "dream_tree")
    {
        price = 200;
        name = "custom_better_dream_nail_name";
        desc = "custom_better_dream_nail_desc";
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