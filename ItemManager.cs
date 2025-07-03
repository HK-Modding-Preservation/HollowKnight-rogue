using UnityEngine.UI;
using rogue.Characters;
using System.ComponentModel;
using Newtonsoft.Json.Serialization;
using rogue.NPCs;
using System.Diagnostics;
using HutongGames.PlayMaker.Ecosystem.Utils;
namespace rogue;

public class ItemManager : MonoBehaviour
{
    internal static ItemManager Instance { get; private set; }
    public ItemManager()
    {
        Instance = this;
    }
    internal const string item_scene = "Tutorial_01";
    internal const string item_name = "_Props/Chest/Item/Shiny Item (1)";
    internal const string shop_scene = "Room_shop";

    internal const string shop_region = "Basement Closed/Shop Region";

    internal const string shop_menu = "Shop Menu";
    public GameObject item;
    public GameObject knight;

    public List<GameObject> items = new();




    private float refreshgap = 1f;
    private float startgap = 3f;

    private float overgap = 3f;

    public TimeSpan timeSpan = TimeSpan.Zero;




    public enum Mode
    {
        one_big_gift,
        one_small_gift,
        select_big_gift,
        select_small_gift,
        fix_gift,
        fix_select_big_gift,
        fix_select_small_gift

    }

    public Mode nowMode;

    public int tempselect;

    Sprite sprite = null;

    public class OneReward
    {
        public Mode mode;
        public int give;

        public int select;

        public List<Gift> gifts;
    }

    internal Stack<OneReward> rewardsStack = new();

    public Dictionary<string, bool> now_scene_items = new Dictionary<string, bool>();

    public GameObject shop_item_temp = null;

    public Action after_revive_action = null;

    internal List<Giftname> roleList = new List<Giftname>
        {
            Giftname.role_nail_master,
            Giftname.role_shaman,
            Giftname.role_hunter,
            Giftname.role_uunn,
            Giftname.role_joni,
            Giftname.role_moth,
            Giftname.role_grey_prince,
            Giftname.role_tuk,
            // Giftname.defender,
            Giftname.role_mantis,
            Giftname.role_collector
        };
    internal static List<string> nobossscene = new List<string> { "GG_Spa", "GG_Engine", "GG_Unn", "GG_Engine_Root", "GG_Wyrm", "GG_Atrium_Roof" };

    public string rouge_introduction = "rogue_introduction".Localize();

    public bool hatchscene = false;

    Action<PlayMakerFSM> fsm_enable = null;
    public Func<OneReward, OneReward> before_spawn_item = null;

    System.Random item_random;
    internal static GameObject menu_go;
    internal static GameObject shop_go;

    internal static void Init()
    {
        menu_go = PreloadManager.getGO(shop_scene, shop_menu);
        shop_go = PreloadManager.getGO(shop_scene, shop_region);
        shop_go.AddComponent<SpriteRenderer>().sprite = PreloadManager.getGO(shop_scene, RogueSceneManager.shop_counter).GetComponent<SpriteRenderer>().sprite;
        shop_go.SetActive(false);
        Rogue.Instance.rogue_go.AddComponent<ItemManager>();
        menu_go = null;
        shop_go = null;
    }
    static void AdjustMenuGO(GameObject menu_go)
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
                    GameInfo.got_items.Add(type);
                    DisplayManager.DisplayStates();
                    if (GiftFactory.all_gifts[type].showConvo) Rogue.Instance.ShowConvo(GiftFactory.all_gifts[type].GetShowString());
                }
                GiftFactory.UpdateWeight();
            }, 1);
        }
    }
    static void AdjustShopGo(GameObject shop_go)
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
        RogueSceneManager.PrePareShopGO(shop_go);
    }
    internal static void GameLoadInit()
    {
        if (menu_go != null)
        {
            menu_go.SetActive(false);
            DestroyImmediate(menu_go);
        }
        menu_go = Instantiate(PreloadManager.getGO(shop_scene, shop_menu));
        AdjustMenuGO(menu_go);
        DontDestroyOnLoad(menu_go);
        menu_go.SetActive(false);

        if (shop_go != null)
        {
            shop_go.SetActive(false);
            DestroyImmediate(shop_go);
        }
        shop_go = Instantiate(PreloadManager.getGO(shop_scene, shop_region));
        AdjustShopGo(shop_go);
        DontDestroyOnLoad(shop_go);
        shop_go.SetActive(false);



    }

    public void Start()
    {
        Modding.ModHooks.LanguageGetHook += OnLanguageGet;
        On.PlayerData.GetBool += OnGetBool;
        On.PlayerData.SetBool += OnSetBool;
        ModHooks.CharmUpdateHook += OnCharmUpdate;
        On.PlayMakerFSM.OnEnable += OnFSMEnable;
        ModHooks.TakeHealthHook += Revive;
        item_random = new();
    }
    public void OnDestroy()
    {
        ModHooks.LanguageGetHook -= OnLanguageGet;
        On.PlayerData.GetBool -= OnGetBool;
        On.PlayerData.SetBool -= OnSetBool;
        ModHooks.CharmUpdateHook -= OnCharmUpdate;
        On.PlayMakerFSM.OnEnable -= OnFSMEnable;
        ModHooks.TakeHealthHook -= Revive;

    }

    private int Revive(int damage)
    {
        if (GameInfo.revive_num > 0 && GameInfo.in_rogue && BossSequenceController.IsInSequence)
        {
            if (((PlayerData.instance.health + PlayerData.instance.healthBlue) <= damage) || (PlayerData.instance.equippedCharm_27 && (PlayerData.instance.healthBlue <= damage)))
            {
                GameInfo.revive_num--;
                after_revive_action?.Invoke();
                GiftFactory.UpdateWeight();
                DisplayManager.DisplayStates();
                return 0;
            }
        }
        return damage;

    }

    private void OnCharmUpdate(PlayerData data, HeroController controller)
    {
        DisplayManager.DisplayEquipped();
        GiftHelper.UpdateCharmsEffects();
    }

    private void OnSetBool(On.PlayerData.orig_SetBool orig, PlayerData self, string boolName, bool value)
    {
        if (now_scene_items.ContainsKey(boolName))
        {
            now_scene_items[boolName] = value;
            return;
        }
        else orig(self, boolName, value);
    }

    private bool OnGetBool(On.PlayerData.orig_GetBool orig, PlayerData self, string boolName)
    {

        if (boolName == "blueRoomActivated") return true;
        else if (boolName == "blueRoomDoorUnlocked") return true;
        if (now_scene_items.ContainsKey(boolName))
        {
            return now_scene_items[boolName];
        }
        else
        {
            return orig(self, boolName);
        }
    }

    private string OnLanguageGet(string key, string sheetTitle, string orig)
    {
        if (key == null) return "key is null";
        if (sheetTitle == "Enemy Dreams" && key.StartsWith("rogue_"))
        {
            return key.Split('_')[1];
        }
        if (sheetTitle == "rouge" && key == "rouge_introduction") return rouge_introduction;
        if (sheetTitle == "Titles" && key == "rouge_introduction_MAIN") return "Gou_bro_MAIN".Localize();
        if (sheetTitle == "Titles" && key == "rouge_introduction_SUB") return "Gou_bro_SUB".Localize();
        if (sheetTitle == "Titles" && key == "rouge_introduction_SUPER") return "Gou_bro_SUPER".Localize();
        else return orig;
    }



    GameObject Gift2ShopItem(Gift gift, ref int count, bool select = false)
    {
        const string selectname = "rogue_select";
        if (shop_item_temp == null)
        {
            shop_item_temp = Instantiate(menu_go.GetComponent<ShopMenuStock>().stock[0]);
            shop_item_temp.name = "shop_item_temp";
            var shopitem = shop_item_temp.GetComponent<ShopItemStats>();
            shopitem.specialType = 0;
            shopitem.charmsRequired = 0;
            shopitem.nameConvo = "物品名称";
            shopitem.descConvo = "物品描述";
            shopitem.priceConvo = "0";
            shop_item_temp.SetActive(false);
            DontDestroyOnLoad(shop_item_temp);
        }
        List<Giftname> charms = new(){
                Giftname.shop_any_charm_1,
                Giftname.shop_any_charm_2,
                Giftname.shop_any_charm_3,
                Giftname.shop_any_charm_4
            };
        if (charms.Contains(gift.giftname)) return null;
        GameObject good = Instantiate(shop_item_temp);
        var goodstats = good.GetComponent<ShopItemStats>();
        var render = good.FindGameObjectInChildren("Item Sprite").GetComponent<SpriteRenderer>();
        good.name = gift.GetName();
        goodstats.nameConvo = gift.GetName();
        goodstats.descConvo = gift.GetDesc();
        goodstats.priceConvo = gift.price.ToString();
        goodstats.specialType = (int)gift.giftname + 18;
        goodstats.cost = gift.price;
        render.sprite = gift.GetSprite();
        var item = good.FindGameObjectInChildren("Item Sprite");
        if (gift.scale != Vector2.zero) item.transform.localScale = gift.scale;
        if (select) goodstats.playerDataBoolName = selectname;
        else
            goodstats.playerDataBoolName = gift.giftname.ToString() + count;
        count++;
        if (select)
        {
            if (!now_scene_items.ContainsKey(selectname))
                now_scene_items.Add(selectname, false);
        }
        else
        {
            now_scene_items.Add(goodstats.playerDataBoolName, false);
        }
        return good;

    }
    public void SpwanShopItem(List<Gift> gifts, bool select = false)
    {

        List<GameObject> shop_items = new();
        now_scene_items.Clear();
        int count = 0;
        foreach (var gift in gifts)
        {
            var item = Gift2ShopItem(gift, ref count, select);
            if (item != null)
                shop_items.Add(item);
        }

        var itemlist = menu_go.FindGameObjectInChildren("Item List");
        var menustock = itemlist.GetComponent<ShopMenuStock>();
        var master_menu_stock = menu_go.GetComponent<ShopMenuStock>();

        master_menu_stock.stock = shop_items.ToArray();
        menustock.stock = shop_items.ToArray();
        Rogue.Instance.shop_items.Clear();
        foreach (var shop_item in shop_items)
        {
            Rogue.Instance.shop_items.Add(shop_item, shop_item);
        }
        ReflectionHelper.SetField(menu_go.GetComponent<ShopMenuStock>(), "spawnedStock", Rogue.Instance.shop_items);
        ReflectionHelper.SetField(menustock, "spawnedStock", Rogue.Instance.shop_items);

    }
    public void StartShopItem()
    {
        List<Gift> roles = new();
        foreach (var role in roleList)
        {
            roles.Add(GiftFactory.all_gifts[role]);
        }
        if (Rogue.Instance._set.owner) roles = roles.Append(GiftFactory.all_gifts[Giftname.role_test]).ToList();
        SpwanShopItem(roles, select: true);

    }
    public void NormalShopItem()
    {
        GiftFactory.UpdateWeight();
        List<Gift> items = new List<Gift>();
        items.Add(GiftFactory.all_gifts[Giftname.shop_keeper_key]);
        if (GiftFactory.all_gifts[Giftname.shop_add_1_notch_1].weight > 0)
            items.Add(GiftFactory.all_gifts[Giftname.shop_add_1_notch_1]);
        if (GiftFactory.all_gifts[Giftname.shop_nail_upgrade].weight > 0)
            items.Add(GiftFactory.all_gifts[Giftname.shop_nail_upgrade]);
        items.Add(GiftFactory.all_gifts[Giftname.shop_random_gift]);
        List<Giftname> charms = new(){
                Giftname.shop_any_charm_1,
                Giftname.shop_any_charm_2,
                Giftname.shop_any_charm_3,
                Giftname.shop_any_charm_4
            };
        var ranlist = RandomList(GameInfo.act_gifts[GiftVariety.shop], num: 9 - items.Count);
        int t = ranlist.Count((gift) => charms.Contains(gift.giftname));
        ranlist.RemoveAll((gift) => charms.Contains(gift.giftname));
        List<Gift> select_charms = RandomList(GameInfo.act_gifts[GiftVariety.charm], t, false);
        foreach (var charm in charms) GiftFactory.all_gifts[charm].now_weight = 0;
        ranlist = RandomList(GameInfo.act_gifts[GiftVariety.shop], 9 - items.Count - select_charms.Count, false);
        SpwanShopItem(items.Concat(ranlist).Concat(select_charms).ToList());


    }
    public void CharmShopItem()
    {
        GiftFactory.UpdateWeight();
        List<Gift> items = new List<Gift>();
        items.Add(GiftFactory.all_gifts[Giftname.shop_add_1_notch_1]);
        items.Add(GiftFactory.all_gifts[Giftname.shop_add_1_notch_2]);
        items.Add(GiftFactory.all_gifts[Giftname.shop_add_1_notch_3]);
        var ranlist = RandomList(GameInfo.act_gifts[GiftVariety.charm], 6);
        SpwanShopItem(items.Concat(ranlist).ToList());
    }
    private IEnumerator DelayGenerater(float delay)
    {
        yield return new WaitForSeconds(delay);
        Next(true);
    }

    public void DestroyAllItems()
    {
        foreach (GameObject item in items)
        {
            if (item != null) DestroyImmediate(item);
        }
        items.Clear();
    }

    public void GiveReward(int door)
    {
        rewardsStack.Push(new OneReward
        {
            mode = Mode.select_small_gift,
            select = 1,
            give = 3,
            gifts = null
        });
        if (door % 2 == 0)
        {
            rewardsStack.Push(new OneReward
            {
                mode = Mode.fix_select_big_gift,
                select = 1,
                gifts = GameInfo.act_gifts[GiftVariety.huge]
            });
        }
        StartCoroutine(DelayGenerater(1f));
    }


    public void Update()
    {
        refreshgap -= Time.deltaTime;
        overgap -= Time.deltaTime;
        startgap -= Time.deltaTime;
        if (HeroController.instance == null) return;
        if (Rogue.self_actions.refresh.IsPressed)
        {
            if (GameInfo.refresh_num > 0)
            {
                if (refreshgap < 0f)
                {
                    if (items.Count > 0)
                    {
                        refreshgap = 0.75f;
                        int t = items.Count;

                        switch (nowMode)
                        {
                            case Mode.select_small_gift:
                                DestroyAllItems();
                                GameInfo.refresh_num--;
                                RandomSelectSmall(tempselect, t);
                                break;
                            case Mode.select_big_gift:
                                DestroyAllItems();
                                GameInfo.refresh_num--;
                                RandomSelectBig(tempselect, t);
                                break;
                            case Mode.one_big_gift:
                                DestroyAllItems();
                                GameInfo.refresh_num--;
                                RandomOneBig();
                                break;
                            case Mode.one_small_gift:
                                DestroyAllItems();
                                GameInfo.refresh_num--;
                                RandomOneSmall();
                                break;
                            case Mode.fix_gift:
                                break;
                            case Mode.fix_select_big_gift:
                                break;
                            case Mode.fix_select_small_gift:
                                break;
                            default:
                                break;

                        }
                    }
                }
            }
        }
        if (Rogue.self_actions.start.IsPressed && startgap < 0)
        {
            startgap = 1f;
            Rogue.Instance.Rogue_Reset();
            Rogue.Instance.Rogue_Start();
            DisplayManager.DisplayEquipped();
            DisplayManager.DisplayStates();
            if (ProcessManager.scene_name == "GG_Atrium_Roof")
            {
                StartShopItem();
                SetShop(false);
                RogueSceneManager.ResetAll();
                RogueSceneManager.SetBench();
                RogueSceneManager.SetGou();
            }
        }
        if (Rogue.self_actions.over.IsPressed && overgap < 0)
        {
            overgap = 1f;
            Rogue.Instance.Rogue_Over();

        }

    }




    private void OnFSMEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "FSM" && self.gameObject.name == "blue stuff")
        {
            self.GetAction<IntCompare>("Get Bindings", 1).integer2 = 0;
        }
        else if (self.FsmName == "Conversation Control" && self.gameObject.name == "Godseeker EngineRoom NPC")
        {

            if (self.GetState("End").Actions.Length == 2)
            {
                self.InsertCustomAction("End", () =>
                {
                    if (GameInfo.in_rogue && !GameInfo.get_birthright)
                    {
                        GiftFactory.all_gifts[Giftname.get_birthright].GetGift();
                    }
                }, 1);
            }
        }
        fsm_enable?.Invoke(self);
        orig(self);

    }

    public List<Gift> RandomList(List<Gift> gifts, int num = 1, bool canRepeat = false, System.Random random = null)
    {
        if (random == null)
        {
            random = this.item_random;
        }
        List<Gift> res = new();
        float whole = gifts.Sum((gift) =>
        {
            return gift.now_weight;
        });
        if (whole == 0) return res;
        int count = gifts.Count((gift) =>
        {
            return gift.now_weight > 0;
        });
        if (count < num)
        {
            res = gifts.Where((gift) =>
            {
                return gift.now_weight > 0;
            }).ToList();
            return res;
        }
        int index = 0;
        while (res.Count < num)
        {
            do
            {
                float now = 0;
                index = 0;
                while (now == 0) now = (float)random.NextDouble() * whole;
                while (now > 0)
                {
                    now -= gifts[index].now_weight;
                    index += 1;
                }
            }
            while (!canRepeat && res.Contains(gifts[index - 1]));
            res.Add(gifts[index - 1]);
        }
        return res;




    }
    public void RandomOneSmall()
    {
        items.Clear();
        nowMode = Mode.one_small_gift;
        int count = GameInfo.act_gifts[GiftVariety.item].Count;
        if (count <= 0) return;
        var list = RandomList(GameInfo.act_gifts[GiftVariety.item], 1);
        if (list.Count > 0)
            SpwanItem(list[0]);
    }
    public void RandomOneBig()
    {
        items.Clear();
        nowMode = Mode.one_big_gift;
        int count = GameInfo.act_gifts[GiftVariety.huge].Count;
        if (count <= 0) return;
        var list = RandomList(GameInfo.act_gifts[GiftVariety.huge], 1);
        if (list.Count > 0)
            SpwanItem(list[0]);
    }
    public void RandomSelectSmall(int select, int give)
    {
        items.Clear();
        nowMode = Mode.select_small_gift;
        tempselect = select;
        int count = GameInfo.act_gifts[GiftVariety.item].Count;
        if (count <= 0) return;
        var list = RandomList(GameInfo.act_gifts[GiftVariety.item], give);
        foreach (var item in list)
        {
            SpwanItem(item);
        }


    }
    public void RandomSelectBig(int select, int give)
    {
        items.Clear();
        nowMode = Mode.select_big_gift;
        tempselect = select;
        int count = GameInfo.act_gifts[GiftVariety.huge].Count;
        if (count <= 0) return;
        var list = RandomList(GameInfo.act_gifts[GiftVariety.huge], give);
        foreach (var item in list)
        {
            SpwanItem(item);
        }


    }

    public void FixGifts(List<Gift> gifts)
    {
        items.Clear();
        nowMode = Mode.fix_gift;
        foreach (var gift in gifts)
        {
            SpwanItem(gift);
        }
    }

    public void FixSelectBig(List<Gift> bigs, int select)
    {
        items.Clear();
        nowMode = Mode.fix_select_big_gift;
        tempselect = select;
        foreach (var gift in bigs)
        {
            SpwanItem(gift, true);
        }

    }
    public void FixSelectSmall(List<Gift> smalls, int select)
    {
        items.Clear();
        nowMode = Mode.fix_select_big_gift;
        tempselect = select;
        foreach (var gift in smalls)
        {
            SpwanItem(gift, true);
        }

    }
    private void SpwanItem(Gift gift, bool inSelect = false)
    {
        if (gift == null) return;
        SpwanItem(gift.level, gift.GetName(), gift.giftname);
        return;

    }


    private void SpwanItem(int level, string name, Giftname giftname)
    {
        Log(giftname.ToString() + " spawn");
        knight = GameObject.Find("Knight");
        item = PreloadManager.getGO(item_scene, item_name);
        if (knight != null)
        {
            var newitem = Instantiate(item);
            if (ProcessManager.scene_name == "GG_Atrium_Roof")
            {
                newitem.transform.SetPosition2D(110, 63);
            }
            else if (ProcessManager.scene_name == "GG_Spa")
            {
                newitem.transform.SetPosition2D(85, 16);
            }
            else
            {
                newitem.transform.position = knight.transform.position;
            }

            bool flag = true;
            if (ProcessManager.scene_name == "GG_Spa")
            {
                var spa_pos = new List<Vector2> { new Vector2(83, 16), new Vector2(85, 16), new Vector2(87, 16) };

                foreach (var try_pos in spa_pos)
                {
                    newitem.transform.position = try_pos;
                    bool spaflag = true;
                    foreach (var item in items)
                    {
                        if (Math.Abs(item.transform.position.x - newitem.transform.position.x) < 0.5f)
                        {
                            spaflag = false;
                            break;
                        }
                    }
                    if (spaflag)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            while (flag)
            {
                var pos = newitem.transform.position;
                pos.x += Random.Range(-2f, 2f);
                newitem.transform.position = pos;
                flag = false;
                foreach (var item in items)
                {
                    if (Math.Abs(item.transform.position.x - newitem.transform.position.x) < 2f)
                    {
                        flag = true;
                        break;
                    }
                }

            }

            switch (level)
            {
                case 0:
                    newitem.GetComponent<SpriteRenderer>().color = Color.white;
                    break;
                case 1:
                    newitem.GetComponent<SpriteRenderer>().color = Color.blue;
                    break;
                case 2:
                    newitem.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 0.5f);
                    break;
                case 3:
                    newitem.GetComponent<SpriteRenderer>().color = new Color(1f, 0.843f, 0);
                    break;
                case 4:
                    newitem.GetComponent<SpriteRenderer>().color = Color.red;
                    break;
                case 5:
                    newitem.GetComponent<SpriteRenderer>().color = Color.black;
                    break;
                default:
                    break;

            }
            var fsm = newitem.LocateMyFSM("Shiny Control");
            var reward = fsm.CopyState("Trink 1", "Reward");
            fsm.ChangeTransition("PD Bool?", "COLLECTED", "Fling?");
            fsm.ChangeTransition("Trink Flash", "FINISHED", "Reward");
            fsm.ChangeTransition("Init", "ACTIVATE", "PD Bool?");
            fsm.GetAction<BoolTest>("Init", 2).isTrue = null;
            reward.GetAction<SetSpriteRendererSprite>(2).sprite = sprite;
            reward.GetAction<SetTextMeshProText>(4).textString = name;
            reward.InsertCustomAction(() =>
            {
                Log(giftname);
                GiftFactory.all_gifts[giftname].GetGift();
                GameInfo.got_items.Add(giftname);
                GiftFactory.UpdateWeight();
                DisplayManager.DisplayStates();
            }, 6);
            reward.RemoveAction(0);
            reward.RemoveAction(0);
            fsm.FsmVariables.GetFsmBool("Charm").Value = false;
            fsm.FsmVariables.GetFsmInt("Trinket Num").Value = 1;
            fsm.FsmVariables.GetFsmBool("Fling On Start").Value = false;
            var waitstate = fsm.CopyState("Fling Bubble", "wait state");
            waitstate.RemoveAction(0);
            waitstate.InsertAction(new Wait() { time = 0.2f, finishEvent = FsmEvent.Finished }, 0);
            waitstate.ChangeTransition("FINISHED", "Idle");
            fsm.ChangeTransition("Fling?", "FINISHED", "wait state");
            fsm.FsmVariables.GetFsmString("Tag").Value = "Boss";
            fsm.gameObject.FindGameObjectInChildren("Inspect Region").LocateMyFSM("inspect").FsmVariables.FindFsmBool("Inspectable").Value = false;
            fsm.InsertCustomAction("Idle", (fsm) =>
            {
                GameObject temp = fsm.gameObject;
                List<string> strings = new() { "Inspect Region", "Arrow Prompt(Clone)", "Labels", "Inspect" };
                foreach (string s in strings)
                {
                    temp = temp.FindGameObjectInChildren(s);
                    if (temp == null) { Log("null is " + s); return; }
                }
                temp.GetComponent<TMPro.TextMeshPro>().text = name;
                temp.GetComponent<TMPro.TextMeshPro>().fontSize = Rogue.Instance._set.item_font_size;
            }, 2);


            fsm.InsertCustomAction("Finish", () =>
            {
                Next();
            }, 8);
            fsm.GetAction<Wait>("Trink Pause", 0).time = 0.2f;


            newitem.SetActive(true);
            // newitem.AddComponent<Test>();
            newitem.LocateMyFSM("Shiny Control").SetState("Pause");
            items.Add(newitem);
        }

    }


    public void Next(bool start = false, bool force = true)
    {
        bool flag = false;
        if (!start)
        {
            switch (nowMode)
            {
                case Mode.select_small_gift:
                    tempselect--;
                    if (tempselect <= 0) flag = true;
                    break;
                case Mode.select_big_gift:
                    tempselect--;
                    if (tempselect <= 0) flag = true;
                    break;
                case Mode.one_big_gift:
                    flag = true;
                    break;
                case Mode.one_small_gift:
                    flag = true;
                    break;
                case Mode.fix_gift:
                    flag = true;
                    break;
                case Mode.fix_select_big_gift:
                    tempselect--;
                    if (tempselect <= 0) flag = true;
                    break;
                case Mode.fix_select_small_gift:
                    tempselect--;
                    if (tempselect <= 0) flag = true;
                    break;
            }
        }
        if (items.Count > 0)
        {
            if (!force) return;
            else
            {
                flag = true;
            }
        }
        if (flag || start)
        {
            DestroyAllItems();
            if (rewardsStack.Count > 0)
            {
                OneReward reward;
                do
                {
                    reward = rewardsStack.Pop();
                }
                while (FixEmptyReward(reward) && rewardsStack.Count > 0);
                if (FixEmptyReward(reward)) return;
                if (before_spawn_item != null)
                {
                    reward = before_spawn_item(reward);
                }
                switch (reward.mode)
                {
                    case Mode.select_small_gift:
                        RandomSelectSmall(reward.select, reward.give);
                        break;
                    case Mode.select_big_gift:
                        RandomSelectBig(reward.select, reward.give);
                        break;
                    case Mode.one_big_gift:
                        RandomOneBig();
                        break;
                    case Mode.one_small_gift:
                        RandomOneSmall();
                        break;
                    case Mode.fix_gift:
                        FixGifts(reward.gifts);
                        break;
                    case Mode.fix_select_big_gift:
                        FixSelectBig(reward.gifts, reward.select);
                        break;
                    case Mode.fix_select_small_gift:
                        FixSelectSmall(reward.gifts, reward.select);
                        break;
                    default:
                        break;
                }
            }
        }

    }

    private bool FixEmptyReward(OneReward reward)
    {
        if ((reward.mode == Mode.fix_gift || reward.mode == Mode.fix_select_small_gift || reward.mode == Mode.fix_select_big_gift) && reward.gifts.Count <= 0) return true;
        return false;
    }

    private void Log(object msg)
    {

        Rogue.Instance.Log(msg);

    }
    private string TimerText()
    {
        //Modding.Logger.Log("now");
        return string.Format(
            "\n{0}:{1:D2}:{2:D3}",
            timeSpan.Minutes,
            timeSpan.Seconds,
            timeSpan.Milliseconds
            );
    }
    internal void SetNoShop()
    {
        menu_go.SetActive(false);
        shop_go.SetActive(false);
    }

    internal void SetShop(bool is_spa)
    {
        menu_go.SetActive(true);
        if (!is_spa)
        {
            shop_go.transform.SetPosition2D(129.9f, 63);
            shop_go.LocateMyFSM("Shop Region").FsmVariables.GetFsmFloat("Move To X").Value = 132f;
        }
        else
        {
            shop_go.transform.SetPosition2D(78, 16);
            shop_go.LocateMyFSM("Shop Region").FsmVariables.GetFsmFloat("Move To X").Value = 78f;
            shop_go.SetActive(true);
        }
        shop_go.SetActive(true);

    }
}
