
namespace rogue;
internal class LoveKey : CustomGift
{
    internal LoveKey() : base(Giftname.custom_love_key, 4, "love_key")
    {
        giftname = Giftname.custom_love_key;
        price = 200;
        desc = "钥匙，似乎可以打开某种通路";
    }

    protected override void _GetGift()
    {
    }

    protected override void _RemoveGift()
    {
    }
    internal override string GetName()
    {
        return Language.Language.Get("INV_NAME_LOVEKEY", "UI");
    }

}