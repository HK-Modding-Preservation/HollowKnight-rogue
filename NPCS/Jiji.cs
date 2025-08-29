using rogue.Characters;

namespace rogue.NPCs;

internal class Jiji : NPC
{
    internal static Vector3 spa_pos = new Vector3(104, 16.54f, 0.1f);
    int num_birthright;
    bool get_birthright = false;
    internal Jiji(tk2dSpriteAnimation library) : base(library, new(0, -2f), KnightAction.LookUP)
    {
        name = "吉吉";
        name_sub = "";
        name_super = "";
        idle_animation_name = "Idle";
        talk_animation_name = "Idle";
        AddConversation("初遇", "npc_jiji_conv_1".Localize());
        OnConvoEnd("初遇", () => { ShowDialogue("交易"); });
        AddConversation("交易", "npc_jiji_conv_2".Localize());
        OnConvoEnd("交易", TryEggSwap);
        AddConversation("拒绝", "npc_jiji_conv_3".Localize());
    }
    internal override string GetName(string pos)
    {
        if (pos == main) return Language.Language.Get("JIJI_MAIN", "Titles");
        return "";
    }
    void TryEggSwap()
    {
        List<RogueUIManager.SelectItem> selectItems = new();
        RogueUIManager.SelectItem yes = new(Lang.YES);
        yes.select_action = (select) =>
        {
            GameInfo.revive_num -= 2;
            if (!get_birthright)
            {
                get_birthright = true;
                GiftFactory.all_gifts[Giftname.get_birthright].GetGift();
                DisplayManager.DisplayStates();
            }
            else
            {
                var gifts = GameInfo.act_gifts[GiftFactory.CustomVariety()];
                var gift = ItemManager.RandomList(gifts, 1)[0];
                gift.GetGift();
                if (gift.showConvo) Rogue.Instance.ShowConvo(gift.GetShowString());
                DisplayManager.DisplayStates();
            }


        };
        yes.not_select_info = "egg_cant_select_info".Localize();
        yes.selectable = GameInfo.revive_num >= 2;
        selectItems.Add(yes);
        RogueUIManager.SelectItem no = new(Lang.NO);
        no.select_action = (select) => { ShowDialogue("拒绝"); };
        selectItems.Add(no);
        RogueUIManager.StartSelection(0.3f, "npc_jiji_select_conv".Localize(), selectItems, 2);
    }
    // void AdjustGift()
    // {
    //     List<RogueUIManager.SelectItem> selectItems = new();
    //     RogueUIManager.SelectItem item1 = new("冲刺刷新二段");
    //     item1.select_action = (select) => { ((DashDoubleRefresh)GiftFactory.all_gifts[Giftname.custom_one_two_dash_doublejump_refresh]).Got_Which_Item = CustomOneOrTwoGift.GotWhichItem.First; };
    //     selectItems.Add(item1);
    //     RogueUIManager.SelectItem item2 = new("二段刷新冲刺");
    //     item2.select_action = (select) => { ((DashDoubleRefresh)GiftFactory.all_gifts[Giftname.custom_one_two_dash_doublejump_refresh]).Got_Which_Item = CustomOneOrTwoGift.GotWhichItem.Second; };
    //     selectItems.Add(item2);
    //     RogueUIManager.SelectItem item3 = new("取消");
    //     selectItems.Add(item3);
    //     RogueUIManager.StartSelection(0.3f, "调整功能", selectItems, 3);
    // }
    internal override void SetPosition(Vector3 pos)
    {
        base.SetPosition(pos);
        var role = HeroController.instance.GetComponent<Character>();
        num_birthright = role.GotBirthrightNum();

    }
    internal override void BeginConvo()
    {
        if (!meet)
        {
            meet = true;
            ShowDialogue("初遇");
        }
        else
        {
            ShowDialogue("交易");
        }


    }


}