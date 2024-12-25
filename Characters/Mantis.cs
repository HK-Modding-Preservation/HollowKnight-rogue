
namespace rogue.Characters;
internal class Mantis : Character
{
    public Mantis()
    {
        this.Selfname = CharacterRole.mantis;
        nail_mul = 1.25f;
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.mantis;
        PlayerData.instance.hasWalljump = true;
        CharmHelper.SetCantUnequip(13);
        PlayerData.instance.gotCharm_13 = true;
        Rogue.Instance.ShowDreamConvo("mantis_dream".Localize());
        On.PlayerData.GetInt += FreePrideMask;
    }

    private int FreePrideMask(On.PlayerData.orig_GetInt orig, PlayerData self, string intName)
    {
        if (intName == "charmCost_13") return 0;
        return orig(self, intName);
    }

    public override void EndCharacter()
    {
        CharmHelper.SetCanEquip(13);
        On.PlayerData.GetInt -= FreePrideMask;
    }
}