namespace rogue.Characters;
internal class Hunter : Character
{
    public Hunter()
    {
        this.Selfname = CharacterRole.hunter;
    }
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.hunter;
        PlayerData.instance.hasDash = true;
        PlayerData.instance.canDash = true;
        PlayerData.instance.hasDoubleJump = true;
        PlayerData.instance.hasWalljump = true;
        PlayerData.instance.canWallJump = true;
        PlayerData.instance.canSuperDash = true;
        PlayerData.instance.hasSuperDash = true;
        PlayerData.instance.gotCharm_14 = true;
        PlayerData.instance.gotCharm_32 = true;
        PlayerData.instance.charmSlots += 1;
        if (PlayerData.instance.charmSlots > 11) PlayerData.instance.charmSlots = 11;
        Rogue.Instance.ShowDreamConvo("hunter_dream".Localize());
    }

    public override void EndCharacter()
    {
    }
}