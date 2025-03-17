namespace rogue.NPCs;
internal class ElderBug : NPC
{
    internal ElderBug(tk2dSpriteAnimation library) : base(library, new(0, -0.2466f), KnightAction.LookUP)
    {
        name = "虫长者";
        name_sub = "test_sub";
        name_super = "test_super";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk Right";

    }


}