using InControl;
using Mono.Cecil;
using Satchel.BetterMenus;
using Satchel.Futils;
using UnityEngine.U2D;
using rogue.Characters;
using Mono.Security.Cryptography;
using IL.tk2dRuntime.TileMap;
using System.Diagnostics;
using rogue.NPCs;

namespace rogue;


public class Rogue : Mod, ICustomMenuMod, IGlobalSettings<Setting>
{
    internal static Rogue Instance;

    public Rogue() : base("Rogue")
    {
        Instance = this;
        SpriteLoader.Init();
        GiftFactory.Initialize();
    }

    public override string GetVersion()
    {
        return "t.e.8.7";
    }
    public class actions : PlayerActionSet
    {
        public PlayerAction refresh;
        public PlayerAction start;
        public PlayerAction reset;

        public PlayerAction over;
        public actions()
        {
            refresh = new PlayerAction("rogue_refresh", this);
            start = new PlayerAction("rogue_start", this);
            reset = new PlayerAction("rogue_reset", this);
            over = new PlayerAction("rogue_over", this);
        }

    }


    internal bool Test = true;
    public Setting _set = new();
    public static actions self_actions = new();
    public GameObject rogue_go = null;
    public GameObject charms;

    public GameObject shiny_item = null;
    public string charmString;

    public bool ToggleButtonInsideMenu => true;

    public ItemManager itemManager => ItemManager.Instance;

    public Dictionary<GameObject, GameObject> shop_items = new();

    public SpriteAtlas all_sprites;

    BossSequence bossSequence;

    BossScene[] ori_boss_scenes = null;

    internal BossScene[] boss_scenes = null;



    public override List<(string, string)> GetPreloadNames()
    {

        return NPCManager.GetPreloadNames().Concat(PreloadManager.GetPreloadNames()).Concat(EnemyWaveManager.GetPreloadNames()).ToList();
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        rogue_go = new GameObject("rogue_go");
        UnityEngine.Object.DontDestroyOnLoad(rogue_go);
        Lang.init();
        PreloadManager.Init(preloadedObjects);
        RogueSceneManager.Init();
        ItemManager.Init();
        Shaman.Init();
        NailMaster.Init();
        RogueUIManager.Init();
        NPCManager.Init(preloadedObjects);
        BossSceneManager.Init();
        BugFixManager.Init();
        DisplayManager.Init();
        ProcessManager.Init();
        EnemyWaveManager.Init();


        On.PlayMakerFSM.OnEnable += CharmsInit;
        On.BossSequenceController.FinishLastBossScene += EndScene;
        On.BossSequenceController.SetupNewSequence += BeginScene;
        On.HeroController.Awake += OnSavegameLoad;
        ModHooks.SavegameSaveHook += TestSavaGame;
        On.GameCameras.Awake += CameraAwake;
    }

    private void CameraAwake(On.GameCameras.orig_Awake orig, GameCameras self)
    {
        Rogue.Instance.Log("CameraAwake");
    }
    private void TestSavaGame(int obj)
    {
        Log("Save Game!!!");
    }






    private void OnSavegameLoad(On.HeroController.orig_Awake orig, HeroController self)
    {
        orig(self);
        ItemManager.GameLoadInit();
        RogueSceneManager.GameLoadInit();
        NPCManager.GameLoadInit();
        BugFixManager.GameLoadInit();
        // PlayerData.instance.respawnMarkerName = "RestBench (1)";

    }


    private void BeginScene(On.BossSequenceController.orig_SetupNewSequence orig, BossSequence sequence, BossSequenceController.ChallengeBindings bindings, string playerData)
    {
        if (sequence.name == "Boss Sequence Tier 5")
        {
            bossSequence = sequence;
            if (ori_boss_scenes == null)
            {
                ori_boss_scenes = ReflectionHelper.GetField<BossSequence, BossScene[]>(sequence, "bossScenes");
                boss_scenes = new BossScene[ori_boss_scenes.Length];
            }
            ori_boss_scenes.CopyTo(boss_scenes, 0);
            foreach (var boss_scene in ori_boss_scenes)
            {
                Log(boss_scene.sceneName);
            }
            ReflectionHelper.SetField<BossSequence, BossScene[]>(sequence, "bossScenes", boss_scenes);
            BossSceneManager.MoreBoss(sequence);

        }
        orig(sequence, bindings, playerData);
    }

    private void EndScene(On.BossSequenceController.orig_FinishLastBossScene orig, BossSceneController self)
    {
        GameInfo.in_rogue = false;
        orig(self);

    }




    private void CharmsInit(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "UI Charms" && self.gameObject.name == "Charms")
        {
            charms = self.gameObject;
            charms.
            FindGameObjectInChildren("Equipped Charms").
            FindGameObjectInChildren("Notches").
            FindGameObjectInChildren("Text Notches").
            GetComponent<TMPro.TextMeshPro>().text =
            "get_any_charm_front".Localize() + GameInfo.get_any_charm_num + "get_any_charm_behind".Localize();

            self.ChangeTransition("Charm Collected?", "UNCOLLECTED", "Collected");
            if (self.GetState("Collected").Actions.Length == 10)
            {
                self.InsertCustomAction("Collected", (fsm) =>
                {
                    charmString = fsm.FsmVariables.GetFsmString("Build String").Value;
                    fsm.FsmVariables.GetFsmBool("Got Charm").Value = GameManager.instance.GetPlayerDataBool(fsm.FsmVariables.GetFsmString("Build String").Value);
                }, 1);
            }
            if (self.GetState("Deactivate UI").Actions.Length == 6)
            {
                self.RemoveAction("Deactivate UI", 2);
                self.InsertCustomAction("Deactivate UI", (fsm) =>
                {
                    if ((CharmHelper.can_stand_equip_charm_scene_names.Contains(ProcessManager.scene_name) || CharmHelper.can_equip_everywhere) && GameInfo.in_rogue)
                    {
                    }
                    else
                    {
                        if (!PlayerData.instance.atBench) fsm.SendEvent("NOT BENCH");
                    }
                }, 2);
                self.InsertCustomAction("Deactivate UI", (fsm) =>
                {
                    var val = fsm.FsmVariables.GetFsmBool("Got Charm").Value;
                    if (!val && GameInfo.get_any_charm_num > 0)
                    {
                        GameManager.instance.SetPlayerDataBool(charmString, true);
                        fsm.FsmVariables.GetFsmBool("Got Charm").Value = true;
                        GameInfo.get_any_charm_num--;
                        charms.FindGameObjectInChildren("Equipped Charms").
                        FindGameObjectInChildren("Notches").FindGameObjectInChildren("Text Notches").GetComponent<TMPro.TextMeshPro>().text = "get_any_charm_front".Localize() + GameInfo.get_any_charm_num + "get_any_charm_behind".Localize();
                    }
                }, 1);
            }
            if (self.GetState("Black Charm?").Actions.Length == 2)
            {
                self.InsertCustomAction("Black Charm?", (fsm) =>
                {
                    if (CharmHelper.cant_unequip_charm.Contains(fsm.GetVariable<FsmInt>("Current Item Number").Value))
                    {
                        fsm.SendEvent("CANCEL");
                    }
                }, 0);
            }
            if (self.GetState("Black Charm? 2").Actions.Length == 2)
            {
                self.InsertCustomAction("Black Charm? 2", (fsm) =>
                                {
                                    if (CharmHelper.cant_unequip_charm.Contains(fsm.GetVariable<FsmInt>("Current Item Number").Value))
                                    {
                                        fsm.SendEvent("CANCEL");
                                    }
                                }, 0);
            }
        }
        orig(self);
    }

    public void Rogue_Reset()
    {
        GiftHelper.AdjustMaskTo(5);
        GiftHelper.AdjustVesselTo(0);
        GiftHelper.AdjustNailLevel(0);
        GiftHelper.RemoveAllCharms();
        GiftHelper.RemoveAllSkills();
        GiftFactory.ResetWeight();

        List<GameObject> childlist = new();
        GameObject itemlist = ItemManager.menu_go.FindGameObjectInChildren("Item List");
        itemlist.FindAllChildren(childlist);
        foreach (var child in childlist)
        {
            if (child == itemlist) continue;
            else GameObject.Destroy(child);
        }

        while (HeroController.instance?.gameObject.GetComponent<Character>() != null)
            HeroController.instance?.gameObject.RemoveComponent<Character>();
        GameInfo.Reset();
        PlayerData.instance.geo = 0;
        itemManager.timeSpan = TimeSpan.FromSeconds(0);


    }

    public void Unload()
    {
        if (itemManager != null)
        {
            GameObject.Destroy(itemManager.gameObject);
        }
    }
    public void Rogue_Start()
    {
        if (itemManager != null)
        {
            GameInfo.Start();
            itemManager.DestroyAllItems();
            itemManager.rewardsStack.Clear();
            itemManager.rewardsStack.Push(new ItemManager.OneReward()
            {
                give_mode = ItemManager.OneReward.GiveMode.random,
                gift_variety = GiftFactory.CustomVariety(),
                select = 1,
                give = 3
            });
            itemManager.rewardsStack.Push(new ItemManager.OneReward()
            {
                give_mode = ItemManager.OneReward.GiveMode.random,
                gift_variety = GiftVariety.item,
                select = 1,
                give = 3
            });
            itemManager.rewardsStack.Push(new ItemManager.OneReward()
            {
                give_mode = ItemManager.OneReward.GiveMode.random,
                gift_variety = GiftVariety.item,
                select = 1,
                give = 3
            });
            itemManager.rewardsStack.Push(new ItemManager.OneReward()
            {
                give_mode = ItemManager.OneReward.GiveMode.fix,
                select = 1,
                gifts = GameInfo.act_gifts[GiftVariety.huge]
            });
            itemManager.Next(true);
            DisplayManager.BeginDisplay();
            HeroController.instance.geoCounter.gameObject.GetComponent<DeactivateIfPlayerdataTrue>().enabled = false;
            HeroController.instance.geoCounter.gameObject.SetActive(true);
            HeroController.instance.TakeGeo(PlayerData.instance.geo);
        }

    }

    public void Rogue_Over()
    {
        if (itemManager != null)
        {
            GameInfo.Over();
            HeroController.instance.geoCounter.gameObject.GetComponent<DeactivateIfPlayerdataTrue>().enabled = false;
            HeroController.instance.geoCounter.gameObject.SetActive(true);
            HeroController.instance.TakeGeo(PlayerData.instance.geo);
            HeroController.instance.geoCounter.gameObject.SetActive(false);
            HeroController.instance.geoCounter.gameObject.GetComponent<DeactivateIfPlayerdataTrue>().enabled = true;
            PlayerData.instance.nailDamage = 21;
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
            GiftHelper.AdjustMaskTo(9);
            GiftHelper.AdjustVesselTo(3);
            GiftHelper.GiveAllCharms();
            GiftHelper.GiveAllSkills();


            DisplayManager.DisplayStates();
        }
    }


    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        Satchel.BetterMenus.Menu menu = new("Rogue");
        var b = new Satchel.BetterMenus.KeyBind("rouge_refresh".Localize(), self_actions.refresh);
        var c = new Satchel.BetterMenus.KeyBind("rouge_end".Localize(), self_actions.over);
        var d = new Satchel.BetterMenus.KeyBind("rogue_begin".Localize(), self_actions.start);
        menu.AddElement(b);
        menu.AddElement(d);
        menu.AddElement(c);

        var text = new Satchel.BetterMenus.TextPanel("menu_intro".Localize(), width: 1500);
        Menu giftBase = new("menu_gift_base".Localize());
        int count = 0;
        List<List<Element>> list = new();
        List<Element> tempList = null;
        giftBase.AddElement(new TextPanel("menu_small_gift".Localize()));
        List<Tuple<string, MenuScreen>> gift_menu_screens = new();
        List<Menu> gift_menus = new();
        foreach (var gifts in GiftFactory.all_kind_of_gifts)
        {
            Menu giftmenu = new(gifts.Key.ToString());
            gift_menus.Add(giftmenu);
            foreach (var gift in gifts.Value)
            {
                Gift g = gift;
                Element temp;
                if (g.GetName() == null) continue;
                if (g.giftname.ToString().StartsWith("custom_"))
                {
                    if (g is CustomGift customGift)
                    {
                        temp = Blueprints.ToggleButton(customGift.GetName().Replace('\n', '&'), "",
                        (act) =>
                        {
                            customGift.Got = act;
                        },
                        () => customGift.Got
                        );
                    }
                    else if (g is CustomOneOrTwoGift customOneOrTwoGift)
                    {
                        temp = new Satchel.BetterMenus.HorizontalOption(customOneOrTwoGift.GetName(),
                        customOneOrTwoGift.GetDesc(),
                        new string[] { "null", customOneOrTwoGift.GetName1(), customOneOrTwoGift.GetName2() },
                        (num) =>
                        {
                            customOneOrTwoGift.Got_Which_Item = (CustomOneOrTwoGift.GotWhichItem)num;
                        },
                        () => (int)customOneOrTwoGift.Got_Which_Item
                        );
                    }
                    else if (g is CustomCountedGift customCountedGift)
                    {
                        temp = new Satchel.BetterMenus.CustomSlider(customCountedGift.GetName(),
                        (value) => customCountedGift.SetCount((int)value),
                         () => (float)customCountedGift.count,
                         0, 10,
                         true);
                    }
                    else temp = new Satchel.BetterMenus.TextPanel("ERROR");
                }
                else
                {
                    temp = Blueprints.ToggleButton(g.GetName().Replace('\n', '&'), "",
                    (act) =>
                    {
                        g.active = act;
                    },
                    () => g.active
                    );
                }
                if (count == 2)
                {
                    giftmenu.AddElement(new MenuRow(tempList, ""));
                    count = 0;
                }
                if (count == 0) tempList = new();
                tempList.Add(temp);
                count++;
            }
            if (count > 0)
            {
                giftmenu.AddElement(new MenuRow(tempList, ""));
                count = 0;
            }
            var gm = giftmenu.GetMenuScreen(giftmenu.returnScreen);
            gift_menu_screens.Add(new(gifts.Key.ToString(), gm));
        }
        foreach (var gm in gift_menu_screens)
        {
            giftBase.AddElement(new MenuButton(gm.Item1, "", (but) =>
            {
                Utils.GoToMenuScreen(gm.Item2);
            }));
        }
        var gs = giftBase.GetMenuScreen(giftBase.returnScreen);
        foreach (var gift_menu in gift_menus)
        {
            gift_menu.returnScreen = gs;
        }

        menu.AddElement(new MenuButton("menu_gift_base".Localize(), "", (but) =>
        {
            Utils.GoToMenuScreen(gs);
        }));
        menu.AddElement(new MenuButton("Reload EnemyWave", "", (but) =>
        {
            EnemyWaveManager.ReloadWaveCollection();
        }));


        Menu setting = new("Settings");
        setting.AddElement(new Satchel.BetterMenus.CustomSlider("font_size".Localize(), (size) => { _set.item_font_size = size; }, () => _set.item_font_size, 0, 10));
        setting.AddElement(new Satchel.BetterMenus.CustomSlider("ui_transparency".Localize(), (alpha) => { _set.UI_alpha = alpha; }, () => _set.UI_alpha, 0, 1));
        setting.AddElement(Blueprints.IntInputField("reroll_num".Localize(), (num) => { _set.reroll_num = num; }, () => _set.reroll_num, 3));
        setting.AddElement(new Satchel.BetterMenus.MenuButton("default", "turn all to default", (but) => { _set.item_font_size = 6.67f; _set.UI_alpha = 0.6f; _set.reroll_num = 3; setting.Update(); }));
        setting.AddElement(Blueprints.ToggleButton("details".Localize(), "", (value) => { _set.details = value; }, () => _set.details));
        var ss = setting.GetMenuScreen(setting.returnScreen);
        menu.AddElement(new MenuButton("Settings", "", (but) =>
        {
            Utils.GoToMenuScreen(ss);
        }));
        menu.AddElement(text);
        var ms = menu.GetMenuScreen(modListMenu);
        giftBase.returnScreen = ms;
        setting.returnScreen = ms;
        return ms;

    }

    public void OnLoadGlobal(Setting s)
    {
        Log("Onload");
        _set = s;
        if (_set.start == null) ;
        else if (_set.start is KeyBindingSource keyBindingSource1)
        {
            KeyBindingSource start = new KeyBindingSource((Key)_set.startname);
            self_actions.start.AddBinding(start);
        }
        else if (_set.start is MouseBindingSource mouseBindingSource)
        {
            MouseBindingSource start = new MouseBindingSource((Mouse)_set.startname);
            self_actions.start.AddBinding(start);
        }

        if (_set.over == null) ;
        else if (_set.over is KeyBindingSource keyBindingSource1)
        {
            KeyBindingSource over = new KeyBindingSource((Key)_set.overname);
            self_actions.over.AddBinding(over);
        }
        else if (_set.over is MouseBindingSource mouseBindingSource)
        {
            MouseBindingSource over = new MouseBindingSource((Mouse)_set.overname);
            self_actions.over.AddBinding(over);
        }

        if (_set.refresh == null) ;
        else if (_set.refresh is KeyBindingSource keyBindingSource1)
        {
            KeyBindingSource refresh = new KeyBindingSource((Key)_set.refreshname);
            self_actions.refresh.AddBinding(refresh);
        }
        else if (_set.refresh is MouseBindingSource mouseBindingSource)
        {
            MouseBindingSource refresh = new MouseBindingSource((Mouse)_set.refreshname);
            self_actions.refresh.AddBinding(refresh);
        }




    }

    public Setting OnSaveGlobal()
    {
        if (self_actions.refresh.Bindings.Count > 0)
        {
            _set.refresh = self_actions.refresh.Bindings[0];
            if (_set.refresh is KeyBindingSource keyBindingSource)
            {
                _set.refreshname = ReflectionHelper.GetField<KeyCombo, ulong>(keyBindingSource.Control, "includeData");
            }
            else if (_set.refresh is MouseBindingSource mouseBindingSource)
            {
                _set.refreshname = (ulong)mouseBindingSource.Control;
            }
        }
        if (self_actions.over.Bindings.Count > 0)
        {
            _set.over = self_actions.over.Bindings[0];
            if (_set.over is KeyBindingSource keyBindingSource)
            {
                _set.overname = ReflectionHelper.GetField<KeyCombo, ulong>(keyBindingSource.Control, "includeData");
            }
            else if (_set.over is MouseBindingSource mouseBindingSource)
            {
                _set.overname = (ulong)mouseBindingSource.Control;
            }
        }
        if (self_actions.start.Bindings.Count > 0)
        {
            _set.start = self_actions.start.Bindings[0];
            if (_set.start is KeyBindingSource keyBindingSource)
            {
                _set.startname = ReflectionHelper.GetField<KeyCombo, ulong>(keyBindingSource.Control, "includeData");
            }
            else if (_set.start is MouseBindingSource mouseBindingSource)
            {
                _set.startname = (ulong)mouseBindingSource.Control;
            }
        }
        return _set;
    }


    public void ShowConvo(string msg)
    {

        if (shiny_item == null) return;
        GameObject get_msg = shiny_item.LocateMyFSM("Shiny Control").GetAction<SpawnObjectFromGlobalPool>("Trink Flash", 5).gameObject.Value;
        GameObject new_msg = get_msg.Spawn(new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        new_msg.FindGameObjectInChildren("Icon").GetComponent<SpriteRenderer>().sprite = null;
        new_msg.FindGameObjectInChildren("Text").GetComponent<TMPro.TextMeshPro>().text = msg;
        return;
    }

    public void ShowDreamConvo(string msg)
    {
        PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HutongGames.PlayMaker.FsmVariables.GlobalVariables.GetFsmGameObject("Enemy Dream Msg").Value, "Display");
        playMakerFSM.FsmVariables.GetFsmString("Convo Title").Value = "rogue_" + msg;
        playMakerFSM.FsmVariables.GetFsmInt("Convo Amount").Value = 1;
        playMakerFSM.SendEvent("DISPLAY ENEMY DREAM");
    }

    internal static void TestLog(object msg)
    {
        if (Instance.Test)
        {
            Instance.Log(msg);
        }

    }
}
