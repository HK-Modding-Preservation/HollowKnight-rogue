
namespace rogue.ModBosses;

internal class BattleChampions : MonoBehaviour
{
    static GameObject false_knight_go;

    internal static void Init(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {

        false_knight_go = Instantiate(preloadedObjects[PreloadManager.false_knight_scene][PreloadManager.false_knight_name]);
        GameObject.DontDestroyOnLoad(false_knight_go);
        false_knight_go.SetActive(false);
    }
    GameObject false1 = null;
    GameObject false2 = null;
    int stun_amount = 0;
    int damage_amount = 0;
    void Start()
    {
        false1 = Instantiate(false_knight_go);
        false2 = Instantiate(false_knight_go);
        false1.SetActive(true);
        false2.SetActive(true);
        On.HealthManager.TakeDamage += CheckDamage;
        damage_amount = 0;
    }

    private void CheckDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        damage_amount += hitInstance.DamageDealt;
        orig(self, hitInstance);
        if (damage_amount >= 1800)
        {
            gameObject.LocateMyFSM("FalseyControl").SendEvent("STUN");
            damage_amount = 0;
        }
    }

    void OnDestroy()
    {
        On.HealthManager.TakeDamage -= CheckDamage;
    }


}
