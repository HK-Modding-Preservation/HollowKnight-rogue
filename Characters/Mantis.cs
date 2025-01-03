
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
    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                PlayerData.instance.hasSpell = false;
                if (nail_mul < 1.4f) nail_mul = 1.4f;
                else nail_mul = 1.5f;
                break;
        }
    }
    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                PlayerData.instance.hasSpell = true;
                nail_mul = 1.25f;
                break;
        }
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