using rogue.Characters;

namespace rogue.NPCs;

internal class ElderBug : NPC
{
    int birthright_num = 0;
    internal ElderBug(tk2dSpriteAnimation library) : base(library, new(0, -0.2466f), KnightAction.LookUP)
    {
        name = "虫长者";
        name_sub = "test_sub";
        name_super = "test_super";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk Right";
        AddConversation("初次见面", "npc_elder_bug_conv_1".Localize());
        AddConversation("再见", "npc_elder_bug_conv_2".Localize());
        OnConvoEnd("初次见面", TryGiveBirthRight);
    }
    internal static Vector3 spa_pos = new Vector3(103.3f, 15.2f, 0.1f);
    internal override string GetName(string pos)
    {
        switch (pos)
        {
            case main: return Language.Language.Get("ELDERBUG_MAIN", "Titles");
            default: return "";
        }

    }
    void TryGiveBirthRight()
    {
        var role = HeroController.instance.GetComponent<Character>();
        if (role != null && role.GotBirthrightNum() <= birthright_num)
        {
            GiftFactory.all_gifts[Giftname.get_birthright].GetGift();
        }
        else
        {
            ShowDialogue("再见");
        }
    }
    internal override void BeginConvo()
    {
        ShowDialogue("初次见面");
    }
    internal override void SetPosition(Vector3 pos)
    {
        base.SetPosition(pos);
        var role = HeroController.instance.GetComponent<Character>();
        if (role != null)
        {
            birthright_num = role.GotBirthrightNum();
        }

    }


}