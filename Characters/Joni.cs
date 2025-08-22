

namespace rogue.Characters;

internal class Joni : Character
{
    public Joni()
    {
        Selfname = CharacterRole.joni;
        nail_mul = 1.75f;
        spell_mul = 1.75f;
        AddBirthRight("joni_birthright_0_name".Localize(), 0, 2);
        AddBirthRight("joni_birthright_1_name".Localize());
        AddBirthRight("joni_birthright_2_name".Localize());
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.joni;
        PlayerData.instance.charmSlots = 11;
        GiftHelper.AdjustMaskTo(9);
        GiftHelper.AdjustVesselTo(3);
        GiftHelper.GiveAllCharms();
        CharmHelper.SetCantUnequip(27);
        ModHooks.GetPlayerIntHook += LowCost;
        Rogue.Instance.ShowDreamConvo("joni_dream".Localize());
    }

    private int LowCost(string name, int orig)
    {

        if (name == "charmCost_27") return 4 - (2 * birthrights[0].got);
        return orig;
    }

    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                break;
            case 1:
                free_charms.Add(29);
                On.PlayMakerFSM.OnEnable += FastHeal;
                break;
            case 2:
                free_charms.Add(8);
                break;


        }
    }

    private void FastHeal(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.name.Contains("Hive") && self.FsmName == "blue_health_display")
        {
            self.FsmVariables.FindFsmFloat("Recover Time").Value /= 3;
        }
        orig(self);
    }

    protected override void AfterRevive()
    {
        // base.AfterRevive();
    }
    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 1:
                free_charms.Remove(29);
                On.PlayMakerFSM.OnEnable -= FastHeal;
                break;
            case 2:
                free_charms.Remove(8);
                break;
            default:
                break;
        }
    }

    public override void EndCharacter()
    {
        ModHooks.GetPlayerIntHook -= LowCost;

    }
}
