public class Config
{
    // #272736
    // #43434f
    // #242424
    // #ccc
    public bool lineNumbers {get; set;}
    public bool outlineHexColors {get; set;}
    
    public string font {get; set;}
    public string fontFilter {get; set;}
    public int fontSize {get; set;}
    public float fontSpacing {get; set;}
    public int tabLength {get; set;}

    public Dictionary<string, string> keybinds {get; set;}
    public Dictionary<string, string> colors {get; set;}
    public Dictionary<string, Dictionary<string, string>> syntax {get; set;}
}
public class Data
{
    public List<string> lastOpenedFiles {get; set;}
}
public class Option
{
    public string name;
    public OptionsMenu.Function? function = null;
    public bool KeyBind = false;
}

public class StrState
{
    public string str;
    public Vector3 pointer;
}