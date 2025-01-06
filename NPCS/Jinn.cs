namespace rogue.NPCs;
internal class Jinn : NPC
{
    internal Jinn(tk2dSpriteAnimation library) : base(library, KnightAction.LookUP)
    {
        name = "吉恩";
        name_sub = "";
        name_super = "钢铁之魂";
        idle_animation_name = "Sleep";
        talk_animation_name = "Talk";
    }

}