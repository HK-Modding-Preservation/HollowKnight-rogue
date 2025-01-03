
using System.Runtime.Serialization.Formatters.Binary;

namespace rogue.Characters;

internal class NailMaster : Character
{
    public NailMaster()
    {
        this.Selfname = CharacterRole.nail_master;
    }
    public override void BeginCharacter()
    {
        GameInfo.role = CharacterRole.nail_master;
        GiftHelper.AddNailDamage();
        GiftHelper.AddNailDamage();
        PlayerData.instance.hasCyclone = true;
        PlayerData.instance.hasDashSlash = true;
        PlayerData.instance.hasUpwardSlash = true;
        PlayerData.instance.hasNailArt = true;
        On.NailSlash.StartSlash += LongerNail;
        Rogue.Instance.ShowDreamConvo("nail_master_dream".Localize());
        GetBirthright(1);

    }

    private void LongerNail(On.NailSlash.orig_StartSlash orig, NailSlash self)
    {
        orig(self);
        var scale = self.transform.localScale;
        float mul = PlayerData.instance.nailDamage / 4 * 0.15f + 1;
        self.transform.localScale = new Vector3(scale.x * mul, scale.y * mul, scale.z);
    }

    public override void EndCharacter()
    {
        On.PlayerData.GetInt -= FreeNailGlory;
        On.NailSlash.StartSlash -= LongerNail;
    }
    public override int GetBirthrightNum()
    {
        return 1;
    }
    public override void GetBirthright(int num)
    {
        switch (num)
        {
            case 0:
                PlayerData.instance.gotCharm_26 = true;
                On.PlayerData.GetInt += FreeNailGlory;
                break;
            case 1:
                On.PlayMakerFSM.OnEnable += InverWhenNailArt;
                On.PlayMakerFSM.OnDisable += DisInverWhenNailArtEnd;
                break;


        }
    }

    private void DisInverWhenNailArtEnd(On.PlayMakerFSM.orig_OnDisable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "nailart_damage")
        {
            Log("End No Damage");
            HeroController.instance.SetDamageMode(0);
        }
        orig(self);
    }

    private void InverWhenNailArt(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        if (self.FsmName == "nailart_damage")
        {
            Log("Begin No Damage");
            HeroController.instance.SetDamageMode(1);
        }
        orig(self);
    }

    public override void RemoveBirthright(int num)
    {
        switch (num)
        {
            case 0:
                PlayerData.instance.gotCharm_26 = true;
                On.PlayerData.GetInt -= FreeNailGlory;
                break;
            case 1:
                On.PlayMakerFSM.OnEnable -= InverWhenNailArt;
                On.PlayMakerFSM.OnDisable -= DisInverWhenNailArtEnd;
                break;


        }
    }
    private int FreeNailGlory(On.PlayerData.orig_GetInt orig, PlayerData self, string intName)
    {
        if (intName == "charmCost_26") return 0;
        return orig(self, intName);
    }


}