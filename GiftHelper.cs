namespace rogue;

static class GiftHelper
{
    public static void GiveMask()
    {
        if (PlayerData.instance.maxHealthBase < 9)
        {
            HeroController.instance.MaxHealth();
            HeroController.instance.AddToMaxHealth(1);
            PlayMakerFSM.BroadcastEvent("MAX HP UP");
        }
        else
        {
        }
    }
    public static void RemoveMask()
    {
        if (PlayerData.instance.maxHealthBase > 1)
        {
            PlayerData.instance.maxHealth -= 1;
            PlayerData.instance.maxHealthBase -= 1;
            if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            else
            {
                GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            }
        }
        else
        {
        }
    }

    public static void AdjustMaskTo(int t)
    {
        if (t < 1) return;
        if (PlayerData.instance.maxHealthBase < t)
        {
            while (PlayerData.instance.maxHealthBase < t)
            {
                GiveMask();
            }
        }
        else
        {
            while (PlayerData.instance.maxHealthBase > t)
            {
                RemoveMask();
            }
        }
    }

    public static void AddNailDamage()
    {
        if (PlayerData.instance.nailDamage < 21)
        {
            PlayerData.instance.nailDamage += 4;
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
        }
    }
    public static void DecreaseNailDamage()
    {
        if (PlayerData.instance.nailDamage > 5)
        {
            PlayerData.instance.nailDamage -= 4;
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
        }
    }

    public static void AdjustNailLevel(int level)
    {
        if (level < 0 || level > 4) return;
        int damage = 5 + level * 4;
        PlayerData.instance.nailDamage = damage;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
    }

    public static void AddCharmSlot(int t)
    {
        PlayerData.instance.charmSlots += t;
        if (PlayerData.instance.charmSlots > 11) PlayerData.instance.charmSlots = 11;
    }
    public static void RemoveCharmSlot(int t)
    {
        PlayerData.instance.charmSlots -= t;
        if (PlayerData.instance.charmSlots < 0) PlayerData.instance.charmSlots = 0;
    }
    public static void GiveVessel()
    {
        if (PlayerData.instance.MPReserveMax < 99)
        {
            HeroController.instance.AddToMaxMPReserve(33);
            PlayMakerFSM.BroadcastEvent("NEW SOUL ORB");
        }
        else
        {
        }
    }
    public static void RemoveVessel()
    {
        if (PlayerData.instance.MPReserveMax > 0)
        {
            PlayerData.instance.MPReserveMax -= 33;
            if (!GameCameras.instance.hudCanvas.gameObject.activeInHierarchy)
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            else
            {
                GameCameras.instance.hudCanvas.gameObject.SetActive(false);
                GameCameras.instance.hudCanvas.gameObject.SetActive(true);
            }
        }
        else
        {
        }
    }

    public static void AdjustVesselTo(int t)
    {
        if (t < 0 || t > 3) return;
        int mp = t * 33;
        if (PlayerData.instance.MPReserveMax > mp)
        {
            while (PlayerData.instance.MPReserveMax > mp) RemoveVessel();
        }
        else
        {
            while (PlayerData.instance.MPReserveMax < mp) GiveVessel();
        }
    }

    private static void UpdateCharmsEffects()
    {
        PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
    }

    public static void GiveAllCharms()
    {
        for (int i = 1; i <= 40; i++)
        {
            PlayerData.instance.SetBoolInternal("gotCharm_" + i, true);
        }

        PlayerData.instance.hasCharm = true;
        PlayerData.instance.charmsOwned = 40;
        PlayerData.instance.royalCharmState = 4;
        PlayerData.instance.gotShadeCharm = true;
        PlayerData.instance.charmCost_36 = 0;
        PlayerData.instance.fragileGreed_unbreakable = true;
        PlayerData.instance.fragileStrength_unbreakable = true;
        PlayerData.instance.fragileHealth_unbreakable = true;
        PlayerData.instance.grimmChildLevel = 5;
        PlayerData.instance.charmCost_40 = 3;
        PlayerData.instance.charmSlots = 11;

    }

    public static void RemoveAllCharms()
    {
        for (int i = 1; i <= 40; i++)
        {
            PlayerData.instance.SetBoolInternal("gotCharm_" + i, false);
            PlayerData.instance.SetBoolInternal("equippedCharm_" + i, false);
        }


        PlayerData.instance.hasCharm = true;
        PlayerData.instance.charmsOwned = 1;
        PlayerData.instance.gotShadeCharm = false;
        PlayerData.instance.fragileGreed_unbreakable = true;
        PlayerData.instance.fragileStrength_unbreakable = true;
        PlayerData.instance.fragileHealth_unbreakable = true;
        PlayerData.instance.grimmChildLevel = 5;
        PlayerData.instance.charmCost_40 = 2;
        PlayerData.instance.charmSlots = 3;
        PlayerData.instance.equippedCharms.Clear();
        PlayerData.instance.equippedCharms.Add(36);
        PlayerData.instance.charmCost_36 = 0;
        PlayerData.instance.royalCharmState = 4;

        PlayerData.instance.SetBoolInternal("gotCharm_" + "36", true);
        PlayerData.instance.SetBoolInternal("equippedCharm_" + "36", true);
        HeroController.instance.CharmUpdate();

        UpdateCharmsEffects();
    }

    public static void GiveAllSkills()
    {
        PlayerData.instance.screamLevel = 2;
        PlayerData.instance.fireballLevel = 2;
        PlayerData.instance.quakeLevel = 2;

        PlayerData.instance.hasDash = true;
        PlayerData.instance.canDash = true;
        PlayerData.instance.hasShadowDash = true;
        PlayerData.instance.canShadowDash = true;
        PlayerData.instance.hasWalljump = true;
        PlayerData.instance.canWallJump = true;
        PlayerData.instance.hasDoubleJump = true;
        PlayerData.instance.hasSuperDash = true;
        PlayerData.instance.canSuperDash = true;
        PlayerData.instance.hasAcidArmour = true;

        PlayerData.instance.hasDreamNail = true;
        PlayerData.instance.dreamNailUpgraded = true;
        PlayerData.instance.hasDreamGate = true;

        PlayerData.instance.hasNailArt = true;
        PlayerData.instance.hasCyclone = true;
        PlayerData.instance.hasDashSlash = true;
        PlayerData.instance.hasUpwardSlash = true;
        PlayerData.instance.hasAllNailArts = true;

    }
    public static void RemoveAllSkills()
    {
        PlayerData.instance.screamLevel = 0;
        PlayerData.instance.fireballLevel = 0;
        PlayerData.instance.quakeLevel = 0;
        PlayerData.instance.nailDamage = 5;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");

        PlayerData.instance.hasDash = false;
        PlayerData.instance.canDash = false;
        PlayerData.instance.hasShadowDash = false;
        PlayerData.instance.canShadowDash = false;
        PlayerData.instance.hasWalljump = false;
        PlayerData.instance.canWallJump = false;
        PlayerData.instance.hasDoubleJump = false;
        PlayerData.instance.hasSuperDash = false;
        PlayerData.instance.canSuperDash = false;
        PlayerData.instance.hasAcidArmour = false;

        PlayerData.instance.hasDreamNail = false;
        PlayerData.instance.dreamNailUpgraded = false;
        PlayerData.instance.hasDreamGate = false;

        PlayerData.instance.hasNailArt = false;
        PlayerData.instance.hasCyclone = false;
        PlayerData.instance.hasDashSlash = false;
        PlayerData.instance.hasUpwardSlash = false;
        PlayerData.instance.hasAllNailArts = false;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
    }
}