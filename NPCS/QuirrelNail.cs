namespace rogue.NPCs;
internal class QuirrelNail : NPC
{
    internal QuirrelNail(Sprite nail) : base(null, KnightAction.LookUP, true)
    {
        go.GetComponent<SpriteRenderer>().sprite = nail;
        name = "";
        name_sub = "";
        name_super = "";
    }

}