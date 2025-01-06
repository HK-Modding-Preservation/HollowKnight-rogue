using rogue;
using rogue.NPCs;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.UIElements;

internal abstract class NPC
{
    internal static GameObject template;
    internal enum KnightAction
    {
        LookUP,
        LookDown,
        Idle
    }
    internal NPC(tk2dSpriteAnimation library, KnightAction knight = KnightAction.Idle, bool only_sprite = false)
    {
        tk2dSpriteAnimation new_library = null;
        if (!only_sprite)
        {
            new_library = GameObject.Instantiate(library);
            GameObject.DontDestroyOnLoad(new_library);
        }
        go = GameObject.Instantiate(template);
        GameObject.DontDestroyOnLoad(go);
        go.name = GetType().Name;
        npc_ctl = go.LocateMyFSM("npc_control");
        convo_ctl = go.LocateMyFSM("Conversation Control");
        if (!only_sprite)
        {
            go.RemoveComponent<SpriteRenderer>();
        }
        go.FindGameObjectInChildren("Shiny").SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            convo_ctl.RemoveAction("Greet", 0);
        }
        AddConversation("test", "这是一个测试用的对话<page>如果它显示正常，说明你的NPC对话是正常的");
        convo_ctl.AddCustomAction("Greet", BeginConvo);
        convo_ctl.AddCustomAction("Hero Anim", ShowTitle);
        if (!only_sprite)
        {
            go.AddComponent<tk2dSprite>();
            go.AddComponent<tk2dSpriteAnimator>().Library = new_library;
        }
        ModHooks.LanguageGetHook += ChangeTitleName;
        switch (knight)
        {
            case KnightAction.LookUP:
                convo_ctl.GetAction<Tk2dPlayAnimation>("Hero Anim", 0).clipName = "LookUp";
                convo_ctl.GetAction<Tk2dPlayAnimation>("Anim End", 0).clipName = "LookUpToIdle";
                break;
            case KnightAction.LookDown:
                break;
            case KnightAction.Idle:
                convo_ctl.GetAction<Tk2dPlayAnimation>("Hero Anim", 0).clipName = "Idle";
                convo_ctl.GetAction<Tk2dPlayAnimation>("Anim End", 0).clipName = "Idle";
                break;
        }
        if (!only_sprite)
        {
            convo_ctl.AddCustomAction("Init", IdleAnimation);
            convo_ctl.AddCustomAction("Hero Anim", TalkAnimation);
            convo_ctl.AddCustomAction("Anim End", IdleAnimation);
        }
        go.SetActive(false);
    }

    private string ChangeTitleName(string key, string sheetTitle, string orig)
    {
        if (sheetTitle == "Titles" && key == GetSpecialName("") + "_MAIN") return GetName("main");
        if (sheetTitle == "Titles" && key == GetSpecialName("") + "_SUB") return GetName("sub");
        if (sheetTitle == "Titles" && key == GetSpecialName("") + "_SUPER") return GetName("super");
        return orig;
    }

    protected string name = "";
    protected string name_super = "";
    protected string name_sub = "";
    protected string idle_animation_name = "";
    protected string talk_animation_name = "";
    internal GameObject go = null;
    internal PlayMakerFSM npc_ctl = null;
    internal PlayMakerFSM convo_ctl = null;
    internal string GetSpecialName(string name)
    {
        return "rogueNPC_" + GetType().Name + "_" + name;
    }
    internal virtual string GetName(string pos)
    {
        pos = pos.ToLower();
        if (pos == "main") return name.Localize();
        else if (pos == "sub") return name_sub.Localize();
        else if (pos == "super") return name_super.Localize();
        return null;
    }
    internal virtual void AddConversation(string name, string convo)
    {
        RogueUIManager.DialogueUI.customDialogueManager.AddConversation(GetSpecialName(name), convo);
    }
    internal virtual void OnConvoEnd(string name, Action action)
    {
        RogueUIManager.DialogueUI.customDialogueManager.OnEndConversation((convo_name) =>
        {
            if (convo_name == GetSpecialName(name)) action();
        });
    }
    internal virtual void ShowDialogue(string name)
    {
        RogueUIManager.DialogueUI.customDialogueManager.ShowDialogue(GetSpecialName(name));
    }
    internal virtual void BeginConvo()
    {
        ShowDialogue("test");
    }
    protected virtual void IdleAnimation()
    {
        go.GetComponent<tk2dSpriteAnimator>().Play(idle_animation_name);
    }
    protected virtual void TalkAnimation()
    {
        go.GetComponent<tk2dSpriteAnimator>().Play(talk_animation_name);
    }
    internal void SetPosition(Vector3 pos)
    {
        if (go != null)
        {
            go.transform.position = pos;
            go.SetActive(true);
        }
    }
    void ShowTitle()
    {
        var title = HutongGames.PlayMaker.FsmVariables.GlobalVariables.FindFsmGameObject("AreaTitle").Value;
        title.SetActive(true);
        var fsm = title.LocateMyFSM("Area Title Control");
        fsm.FsmVariables.FindFsmBool("NPC Title").Value = true;
        fsm.FsmVariables.FindFsmString("Area Event").Value = GetSpecialName("");
    }
}
internal static class NPCManager
{
    static List<Type> types = new List<Type>(){
        typeof(NPC),
    };
    internal static Dictionary<string, NPC> npcs = new();
    const string elderbug_scene = "Town";
    const string elderbug_go = "_NPCs/Elderbug";
    const string jiji_scene = "Room_Ouiji";
    const string jiji_name = "Jiji NPC";
    const string jinn_scene = "Room_Jinn";
    const string jinn_name = "Jinn NPC";
    const string mask_maker_scene = "Room_Mask_Maker";
    const string mask_maker_mask = "Maskmaker NPC";
    const string mask_maker_body = "mm_body";
    const string mask_maker_desk = "Mask_Maker_Egg_Chamber_0008_Mid";
    const string xun_scene = "Room_Mansion";
    const string xun_name = "Xun NPC";
    const string emilitia_scene = "Ruins_House_03";
    const string emilitia_name = "Emilitia NPC";
    const string nailsmith_scene = "Room_nailsmith";
    const string nailsmith_name = "Nailsmith";
    const string quirrel_nail_scene = "Crossroads_50";
    const string quirrel_nail_name = "quirrel_death_nail";
    const string charm_slug_scene = "Room_Charm_Shop";
    const string charm_slug_name = "Charm Slug";
    const string charm_slug_desk = "shop_0000_a";
    internal static List<(string, string)> GetPreloadNames()
    {
        List<(string, string)> values = new List<(string, string)>()
        {
            (elderbug_scene,elderbug_go),
            (jiji_scene,jiji_name),
            (jinn_scene,jinn_name),
            (xun_scene,xun_name),
            (emilitia_scene,emilitia_name),
            (nailsmith_scene,nailsmith_name),
            (quirrel_nail_scene,quirrel_nail_name),
            (charm_slug_scene,charm_slug_name),
            (charm_slug_scene,charm_slug_desk)
        };
        return values;

    }
    internal static void Init(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        NPC.template = GameObject.Instantiate(preloadedObjects[Rogue.card_scene][Rogue.card_name]);
        NPC.template.SetActive(false);
        GameObject.DontDestroyOnLoad(NPC.template);
        npcs.Add(typeof(ElderBug).Name, new ElderBug(preloadedObjects[elderbug_scene][elderbug_go].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(Jiji).Name, new Jiji(preloadedObjects[jiji_scene][jiji_name].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(Jinn).Name, new Jinn(preloadedObjects[jinn_scene][jinn_name].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(Xun).Name, new Xun(preloadedObjects[xun_scene][xun_name].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(Emilitia).Name, new Emilitia(preloadedObjects[emilitia_scene][emilitia_name].GetComponent<tk2dSpriteAnimator>().Library));
        npcs.Add(typeof(QuirrelNail).Name, new QuirrelNail(preloadedObjects[quirrel_nail_scene][quirrel_nail_name].GetComponent<SpriteRenderer>().sprite));
        npcs.Add(typeof(CharmSlug).Name, new CharmSlug(preloadedObjects[charm_slug_scene][charm_slug_name].GetComponent<tk2dSpriteAnimator>().Library, GameObject.Instantiate(preloadedObjects[charm_slug_scene][charm_slug_desk])));
    }
}