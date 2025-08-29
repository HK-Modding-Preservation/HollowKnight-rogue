using Satchel.Futils;
using UnityEngine.Rendering;

namespace rogue.ModBosses;

internal class GreyKingZote : MonoBehaviour
{
    internal const string grey_king_scene = "GG_Mighty_Zote";
    internal const string turret_name = "Zote Turret";
    internal const string fat_name = "Battle Control/Fat Zotes/Zote Crew Fat (1)";

    static GameObject turret;
    static GameObject fat_zote;
    PlayMakerFSM con;
    HealthManager hm;
    bool p2 = false;
    bool p3 = false;
    List<GameObject> spawns = new();


    internal static void Init()
    {
        turret = PreloadManager.getGO(grey_king_scene, turret_name);
        fat_zote = PreloadManager.getGO(grey_king_scene, fat_name);
    }
    //17   35
    void SpawnTurret(float x)
    {
        GameObject t = GameObject.Instantiate(turret);
        t.SetActive(true);
        t.transform.position = new Vector3(0, 0, 0);
        var fsm = t.LocateMyFSM("Control");
        fsm.GetAction<RandomFloat>("Pos", 0).min = x;
        fsm.GetAction<RandomFloat>("Pos", 0).max = x;
        fsm.GetAction<SetPosition>("Pos", 1).y = 17f;
        fsm.ChangeTransition("Pos", "RETRY", "Appear");
        fsm.GetAction<SetHP>("Appear", 4).hp = 99999;
        fsm.RemoveTransition("Idle", "RETRACT");
        fsm.SendEvent("GO");
        fsm.InsertCustomAction("Fire", (fsm) =>
        {
            fsm.FsmVariables.FindFsmGameObject("Shot").Value.SetActive(true);
        }, 2);
        spawns.Add(t);
    }
    void SpawnFat()
    {
        GameObject f = GameObject.Instantiate(fat_zote);
        f.SetActive(true);
        f.transform.position = new Vector3(0, 0, 0);
        var fsm = f.LocateMyFSM("Control");
        fsm.GetAction<RandomFloat>("Spawn Antic", 0).min = 17f;
        fsm.GetAction<RandomFloat>("Spawn Antic", 0).max = 35f;
        fsm.GetAction<SetHP>("Activate", 3).hp = 99999;
        fsm.AddVariable<FsmFloat>("Hero X");
        fsm.GetAction<FloatCompare>("Dr", 1).float2 = fsm.FsmVariables.FindFsmFloat("Hero X");
        fsm.InsertCustomAction("Dr", (fsm) =>
        {
            fsm.FsmVariables.FindFsmFloat("Hero X").Value = HeroController.instance.transform.position.x;
        }, 1);
        fsm.SendEvent("SPAWN");
        spawns.Add(f);
    }
    void Start()
    {
        con = base.gameObject.LocateMyFSM("Control");
        hm = base.GetComponent<HealthManager>();
        con.GetAction<SetHP>("Level 3", 0).hp = 2100;
        hm.OnDeath += () =>
        {
            foreach (var spawn in spawns)
            {
                spawn.LocateMyFSM("Control").SendEvent("ZERO HP");
            }
        };
    }
    void Update()
    {
        if (!p2 && hm.hp <= 1600)
        {
            p2 = true;
            SpawnTurret(17f);
            SpawnTurret(35f);
        }
        if (!p3 && hm.hp <= 400)
        {
            p3 = true;
            SpawnFat();
            SpawnFat();
            con.ChangeTransition("Move Level", "FINISHED", "Set Jumps");
            con.ChangeTransition("Move Level", "3", "Set Jumps");
            con.GetAction<SendRandomEvent>("Fall Through?", 1).weights[1] = 0;
            con.ChangeTransition("FT Recover", "FINISHED", "Jump Antic");

        }
    }

}