namespace rogue;
internal class Darkness : CustomGift
{
    private readonly Dictionary<(string Name, string EventName), string> OriginalTransitions = new();
    public Darkness() : base(Giftname.custom_darkness, 4, "witches_eye")
    {
        name = "黑暗";
        desc = "万物皆虚，万事皆允";
        weight = 1f;
    }
    private void Lighten()
    {
        if (HeroController.UnsafeInstance == null) return;

        foreach (FsmState state in HeroController.instance.vignetteFSM.FsmStates)
        {
            foreach (FsmTransition trans in state.Transitions)
            {
                if (!OriginalTransitions.TryGetValue((state.Name, trans.EventName), out string orig)) continue;

                trans.ToState = orig;
            }
        }

        HeroController.instance.vignetteFSM.SetState("Normal");
        HeroController.instance.vignette.enabled = false;
    }
    private void Darken()
    {
        if (HeroController.instance == null)
            return;

        foreach (FsmState state in HeroController.instance.vignetteFSM.FsmStates)
        {
            foreach (FsmTransition trans in state.Transitions)
            {
                switch (trans.ToState)
                {
                    case "Dark -1":
                    case "Normal":
                    case "Dark 1":
                    case "Lantern":

                        OriginalTransitions[(state.Name, trans.EventName)] = trans.ToState;

                        trans.ToState = "Dark 2";
                        break;
                    case "Dark -1 2":
                    case "Normal 2":
                    case "Dark 1 2":
                    case "Lantern 2":

                        OriginalTransitions[(state.Name, trans.EventName)] = trans.ToState;

                        trans.ToState = "Dark 2 2";
                        break;
                }
            }
        }

        HeroController.instance.vignetteFSM.SetState("Dark 2");
        HeroController.instance.vignette.enabled = true;
    }
    protected override void _GetGift()
    {
        Darken();
    }

    protected override void _RemoveGift()
    {
        Lighten();
    }
}