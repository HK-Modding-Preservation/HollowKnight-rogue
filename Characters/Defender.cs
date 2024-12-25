namespace rogue.Characters;
internal class Defender : Character
{

    public Defender()
    {
        this.Selfname = CharacterRole.defender;

    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.defender;
        GiftHelper.AddNailDamage();
        PlayerData.instance.quakeLevel = 1;
        PlayerData.instance.gotCharm_10 = true;
        if (!PlayerData.instance.equippedCharms.Contains(10))
        {
            PlayerData.instance.equippedCharms.Insert(1, 10);
            PlayerData.instance.equippedCharm_10 = true;
        }
        CharmHelper.SetCantUnequip(10);
    }

    public override void EndCharacter()
    {
        CharmHelper.SetCanEquip(10);
    }
}