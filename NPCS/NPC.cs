
namespace rogue.NPCs;
internal abstract class NPC
{
    internal static GameObject template;
    internal enum KnightAction
    {
        LookUP,
        LookDown,
        Idle
    }
    internal NPC(tk2dSpriteAnimation library, Vector2 offset, KnightAction knight = KnightAction.Idle, bool only_sprite = false)
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
        go.GetComponent<Collider2D>().offset = offset;
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
    internal bool keep = false;
    internal bool meet = false;
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
        RogueUIManager.StartConversation(0.3f, GetSpecialName(name));
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
        if (keep)
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= DeactivateSelf;
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += DeactivateSelf;
        meet = false;
    }

    private void DeactivateSelf(Scene arg0, Scene arg1)
    {
        if (!keep)
        {
            go.SetActive(false);
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= DeactivateSelf;
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