namespace rogue.Characters;
internal class Joni : Character
{
    public Joni()
    {
        Selfname = CharacterRole.joni;
        nail_mul = 1.75f;
        spell_mul = 1.75f;
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.joni;
        PlayerData.instance.charmSlots = 11;
        GiftHelper.AdjustMaskTo(9);
        GiftHelper.AdjustVesselTo(3);
        GiftHelper.GiveAllCharms();
        CharmHelper.SetCantUnequip(27);
        Rogue.Instance.ShowDreamConvo("joni_dream".Localize());
    }
    protected override void AfterRevive()
    {

    }

    public override void EndCharacter()
    {

    }
}
