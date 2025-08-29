

namespace rogue;

internal class AnotherCollider : CustomOneOrTwoGift
{
    internal AnotherCollider() : base(Giftname.custom_one_two_another_collider, 2, "keeper_key", "easy_key")
    {
        name = "custom_one_two_another_collider_name";
        desc = "custom_one_two_another_collider_desc";
        weight = 0.5f;
    }

    internal override string GetDesc1()
    {
        return "custom_one_two_another_collider_desc_1".Localize();
    }

    internal override string GetDesc2()
    {
        return "custom_one_two_another_collider_desc_2".Localize();
    }



    internal override string GetName1()
    {
        return "custom_one_two_another_collider_name_1".Localize();
    }

    internal override string GetName2()
    {
        return "custom_one_two_another_collider_name_2".Localize();
    }

    internal override void GetGift2()
    {
        GameObject shield = new("another_collider_shield");
        shield.transform.SetParent(HeroController.instance.gameObject.transform, false);
        shield.transform.localPosition = new(0, 0, 0);
        shield.layer = LayerMask.NameToLayer("Orbit Shield");
        var box = shield.AddComponent<BoxCollider2D>();
        box.size = new Vector2(0.1f, 0.1f);
        box.offset = new Vector2(0, 0f);
        box.isTrigger = false;
    }


    internal override void RemoveGift2()
    {
        GameObject shield = HeroController.instance.gameObject.FindGameObjectInChildren("another_collider_shield");
        if (shield != null)
        {
            GameObject.Destroy(shield);
        }


    }
    internal override void GetGift1()
    {
        GameObject shield = new("another_collider_shield");
        shield.transform.SetParent(HeroController.instance.gameObject.transform, false);
        shield.transform.localPosition = new(0, 0, 0);
        shield.transform.SetScaleX(-1);
        shield.layer = LayerMask.NameToLayer("Orbit Shield");
        var box = shield.AddComponent<BoxCollider2D>();
        box.size = new Vector2(0.1f, 0.1f);
        box.offset = new Vector2(0.3f, -0.75f);
        box.isTrigger = false;
    }


    internal override void RemoveGift1()
    {
        GameObject shield = HeroController.instance.gameObject.FindGameObjectInChildren("another_collider_shield");
        if (shield != null)
        {
            GameObject.Destroy(shield);
        }
    }
}