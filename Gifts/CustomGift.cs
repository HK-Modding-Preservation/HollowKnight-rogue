using IL.TMPro;
using rogue;

internal abstract class CustomGift : Gift
{
    internal CustomGift(Giftname giftname, int level, string sprite_name) : base(level)
    {
        this.giftname = giftname;
        got = false;
        name_convo = "rogue_" + giftname.ToString() + "_name";
        desc_convo = "rogue_" + giftname.ToString() + "_desc";
        bool_convo = "rogue_" + giftname.ToString() + "_got";
        ModHooks.LanguageGetHook += GetNameAndDesc;
        ModHooks.GetPlayerBoolHook += GetIsGot;
        sprite = SpriteLoader.GetSprite(sprite_name);
        SFCore.ItemHelper.AddNormalItem(sprite, bool_convo, name_convo, desc_convo);
    }

    protected virtual bool GetIsGot(string name, bool orig)
    {
        if (name == bool_convo) return got;
        return orig;
    }


    protected virtual string GetNameAndDesc(string key, string sheetTitle, string orig)
    {
        if (key == name_convo) { return GetName(); }
        if (key == desc_convo) { return GetDesc(); }
        return orig;
    }

    protected string name_convo;
    protected string desc_convo;
    protected string bool_convo;
    protected bool got;
}