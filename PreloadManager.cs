using rogue;
using rogue.ModBosses;

internal static class PreloadManager
{
    internal const string hive_scene = "Hive_03_c";
    internal const string hive_big_bee = "Big Bee";
    internal const string hive_bee_stinger = "Bee Stinger (5)";
    internal const string collector_scene = "GG_Collector";
    internal const string collector_name = "Battle Scene/Jar Collector";

    internal const string mushroom_scene = "Fungus2_29";
    internal const string bow_mushroom_name = "Mushroom Brawler";
    internal const string roller_mushroom_name = "Mushroom Roller (3)";

    internal const string grimm_scene = "Hive_03";
    internal const string grimm_name = "Flamebearer Spawn";
    internal const string hk_scene = "GG_Hollow_Knight";
    internal const string hk_slash_effect_name = "Battle Scene/HK Prime/Slashes";
    internal static List<(string, string)> GetPreloadNames()
    {
        return new()
        {
            (hive_scene,hive_big_bee),
            (hive_scene,hive_bee_stinger),
            (collector_scene,collector_name),
            (mushroom_scene,bow_mushroom_name),
            (mushroom_scene,roller_mushroom_name),
            (grimm_scene,grimm_name),
            (hk_scene,hk_slash_effect_name)

        };
    }

    internal static void Init(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        BossManager.Init(preloadedObjects);
        Uradiance.Init(preloadedObjects);
    }

}