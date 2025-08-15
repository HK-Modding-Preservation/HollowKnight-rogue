
namespace rogue.Characters;

internal class Hunter : Character
{
    System.Random random = new();
    AudioClip heal_clip;
    public Hunter()
    {
        this.Selfname = CharacterRole.hunter;
        AddBirthRight("嗜血");
        AddBirthRight("护符槽+3");
        AddBirthRight("0槽坚硬外壳");
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
        On.NailSlash.StartSlash += LongerNail;
        Rogue.Instance.ShowDreamConvo("hunter_dream".Localize());
        heal_clip = (AudioClip)Resources.InstanceIDToObject(32826);
    }

    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                On.HealthManager.TakeDamage += AttackRandomAddhealth;
                break;
            case 1:
                GiftHelper.AddCharmSlot(3);
                break;
            case 2:
                PlayerData.instance.gotCharm_4 = true;
                free_charms.Add(4);
                break;
        }
    }
    private void LongerNail(On.NailSlash.orig_StartSlash orig, NailSlash self)
    {
        orig(self);
        var scale = self.transform.localScale;
        float mul = PlayerData.instance.nailDamage / 4 * 0.15f + 1;
        self.transform.localScale = new Vector3(scale.x * mul, scale.y * mul, scale.z);
    }

    private void AttackRandomAddhealth(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        if (hitInstance.AttackType == AttackTypes.Nail && hitInstance.DamageDealt > 0)
        {
            if (random.Next(0, 100) < 8)
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
            case 1:
                break;
            case 2:
                free_charms.Remove(4);
                break;
        }
    }

    public override void EndCharacter()
    {
        On.NailSlash.StartSlash -= LongerNail;
    }
}