namespace rogue.Characters;
internal class Joni : Character
{

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.joni;
        PlayerData.instance.charmSlots = 11;
        GiftHelper.AdjustMaskTo(9);
        GiftHelper.AdjustVesselTo(3);
        GiftHelper.GiveAllCharms();
        if (!PlayerData.instance.equippedCharms.Contains(27))
        {
            PlayerData.instance.equippedCharms.Insert(1, 27);
            PlayerData.instance.equippedCharm_27 = true;
        }
        Rogue.Instance.ShowDreamConvo("joni_dream".Localize());
    }
    protected override void AfterRevive()
    {

    }

    public override void EndCharacter()
    {

    }
}
