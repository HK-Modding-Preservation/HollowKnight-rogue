
namespace rogue.Characters;
internal class Mantis : Character
{
    public Mantis()
    {
        this.Selfname = CharacterRole.mantis;
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.mantis;
        PlayerData.instance.hasWalljump = true;
        PlayerData.instance.gotCharm_13 = true;
        if (!PlayerData.instance.equippedCharms.Contains(13))
        {
            PlayerData.instance.equippedCharms.Insert(1, 13);
            PlayerData.instance.equippedCharm_13 = true;
            PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
            // HeroController.instance.CharmUpdate();
        }
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
        On.PlayerData.GetInt -= FreePrideMask;
    }
}