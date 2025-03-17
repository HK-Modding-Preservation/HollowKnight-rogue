using IL.InControl.UnityDeviceProfiles;
using rogue;

internal class Trinket1 : CustomCountedGift
{
    System.Random random = new System.Random();
    public Trinket1() : base(Giftname.custom_counted_trinket1, 3, "trinket1")
    {
        weight = 0f;
        price = 200;
    }

    protected override void _SetCount(int ori, int t)
    {
        if (ori > 0 && t == 0)
        {
            ItemManager.Instance.after_scene_add_geo_num -= GetAnotherDiary;
        }
        else if (ori == 0 && t > 0)
        {
            ItemManager.Instance.after_scene_add_geo_num += GetAnotherDiary;
        }
    }
    internal override string GetName()
    {
        return Language.Language.Get("INV_NAME_TRINKET1", "UI");
    }
    internal override string GetDesc()
    {
        return Language.Language.Get("INV_DESC_TRINKET1", "UI");
    }

    private int GetAnotherDiary(int geo, int damage_num)
    {
        if (count >= 10) return geo;
        int t = random.Next(1, 101);
        if (t < count * 5)
        {
            ItemManager.Instance.StartCoroutine(DelayShowDreamConvo());
            SetCount(count + 1);
        }
        return geo;
    }
    IEnumerator DelayShowDreamConvo()
    {
        yield return new WaitForSeconds(0.5f);
        Rogue.Instance.ShowDreamConvo("你的经历又填满一页日记".Localize());
    }
}
internal class Trinket2 : CustomGift
{
    internal Trinket2() : base(Giftname.custom_counted_trinket2, 4, "trinket2")
    {
        weight = 0.5f;
        price = 200;
    }

    protected override void _GetGift()
    {
        On.HeroController.TakeGeo += FreeOnce;
    }

    private void FreeOnce(On.HeroController.orig_TakeGeo orig, HeroController self, int amount)
    {
        if (amount > 0)
        {
            RemoveGift();
            orig(self, 0);
        }
    }

    protected override void _RemoveGift()
    {
        On.HeroController.TakeGeo -= FreeOnce;
    }
    internal override string GetName()
    {
        return Language.Language.Get("INV_NAME_TRINKET2", "UI");
    }
    internal override string GetDesc()
    {
        return Language.Language.Get("INV_DESC_TRINKET2", "UI");
    }
}

internal class Trinket3 : CustomGift
{
    internal Trinket3() : base(Giftname.custom_counted_trinket3, 4, "trinket3")
    {
        weight = 0.5f;
        price = 200;
    }

    protected override void _GetGift()
    {
        HeroController.instance.AddGeo(PlayerData.instance.geo);
        Rogue.Instance.ShowDreamConvo("让圣巢的苍白之王为你解除负担。".Localize());
    }

    protected override void _RemoveGift()
    {

    }
    internal override string GetName()
    {
        return Language.Language.Get("INV_NAME_TRINKET3", "UI");
    }
    internal override string GetDesc()
    {
        return Language.Language.Get("INV_DESC_TRINKET3", "UI");
    }
}


internal class Trinket4 : CustomGift
{
    internal Trinket4() : base(Giftname.custom_counted_trinket4, 4, "trinket4")
    {
        weight = 0.5f;
        price = 200;
    }
    int count = 0;
    const int max = 25;
    protected override void _GetGift()
    {
        On.HeroController.AddHealth += GiveEgg;
        count = 0;
    }

    private void GiveEgg(On.HeroController.orig_AddHealth orig, HeroController self, int amount)
    {
        if (ItemManager.Instance.scenename != "GG_Spa")
        {
            int t = amount;
            t -= Math.Min(amount, max - count);
            count += amount;
            if (count >= max)
            {
                count = max;
                GiveRandomGift();
                On.HeroController.AddHealth -= GiveEgg;
            }
            orig(self, t);
        }
        else
        {
            orig(self, amount);
        }
    }

    private void GiveRandomGift()
    {
        var gifts = ItemManager.Instance.RandomList(GameInfo.act_gifts[GiftVariety.item], 1);
        if (gifts.Count == 0)
        {
            Rogue.Instance.ShowConvo("什么都没有发生".Localize());
        }
        else
        {
            var gift = gifts[0];
            gift.GetGift();
            Rogue.Instance.ShowConvo(gift.GetShowString());
        }
    }

    protected override void _RemoveGift()
    {
        On.HeroController.AddHealth -= GiveEgg;
        count = 0;
    }
    internal override string GetName()
    {
        return Language.Language.Get("INV_NAME_TRINKET4", "UI");
    }
    internal override string GetDesc()
    {
        return Language.Language.Get("INV_DESC_TRINKET4", "UI") + "\n\n" + "当前进度：".Localize() + count + "/" + max;
    }
}
