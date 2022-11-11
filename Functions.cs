class Functions
{
    static int lastCurrentKey = 0;
    static int currentKey = 0;
    static KeyboardKey currentKeyboardKey = 0;
    static float timer = 1;
    public static string getCharSet = "`1234567890-=+\\~!@#£€¦`¬$%^&*()_+|;:'\",.<>/?qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM[]{}\n ";
    
    public static void DrawCurvedBox(Texture2D texture, Rectangle rec, Color tint)
    {
        int m = 1;
        rec = new Rectangle(rec.x, rec.y, rec.width*m, rec.height*m);
        float ts = texture.width;
        float side = 1f/ts;
        float corner = (ts/2)/ts - side/2;
        ts *= m;
        Raylib.DrawTextureQuad(texture, new Vector2(corner, corner), new Vector2(0, 0), new Rectangle(rec.x, rec.y, corner*ts, corner*ts), tint);
        Raylib.DrawTextureQuad(texture, new Vector2(corner, corner), new Vector2(corner+side, 0), new Rectangle(rec.x + rec.width - corner*ts, rec.y, corner*ts, corner*ts), tint);

        Raylib.DrawTextureQuad(texture, new Vector2(corner, corner), new Vector2(0, corner+side), new Rectangle(rec.x, rec.y + rec.height - corner*ts, corner*ts, corner*ts), tint);
        Raylib.DrawTextureQuad(texture, new Vector2(corner, corner), new Vector2(corner+side, corner+side), new Rectangle(rec.x + rec.width - corner*ts, rec.y + rec.height - corner*ts, corner*ts, corner*ts), tint);
        

        Raylib.DrawTextureQuad(texture, new Vector2(side, corner), new Vector2(corner, 0), new Rectangle(rec.x + corner*ts, rec.y, rec.width - corner*ts*2, corner*ts), tint);
        Raylib.DrawTextureQuad(texture, new Vector2(side, corner), new Vector2(corner, corner+side), new Rectangle(rec.x + corner*ts, rec.y + rec.height - corner*ts, rec.width - corner*ts*2, corner*ts), tint);
        
        Raylib.DrawTextureQuad(texture, new Vector2(corner, side), new Vector2(0, corner), new Rectangle(rec.x, rec.y + corner*ts, corner*ts, rec.height - corner*ts*2), tint);
        Raylib.DrawTextureQuad(texture, new Vector2(corner, side), new Vector2(side+corner, corner), new Rectangle(rec.x + rec.width - corner*ts, rec.y + corner*ts, corner*ts, rec.height - corner*ts*2), tint);
        

        Raylib.DrawTextureQuad(texture, new Vector2(side, side), new Vector2(corner, corner), new Rectangle(rec.x + corner*ts, rec.y + corner*ts, rec.width - corner*ts*2, rec.height - corner*ts*2), tint);
        
    }
    public static void DrawCurvedBoxEx(Texture2D texture, Rectangle rec, int[] cornerModes, Color border, bool outerBorder = false, Color? inside = null)
    {
        //cornerModes = new int[]{2,2,1,1};
        /*
        int m = 1;
        rec = new Rectangle(rec.x, rec.y, rec.width*m, rec.height*m);
        float side = 1f/ts;
        float corner = (ts/2)/ts - side/2;
        ts *= m;
        */
        if (inside == null) inside = border;
        float w = texture.width;
        float h = texture.height;
        Rectangle newRec = rec;
        if (outerBorder) newRec = AddRect(rec, new Rectangle(-h,-h, h*2,h*2));
        //float side = 1/ts;
        float corner = (w-1)/3;
        //Raylib.DrawTexturePoly(texture, Vector2.One*100, new Vector2[]{Vector2.Zero*ts, Vector2.UnitY*ts, Vector2.UnitX*ts, Vector2.One*ts}, new Vector2[]{Vector2.Zero, Vector2.UnitY, Vector2.UnitX, Vector2.One}, 3, Color.WHITE);
        switch (cornerModes[0])
        {
            case 0: def0: Raylib.DrawTexturePro(texture, new Rectangle(1,0,-corner,h), new Rectangle(newRec.x,newRec.y,h,h), Vector2.Zero, 0, border); break;
            case 1: Raylib.DrawTexturePro(texture, new Rectangle(1+corner,0,-corner,h), new Rectangle(newRec.x,newRec.y,h,h), Vector2.Zero, 0, border); break;
            case 2: Raylib.DrawTexturePro(texture, new Rectangle(1+corner*2,0,-corner,-h), new Rectangle(newRec.x-h,newRec.y,h,h), Vector2.Zero, 0, border); goto def0;
            case 3:
                Raylib.DrawTexturePro(texture, new Rectangle(1+corner*2,0,corner,h), new Rectangle(newRec.x,newRec.y-h,h,h), Vector2.Zero, 0, border); goto def0;
            default: break;
        }
        switch (cornerModes[1])
        {
            case 0: def1: Raylib.DrawTexturePro(texture, new Rectangle(1,0,corner,h), new Rectangle(newRec.x + newRec.width - h,newRec.y,h,h), Vector2.Zero, 0, border); break;
            case 1: Raylib.DrawTexturePro(texture, new Rectangle(1+corner,0,corner,h), new Rectangle(newRec.x + newRec.width - h,newRec.y,h,h), Vector2.Zero, 0, border); break;
            case 2: Raylib.DrawTexturePro(texture, new Rectangle(1+corner*2,0,corner,-h), new Rectangle(newRec.x + newRec.width,newRec.y,h,h), Vector2.Zero, 0, border); goto def1;
            case 3: Raylib.DrawTexturePro(texture, new Rectangle(1+corner*2,0,-corner,h), new Rectangle(newRec.x + newRec.width - h,newRec.y - h,h,h), Vector2.Zero, 0, border); goto def1;
            default: break;
        }
        switch (cornerModes[2])
        {
            case 0: def2: Raylib.DrawTexturePro(texture, new Rectangle(1,0,-corner,-h), new Rectangle(newRec.x,newRec.y + newRec.height - h,h,h), Vector2.Zero, 0, border); break;
            case 1: Raylib.DrawTexturePro(texture, new Rectangle(1+corner,0,-corner,-h), new Rectangle(newRec.x,newRec.y + newRec.height - h,h,h), Vector2.Zero, 0, border); break;
            case 2: Raylib.DrawTexturePro(texture, new Rectangle(1+corner*2,0,-corner,h), new Rectangle(newRec.x-h,newRec.y + newRec.height - h,h,h), Vector2.Zero, 0, border); goto def2;
            case 3: Raylib.DrawTexturePro(texture, new Rectangle(1+corner*2,0,corner,-h), new Rectangle(newRec.x,newRec.y + newRec.height,h,h), Vector2.Zero, 0, border); goto def2;
            default: break;
        }
        switch (cornerModes[3])
        {
            case 0: def3: Raylib.DrawTexturePro(texture, new Rectangle(1,0,corner,-h), new Rectangle(newRec.x + newRec.width - h,newRec.y + newRec.height - h,h,h), Vector2.Zero, 0, border); break;
            case 1: Raylib.DrawTexturePro(texture, new Rectangle(1+corner,0,corner,-h), new Rectangle(newRec.x + newRec.width - h,newRec.y + newRec.height - h,h,h), Vector2.Zero, 0, border); break;
            case 2: Raylib.DrawTexturePro(texture, new Rectangle(1+corner*2,0,corner,h), new Rectangle(newRec.x + newRec.width,newRec.y + newRec.height - h,h,h), Vector2.Zero, 0, border); goto def3;
            case 3: Raylib.DrawTexturePro(texture, new Rectangle(1+corner*2,0,-corner,-h), new Rectangle(newRec.x + newRec.width - h,newRec.y + newRec.height,h,h), Vector2.Zero, 0, border); goto def3;
            default: break;
        }

        Raylib.DrawTexturePro(texture, new Rectangle(0,0,1,h), new Rectangle(newRec.x + h,newRec.y,newRec.width-h*2,h), Vector2.Zero, 0, border);
        Raylib.DrawTexturePro(texture, new Rectangle(0,0,1,-h), new Rectangle(newRec.x+h,newRec.y+h,newRec.height-h*2,h), Vector2.Zero, 90, border);
        Raylib.DrawTexturePro(texture, new Rectangle(0,0,1,h), new Rectangle(newRec.x+newRec.width,newRec.y+h,newRec.height-h*2,h), Vector2.Zero, 90, border);
        Raylib.DrawTexturePro(texture, new Rectangle(0,0,1,-h), new Rectangle(newRec.x + h,newRec.y + newRec.height - h,newRec.width-h*2,h), Vector2.Zero, 0, border);


        //Raylib.DrawTexturePro(texture, new Rectangle(1+corner,0,corner,h), rec, Vector2.Zero, 0, Color.BLUE);
        Raylib.DrawRectangleRec(AddRect(newRec, new Rectangle(h,h,-h*2,-h*2)), inside.Value);
    }
    public static bool StringToKeyBind(string keyBind)
    {
        string[] keys = keyBind.Split("+");
        bool ret = true;
        for (int i = 0; i < keys.Length; i++)
        {
            string key = keys[i];
            bool useDirections = false;
            if (key == "ctrl")
            {
                key = "control";
                useDirections = true;
            }
            else if (key == "win")
            {
                key = "super";
                useDirections = true;
            }
            else if (key=="alt"||key=="shift") useDirections = true;
            if (key == ",") key = "comma";
            KeyboardKey k1 = (KeyboardKey)Enum.Parse(typeof(KeyboardKey), "KEY_"+(useDirections?"LEFT_":"")+key.ToUpper());
            KeyboardKey k2 = (KeyboardKey)Enum.Parse(typeof(KeyboardKey), "KEY_"+(useDirections?"RIGHT_":"")+key.ToUpper());
            if (i == keys.Length-1) ret &= Raylib.IsKeyPressed(k1) || Raylib.IsKeyPressed(k2);
            else ret &= Raylib.IsKeyDown(k1) || Raylib.IsKeyDown(k2);
            //else ret &= keyIsDown;
        }
        
        return ret;
    }
    public static int GetKeyPressed()
    {
        for (int i = 1; i < 400; i++)
        {
            if (i == (int)KeyboardKey.KEY_LEFT_SHIFT || i == (int)KeyboardKey.KEY_RIGHT_SHIFT || i == (int)KeyboardKey.KEY_LEFT_CONTROL) continue;
            if (Raylib.IsKeyDown((KeyboardKey)i))
            {
                if (getCharSet.Contains((char)i)) goto getCharPressed;
                if (i == (int)KeyboardKey.KEY_ENTER) currentKey = 10;
                else currentKey = i;

                currentKeyboardKey = (KeyboardKey)i;
            }
        }
        if (Raylib.IsKeyReleased(currentKeyboardKey)) currentKey = 0;
        
        if (lastCurrentKey != currentKey)
        {
            timer = 0;
            lastCurrentKey = currentKey;
            return currentKey;
        }
        if (timer <= 1) timer += .025f;
        lastCurrentKey = currentKey;
        return timer>=1?currentKey:0;
        getCharPressed:
        return Raylib.GetCharPressed();
    }

    // a function that takes a string with a hex color and returns a Color object
    public static Color HexToColor(string hex, string alpha = "f")
    {
        hex = hex.Replace("#", "");
        //if (hex.Length == 6) hex += alpha.Length==1?alpha+alpha:alpha;
        //if (hex.Length == 3) hex += alpha.Length==2?alpha:alpha[0];
        byte r = 255;
        byte g = 255;
        byte b = 255;
        byte a = 255;
        if (hex.Length >= 6)
        {
            byte.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out r);
            byte.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out g);
            byte.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out b);
            if (hex.Length == 8) byte.TryParse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, null, out a);
        }
        else if (hex.Length >= 3)
        {
            byte.TryParse(String.Concat(Enumerable.Repeat(hex.Substring(0, 1),2)), System.Globalization.NumberStyles.HexNumber, null, out r);
            byte.TryParse(String.Concat(Enumerable.Repeat(hex.Substring(1, 1),2)), System.Globalization.NumberStyles.HexNumber, null, out g);
            byte.TryParse(String.Concat(Enumerable.Repeat(hex.Substring(2, 1),2)), System.Globalization.NumberStyles.HexNumber, null, out b);
            if (hex.Length == 4) byte.TryParse(String.Concat(Enumerable.Repeat(hex.Substring(3, 1),2)), System.Globalization.NumberStyles.HexNumber, null, out a);
        }
        else return new Color(255,0,255,255);
        // something broken
        if (alpha.Length == 1)
        {
            alpha += alpha;
        }

        if (a == 255) byte.TryParse(alpha, System.Globalization.NumberStyles.HexNumber, null, out a);
        return new Color(r, g, b, a);
    }
    // invert a color
    public static Color InvertColor(Color color)
    {
        return new Color(255 - color.r, 255 - color.g, 255 - color.b, color.a);
    }
    
    public static Font LoadFont(string path, int targetFontSize)
    {
        if (!path.Contains(".ttf")) path = "C:/Windows/Fonts/"+path+".ttf";
        
        Font ret;
        int[] arr = new int[1600];
        for (int i=0; i < arr.Length; i++) arr[i] = i;
        ret = Raylib.LoadFontEx(path, targetFontSize, arr, arr.Length);
        Raylib.SetTextureFilter(ret.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);
        return ret;
    }
    // return a random color
    public static Color RandomColor()
    {
        return new Color(
            (int)Math.Round((decimal)Raylib.GetRandomValue(0, 255)),
            (int)Math.Round((decimal)Raylib.GetRandomValue(0, 255)),
            (int)Math.Round((decimal)Raylib.GetRandomValue(0, 255)),
            (int)Math.Round((decimal)Raylib.GetRandomValue(0, 255))
        );
    }

    // a function that adds two rectangle objects 
    public static Rectangle AddRect(Rectangle a, Rectangle b)
    {
        return new Rectangle(a.x + b.x, a.y + b.y, a.width + b.width, a.height + b.height);
    }

    // check if mouse is on rectangle and is on screen
    public static bool IsMouseOnRect(Rectangle rect)
    {
        Vector2 mouse = Raylib.GetMousePosition();
        bool mouseOnRec = mouse.X >= rect.x && mouse.X <= rect.x+rect.width && mouse.Y >= rect.y && mouse.Y < rect.y+rect.height;
        return mouseOnRec && Raylib.IsCursorOnScreen();
    }
    static bool cursorHasBeenSet = false;
    static MouseCursor currentCursor = MouseCursor.MOUSE_CURSOR_DEFAULT;
    static MouseCursor newCursor = MouseCursor.MOUSE_CURSOR_DEFAULT;
    public static void SetMouseCursor(MouseCursor cursor)
    {
        newCursor = cursor;
    }
    public static void UpdateMouseCursor()
    {
        if (currentCursor != newCursor)
        {
            Raylib.SetMouseCursor(newCursor);
            currentCursor = newCursor;
        }
    }

    public static void ErrorMessage()
    {
        string caption = "File does not exist";

        Dialogs.MessageBox(Dialogs.MessageBoxButtons.Ok, Dialogs.MessageBoxIconType.Error, Dialogs.MessageBoxDefaultButton.OkYes, "", caption);
    }
}