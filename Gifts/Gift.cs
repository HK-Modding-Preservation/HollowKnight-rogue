using System.Runtime.Remoting;
using rogue.Characters;
using UnityEngine.EventSystems;

namespace rogue;
public enum Giftname
{
    role_test,
    charm_fengqun,
    charm_compass,
    charm_chongge,
    charm_yingke,
    charm_baldur,
    charm_wangnu,
    charm_quick_focus,
    charm_blue_heart,
    charm_blue_hexin,
    charm_defender,
    charm_xichong,
    charm_jingji,
    charm_jiaoao,
    charm_wenti,
    charm_chenzhong,
    charm_fengli,
    charm_mogu,
    charm_xiuchang,
    charm_saman,
    charm_bushou,
    charm_shihun,
    charm_zigong,
    charm_xinzang,
    charm_tanlan,
    charm_power,
    charm_dingyao,
    charm_jiaoni,
    charm_wuen,
    charm_fengchao,
    charm_wumeng,
    charm_dash_master,
    charm_kuaipi,
    charm_faniu,
    charm_deep_facus,
    charm_tuibian,
    charm_void_heart,

    charm_feimaotui,

    charm_meng_zhi_dun,
    charm_bian_zhi_zhe,
    charm_wuyou,
    add_2_mask,
    add_1_vessel,
    add_3_notch,
    nail_upgrade,
    get_fireball,
    get_scream,
    get_quake,
    get_dash,
    get_double_jump,
    get_zhua_lei_ding,
    get_one_skill_up,
    get_dash_slash,
    get_upward_slash,
    get_cyclone,
    get_any_spell,
    get_any_nail_art,
    get_any_shift,
    get_a_big_gift,
    get_500_geo,
    get_power,
    get_saman,
    get_quick_and_wenti,
    get_jiaoao,
    get_faniu_and_bushou,
    get_shihun,
    get_kuaiju_shenju_wuen,
    get_badeer_yingke_wuyou,
    get_zigong_bianzhizhe_chongge,
    get_any_2_charms,
    get_one_egg,
    get_xichong,
    get_birthright,
    huge_warrior,
    huge_mage,
    huge_ranger,
    role_nail_master,
    role_shaman,
    role_hunter,
    role_uunn,
    role_joni,
    role_guarder,
    role_moth,
    role_grey_prince,
    role_grimm,
    role_tuk,
    role_defender,
    role_mantis,
    role_collector,
    shop_keeper_key,
    shop_add_1_notch_1,
    shop_add_1_notch_2,
    shop_add_1_notch_3,
    shop_add_1_notch_4,
    shop_nail_upgrade,
    shop_random_gift,
    shop_any_charm_1,
    shop_any_charm_2,
    shop_any_charm_3,
    shop_any_charm_4,
    shop_egg,
    shop_super_dash,
    shop_wall_jump,
    shop_dream_nail,
    shop_add_1_mask,
    shop_add_1_vessel,
    shop_prettey_key,
    shop_refresh,
    shop_acid_swim,
    shop_double_jump,
    shop_dash,
    custom_refresh_dash,
    custom_one_two_dash_doublejump_refresh,
    custom_nail_art_after_dash,
    custom_one_mask_two_blue,
    custom_gliding,
    custom_another_jump,
    custom_equip_charm_everywhere,
    custom_always_parry,
    custom_darkness,
    custom_little_box,
    custom_money_is_power,
    custom_chaos_boss,
    custom_better_fluke,
    custom_double_scream,
    custom_love_key,
    custom_counted_trinket1,
    custom_counted_trinket2,
    custom_counted_trinket3,
    custom_counted_trinket4,
    custom_fast_shadow_dash,
    custom_dont_move_when_damaged,
    custom_better_dream_nail,
    custom_sleep_knight,
    custom_circle_super_dash,
    custom_super_dash_tail

}
public class Gift
{
    public Gift(int level, Func<string> getName = null, Func<string> getDesc = null)
    {
        this.level = level;
        this.getName = getName;
        this.getDesc = getDesc;
    }

    public bool active = true;
    [NonSerialized]
    public int id;
    public int level { get; private set; }
    [NonSerialized]
    public int price;

    [NonSerialized]
    internal Action<Giftname> reward;


    [NonSerialized]
    internal Giftname giftname;

    [NonSerialized]
    internal protected string name = "";

    [NonSerialized]
    internal protected string desc = "";

    [NonSerialized]
    public float weight = 0;

    public float now_weight = 0;

    [NonSerialized]
    public bool showConvo = true;



    [NonSerialized]
    internal protected Func<Sprite> getSprite = null;
    [NonSerialized]
    public bool force_active = true;
    [NonSerialized]
    public Vector2 scale = Vector2.zero;
    Func<string> getName = null;
    Func<string> getDesc = null;
    internal virtual string GetName()
    {
        if (getName != null) return getName();
        return name.Localize();
    }
    internal virtual string GetDesc()
    {
        if (getDesc != null) return getName();
        return desc.Localize();
    }
    internal virtual string GetShowString()
    {
        return GetName();
    }
    internal virtual Sprite GetSprite()
    {
        if (getSprite != null) return getSprite();
        return null;
    }

    internal virtual void GetGift()
    {
        reward?.Invoke(giftname);
    }

    internal virtual void RemoveGift()
    {

    }
    internal virtual void Log(object msg)
    {
        Rogue.Instance.Log(msg);
    }
}

public enum GiftVariety
{
    item,
    huge,
    role,
    shop,
    charm,
    custom
}
internal static class GiftFactory
{
    public static Dictionary<GiftVariety, List<Gift>> all_kind_of_gifts = new();

    public static Dictionary<Giftname, Gift> all_gifts = new();
    public static Action after_update_weight = null;
    private static void Log(object msg)
    {
        Rogue.Instance.Log(msg);
    }
    private static void CharmInit()
    {
        for (int i = 1; i <= 40; i++)
        {
            if (i == 36) continue;
            all_gifts.Add((Giftname)i, new CharmGift((Giftname)i));
        }

    }
    private static void RoleInit()
    {

        all_gifts.Add(Giftname.role_test, new RoleGift<Test>(Giftname.role_test));
        all_gifts.Add(Giftname.role_nail_master, new RoleGift<NailMaster>(Giftname.role_nail_master));
        all_gifts.Add(Giftname.role_shaman, new RoleGift<Shaman>(Giftname.role_shaman));
        all_gifts.Add(Giftname.role_hunter, new RoleGift<Hunter>(Giftname.role_hunter));
        all_gifts.Add(Giftname.role_uunn, new RoleGift<Uunn>(Giftname.role_uunn));
        all_gifts.Add(Giftname.role_joni, new RoleGift<Joni>(Giftname.role_joni));
        all_gifts.Add(Giftname.role_moth, new RoleGift<Moth>(Giftname.role_moth));
        all_gifts.Add(Giftname.role_grey_prince, new RoleGift<GreyPrince>(Giftname.role_grey_prince));
        all_gifts.Add(Giftname.role_tuk, new RoleGift<Tuk>(Giftname.role_tuk));
        all_gifts.Add(Giftname.role_defender, new RoleGift<Defender>(Giftname.role_defender));
        all_gifts.Add(Giftname.role_mantis, new RoleGift<Mantis>(Giftname.role_mantis));
        all_gifts.Add(Giftname.role_collector, new RoleGift<Collector>(Giftname.role_collector));
        Log("role is loaded");
    }
    private static void ShopInit()
    {
        RoleInit();
        all_gifts.Add(Giftname.shop_keeper_key, new Gift(2)
        {
            price = 100,
            reward = (giftname) =>
            {
                ItemManager.Instance.NormalShopItem();
            },
            name = "shop_keeper_key_name",
            desc = "shop_keeper_key_desc",
            active = false,
            weight = 0,
            getSprite = () => SpriteLoader.GetSprite("keeper_key")
        });
        all_gifts.Add(Giftname.shop_add_1_notch_1, new Gift(1)
        {
            price = 50,
            reward = (giftname) =>
            {
                if (PlayerData.instance.charmSlots < 11)
                {
                    PlayerData.instance.charmSlots++;
                }
            },
            name = "shop_add_1_notch_name",
            desc = "shop_add_1_notch_desc",
            active = false,
            weight = 1,
            getSprite = () => SpriteLoader.GetSprite("charm_notch")
        });
        all_gifts.Add(Giftname.shop_nail_upgrade, new Gift(3)
        {
            price = 500,
            reward = (giftname) =>
            {
                GiftHelper.AddNailDamage();
            },
            name = "shop_nail_upgrade_name",
            desc = "shop_nail_upgrade_desc",
            active = false,
            weight = 1,
            getSprite = () => SpriteLoader.GetSprite("pale_stone")
        });
        all_gifts.Add(Giftname.shop_random_gift, new Gift(2)
        {
            price = 400,
            reward = (giftname) =>
            {
                ItemManager.Instance.rewardsStack.Push(new ItemManager.OneReward() { mode = ItemManager.Mode.one_small_gift, give = 1, select = 1 });
                ItemManager.Instance.Next(force: false);
            },
            name = "shop_random_gift_name",
            desc = "shop_random_gift_desc",
            active = false,
            weight = 0,
            getSprite = () => SpriteLoader.GetSprite("witches_eye")

        });
        all_gifts.Add(Giftname.shop_any_charm_1, new Gift(1)
        {
            price = 0,
            reward = (giftname) =>
            {
                // getAnyCharm();
            },
            name = "任意护符",
            desc = "",
            active = true,
            weight = 2
        });
        all_gifts.Add(Giftname.shop_any_charm_2, new Gift(1)
        {
            price = 0,
            reward = (giftname) =>
            {
                // getAnyCharm();
            },
            name = "任意护符",
            desc = "",
            active = true,
            weight = 1.5f,
        });
        all_gifts.Add(Giftname.shop_any_charm_3, new Gift(1)
        {
            price = 0,
            reward = (giftname) =>
            {
                // getAnyCharm();
            },
            name = "任意护符",
            desc = "",
            active = true,
            weight = 0.8f
        });
        all_gifts.Add(Giftname.shop_any_charm_4, new Gift(1)
        {
            price = 0,
            reward = (giftname) =>
            {
                // getAnyCharm();
            },
            name = "任意护符",
            desc = "",
            active = true,
            weight = 0.8f
        });
        all_gifts.Add(Giftname.shop_add_1_notch_2, new Gift(1)
        {
            price = 50,
            reward = (giftname) =>
            {
                if (PlayerData.instance.charmSlots < 11)
                {
                    PlayerData.instance.charmSlots++;
                }
            },
            name = "shop_add_1_notch_name",
            desc = "shop_add_1_notch_desc",
            weight = 1,
            getSprite = () =>
            {
                return all_gifts[Giftname.shop_add_1_notch_1].GetSprite();
            }

        });
        all_gifts.Add(Giftname.shop_add_1_notch_3, new Gift(1)
        {
            price = 50,
            reward = (giftname) =>
            {
                if (PlayerData.instance.charmSlots < 11)
                {
                    PlayerData.instance.charmSlots++;
                }
            },
            name = "shop_add_1_notch_name",
            desc = "shop_add_1_notch_desc",
            weight = 1,
            getSprite = () =>
            {
                return all_gifts[Giftname.shop_add_1_notch_1].GetSprite();
            }
        });
        all_gifts.Add(Giftname.shop_add_1_notch_4, new Gift(1)
        {
            price = 50,
            reward = (giftname) =>
            {
                if (PlayerData.instance.charmSlots < 11)
                {
                    PlayerData.instance.charmSlots++;
                }
            },
            name = "shop_add_1_notch_name",
            desc = "shop_add_1_notch_desc",
            weight = 1,
            getSprite = () =>
            {
                return all_gifts[Giftname.shop_add_1_notch_1].GetSprite();
            }
        });
        all_gifts.Add(Giftname.shop_egg, new Gift(1)
        {
            price = 400,
            reward = (giftname) =>
            {
                GameInfo.revive_num++;
            },
            name = "shop_egg_name",
            desc = "shop_egg_desc",
            weight = 3,
            getSprite = () =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").
                    FindGameObjectInChildren("Inventory").
                    Find("Inv").
                    FindGameObjectInChildren("Equipment").
                    FindGameObjectInChildren("Rancid Egg").
                    GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        all_gifts.Add(Giftname.shop_super_dash, new Gift(1,
         getName: () => Language.Language.Get("INV_NAME_SUPERDASH", "UI"), getDesc: () => Language.Language.Get("INV_DESC_SUPERDASH", "UI"))
        {
            price = 100,
            reward = (giftname) =>
            {
                PlayerData.instance.hasSuperDash = true;
                PlayerData.instance.canSuperDash = true;
            },
            name = Language.Language.Get("INV_NAME_SUPERDASH", "UI"),
            desc = Language.Language.Get("INV_DESC_SUPERDASH", "UI"),
            weight = 0.8f,
            getSprite = () =>
            {
                try
                {
                    return GameCameras.instance.gameObject.
                    FindGameObjectInChildren("HudCamera").
                    FindGameObjectInChildren("Inventory").
                    Find("Inv").
                    FindGameObjectInChildren("Equipment").
                    FindGameObjectInChildren("Super Dash").
                    GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        all_gifts.Add(Giftname.shop_wall_jump, new Gift(1,
        getName: () => Language.Language.Get("INV_NAME_WALLJUMP", "UI"), getDesc: () => Language.Language.Get("INV_DESC_WALLJUMP", "UI"))
        {
            price = 200,
            reward = (giftname) =>
            {
                PlayerData.instance.hasWalljump = true;
                PlayerData.instance.canWallJump = true;
            },
            name = Language.Language.Get("INV_NAME_WALLJUMP", "UI"),
            desc = Language.Language.Get("INV_DESC_WALLJUMP", "UI"),
            weight = 0.8f,
            getSprite = () =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").
                    FindGameObjectInChildren("Inventory").
                    Find("Inv").
                    FindGameObjectInChildren("Equipment").
                    FindGameObjectInChildren("Mantis Claw").
                    GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        all_gifts.Add(Giftname.shop_dream_nail, new Gift(1,
        getName: () => Language.Language.Get("INV_NAME_DREAMNAIL_A", "UI"), getDesc: () => Language.Language.Get("INV_DESC_DREAMNAIL_A", "UI"))
        {
            price = 100,
            reward = (giftname) =>
            {
                PlayerData.instance.hasDreamNail = true;

            },
            name = Language.Language.Get("INV_NAME_DREAMNAIL_A", "UI"),
            desc = Language.Language.Get("INV_DESC_DREAMNAIL_A", "UI"),
            weight = 0.8f,
            getSprite = () =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").
                    FindGameObjectInChildren("Inventory").
                    Find("Inv").
                    FindGameObjectInChildren("Inv_Items").
                    FindGameObjectInChildren("Dream Nail").
                    GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        all_gifts.Add(Giftname.shop_add_1_mask, new Gift(1)
        {
            price = 250,
            reward = (giftname) =>
            {
                GiftHelper.GiveMask();
            },
            name = "shop_add_1_mask_name",
            desc = "shop_add_1_mask_desc",
            weight = 1.5f,
            getSprite = () =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").
                    FindGameObjectInChildren("Inventory").
                    Find("Inv").
                    FindGameObjectInChildren("Inv_Items").
                    FindGameObjectInChildren("Heart Pieces").
                    FindGameObjectInChildren("Pieces 4").
                    GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            },
            scale = new Vector2(0.25f, 0.25f)
        });
        all_gifts.Add(Giftname.shop_add_1_vessel, new Gift(1)
        {
            price = 200,
            reward = (giftname) =>
            {
                GiftHelper.GiveVessel();
            },
            name = "shop_add_1_vessel_name",
            desc = "shop_add_1_vessel_desc",
            weight = 1.5f,
            getSprite = () =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").
                    FindGameObjectInChildren("Inventory").
                    Find("Inv").
                    FindGameObjectInChildren("Inv_Items").
                    FindGameObjectInChildren("Soul Orb").
                    FindGameObjectInChildren("Piece All").
                    GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        all_gifts.Add(Giftname.shop_prettey_key, new Gift(0, getName: () => Language.Language.Get("INV_NAME_WHITEKEY", "UI"))
        {
            price = 8,
            reward = (giftname) =>
            {
                List<string> dreams = new List<string>
                {
                    "pretty_key_Easter_egg_1".Localize(),
                    "pretty_key_Easter_egg_2".Localize(),
                    "pretty_key_Easter_egg_3".Localize(),
                    "pretty_key_Easter_egg_4".Localize(),
                    "pretty_key_Easter_egg_5".Localize(),
                    "pretty_key_Easter_egg_6".Localize(),
                    "pretty_key_Easter_egg_7".Localize(),
                };
                if (GameInfo.pretty_key_num == dreams.Count)
                {
                    Rogue.Instance.ShowDreamConvo(dreams[GameInfo.pretty_key_num - 1]);
                    Rogue.Instance._set.owner = true;
                }
                else if (GameInfo.pretty_key_num < dreams.Count)
                {
                    Rogue.Instance.ShowDreamConvo(dreams[GameInfo.pretty_key_num]);
                    GameInfo.pretty_key_num++;
                    if (GameInfo.pretty_key_num == dreams.Count)
                        Rogue.Instance._set.owner = true;
                }


            },
            name = Language.Language.Get("INV_NAME_WHITEKEY", "UI"),
            desc = "shop_pretty_key_desc",
            weight = 0.1f,
            getSprite = () => SpriteLoader.GetSprite("elegant_key"),
        });
        all_gifts.Add(Giftname.shop_refresh, new Gift(1)
        {
            price = 150,
            reward = (giftname) =>
            {
                GameInfo.refresh_num++;
            },
            name = "shop_refresh_name",
            desc = "shop_refresh_desc",
            weight = 2f,
            getSprite = () => SpriteLoader.GetSprite("easy_key"),
        });
        all_gifts.Add(Giftname.shop_acid_swim, new Gift(1,
        getName: () => Language.Language.Get("INV_NAME_ACIDARMOUR", "UI"), getDesc: () => Language.Language.Get("INV_DESC_ACIDARMOUR", "UI"))
        {
            price = 50,
            reward = (giftname) =>
            {
                PlayerData.instance.hasAcidArmour = true;
            },
            name = Language.Language.Get("INV_NAME_ACIDARMOUR", "UI"),
            desc = Language.Language.Get("INV_DESC_ACIDARMOUR", "UI"),
            weight = 0.8f,
            getSprite = () =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").
                    FindGameObjectInChildren("Inventory").
                    Find("Inv").
                    FindGameObjectInChildren("Equipment").
                    FindGameObjectInChildren("Acid Armour").
                    GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        all_gifts.Add(Giftname.shop_double_jump, new Gift(3,
        getName: () => Language.Language.Get("INV_NAME_DOUBLEJUMP", "UI"), getDesc: () => Language.Language.Get("INV_DESC_DOUBLEJUMP", "UI"))
        {
            price = 600,
            reward = (giftname) =>
            {
                PlayerData.instance.hasDoubleJump = true;
            },
            name = Language.Language.Get("INV_NAME_DOUBLEJUMP", "UI"),
            desc = Language.Language.Get("INV_DESC_DOUBLEJUMP", "UI"),
            weight = 0.4f,
            getSprite = () =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").
                    FindGameObjectInChildren("Inventory").
                    Find("Inv").
                    FindGameObjectInChildren("Equipment").
                    FindGameObjectInChildren("Double Jump").
                    GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
        all_gifts.Add(Giftname.shop_dash, new Gift(3)
        {
            price = 600,
            reward = (giftname) =>
            {
                if (!PlayerData.instance.hasDash)
                {
                    PlayerData.instance.hasDash = true;
                    PlayerData.instance.canDash = true;
                }
                else
                {
                    PlayerData.instance.hasShadowDash = true;
                    PlayerData.instance.canShadowDash = true;
                }
            },
            name = "shop_dash_name",
            desc = "shop_dash_desc",
            weight = 0.4f,
            getSprite = () =>
            {
                try
                {
                    return GameCameras.instance.gameObject.FindGameObjectInChildren("HudCamera").
                    FindGameObjectInChildren("Inventory").
                    Find("Inv").
                    FindGameObjectInChildren("Equipment").
                    FindGameObjectInChildren("Dash Cloak").
                    GetComponent<SpriteRenderer>().sprite;
                }
                catch
                {
                    return null;
                }
            }
        });
    }
    private static void HugeGiftInit()
    {
        all_gifts.Add(Giftname.huge_warrior, new Gift(5)
        {
            reward = (giftname) =>
            {
                GiftHelper.GiveMask();
                GiftHelper.GiveMask();
                GiftHelper.AddNailDamage();

            },
            name = "warrior_name",
            desc = "面具+2&骨钉+1",
            weight = 1f,
        });
        all_gifts.Add(Giftname.huge_mage, new Gift(5)
        {
            reward = (giftname) =>
            {
                GiftHelper.GiveVessel();
                List<Gift> gifts2 = new();
                bool saman = GameInfo.role == CharacterRole.shaman;
                if (PlayerData.instance.screamLevel < 2 || (saman && PlayerData.instance.screamLevel < 3)) gifts2.Add(all_gifts[Giftname.get_scream]);
                if (PlayerData.instance.fireballLevel < 2 || (saman && PlayerData.instance.fireballLevel < 3)) gifts2.Add(all_gifts[Giftname.get_fireball]);
                if (PlayerData.instance.quakeLevel < 2 || (saman && PlayerData.instance.quakeLevel < 3)) gifts2.Add(all_gifts[Giftname.get_quake]);
                ItemManager.Instance.rewardsStack.Push(
                    new ItemManager.OneReward() { mode = ItemManager.Mode.fix_gift, gifts = gifts2 }
                    );
            },
            name = "mage_name",
            desc = "魂槽+1&法术+1",
            weight = 1f,
        });
        all_gifts.Add(Giftname.huge_ranger, new Gift(5)
        {
            reward = (giftname) =>
            {
                List<Gift> gifts1 = new();
                if (!PlayerData.instance.hasCyclone) gifts1.Add(all_gifts[Giftname.get_cyclone]);
                if (!PlayerData.instance.hasUpwardSlash) gifts1.Add(all_gifts[Giftname.get_dash_slash]);
                if (!PlayerData.instance.hasDashSlash) gifts1.Add(all_gifts[Giftname.get_upward_slash]);
                ItemManager.Instance.rewardsStack.Push(
                    new ItemManager.OneReward() { mode = ItemManager.Mode.fix_select_small_gift, select = 1, gifts = gifts1 }
                    );

                List<Gift> gifts2 = new();
                if (all_gifts[Giftname.get_dash].now_weight != 0) gifts2.Add(all_gifts[Giftname.get_dash]);
                if (all_gifts[Giftname.get_zhua_lei_ding].now_weight != 0) gifts2.Add(all_gifts[Giftname.get_zhua_lei_ding]);
                if (all_gifts[Giftname.get_double_jump].now_weight != 0) gifts2.Add(all_gifts[Giftname.get_double_jump]);
                ItemManager.Instance.rewardsStack.Push(
                    new ItemManager.OneReward() { mode = ItemManager.Mode.fix_select_small_gift, select = 1, gifts = gifts2 }
                    );
            },
            name = "ranger_name",
            desc = "剑技+1&位移+1",
            weight = 1f,
        });
    }
    private static void CustomItemInit()
    {
        all_gifts.Add(Giftname.custom_refresh_dash, new RefreshDash());
        all_gifts.Add(Giftname.custom_one_two_dash_doublejump_refresh, new DashDoubleRefresh());
        all_gifts.Add(Giftname.custom_nail_art_after_dash, new NailArtAfterDash());
        all_gifts.Add(Giftname.custom_one_mask_two_blue, new OneMaskTwoBlue());
        all_gifts.Add(Giftname.custom_gliding, new Gliding());
        all_gifts.Add(Giftname.custom_another_jump, new AnotherJump());
        all_gifts.Add(Giftname.custom_equip_charm_everywhere, new EquipCharmEveryWhere());
        all_gifts.Add(Giftname.custom_always_parry, new AlwaysParry());
        all_gifts.Add(Giftname.custom_little_box, new LittleBox());
        all_gifts.Add(Giftname.custom_money_is_power, new MoneyIsPower());
        all_gifts.Add(Giftname.custom_chaos_boss, new ChaosBoss());
        all_gifts.Add(Giftname.custom_better_fluke, new BetterFluke());
        all_gifts.Add(Giftname.custom_double_scream, new DoubleScream());
        all_gifts.Add(Giftname.custom_love_key, new LoveKey());
        all_gifts.Add(Giftname.custom_counted_trinket1, new Trinket1());
        all_gifts.Add(Giftname.custom_counted_trinket2, new Trinket2());
        all_gifts.Add(Giftname.custom_counted_trinket3, new Trinket3());
        all_gifts.Add(Giftname.custom_counted_trinket4, new Trinket4());
        all_gifts.Add(Giftname.custom_fast_shadow_dash, new FastShadowDash());
        all_gifts.Add(Giftname.custom_better_dream_nail, new BetterDreamNail());
        all_gifts.Add(Giftname.custom_sleep_knight, new SleepKnight());
        all_gifts.Add(Giftname.custom_circle_super_dash, new CircleSuperDash());
        all_gifts.Add(Giftname.custom_super_dash_tail, new SuperDashTail());
        // all_gifts.Add(Giftname.custom_darkness, new Darkness());摸黑好难
    }
    private static void ItemInit()
    {
        all_gifts.Add(Giftname.add_2_mask, new Gift(1)
        {
            giftname = Giftname.add_2_mask,
            reward = (giftname) =>
                    {
                        GiftHelper.GiveMask();
                        GiftHelper.GiveMask();
                    },
            name = "add_2_mask_name",
            desc = "add_2_mask_name",
            weight = 1f,
        });
        all_gifts.Add(Giftname.add_1_vessel, new Gift(0)
        {
            giftname = Giftname.add_1_vessel,
            reward = (giftname) =>
                    {
                        GiftHelper.GiveVessel();
                    },
            name = "add_1_vessel_name",
            desc = "add_1_vessel_name",
            weight = 1.1f,
        });
        all_gifts.Add(Giftname.nail_upgrade, new Gift(1)
        {
            giftname = Giftname.nail_upgrade,
            reward = (giftname) =>
            {
                GiftHelper.AddNailDamage();
            },
            name = "nail_upgrade_name",
            desc = "+1骨钉",
            weight = 1f,
        });
        all_gifts.Add(Giftname.add_3_notch, new Gift(0)
        {
            giftname = Giftname.add_3_notch,
            reward = (giftname) =>
            {
                PlayerData.instance.charmSlots += 3;
                if (PlayerData.instance.charmSlots > 11)
                    PlayerData.instance.charmSlots = 11;
            },
            name = "add_3_notch_name",
            desc = "+3护符槽",
            weight = 1.1f,
        });
        all_gifts.Add(Giftname.get_fireball, new Gift(2)
        {
            giftname = Giftname.get_fireball,
            reward = (giftname) =>
            {
                PlayerData.instance.hasSpell = true;
                if (PlayerData.instance.fireballLevel < 2 ||
                (GameInfo.role == CharacterRole.shaman && PlayerData.instance.fireballLevel < 3))
                    PlayerData.instance.fireballLevel++;
            },
            name = "get_fireball_name",
            desc = "获得波法术",
            weight = 0.9f,
        });
        all_gifts.Add(Giftname.get_scream, new Gift(2)
        {
            giftname = Giftname.get_scream,
            reward = (giftname) =>
        {
            PlayerData.instance.hasSpell = true;
            if (PlayerData.instance.screamLevel < 2 ||
                (GameInfo.role == CharacterRole.shaman && PlayerData.instance.screamLevel < 3))
                PlayerData.instance.screamLevel++;
        },
            name = "get_scream_name",
            desc = "获得吼法术",
            weight = 0.9f,
        });
        all_gifts.Add(Giftname.get_quake, new Gift(3)
        {
            giftname = Giftname.get_quake,
            reward = (giftname) =>
                    {
                        PlayerData.instance.hasSpell = true;
                        if (PlayerData.instance.quakeLevel < 2 ||
                (GameInfo.role == CharacterRole.shaman && PlayerData.instance.quakeLevel < 3))
                            PlayerData.instance.quakeLevel++;
                    },
            name = "get_quake_name",
            desc = "获得砸法术",
            weight = 0.8f,
        });
        all_gifts.Add(Giftname.get_dash, new Gift(3)
        {
            giftname = Giftname.get_dash,
            reward = (giftname) =>
        {
            if (PlayerData.instance.hasDash)
            {
                PlayerData.instance.hasShadowDash = true;
                PlayerData.instance.canShadowDash = true;
            }
            else
            {
                PlayerData.instance.hasDash = true;
                PlayerData.instance.canDash = true;
            }
        },
            name = "get_dash_name",
            desc = "获得冲刺",
            weight = 0.8f,
        });
        all_gifts.Add(Giftname.get_double_jump, new Gift(3,
        getName: () => Language.Language.Get("INV_NAME_DOUBLEJUMP", "UI"))
        {
            giftname = Giftname.get_double_jump,
            reward = (giftname) =>
        {
            PlayerData.instance.hasDoubleJump = true;
        },
            name = Language.Language.Get("INV_NAME_DOUBLEJUMP", "UI"),
            desc = "获得二段跳",
            weight = 0.8f,
        });
        all_gifts.Add(Giftname.get_zhua_lei_ding, new Gift(1,
        getName: () => Language.Language.Get("INV_NAME_WALLJUMP", "UI") + "&" + Language.Language.Get("INV_NAME_SUPERDASH", "UI") + "&" + Language.Language.Get("INV_NAME_ACIDARMOUR", "UI") + "&" + Language.Language.Get("INV_NAME_DREAMNAIL_A", "UI"))
        {
            giftname = Giftname.get_zhua_lei_ding,
            reward = (giftname) =>
                    {
                        Log("飞舞大礼包");
                        PlayerData.instance.hasSuperDash = true;
                        PlayerData.instance.hasDreamNail = true;
                        PlayerData.instance.hasAcidArmour = true;
                        PlayerData.instance.hasWalljump = true;
                        PlayerData.instance.canWallJump = true;
                        PlayerData.instance.canSuperDash = true;
                    },
            name = Language.Language.Get("INV_NAME_WALLJUMP", "UI") + "&" + Language.Language.Get("INV_NAME_SUPERDASH", "UI") + "&" + Language.Language.Get("INV_NAME_ACIDARMOUR", "UI") + "&" + Language.Language.Get("INV_NAME_DREAMNAIL_A", "UI"),
            desc = "获得螳螂爪/超冲/酸泪/梦钉",
            weight = 1f,
        });
        all_gifts.Add(Giftname.get_one_skill_up, new Gift(3)
        {
            giftname = Giftname.get_one_skill_up,
            reward = (giftname) =>
        {
            List<Gift> gifts = new();
            if (PlayerData.instance.fireballLevel == 1) gifts.Add(all_gifts[Giftname.get_fireball]);
            if (PlayerData.instance.screamLevel == 1) gifts.Add(all_gifts[Giftname.get_scream]);
            if (PlayerData.instance.quakeLevel == 1) gifts.Add(all_gifts[Giftname.get_quake]);
            if (PlayerData.instance.nailDamage < 21) gifts.Add(all_gifts[Giftname.nail_upgrade]);
            if (PlayerData.instance.hasDash && (!PlayerData.instance.hasShadowDash)) gifts.Add(all_gifts[Giftname.get_dash]);
            Rogue.Instance.itemManager.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
            name = "get_one_skill_up_name",
            desc = "选择一已有法术/冲刺/骨钉升级",
            weight = 0.8f,
        });
        all_gifts.Add(Giftname.get_dash_slash, new Gift(0, getName: () => Language.Language.Get("INV_NAME_ART_UPPER", "UI"))
        {
            reward = (giftname) =>
            {
                PlayerData.instance.hasUpwardSlash = true;
                PlayerData.instance.hasNailArt = true;
            }
            ,
            name = Language.Language.Get("INV_NAME_ART_UPPER", "UI"),
            desc = "获得冲刺劈砍",
            weight = 1.1f,
        });
        all_gifts.Add(Giftname.get_upward_slash, new Gift(0, getName: () => Language.Language.Get("INV_NAME_ART_DASH", "UI"))
        {
            reward = (giftname) =>
                    {
                        PlayerData.instance.hasDashSlash = true;
                        PlayerData.instance.hasNailArt = true;
                    },
            name = Language.Language.Get("INV_NAME_ART_DASH", "UI"),
            desc = "获得强力劈砍",
            weight = 1.1f,
        });
        all_gifts.Add(Giftname.get_cyclone, new Gift(1, getName: () => Language.Language.Get("INV_NAME_ART_CYCLONE", "UI"))
        {
            reward = (giftname) =>
                    {
                        PlayerData.instance.hasCyclone = true;
                        PlayerData.instance.hasNailArt = true;
                    },
            name = Language.Language.Get("INV_NAME_ART_CYCLONE", "UI"),
            desc = "获得旋风劈砍",
            weight = 1f,
        });
        all_gifts.Add(Giftname.get_any_spell, new Gift(3)
        {
            reward = (giftname) =>
        {
            List<Gift> gifts = new() { all_gifts[Giftname.get_fireball], all_gifts[Giftname.get_scream], all_gifts[Giftname.get_quake] };
            ItemManager.Instance.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
            name = "get_any_spell_name",
            desc = "获得任一法术",
            weight = 0f,
            active = false,
        });
        all_gifts.Add(Giftname.get_any_shift, new Gift(3)
        {
            reward = (Giftname) =>
        {
            List<Gift> gifts = new() { all_gifts[Giftname.get_dash], all_gifts[Giftname.get_zhua_lei_ding], all_gifts[Giftname.get_double_jump] };
            ItemManager.Instance.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
            name = "get_any_shift_name",
            desc = "获得任一位移",
            weight = 0f,
            active = false,
        });
        all_gifts.Add(Giftname.get_any_nail_art, new Gift(3)
        {
            reward = (Giftname) =>
        {
            List<Gift> gifts = new() { all_gifts[Giftname.get_dash_slash], all_gifts[Giftname.get_upward_slash], all_gifts[Giftname.get_cyclone] };
            ItemManager.Instance.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.fix_select_small_gift, gifts = gifts });

        },
            name = "get_any_nail_art_name",
            desc = "获得任一剑技",
            weight = 0f,
            active = false
        });
        all_gifts.Add(Giftname.get_a_big_gift, new Gift(4)
        {
            reward = (giftname) =>
            {
                ItemManager.Instance.rewardsStack.Push(new ItemManager.OneReward() { select = 1, mode = ItemManager.Mode.one_big_gift });
                all_gifts[Giftname.get_a_big_gift].now_weight = 0;
            },
            name = "get_a_big_gift_name",
            desc = "随机一个大天赋",
            weight = 0.7f,
        });
        all_gifts.Add(Giftname.get_500_geo, new Gift(1)
        {
            reward = (giftname) =>
            {
                HeroController.instance.AddGeo(500);
                all_gifts[Giftname.get_500_geo].now_weight = 0;
            },
            name = "get_500_geo_name",
            desc = "获得500吉欧",
            weight = 1
        });
        all_gifts.Add(Giftname.get_power, new Gift(2, getName: () => Language.Language.Get("CHARM_NAME_25_G", "UI"))
        {
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_25 = true;
            },
            name = Language.Language.Get("CHARM_NAME_25_G", "UI"),
            desc = "获得坚固力量",
            weight = 0.9f,
        });
        all_gifts.Add(Giftname.get_saman, new Gift(3, getName: () => Language.Language.Get("CHARM_NAME_19", "UI"))
        {
            reward = (giftname) =>
                    {
                        PlayerData.instance.gotCharm_19 = true;
                    },
            name = Language.Language.Get("CHARM_NAME_19", "UI"),
            desc = "获得萨满之石",
            weight = 0.8f,
        });
        all_gifts.Add(Giftname.get_quick_and_wenti, new Gift(2, getName: () => Language.Language.Get("CHARM_NAME_14", "UI") + "&" + Language.Language.Get("CHARM_NAME_32", "UI"))
        {
            reward = (giftname) =>
        {
            PlayerData.instance.gotCharm_14 = true;
            PlayerData.instance.gotCharm_32 = true;

        },
            name = Language.Language.Get("CHARM_NAME_14", "UI") + "&" + Language.Language.Get("CHARM_NAME_32", "UI"),
            desc = "获得稳定之体&快速劈砍",
            weight = 0.9f,
        });
        all_gifts.Add(Giftname.get_jiaoao, new Gift(1, getName: () => Language.Language.Get("CHARM_NAME_13", "UI"))
        {
            reward = (giftname) =>
                    {
                        PlayerData.instance.gotCharm_13 = true;
                    },
            name = Language.Language.Get("CHARM_NAME_13", "UI"),
            desc = "获得骄傲印记",
            weight = 1f,
        });
        all_gifts.Add(Giftname.get_faniu_and_bushou, new Gift(2, getName: () => Language.Language.Get("CHARM_NAME_33", "UI") + "&" + Language.Language.Get("CHARM_NAME_20", "UI"))
        {
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_33 = true;
                PlayerData.instance.gotCharm_20 = true;
            },
            name = Language.Language.Get("CHARM_NAME_33", "UI") + "&" + Language.Language.Get("CHARM_NAME_20", "UI"),
            desc = "获得法术扭曲者&灵魂捕手",
            weight = 0.9f,
        });
        all_gifts.Add(Giftname.get_shihun, new Gift(2, getName: () => Language.Language.Get("CHARM_NAME_21", "UI"))
        {
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_21 = true;
            }
            ,
            name = Language.Language.Get("CHARM_NAME_21", "UI"),
            desc = "获得噬魂者",
            weight = 0.9f,
        });
        all_gifts.Add(Giftname.get_kuaiju_shenju_wuen, new Gift(1, getName: () => Language.Language.Get("CHARM_NAME_7", "UI") + "&" + Language.Language.Get("CHARM_NAME_34", "UI") + "&" + Language.Language.Get("CHARM_NAME_28", "UI"))
        {
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_7 = true;
                PlayerData.instance.gotCharm_34 = true;
                PlayerData.instance.gotCharm_28 = true;
            },
            name = Language.Language.Get("CHARM_NAME_7", "UI") + "&" + Language.Language.Get("CHARM_NAME_34", "UI") + "&" + Language.Language.Get("CHARM_NAME_28", "UI"),
            desc = "获得快速聚集&深度聚集&乌恩之形",
            weight = 1f,
            active = true,
        });
        all_gifts.Add(Giftname.get_badeer_yingke_wuyou, new Gift(0, getName: () => Language.Language.Get("CHARM_NAME_5", "UI") + "&" + Language.Language.Get("CHARM_NAME_4", "UI") + "&" + Language.Language.Get("CHARM_NAME_40_N", "UI"))
        {
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_5 = true;
                PlayerData.instance.gotCharm_4 = true;
                PlayerData.instance.gotCharm_40 = true;
            },
            name = Language.Language.Get("CHARM_NAME_5", "UI") + "&" + Language.Language.Get("CHARM_NAME_4", "UI") + "&" + Language.Language.Get("CHARM_NAME_40_N", "UI"),
            desc = "获得巴德尔纸壳&坚硬外壳&无忧旋律",
            weight = 1.1f,
        });
        all_gifts.Add(Giftname.get_zigong_bianzhizhe_chongge, new Gift(1, getName: () => Language.Language.Get("CHARM_NAME_22", "UI") + "&" + Language.Language.Get("CHARM_NAME_39", "UI") + "&" + Language.Language.Get("CHARM_NAME_3", "UI"))
        {
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_22 = true;
                PlayerData.instance.gotCharm_39 = true;
                PlayerData.instance.gotCharm_3 = true;
            },
            name = Language.Language.Get("CHARM_NAME_22", "UI") + "&" + Language.Language.Get("CHARM_NAME_39", "UI") + "&" + Language.Language.Get("CHARM_NAME_3", "UI"),
            desc = "获得发光子宫&编织者之歌&幼虫之歌",
            weight = 1f,
        });
        all_gifts.Add(Giftname.get_any_2_charms, new Gift(4)
        {
            reward = (giftname) =>
            {
                GameInfo.get_any_charm_num += 2;
                all_gifts[Giftname.get_any_2_charms].now_weight = 0;
            },
            name = "get_any_2_charms_name",
            desc = "自选任意两个护符",
            weight = 0.7f,
        });
        all_gifts.Add(Giftname.get_one_egg, new Gift(1, getName: () => Language.Language.Get("INV_NAME_RANCIDEGG", "UI"))
        {
            reward = (giftname) =>
            {
                GameInfo.revive_num++;
            },
            name = Language.Language.Get("INV_NAME_RANCIDEGG", "UI"),
            desc = "腐臭蛋+1",
            weight = 1f,
        });
        all_gifts.Add(Giftname.get_xichong, new Gift(3, getName: () => Language.Language.Get("CHARM_NAME_11", "UI"))
        {
            reward = (giftname) =>
            {
                PlayerData.instance.gotCharm_11 = true;
            },
            name = Language.Language.Get("CHARM_NAME_11", "UI"),
            desc = "获得吸虫之巢",
            weight = 0.8f,
        });
        all_gifts.Add(Giftname.get_birthright, new Gift(3)
        {
            reward = (giftname) =>
            {
                GameInfo.get_birthright = true;
                Character character = HeroController.instance?.gameObject.GetComponent<Character>();
                character?.SetGetBirthright();
                character?.SelectBirthright();
            },
            name = "birthright",
            desc = "长子权",
            weight = 0,
            active = false,
            force_active = true
        });
        CustomItemInit();

    }
    internal static void Initialize()
    {
        all_gifts.Clear();
        all_kind_of_gifts.Clear();
        ItemInit();
        HugeGiftInit();
        CharmInit();
        ShopInit();
        for (int i = 0; i < Enum.GetValues(typeof(GiftVariety)).Length; i++)
        {
            all_kind_of_gifts.Add((GiftVariety)i, new());
        }
        foreach (var gift in all_gifts)
        {
            gift.Value.giftname = gift.Key;
            gift.Value.active = true;
            string type = gift.Key.ToString();
            if (type.StartsWith("role_")) all_kind_of_gifts[GiftVariety.role].Add(gift.Value);
            else if (type.StartsWith("charm_")) all_kind_of_gifts[GiftVariety.charm].Add(gift.Value);
            else if (type.StartsWith("huge_")) all_kind_of_gifts[GiftVariety.huge].Add(gift.Value);
            else if (type.StartsWith("shop_")) all_kind_of_gifts[GiftVariety.shop].Add(gift.Value);
            else if (type.StartsWith("custom_"))
            {
                all_kind_of_gifts[GiftVariety.custom].Add(gift.Value);
                if (!type.StartsWith("custom_one_two"))
                    all_kind_of_gifts[GiftVariety.item].Add(gift.Value);
            }
            else all_kind_of_gifts[GiftVariety.item].Add(gift.Value);
        }
        ResetWeight();
    }

    public static void UpdateWeight()
    {
        PlayerData playerData = PlayerData.instance;
        all_gifts[Giftname.add_2_mask].SetWeight(playerData.maxHealthBase < 9);
        if (playerData.MPReserveMax >= 99) all_gifts[Giftname.add_1_vessel].now_weight = 0;
        if (playerData.nailDamage >= 21) all_gifts[Giftname.nail_upgrade].now_weight = 0;
        if (playerData.charmSlots >= 11) all_gifts[Giftname.add_3_notch].now_weight = 0;
        if (playerData.fireballLevel == 2) all_gifts[Giftname.get_fireball].now_weight = 0;
        if (playerData.screamLevel == 2) all_gifts[Giftname.get_scream].now_weight = 0;
        if (playerData.quakeLevel == 2) all_gifts[Giftname.get_quake].now_weight = 0;
        if (playerData.hasDash && playerData.hasShadowDash) all_gifts[Giftname.get_dash].now_weight = 0;
        if (playerData.hasDoubleJump) all_gifts[Giftname.get_double_jump].now_weight = 0;
        if (playerData.hasWalljump && playerData.hasSuperDash && playerData.hasAcidArmour && playerData.hasDreamNail) all_gifts[Giftname.get_zhua_lei_ding].now_weight = 0;
        if (playerData.hasUpwardSlash) all_gifts[Giftname.get_dash_slash].now_weight = 0;
        if (playerData.hasDashSlash) all_gifts[Giftname.get_upward_slash].now_weight = 0;
        if (playerData.hasCyclone) all_gifts[Giftname.get_cyclone].now_weight = 0;
        if (playerData.gotCharm_25) all_gifts[Giftname.get_power].now_weight = 0;
        if (playerData.gotCharm_19) all_gifts[Giftname.get_saman].now_weight = 0;
        if (playerData.gotCharm_14 && playerData.gotCharm_32) all_gifts[Giftname.get_quick_and_wenti].now_weight = 0;
        if (playerData.gotCharm_13) all_gifts[Giftname.get_jiaoao].now_weight = 0;
        if (playerData.gotCharm_33 && playerData.gotCharm_20) all_gifts[Giftname.get_faniu_and_bushou].now_weight = 0;
        if (playerData.gotCharm_21) all_gifts[Giftname.get_shihun].now_weight = 0;
        if (playerData.gotCharm_5 && playerData.gotCharm_4 && playerData.gotCharm_40) all_gifts[Giftname.get_badeer_yingke_wuyou].now_weight = 0;
        if (playerData.gotCharm_3 && playerData.gotCharm_22 && playerData.gotCharm_39) all_gifts[Giftname.get_zigong_bianzhizhe_chongge].now_weight = 0;
        if (playerData.gotCharm_11) all_gifts[Giftname.get_xichong].now_weight = 0;
        if (playerData.gotCharm_7 && playerData.gotCharm_28 && playerData.gotCharm_34) all_gifts[Giftname.get_kuaiju_shenju_wuen].now_weight = 0;




        if (playerData.charmSlots >= 11)
        {
            all_gifts[Giftname.shop_add_1_notch_1].now_weight = 0;
            all_gifts[Giftname.shop_add_1_notch_2].now_weight = 0;
            all_gifts[Giftname.shop_add_1_notch_3].now_weight = 0;
            all_gifts[Giftname.shop_add_1_notch_4].now_weight = 0;
        }
        if (playerData.nailDamage >= 21)
        {
            all_gifts[Giftname.shop_nail_upgrade].now_weight = 0;
        }
        if (playerData.hasSuperDash) all_gifts[Giftname.shop_super_dash].now_weight = 0;
        if (playerData.hasWalljump) all_gifts[Giftname.shop_wall_jump].now_weight = 0;
        if (playerData.hasDreamNail) all_gifts[Giftname.shop_dream_nail].now_weight = 0;
        if (playerData.maxHealthBase >= 9) all_gifts[Giftname.shop_add_1_mask].now_weight = 0;
        if (playerData.MPReserveMax >= 99) all_gifts[Giftname.shop_add_1_vessel].now_weight = 0;
        if (playerData.hasAcidArmour) all_gifts[Giftname.shop_acid_swim].now_weight = 0;
        if (playerData.hasDoubleJump) all_gifts[Giftname.shop_double_jump].now_weight = 0;
        if (playerData.hasShadowDash) all_gifts[Giftname.shop_dash].now_weight = 0;


        for (int i = 1; i <= 40; i++)
        {
            if (i == 36) continue;
            all_gifts[(Giftname)i].SetWeight(!playerData.GetBool("gotCharm_" + i));
        }
        int t = CharmHelper.GotCharmNum();
        all_gifts[Giftname.shop_any_charm_1].SetWeight(t <= 39);
        all_gifts[Giftname.shop_any_charm_2].SetWeight(t <= 38);
        all_gifts[Giftname.shop_any_charm_3].SetWeight(t <= 37);
        all_gifts[Giftname.shop_any_charm_4].SetWeight(t <= 36);
        after_update_weight?.Invoke();
    }
    internal static void SetWeight(this Gift gift, bool weight_not_zero)
    {
        if (weight_not_zero) gift.now_weight = gift.weight;
        else gift.now_weight = 0;
    }


    public static void ResetWeight()
    {
        foreach (var gift in all_gifts)
        {
            gift.Value.now_weight = gift.Value.weight;
        }
        // foreach (var item in smallRewards)
        // {
        //     if (item.Value.level == 0) item.Value.weight = 1.1f;
        //     else if (item.Value.level == 1) item.Value.weight = 1f;
        //     else if (item.Value.level == 2) item.Value.weight = 0.9f;
        //     else if (item.Value.level == 3) item.Value.weight = 0.8f;
        //     else if (item.Value.level == 4) item.Value.weight = 0.7f;
        // }
        // shopRewards[giftname.shop_keeper_key].weight = 1;
        // shopRewards[giftname.shop_add_1_notch_1].weight = 1;
        // shopRewards[giftname.shop_add_1_notch_2].weight = 1;
        // shopRewards[giftname.shop_add_1_notch_3].weight = 1;
        // shopRewards[giftname.shop_add_1_notch_4].weight = 1;
        // shopRewards[giftname.shop_nail_upgrade].weight = 1;
        // shopRewards[giftname.shop_random_gift].weight = 1;
        // shopRewards[giftname.shop_any_charm_1].weight = 2;
        // shopRewards[giftname.shop_any_charm_2].weight = 1.5f;
        // shopRewards[giftname.shop_any_charm_3].weight = 0.8f;
        // shopRewards[giftname.shop_any_charm_4].weight = 0.8f;
        // shopRewards[giftname.shop_egg].weight = 3f;
        // shopRewards[giftname.shop_super_dash].weight = 0.8f;
        // shopRewards[giftname.shop_wall_jump].weight = 0.8f;
        // shopRewards[giftname.shop_dream_nail].weight = 0.8f;
        // shopRewards[giftname.shop_add_1_mask].weight = 1.5f;
        // shopRewards[giftname.shop_add_1_vessel].weight = 1.5f;
        // shopRewards[giftname.shop_prettey_key].weight = 0.1f;
        // shopRewards[giftname.shop_refresh].weight = 2f;
        // shopRewards[giftname.shop_acid_swim].weight = 0.8f;
        // shopRewards[giftname.shop_double_jump].weight = 0.4f;
        // shopRewards[giftname.shop_dash].weight = 0.4f;

        // for (int i = 1; i <= 40; i++)
        // {
        //     if (i == 36) continue;
        //     charmRewards[(giftname)i].weight = charm_weight_and_price[(giftname)i].Item1;
        // }
    }




}









