using rogue.Characters;
namespace rogue;

internal static class GameInfo
{
    internal enum GameMode
    {
        NO_MODE,
        MODE0,
        MODE1
    }
    internal static bool in_rogue = false;
    internal static CharacterRole role = CharacterRole.no_role;

    internal static GameMode gameMode = GameMode.MODE0;

    internal static int get_any_charm_num = 0;

    internal static int revive_num = 0;
    internal static StatsData data = null;

    internal static int num_can_get_birthright = 0;
    internal static bool can_get_birthright = true;
    internal static int pretty_key_num = 0;
    internal static int refresh_num = 0;
    internal static int spa_count = 0;
    internal static float score = 0;
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
    internal static class Branch
    {
        internal static bool meet_collector = false;
        internal static bool collector = false;
        internal static bool meet_lost_kin = false;
        internal static bool lost_kin = false;
        internal static bool modboss = false;
        internal static bool radiance = false;
        internal static void Reset()
        {
            meet_collector = false;
            collector = false;
            
            lost_kin = false;
            modboss = false;
            radiance = false;
        }
    }
    internal static void Reset()
    {
        in_rogue = false;
        role = CharacterRole.no_role;
        get_any_charm_num = 0;
        revive_num = 0;
        num_can_get_birthright = 0;
        pretty_key_num = 0;
        refresh_num = 0;
        spa_count = 0;
        score = 0;
        data?.EndCount();
        data = new();
        Branch.Reset();
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
        data.StartCount();
    }
    internal static void Over()
    {
        in_rogue = false;
        foreach (var item in got_items)
        {
            GiftFactory.all_gifts[item].RemoveGift();
        }
        data?.EndCount();
        got_items.Clear();
        role = CharacterRole.no_role;
    }
}