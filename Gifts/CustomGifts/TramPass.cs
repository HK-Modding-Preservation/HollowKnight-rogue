
namespace rogue;

internal class TramPass : CustomGift
{
    //电车通行证，用于打开表哥通路
    //具体实现方式，将GameInfo.Branch.lost_kin设置为true
    //在GameInfo.Branch.lost_kin为true时，四门上将替换为感染小怪房，失落近亲会变为modboss形式
    internal TramPass() : base(Giftname.custom_tram_pass, 4, "tram_pass")
    {
        giftname = Giftname.custom_tram_pass;
        price = 200;
        desc = "电车通行证，似乎可以打开某种通路";
        force_active = false;
    }

    protected override void _GetGift()
    {
        GameInfo.Branch.lost_kin = true;
    }

    protected override void _RemoveGift()
    {
        GameInfo.Branch.lost_kin = false;
    }
    internal override string GetName()
    {
        return Language.Language.Get("INV_NAME_TRAM_PASS", "UI");
    }

}