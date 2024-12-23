
namespace rogue.Characters;

internal class NailMaster : Character
{
    public NailMaster()
    {
        this.Selfname = CharacterRole.nail_master;
    }
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.nail_master;
        GiftHelper.AddNailDamage();
        GiftHelper.AddNailDamage();
        PlayerData.instance.hasCyclone = true;
        PlayerData.instance.hasDashSlash = true;
        PlayerData.instance.hasUpwardSlash = true;
        PlayerData.instance.hasNailArt = true;
        Rogue.Instance.ShowDreamConvo("nail_master_dream".Localize());
        On.PlayerData.GetInt += FreeNailGlory;
    }

    private int FreeNailGlory(On.PlayerData.orig_GetInt orig, PlayerData self, string intName)
    {
        if (intName == "charmCost_26") return 0;
        return orig(self, intName);
    }

    public override void EndCharacter()
    {
        On.PlayerData.GetInt -= FreeNailGlory;
    }
}