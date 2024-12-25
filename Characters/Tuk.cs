
namespace rogue.Characters;
internal class Tuk : Character
{
    public Tuk()
    {
        this.Selfname = CharacterRole.tuk;
    }
    System.Random random = new System.Random();

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.tuk;
        GiftHelper.AdjustMaskTo(3);
        HeroController.instance.AddGeo(500);
        GameInfo.revive_num = 5;
        Rogue.Instance.ShowDreamConvo("Tuk_dream".Localize());
        ItemManager.Instance.after_revive_action += TukRevive;
    }

    private void TukRevive()
    {
        if (random.Next(0, 2) == 0)
        {
            GiftHelper.AddNailDamage();
        }
        else
        {
            GiftHelper.GiveMask();
            GiftHelper.GiveMask();
        }

    }

    public override void EndCharacter()
    {
        ItemManager.Instance.after_revive_action -= TukRevive;
    }
}