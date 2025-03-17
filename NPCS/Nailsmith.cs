namespace rogue.NPCs;
internal class Nailsmith : NPC
{
    internal static Vector3 spa_pos = new Vector3(104, 16.14f, 0.1f);
    internal Nailsmith(tk2dSpriteAnimation library) : base(library, new(0, -1.5f), KnightAction.LookUP)
    {
        name = "铁匠";
        name_sub = "";
        name_super = "";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk";
        AddConversation("初遇", "我可以帮你锻造骨钉<page>无需苍白矿石，给我吉欧当报酬就行");
        OnConvoEnd("初遇", () => TryUpdateNail(1));
        AddConversation("接受", "如你所愿");
        AddConversation("拒绝", "请自便");
        AddConversation("最高级", "我有了新的想法<page>如果你愿意给我更多报酬，我可以让骨钉更完美");
        OnConvoEnd("最高级", () => TryUpdateNail(2));
        AddConversation("已经满级", "完美的骨钉，你已经不需要我的帮助了");
    }
    void TryUpdateNail(int mode)
    {
        List<RogueUIManager.SelectItem> selectItems = new();
        RogueUIManager.SelectItem yes = new("是");
        yes.select_action = (select) => { ShowDialogue("接受"); HeroController.instance.TakeGeo(mode == 1 ? 600 : 1000); GiftHelper.AddNailDamage(); };
        yes.selectable = mode == 1 ? PlayerData.instance.geo >= 600 : PlayerData.instance.geo >= 1000;
        yes.not_select_info = "吉欧不足";
        selectItems.Add(yes);
        RogueUIManager.SelectItem no = new("否");
        no.select_action = (select) => { ShowDialogue(" 拒绝"); };
        selectItems.Add(no);
        RogueUIManager.StartSelection(0.3f, "消耗" + (mode == 1 ? 600 : 1000) + "geo" + "来升级骨钉", selectItems, 2);
    }
    internal override void BeginConvo()
    {
        int level = PlayerData.instance.nailDamage / 4;
        if (level < 5) ShowDialogue("初遇");
        else if (level == 5) ShowDialogue("最高级");
        else ShowDialogue("已经满级");
    }
}