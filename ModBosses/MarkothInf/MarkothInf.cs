using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Satchel;
using UnityEngine;
namespace rogue.ModBosses;

internal class MarkothInf : MonoBehaviour
{

    PlayMakerFSM rage_check;
    PlayMakerFSM attack;
    PlayMakerFSM shield_attack;
    bool raged;
    HealthManager healthManager;
    void Awake()
    {
        raged = false;
        healthManager = base.gameObject.GetComponent<HealthManager>();
        rage_check = gameObject.LocateMyFSM("Rage Check");
        shield_attack = gameObject.LocateMyFSM("Shield Attack");
        attack = gameObject.LocateMyFSM("Attacking");

    }
    IEnumerator ModifyShield()
    {
        yield return new WaitForSeconds(0.5f);
        var shield = shield_attack.FsmVariables.FindFsmGameObject("Shield 1").Value;
        var shield_3 = Instantiate(shield.FindGameObjectInChildren("Shield 2"));
        shield_3.transform.SetParent(shield.transform);
        shield_3.name = "Shield 3";
        shield_3.SetActive(false);
    }
    private void Start()
    {
        healthManager.hp = 1200;
        rage_check.GetAction<IntOperator>("Set HP", 1).integer2 = 3;
        StartCoroutine(ModifyShield());
        rage_check.InsertCustomAction("Set Rage", () =>
        {

            var shield = shield_attack.FsmVariables.FindFsmGameObject("Shield 1").Value;
            shield.LocateMyFSM("Summon Shield").SetState("Idle");
            var itween = shield.LocateMyFSM("Summon Shield").GetAction<iTweenMoveTo>("Summon", 1);
            // shield.LocateMyFSM("Summon Shield").InsertAction("Summon", new iTweenMoveTo()
            // {
            //     gameObject = new FsmOwnerDefault()
            //     {
            //         OwnerOption = OwnerDefaultOption.SpecifyGameObject,
            //         GameObject = shield.FindGameObjectInChildren("Shield 3")
            //     },
            //     vectorPosition = new Vector3(4.05f * Mathf.Cos(Mathf.PI / 3), 4.05f * Mathf.Sin(Mathf.PI / 3), 0),
            //     id = "",
            //     transformPosition = itween.transformPosition,
            //     time = itween.time,
            //     delay = itween.delay,
            //     speed = itween.speed,
            //     space = itween.space,
            //     easeType = itween.easeType,
            //     loopType = itween.loopType,
            //     realTime = false,
            //     stopOnExit = true,
            //     loopDontFinish = true
            // }, 2);

            shield.LocateMyFSM("Summon Shield").InsertCustomAction("Summon", () =>
            {
                shield.FindGameObjectInChildren("Shield 3").SetActive(true);
                shield.FindGameObjectInChildren("Shield 3").transform.localPosition = new Vector3(4.05f * Mathf.Cos(Mathf.PI / 3), 4.05f * Mathf.Sin(Mathf.PI / 3), 0);
            }, 2);
            shield.LocateMyFSM("Summon Shield").InsertCustomAction("Summon", () =>
            {
                shield.FindGameObjectInChildren("Shield 3").transform.SetRotationZ(60f);
                shield.FindGameObjectInChildren("Shield 2").transform.SetRotationZ(-60f);
            }, 3);
            itween.vectorPosition = new Vector3(4.05f * Mathf.Cos(Mathf.PI / 3), -4.05f * Mathf.Sin(Mathf.PI / 3), 0);

        }, 2);
        shield_attack.InsertCustomAction("Send Summon", (fsm) =>
        {
            if (!raged)
            {
                raged = true;
                fsm.FsmVariables.FindFsmBool("Rage Shield").Value = false;
                shield_attack.FsmVariables.FindFsmBool("Rage").Value = false;
                attack.GetAction<WaitRandom>("Wait", 1).timeMin = 0.75f;
                attack.GetAction<WaitRandom>("Wait", 1).timeMax = 1.25f;
            }
            else
            {
                healthManager.hp = 800;
            }
        }, 4);
        attack.GetAction<WaitRandom>("Wait rage", 0).timeMin = 0.5f;
        attack.GetAction<WaitRandom>("Wait rage", 0).timeMax = 1f;
    }
    private void Update()
    {
        if (healthManager.hp < 800 && !raged)
        {
            shield_attack.FsmVariables.FindFsmBool("Rage").Value = true;
        }
    }


}