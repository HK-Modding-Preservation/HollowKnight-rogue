namespace rogue.NPCs;
internal class Nailsmith : NPC
{
    internal Nailsmith(tk2dSpriteAnimation library) : base(library, KnightAction.LookUP)
    {
        name = "铁匠";
        name_sub = "";
        name_super = "";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk";
    }

}