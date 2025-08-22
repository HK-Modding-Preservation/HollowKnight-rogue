using rogue;
internal class RefreshDash : CustomGift
{
    public RefreshDash() : base(Giftname.custom_refresh_dash, 3, "b_alubab_aluba")
    {
        weight = 1f;
        price = 200;
        name = "custom_refresh_dash_name";
        desc = "custom_refresh_dash_desc";
    }

    protected override void _GetGift()
    {
        On.HealthManager.TakeDamage += RefreshDashAndDoublejump;
        Rogue.Instance.Log("Get RefreshDash");

    }

    private void RefreshDashAndDoublejump(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        if (hitInstance.DamageDealt > 0 && hitInstance.AttackType == AttackTypes.Nail)
        {
            ReflectionHelper.SetField(HeroController.instance, "doubleJumped", false);
            ReflectionHelper.SetField(HeroController.instance, "airDashed", false);
        }
        orig(self, hitInstance);
    }

    protected override void _RemoveGift()
    {
        On.HealthManager.TakeDamage -= RefreshDashAndDoublejump;
        Rogue.Instance.Log("Remove refreshDash");

    }


}