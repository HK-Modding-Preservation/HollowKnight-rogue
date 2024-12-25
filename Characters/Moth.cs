
namespace rogue.Characters;
internal class Moth : Character
{
    public Moth()
    {
        this.Selfname = CharacterRole.moth;
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.moth;
        PlayerData.instance.hasDreamNail = true;
        GameInfo.get_any_charm_num += 1;
        GiftHelper.AddNailDamage();
        GiftHelper.GiveMask();
        GiftHelper.GiveVessel();
        GiftHelper.AddCharmSlot(8);
        Rogue.Instance.ShowDreamConvo("moth_dream".Localize());
    }

    public override void EndCharacter()
    {
    }
}