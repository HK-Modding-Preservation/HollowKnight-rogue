
namespace rogue.NPCs;

internal class Xun : NPC
{
    internal static Vector3 spa_pos = new Vector3(104, 17.54f, 0.1f);
    internal Xun(tk2dSpriteAnimation library) : base(library, new(0, -3f), KnightAction.LookUP)
    {
        name = "灰色哀悼者";
        name_sub = "";
        name_super = "";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk";
        AddConversation("遇见", "npc_xun_conv_1".Localize());
        AddConversation("拿花", "npc_xun_conv_2".Localize());
        AddConversation("不拿花", "npc_xun_conv_3".Localize());
        OnConvoEnd("遇见", TryGiveFlower);
        AddConversation("已经有花", "npc_xun_conv_4".Localize());
    }
    internal override string GetName(string pos)
    {
        return Language.Language.Get("XUN_" + pos.ToUpperInvariant(), "Titles");
    }
    void TryGiveFlower()
    {
        List<RogueUIManager.SelectItem> selectItems = new();
        RogueUIManager.SelectItem yes = new(Lang.YES);
        yes.select_action = (select) => { ShowDialogue("拿花"); PlayerData.instance.hasXunFlower = true; PlayerData.instance.xunFlowerBroken = false; };
        selectItems.Add(yes);
        RogueUIManager.SelectItem no = new(Lang.NO);
        no.select_action = (select) => { ShowDialogue("不拿花"); };
        selectItems.Add(no);
        RogueUIManager.StartSelection(0.3f, "npc_xun_select_conv".Localize(), selectItems, 2);
    }
    internal override void BeginConvo()
    {
        if (!(PlayerData.instance.hasXunFlower && !PlayerData.instance.xunFlowerBroken)) ShowDialogue("遇见");
        else ShowDialogue("已经有花");
    }

}