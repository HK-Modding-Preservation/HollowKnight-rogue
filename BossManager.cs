using System.Data;
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



        On.PlayMakerFSM.OnEnable += ModifyRadiance;
    }

    private static void ModifyRadiance(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.gameObject.name == "Absolute Radiance" && self.FsmName == "Control")
        {
            self.gameObject.AddComponent<ModBosses.Uradiance>();
        }
        orig(self);
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
            if (ItemManager.Instance.scenename.Contains("GG_Collector"))
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
            if (ItemManager.Instance.scenename.Contains("GG_Collector"))
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
            if (ItemManager.Instance.scenename.Contains("GG_Collector"))
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




    static void DeathCount()
    {
        bossleft--;
        Rogue.Instance.Log(bossleft);
        if (bossleft == 0 && can_self_over)
        {
            BossSceneController.Instance.EndBossScene();
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
        thk.GetComponent<HealthManager>().hp = 1800;
        phaseCtrl.Fsm.GetFsmInt("Phase2 HP").Value = 1300;
        phaseCtrl.Fsm.GetFsmInt("Phase3 HP").Value = 950;
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

}