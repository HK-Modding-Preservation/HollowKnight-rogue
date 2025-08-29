namespace rogue;

internal class SleepKnight : CustomGift
{
    internal SleepKnight() : base(Giftname.custom_sleep_knight, 0, "witches_eye")
    {
        price = 100;
        name = "custom_sleep_knight_name";
        desc = "custom_sleep_knight_desc";
    }

    protected override void _GetGift()
    {
        HeroController.instance.gameObject.transform.SetRotationZ(90);
        ModHooks.HeroUpdateHook += ChangeDirection;
    }
    float cd = 0.5f;
    float cd_timer = 0f;

    private void ChangeDirection()
    {
        cd_timer -= Time.deltaTime;
        if (InputHandler.Instance.inputActions.superDash.WasPressed/*&&cd_timer<0*/)
        {
            if (HeroController.instance.gameObject.transform.localEulerAngles.z == 90f)
            {
                HeroController.instance.gameObject.transform.SetRotationZ(270);
            }
            else
            {
                HeroController.instance.gameObject.transform.SetRotationZ(90);
            }
            cd_timer = cd;
        }
    }

    protected override void _RemoveGift()
    {
        HeroController.instance.gameObject.transform.SetRotationZ(0);
        ModHooks.HeroUpdateHook -= ChangeDirection;
    }
}