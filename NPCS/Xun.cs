
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
        AddConversation("遇见", "送你一朵小白花");
        AddConversation("拿花", "谢谢你");
        AddConversation("不拿花", "我不该有这些奢望");
        OnConvoEnd("遇见", TryGiveFlower);
        AddConversation("已经有花", "一路小心");
    }
    void TryGiveFlower()
    {
        List<RogueUIManager.SelectItem> selectItems = new();
        RogueUIManager.SelectItem yes = new("是");
        yes.select_action = (select) => { ShowDialogue("拿花"); PlayerData.instance.hasXunFlower = true; PlayerData.instance.xunFlowerBroken = false; };
        selectItems.Add(yes);
        RogueUIManager.SelectItem no = new("否");
        no.select_action = (select) => { ShowDialogue("不拿花"); };
        selectItems.Add(no);
        RogueUIManager.StartSelection(0.3f, "接受这朵花?", selectItems, 2);
    }
    internal override void BeginConvo()
    {
        if (!PlayerData.instance.hasXunFlower) ShowDialogue("遇见");
        else ShowDialogue("已经有花");
    }

}