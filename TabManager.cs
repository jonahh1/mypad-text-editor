class TabManager
{
    public static List<TextBoxInfo> tabs = new List<TextBoxInfo>();
    public static int current = 0;
    public static void OpenLastOpenedFiles()
    {
        foreach (string path in TextBoxInfo.data.lastOpenedFiles) OpenTab(path);
        if (tabs.Count == 0) NewTab();
    }
    public static void NewTab()
    {
        OpenTab("");
    }
    public static void OpenTab(string path)
    {
        string str = "";
        if (path != "") str = File.ReadAllText(path).Replace("\r", "");
        tabs.Add(new TextBoxInfo(path, str));
        current = tabs.Count - 1;
    }
    public static void CloseTab(int i)
    {
        tabs.RemoveAt(i);
        if (tabs.Count == 0) NewTab();
        if (i >= current)
        {
            current++;
            while (current > tabs.Count-1) current--;
        }
        else// </>
        {
            current--;
            while (current < 0) current++;
        }
        //if (current > i) current--;
        //else if (current == 0) current++;
    }
    public static TextBoxInfo Current()
    {
        return tabs[current];
    }
}