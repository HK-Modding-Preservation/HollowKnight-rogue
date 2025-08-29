namespace rogue;

internal class CityCrest : CustomGift
{
    //城市纹章，用于打开modboss线
    //具体实现方式，将GameInfo.Branch.modboss设置为true
    //在GameInfo.Branch.modboss为true时，四门后半将变为modboss形式
    internal CityCrest() : base(Giftname.custom_city_crest, 4, "city_crest")
    {
        giftname = Giftname.custom_city_crest;
        price = 200;
        desc = "city_crest_desc";
        force_active = false;
    }

    protected override void _GetGift()
    {
        GameInfo.Branch.modboss = true;
    }

    protected override void _RemoveGift()
    {
        GameInfo.Branch.modboss = false;
    }

    internal override string GetName()
    {
        return Language.Language.Get("INV_NAME_CITYKEY", "UI");
    }
    internal override string GetDesc()
    {
        return base.GetDesc();
    }
}