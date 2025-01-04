using InControl;
using Mono.Cecil;
using Satchel.BetterMenus;
using Satchel.Futils;
using UnityEngine.U2D;
using rogue.Characters;
using Mono.Security.Cryptography;
using IL.tk2dRuntime.TileMap;

namespace rogue;


public class Rogue : Mod, ICustomMenuMod, IGlobalSettings<setting>
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
        return "1.0.0.5";
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



    public setting _set = new();
    public static actions self_actions = new();
    private string item_scene = "Tutorial_01";
    private string item = "_Props/Chest/Item/Shiny Item (1)";

    private string shop_scene = "Room_shop";

    private string shop_region = "Basement Closed/Shop Region";

    private string shop_menu = "Shop Menu";
    private string shop_counter = "_Scenery/Shop Counter";

    private string gouBro_scene = "Mines_18_boss";

    private string gouBro = "Mega Zombie Beam Miner (1)";

    private string bench_scene = "Mines_18";

    private string bench = "_Props/RestBench";

    private string bank_scene = "Fungus3_35";
    private string bank = "Bank Stand";
    private string banker = "Banker";

    private string white_palace = "White_Palace_01";

    private string white_fly = "White Palace Fly";

    internal const string card_scene = "RestingGrounds_09";
    internal const string card_name = "Cornifer Card";

    internal const string beam_scene = "GG_Radiance";
    internal const string beam_name = "Boss Control/Radiant Beam";

    internal const string butterfly_scene = "Cliffs_05";
    internal const string butterfly_name = "Butterflies FG 1";
    internal const string hk_scene = "GG_Hollow_Knight";
    internal const string hk_name = "Battle Scene/HK Prime";
    public GameObject charms;

    public GameObject shiny_item = null;

    public GameObject menu_go = null;

    public GameObject shop_go = null;

    public GameObject fly_go = null;
    public string charmString;

    public bool ToggleButtonInsideMenu => true;

    public ItemManager itemManager;

    public Dictionary<GameObject, GameObject> shop_items = new();

    public SpriteAtlas all_sprites;



    public tk2dSpriteAnimation gou_animation = null;

    public tk2dSpriteAnimation bank_animation = null;
    public tk2dSpriteAnimation banker_animation = null;

    public GameObject bench_go = null;

    public static int pretty_key_num = 0;




    public override List<(string, string)> GetPreloadNames()
    {
        var list = new List<(string, string)>
        {
            ("Tutorial_01","_Props/Chest/Item/Shiny Item (1)"),
            (shop_scene,shop_region),
            (shop_scene,shop_menu),
            (shop_scene,shop_counter),
            (gouBro_scene,gouBro),
            (bench_scene,bench),
            (bank_scene,bank),
            (bank_scene,banker),
            (white_palace,white_fly),
            (card_scene,card_name),
            (beam_scene,beam_name),
            (butterfly_scene,butterfly_name),
            (hk_scene,hk_name)
        };
        return list.Concat(NPCManager.GetPreloadNames()).ToList();
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        Lang.init();


        shiny_item = UnityEngine.Object.Instantiate(preloadedObjects[item_scene][item]);
        shiny_item.LocateMyFSM("Shiny Control").ChangeTransition("PD Bool?", "COLLECTED", "Fling?");
        shiny_item.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(shiny_item);

        var gou_go = preloadedObjects[gouBro_scene][gouBro];
        gou_animation = UnityEngine.Object.Instantiate(gou_go.GetComponent<tk2dSpriteAnimator>().Library);
        UnityEngine.Object.DontDestroyOnLoad(gou_animation);
        gou_go.SetActive(false);

        var bank_go = preloadedObjects[bank_scene][bank];
        bank_animation = UnityEngine.Object.Instantiate(bank_go.GetComponent<tk2dSpriteAnimator>().Library);
        UnityEngine.Object.DontDestroyOnLoad(bank_animation);
        bank_go.SetActive(false);

        var banker_go = preloadedObjects[bank_scene][banker];
        banker_animation = UnityEngine.Object.Instantiate(banker_go.GetComponent<tk2dSpriteAnimator>().Library);
        UnityEngine.Object.DontDestroyOnLoad(banker_animation);
        banker_go.SetActive(false);

        fly_go = UnityEngine.Object.Instantiate(preloadedObjects[white_palace][white_fly]);
        UnityEngine.Object.DontDestroyOnLoad(fly_go);
        fly_go.SetActive(false);

        bench_go = UnityEngine.Object.Instantiate(preloadedObjects[bench_scene][bench]);
        UnityEngine.Object.DontDestroyOnLoad(bench_go);
        bench_go.SetActive(false);

        menu_go = UnityEngine.Object.Instantiate(preloadedObjects[shop_scene][shop_menu]);
        menu_go.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(menu_go);

        shop_go = UnityEngine.Object.Instantiate(preloadedObjects[shop_scene][shop_region]);
        shop_go.AddComponent<SpriteRenderer>().sprite = preloadedObjects[shop_scene][shop_counter].GetComponent<SpriteRenderer>().sprite;
        shop_go.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(shop_go);

        Shaman.beam = UnityEngine.Object.Instantiate(preloadedObjects[beam_scene][beam_name]);
        Shaman.beam.RemoveComponent<DamageHero>();
        var de = Shaman.beam.AddComponent<DamageEnemies>();
        de.attackType = AttackTypes.Spell;
        de.damageDealt = 20;
        de.ignoreInvuln = false;
        Shaman.beam.layer = LayerMask.NameToLayer("Attack");
        Shaman.beam.SetActive(false);
        GameObject.DontDestroyOnLoad(Shaman.beam);

        Shaman.butterfly = UnityEngine.Object.Instantiate(preloadedObjects[butterfly_scene][butterfly_name]);
        var shape = Shaman.butterfly.GetComponent<ParticleSystem>().shape;
        shape.scale = new Vector3(0.2f, 1, 1);
        var main = Shaman.butterfly.GetComponent<ParticleSystem>().main;
        main.duration = 0.5f;
        main.startSpeed = 20;
        main.loop = false;
        var emiss = Shaman.butterfly.GetComponent<ParticleSystem>().emission;
        emiss.burstCount = 8;
        Shaman.butterfly.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(Shaman.butterfly);

        NailMaster.hk_shot = UnityEngine.Object.Instantiate(preloadedObjects[hk_scene][hk_name].LocateMyFSM("Control").GetAction<FlingObjectsFromGlobalPoolTime>("SmallShot LowHigh", 2).gameObject.Value);
        NailMaster.hk_shot.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(NailMaster.hk_shot);
        NailMaster.hk_shot.RemoveComponent<DamageHero>();
        NailMaster.hk_shot.GetComponent<AudioSource>().enabled = false;
        var shot_de = NailMaster.hk_shot.AddComponent<DamageEnemies>();
        shot_de.attackType = AttackTypes.Nail;
        shot_de.damageDealt = 20;
        shot_de.ignoreInvuln = false;
        NailMaster.hk_shot.layer = LayerMask.NameToLayer("Attack");


        RogueUIManager.DialogueUI.customDialogueManager = new(preloadedObjects[card_scene][card_name]);
        RogueUIManager.DialogueUI.customDialogueManager.DialogManager.GetComponent<SpriteRenderer>().enabled = false;
        RogueUIManager.DialogueUI.customDialogueManager.DialogManager.FindGameObjectInChildren("Shiny").SetActive(false);
        preloadedObjects[card_scene][card_name].SetActive(false);
        RogueUIManager.DialogueUI.initialized = true;

        NPCManager.Init(preloadedObjects);


        GameObject rogue_go = new GameObject("rogue_go");
        UnityEngine.Object.DontDestroyOnLoad(rogue_go);
        itemManager = rogue_go.AddComponent<ItemManager>();


        On.PlayMakerFSM.OnEnable += CharmsInit;

        On.BossSequenceController.FinishLastBossScene += EndScene;
        On.BossSequenceController.SetupNewSequence += BeginScene;

        On.HeroController.Awake += OnSavegameLoad;
        ModHooks.SavegameSaveHook += TestSavaGame;
        On.GameCameras.Awake += CameraAwake;
        RogueUIManager.conversation = "132";
    }

    private void CameraAwake(On.GameCameras.orig_Awake orig, GameCameras self)
    {
        Rogue.Instance.Log("CameraAwake");
    }
    private void TestSavaGame(int obj)
    {
        Log("Save Game!!!");
    }

    public void adjust_menu_go(GameObject menu_go)
    {
        menu_go.LocateMyFSM("shop_control").ChangeTransition("Stock?", "FINISHED", "Open Window");
        // menu_go.LocateMyFSM("shop_control").ChangeTransition("Stock?", "NO STOCK", "Open Window");
        menu_go.LocateMyFSM("shop_control").FsmVariables.FindFsmString("No Stock Event").Value = "ISELDA";
        menu_go.LocateMyFSM("shop_control").GetAction<SetFsmString>("Iselda", 4).setValue = "rouge_introduction";
        menu_go.LocateMyFSM("shop_control").GetAction<CallMethodProper>("Iselda", 5).parameters[0].stringValue = "rouge_introduction";
        menu_go.LocateMyFSM("shop_control").GetAction<CallMethodProper>("Iselda", 5).parameters[1].stringValue = "rouge";
        var fsm = menu_go.FindGameObjectInChildren("Item List").LocateMyFSM("Item List Control");
        fsm.GetAction<SetTextMeshProText>("Get Details Init", 2).textString = fsm.FsmVariables.GetFsmString("Item Name Convo");
        fsm.GetAction<SetTextMeshProText>("Get Details Init", 6).textString = fsm.FsmVariables.GetFsmString("Item Desc Convo");
        fsm.GetAction<SetTextMeshProText>("Get Details", 3).textString = fsm.FsmVariables.GetFsmString("Item Name Convo");
        fsm.GetAction<SetTextMeshProText>("Get Details", 6).textString = fsm.FsmVariables.GetFsmString("Item Desc Convo");
        if (menu_go.FindGameObjectInChildren("Confirm").FindGameObjectInChildren("UI List").LocateMyFSM("Confirm Control").GetState("Special Type?").Actions.Length < 3)
        {
            menu_go.FindGameObjectInChildren("Confirm").FindGameObjectInChildren("UI List").LocateMyFSM("Confirm Control").InsertCustomAction("Special Type?", (fsm) =>
            {
                Giftname type = (Giftname)(fsm.FsmVariables.GetFsmInt("Special Type").Value - 18);
                if (GiftFactory.all_gifts.ContainsKey(type))
                {
                    GiftFactory.all_gifts[type].GetGift();
                    itemManager.DisplayStates();
                    if (GiftFactory.all_gifts[type].showConvo) ShowConvo(GiftFactory.all_gifts[type].GetShowString());
                }
                GiftFactory.UpdateWeight();
            }, 1);
        }
    }
    public void adjust_shop_go(GameObject shop_go)
    {
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Intro Convo?", "YES", "Shop Up");
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Intro Convo?", "NO", "Shop Up");
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Check Facing", "TURN RIGHT", "Turn Hero Left");
        shop_go.LocateMyFSM("Shop Region").ChangeTransition("Check Facing", "FINISHED", "Turn Hero Left");
        if (shop_go.LocateMyFSM("Shop Region").GetState("Out Of Range").Actions.Length == 2)
        {
            shop_go.LocateMyFSM("Shop Region").InsertCustomAction("Out Of Range", (fsm) =>
            {
                fsm.gameObject.FindGameObjectInChildren("gou_Bro").GetComponent<tk2dSpriteAnimator>().Play("Sleep");
            }, 2);
        }
        if (shop_go.LocateMyFSM("Shop Region").GetState("Shop Up").Actions.Length == 2)
        {
            shop_go.LocateMyFSM("Shop Region").InsertCustomAction("Shop Up", (fsm) =>
            {
                fsm.gameObject.FindGameObjectInChildren("gou_Bro").GetComponent<tk2dSpriteAnimator>().Play("Wake");
            }, 2);
        }
        GameObject gou = new("gou_Bro");
        gou.transform.SetParent(shop_go.transform);
        gou.transform.localPosition = new Vector3(-0.1f, 0.3f, -0.009f);
        gou.AddComponent<tk2dSprite>();
        gou.AddComponent<tk2dSpriteAnimator>().Library = gou_animation;


        GameObject bank = new("bank");
        bank.transform.SetParent(shop_go.transform);
        bank.transform.localPosition = new Vector3(0.1f, 1.2f, 0);
        bank.AddComponent<tk2dSprite>();
        bank.AddComponent<tk2dSpriteAnimator>().Library = bank_animation;

        GameObject banker = new("banker");
        banker.transform.SetParent(shop_go.transform);
        banker.transform.localPosition = new Vector3(0, 0, 0);
        banker.AddComponent<tk2dSprite>();
        banker.AddComponent<tk2dSpriteAnimator>().Library = banker_animation;
    }
    public void adjust_bench_go(GameObject bench_go)
    {
        bench_go.LocateMyFSM("Bench Control").FsmVariables.FindFsmVector3("Adjust Vector").Value = new Vector3(-0.5f, 0.1f, 0);
        bench_go.transform.position = new Vector3(128.8f, 61.7f, 0.1f);
        bench_go.name = "RestBench (1)";
    }



    private void OnSavegameLoad(On.HeroController.orig_Awake orig, HeroController self)
    {
        orig(self);
        // PlayerData.instance.respawnMarkerName = "RestBench (1)";
        if (itemManager == null)
        {
            Log("ERR   itemmanager is null!");
            return;
        }
        if (itemManager.item_menu_go != null)
        {
            itemManager.item_menu_go.SetActive(false);
            GameObject.DestroyImmediate(itemManager.item_menu_go);
        }

        itemManager.item_menu_go = GameObject.Instantiate(menu_go);
        adjust_menu_go(itemManager.item_menu_go);
        GameObject.DontDestroyOnLoad(itemManager.item_menu_go);
        itemManager.item_menu_go.SetActive(false);

        if (itemManager.item_shop_go != null)
        {
            itemManager.item_shop_go.SetActive(false);
            GameObject.DestroyImmediate(itemManager.item_shop_go);
        }
        itemManager.item_shop_go = GameObject.Instantiate(shop_go);
        adjust_shop_go(itemManager.item_shop_go);
        GameObject.DontDestroyOnLoad(itemManager.item_shop_go);
        itemManager.item_shop_go.SetActive(false);

        if (itemManager.item_fly_go != null)
        {
            itemManager.item_fly_go.SetActive(false);
            GameObject.DestroyImmediate(itemManager.item_fly_go);
        }
        itemManager.item_fly_go = GameObject.Instantiate(fly_go);
        GameObject.DontDestroyOnLoad(itemManager.item_fly_go);
        itemManager.item_fly_go.SetActive(false);

        if (itemManager.item_restbench != null)
        {
            itemManager.item_restbench.SetActive(false);
            GameObject.DestroyImmediate(itemManager.item_restbench);
        }
        HeroController.instance.Respawn();
        itemManager.item_restbench = GameObject.Instantiate(bench_go);
        adjust_bench_go(itemManager.item_restbench);
        GameObject.DontDestroyOnLoad(itemManager.item_restbench);
        itemManager.item_shop_go.SetActive(false);
        Log("reload  shop and menu");

    }


    private void BeginScene(On.BossSequenceController.orig_SetupNewSequence orig, BossSequence sequence, BossSequenceController.ChallengeBindings bindings, string playerData)
    {
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
                    if ((CharmHelper.can_stand_equip_charm_scene_names.Contains(itemManager.scenename) || CharmHelper.can_equip_everywhere) && GameInfo.in_rogue)
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
        GameObject itemlist = itemManager.item_menu_go.FindGameObjectInChildren("Item List");
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
            itemManager = null;
        }
    }
    public void Rogue_Start()
    {
        if (itemManager != null)
        {
            GameInfo.Start();
            itemManager.DestroyAllItems();
            itemManager.rewardsStack.Clear();
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.select_small_gift, select = 1, give = 3 });
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.select_small_gift, select = 1, give = 3 });
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.select_small_gift, select = 1, give = 3 });
            itemManager.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.fix_select_big_gift, select = 1, gifts = GameInfo.act_gifts[GiftVariety.huge] });
            itemManager.Next(true);
            itemManager.BeginDisplay();
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


            itemManager.DisplayStates();
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


        Menu setting = new("Settings");
        setting.AddElement(new Satchel.BetterMenus.CustomSlider("font_size".Localize(), (size) => { _set.item_font_size = size; }, () => _set.item_font_size, 0, 10));
        setting.AddElement(new Satchel.BetterMenus.CustomSlider("ui_transparency".Localize(), (alpha) => { _set.UI_alpha = alpha; }, () => _set.UI_alpha, 0, 1));
        setting.AddElement(Blueprints.IntInputField("reroll_num".Localize(), (num) => { _set.reroll_num = num; }, () => _set.reroll_num, 3));
        setting.AddElement(new Satchel.BetterMenus.MenuButton("default", "turn all to default", (but) => { _set.item_font_size = 6.67f; _set.UI_alpha = 0.6f; _set.reroll_num = 3; setting.Update(); }));
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

    public void OnLoadGlobal(setting s)
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

    public setting OnSaveGlobal()
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


}
