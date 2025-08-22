using System.IO;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace rogue;

[Serializable]
public class Enemy
{
    public string name;
    public int hp;
    public int num;
}
[Serializable]
public class EnemyWave
{
    public List<Enemy> enemies = new List<Enemy>();
}
[Serializable]
public class EnemyWaveCollection
{
    public class OnePossibleWave
    {
        public float weight;
        public EnemyWave enemyWave;
    }
    public Dictionary<int, List<OnePossibleWave>> whole_wave = new();


}

internal static class EnemyWaveManager
{
    internal const string fly_scene = "Crossroads_03";
    internal const string fly_name = "Uninfected Parent/Buzzer";
    internal const string crawler_name = "_Enemies/Crawler";
    internal const string spitter_name = "Spitter";
    internal const string hatcher_scene = "Crossroads_35";
    internal const string hatcher_name = "_Enemies/Hatcher";
    internal const string angry_buzzer_scene = "Crossroads_12";
    internal const string angry_buzzer_name = "Infected Parent/Angry Buzzer";
    internal const string gruzz_scene = "Crossroads_07";
    internal const string gruzz_name = "Uninfected Parent/Fly";
    internal const string burst_gruzz_name = "Infected Parent/Bursting Bouncer";

    internal const string zombie_scene = "Crossroads_40";
    internal const string zombie_name = "Uninfected Parent/Zombie Runner 2";
    internal const string jump_zombie_name = "Uninfected Parent/Zombie Leaper";
    internal const string hornhead_zombie_name = "_Enemies/Zombie Hornhead";
    internal const string zombie_shield_scene = "Crossroads_15";
    internal const string zombie_shield_name = "_Enemies/Zombie Shield";
    internal const string zombie_guard_scene = "Crossroads_21";
    internal const string zombie_barger_name = "Zombie Barger";
    internal const string zombie_guard_name = "non_infected_event/Zombie Guard";
    internal const string shrubb_scene = "Fungus1_01";
    internal const string shrubb_name = "Moss Walker";
    internal const string moss_runner_name = "Mossman_Runner";
    internal const string moss_shaker_name = "Mossman_Shaker";
    internal const string moss_flyer_scene = "Fungus3_13";
    internal const string moss_flyer_name = "Moss Flyer";
    internal const string moss_charger_scene = "Fungus1_17";
    internal const string moss_charger_name = "Moss Charger";
    internal const string moss_fat_scene = "Fungus3_39";
    internal const string moss_fat_name = "Moss Knight Fat";
    internal const string moss_knight_scene = "Fungus1_21";
    internal const string moss_knight_name = "Moss Knight";
    internal const string ruins_fly_scene = "Ruins1_05c";
    internal const string ruins_fly_name = "Ruins Flying Sentry Javelin (2)";
    internal const string ruins_fly2_scene = "Ruins1_03";
    internal const string ruins_fly2_name = "_Scenery/Ruins Flying Sentry";
    internal const string mantis_fly_scene = "Fungus3_05";
    internal const string mantis_fly_name = "Mantis Heavy Flyer";
    internal const string mantis_child_scene = "Fungus2_15";
    internal const string mantis_child_name = "Mantis Flyer Child";
    internal const string Mosquito_scene = "Fungus1_02";
    internal const string Mosquito_name = "Mosquito";
    internal const string beam_miner_scene = "Mines_23";
    internal const string beam_miner_name = "Zombie Beam Miner";
    internal const string crystal_flyer_name = "Crystal Flyer";
    internal const string miner_scene = "Mines_11";
    internal const string miner_name = "Zombie Miner 1";
    internal const string mine_crawler_name = "Mines Crawler";
    internal const string crystal_crawler_name = "Crystal Crawler";

    internal const string colosseum_scene = "Room_Colosseum_Gold";
    internal const string electric_name = "Colosseum Manager/Waves/Wave 47/Electric Mage New";
    internal const string blobble_name = "Colosseum Manager/Waves/Wave 6/Colosseum Cage Small";
    internal const string colosseum_shield_name = "Colosseum Manager/Waves/Wave 3/Colosseum Cage Large";
    internal const string colosseum_miner_name = "Colosseum Manager/Waves/Wave 1/Colosseum Cage Large (1)";
    internal const string colosseum_worm_name = "Colosseum Manager/Waves/Wave 1/Colosseum Cage Large";
    internal const string colosseum_fly_name = "Colosseum Manager/Waves/Wave 4/Colosseum Cage Large";
    internal const string colosseum_hopper_name = "Colosseum Manager/Waves/Wave 10/Colosseum Cage Small (1)";
    internal const string colosseum_roller_name = "Colosseum Manager/Waves/Wave 4/Colosseum Cage Small (1)";
    internal const string colosseum_Mosquito_name = "Colosseum Manager/Waves/Wave 2/Colosseum Cage Small";

    internal const string bombbird_scene = "Waterways_01";
    internal const string bombbird_name = "_Enemies/Ceiling Dropper";
    internal const string flip_hopper_name = "_Enemies/Flip Hopper";
    internal const string inflater_name = "_Enemies/Inflater";
    internal const string fluke_scene = "Waterways_02";
    internal const string fluke_name = "Flukeman";
    internal const string fluke_child_name = "Fluke Fly";
    internal const string fat_fluke_scene = "GG_Pipeway";
    internal const string fat_fluke_name = "Fat Fluke";


    internal const string mage_scene = "Ruins1_23";
    internal const string mage_blob_name = "Mage Blob 1";
    internal const string mage_name = "Mage";
    internal const string mage_balloon_scene = "Ruins1_32";
    internal const string mage_balloon_name = "Mage Balloon";

    internal const string mawlek_scene = "Abyss_17";
    internal const string mawlek_name = "Lesser Mawlek";
    internal const string mawlek_turret_scene = "Abyss_20";
    internal const string mawlek_turret_name = "Mawlek Turret";


    internal const string sentry_scene = "Ruins2_05";
    internal const string sentry1_name = "Ruins Sentry 1";
    internal const string sentry2_name = "Ruins Sentry Fat";

    internal const string obo_scene = "Fungus1_31";
    internal const string obo_name = "_Enemies/Fat Fly (1)";

    internal const string hopper_scene = "Fungus1_23";
    internal const string hopper_name = "Grass Hopper";

    internal const string balloon_scene = "Abyss_19";
    internal const string balloon_name = "Parasite Balloon (1)";

    internal const string centipode_hatcher_scene = "Deepnest_26";
    internal const string centipode_hatcher_name = "Centipede Hatcher";

    internal const string zombie_spider_scene = "Deepnest_03";
    internal const string zombie_spider_name = "Zombie Hornhead Sp (4)/Zombie Spider 2(Clone)";

    internal const string slash_spider_scene = "Deepnest_39";
    internal const string slash_spider_name = "Slash Spider";

    internal const string fungus_flyer_scene = "Fungus2_03";
    internal const string fungus_flyer_name = "Fungus Flyer";

    internal const string great_shield_zombie_scene = "Ruins2_04";
    internal const string great_shield_zombie_name = "Great Shield Zombie";

    internal const string jellyfish_scene = "Fungus3_02";
    internal const string jellyfish_name = "Jellyfish";
    internal const string east_hopper_scene = "Deepnest_East_06";
    internal const string east_hopper_name = "Hopper";
    internal const string super_spitter_name = "Super Spitter";
    internal const string giant_hopper_name = "Giant Hopper (1)";

    internal const string mantis_heavy_scene = "Fungus3_39";
    internal const string mantis_heavy_name = "Battle Scene/Mantis Heavy";

    internal const string garden_zombie_scene = "Fungus3_22";
    internal const string garden_zombie_name = "Garden Zombie";
    internal const string grave_zombie_scene = "RestingGrounds_10";
    internal const string grave_zombie_name = "Grave Zombie";

    internal const string royal_guard_scene = "White_Palace_02";
    internal const string royal_guard_name = "Battle Scene/Royal Gaurd";


    internal const string flame_small_scene = "Fungus1_10";
    internal const string flame_small_name = "Flamebearer Spawn";

    internal const string flame_med_scene = "Tutorial_01";
    internal const string flame_med_name = "Flamebearer Spawn";
    internal const string centipode_name = "Centipede Cage/Baby Centipede Spawner";















    internal static List<(string, string)> GetPreloadNames()
    {
        return new()
        {
            (fly_scene, fly_name),
            (fly_scene, crawler_name),
            (fly_scene, spitter_name),
            (hatcher_scene, hatcher_name),
            (angry_buzzer_scene, angry_buzzer_name),
            (gruzz_scene, gruzz_name),
            (gruzz_scene, burst_gruzz_name),
            (zombie_scene, zombie_name),
            (zombie_scene, jump_zombie_name),
            (zombie_scene, hornhead_zombie_name),
            (zombie_shield_scene, zombie_shield_name),
            (zombie_guard_scene, zombie_guard_name),
            (zombie_guard_scene, zombie_barger_name),
            (shrubb_scene,shrubb_name),
            (shrubb_scene,moss_runner_name),
            (shrubb_scene,moss_shaker_name),
            (moss_flyer_scene,moss_flyer_name),
            (moss_charger_scene,moss_charger_name),
            (moss_fat_scene,moss_fat_name),
            (moss_knight_scene,moss_knight_name),
            (ruins_fly_scene,ruins_fly_name),
            (ruins_fly2_scene,ruins_fly2_name),
            (mantis_fly_scene,mantis_fly_name),
            (mantis_child_scene,mantis_child_name),
            (Mosquito_scene,Mosquito_name),
            (beam_miner_scene,beam_miner_name),
            (beam_miner_scene,crystal_flyer_name),
            (miner_scene,miner_name),
            (miner_scene,mine_crawler_name),
            (miner_scene,crystal_crawler_name),
            (colosseum_scene,electric_name),
            (colosseum_scene,blobble_name),
            (colosseum_scene,colosseum_shield_name),
            (colosseum_scene,colosseum_hopper_name),
            (colosseum_scene,colosseum_miner_name),
            (colosseum_scene,colosseum_worm_name),
            (colosseum_scene,colosseum_fly_name),
            (colosseum_scene,colosseum_roller_name),
            (colosseum_scene,colosseum_Mosquito_name),
            (bombbird_scene,bombbird_name),
            (bombbird_scene,flip_hopper_name),
            (bombbird_scene,inflater_name),
            (fluke_scene,fluke_name),
            (fluke_scene,fluke_child_name),
            (fat_fluke_scene,fat_fluke_name),
            (mage_scene,mage_name),
            (mage_scene,mage_blob_name),
            (mage_balloon_scene,mage_balloon_name),
            (mawlek_scene, mawlek_name),
            (mawlek_turret_scene, mawlek_turret_name),
            (sentry_scene, sentry1_name),
            (sentry_scene, sentry2_name),
            (obo_scene, obo_name),
            (hopper_scene, hopper_name),
            (balloon_scene, balloon_name),
            (centipode_hatcher_scene, centipode_hatcher_name),
            (centipode_hatcher_scene,centipode_name),
            // (zombie_spider_scene, zombie_spider_name),
            (slash_spider_scene, slash_spider_name),
            (PreloadManager.hive_scene, PreloadManager.hive_big_bee),
            (PreloadManager.hive_scene, PreloadManager.hive_bee_stinger),
            (PreloadManager.mushroom_scene, PreloadManager.bow_mushroom_name),
            (PreloadManager.mushroom_scene, PreloadManager.roller_mushroom_name),
            (fungus_flyer_scene,fungus_flyer_name),
            (great_shield_zombie_scene,great_shield_zombie_name),
            (jellyfish_scene,jellyfish_name),
            (east_hopper_scene,east_hopper_name),
            (east_hopper_scene,super_spitter_name),
            (east_hopper_scene,giant_hopper_name),
            (mantis_heavy_scene,mantis_heavy_name),
            (garden_zombie_scene,garden_zombie_name),
            (grave_zombie_scene,grave_zombie_name),
            (royal_guard_scene,royal_guard_name),
            (PreloadManager.grimm_scene,PreloadManager.grimm_name),
            (flame_small_scene,flame_small_name),
            (flame_med_scene,flame_med_name),



        };
    }
    internal static bool ReloadWaveCollection()
    {
        var direc = AssemblyUtils.getCurrentDirectory();
        bool error = false;
        string res = "";
        try
        {
            res = File.ReadAllText(Path.Combine(direc, "EnemyWave.json"));
        }
        catch (Exception)
        {
            error = true;
            Log("Read ERROR");
        }
        if (!error)
        {
            wave_collections = JsonConvert.DeserializeObject<Dictionary<string, EnemyWaveCollection>>(res);
            Log("Read OK");
        }
        return error;
    }
    internal static void Init()
    {
        collector_collections = JsonConvert.DeserializeObject<Dictionary<string, EnemyWaveCollection>>(System.Text.Encoding.UTF8.GetString(
            AssemblyUtils.GetBytesFromResources("EnemyWaves.Collector_EnemyWave.json")));
        lost_kin_collections = JsonConvert.DeserializeObject<Dictionary<string, EnemyWaveCollection>>(System.Text.Encoding.UTF8.GetString(
            AssemblyUtils.GetBytesFromResources("EnemyWaves.LostKin_EnemyWave.json")));
        var bytes = AssemblyUtils.GetBytesFromResources("EnemyWaves.EnemyWave.json");
        if (ReloadWaveCollection())
            wave_collections = JsonConvert.DeserializeObject<Dictionary<string, EnemyWaveCollection>>(System.Text.Encoding.UTF8.GetString(bytes));


        // wave_collections = new();
        // Enemy enemy1 = new() { name = "name1", hp = 50, num = 3 };
        // Enemy enemy2 = new() { name = "name2", hp = 80, num = 2 };
        // EnemyWave enemyWave1 = new();
        // enemyWave1.enemies.Add(enemy1);
        // enemyWave1.enemies.Add(enemy2);
        // EnemyWaveCollection.OnePossibleWave possibleWave1 = new();
        // possibleWave1.enemyWave = enemyWave1;
        // possibleWave1.weight = 2;
        // EnemyWaveCollection.OnePossibleWave possibleWave2 = new();
        // possibleWave2.enemyWave = enemyWave1;
        // possibleWave2.weight = 3;
        // List<EnemyWaveCollection.OnePossibleWave> possibleWaves = new();
        // possibleWaves.Add(possibleWave1);
        // possibleWaves.Add(possibleWave2);
        // List<EnemyWaveCollection.OnePossibleWave> possibleWaves2 = new();
        // possibleWaves2.Add(possibleWave2);
        // possibleWaves2.Add(possibleWave1);
        // EnemyWaveCollection waveCollection1 = new();
        // waveCollection1.whole_wave.Add(1, possibleWaves);
        // waveCollection1.whole_wave.Add(2, possibleWaves2);
        // wave_collections.Add("GG_Vengefly_V", waveCollection1);
        // string res = JsonConvert.SerializeObject(wave_collections);
        // File.WriteAllText("c:\\Users\\shownyoung\\Desktop\\EnemyWave.json", res);
        CheckPreloadEnemy();
    }
    private static readonly List<string> modboss_scenes = new(){

    "GG_Soul_Tyrant",
    "GG_Ghost_Markoth_V",
    "GG_Grey_Prince_Zote",
    "GG_Failed_Champion",
    "GG_Grimm_Nightmare"

    };
    internal static bool SceneInBranch(string scene_name)
    {
        if (GameInfo.Branch.collector && collector_collections.ContainsKey(scene_name)) return true;
        if (GameInfo.Branch.lost_kin && lost_kin_collections.ContainsKey(scene_name)) return true;

        return false;
    }
    internal static bool DontReplacyScene(string scene_name)
    {
        if (GameInfo.Branch.modboss && modboss_scenes.Contains(scene_name)) return true;
        return false;

    }
    internal static void CheckPreloadEnemy()
    {
        foreach (var names in GetPreloadNames())
        {
            string true_name = names.Item2.Split('/').Last();
            if (!enemy_prefabs.ContainsKey(true_name))
            {
                Log(true_name);
                GameObject go = PreloadManager.getGO(names.Item1, names.Item2);
                if (true_name.Contains("Large"))
                {
                    var true_go = go.LocateMyFSM("Spawn").FsmVariables.FindFsmGameObject("Corpse to Instantiate").Value;
                    var instance = GameObject.Instantiate(true_go);
                    GameObject.DontDestroyOnLoad(instance);
                    instance.SetActive(false);
                    enemy_prefabs.Add(true_go.name, instance);
                }
                else if (true_name.Contains("Small"))
                {
                    Log(go);
                    var true_go = go
                        .LocateMyFSM("Spawn")
                        .FsmVariables
                        .FindFsmGameObject("Enemy Type")
                        .Value;
                    var instance = GameObject.Instantiate(true_go);
                    GameObject.DontDestroyOnLoad(instance);
                    instance.SetActive(false);
                    enemy_prefabs.Add(true_go.name, instance);
                }
                else if (true_name == "Flamebearer Spawn")
                {
                    var true_go = go.LocateMyFSM("Spawn Control").FsmVariables.FindFsmGameObject("Grimmkin Obj").Value;
                    var instance = GameObject.Instantiate(true_go);
                    GameObject.DontDestroyOnLoad(instance);
                    instance.SetActive(false);
                    if (names.Item1 == flame_small_scene)
                    {
                        enemy_prefabs.Add("Flamebearer Small", instance);
                    }
                    else if (names.Item1 == flame_med_scene)
                    {
                        enemy_prefabs.Add("Flamebearer Med", instance);
                    }
                    else if (names.Item1 == PreloadManager.grimm_scene)
                    {
                        enemy_prefabs.Add("Flamebearer Large", instance);
                    }

                }
                else
                {
                    enemy_prefabs.Add(true_name, PreloadManager.getGO(names.Item1, names.Item2));
                }
            }

        }
    }

    static Dictionary<string, EnemyWaveCollection> wave_collections = new Dictionary<string, EnemyWaveCollection>();
    static Dictionary<string, EnemyWaveCollection> collector_collections = new Dictionary<string, EnemyWaveCollection>();
    static Dictionary<string, EnemyWaveCollection> lost_kin_collections = new Dictionary<string, EnemyWaveCollection>();
    internal static Dictionary<string, GameObject> enemy_prefabs = new Dictionary<string, GameObject>();
    internal static GameObject GetEnemy(string name)
    {
        GameObject res = null;
        if (name == "Ruins Flying Sentry Javelin") name = ruins_fly_name;
        if (name == "Zombie Runner") name = "Zombie Runner 2";
        if (name == "Mage Blob") name = mage_blob_name;
        if (name == "Ruins Sentry") name = sentry1_name;
        if (name == "Fat Fly") name = "Fat Fly (1)";
        if (name == "Parasite Balloon") name = balloon_name;
        if (name == "Giant Hopper") name = giant_hopper_name;
        if (name == "Bee Stinger") name = PreloadManager.hive_bee_stinger;
        if (name == "Mushroom Roller") name = PreloadManager.roller_mushroom_name;
        if (name == "Zombie Miner") name = miner_name;

        if (enemy_prefabs.ContainsKey(name))
        {
            res = GameObject.Instantiate(enemy_prefabs[name]);
            AdjustEnemy(res, name);
            return res;
        }
        else return null;
    }
    internal static void AdjustEnemy(GameObject enemy, string name)
    {
        Log("adjust " + name);
        PlayMakerFSM temp_fsm;

        switch (name)
        {

            case mage_name:
                var fsm = enemy.LocateMyFSM("Mage");
                fsm.RemoveAction("Select Target", 1);
                fsm.RemoveAction("Select Target", 1);
                fsm.InsertCustomAction("Select Target", (fsm) =>
                {
                    Vector2 ZeroPoint()
                    {
                        return fsm.FsmVariables.FindFsmGameObject("Zero Point").Value.transform.position;
                    }
                    bool in_arena = BossSceneManager.arena_info.ContainsKey(ProcessManager.scene_name);
                    BossSceneManager.ArenaInfo cur_info = in_arena ? BossSceneManager.arena_info[ProcessManager.scene_name] : null;

                    fsm.FsmVariables.FindFsmFloat("X Min").Value = in_arena ? cur_info.left : ZeroPoint().x - 5f;
                    fsm.FsmVariables.FindFsmFloat("X Max").Value = in_arena ? cur_info.right : ZeroPoint().x + 5f;
                    fsm.FsmVariables.FindFsmFloat("Y Min").Value = in_arena ? cur_info.down : ZeroPoint().y - 5f;
                    fsm.FsmVariables.FindFsmFloat("Y Max").Value = in_arena ? cur_info.up : ZeroPoint().y + 5f;
                }, 1);
                break;
            case "Electric Mage New":
                temp_fsm = enemy.LocateMyFSM("Electric Mage");
                temp_fsm.RemoveAction("Select Target", 1);
                temp_fsm.RemoveAction("Select Target", 1);
                temp_fsm.InsertCustomAction("Select Target", (fsm) =>
                {
                    Vector2 ZeroPoint()
                    {
                        return fsm.FsmVariables.FindFsmGameObject("Zero Point").Value.transform.position;
                    }
                    bool in_arena = BossSceneManager.arena_info.ContainsKey(ProcessManager.scene_name);
                    BossSceneManager.ArenaInfo cur_info = in_arena ? BossSceneManager.arena_info[ProcessManager.scene_name] : null;

                    fsm.FsmVariables.FindFsmFloat("X Min").Value = in_arena ? cur_info.left : ZeroPoint().x - 5f;
                    fsm.FsmVariables.FindFsmFloat("X Max").Value = in_arena ? cur_info.right : ZeroPoint().x + 5f;
                    fsm.FsmVariables.FindFsmFloat("Y Min").Value = in_arena ? cur_info.down : ZeroPoint().y - 5f;
                    fsm.FsmVariables.FindFsmFloat("Y Max").Value = in_arena ? cur_info.up : ZeroPoint().y + 5f;
                }, 1);
                break;

            default:
                break;
        }
    }
    internal static EnemyWaveCollection GetCollection(string scene_name)
    {
        if (GameInfo.Branch.lost_kin && lost_kin_collections.ContainsKey(scene_name))
        {
            return lost_kin_collections[scene_name];
        }
        if (GameInfo.Branch.collector && collector_collections.ContainsKey(scene_name))
        {
            return collector_collections[scene_name];
        }
        if (wave_collections.ContainsKey(scene_name))
        {
            return wave_collections[scene_name];
        }
        else
        {
            return null;
        }

    }
    static void Log(object msg)
    {
        Rogue.Instance.Log(msg);
    }
}