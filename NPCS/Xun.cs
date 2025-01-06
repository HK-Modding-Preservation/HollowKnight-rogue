namespace rogue.NPCs;
internal class Xun : NPC
{
    internal Xun(tk2dSpriteAnimation library) : base(library, KnightAction.LookUP)
    {
        name = "灰色哀悼者";
        name_sub = "";
        name_super = "";
        idle_animation_name = "Rest";
        talk_animation_name = "Talk";
    }

}