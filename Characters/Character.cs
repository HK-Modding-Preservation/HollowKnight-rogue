
using System.CodeDom;
using System.Configuration;
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
    tuk = (int)Giftname.role_tuk,
    defender = (int)Giftname.role_defender,
    mantis = (int)Giftname.role_mantis,
    collector = (int)Giftname.role_collector,
    no_role,
}
internal static class CharacterInfo
{
    internal static Dictionary<CharacterRole, Vector2> role_sprite_scales = new()
    {
        {CharacterRole.test, new Vector2(1, 1)},
        {CharacterRole.nail_master, new Vector2(1, 1)},
        {CharacterRole.shaman, new Vector2(1, 1)},
        {CharacterRole.hunter, new Vector2(0.2f, 0.2f)},
        {CharacterRole.uunn, new Vector2(0.15f, 0.15f)},
        {CharacterRole.joni, new Vector2(0.7f, 0.7f)},
        {CharacterRole.guarder, new Vector2(1, 1)},
        {CharacterRole.moth, new Vector2(0.6f, 0.6f)},
        {CharacterRole.grey_prince, new Vector2(0.45f, 0.45f)},
        {CharacterRole.grimm, new Vector2(1,1)},
        {CharacterRole.tuk, new Vector2(0.45f, 0.45f)},
        {CharacterRole.defender, new Vector2(0.5f, 0.5f)},
        {CharacterRole.mantis, new Vector2(0.55f, 0.55f)},
        {CharacterRole.collector, new Vector2(0.5f, 0.5f)},
    };
}
public class RoleGift<T> : Gift where T : Character
{
    public RoleGift(Giftname role_name) : base(5)
    {
        this.giftname = role_name;
        role = (CharacterRole)role_name;
        showConvo = false;
        active = true;
        force_active = true;
        weight = 0;
        price = 0;
        string true_name = role_name.ToString().Replace("role_", "");
        name = (true_name + "_name").Localize();
        desc = (true_name + "_desc").Localize();
        sprite = AssemblyUtils.GetSpriteFromResources(true_name + ".png");
        scale = CharacterInfo.role_sprite_scales.ContainsKey(role) ? CharacterInfo.role_sprite_scales[role] : new Vector2(1, 1);
    }
    CharacterRole role;
    internal override void GetGift()
    {
        HeroController.instance?.gameObject.RemoveComponent<Character>();
        HeroController.instance?.gameObject.AddComponent<T>();
    }



}
public abstract class Character : MonoBehaviour
{
    internal CharacterRole Selfname { get; protected private set; }
    protected float nail_mul = 1;
    protected float spell_mul = 1;
    protected List<int> got_birthright = new();
    public virtual int GetBirthrightNum()
    {
        return 0;
    }
    public virtual void ExecGetBirthright(int num)
    {
        if (got_birthright.Contains(num)) return;
        GetBirthright(num);
        got_birthright.Add(num);
    }
    public virtual void ExecRemoveBirthright(int num)
    {
        if (!got_birthright.Contains(num)) return;
        RemoveBirthright(num);
        got_birthright.Remove(num);
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

    public void OnDestroy()
    {
        foreach (var br in got_birthright)
        {
            RemoveBirthright(br);
        }
        got_birthright.Clear();
        EndCharacter();
        On.HealthManager.TakeDamage -= DamageMul;
        ItemManager.Instance.after_revive_action -= AfterRevive;
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

}
