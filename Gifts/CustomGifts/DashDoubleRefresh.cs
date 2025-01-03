

namespace rogue;
internal class DashDoubleRefresh : CustomOneOrTwoGift
{
    internal DashDoubleRefresh() : base(Giftname.custom_one_two_dash_doublejump_refresh, 4, "keeper_key", "easy_key")
    {
        name = "虎符咒";
        desc = "阴阳流转，百世不绝\n当获得1时冲刺刷新二段，获得2时二段刷新冲刺";
    }

    internal override string GetDesc1()
    {
        return "冲刺可刷新二段跳";
    }

    internal override string GetDesc2()
    {
        return "二段跳可刷新冲刺";
    }



    internal override string GetName1()
    {
        return "虎符咒：阳";
    }

    internal override string GetName2()
    {
        return "虎符咒：阴";
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