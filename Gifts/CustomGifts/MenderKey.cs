namespace rogue;

internal class MenderKey : CustomGift
{
    //修补者钥匙，用于打开辐光modboss
    //具体实现方式，将GameInfo.Branch.mender设置为true
    //在GameInfo.Branch.mender为true时，四门上将替换为修补者小怪房，修补者会变为modboss形式
    internal MenderKey() : base(Giftname.custom_mender_key, 4, "mender_key")
    {
        giftname = Giftname.custom_mender_key;
        price = 200;
        desc = "钥匙，似乎可以打开某种通路";
        force_active = false;
    }

    protected override void _GetGift()
    {
        GameInfo.Branch.radiance = true;
    }

    protected override void _RemoveGift()
    {
        GameInfo.Branch.radiance = false;
    }

    internal override string GetName()
    {
        return "Mender_Key".Localize();
    }
}