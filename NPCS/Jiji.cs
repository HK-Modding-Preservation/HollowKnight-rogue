namespace rogue.NPCs;
internal class Jiji : NPC
{
    internal Jiji(tk2dSpriteAnimation library) : base(library, KnightAction.LookUP)
    {
        name = "吉吉";
        name_sub = "";
        name_super = "";
        idle_animation_name = "Idle";
        talk_animation_name = "Idle";
    }

}