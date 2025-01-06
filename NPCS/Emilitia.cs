namespace rogue.NPCs;
internal class Emilitia : NPC
{
    internal Emilitia(tk2dSpriteAnimation library) : base(library, KnightAction.LookUP)
    {
        name = "艾米莉塔";
        name_sub = "";
        name_super = "永恒的";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk L";
    }

}