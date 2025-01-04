

namespace rogue.Characters;
internal class Moth : Character
{
    public Moth()
    {
        this.Selfname = CharacterRole.moth;
    }

    int dream_mul = 0;
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.moth;
        PlayerData.instance.hasDreamNail = true;
        GameInfo.get_any_charm_num += 1;
        GiftHelper.AddNailDamage();
        GiftHelper.GiveMask();
        GiftHelper.GiveVessel();
        GiftHelper.AddCharmSlot(8);
        Rogue.Instance.ShowDreamConvo("moth_dream".Localize());
        On.EnemyDreamnailReaction.RecieveDreamImpact += DoDamage;

    }

    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                if (dream_mul == 0) dream_mul = 5;
                else
                {
                    dream_mul = 10;
                }
                break;
        }
        base.GetBirthright(num);
    }

    private void DoDamage(On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
    {

        FSMUtility.SendEventToGameObject(self.gameObject, "TAKE DAMAGE");
        HitTaker.Hit(self.gameObject, new HitInstance
        {
            Source = base.gameObject,
            AttackType = AttackTypes.Generic,
            CircleDirection = false,
            DamageDealt = dream_mul * PlayerData.instance.nailDamage,
            Direction = HeroController.instance.transform.localScale.x < 0 ? 180f : 0,
            IgnoreInvulnerable = false,
            MagnitudeMultiplier = 0,
            MoveAngle = 0f,
            MoveDirection = false,
            Multiplier = 1f,
            SpecialType = SpecialTypes.None,
            IsExtraDamage = false
        });
    }
    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                dream_mul = 0;
                break;

        }
        base.RemoveBirthright(num);
    }
    public override void EndCharacter()
    {
        On.EnemyDreamnailReaction.RecieveDreamImpact -= DoDamage;

    }
}