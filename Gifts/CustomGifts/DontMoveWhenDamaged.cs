namespace rogue;
internal class DontMoveWhenDamaged : CustomGift
{
    internal DontMoveWhenDamaged() : base(Giftname.custom_dont_move_when_damaged, 4, "witches_eye")
    {
        price = 200;
        weight = 0.5f;
        name = "霸体";
        desc = "受伤时不会再产生强制移动";
    }
    float ori_vel;
    protected override void _GetGift()
    {
        ori_vel = HeroController.instance.RECOIL_VELOCITY;
        HeroController.instance.RECOIL_VELOCITY = 0;
    }

    protected override void _RemoveGift()
    {
        HeroController.instance.RECOIL_VELOCITY = ori_vel;
    }
}