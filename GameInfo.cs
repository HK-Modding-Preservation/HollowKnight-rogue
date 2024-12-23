using rogue.Characters;
namespace rogue;

internal static class GameInfo
{
    internal static bool in_rogue;
    internal static CharacterRole role;
    internal static int get_any_charm_num;

    internal static int revive_num;

    internal static bool get_birthright;
    internal static int pretty_key_num;
    internal static int refresh_num;
    internal static int spa_count;
    internal static float timer;
    internal static Dictionary<GiftVariety, List<Gift>> act_gifts = new()
    {
        {GiftVariety.item,new()},
        {GiftVariety.huge,new()},
        {GiftVariety.role,new()},
        {GiftVariety.shop,new()},
        {GiftVariety.charm,new()},
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
        role = CharacterRole.no_role;
    }
}