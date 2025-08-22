
using Mono.Cecil;

namespace rogue;

internal class AlwaysParry : CustomGift
{
    public AlwaysParry() : base(Giftname.custom_always_parry, 4, "blue_idle")
    {
        name = "custom_always_parry_name";
        desc = "custom_always_parry_desc";
        weight = 0.5f;
        price = 200;
    }
    AudioClip parry;

    protected override void _GetGift()
    {
        On.HealthManager.TakeDamage += KnightParry;
        parry = (AudioClip)Resources.InstanceIDToObject(29978);
    }

    private void KnightParry(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        if (hitInstance.DamageDealt > 0 && hitInstance.AttackType == AttackTypes.Nail)
        {
            GameManager._instance.FreezeMoment(1);
            HeroController.instance.NailParry();
            HeroController.instance.gameObject.GetComponent<AudioSource>().PlayOneShot(parry);

        }
        orig(self, hitInstance);
    }

    protected override void _RemoveGift()
    {
        On.HealthManager.TakeDamage -= KnightParry;
    }
}