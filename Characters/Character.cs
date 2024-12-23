
using Unity.IO.LowLevel.Unsafe;

namespace rogue.Characters;
public enum CharacterRole
{

    test = (int)Giftname.role_test,
    nail_master = (int)Giftname.role_nail_master,
    shaman = (int)Giftname.role_shaman,
    hunter = (int)Giftname.role_hunter,
    uunn = (int)Giftname.role_uunn,
    joni = (int)Giftname.role_joni,
    guarder = (int)Giftname.role_guarder,
    moth = (int)Giftname.role_moth,
    grey_prince = (int)Giftname.role_grey_prince,
    grimm = (int)Giftname.role_grimm,
    Tuk = (int)Giftname.role_tuk,
    defender = (int)Giftname.role_defender,
    mantis = (int)Giftname.role_mantis,
    collector = (int)Giftname.role_collector,
    no_role,
}

public abstract class Character : MonoBehaviour
{
    internal CharacterRole Selfname { get; protected private set; }
    protected float nail_mul = 1;
    protected float spell_mul = 1;
    public virtual int GetBirthrightNum()
    {
        return 0;
    }
    public virtual void GetBirthright(int num)
    {

    }
    public virtual void RemoveBirthright(int num)
    {

    }
    public abstract void BeginCharacter();
    public abstract void EndCharacter();
    public void Start()
    {
        BeginCharacter();
        On.HealthManager.TakeDamage += DamageMul;
        ItemManager.Instance.after_revive_action += AfterRevive;
    }

    protected virtual void AfterRevive()
    {
        HeroController.instance.CharmUpdate();
        PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
    }

    protected virtual void DamageMul(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        if (!GameInfo.in_rogue)
        {
            orig(self, hitInstance);
            return;
        }
        switch (hitInstance.AttackType)
        {
            case AttackTypes.Nail:
                hitInstance.Multiplier *= nail_mul;
                break;
            case AttackTypes.Generic:
                break;
            case AttackTypes.Spell:
                hitInstance.Multiplier *= spell_mul;
                break;
            case AttackTypes.Splatter:
                break;
            case AttackTypes.SharpShadow:
                break;
            case AttackTypes.NailBeam:
                break;
            case AttackTypes.RuinsWater:
                break;
            default: break;
        }
        orig(self, hitInstance);

    }
    public void OnDestroy()
    {
        EndCharacter();
        On.HealthManager.TakeDamage -= DamageMul;
        ItemManager.Instance.after_revive_action -= AfterRevive;
    }
}
