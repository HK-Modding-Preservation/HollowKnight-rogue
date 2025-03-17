namespace rogue.NPCs;
internal class Jinn : NPC
{
    internal static Vector3 spa_pos = new Vector3(104, 16.94f, 0.1f);
    internal Jinn(tk2dSpriteAnimation library) : base(library, new(0, -2.5f), KnightAction.LookUP)
    {
        name = "吉恩";
        name_sub = "";
        name_super = "钢铁之魂";
        idle_animation_name = "Sleep";
        talk_animation_name = "Talk";
        AddConversation("交易", "……它回来了。它记得吉恩吗？它会给礼物吗？交换没用的发光的东西？");
        OnConvoEnd("交易", TrySwapEgg);
        AddConversation("接受", "发光的东西给小家伙……给发光的东西好。减少了吉恩的负担。");
        AddConversation("拒绝", "……没关系……");

    }
    void TrySwapEgg()
    {
        List<RogueUIManager.SelectItem> selectItems = new();
        RogueUIManager.SelectItem yes = new("是");
        yes.not_select_info = "腐臭蛋数量不足";
        yes.selectable = GameInfo.revive_num >= 1;
        yes.select_action = (select) => { ShowDialogue("接受"); GameInfo.revive_num -= 1; HeroController.instance.AddGeo(300); };
        selectItems.Add(yes);
        RogueUIManager.SelectItem no = new("否");
        no.select_action = (select) => { ShowDialogue("拒绝"); };
        selectItems.Add(no);
        RogueUIManager.StartSelection(0.3f, "给吉恩一个腐臭蛋？", selectItems, 2);
    }
    internal override void BeginConvo()
    {
        ShowDialogue("交易");
    }

}