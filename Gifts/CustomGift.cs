using System.Diagnostics.PerformanceData;
using System.Threading;
using IL.TMPro;
using rogue;

internal abstract class CustomGift : Gift
{
    internal CustomGift(Giftname giftname, int level, string sprite_name) : base(level)
    {
        this.giftname = giftname;
        Got = false;
        name_convo = "rogue_" + giftname.ToString() + "_name";
        desc_convo = "rogue_" + giftname.ToString() + "_desc";
        bool_convo = "rogue_" + giftname.ToString() + "_got";
        ModHooks.LanguageGetHook += GetNameAndDesc;
        ModHooks.GetPlayerBoolHook += GetIsGot;
        getSprite = () => SpriteLoader.GetSprite(sprite_name);
        SFCore.ItemHelper.AddNormalItem(getSprite(), bool_convo, name_convo, desc_convo);
        this.weight = 1.1f - (0.1f * level);
    }

    protected virtual bool GetIsGot(string name, bool orig)
    {
        if (name == bool_convo) return Got;
        return orig;
    }
    protected abstract void _GetGift();
    protected abstract void _RemoveGift();

    internal override void GetGift()
    {
        Got = true;

    }
    internal override void RemoveGift()
    {
        Got = false;
    }

    protected virtual string GetNameAndDesc(string key, string sheetTitle, string orig)
    {
        if (key == name_convo) { return GetName(); }
        if (key == desc_convo) { return GetDesc(); }
        return orig;
    }

    private string name_convo;
    private string desc_convo;
    private string bool_convo;
    bool got = false;
    internal bool Got
    {
        get { return got; }
        set
        {
            if (got == value) return;
            if (got)
            {
                now_weight = weight;
                _RemoveGift();
            }
            else
            {
                now_weight = 0;
                _GetGift();
            }
            got = value;
        }
    }
}
internal abstract class CustomOneOrTwoGift : Gift
{
    internal CustomOneOrTwoGift(Giftname giftname, int level, string sprite_name1, string sprite_name2) : base(level)
    {
        this.giftname = giftname;
        Got_Which_Item = GotWhichItem.None;
        name_convo_1 = "rogue_" + giftname.ToString() + "_name_1";
        desc_convo_1 = "rogue_" + giftname.ToString() + "_desc_1";
        bool_convo_1 = "rogue_" + giftname.ToString() + "_got_1";
        name_convo_2 = "rogue_" + giftname.ToString() + "_name_2";
        desc_convo_2 = "rogue_" + giftname.ToString() + "_desc_2";
        bool_convo_2 = "rogue_" + giftname.ToString() + "_got_2";
        ModHooks.LanguageGetHook += GetNameAndDesc;
        ModHooks.GetPlayerBoolHook += GetIsGot;
        SFCore.ItemHelper.AddOneTwoItem(
         SpriteLoader.GetSprite(sprite_name1), SpriteLoader.GetSprite(sprite_name2),
         bool_convo_1, bool_convo_2,
         name_convo_1, name_convo_2,
         desc_convo_1, desc_convo_2);
    }
    internal override Sprite GetSprite()
    {
        return base.GetSprite();
    }
    protected virtual bool GetIsGot(string name, bool orig)
    {
        if (name == bool_convo_1) return Got_Which_Item == GotWhichItem.First;
        else if (name == bool_convo_2) return Got_Which_Item == GotWhichItem.Second;
        return orig;
    }
    internal abstract string GetName1();
    internal abstract string GetDesc1();
    internal abstract string GetName2();
    internal abstract string GetDesc2();
    internal abstract void GetGift1();
    internal abstract void GetGift2();
    internal abstract void RemoveGift1();
    internal abstract void RemoveGift2();

    protected virtual string GetNameAndDesc(string key, string sheetTitle, string orig)
    {
        if (key == name_convo_1) { return GetName() + ":" + GetName1(); }
        if (key == desc_convo_1) { return GetDesc1(); }
        if (key == name_convo_2) { return GetName() + ":" + GetName2(); }
        if (key == desc_convo_2) { return GetDesc2(); }
        return orig;
    }

    private string name_convo_1;
    private string desc_convo_1;
    private string bool_convo_1;
    private string name_convo_2;
    private string desc_convo_2;
    private string bool_convo_2;
    internal enum GotWhichItem
    {
        None,
        First,
        Second
    }
    private GotWhichItem got_which_item = CustomOneOrTwoGift.GotWhichItem.None;
    internal override void GetGift()
    {
        // RogueUIManager.SelectItem item1 = new(GetName1())
        // {
        //     select_action = (index) =>
        //     {
        //         Got_Which_Item = GotWhichItem.First;
        //     }
        // };
        // RogueUIManager.SelectItem item2 = new(GetName2())
        // {
        //     select_action = (index) =>
        //     {
        //         Got_Which_Item = GotWhichItem.Second;
        //     }
        // };
        // RogueUIManager.SelectItem cancel = new(Lang.Cancel)
        // {
        // };
        // RogueUIManager.StartSelection(0.3f, "custom_two_choice_gift_conv".Localize(), new List<RogueUIManager.SelectItem>
        // {
        //     item1,item2,cancel
        // }, 3);
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            Got_Which_Item = GotWhichItem.First;
            Rogue.Instance.ShowConvo(GetName1());
        }
        else
        {
            Got_Which_Item = GotWhichItem.Second;
            Rogue.Instance.ShowConvo(GetName2());
        }
    }
    internal override void RemoveGift()
    {
        Got_Which_Item = GotWhichItem.None;
        base.RemoveGift();
    }
    public GotWhichItem Got_Which_Item
    {
        get { return got_which_item; }
        set
        {
            if (got_which_item == value) return;
            switch (got_which_item)
            {
                case GotWhichItem.None:
                    if (value == GotWhichItem.First) GetGift1();
                    else if (value == GotWhichItem.Second) GetGift2();
                    break;
                case GotWhichItem.First:
                    RemoveGift1();
                    if (value == GotWhichItem.Second)
                    {
                        GetGift2();
                    }
                    break;
                case GotWhichItem.Second:
                    RemoveGift2();
                    if (value == GotWhichItem.First)
                    {
                        GetGift1();
                    }
                    break;
            }
            got_which_item = value;
            return;
        }
    }
}

internal abstract class CustomCountedGift : Gift
{
    internal CustomCountedGift(Giftname giftname, int level, string sprite_name) : base(level)
    {
        this.giftname = giftname;
        name_convo = "rogue_" + giftname.ToString() + "_name";
        desc_convo = "rogue_" + giftname.ToString() + "_desc";
        int_convo = "rogue_" + giftname.ToString() + "_got";
        ModHooks.LanguageGetHook += GetNameAndDesc;
        ModHooks.GetPlayerIntHook += GetCount;
        getSprite = () => SpriteLoader.GetSprite(sprite_name);
        SFCore.ItemHelper.AddCountedItem(getSprite(), int_convo, name_convo, desc_convo);
    }

    private int GetCount(string name, int orig)
    {
        if (name == int_convo) return count;
        return orig;
    }
    internal override void GetGift()
    {
        now_weight = 0;
        if (count == 0)
        {

        }
        SetCount(count + 1);
    }
    internal override void RemoveGift()
    {
        now_weight = weight;
        SetCount(0);
    }

    string name_convo;
    string desc_convo;
    string int_convo;
    internal int count;
    protected virtual string GetNameAndDesc(string key, string sheetTitle, string orig)
    {
        if (key == name_convo) { return GetName(); }
        if (key == desc_convo) { return GetDesc(); }
        return orig;
    }
    internal void SetCount(int t)
    {
        if (count == t) return;
        int ori = count;
        count = t;
        _SetCount(ori, t);
    }
    protected abstract void _SetCount(int ori, int now);


}