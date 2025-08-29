
namespace rogue.ModBosses;

internal class ChronosDesideria : MonoBehaviour
{
    internal const string broken_vessel_scene = "GG_Broken_Vessel";
    internal const string broken_vessel_boss = "Infected Knight";
    static GameObject broken_vessel;
    GameObject broken_vessel_instance;
    HealthManager hm;
    PlayMakerFSM con;
    PlayMakerFSM ik_con;
    int whole_hp;
    int p1_hp = 2000;
    int p2_hp = 1400;
    bool p2 = false;
    int p3_hp = 800;
    bool p3 = false;

    internal static void Init()
    {
        broken_vessel = PreloadManager.getGO(broken_vessel_scene, broken_vessel_boss);
        GameObject.DontDestroyOnLoad(broken_vessel);
        broken_vessel.SetActive(false);
    }
    void Awake()
    {
        con = base.gameObject.LocateMyFSM("IK Control");
        hm = base.gameObject.GetComponent<HealthManager>();
    }
    void Start()
    {
        ("Start").TestLog();
        broken_vessel_instance = GameObject.Instantiate(broken_vessel);
        P1();
        hm.hp = p1_hp;
        whole_hp = hm.hp;
        ik_con = broken_vessel_instance.LocateMyFSM("IK Control");
        ik_con.RemoveTransition("Intro Land", "FINISHED");
        ik_con.AddCustomAction("Intro Land", () =>
        {
            con.SendEvent("WAITEND");
            ik_con.FsmGlobalTransitions.RemoveAt(0);
            foreach (var state in ik_con.FsmStates)
            {
                int t = state.Transitions.Length;
                for (int i = 0; i < t; i++)
                {
                    state.Transitions = new FsmTransition[0];
                }
            }
        });
        broken_vessel_instance.GetComponent<HealthManager>().hasSpecialDeath = true;
        broken_vessel_instance.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 0.6f);
        broken_vessel_instance.GetComponent<NonBouncer>().SetActive(false);
        ik_con.AddCustomAction("Intro Fall", () =>
        {
            ik_con.GetComponent<Rigidbody2D>().isKinematic = false;
            ik_con.GetComponent<Collider2D>().enabled = true;
            // ik_con.GetComponent<DamageHero>().damageDealt = 0;
            ik_con.GetAction<SetInvincible>("Roar End", 5).Invincible = true;


        });
        hm.OnDeath += () =>
        {
            ik_con.SendEvent("SHAKEEND");
            broken_vessel_instance.GetComponent<HealthManager>().hasSpecialDeath = false;
            broken_vessel_instance.GetComponent<HealthManager>().Die(0, AttackTypes.Generic, true);
        };




    }
    void P1()
    {
        "P1".TestLog();
        con.FsmVariables.FindFsmFloat("Gravity").Value = 4f;
        con.FsmVariables.FindFsmFloat("Dash Speed").Value = 40f;
        // con.GetAction<Wait>("Dash Antic 2", 2).time = 0.2f;
        con.GetAction<RandomFloat>("Set Counter", 0).min = 0.25f;
        con.GetAction<RandomFloat>("Set Counter", 0).max = 0.5f;
        base.gameObject.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL STOP");
        con.FsmVariables.FindFsmBool("Angry").Value = true;
        con.InsertCustomAction("In Air", (fsm) =>
        {
            if (fsm.FsmVariables.FindFsmBool("Did Air Dash").Value)
            {
                fsm.SendEvent("DOWNSTAB");
            }
        }, 8);
        con.GetAction<Wait>("Dstab Recover", 0).time = 0.15f;
        con.ChangeTransition("Dash Recover", "FINISHED", "Ohead Slashing");
        con.GetAction<SendRandomEvent>("To Attack?", 2).weights[0].Value = 0.35f;
        con.GetAction<SendRandomEvent>("To Attack?", 2).weights[1].Value = 1f;
        con.GetAction<SendRandomEvent>("To Attack?", 2).weights[2].Value = 0.65f;
        var wait_state = con.AddState("WaitForP2");
        con.AddTransition("Idle", "P2", "WaitForP2");
        con.InsertCustomAction("Idle", (fsm) =>
        {
            if (hm.hp <= p2_hp && !p2)
            {
                fsm.SendEvent("P2");
            }
            if (hm.hp <= p3_hp && !p3)
            {
                P3();
                fsm.SendEvent("DASH");
            }
        }, 8);
        wait_state.AddCustomAction(() =>
        {
            base.gameObject.LocateMyFSM("Spawn Balloon").SendEvent("STOP SPAWN");
            con.AddCustomAction("First Counter", () => { base.gameObject.LocateMyFSM("Spawn Balloon").SendEvent("START SPAWN"); });
            P2();
            con.AddCustomAction("Roar", () => { base.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero; });

        });
        wait_state.AddTransition("WAITEND", "Roar");






    }

    private void RepeatState(On.HutongGames.PlayMaker.Fsm.orig_SwitchState orig, HutongGames.PlayMaker.Fsm self, HutongGames.PlayMaker.FsmState toState)
    {
        if (self == con.Fsm)
        {
            try
            {
                if (broken_vessel_instance != null)
                    ik_con.SetState(toState.Name);
            }
            catch (Exception e)
            {
                e.TestLog();
            }
        }
        orig(self, toState);
    }

    void P2()
    {
        "P2".TestLog();
        p2 = true;
        broken_vessel_instance.SetActive(true);
        broken_vessel_instance.transform.position = new(base.transform.position.x, 41.82f);
        ik_con.ChangeTransition("Init", "ACTIVE", "Intro Fall");
        On.HutongGames.PlayMaker.Fsm.SwitchState += RepeatState;
    }
    void P3()
    {
        "P3".TestLog();
        p3 = true;
        base.gameObject.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL STOP");
        On.HutongGames.PlayMaker.Fsm.SwitchState -= RepeatState;
        ik_con.AddTransition("Idle", "SHAKE", "Jump Antic 2");
        ik_con.AddTransition("Jump Antic 2", "FINISHED", "Aim Jump 2");
        ik_con.AddTransition("Aim Jump 2", "FINISHED", "Jump 2");
        ik_con.AddTransition("Jump 2", "FINISHED", "In Air 2");
        ik_con.AddTransition("In Air 2", "LAND", "Land 2");
        ik_con.AddTransition("Land 2", "FINISHED", "Shake Antic");
        ik_con.AddTransition("Shake Antic", "FINISHED", "Shaking Start");
        ik_con.AddTransition("Shaking Start", "FINISHED", "Shaking Spawn");
        if (ik_con.GetTransition("Shaking Spawn", "FINISHED") != null)
        {
            ik_con.RemoveTransition("Shaking Spawn", "FINISHED");
        }
        ik_con.AddTransition("Shaking Spawn", "SHAKEEND", "Shake End");
        ik_con.FsmVariables.FindFsmBool("Do Shake").Value = true;
        ik_con.SetState("Idle");
        con.ChangeTransition("Idle", "DASH", "Dash Antic");
        con.ChangeTransition("Dash Attack 3", "FINISHED", "Dash Antic");
        con.GetAction<Wait>("Dash Antic 2", 2).time = 0.6f;
        con.InsertCustomAction("Dash Antic", () =>
        {
            base.gameObject.transform.SetScaleX(HeroController.instance.transform.position.x > base.gameObject.transform.position.x ? 1 : -1);
            base.gameObject.transform.SetPositionY(HeroController.instance.transform.position.y);
        }, 0);

    }
    void OnDestroy()
    {
        if (p2)
        {
            On.HutongGames.PlayMaker.Fsm.SwitchState -= RepeatState;
        }
    }
}