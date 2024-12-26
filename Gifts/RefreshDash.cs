using rogue;
internal class RefreshDash : CustomGift
{
    public RefreshDash() : base(Giftname.refresh_dash, 3, "witches_eye")
    {
        weight = 1f;
        price = 200;
    }


    internal override void GetGift()
    {

        Rogue.Instance.Log("Get RefreshDash");
        base.GetGift();
    }
    internal override void RemoveGift()
    {
        base.RemoveGift();
    }


}