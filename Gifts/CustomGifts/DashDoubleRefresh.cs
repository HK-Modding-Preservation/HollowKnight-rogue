

namespace rogue;

internal class DashDoubleRefresh : CustomOneOrTwoGift
{
    internal DashDoubleRefresh() : base(Giftname.custom_one_two_dash_doublejump_refresh, 2, "keeper_key", "easy_key")
    {
        name = "custom_one_two_dash_doublejump_refresh_name";
        desc = "custom_one_two_dash_doublejump_refresh_desc";
        weight = 0.5f;
    }

    internal override string GetDesc1()
    {
        return "custom_one_two_dash_doublejump_refresh_desc_1".Localize();
    }

    internal override string GetDesc2()
    {
        return "custom_one_two_dash_doublejump_refresh_desc_2".Localize();
    }



    internal override string GetName1()
    {
        return "顺";
    }

    internal override string GetName2()
    {
        return "逆";
    }

    internal override void GetGift2()
    {
        Log("获得虎符咒：阴");
        On.HeroController.HeroDash += DashRefreshDoubleJump;
    }

    private void DashRefreshDoubleJump(On.HeroController.orig_HeroDash orig, HeroController self)
    {
        orig(self);
        ReflectionHelper.SetField(HeroController.instance, "doubleJumped", false);
    }

    internal override void RemoveGift2()
    {
        Log("失去虎符咒：阴");
        On.HeroController.HeroDash -= DashRefreshDoubleJump;
    }

    internal override void GetGift1()
    {
        Log("获得虎符咒：阳");
        On.HeroController.DoDoubleJump += DoubleJumpRefreshDash;
    }

    private void DoubleJumpRefreshDash(On.HeroController.orig_DoDoubleJump orig, HeroController self)
    {
        orig(self);
        ReflectionHelper.SetField(HeroController.instance, "airDashed", false);

    }

    internal override void RemoveGift1()
    {
        Log("失去虎符咒：阳");
        On.HeroController.DoDoubleJump -= DoubleJumpRefreshDash;
    }
}