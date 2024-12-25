
namespace rogue.Characters;
internal class Uunn : Character
{
    public Uunn()
    {
        this.Selfname = CharacterRole.uunn;
    }

    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.uunn;
        PlayerData.instance.charmSlots += 2;
        if (PlayerData.instance.charmSlots > 11) PlayerData.instance.charmSlots = 11;
        PlayerData.instance.gotCharm_7 = true;
        PlayerData.instance.gotCharm_28 = true;
        ItemManager.Instance.after_scene_add_geo_num += UunnReward;
    }

    private int UunnReward(int geo, int damage_num)
    {
        if (damage_num == 0) geo += 50;
        return geo;
    }

    public override void EndCharacter()
    {
        ItemManager.Instance.after_scene_add_geo_num -= UunnReward;
    }
}