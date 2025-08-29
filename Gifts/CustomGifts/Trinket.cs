using IL.InControl.UnityDeviceProfiles;
using rogue;

internal class Trinket1 : CustomCountedGift
{
    System.Random random = new System.Random();
    public Trinket1() : base(Giftname.custom_counted_trinket1, 4, "trinket1")
    {
        weight = 0f;
        price = 200;
    }

    protected override void _SetCount(int ori, int t)
    {
        if (ori > 0 && t == 0)
        {
            ProcessManager.Instance.after_scene_add_geo_num -= GetAnotherDiary;
            On.HealthManager.TakeDamage -= DiaryDamage;
        }
        else if (ori == 0 && t > 0)
        {
            ProcessManager.Instance.after_scene_add_geo_num += GetAnotherDiary;
            On.HealthManager.TakeDamage += DiaryDamage;
        }
    }

    private void DiaryDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        hitInstance.Multiplier *= (1f + (0.03f * count));
        orig(self, hitInstance);
    }

    internal override string GetName()
    {
        return Language.Language.Get("INV_NAME_TRINKET1", "UI");
    }
    internal override string GetDesc()
    {
        return "custom_trinket1_desc".Localize();
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
        Rogue.Instance.ShowDreamConvo("custom_counted_trinket1_convo".Localize());
    }
}
internal class Trinket2 : CustomGift
{
    internal Trinket2() : base(Giftname.custom_counted_trinket2, 1, "trinket2")
    {
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
        return "custom_trinket2_desc".Localize();
    }
}

internal class Trinket3 : CustomGift
{
    internal Trinket3() : base(Giftname.custom_counted_trinket3, 2, "trinket3")
    {
        price = 200;
    }

    protected override void _GetGift()
    {
        HeroController.instance.AddGeo(PlayerData.instance.geo);
        Rogue.Instance.ShowDreamConvo("custom_counted_trinket3_convo".Localize());
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
        return "custom_trinket3_desc".Localize();
    }
}


internal class Trinket4 : CustomGift
{
    internal Trinket4() : base(Giftname.custom_counted_trinket4, 0, "trinket4")
    {
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
        if (ProcessManager.scene_name != "GG_Spa")
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
        ItemManager.Instance.rewardsStack.Push(new ItemManager.OneReward()
        {
            give_mode = ItemManager.OneReward.GiveMode.random,
            gift_variety = GiftVariety.custom,
            give = 4,
            select = 2,
            gifts = null
        });
        Rogue.Instance.ShowDreamConvo("something_change".Localize());
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
        return "custom_trinket4_desc".Localize() + "\n\n" + count + "/" + max;
    }
}
