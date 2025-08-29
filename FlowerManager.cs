using System.Diagnostics;

namespace rogue;

internal class Check : MonoBehaviour
{
    internal static string music_scene = "Opening_Sequence";
    internal static string music_object = "Excerpt/Animation/Music";

    AudioClip clip;
    AudioSource au;
    float timer = 0f;
    PlayMakerFSM spell_con;
    void OnEnable()
    {
        au = base.gameObject.GetAddComponent<AudioSource>();
        transform.position.Set(250f, 10f, 0);
        On.HutongGames.PlayMaker.Fsm.SwitchState += CheckSpell;
        clip = PreloadManager.getGO(music_scene, music_object).GetComponent<AudioSource>().clip;
        spell_con = HeroController.instance.gameObject.LocateMyFSM("Spell Control");
    }

    private void CheckSpell(On.HutongGames.PlayMaker.Fsm.orig_SwitchState orig, HutongGames.PlayMaker.Fsm self, HutongGames.PlayMaker.FsmState toState)
    {

        if (self == spell_con.Fsm)
        {
            if (toState.Name == "Scream End 2" || toState.Name == "Scream End")
            {
                if (HeroController.instance.transform.position.x < 240f || HeroController.instance.transform.position.x > 260f)
                {
                    orig(self, toState);
                    return;
                }
                if (HeroController.instance.transform.position.y > 16f || HeroController.instance.transform.position.y < 6f)
                {
                    orig(self, toState);
                    return;
                }
                if (timer <= 0f)
                {
                    timer = 15f;
                    Rogue.Instance.ShowDreamConvo("Try again");
                    au.PlayOneShot(clip);
                }
            }
        }
        orig(self, toState);
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;

        }
    }



    void OnDestroy()
    {
        On.HutongGames.PlayMaker.Fsm.SwitchState -= CheckSpell;

    }
}

internal static class FlowerManager
{
    internal static bool open = true;
    internal static bool HasFlower => PlayerData.instance.hasXunFlower && !PlayerData.instance.xunFlowerBroken;
    static IEnumerator DelayShowDreamConvo(float wait_time, string conv)
    {
        yield return new WaitForSeconds(wait_time);
        Rogue.Instance.ShowDreamConvo(conv);
        yield break;
    }
    internal static bool CheckFlowerEnd()
    {
        if (open)
        {
            if (HasFlower)
            {
                return true;
            }
        }
        return false;
    }
    internal static void CheckFlower(string scene_name)
    {

        if (open)
        {
            if (scene_name == "GG_Spa")
            {
                if (HasFlower)
                {
                    if (GameInfo.spa_count == 3)
                    {
                        ItemManager.Instance.StartCoroutine(DelayShowDreamConvo(0.5f, "flower_spa_3".Localize()));
                    }
                    else if (GameInfo.spa_count == 4)
                    {
                        if (GameInfo.Branch.collector)
                        {
                            ItemManager.Instance.StartCoroutine(DelayShowDreamConvo(0.5f, "flower_spa_4_1".Localize()));
                        }
                        else
                        {
                            ItemManager.Instance.StartCoroutine(DelayShowDreamConvo(0.5f, "flower_spa_4_2".Localize()));
                        }
                    }
                    else if (GameInfo.spa_count == 5)
                    {
                        ItemManager.Instance.StartCoroutine(DelayShowDreamConvo(0.5f, "180,315  225,270  45,180  180,315  0,225"));
                    }
                    else if (GameInfo.spa_count == 6)
                    {
                        if (GameInfo.Branch.lost_kin)
                        {
                            ItemManager.Instance.StartCoroutine(DelayShowDreamConvo(0.5f, "135,270  45,90  225,315  0,315  180,315  225,270"));
                        }
                        else
                        {
                            ItemManager.Instance.StartCoroutine(DelayShowDreamConvo(0.5f, "0,315  225,270  45,180"));
                        }
                    }
                    else if (GameInfo.spa_count == 7)
                    {
                        if (GameInfo.Branch.modboss)
                        {
                            ItemManager.Instance.StartCoroutine(DelayShowDreamConvo(0.5f, "180,225  0,315"));
                        }
                        else
                        {
                            ItemManager.Instance.StartCoroutine(DelayShowDreamConvo(0.5f, "45,225  225,315  90,180  45,180\n180,270  45,225  270,315  270,315  0,180"));
                        }

                    }
                }
            }
        }
    }

}