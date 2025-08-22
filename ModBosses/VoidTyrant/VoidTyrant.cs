using HutongGames.PlayMaker.Actions;
using Satchel;
using UnityEngine;
namespace rogue.ModBosses;

internal class VoidTyrant : MonoBehaviour
{
    PlayMakerFSM mage_lord;
    HealthManager health_manager;
    bool is_low_health = false;
    private void Awake()
    {
        mage_lord = gameObject.LocateMyFSM("Mage Lord");
        is_low_health = false;
        health_manager = gameObject.GetComponent<HealthManager>();
    }
    private void Start()
    {
        health_manager.hp = 1400;
        var summon_void = mage_lord.CopyState("HS Summon", "HS Summon Void");
        var orb_void = mage_lord.CopyState("HS Orb", "HS Orb Void");
        mage_lord.AddTransition("Idle", "LOW HEALTH", "HS Summon Void");
        mage_lord.InsertCustomAction("Idle", (fsm) =>
        {
            if (health_manager.hp <= 800 && !is_low_health)
            {
                fsm.SendEvent("LOW HEALTH");
            }
        }, 2);
        summon_void.ChangeTransition("FINISHED", "HS Orb Void");
        orb_void.ChangeTransition("FINISHED", "Set Idle Timer");
        orb_void.InsertCustomAction(() =>
        {
            is_low_health = true;
            GetComponent<tk2dSprite>().color = new Color(0.5f, 0.5f, 0.5f);
            mage_lord.GetAction<SendEventByName>("HS Dissipate", 1).Enabled = false;
            mage_lord.GetAction<SendEventByName>("Stun Init", 3).Enabled = false;
            mage_lord.ChangeTransition("HS Summon", "FINISHED", "HS Dir");
            mage_lord.InsertCustomAction("Teleport", (fsm) =>
            {
                GameObject spinner = fsm.FsmVariables.FindFsmGameObject("Orb Spinner").Value;
                StartCoroutine(TeleInit(spinner));
            }, 13);
            mage_lord.InsertCustomAction("TeleportQ", (fsm) =>
           {
               GameObject spinner = fsm.FsmVariables.FindFsmGameObject("Orb Spinner").Value;
               StartCoroutine(TeleInit(spinner));
           }, 12);
        }, 2);


    }
    IEnumerator TeleInit(GameObject spinner)
    {
        var fsm = spinner.LocateMyFSM("Summon Orbs");
        for (int i = 1; i <= 6; i++)
        {
            var orb = fsm.FsmVariables.FindFsmGameObject("Orb " + i).Value;
            if (orb != null)
            {
                orb.FindGameObjectInChildren("Hero Hurter").GetComponent<DamageHero>().damageDealt = 0;
                orb.GetComponent<tk2dSprite>().color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
        yield return new WaitForSeconds(0.6f);
        for (int i = 1; i <= 6; i++)
        {
            var orb = fsm.FsmVariables.FindFsmGameObject("Orb " + i).Value;
            if (orb != null)
            {
                orb.FindGameObjectInChildren("Hero Hurter").GetComponent<DamageHero>().damageDealt = 1;
                orb.GetComponent<tk2dSprite>().color = Color.white;
            }
        }
    }

}