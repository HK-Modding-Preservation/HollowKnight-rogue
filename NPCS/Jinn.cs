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
        AddConversation("交易", "npc_jinn_conv_1".Localize());
        OnConvoEnd("交易", TrySwapEgg);
        AddConversation("接受", "npc_jinn_conv_2".Localize());
        AddConversation("拒绝", "npc_jinn_conv_3".Localize());

    }
    internal override string GetName(string pos)
    {
        return Language.Language.Get("JINN_" + pos.ToUpperInvariant(), "Titles");

    }
    void TrySwapEgg()
    {
        List<RogueUIManager.SelectItem> selectItems = new();
        RogueUIManager.SelectItem yes = new(Lang.YES);
        yes.not_select_info = "egg_cant_select_info".Localize();
        yes.selectable = GameInfo.revive_num >= 1;
        yes.select_action = (select) =>
        {
            ShowDialogue("接受");
            GameInfo.revive_num -= 1;
            HeroController.instance.AddGeo(300);
            DisplayManager.DisplayStates();
        };
        selectItems.Add(yes);
        RogueUIManager.SelectItem no = new(Lang.NO);
        no.select_action = (select) => { ShowDialogue("拒绝"); };
        selectItems.Add(no);
        RogueUIManager.StartSelection(0.3f, "npc_jinn_select_conv".Localize(), selectItems, 2);
    }
    internal override void BeginConvo()
    {
        ShowDialogue("交易");
    }

}