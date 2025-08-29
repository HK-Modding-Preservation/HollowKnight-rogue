namespace rogue.NPCs;

internal class Nailsmith : NPC
{
    internal static Vector3 spa_pos = new Vector3(104, 16.14f, 0.1f);
    bool update = false;
    internal Nailsmith(tk2dSpriteAnimation library) : base(library, new(0, -1.5f), KnightAction.LookUP)
    {
        name = "铁匠";
        name_sub = "";
        name_super = "";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk";
        AddConversation("初遇", "npc_smith_conv_1".Localize());
        OnConvoEnd("初遇", () => TryUpdateNail(1));
        AddConversation("接受", "npc_smith_conv_2".Localize());
        AddConversation("拒绝", "npc_smith_conv_3".Localize());
        AddConversation("最高级", "npc_smith_conv_4".Localize());
        OnConvoEnd("最高级", () => TryUpdateNail(2));
        AddConversation("已经满级", "npc_smith_conv_5".Localize());
    }
    internal override string GetName(string pos)
    {
        return Language.Language.Get("NAILSMITH_" + pos.ToUpperInvariant(), "Titles");
    }
    void TryUpdateNail(int mode)
    {
        List<RogueUIManager.SelectItem> selectItems = new();
        RogueUIManager.SelectItem yes = new(Lang.YES);
        yes.select_action = (select) =>
        {
            ShowDialogue("接受");
            int nail_damage = PlayerData.instance.nailDamage + 4;
            if (mode == 2)
            {
                GameInfo.max_nail_level += 1;
                update = true;
            }
            HeroController.instance.TakeGeo(mode == 1 ? 600 : 1000); GiftHelper.AddNailDamage();
            DisplayManager.DisplayStates();
        };
        yes.selectable = mode == 1 ? PlayerData.instance.geo >= 600 : PlayerData.instance.geo >= 1000;
        yes.not_select_info = "geo_cant_select_info".Localize();
        selectItems.Add(yes);
        RogueUIManager.SelectItem no = new(Lang.NO);
        no.select_action = (select) => { ShowDialogue(" 拒绝"); };
        selectItems.Add(no);
        RogueUIManager.StartSelection(0.3f, "npc_smith_update_conv_1".Localize() + (mode == 1 ? 600 : 1000) + "geo" + "npc_smith_update_conv_2".Localize(), selectItems, 2);
    }
    internal override void SetPosition(Vector3 pos)
    {
        update = false;
        base.SetPosition(pos);
    }
    internal override void BeginConvo()
    {
        int level = GiftHelper.GetNailLevel();
        if (!GiftHelper.MaxNailDamage()) ShowDialogue("初遇");
        else if (!update) ShowDialogue("最高级");
        else ShowDialogue("已经满级");
    }
}