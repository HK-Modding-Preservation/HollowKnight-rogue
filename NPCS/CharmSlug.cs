namespace rogue.NPCs;
internal class CharmSlug : NPC
{
    GameObject desk = null;
    internal CharmSlug(tk2dSpriteAnimation library, GameObject desk) : base(library, new(0, -0.2466f), KnightAction.LookUP)
    {
        name = "萨鲁巴";
        name_sub = "";
        name_super = "护符爱好者";
        idle_animation_name = "Idle";
        talk_animation_name = "Talk Start";
        this.desk = desk;
        this.desk.transform.SetParent(go.transform);
        this.desk.transform.localPosition = new Vector3(0, -0.8193f, -0.277f);
        this.desk.SetActive(true);
        this.go.GetComponent<BoxCollider2D>().enabled = false;
    }

}