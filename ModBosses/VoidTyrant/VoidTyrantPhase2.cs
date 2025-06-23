using HutongGames.PlayMaker.Actions;
using Satchel;
using UnityEngine;
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

    }

}