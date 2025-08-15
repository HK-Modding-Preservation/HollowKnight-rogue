

namespace rogue.Characters;

internal class Mantis : Character
{
    public Mantis()
    {
        this.Selfname = CharacterRole.mantis;
        nail_mul = 1.25f;
        AddBirthRight("纯钉", 0, 2);
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.mantis;
        PlayerData.instance.hasWalljump = true;
        CharmHelper.SetCantUnequip(13);
        PlayerData.instance.gotCharm_13 = true;
        PlayerData.instance.hasSpell = false;
        GiftHelper.AddNailDamage();
        Rogue.Instance.ShowDreamConvo("mantis_dream".Localize());
        On.PlayerData.GetInt += FreePrideMask;
        GiftHelper.max_fireball_level += NoSpell;
        GiftHelper.max_quake_level += NoSpell;
        GiftHelper.max_scream_level += NoSpell;
    }

    private bool NoSpell(bool orig)
    {
        return true;
    }

    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:

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
                if (birthrights[0].got == 2) nail_mul = 1.4f;
                else if (birthrights[0].got == 1) nail_mul = 1.25f;
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
        PlayerData.instance.hasSpell = true;
        CharmHelper.SetCanEquip(13);
        On.PlayerData.GetInt -= FreePrideMask;
        GiftHelper.max_fireball_level -= NoSpell;
        GiftHelper.max_quake_level -= NoSpell;
        GiftHelper.max_scream_level -= NoSpell;
    }
}