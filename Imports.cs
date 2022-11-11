
static class ButtonPlaces
{
    public static int options = 0;
    public static int search = 24;
    public static int caseSensitive = 48;
    public static int regex = 72;
    public static int plus = 96;
    public static int resize = 120;
    public static int minimize = 144;
    public static int maximize = 168;
    public static int close = 192;
}
public static class Imports
{
    public static Font font;
    public static Font firaCode;
    public static Texture2D icon;
    public static Image iconBig;
    public static Texture2D curvedBox;

    public static Texture2D icons;

    public static void LoadAll()
    {
        font      = Functions.LoadFont(OptionsMenu.config.font, OptionsMenu.config.fontSize);
        firaCode  = Functions.LoadFont("assets/font.ttf", 24);
        icon      = Raylib.LoadTexture("assets/icon.png");
        iconBig   = Raylib.LoadImage  ("assets/icon_big.png");
        curvedBox = Raylib.LoadTexture("assets/frames/frame_3.png");
        icons = Raylib.LoadTexture("assets/icons.png");
        
        //Raylib.SetTextureFilter(Imports.icons, TextureFilter.TEXTURE_FILTER_BILINEAR);
        Raylib.SetTextureFilter(Imports.firaCode.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);
        if (OptionsMenu.config.fontFilter=="point") Raylib.SetTextureFilter(Imports.font.texture, TextureFilter.TEXTURE_FILTER_POINT);
        else Raylib.SetTextureFilter(Imports.font.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);
        
        if (OptionsMenu.config.outlineHexColors)
        {
            foreach (var t in OptionsMenu.config.syntax)
            {
                if (!t.Value.ContainsKey("\\#[A-Fa-f0-9]+")) t.Value.Add("\\#[A-Fa-f0-9]+", "self");
            }
        }
    }
    public static void UnloadAll()
    {
        Raylib.UnloadFont(font);
        Raylib.UnloadFont(firaCode);
        Raylib.UnloadTexture(icon);
        Raylib.UnloadImage(iconBig);
        Raylib.UnloadTexture(curvedBox);
        Raylib.UnloadTexture(icons);
    }
}