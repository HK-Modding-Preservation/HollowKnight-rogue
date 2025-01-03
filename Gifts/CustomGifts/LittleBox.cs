namespace rogue;
internal class LittleBox : CustomGift
{
    public LittleBox() : base(Giftname.custom_little_box, 4, "health_butterfly")
    {
        weight = 1f;
        price = 200;
        name = "变小";
        desc = "小骑士的受伤碰撞箱变小";
    }

    protected override void _GetGift()
    {
        var hero_box = HeroController.instance.gameObject.FindGameObjectInChildren("HeroBox");
        hero_box.transform.localPosition = new Vector3(0, -0.8f, 0);
        hero_box.transform.localScale = new Vector3(0.4f, 0.1f, 1);
    }

    protected override void _RemoveGift()
    {
        var hero_box = HeroController.instance.gameObject.FindGameObjectInChildren("HeroBox");
        hero_box.transform.localPosition = new Vector3(0, 0f, 0);
        hero_box.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}