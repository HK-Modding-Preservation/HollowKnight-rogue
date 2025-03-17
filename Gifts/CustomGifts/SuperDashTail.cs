
namespace rogue;
internal class SuperDashTail : CustomGift
{
    internal SuperDashTail() : base(Giftname.custom_super_dash_tail, 4, "witches_eye")
    {
        name = "超冲尾气";
        desc = "超冲尾气持续存在且伤害增加";
        weight = 0.5f;
        price = 200;
    }
    GameObject burst = null;
    GameObject GetBurst()
    {
        if (burst == null)
        {
            burst = HeroController.instance.superDash.FsmVariables.FindFsmGameObject("SD Burst").Value;
        }
        return burst;
    }

    protected override void _GetGift()
    {
        var burst = HeroController.instance.superDash.FsmVariables.FindFsmGameObject("SD Burst").Value;
        burst.LocateMyFSM("FSM").FsmVariables.FindFsmBool("Unparent").Value = false;
        burst.LocateMyFSM("FSM").GetAction<ActivateGameObject>("Destroy", 0).activate = true;
        burst.LocateMyFSM("Damage Control").GetAction<SetPolygonCollider>("Inactive", 0).active = true;
        burst.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = 30;
        On.HeroController.FixedUpdate += CheckIfSuperDashing;


    }

    private void CheckIfSuperDashing(On.HeroController.orig_FixedUpdate orig, HeroController self)
    {
        orig(self);
        if (self.cState.superDashing)
        {
            GetBurst().SetActive(true);
        }
        else
        {
            GetBurst().SetActive(false);
        }
    }

    protected override void _RemoveGift()
    {
        var burst = HeroController.instance.superDash.FsmVariables.FindFsmGameObject("SD Burst").Value;
        burst.LocateMyFSM("FSM").FsmVariables.FindFsmBool("Unparent").Value = true;
        burst.LocateMyFSM("FSM").GetAction<ActivateGameObject>("Destroy", 0).activate = false;
        burst.LocateMyFSM("Damage Control").GetAction<SetPolygonCollider>("Inactive", 0).active = false;
        burst.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = 10;
        On.HeroController.FixedUpdate -= CheckIfSuperDashing;
    }
}