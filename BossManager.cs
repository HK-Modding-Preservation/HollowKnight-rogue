using System.Data;
using System.Diagnostics;
using Steamworks;

namespace rogue;

internal static class BossManager
{
    static GameObject big_bee;
    static GameObject stringer_bee;
    static GameObject jar;

    static GameObject roller_mushroom;
    static GameObject bow_mushroom;


    static GameObject grimmbear;


    static bool can_self_over;

    internal static int bossleft;
    internal static int waveleft;

    static List<GameObject> spawn_gos = new();
    static List<GameObject> spawn_health_gos = new();

    internal static void Init(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        GameObject EnemyInit(string scene, string name, bool on_death = false, bool add_spawn = true)
        {
            var res = GameObject.Instantiate(preloadedObjects[scene][name]);
            res.SetActive(false);
            if (on_death)
            {
                try
                {
                    res.GetComponent<HealthManager>().OnDeath += DeathCount;
                }
                catch (Exception)
                {
                    Rogue.Instance.Log("No HealthManager");
                }
            }
            GameObject.DontDestroyOnLoad(res);
            if (add_spawn)
            {
                spawn_gos.Add(res);
                if (on_death)
                {
                    spawn_health_gos.Add(res);
                }
            }
            return res;

        }
        big_bee = EnemyInit(PreloadManager.hive_scene, PreloadManager.hive_big_bee, true);
        stringer_bee = EnemyInit(PreloadManager.hive_scene, PreloadManager.hive_bee_stinger, true);
        roller_mushroom = EnemyInit(PreloadManager.mushroom_scene, PreloadManager.roller_mushroom_name, true);
        bow_mushroom = EnemyInit(PreloadManager.mushroom_scene, PreloadManager.bow_mushroom_name, true);

        grimmbear = GameObject.Instantiate(preloadedObjects[PreloadManager.grimm_scene][PreloadManager.grimm_name].
                                        LocateMyFSM("Spawn Control").
                                        FsmVariables.FindFsmGameObject("Grimmkin Obj").Value);
        grimmbear.SetActive(false);
        GameObject.DontDestroyOnLoad(grimmbear);
        spawn_gos.Add(grimmbear);
        spawn_health_gos.Add(grimmbear);


        jar = GameObject.Instantiate(preloadedObjects[PreloadManager.collector_scene][PreloadManager.collector_name].
        LocateMyFSM("Control").
        GetAction<SpawnObjectFromGlobalPool>("Spawn", 3).gameObject.Value);
        jar.SetActive(false);
        GameObject.DontDestroyOnLoad(jar);
        spawn_gos.Add(jar);



        On.PlayMakerFSM.OnEnable += ModifyBoss;
        ModHooks.LanguageGetHook += ModifyBossName;
    }

    private static string ModifyBossName(string key, string sheetTitle, string orig)
    {
        if (GameInfo.Branch.radiance || (Rogue.Instance._set.modboss_in_workshop && !BossSequenceController.IsInSequence))
        {
            if (key == "ABSOLUTE_RADIANCE_MAIN") return "uradiance".Localize();
        }
        if (GameInfo.Branch.modboss || (Rogue.Instance._set.modboss_in_workshop && !BossSequenceController.IsInSequence))
        {
            if (key == "NAME_SOUL_TYRANT" || key == "MAGE_LORD_DREAM_MAIN") return "void_tyrant".Localize();
            if (key == "NAME_GHOST_MARKOTH" || key == "GH_MARKOTH_C_MAIN" || key == "GH_MARKOTH_NC_MAIN") return "markoth_infinity".Localize();
            if (key == "GREY_PRINCE_SUPER") return "grey_king".Localize();
            if (key == "NAME_GREY_PRINCE") return "grey_king_zote".Localize();
            if (key == "FALSE_KNIGHT_MAIN" || key == "NAME_FALSEKNIGHT") return "champions_of_battle".Localize();
            if (key == "NAME_FAILED_CHAMPION" || key == "FALSE_KNIGHT_DREAM_MAIN") return "champions_of_battle".Localize();
        }
        if ((GameInfo.Branch.collector && GameInfo.Branch.meet_collector) || (Rogue.Instance._set.modboss_in_workshop && !BossSequenceController.IsInSequence))
        {
            if (key == "NAME_JAR_COLLECTOR" || key == "COLLECTOR_MAIN") return "monkey_mount".Localize();
            if (key == "COLLECTOR_SUB") return "a_celestial_abode".Localize();
            if (key == "COLLECTOR_SUPER") return "a_blessed_land".Localize();
        }
        if ((GameInfo.Branch.lost_kin && GameInfo.Branch.meet_lost_kin) || (Rogue.Instance._set.modboss_in_workshop && !BossSequenceController.IsInSequence))
        {
            if (key == "INFECTED_KNIGHT_DREAM_MAIN" || key == "NAME_LOST_KIN") return "chronos_desiderium".Localize();
        }
        return orig;
    }

    private static void ModifyBoss(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (GameInfo.Branch.radiance || (Rogue.Instance._set.modboss_in_workshop && !BossSequenceController.IsInSequence))
        {


            if (self.gameObject.name == "Absolute Radiance" && self.FsmName == "Control")
            {
                self.gameObject.AddComponent<ModBosses.Uradiance>();
            }
        }


        if (GameInfo.Branch.modboss || (Rogue.Instance._set.modboss_in_workshop && !BossSequenceController.IsInSequence))
        {

            if (self.gameObject.name == "Dream Mage Lord" && self.FsmName == "Mage Lord")
            {
                self.gameObject.AddComponent<ModBosses.VoidTyrant>();
            }
            if (self.gameObject.name == "Dream Mage Lord Phase2" && self.FsmName == "Mage Lord 2")
            {
                self.gameObject.AddComponent<ModBosses.VoidTyrantPhase2>();
            }
            if (self.gameObject.name == "Ghost Warrior Markoth" && self.FsmName == "Movement")
            {
                self.gameObject.AddComponent<ModBosses.MarkothInf>();
            }
            if (self.gameObject.name == "False Knight Dream" && self.FsmName == "FalseyControl")
            {
                self.gameObject.GetAddComponent<ModBosses.BattleChampions>();
            }
            if (self.gameObject.name == "Grey Prince" && self.FsmName == "Control")
            {
                self.gameObject.GetAddComponent<ModBosses.GreyKingZote>();
            }
        }

        if ((GameInfo.Branch.collector && GameInfo.Branch.meet_collector) || (Rogue.Instance._set.modboss_in_workshop && !BossSequenceController.IsInSequence))
        {

            if (self.gameObject.name == "Jar Collector" && self.FsmName == "Control")
            {
                self.gameObject.GetAddComponent<ModBosses.BlackMonkey>();
            }
        }


        if ((GameInfo.Branch.lost_kin && GameInfo.Branch.meet_lost_kin) || (Rogue.Instance._set.modboss_in_workshop && !BossSequenceController.IsInSequence))
        {
            if (self.gameObject.name == "Lost Kin" && self.FsmName == "IK Control")
            {
                self.gameObject.GetAddComponent<ModBosses.ChronosDesideria>();
            }
        }

        orig(self);
    }
    internal static bool BranchBoss()
    {
        if (ProcessManager.scene_name == "GG_Collector_V")
        {
            if (GameInfo.Branch.collector)
            {
                if (GameInfo.Branch.meet_collector)
                {
                    return true;

                }
                else
                {
                    GameInfo.Branch.meet_collector = true;
                }
            }
        }
        if (ProcessManager.scene_name == "GG_Lost_Kin")
        {
            if (GameInfo.Branch.lost_kin)
            {
                if (GameInfo.Branch.meet_lost_kin)
                {
                    return true;
                }
                else
                {
                    GameInfo.Branch.meet_lost_kin = true;
                }
            }
        }
        return false;
    }
    internal static IEnumerator AdjustBossHP(string scenename)
    {
        switch (scenename)
        {
            case "GG_Vengefly_V":
                GameObject.Find("Giant Buzzer Col (1)").GetComponent<HealthManager>().hp = 250;
                GameObject.Find("Giant Buzzer Col").GetComponent<HealthManager>().hp = 100;
                break;
            case "GG_Gruz_Mother_V":
                yield return new WaitForSeconds(0.2f);
                GameObject.Find("_Enemies").
                FindGameObjectInChildren("Giant Fly").
                GetComponent<HealthManager>().hp = 350;
                break;
            case "GG_False_Knight":
                yield return new WaitForSeconds(0.2f);
                GameObject.Find("Battle Scene").
                FindGameObjectInChildren("False Knight New")
                .GetComponent<HealthManager>().hp = 160;
                break;
            case "GG_Mega_Moss_Charger":
                GameObject.Find("Mega Moss Charger").GetComponent<HealthManager>().hp = 360;
                break;
            case "GG_Hornet_1":
                GameObject.Find("Boss Holder")
                .FindGameObjectInChildren("Hornet Boss 1")
                .GetComponent<HealthManager>().hp = 500;
                break;
            case "GG_Ghost_Gorb_V":
                GameObject.Find("Warrior")
                .FindGameObjectInChildren("Ghost Warrior Slug")
                .GetComponent<HealthManager>().hp = 550;
                break;
            case "GG_Dung_Defender":
                GameObject.Find("Dung Defender").GetComponent<HealthManager>().hp = 650;
                break;
            case "GG_Mage_Knight_V":
                GameObject.Find("Mage Knight").GetComponent<HealthManager>().hp = 600;
                break;
            case "GG_Brooding_Mawlek_V":
                GameObject.Find("Battle Scene")
                .FindGameObjectInChildren("Mawlek Body")
                .GetComponent<HealthManager>().hp = 600;
                break;
            case "GG_Nailmasters":
                GameObject.Find("Brothers")
                .FindGameObjectInChildren("Oro")
                .GetComponent<HealthManager>().hp = 400;
                GameObject.Find("Brothers")
                .FindGameObjectInChildren("Oro")
                .LocateMyFSM("nailmaster").FsmVariables.FindFsmInt("P2 HP").Value = 500;
                GameObject.Find("Brothers")
                .FindGameObjectInChildren("Mato")
                .GetComponent<HealthManager>().hp = 800;
                break;
            default:
                break;
        }
        yield break;
    }
    internal static IEnumerator Replacify(string scene_name)
    {
        if (EnemyWaveManager.GetCollection(scene_name) != null)
        {
            if (!BranchBoss())
            {
                RogueSceneManager.AddRoof(scene_name);
                yield return ItemManager.Instance.StartCoroutine(DisableBoss(scene_name));
                yield return ItemManager.Instance.StartCoroutine(EnemyWaves());
            }
            else
            {

            }
        }
        if (scene_name == "GG_Hollow_Knight")
        {
            yield return ItemManager.Instance.StartCoroutine(Modify(scene_name));
        }
    }
    internal static IEnumerator LoveKeyify(string sceneName)
    {
        On.ObjectPool.Spawn_GameObject_Transform_Vector3_Quaternion += SetActive;
        switch (sceneName)
        {
            case "GG_Hive_Knight":
                yield return null;
                yield return ItemManager.Instance.StartCoroutine(LoveKeyifyHiveKnight());
                break;
            case "GG_Ghost_Hu":
                yield return null;
                yield return ItemManager.Instance.StartCoroutine(LoveKeyifyHu());
                break;
            case "GG_Grimm":
                yield return null;
                yield return ItemManager.Instance.StartCoroutine(LoveKeyifyGrimm());
                break;
            default:
                break;
        }
        On.ObjectPool.Spawn_GameObject_Transform_Vector3_Quaternion -= SetActive;
    }


    private static IEnumerator LoveKeyifyGrimm()
    {
        can_self_over = true;
        bossleft = 3;
        GameObject.Find("Grimm Scene").LocateMyFSM("FSM").GetAction<ActivateGameObject>("Battle Start", 8).activate = false;
        float breakY = HeroController.instance.transform.position.y;
        for (int i = 0; i < 3; i++)
        {
            var cur_jar = jar.Spawn();
            cur_jar.transform.position = new Vector3(HeroController.instance.transform.position.x, breakY + 8);
            if (ProcessManager.scene_name.Contains("GG_Collector"))
            {
                cur_jar.GetComponent<SpawnJarControl>().breakY = 94.55f;
                cur_jar.GetComponent<SpawnJarControl>().spawnY = 106.52f;
            }
            cur_jar.GetComponent<SpawnJarControl>().breakY = breakY;
            cur_jar.GetComponent<SpawnJarControl>().spawnY = breakY + 8;
            cur_jar.GetComponent<SpawnJarControl>().SetEnemySpawn(grimmbear, 40);
            cur_jar.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        yield break;

    }

    private static IEnumerator LoveKeyifyHu()
    {
        can_self_over = true;
        bossleft = 6;
        GameObject.Find("Warrior").LocateMyFSM("FSM").GetAction<ActivateGameObject>("Enable", 2).activate = false;
        float breakY = HeroController.instance.transform.position.y;
        for (int i = 0; i < 6; i++)
        {
            var cur_jar = jar.Spawn();
            cur_jar.transform.position = new Vector3(HeroController.instance.transform.position.x, breakY + 8);
            if (ProcessManager.scene_name.Contains("GG_Collector"))
            {
                cur_jar.GetComponent<SpawnJarControl>().breakY = 94.55f;
                cur_jar.GetComponent<SpawnJarControl>().spawnY = 106.52f;
            }
            cur_jar.GetComponent<SpawnJarControl>().breakY = breakY;
            cur_jar.GetComponent<SpawnJarControl>().spawnY = breakY + 8;
            cur_jar.GetComponent<SpawnJarControl>().SetEnemySpawn(bow_mushroom, 40);
            cur_jar.SetActive(true);

            yield return new WaitForSeconds(1f);
        }
        yield break;
    }

    private static IEnumerator LoveKeyifyHiveKnight()
    {

        can_self_over = true;
        bossleft = 6;
        foreach (var boss in BossSceneController.Instance.bosses)
        {
            boss.gameObject.SetActive(false);
        }
        float breakY = HeroController.instance.transform.position.y;
        for (int i = 0; i < 6; i++)
        {
            var cur_jar = jar.Spawn();
            cur_jar.transform.position = new Vector3(HeroController.instance.transform.position.x, breakY + 8);
            if (ProcessManager.scene_name.Contains("GG_Collector"))
            {
                cur_jar.GetComponent<SpawnJarControl>().breakY = 94.55f;
                cur_jar.GetComponent<SpawnJarControl>().spawnY = 106.52f;
            }
            cur_jar.GetComponent<SpawnJarControl>().breakY = breakY;
            cur_jar.GetComponent<SpawnJarControl>().spawnY = breakY + 8;
            cur_jar.GetComponent<SpawnJarControl>().SetEnemySpawn(stringer_bee, 40);
            cur_jar.SetActive(true);
            yield return new WaitForSeconds(1f);
        }

    }

    private static GameObject SetActive(On.ObjectPool.orig_Spawn_GameObject_Transform_Vector3_Quaternion orig, GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        var res = orig(prefab, parent, position, rotation);
        if (spawn_gos.Contains(prefab))
        {

            res.SetActive(true);
            res.transform.parent = parent;
            res.transform.position = position;
            res.transform.rotation = rotation;
            if (spawn_health_gos.Contains(prefab))
            {
                res.GetComponent<HealthManager>().OnDeath -= DeathCount;
                res.GetComponent<HealthManager>().OnDeath += DeathCount;
            }
            if (prefab == grimmbear)
            {
                var fsm = res.LocateMyFSM("Control");
                if (fsm.GetState("Set Level").Actions.Length == 1)
                {
                    fsm.InsertCustomAction("Set Level", (fsm) => { fsm.FsmVariables.FindFsmInt("Grimmchild Level").Value = 3; }, 0);
                }
                if (fsm.GetState("Init").Actions.Length == 23)
                {
                    fsm.InsertCustomAction("Init", (fsm) => { fsm.SendEvent("START"); }, 23);
                }
                if (fsm.GetState("Destroy").Actions.Length == 1)
                {
                    fsm.InsertCustomAction("Destroy", DeathCount, 0);
                }
            }
        }
        return res;
    }
    static IEnumerator DisableBoss(string scene_name)
    {
        yield return null;
        if (BossSceneController.Instance != null)
        {
            foreach (var boss in BossSceneController.Instance.bosses)
            {
                boss.gameObject.SetActive(false);
            }
        }
        GameCameras.instance.StopCameraShake();
        switch (scene_name)
        {
            case "GG_Vengefly_V":
                try
                {
                    GameObject.Find("Giant Buzzer Col").SetActive(false);
                    GameObject.Find("Giant Buzzer Col (1)").SetActive(false);
                }
                catch (Exception) { }
                break;
            case "GG_Gruz_Mother_V":
                break;
            case "GG_False_Knight":

                GameObject.Find("Battle Scene").LocateMyFSM("Activate FK").GetAction<ActivateGameObject>("Activate", 0).activate = false;
                GameObject.Find("Battle Scene").FindGameObjectInChildren("CamLock Intro").SetActive(false);
                break;
            case "GG_Mega_Moss_Charger":
                break;
            case "GG_Hornet_1":
                break;
            case "GG_Ghost_Gorb_V":
                GameObject.Find("Warrior").LocateMyFSM("FSM").GetAction<ActivateGameObject>("Enable", 2).activate = false;
                break;
            case "GG_Dung_Defender":
                GameObject.Find("Dung Defender").SetActive(false);
                break;
            case "GG_Mage_Knight_V":
                GameObject.Find("Balloon Spawner").FindGameObjectInChildren("Balloons").SetActive(false);
                break;
            case "GG_Brooding_Mawlek_V":
                var fsm = GameObject.Find("Battle Scene").LocateMyFSM("Activate Boss");
                fsm.GetAction<ActivateGameObject>("Call Mawlek", 1).activate = false;
                fsm.GetAction<ActivateGameObject>("Mawlek Fall", 0).activate = false;
                break;
            case "GG_Nailmasters":
                GameObject.Find("Brothers").FindGameObjectInChildren("Oro").SetActive(false);
                GameObject.Find("Brothers").FindGameObjectInChildren("Mato").SetActive(false);
                GameObject.Find("Brothers").FindGameObjectInChildren("CamLock Intro").SetActive(false);
                break;
            case "GG_Ghost_Xero_V":
                GameObject.Find("Warrior").LocateMyFSM("FSM").GetAction<ActivateGameObject>("Enable", 2).activate = false;
                break;
            case "GG_Crystal_Guardian":
                break;
            case "GG_Soul_Master":
                GameObject.Find("Mage Lord").SetActive(false);
                GameObject.Find("Mage Lord Phase2").SetActive(false);
                break;
            case "GG_Oblobbles":
                break;
            case "GG_Mantis_Lords_V":
                GameObject.Find("Mantis Battle").SetActive(false);
                break;
            case "GG_Ghost_Marmu_V":
                GameObject.Find("Warrior").LocateMyFSM("FSM").GetAction<ActivateGameObject>("Enable", 2).activate = false;

                break;
            case "GG_Flukemarm":
                GameObject.Find("CameraLockArea Intro").SetActive(false);
                break;
            case "GG_Broken_Vessel":
                GameObject.Find("Infected Knight").SetActive(false);
                break;
            case "GG_Ghost_Galien":
                GameObject.Find("Warrior").SetActive(false);
                break;
            case "GG_Painter":
                GameObject.Find("Battle Scene").FindGameObjectInChildren("CamLock Intro").SetActive(false);
                break;
            case "GG_Hive_Knight":
                break;
            case "GG_Ghost_Hu":
                GameObject.Find("Warrior").LocateMyFSM("FSM").GetAction<ActivateGameObject>("Enable", 2).activate = false;
                break;
            case "GG_Collector_V":
                GameObject.Find("Battle Scene").LocateMyFSM("Control").GetAction<ActivateGameObject>("Start", 0).activate = false;
                break;
            case "GG_God_Tamer":
                GameObject.Find("Entry Object").SetActive(false);
                break;
            case "GG_Grimm":
                GameObject.Find("Grimm Scene").LocateMyFSM("FSM").GetAction<ActivateGameObject>("Battle Start", 8).activate = false;
                break;
            case "GG_Watcher_Knights":
                GameObject.Find("Battle Control").SetActive(false);
                break;
            case "GG_Uumuu_V":
                GameObject.Find("Mega Jellyfish GG").SetActive(false);
                GameObject.Find("CameraLockArea Intro").SetActive(false);
                break;
            case "GG_Nosk_Hornet":
                GameObject.Find("Battle Scene").LocateMyFSM("Battle Control").enabled = false;
                break;
            case "GG_Sly":
                GameObject.Find("Battle Scene").FindGameObjectInChildren("Sly Boss").SetActive(false);
                break;
            case "GG_Hornet_2":

                break;
            case "GG_Crystal_Guardian_2":
                break;
            case "GG_Lost_Kin":
                break;
            case "GG_Ghost_No_Eyes_V":
                GameObject.Find("Warrior").LocateMyFSM("FSM").GetAction<ActivateGameObject>("Enable", 2).activate = false;
                break;
            case "GG_Traitor_Lord":
                GameObject.Find("Battle Scene").FindGameObjectInChildren("Wave 3").SetActive(false);
                GameObject.Find("Battle Scene").FindGameObjectInChildren("CameraLockArea Intro").SetActive(false);
                GameObject.Find("Battle Scene").LocateMyFSM("Battle Control").GetAction<ActivateAllChildren>("Wave 3", 0).activate = false;
                break;
            case "GG_White_Defender":

                break;
            case "GG_Soul_Tyrant":
                GameObject.Find("Dream Mage Lord").SetActive(false);
                GameObject.Find("Dream Mage Lord Phase2").SetActive(false);
                break;
            case "GG_Ghost_Markoth_V":
                GameObject.Find("Warrior").LocateMyFSM("FSM").GetAction<ActivateGameObject>("Enable", 2).activate = false;

                break;
            case "GG_Grey_Prince_Zote":
                break;
            case "GG_Failed_Champion":
                GameObject.Find("Battle Scene").FindGameObjectInChildren("CamLock Intro").SetActive(false);
                break;
            case "GG_Grimm_Nightmare":
                GameObject.Find("Grimm Control").SetActive(false);
                break;
            case "GG_Hollow_Knight":
                GameObject.Find("Battle Scene").FindGameObjectInChildren("HK Prime").SetActive(false);
                break;
            case "GG_Radiance":
                break;
            default:
                break;

        }
        yield break;
    }




    static void DeathCount()
    {
        bossleft--;
        ("enemy left " + bossleft).TestLog();
        if (bossleft == 0)
        {
            waveleft--;
            if (waveleft <= 0)
            {
                BossSceneController.Instance.EndBossScene();
            }

        }

    }

    internal static IEnumerator Modify(string sceneName)
    {
        switch (sceneName)
        {
            case "GG_Hollow_Knight":
                yield return null;
                yield return null;
                FsmVariables.GlobalVariables.FindFsmGameObject("HUD Canvas").Value.LocateMyFSM("Slide Out").SendEvent("IN");
                ModifyTHK();
                break;


            default:
                break;
        }
    }
    internal static IEnumerator EnemyWaves()
    {
        string current_scene = ProcessManager.scene_name;
        EnemyWaveCollection collection = EnemyWaveManager.GetCollection(ProcessManager.scene_name);
        if (collection == null)
        {
            ("No EnemyWaveCollection for " + ProcessManager.scene_name).TestLog();
            yield break;
        }
        waveleft = collection.whole_wave.Keys.Count;
        int max = collection.whole_wave.Keys.Max();
        for (int i = collection.whole_wave.Keys.Min(); i <= collection.whole_wave.Keys.Max(); i++)
        {
            if (current_scene != ProcessManager.scene_name)
            {
                ("Scene Over").TestLog();
                yield break;
            }
            if (!collection.whole_wave.Keys.Contains(i)) continue;
            ("wave " + i).TestLog();
            var wave = collection.whole_wave[i];
            if (wave.Count == 0) continue;
            EnemyWaveCollection.OnePossibleWave possibleWave = wave[0];
            yield return new WaitForSeconds(2f);
            float s = wave.Sum((x) => x.weight);
            ("whole weight s:" + s).TestLog();
            float r = UnityEngine.Random.Range(0, s);
            ("random weight r:" + r).TestLog();
            foreach (var x in wave)
            {
                r -= x.weight;
                ("del " + x.weight + " now is " + r).TestLog();
                if (r <= 0)
                {
                    possibleWave = x;
                    break;
                }

            }
            bossleft = possibleWave.enemyWave.enemies.Sum((x) => x.num);
            foreach (var enemy in possibleWave.enemyWave.enemies)
            {
                int j = 0;
                while (j < enemy.num)
                {
                    j++;

                    yield return new WaitForSeconds(0.5f);

                    var enemy_go = EnemyWaveManager.GetEnemy(enemy.name);
                    int random_limit = 0;
                    if (enemy_go != null)
                    {

                        Vector2 pos;
                        Vector2 now_pos = HeroController.instance.transform.position;
                        do
                        {
                            random_limit++;
                            float angle = UnityEngine.Random.Range(0f, 180f) * Mathf.PI / 180f;
                            pos = now_pos + (UnityEngine.Random.Range(4f, 7.5f) * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
                        }
                        while (!PosInArena(pos) && random_limit <= 10);
                        if (random_limit > 10)
                        {
                            pos = HeroController.instance.transform.position + new Vector3(0, 3f);
                            pos.y = Mathf.Min(pos.y, BossSceneManager.arena_info[ProcessManager.scene_name].up - 1f);
                            ("Random pos OUT OF LIMIT").TestLog();
                        }
                        yield return ItemManager.Instance.StartCoroutine(EnemyAppear(enemy_go, pos, enemy.hp, enemy.name));
                    }
                    else
                    {
                        ("No prefab for " + enemy.name).TestLog();
                    }

                }
            }
            yield return new WaitUntil(() => bossleft <= 0);
        }

    }
    static bool PosInArena(Vector2 pos)
    {
        if (BossSceneManager.arena_info.ContainsKey(ProcessManager.scene_name))
        {
            BossSceneManager.ArenaInfo ai = BossSceneManager.arena_info[ProcessManager.scene_name];
            if (pos.x <= ai.right - 2f && pos.x >= ai.left + 2f && pos.y >= ai.down + 2f && pos.y <= ai.up - 2f)
            {
                return true;
            }
        }
        return false;
    }
    static void AfterActiveAdjust(GameObject enemy, string name, int hp)
    {
        PlayMakerFSM temp_fsm = null;
        switch (name)
        {
            case "Flamebearer Small":
                temp_fsm = enemy.LocateMyFSM("Control");
                temp_fsm.InsertCustomAction("Set Level", (fsm) => { fsm.FsmVariables.FindFsmInt("Grimmchild Level").Value = 1; }, 0);
                temp_fsm.InsertCustomAction("Init", (fsm) => { fsm.SendEvent("START"); }, 23);
                temp_fsm.InsertCustomAction("Destroy", DeathCount, 0);
                temp_fsm = enemy.LocateMyFSM("hp_scaler");
                temp_fsm.FsmVariables.FindFsmInt("Level 1").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 2").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 3").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 4").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 5").Value = hp;

                break;
            case "Flamebearer Med":
                temp_fsm = enemy.LocateMyFSM("Control");
                temp_fsm.InsertCustomAction("Set Level", (fsm) => { fsm.FsmVariables.FindFsmInt("Grimmchild Level").Value = 2; }, 0);
                temp_fsm.InsertCustomAction("Init", (fsm) => { fsm.SendEvent("START"); }, 23);
                temp_fsm.InsertCustomAction("Destroy", DeathCount, 0);
                temp_fsm = enemy.LocateMyFSM("hp_scaler");
                temp_fsm.FsmVariables.FindFsmInt("Level 1").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 2").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 3").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 4").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 5").Value = hp;

                break;
            case "Flamebearer Large":
                temp_fsm = enemy.LocateMyFSM("Control");
                temp_fsm.InsertCustomAction("Set Level", (fsm) => { fsm.FsmVariables.FindFsmInt("Grimmchild Level").Value = 3; }, 0);
                temp_fsm.InsertCustomAction("Init", (fsm) => { fsm.SendEvent("START"); }, 23);
                temp_fsm.InsertCustomAction("Destroy", DeathCount, 0);
                temp_fsm = enemy.LocateMyFSM("hp_scaler");
                temp_fsm.FsmVariables.FindFsmInt("Level 1").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 2").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 3").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 4").Value = hp;
                temp_fsm.FsmVariables.FindFsmInt("Level 5").Value = hp;
                break;
            case "Centipede Hatcher":
                temp_fsm = enemy.LocateMyFSM("Centipede Hatcher");
                temp_fsm.InsertCustomAction("Get Centipede", (fsm) =>
                {
                    fsm.FsmVariables.FindFsmGameObject("Hatchling").Value = EnemyWaveManager.GetEnemy("Baby Centipede Spawner");
                    fsm.FsmVariables.FindFsmGameObject("Hatchling").Value.SetActive(true);
                }, 2);
                break;
            case "Mawlek Turret":
                try
                {
                    enemy.transform.SetPositionY(BossSceneManager.arena_info[ProcessManager.scene_name].down - 0.75f);

                }
                catch (Exception)
                {
                    Log("喷吐有误");
                }
                break;
            case "Moss Charger":
                try
                {
                    enemy.transform.SetPositionY(BossSceneManager.arena_info[ProcessManager.scene_name].down);
                    var x = enemy.transform.position.x;
                    if (x > BossSceneManager.arena_info[ProcessManager.scene_name].right - 12f)
                    {
                        enemy.transform.SetPositionX(BossSceneManager.arena_info[ProcessManager.scene_name].right - 12f);
                    }
                    else if (x < BossSceneManager.arena_info[ProcessManager.scene_name].left + 12f)
                    {
                        enemy.transform.SetPositionX(BossSceneManager.arena_info[ProcessManager.scene_name].left + 12f);
                    }

                }
                catch (Exception)
                {
                    Log("草团子有误");
                }
                break;
            case "Mage Blob":
                try
                {
                    // enemy.transform.SetPositionY(BossSceneManager.arena_info[ProcessManager.scene_name].down);
                    enemy.LocateMyFSM("Blob").InsertCustomAction("Hide", (fsm) =>
                    {
                        fsm.SendEvent("SPAWN");
                    }, 0);
                }
                catch (Exception)
                {
                    Log("愚蠢错误有误");
                }
                break;
            case EnemyWaveManager.beam_miner_name:
                var cp = enemy.GetAddComponent<ConstrainPosition>();
                cp.enabled = true;
                cp.constrainX = true;
                cp.constrainY = false;
                cp.xMin = BossSceneManager.arena_info[ProcessManager.scene_name].left;
                cp.xMax = BossSceneManager.arena_info[ProcessManager.scene_name].right;
                break;
            case "Zombie Guard":
                enemy.FindGameObjectInChildren("Hero Solid").SetActive(false);
                break;
            case "Great Shield Zombie":
                enemy.FindGameObjectInChildren("Hero Blocker").SetActive(false);
                break;
            case "Grave Zombie":
                enemy.FindGameObjectInChildren("Hero Blocker").SetActive(false);
                break;
            case "Slash Spider":
                enemy.FindGameObjectInChildren("Hero Blocker").SetActive(false);
                break;
            default:
                break;
        }
    }
    internal static IEnumerator EnemyAppear(GameObject enemy, Vector3 pos, int health, string enemy_name)
    {
        if (enemy == null) yield break;
        ("EnemyAppear " + enemy.name + " " + health).TestLog();
        var res = enemy;
        res.GetComponent<HealthManager>().OnDeath -= DeathCount;
        res.GetComponent<HealthManager>().OnDeath += DeathCount;
        res.transform.position = pos;
        if (res.GetComponent<PersistentBoolItem>() != null)
        {
            res.GetComponent<PersistentBoolItem>().enabled = false;
        }
        res.SetActive(true);
        res.GetComponent<HealthManager>().isDead = false;
        AfterActiveAdjust(res, enemy_name, health);

        res.GetComponent<HealthManager>().hp = health;

        var da = res.GetComponent<DamageHero>();
        int ori_damage = da.damageDealt;
        if (da != null)
        {
            da.damageDealt = 0;
        }
        var sprite = res.GetComponent<tk2dSprite>();
        if (sprite == null)
        {
            try
            {
                sprite = res.FindGameObjectInChildren("Sprite").GetComponent<tk2dSprite>();
            }
            catch (Exception) { }
        }
        float delay = 1f;
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            sprite.color = new Color(1, 1, 1, 1 - delay);
            yield return null;
        }
        sprite.color = Color.white;
        if (da != null)
        {
            da.damageDealt = ori_damage;
        }
        yield break;
    }



    private static void ModifyTHK()
    {
        var bossCtrl = GameObject.Instantiate(BossSceneManager.thk_bc);
        bossCtrl.transform.Translate(10, 0, 0);
        bossCtrl.transform.Find("break_chains").Translate(-10, 0, 0);
        bossCtrl.transform.Find("Title").Translate(-10, 0, 0);
        bossCtrl.SetActive(true);

        PlayMakerFSM battleStart = bossCtrl.LocateMyFSM("Battle Start");
        battleStart.ChangeTransition("Init", "FINISHED", "Revisit");

        GameObject thk = bossCtrl.transform.Find("Hollow Knight Boss").gameObject;

        PlayMakerFSM control = thk.LocateMyFSM("Control");
        PlayMakerFSM phaseCtrl = thk.LocateMyFSM("Phase Control");

        var bsc = BossSceneController.Instance;
        // if (bsc.BossLevel >= 1)
        // {
        thk.GetComponent<HealthManager>().hp = 2000;
        phaseCtrl.GetAction<SetHP>("Set Phase 4", 5).hp = 650;
        phaseCtrl.Fsm.GetFsmInt("Phase2 HP").Value = 1650;
        phaseCtrl.Fsm.GetFsmInt("Phase3 HP").Value = 1000;
        // }

        control.GetState("Long Roar End").RemoveAction(2); //.RemoveAction<PlayerDataBoolTest>(2);
        phaseCtrl.GetState("Set Phase 4").RemoveAction(6);//<PlayerDataBoolTest>();
        GameObject bossCorpse = thk.transform.Find("Boss Corpse").gameObject;
        PlayMakerFSM corpse = bossCorpse.LocateMyFSM("Corpse");
        corpse.GetState("Burst").RemoveAction(0);//<SendEventByName>();//存疑
        corpse.GetState("Blow").AddCustomAction(() => bsc.EndBossScene());// AddMethod(() => bsc.EndBossScene());
        corpse.GetState("Set Knight Focus").RemoveAction(0);//<SetFsmBool>();

        control.SetState("Init");

        var battleScene = GameObject.Find("Battle Scene");
        GameObject godseeker = battleScene.FindGameObjectInChildren("Godseeker Crowd");
        godseeker.transform.SetParent(null);
        FsmGameObject target = godseeker.LocateMyFSM("Control").Fsm.GetFsmGameObject("Target");
        target.Value = thk;
        battleStart.GetState("Roar Antic").AddCustomAction(() => target.Value = HeroController.instance.gameObject);// AddMethod(() => target.Value = HeroController.instance.gameObject);
        GameObject.Destroy(battleScene);
    }
    static void Log(object msg)
    {
        Rogue.Instance.Log(msg);
    }
}