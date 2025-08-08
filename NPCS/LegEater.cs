namespace rogue.NPCs;

internal class LegEater : NPC
{
    internal LegEater(tk2dSpriteAnimation library) : base(library, new(0, -1.5f), KnightAction.LookUP)
    {
        name = "食腿者";
        name_sub = "";
        name_super = "";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk Left";
    }
    Vector3 spa_pos = new(0, 0, 0);

}