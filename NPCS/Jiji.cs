namespace rogue.NPCs;
internal class Jiji : NPC
{
    internal static Vector3 spa_pos = new Vector3(104, 16.54f, 0.1f);
    internal Jiji(tk2dSpriteAnimation library) : base(library, new(0, -2f), KnightAction.LookUP)
    {
        name = "吉吉";
        name_sub = "";
        name_super = "";
        idle_animation_name = "Idle";
        talk_animation_name = "Idle";
        AddConversation("初遇", "你好啊，我不知道怎么就来到了这里<page>这里没有遗憾，但我也捡到了一些稀罕的东西");
        OnConvoEnd("初遇", () => { ShowDialogue("交易"); });
        AddConversation("交易", "给我一点食物，我把这个东西给你，怎么样？");
        OnConvoEnd("交易", TryEggSwap);
        AddConversation("接受", "好了，如果需要调整，我应该也能帮上忙");
        AddConversation("拒绝", "好吧，那就只好饿一会肚子了");
        AddConversation("调整", "需要我调整？那我来试试");
        OnConvoEnd("调整", AdjustGift);
    }
    void TryEggSwap()
    {
        List<RogueUIManager.SelectItem> selectItems = new();
        RogueUIManager.SelectItem yes = new("是");
        yes.select_action = (select) =>
        {
            ShowDialogue("接受");
            GameInfo.revive_num -= 2;
            ((DashDoubleRefresh)GiftFactory.all_gifts[Giftname.custom_one_two_dash_doublejump_refresh]).Got_Which_Item = CustomOneOrTwoGift.GotWhichItem.First;
        };
        yes.not_select_info = "腐臭蛋数量不足";
        yes.selectable = GameInfo.revive_num >= 2;
        selectItems.Add(yes);
        RogueUIManager.SelectItem no = new("否");
        no.select_action = (select) => { ShowDialogue("拒绝"); };
        selectItems.Add(no);
        RogueUIManager.StartSelection(0.3f, "用两个腐臭蛋换稀罕东西？", selectItems, 2);
    }
    void AdjustGift()
    {
        List<RogueUIManager.SelectItem> selectItems = new();
        RogueUIManager.SelectItem item1 = new("冲刺刷新二段");
        item1.select_action = (select) => { ((DashDoubleRefresh)GiftFactory.all_gifts[Giftname.custom_one_two_dash_doublejump_refresh]).Got_Which_Item = CustomOneOrTwoGift.GotWhichItem.First; };
        selectItems.Add(item1);
        RogueUIManager.SelectItem item2 = new("二段刷新冲刺");
        item2.select_action = (select) => { ((DashDoubleRefresh)GiftFactory.all_gifts[Giftname.custom_one_two_dash_doublejump_refresh]).Got_Which_Item = CustomOneOrTwoGift.GotWhichItem.Second; };
        selectItems.Add(item2);
        RogueUIManager.SelectItem item3 = new("取消");
        selectItems.Add(item3);
        RogueUIManager.StartSelection(0.3f, "调整功能", selectItems, 3);
    }
    internal override void BeginConvo()
    {
        if (((DashDoubleRefresh)GiftFactory.all_gifts[Giftname.custom_one_two_dash_doublejump_refresh]).Got_Which_Item == CustomOneOrTwoGift.GotWhichItem.None)
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
        else
        {
            ShowDialogue("调整");
        }

    }


}