

namespace rogue;

internal class OneMaskTwoBlue : CustomGift
{
    public OneMaskTwoBlue() : base(Giftname.custom_one_mask_two_blue, 0, "blue_idle")
    {
        price = 200;
        name = "custom_one_mask_two_blue_name";
        desc = "custom_one_mask_two_blue_desc";
    }

    protected override void _GetGift()
    {
        ProcessManager.Instance.after_scene_add_geo_num += RemoveMaskAddBlue;
    }

    private int RemoveMaskAddBlue(int geo, int damage_num)
    {
        GiftHelper.RemoveMask();
        GiftHelper.AddBlueMask();
        GiftHelper.AddBlueMask();
        return geo;
    }



    protected override void _RemoveGift()
    {
        ProcessManager.Instance.after_scene_add_geo_num -= RemoveMaskAddBlue;
    }
}