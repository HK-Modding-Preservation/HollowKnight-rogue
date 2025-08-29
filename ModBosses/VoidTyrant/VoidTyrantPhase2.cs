using HutongGames.PlayMaker.Actions;
using Satchel;
using UnityEngine;
namespace rogue.ModBosses;

internal class VoidTyrantPhase2 : MonoBehaviour
{
    PlayMakerFSM mage_lord;
    HealthManager health_manager;
    bool is_low_health = false;
    private void Awake()
    {
        mage_lord = gameObject.LocateMyFSM("Mage Lord 2");
        health_manager = gameObject.GetComponent<HealthManager>();
    }
    private void Start()
    {
        health_manager.hp = 500;
        mage_lord.InsertCustomAction("Music", (fsm) =>
        {
            GetComponent<tk2dSprite>().color = new Color(0.5f, 0.5f, 0.5f);
            fsm.FsmVariables.FindFsmGameObject("Orb Spinner").Value.LocateMyFSM("Summon Orbs").SendEvent("SPINNER SUMMON");
        }, 0);
        mage_lord.InsertCustomAction("Teleport", (fsm) =>
            {
                GameObject spinner = fsm.FsmVariables.FindFsmGameObject("Orb Spinner").Value;
                StartCoroutine(TeleInit(spinner));
            }, 13);

        mage_lord.InsertCustomAction("TeleportQ", (fsm) =>
       {
           GameObject spinner = fsm.FsmVariables.FindFsmGameObject("Orb Spinner").Value;
           StartCoroutine(TeleInit(spinner));
       }, 10);
        mage_lord.InsertCustomAction("After Tele", (fsm) =>
        {
            GameObject spinner = fsm.FsmVariables.FindFsmGameObject("Orb Spinner").Value;
            StartCoroutine(DamageAble(spinner));
        }, 0);
        mage_lord.InsertCustomAction("Quake Land", (fsm) =>
        {
            GameObject spinner = fsm.FsmVariables.FindFsmGameObject("Orb Spinner").Value;
            StartCoroutine(DamageAble(spinner));
        }, 16);

    }
    IEnumerator TeleInit(GameObject spinner)
    {
        var fsm = spinner.LocateMyFSM("Summon Orbs");
        for (int i = 1; i <= 4; i++)
        {
            var orb = fsm.FsmVariables.FindFsmGameObject("Orb " + i).Value;
            if (orb != null)
            {
                orb.FindGameObjectInChildren("Hero Hurter").GetComponent<DamageHero>().damageDealt = 0;
                orb.GetComponent<tk2dSprite>().color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
        yield break;

    }
    IEnumerator DamageAble(GameObject spinner)
    {
        var fsm = spinner.LocateMyFSM("Summon Orbs");
        for (int i = 1; i <= 4; i++)
        {
            var orb = fsm.FsmVariables.FindFsmGameObject("Orb " + i).Value;
            if (orb != null)
            {
                orb.FindGameObjectInChildren("Hero Hurter").GetComponent<DamageHero>().damageDealt = 1;
                orb.GetComponent<tk2dSprite>().color = Color.white;
            }
        }
        yield break;
    }
}