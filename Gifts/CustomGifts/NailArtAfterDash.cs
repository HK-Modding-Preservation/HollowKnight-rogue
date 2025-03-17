using rogue;

internal class NailArtAfterDash : CustomGift
{
    public NailArtAfterDash() : base(Giftname.custom_nail_art_after_dash, 4, "witches_eye")
    {
        weight = 0.5f;
        price = 200;
        name = "冲刺必定冲刺斩";
        desc = "冲刺必定冲刺斩";
    }
    GameObject GetKnight()
    {
        return HeroController.instance.gameObject;
    }

    protected override void _GetGift()
    {
        PlayerData.instance.hasNailArt = true;
        PlayerData.instance.hasUpwardSlash = true;
        On.HeroController.HeroDash += DoNailArt;
        GetKnight().LocateMyFSM("Nail Arts").AddCustomAction("Has Dash?", () => { On.HeroController.CanNailArt -= MustNailArt; });
    }

    private void DoNailArt(On.HeroController.orig_HeroDash orig, HeroController self)
    {
        orig(self);
        On.HeroController.CanNailArt += MustNailArt;
        GetKnight().LocateMyFSM("Nail Arts").SendEvent("BUTTON UP");
    }

    private bool MustNailArt(On.HeroController.orig_CanNailArt orig, HeroController self)
    {
        if (Got)
            return true;
        else
        {
            return orig(self);
        }
    }

    protected override void _RemoveGift()
    {
        On.HeroController.HeroDash -= DoNailArt;
        try
        {
            GetKnight().LocateMyFSM("Nail Arts").RemoveAction("Has Dash?", 1);

        }
        catch (Exception)
        {

        }
    }
}