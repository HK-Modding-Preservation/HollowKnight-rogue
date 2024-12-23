namespace rogue.Characters;
internal class Tuk : Character
{
    public Tuk()
    {
        this.Selfname = CharacterRole.Tuk;
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.Tuk;
        GiftHelper.AdjustMaskTo(3);
        HeroController.instance.AddGeo(500);
        GameInfo.revive_num = 5;
        Rogue.Instance.ShowDreamConvo("Tuk_dream".Localize());
    }

    public override void EndCharacter()
    {
    }
}