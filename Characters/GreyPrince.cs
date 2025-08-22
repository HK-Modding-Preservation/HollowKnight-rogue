
namespace rogue.Characters;

internal class GreyPrince : Character
{
    public GreyPrince()
    {
        this.Selfname = CharacterRole.grey_prince;
        AddBirthRight("grey_prince_birthright_0_name".Localize());
        AddBirthRight("grey_prince_birthright_1_name".Localize());
        AddBirthRight("grey_prince_birthright_2_name".Localize());
    }
    bool charm_to_power = false;

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.grey_prince;
        PlayerData.instance.fireballLevel = 1;
        PlayerData.instance.screamLevel = 1;
        PlayerData.instance.quakeLevel = 1;
        KeepNailDamage();
        PlayerData.instance.hasDash = true;
        PlayerData.instance.canDash = true;
        PlayerData.instance.hasShadowDash = true;
        PlayerData.instance.canShadowDash = true;
        PlayerData.instance.hasDreamNail = true;
        charm_to_power = false;
        Rogue.Instance.ShowDreamConvo("grey_prince_dream".Localize());
        On.PlayerData.GetInt += FreeAllCharms;
        GiftFactory.after_update_weight += KeepNailDamage;
    }
    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                PlayerData.instance.gotCharm_24 = true;
                free_charms.Add(24);
                break;
            case 1:
                HeroController.instance.AddGeo(400);
                break;
            case 2:
                charm_to_power = true;
                break;
        }
    }
    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                free_charms.Remove(24);
                break;
            case 1:
                break;
            case 2:
                charm_to_power = false;
                break;
        }
    }
    private void KeepNailDamage()
    {
        var damage = 5;
        if (charm_to_power)
        {
            damage = PlayerData.instance.equippedCharms.Count / 2;
        }
        PlayerData.instance.nailDamage = damage;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
    }

    private int FreeAllCharms(On.PlayerData.orig_GetInt orig, PlayerData self, string intName)
    {
        if (intName.StartsWith("charmCost_")) return 0;
        return orig(self, intName);
    }

    public override void EndCharacter()
    {
        On.PlayerData.GetInt -= FreeAllCharms;
        GiftFactory.after_update_weight -= KeepNailDamage;
    }
}