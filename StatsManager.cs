



using System.Diagnostics;

namespace rogue;

internal class StatsData
{

    static readonly List<string> long_battle_scenes = new()
    {
        "GG_Nailmasters",
        "GG_Mantis_Lord",
        "GG_Grimm",
        "GG_Watcher_Knights",
        "GG_Sly",
        "GG_White_Defender",
        "GG_Soul_Tyrant",
        "GG_Grey_Prince_Zote",
        "GG_Failed_Champion",
        "GG_Grimm_Nightmare",
        "GG_Hollow_Knight",
    };
    static readonly List<string> extreme_battle_scenes = new()
    {
        "GG_Radiance"
    };

    internal float last_timer = 0;
    internal float timer = 0;
    internal int num_injured = 0;
    internal int single_scene_num_injured = 0;
    internal int num_hitted = 0;
    internal int num_attack = 0;
    internal int num_nail_arts = 0;
    internal int num_spells = 0;
    bool cal_collector = false;
    bool cal_lost_kin = false;

    internal float score = 0;

    internal void CalScore()
    {
        bool just_update_timer = false;
        if (ItemManager.nobossscene.Contains(ProcessManager.scene_name)) just_update_timer = true;
        float deltatime = timer - last_timer;
        last_timer = timer;
        if (just_update_timer)
            return;
        float res = 0;
        if (IsExtremeBattle(ProcessManager.scene_name))
        {
            res = (float)(500d / (1 + 0.0005d * deltatime + 0.00005d * Math.Pow(deltatime, 2) + 0.0000005d * Math.Pow(deltatime, 3)));
        }
        else if (IsLongBattle(ProcessManager.scene_name))
        {
            res = (float)(500d / (1 + 0.001d * deltatime + 0.0001d * Math.Pow(deltatime, 2) + 0.0000008d * Math.Pow(deltatime, 3)));
        }
        else
        {
            res = (float)(500d / (1 + 0.003d * deltatime + 0.0002d * Math.Pow(deltatime, 2) + 0.0000012d * Math.Pow(deltatime, 3)));
        }
        if (EnemyWaveManager.IsCollectorBranch())
        {
            res += 300;
            if (ProcessManager.scene_name == "GG_Collector_V")
            {
                if (cal_collector) res += 500;
                else cal_collector = true;
            }

        }
        if (EnemyWaveManager.IsLostKinBranch())
        {
            res += 400;
            if (ProcessManager.scene_name == "GG_Lost_Kin")
            {
                if (cal_lost_kin) res += 400;
                else cal_lost_kin = true;
            }

        }
        if (EnemyWaveManager.DontReplacyScene(ProcessManager.scene_name))
        {
            res += 1200;
        }
        if (GameInfo.Branch.radiance && ProcessManager.scene_name == "GG_Radiance")
        {
            res += 2000;
        }
        score += res;

    }
    internal void CalSoreEnd()
    {
        var pd = PlayerData.instance;
        int count1 = 0;
        if (pd.hasDash) count1++;
        if (pd.hasShadowDash) count1++;
        if (pd.hasDoubleJump) count1++;
        if (pd.hasWalljump) count1++;
        if (pd.hasAcidArmour) count1++;
        if (pd.hasDashSlash) count1++;
        if (pd.hasUpwardSlash) count1++;
        if (pd.hasCyclone) count1++;
        if (pd.hasDreamNail) count1++;
        count1 += pd.fireballLevel;
        count1 += pd.screamLevel;
        count1 += pd.quakeLevel;
        score += count1 * 200;
        if (GameInfo.role != Characters.CharacterRole.joni)
        {
            int count2 = 0;
            for (int i = 1; i <= 40; i++)
            {
                if (pd.GetBool("gotCharm_" + i))
                {
                    count2++;
                }
            }
            score += count2 * 200;
        }
        if (GameInfo.role != Characters.CharacterRole.grey_prince)
        {
            score += GiftHelper.GetNailLevel() * 400;
        }
        score += 3 * pd.geo;
        int count3 = 0;
        count3 += pd.maxHealthBase - 5;
        count3 += pd.MPReserveMax / 33;
        score += count3 * 200;
        score += 300 * Mathf.Exp(-0.3f * num_injured);
        score += (float)(100 / (1 + 0.05 * (num_attack + 2 * num_nail_arts + 2 * num_spells) + 0.002f * Mathf.Pow((num_attack + 2 * num_nail_arts + 2 * num_spells), 2)));


    }
    internal void ResetSceneInjured()
    {
        single_scene_num_injured = 0;
    }
    bool IsLongBattle(string scene_name)
    {
        return long_battle_scenes.Contains(scene_name);
    }
    bool IsExtremeBattle(string scene_name)
    {
        return extreme_battle_scenes.Contains(scene_name);
    }
    internal void StartCount()
    {
        On.NailSlash.StartSlash += CountSlash;
        ModHooks.AfterTakeDamageHook += CountInjured;
        ModHooks.TakeDamageHook += CountHitted;
        CountNailArts();
        CountSpells();


    }

    private void CountSpells()
    {
        var fsm = GameObject.Find("Knight").LocateMyFSM("Spell Control");
        if (fsm.GetState("Spell End").Actions.Length == 4)
        {
            fsm.InsertCustomAction("Spell End", () => { num_spells++; }, 4);
        }
    }

    private void CountNailArts()
    {
        var fsm = GameObject.Find("Knight").LocateMyFSM("Nail Arts");
        if (fsm.GetState("Flash").Actions.Length == 1)
        {
            fsm.InsertCustomAction("Flash", () => { num_nail_arts++; }, 1);
        }
        if (fsm.GetState("Flash 2").Actions.Length == 1)
        {
            fsm.InsertCustomAction("Flash 2", () => { num_nail_arts++; }, 1);
        }
        if (fsm.GetState("Dash Slash Ready").Actions.Length == 0)
        {
            fsm.InsertCustomAction("Dash Slash Ready", () => { num_nail_arts++; }, 0);
        }
    }

    private int CountHitted(ref int hazardType, int damage)
    {
        return damage;
    }

    private int CountInjured(int hazardType, int damageAmount)
    {
        if (damageAmount > 0)
        {
            single_scene_num_injured++;
            num_injured++;
        }
        num_hitted++;
        return damageAmount;
    }

    private void CountSlash(On.NailSlash.orig_StartSlash orig, NailSlash self)
    {
        num_attack++;
        orig(self);
    }

    internal void EndCount()
    {
        On.NailSlash.StartSlash -= CountSlash;
        ModHooks.AfterTakeDamageHook -= CountInjured;
        ModHooks.TakeDamageHook -= CountHitted;
        EndCountNailArts();
        EndCountSpells();
    }

    private void EndCountSpells()
    {
        var fsm = GameObject.Find("Knight").LocateMyFSM("Spell Control");
        if (fsm.GetState("Spell End").Actions.Length == 5)
        {
            fsm.RemoveAction("Spell End", 4);
        }
    }

    private void EndCountNailArts()
    {
        var fsm = GameObject.Find("Knight").LocateMyFSM("Nail Arts");
        if (fsm.GetState("Flash").Actions.Length == 2)
        {
            fsm.RemoveAction("Flash", 1);
        }
        if (fsm.GetState("Flash 2").Actions.Length == 2)
        {
            fsm.RemoveAction("Flash 2", 1);
        }
        if (fsm.GetState("Dash Slash Ready").Actions.Length == 1)
        {
            fsm.RemoveAction("Dash Slash Ready", 0);
        }

    }


}