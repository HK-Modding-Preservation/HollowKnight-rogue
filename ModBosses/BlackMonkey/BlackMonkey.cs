using System.Collections;
using System.Deployment.Internal;
using HutongGames.PlayMaker.Actions;
using Satchel;
using UnityEngine;
namespace rogue.ModBosses;

internal class BlackMonkey : MonoBehaviour
{
    static int bossleft = 7;
    static GameObject original;
    internal int layer = 0;
    void Start()
    {
        if (layer == 0)
        {
            bossleft = 7;
            original = base.gameObject;
            var new_layer = GameObject.Instantiate(original);
            new_layer.GetComponent<BlackMonkey>().layer = layer + 1;
            new_layer.SetActive(true);
            base.gameObject.SetActive(false);
            base.gameObject.transform.parent.gameObject.LocateMyFSM("Control").GetAction<ActivateGameObject>("Start", 0).activate = false;
            return;

        }
        GetComponent<HealthManager>().hp = 500;
        GetComponent<HealthManager>().OnDeath += () =>
        {
            bossleft--;
            if (bossleft <= 0)
            {
                original.transform.parent.gameObject.LocateMyFSM("Control").SendEvent("BATTLE END");
                BossSceneController.Instance.EndBossScene();
            }
            else
            {
                base.gameObject.SetActive(false);
            }
        };
        var fsm = base.gameObject.LocateMyFSM("Control");
        fsm.ChangeTransition("Summon?", "SUMMON", "Return Antic");
        if (layer != 1)
        {
            fsm.ChangeTransition("Roar Antic", "FINISHED", "Roar End");
        }
        fsm.InsertCustomAction("Death Start", () =>
        {
            if (layer < 3)
            {
                StartCoroutine(SpawnChild());

            }
        }, 14);
        var death = base.gameObject.LocateMyFSM("Death");
        death.GetAction<PlayParticleEmitter>("Death Antic", 3).Enabled = false;
        death.GetAction<PlayParticleEmitter>("Blow", 2).Enabled = false;

    }
    IEnumerator SpawnChild()
    {
        for (int i = 0; i < 2; i++)
        {

            GameObject gameObject = GameObject.Instantiate(original);
            gameObject.GetComponent<BlackMonkey>().layer = layer + 1;
            gameObject.SetActive(true);
            gameObject.LocateMyFSM("Control").SendEvent("WAKE");
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }



}