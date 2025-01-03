
namespace rogue.Characters;
internal class Hunter : Character
{
    System.Random random = new();
    AudioClip heal_clip;
    public Hunter()
    {
        this.Selfname = CharacterRole.hunter;
    }
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.hunter;
        PlayerData.instance.hasDash = true;
        PlayerData.instance.canDash = true;
        PlayerData.instance.hasDoubleJump = true;
        PlayerData.instance.hasWalljump = true;
        PlayerData.instance.canWallJump = true;
        PlayerData.instance.canSuperDash = true;
        PlayerData.instance.hasSuperDash = true;
        PlayerData.instance.gotCharm_14 = true;
        PlayerData.instance.gotCharm_32 = true;
        GiftHelper.AddCharmSlot(1);
        Rogue.Instance.ShowDreamConvo("hunter_dream".Localize());
        GetBirthright(0);
        heal_clip = (AudioClip)Resources.InstanceIDToObject(32826);
    }
    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                On.HealthManager.TakeDamage += AttackRandomAddhealth;
                break;
        }
    }

    private void AttackRandomAddhealth(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        if (hitInstance.AttackType == AttackTypes.Nail && hitInstance.DamageDealt > 0)
        {
            if (random.Next(0, 100) < 5)
            {
                Log("Attack AddHealth");
                HeroController.instance.AddHealth(1);
                HeroController.instance.gameObject.GetComponent<AudioSource>().PlayOneShot(heal_clip);
                HeroController.instance.gameObject.SendMessage("flashFocusHeal", options: SendMessageOptions.DontRequireReceiver);
            }
        }
        orig(self, hitInstance);
    }

    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                On.HealthManager.TakeDamage -= AttackRandomAddhealth;
                break;
        }
    }

    public override void EndCharacter()
    {
    }
}