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
        }, 2);

    }

}