using rogue;

internal static class RogueUIManager
{
    //UI步骤：小骑士固定住，不接受骑士信息，Box升起，等0.3s后开始对话
    static RogueUIManager()
    {
        ModHooks.LanguageGetHook += ChangeLanguage;
        ModHooks.HeroUpdateHook += Test;
    }
    static int try_cnt = 0;
    private static void Test()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            TestSelect();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            TestDialogue();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            NPCManager.npcs[typeof(ElderBug).Name].SetPosition(GameObject.Find("Knight").transform.position);
        }
    }
    static class SelectUI
    {
        internal const float left_bound = -9.5f;
        internal const float right_bound = 9.5f;
        internal static GameObject selectList = null;
        internal static DialogueBox select_dialogue_box = null;
        internal static GameObject yesTem = null;
        internal static List<GameObject> selections = new();
        internal static void SetNum(int num, int cancel_num)
        {
            var ui_list = selectList.FindGameObjectInChildren("UI List");
            var list_fsm = selectList.FindGameObjectInChildren("UI List").LocateMyFSM("ui_list");
            list_fsm.FsmVariables.GetFsmInt("Initial Item").Value = 1;
            list_fsm.FsmVariables.GetFsmInt("Items").Value = num;
            list_fsm.FsmVariables.GetFsmInt("Cancel Item").Value = cancel_num;
            while (selections.Count < num)
            {
                var new_selection = GameObject.Instantiate(yesTem);
                new_selection.transform.SetParent(ui_list.transform, false);
                selections.Add(new_selection);
                var item_fsm = new_selection.LocateMyFSM("ui_list_item");
                item_fsm.FsmVariables.GetFsmInt("Item Number").Value = selections.Count;
                item_fsm.ChangeTransition("Selection Made Cancel", "GET SELECTED", "Selected?");
                var enough_fsm = new_selection.LocateMyFSM("Enough Geo");
                enough_fsm.AddCustomAction("Not Enuff", (fsm) => { fsm.RemoveTransition("Init", "FINISHED"); });
            }
        }
        internal static void SetCanSelect(int select_num, bool selectable)
        {
            if (selections.Count <= select_num) return;
            if (!selectable)
            {
                var enough_fsm = selections[select_num].LocateMyFSM("Enough Geo");
                enough_fsm.AddTransition("Init", "FINISHED", "Not Enuff");
                var item_fsm = selections[select_num].LocateMyFSM("ui_list_item");
                item_fsm.ChangeTransition("Selection Made Cancel", "GET SELECTED", "Selected?");
            }
        }
    }

    static bool initialized = false;

    //Toll Cost 显示吉欧数

    internal static class DialogueUI
    {
        internal static bool initialized = false;
        internal static Satchel.CustomDialogueManager customDialogueManager;
    }

    internal static string conversation = "";
    const string ui_sheet = "UI";
    const string ui_key = "RogueUI_key";

    static List<SelectItem> now_select_items;
    static GameObject GetDialogueManager()
    {
        return FsmVariables.GlobalVariables.FindFsmGameObject("DialogueManager").Value;
    }
    static bool TryInit()
    {
        if (!initialized)
        {
            var yn = FsmVariables.GlobalVariables.GetFsmGameObject("DialogueTextYN");
            if (yn == null) return false;
            SelectUI.selectList = GameObject.Instantiate(yn.Value);
            GameObject.DontDestroyOnLoad(SelectUI.selectList);
            foreach (var fsm in SelectUI.selectList.GetComponents<PlayMakerFSM>())
            {
                if (fsm.FsmName == "Globalise") fsm.enabled = false;
            }
            var page_con = SelectUI.selectList.LocateMyFSM("Dialogue Page Control");
            page_con.ChangeTransition("End Conversation", "YES", "No");
            page_con.InsertCustomAction("End Conversation", (fsm) => { fsm.SendEvent("NO"); }, 5);
            page_con.InsertCustomAction("No", (fsm) =>
            {
                int item_num = fsm.FsmVariables.GetFsmInt("Item Number").Value;
                DoAfterSelection(item_num);
            }, 1);
            initialized = true;
            SelectUI.select_dialogue_box = SelectUI.selectList.GetComponent<DialogueBox>();
            SelectUI.yesTem = SelectUI.selectList.FindGameObjectInChildren("UI List").FindGameObjectInChildren("Yes");
            // SelectUI.SetNum(5, 1);
            var no = SelectUI.selectList.FindGameObjectInChildren("UI List").FindGameObjectInChildren("No");
            GameObject.Destroy(no);
            SelectUI.yesTem.SetActive(false);
            SelectUI.selectList.SetActive(false);

            return true;

        }
        return initialized;
    }
    private static string ChangeLanguage(string key, string sheetTitle, string orig)
    {
        if (key == ui_key) { return conversation; }
        return orig;
    }
    internal class SelectItem
    {
        public SelectItem(string selection)
        {
            can_select = selection;
            not_select = selection;
            not_select_info = "你没资格啊";
            selectable = true;

        }
        public string can_select;
        public bool selectable;
        public string not_select;
        public string not_select_info;

    }
    internal static void StartSelection(float delay, List<SelectItem> items, int cancel_num)
    {
        ItemManager.Instance?.StartCoroutine(_StartSelection(delay, items, cancel_num));
    }

    internal static IEnumerator _StartSelection(float delay, List<SelectItem> items, int cancel_num)
    {
        yield return new WaitForSeconds(delay);
        if (!TryInit()) yield break;
        now_select_items = items;
        int select_num = items.Count;
        SelectUI.SetNum(select_num, cancel_num);
        float gap = (SelectUI.right_bound - SelectUI.left_bound) / (select_num + 1);
        float select_item_x = SelectUI.left_bound + gap;
        for (int i = 0; i < SelectUI.selections.Count; i++)
        {
            if (i < select_num)
            {
                SelectUI.selections[i].GetComponent<TMPro.TextMeshPro>().text = items[i].can_select;
                SelectUI.selections[i].GetComponent<SetTextMeshProGameText>().enabled = false;
                // SelectUI.selections[i].GetComponent<TMPro.TextMeshPro>().DelaySetText(0.2f, items[i].can_select);
                var not_enough = SelectUI.selections[i].FindGameObjectInChildren("Not Enough");
                not_enough.GetComponent<TMPro.TextMeshPro>().text = items[i].not_select;
                not_enough.GetComponent<SetTextMeshProGameText>().enabled = false;
                // not_enough.GetComponent<TMPro.TextMeshPro>().DelaySetText(0.2f, items[i].not_select);
                var not_enough_sub = not_enough.FindGameObjectInChildren("Not Enough Sub");
                not_enough_sub.GetComponent<SetTextMeshProGameText>().enabled = false;
                not_enough_sub.GetComponent<TMPro.TextMeshPro>().text = items[i].not_select_info;
                // not_enough_sub.GetComponent<TMPro.TextMeshPro>().DelaySetText(0.2f, items[i].not_select_info);
                Vector3 last_pos = SelectUI.selections[i].transform.localPosition;
                last_pos.x = select_item_x;
                select_item_x += gap;
                SelectUI.selections[i].transform.localPosition = last_pos;
                SelectUI.selections[i].SetActive(true);
                SelectUI.SetCanSelect(i, items[i].selectable);

            }
            else
            {
                SelectUI.selections[i].SetActive(false);
            }

        }
        SelectUI.selectList.SetActive(true);
        GetDialogueManager().LocateMyFSM("Box Open YN").SendEvent("BOX UP YN");
        InputHandler.Instance.StopUIInput();
        HeroController.instance.RelinquishControl();
        yield return new WaitForSeconds(0.3f);
        SelectUI.select_dialogue_box.StartConversation(ui_key, ui_sheet);
    }
    static void DoSetSelections()
    {

    }
    static void DoAfterSelection(int item_num)
    {
        Rogue.Instance.ShowDreamConvo("select " + item_num);
        GetDialogueManager().LocateMyFSM("Box Open YN").SendEvent("BOX DOWN YN");
        HeroController.instance.RegainControl();
        InputHandler.Instance.StartUIInput();
    }
    static void DelaySetText(this TMPro.TextMeshPro com, float delay, string text)
    {
        static IEnumerator _DelaySetText(TMPro.TextMeshPro com, float delay, string text)
        {

            Rogue.Instance.Log("before delay" + com.text);
            yield return new WaitForSeconds(delay);
            Rogue.Instance.Log("before" + com.text);
            com.SetText(text);
            com.ForceMeshUpdate();
            yield return null;
            Rogue.Instance.Log("after" + com.text);
        }
        Rogue.Instance.Log("before" + com.text);
        com.StartCoroutine(_DelaySetText(com, delay, text));
    }
    static void TestSelect()
    {
        if (try_cnt == 0)
        {
            DialogueUI.customDialogueManager.OnEndConversation((name) =>
        {
            if (name == "test")
            {
                TestSelect();
            }
        });
        }
        Log("Test Select" + try_cnt);
        try_cnt++;
        conversation = "敬请见证";
        List<SelectItem> items = new()
        {
        };
        for (int i = 0; i < try_cnt + 1; i++)
        {
            items.Add(new SelectItem("选项" + (i + 1)));
        }
        if (items.Count > 2)
        {
            items[0].selectable = false;
            foreach (SelectItem item in items)
            {
                item.selectable = false;
            }
        }
        StartSelection(0.3f, items, 1);
    }
    static void TestDialogue()
    {
        string test_text = "test";
        string test_conversation = "1<page>2<page>3<page>4<page>567";
        DialogueUI.customDialogueManager.AddConversation(test_text, test_conversation);
        DialogueUI.customDialogueManager.ShowDialogue(test_text);

    }
    static void Log(object msg)
    {
        Rogue.Instance.Log(msg);
    }
}

