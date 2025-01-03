using rogue.Characters;
namespace rogue;

internal static class GameInfo
{
    internal static bool in_rogue = false;
    internal static CharacterRole role = CharacterRole.no_role;
    internal static int get_any_charm_num = 0;

    internal static int revive_num = 0;

    internal static bool get_birthright = false;
    internal static int pretty_key_num = 0;
    internal static int refresh_num = 0;
    internal static int spa_count = 0;
    internal static float timer = 0;
    internal static List<Giftname> got_items = new();
    internal static Dictionary<GiftVariety, List<Gift>> act_gifts = new()
    {
        {GiftVariety.item,new()},
        {GiftVariety.huge,new()},
        {GiftVariety.role,new()},
        {GiftVariety.shop,new()},
        {GiftVariety.charm,new()},
        {GiftVariety.custom,new()},
    };
    internal static void Reset()
    {
        in_rogue = false;
        role = CharacterRole.no_role;
        get_any_charm_num = 0;
        revive_num = 0;
        get_birthright = false;
        pretty_key_num = 0;
        refresh_num = 0;
        spa_count = 0;
        timer = 0;
        foreach (var gifts in act_gifts)
        {
            gifts.Value.Clear();
        }
    }
    internal static void Start()
    {
        foreach (var gifts in act_gifts)
        {
            foreach (var gift in GiftFactory.all_kind_of_gifts[gifts.Key])
            {
                if (gift.active && gift.force_active)
                {
                    gifts.Value.Add(gift);
                }
            }
        }
        in_rogue = true;
        get_any_charm_num = 0;
        revive_num = 1;
        refresh_num = Rogue.Instance._set.reroll_num;
    }
    internal static void Over()
    {
        in_rogue = false;
        get_birthright = false;
        foreach (var item in got_items)
        {
            GiftFactory.all_gifts[item].RemoveGift();
        }
        got_items.Clear();
        role = CharacterRole.no_role;
    }
}