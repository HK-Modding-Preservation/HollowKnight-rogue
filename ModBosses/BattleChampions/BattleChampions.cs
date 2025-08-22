
namespace rogue.ModBosses;

internal class BattleChampions : MonoBehaviour
{
    static GameObject false_knight;
    internal const string false_knight_scene = "GG_False_Knight";
    internal const string false_knight_name = "Battle Scene/False Knight New";
    static string ln;
    GameObject fk1;
    GameObject fk2;
    GameObject fk3;
    int damage;
    int whole_damage = 0;
    int whole_hp;
    int head_damage = 0;
    int head_hp;
    bool is_head = false;
    List<HealthManager> wholes = new();
    List<HealthManager> heads = new();
    bool only_fk = true;
    bool death = false;

    internal static void Init(Dictionary<string, Dictionary<string, GameObject>> preloadobjects)
    {
        false_knight = GameObject.Instantiate(preloadobjects[false_knight_scene][false_knight_name]);
        GameObject.DontDestroyOnLoad(false_knight);
        false_knight.SetActive(false);
    }
    void Death()
    {
        if (death == false)
        {
            death = true;
            gameObject.GetComponent<HealthManager>().SendDeathEvent();
        }
    }
    void Start()
    {
        fk1 = Instantiate(false_knight);
        fk2 = Instantiate(false_knight);
        fk1.SetActive(true);
        fk2.SetActive(true);
        fk1.transform.position = transform.position + new Vector3(-5, 0, 0);
        fk2.transform.position = transform.position + new Vector3(5, 0, 0);
        fk1.LocateMyFSM("FalseyControl").ChangeTransition("Idle Pause", "WAIT", "Rage Check");
        fk2.LocateMyFSM("FalseyControl").ChangeTransition("Idle Pause", "WAIT", "Rage Check");
        fk1.LocateMyFSM("FalseyControl").RemoveTransition("Opened", "STUN FAIL");
        fk2.LocateMyFSM("FalseyControl").RemoveTransition("Opened", "STUN FAIL");
        fk1.LocateMyFSM("FalseyControl").InsertCustomAction("Decrement Battle Enemies", Death, 3);
        fk2.LocateMyFSM("FalseyControl").InsertCustomAction("Decrement Battle Enemies", Death, 3);
        gameObject.LocateMyFSM("FalseyControl").InsertCustomAction("Cough", () => { death = true; }, 3);
        gameObject.LocateMyFSM("FalseyControl").RemoveTransition("Opened", "STUN FAIL");
        fk1.FindGameObjectInChildren("Head").LocateMyFSM("Health Check").GetAction<SetHP>("State 1", 4).hp = 99999;
        fk2.FindGameObjectInChildren("Head").LocateMyFSM("Health Check").GetAction<SetHP>("State 1", 4).hp = 99999;
        gameObject.FindGameObjectInChildren("Head").LocateMyFSM("Health Check").GetAction<SetHP>("State 1", 5).hp = 99999;
        fk1.LocateMyFSM("Check Health").GetAction<SetHP>("Stun", 1).hp = 99999;
        fk2.LocateMyFSM("Check Health").GetAction<SetHP>("Stun", 1).hp = 99999;
        gameObject.LocateMyFSM("Check Health").GetAction<SetHP>("Stun", 2).hp = 99999;
        if (only_fk)
        {
            fk3 = Instantiate(false_knight);
            fk3.SetActive(true);
            fk3.transform.position = transform.position;
            fk3.FindGameObjectInChildren("Head").LocateMyFSM("Health Check").GetAction<SetHP>("State 1", 4).hp = 99999;
            fk3.LocateMyFSM("FalseyControl").RemoveTransition("Opened", "STUN FAIL");
            fk3.LocateMyFSM("FalseyControl").FsmVariables.FindFsmFloat("Rage Point X").Value = gameObject.LocateMyFSM("FalseyControl").FsmVariables.FindFsmFloat("Rage Point X").Value;
            fk3.LocateMyFSM("FalseyControl").InsertCustomAction("Decrement Battle Enemies", Death, 3);
            fk3.LocateMyFSM("Check Health").GetAction<SetHP>("Stun", 1).hp = 99999;
            base.gameObject.LocateMyFSM("FalseyControl").enabled = false;
            base.gameObject.LocateMyFSM("Check Health").enabled = false;
            base.gameObject.GetComponent<DamageHero>().enabled = false;
            base.gameObject.transform.SetPositionY(56);
        }
        wholes.Add(only_fk ? fk3.GetComponent<HealthManager>() : base.GetComponent<HealthManager>());
        wholes.Add(fk1.GetComponent<HealthManager>());
        wholes.Add(fk2.GetComponent<HealthManager>());
        foreach (var h in wholes)
        {
            h.hp = 99999;
        }
        whole_hp = wholes.Sum((h) => h.hp);
        heads.Add(only_fk ? fk3.FindGameObjectInChildren("Head").GetComponent<HealthManager>() : base.gameObject.FindGameObjectInChildren("Head").GetComponent<HealthManager>());
        heads.Add(fk1.FindGameObjectInChildren("Head").GetComponent<HealthManager>());
        heads.Add(fk2.FindGameObjectInChildren("Head").GetComponent<HealthManager>());
        heads.ForEach((h) => h.hp = 99999);
        head_hp = heads.Sum((h) => h.hp);



    }


    void Update()
    {
        if (is_head)
        {
            head_damage = head_hp - heads.Sum((h) => h.hp);
            if (head_damage >= 180)
            {

                fk1.FindGameObjectInChildren("Head").LocateMyFSM("Health Check").SendEvent("STUN");
                fk2.FindGameObjectInChildren("Head").LocateMyFSM("Health Check").SendEvent("STUN");
                if (only_fk)
                {
                    fk3.FindGameObjectInChildren("Head").LocateMyFSM("Health Check").SendEvent("STUN");
                }
                else
                {
                    base.gameObject.FindGameObjectInChildren("Head").LocateMyFSM("Health Check").SendEvent("STUN");
                }
                is_head = !is_head;
            }
        }
        if (!is_head)
        {
            whole_damage = whole_hp - wholes.Sum((h) => h.hp);
            if (whole_damage >= 1800)
            {

                fk1.LocateMyFSM("Check Health").SendEvent("ZERO HP");
                fk2.LocateMyFSM("Check Health").SendEvent("ZERO HP");
                if (only_fk)
                {
                    fk3.LocateMyFSM("Check Health").SendEvent("ZERO HP");
                }
                else
                {
                    base.gameObject.LocateMyFSM("Check Health").SendEvent("STUN");
                }
                is_head = !is_head;
            }
        }
    }
    void OnDestroy()
    {
    }
}
