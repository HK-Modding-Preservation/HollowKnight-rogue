

namespace rogue;
internal class DoubleScream : CustomGift
{
    class DestroyOnDisable : MonoBehaviour
    {
        void OnDisable()
        {
            Destroy(gameObject);
        }
    }
    internal DoubleScream() : base(Giftname.custom_double_scream, 4, "witches_eye")
    {
        weight = 0.5f;
        price = 200;
        name = "深渊回响";
        desc = "吼结束时会在原地再释放一次";
    }


    protected override void _GetGift()
    {
        On.PlayMakerFSM.OnDisable += AnotherScream;
    }

    private void AnotherScream(On.PlayMakerFSM.orig_OnDisable orig, PlayMakerFSM self)
    {
        if ((self.gameObject.name == "Scr Heads" || self.gameObject.name == "Scr Heads 2") && self.FsmName == "FSM")
        {
            var ano_scream = GameObject.Instantiate(self.gameObject);
            ano_scream.transform.position = self.transform.position;
            ano_scream.LocateMyFSM("FSM").FsmVariables.FindFsmBool("Reposition").Value = false;
            ano_scream.LocateMyFSM("Deactivate on Hit").enabled = false;
            ano_scream.AddComponent<DestroyOnDisable>();
            ano_scream.SetActive(true);
        }
        orig(self);
    }

    protected override void _RemoveGift()
    {
        On.PlayMakerFSM.OnDisable -= AnotherScream;
    }
}