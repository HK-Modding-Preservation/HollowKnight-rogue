namespace rogue.NPCs;

internal class QuirrelNail : NPC
{
    internal QuirrelNail(Sprite nail) : base(null, new(0, -0.2466f), KnightAction.LookUP, true)
    {
        go.GetComponent<SpriteRenderer>().sprite = nail;
        name = "";
        name_sub = "";
        name_super = "";
        AddConversation("初遇", "npc_quirrel_nail_conv".Localize());
        OnConvoEnd("初遇", GetNail);
    }
    internal static Vector3 spa_pos = new Vector3(104f, 14.64f, 0.1f);
    internal override string GetName(string pos)
    {
        if (pos == main) return "npc_quirrel_nail_name".Localize();
        return "";
    }
    void GetNail()
    {
        RogueUIManager.StartSelection(0.3f, "npc_quirrel_nail_select".Localize(), new()
        {
            new RogueUIManager.SelectItem(Lang.YES){
                select_action=(t)=>{
                    GameInfo.max_nail_level+=1;
                    GiftHelper.AddNailDamage();
                    DisplayManager.DisplayStates();
                    Rogue.Instance.ShowDreamConvo("npc_quirrel_nail_dream_1".Localize());
                    go.SetActive(false);
                }
            },
            new RogueUIManager.SelectItem(Lang.NO){
                select_action=(t)=>{
                    Rogue.Instance.ShowDreamConvo("npc_quirrel_nail_dream_2".Localize());
                }
            }
        }, 2);
    }
    internal override void BeginConvo()
    {
        ShowDialogue("初遇");
    }

}