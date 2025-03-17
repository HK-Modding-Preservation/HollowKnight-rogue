
namespace rogue.Characters;
internal class GreyPrince : Character
{
    public GreyPrince()
    {
        this.Selfname = CharacterRole.grey_prince;
    }

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
        Rogue.Instance.ShowDreamConvo("grey_prince_dream".Localize());
        On.PlayerData.GetInt += FreeAllCharms;
        GiftFactory.after_update_weight += KeepNailDamage;
    }
    private void KeepNailDamage()
    {
        PlayerData.instance.nailDamage = 5;
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