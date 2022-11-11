public class OptionsMenu
{
    public static Config config;
    public delegate void Function(string input = "");
    public static Function func;
    public static bool showMenu = false;
    public static Rectangle rec;
    public static void Options()
    {
        List<Option> options = new List<Option>();
        //Dictionary<string, Function?> buttons = new Dictionary<string, Function?>();
        options.Add(new Option(){
            name = "new file",
            function = NewFile,
            KeyBind = Functions.StringToKeyBind(OptionsMenu.config.keybinds["new file"]),
        });
        options.Add(new Option(){
            name = "open file",
            function = OpenFile,
            KeyBind = Functions.StringToKeyBind(OptionsMenu.config.keybinds["open file"])
        });
        options.Add(new Option(){name = "[line]"});
        options.Add(new Option(){
            name = "save",
            function = Save,
            KeyBind = Functions.StringToKeyBind(OptionsMenu.config.keybinds["save"]),
        });
        options.Add(new Option(){
            name = "save as",
            function = SaveAs,
            KeyBind = Functions.StringToKeyBind(OptionsMenu.config.keybinds["save as"]),
        });
        options.Add(new Option(){name = "[line]"});
        options.Add(new Option(){
            name = "copy",
            function = Copy,
            KeyBind = Functions.StringToKeyBind(OptionsMenu.config.keybinds["copy"]),
        });
        options.Add(new Option(){
            name = "paste",
            function = Paste,
            KeyBind = Functions.StringToKeyBind(OptionsMenu.config.keybinds["paste"]),
        });
        options.Add(new Option(){
            name = "undo",
            function = Undo,
            KeyBind = Functions.StringToKeyBind(OptionsMenu.config.keybinds["undo"]),
        });
        options.Add(new Option(){name = "[line]"});
        options.Add(new Option(){
            name = "open config",
            function = OpenConfig,
            KeyBind = Functions.StringToKeyBind(OptionsMenu.config.keybinds["open config"]),
        });
        options.Add(new Option(){name = "[line]"});
        options.Add(new Option(){
            name = "reload window",
            function = ReloadWindow,
            KeyBind = Functions.StringToKeyBind(OptionsMenu.config.keybinds["reload window"]),
        });

        foreach (var option in options)
        {
            if (option.KeyBind && option.function != null) option.function();
        }
        if (!showMenu) return;
        
        float h = 20;
        Color fgCol = Functions.HexToColor(OptionsMenu.config.colors["UI elements"]);
        Color bgCol = Functions.HexToColor(OptionsMenu.config.colors["UI border"]);
        Color highlight = Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]);

        rec = new Rectangle(4 + Imports.curvedBox.height, 24 + Imports.curvedBox.height, 200, (h*options.Where(o => o.name != "[line]").Count() + (h/2)*options.Where(o => o.name == "[line]").Count()));
        if (Functions.IsMouseOnRect(rec)) Functions.SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT);

        Functions.DrawCurvedBoxEx(Imports.curvedBox, rec, new int[]{0,2,3,1}, bgCol, true);
        float y = rec.y;
        foreach (Option option in options)
        {
            if (option.name == "[line]")
            {
                Raylib.DrawLineEx(new Vector2(rec.x, y+h/4), new Vector2(rec.x + rec.width, y+h/4), 1, fgCol);
                y += h/2;
                continue;
            }
            Rectangle irec = new Rectangle(rec.x, y, rec.width, h);
            if (Functions.IsMouseOnRect(irec))
            {
                Raylib.DrawRectangleRec(irec, highlight);//Raylib.DrawRectangleRec(irec, highlight);
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && option.function != null)
                {
                    option.function();
                    showMenu = false;
                }
            }
            Raylib.DrawTextEx(Imports.firaCode, option.name, new Vector2(irec.x, irec.y + (h-16)/2), 16, 0, fgCol);
            Raylib.DrawTextEx(Imports.firaCode, OptionsMenu.config.keybinds[option.name], new Vector2(irec.x + irec.width - Raylib.MeasureTextEx(Imports.firaCode, OptionsMenu.config.keybinds[option.name], 16, 0).X, irec.y + (h-16)/2), 16, 0, fgCol);
            y += h;
        }
        if (Functions.IsMouseOnRect(rec)) Functions.SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT);
    }
    public static void NewFile(string input = "") { TabManager.NewTab(); }
    public static void OpenFile(string input = "")
    {
        if (input != "") goto jump;
        bool canceled = false;
        var rawResults = Dialogs.OpenFileDialog();
        if (rawResults == null) return;
        string[] results = rawResults.ToArray();
        foreach (var result in results)
        {
            if (result != null)
            {
                input = result;
                goto jump;
            }
            else canceled = true;
        }
        if (input == "" && !canceled)
        {
            Functions.ErrorMessage();
            NewFile();
            return;
        }
        jump:
        if (File.Exists(input)) TabManager.OpenTab(input);
        else Functions.ErrorMessage();
    }
    public static void Save(string input = "")
    {
        TextBoxInfo tab = TabManager.Current();
        if (input != "") tab = TabManager.tabs[Convert.ToInt32(input)];
        if (TabManager.Current().filePath != "")
        {    
            File.WriteAllText(tab.filePath, tab.str);
            tab.savedStr = tab.str;
        }
        else SaveAs();
        return;
    }
    public static void SaveAs(string input = "")
    {
        string result = Dialogs.SaveFileDialog();
        bool canceled = false;
        if (result != null)  input = result;
        else canceled = true;

        if (input == "" || canceled) return;
        
        TabManager.Current().filePath = input;
        Save();
        return;

    }

    public static void OpenConfig(string input = "")
    {
        OpenFile("assets/config.json");
    }

    public static void Paste(string input = "")
    {
        string paste = TextCopy.ClipboardService.GetText();
        if (paste == null) return;
        paste = paste.Replace("\r", "");
        TabManager.Current().str = TabManager.Current().str.Insert((int)TabManager.Current().pointer.Z,paste);
        TabManager.Current().splitStr = TabManager.Current().str.Split("\n");
        TabManager.Current().pointer.Z += paste.Length;
        TabManager.Current().pointer.Y += paste.Where(c => c == '\n').Count();
        if (paste.Split("\n").Length > 1) TabManager.Current().pointer.X = paste.Split("\n").Last().Length;
        else TabManager.Current().pointer.X += paste.Length;
        TabManager.Current().strHistory.Add(new StrState(){str=TabManager.Current().str, pointer=TabManager.Current().pointer});
        TabManager.Current().secondPointer = TabManager.Current().pointer;
    }
    public static void Copy(string input = "")
    {
        string copy = TabManager.Current().splitStr[(int)TabManager.Current().pointer.Y];
        if (TabManager.Current().pointer != TabManager.Current().secondPointer)
        {
            var p = TabManager.Current().pointer;
            var sp = TabManager.Current().secondPointer;
            if (p.Z < sp.Z) copy = TabManager.Current().str.Substring((int)p.Z, (int)(sp.Z-p.Z));
            else copy = TabManager.Current().str.Substring((int)sp.Z, (int)(p.Z-sp.Z));
        }
        TextCopy.ClipboardService.SetText(copy);
    }
    public static void Undo(string input = "")
    {
        //string copy = splitStr[(int)pointer.Y];
        //TextCopy.ClipboardService.SetText(copy);
        if (TabManager.Current().strHistory.Count <= 1) return;
        TabManager.Current().str = TabManager.Current().strHistory[TabManager.Current().strHistory.Count-2].str;
        TabManager.Current().pointer = TabManager.Current().strHistory[TabManager.Current().strHistory.Count-2].pointer;
        TabManager.Current().splitStr = TabManager.Current().str.Split("\n");
        TabManager.Current().strHistory.RemoveAt(TabManager.Current().strHistory.Count-1);
        TabManager.Current().secondPointer = TabManager.Current().pointer;
    }

    public static void ReloadWindow(string input = "")
    {
        try {OptionsMenu.config = JsonSerializer.Deserialize<Config>(File.ReadAllText("assets/config.json"))!;}
        catch {OptionsMenu.config = JsonSerializer.Deserialize<Config>("{  \"lineNumbers\": true,  \"font\": \"assets/FiraCode-Light.ttf\",  \"fontFilter\": \"point\",  \"fontSize\": 20,  \"fontSpacing\": 0,  \"keybinds\": {    \"new file\": \"ctrl+n\",    \"open file\": \"ctrl+o\",    \"save\": \"ctrl+s\",    \"save as\": \"ctrl+shift+s\",    \"copy\": \"ctrl+c\",    \"paste\": \"ctrl+v\",    \"undo\": \"ctrl+z\",    \"open config\": \"ctrl+,\",    \"reload window\": \"ctrl+r\"  },  \"colors\": {    \"background\": \"#181818\",    \"text\": \"#ccc\",    \"line numbers\": \"#dd7\",    \"UI border\": \"#222\",    \"UI elements\": \"#ccc\",    \"UI highlight\": \"#22f6\"  }}")!;}
        
        Imports.UnloadAll();
        Imports.LoadAll();
    }

    public static bool AreAllFilesSaved()
    {
        for (int I=0; I < TabManager.tabs.Count; I++)
        {
            var i = TabManager.tabs[I];
            if (i.str != i.savedStr)
            {
                bool result = IsFileSaved(I);
                if (result == false) return false;
            }
        }
        return true;
    }
    public static bool IsFileSaved(int input)
    {
        var tab = TabManager.tabs[input];
        if (tab.str != tab.savedStr)
        {
            string name = tab.windowTitle==""?"new file":tab.windowTitle;
            string caption = name+" not saved";
            string message = name+" is not saved, do you want to save it?";
            int result = Dialogs.MessageBox(Dialogs.MessageBoxButtons.YesNo, Dialogs.MessageBoxIconType.Warning, Dialogs.MessageBoxDefaultButton.OkYes, caption, message);
            //DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            /*save*/if (result == 1) Save(input.ToString());
            /* no */else if (result != 0) return false;
        }
        return true;
    }
}