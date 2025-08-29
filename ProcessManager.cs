

using rogue;

internal class ProcessManager : MonoBehaviour
{
    internal static ProcessManager Instance = null;
    internal static string scene_name = "";
    internal static List<string> nobossscene = new List<string> { "GG_Spa",
        "GG_Engine",
        "GG_Unn",
        "GG_Engine_Root",
        "GG_Wyrm",
        "GG_Atrium_Roof" };

    public Hooks.OnChangeSceneAndAddGeo after_scene_add_geo_num;
    int ReplacifyRate = 50;

    internal static void Init()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = Rogue.Instance.rogue_go.AddComponent<ProcessManager>();
    }
    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
    }
    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    internal void GameStart()
    {
        Rogue.Instance.Rogue_Reset();
        Rogue.Instance.Rogue_Start();
        DisplayManager.DisplayEquipped();
        DisplayManager.DisplayStates();
        if (scene_name == "GG_Atrium_Roof")
        {
            ItemManager.Instance.StartShopItem();
            ItemManager.Instance.SetShop(false);
            RogueSceneManager.ResetAll();
            RogueSceneManager.SetBench();
            RogueSceneManager.SetGou();
        }
    }
    internal void OnSceneChanged(Scene last_scene, Scene next_scene)
    {
        ClearLastScene();
        scene_name = next_scene.name;
        PrepareNextScene();

    }

    private void ClearLastScene()
    {
        if (GameInfo.in_rogue && BossSequenceController.IsInSequence)
        {
            //战斗给予吉欧
            BattleGeoReward(scene_name);

            //计算本轮分数
            GameInfo.data.CalScore();

            //重置本房间受伤数
            GameInfo.data.ResetSceneInjured();
        }
    }

    private void BattleGeoReward(string scene_name)
    {
        if (GameInfo.in_rogue && BossSequenceController.IsInSequence)
        {
            if (!nobossscene.Contains(scene_name))
            {
                int geo = 50;
                if (PlayerData.instance.equippedCharm_24) geo += 20;
                if (GameInfo.data.single_scene_num_injured == 0) geo += 20;
                if (after_scene_add_geo_num != null) geo = after_scene_add_geo_num(geo, GameInfo.data.single_scene_num_injured);
                HeroController.instance.AddGeo(geo);
            }

        }
    }

    private void PrepareNextScene()
    {
        ModifyScene();
        DisplayUI();
        FlowerManager.CheckFlower(scene_name);
        GameInfo.can_get_birthright = true;
    }

    private void DisplayUI()
    {
        DisplayManager.DisplayStates();
        DisplayManager.DisplayEquipped();
        if (scene_name == "GG_End_Sequence")
        {
            GameInfo.data.CalSoreEnd();
            DisplayManager.ShowScore();
        }
    }

    private void ModifyScene()
    {
        //场景部分
        RogueSceneManager.ModifyScene(scene_name);
        //Boss部分
        if (!nobossscene.Contains(scene_name) && GameInfo.in_rogue)
        {
            StartCoroutine(BossManager.AdjustBossHP(scene_name));
            if ((UnityEngine.Random.Range(0, 101) <= ReplacifyRate || EnemyWaveManager.SceneInBranch(scene_name))
             && !EnemyWaveManager.DontReplacyScene(scene_name))
            {
                ("Replacify Boss in " + scene_name).TestLog();
                StartCoroutine(BossManager.Replacify(scene_name));
            }

            // StartCoroutine(BossManager.LoveKeyify(scene_name));
        }
    }
}