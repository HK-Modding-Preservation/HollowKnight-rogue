

namespace rogue;
internal class AnotherJump : CustomGift
{
    public AnotherJump() : base(Giftname.custom_another_jump, 4, "attack_projectile")
    {
        name = "三段跳";
        desc = "可以使用三段跳";
        weight = 0.5f;
        price = 200;
    }
    bool can_another_jump = true;

    protected override void _GetGift()
    {
        On.HeroController.DoDoubleJump += KnightAnotherJump;
        On.HeroController.BackOnGround += Land;
    }

    private void Land(On.HeroController.orig_BackOnGround orig, HeroController self)
    {
        orig(self);
        can_another_jump = true;
    }

    private void KnightAnotherJump(On.HeroController.orig_DoDoubleJump orig, HeroController self)
    {
        orig(self);
        if (can_another_jump)
        {
            can_another_jump = false;
            ReflectionHelper.SetField(HeroController.instance, "doubleJumped", false);
        }
    }

    protected override void _RemoveGift()
    {
        On.HeroController.DoDoubleJump -= KnightAnotherJump;
        On.HeroController.BackOnGround -= Land;
        can_another_jump = true;
    }
}