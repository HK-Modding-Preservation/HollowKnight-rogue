

namespace rogue;

internal class OneMaskTwoBlue : CustomGift
{
    public OneMaskTwoBlue() : base(Giftname.custom_one_mask_two_blue, 4, "witches_eye")
    {
        weight = 0.5f;
        price = 200;
        name = "custom_one_mask_two_blue_name";
        desc = "custom_one_mask_two_blue_desc";
    }
    int prev_blue_health;

    protected override void _GetGift()
    {
        ProcessManager.Instance.after_scene_add_geo_num += RemoveMaskAddBlue;
        On.PlayerData.UpdateBlueHealth += AddTwoBlue;
    }

    private int RemoveMaskAddBlue(int geo, int damage_num)
    {
        prev_blue_health = PlayerData.instance.healthBlue;
        GiftHelper.RemoveMask();
        return geo;
    }

    private void AddTwoBlue(On.PlayerData.orig_UpdateBlueHealth orig, PlayerData self)
    {
        orig(self);
        self.healthBlue += 2;
    }


    protected override void _RemoveGift()
    {
        ProcessManager.Instance.after_scene_add_geo_num -= RemoveMaskAddBlue;
        On.PlayerData.UpdateBlueHealth -= AddTwoBlue;
    }
}