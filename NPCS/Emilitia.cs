namespace rogue.NPCs;

internal class Emilitia : NPC
{
    internal Emilitia(tk2dSpriteAnimation library) : base(library, new(0, -0.2466f), KnightAction.LookUP)
    {
        name = "艾米莉塔";
        name_sub = "";
        name_super = "永恒的";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk L";
        AddConversation("初遇", "npc_emilitia_conv_1".Localize());
        AddConversation("第一段", "Stepping out of the lift, you see the vessel's monument.Two figures are standing in the rain.");
        AddConversation("第二段", "It is so silent here, no shout, no scream, no booms from those infected body.");
        AddConversation("第三段", "Beside the fountain Leem stands, seems to be staring at the city in the endless rain.");
        AddConversation("第四段", "your eyes focus on qurrel under the statue of the square. Rainwater slowly move through his mask.");
        AddConversation("另一个", "00111,11110<page>00011,01111,11111<page>00001,01111<page>00001,11100");
        OnConvoEnd("初遇", StartStory);

    }
    internal override string GetName(string pos)
    {
        return Language.Language.Get("EMILITIA_NC_" + pos.ToUpperInvariant(), "Titles");

    }
    internal static Vector3 blue_house_pos = new Vector3(98.5145f, 15.1709f, -3.6f);
    internal void StartStory()
    {
        RogueUIManager.StartSelection(0.3f, "npc_emilitia_conv_2".Localize(), new List<RogueUIManager.SelectItem>()
        {
            new RogueUIManager.SelectItem(Lang.YES){
                select_action=StartSelect,
                selectable=true
            },
            new RogueUIManager.SelectItem(Lang.YES){
                select_action=StartSelect,
                selectable=true
            },
            new RogueUIManager.SelectItem(Lang.NO){
                select_action=StartSelect,
                selectable=false
            },
        }, 3);

    }
    internal void StartSelect(int t)
    {
        RogueUIManager.StartSelection(0.3f, "npc_emilitia_conv_3".Localize(), new List<RogueUIManager.SelectItem>()
        {
            new RogueUIManager.SelectItem("npc_emilitia_count_1".Localize()){
                select_action=(t)=>ShowDialogue("第一段"),
                selectable=true
            },
            new RogueUIManager.SelectItem("npc_emilitia_count_2".Localize()){
                select_action=(t)=>ShowDialogue("第二段"),
                selectable=true
            },
            new RogueUIManager.SelectItem("npc_emilitia_count_3".Localize()){
                select_action=(t)=>ShowDialogue("第三段"),
                selectable=true
            },
            new RogueUIManager.SelectItem("npc_emilitia_count_4".Localize()){
                select_action=(t)=>ShowDialogue("第四段"),
                selectable=true
            },
            new RogueUIManager.SelectItem(Lang.Cancel){
                select_action=(t)=>StartSelect(t),
                selectable=false
            },
        }, 5);
    }
    internal override void BeginConvo()
    {
        if (PlayerData.instance.statueStateNosk.usingAltVersion)
        {
            ShowDialogue("初遇");
        }
        else
        {
            ShowDialogue("另一个");
        }
    }


}