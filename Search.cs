public class SearchMenu
{
    public static bool showMenu = false;
    public static Rectangle rec = new Rectangle(0,0,0,0);
    public static string searchString = "";
    static int pointer = 0;


    static bool caseSensitive = false;
    static bool useRegex = true;
    public static void Search()
    {
        if (!showMenu) return;
        Color fgCol = Functions.HexToColor(OptionsMenu.config.colors["UI elements"]);
        Color bgCol = Functions.HexToColor(OptionsMenu.config.colors["UI border"]);
        Color highlight = Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]);
        rec.x = 24 + Imports.curvedBox.height;
        rec.y = 24 + Imports.curvedBox.height;
        rec.height = 24;
        Vector2 textSize = Raylib.MeasureTextEx(Imports.firaCode, searchString, 16, 0);

        Rectangle caseRec = new Rectangle(rec.x+rec.width-48, rec.y, 24, 24);
        Rectangle regRec = new Rectangle(rec.x+rec.width-24, rec.y, 24, 24);
        caseRec = Functions.AddRect(caseRec, new Rectangle(2,2,-4,-4));
        regRec = Functions.AddRect(regRec, new Rectangle(2,2,-4,-4));

        if (Functions.IsMouseOnRect(caseRec) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) caseSensitive = !caseSensitive;
        if (Functions.IsMouseOnRect(regRec) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) useRegex = !useRegex;


        //Raylib.DrawRectangleRec(rec, Functions.HexToColor(OptionsMenu.config.colors["UI border"]));
        
        TextBoxInfo.AlterTextMonoline(ref pointer, ref searchString);

        rec.width = Math.Max(200, textSize.X+53);

        List<Option> results = new List<Option>();
        //MatchCollection matches;// = new MatchCollection();
        string tempStr = TabManager.Current().str; // temporary str
        string match = findMatch(tempStr);
        int times = 0;
        while (match != "")
        {
            times++;
            if (times > 100) break;
            //match = findMatch(tempStr);//Regex.Match(tempStr, searchString, caseSensitive?RegexOptions.None:RegexOptions.IgnoreCase).Value;
            int index = tempStr.IndexOf(match);
            //if (!caseSensitive) index = tempStr.ToUpper().IndexOf(match.ToUpper());
            //else index = tempStr.IndexOf(match);
            Vector2 where = Vector2.Zero;
            for (int a = 0; a < index; a++)
            {
                if (TabManager.Current().str[a] == '\n')
                {
                    where.Y++;
                    where.X=0;
                }
                else where.X++;
            }
            string name = match + " at line " + where.Y + ", index " + where.X;
            results.Add(new Option(){
                name = name,
                function = GoToLine
            });
            rec.width = Math.Max(rec.width, Raylib.MeasureTextEx(Imports.firaCode, name, 16, 0).X);
            if (index >= 0)
            {
                tempStr = tempStr.Remove(index, match.Length);
                tempStr = tempStr.Insert(index, new string(match[0]==' '?'n':' ', match.Length));
            }
            match = findMatch(tempStr);
        }
        int h = 20;
        int by = 6;
        rec.height += h*results.Count() + by;
        // background
        Functions.DrawCurvedBoxEx(Imports.curvedBox, rec, new int[]{2,2,1,1}, bgCol, true);
        
        // search string
        Raylib.DrawTextEx(Imports.firaCode, searchString==""?"search":searchString, new Vector2(rec.x, rec.y + (24-16)/2), 16, 0, fgCol);
        
        // vertical dividing line
        Raylib.DrawLineEx(new Vector2(rec.x + (rec.width-48)-2.5f, rec.y), new Vector2(rec.x + (rec.width-48)-2.5f, rec.y+26), 1, fgCol);
        
        // horizontal dividing line
        Raylib.DrawLineEx(new Vector2(rec.x, rec.y+26), new Vector2(rec.x + rec.width, rec.y+26), 1, fgCol);
        
        // pointer
        int px = (int)Raylib.MeasureTextEx(Imports.firaCode, searchString.Remove(pointer), 16,0).X; // pointer x;
        Raylib.DrawLineEx(new Vector2(rec.x + px, rec.y + (24-16)/2), new Vector2(rec.x + px,  rec.y + 16+(24-16)/2), 1, fgCol);
        
        // searchOptions
        if (caseSensitive || Functions.IsMouseOnRect(caseRec)) Functions.DrawCurvedBoxEx(Imports.curvedBox, caseRec, new int[]{1,1,1,1}, highlight);
        if (useRegex || Functions.IsMouseOnRect(regRec)) Functions.DrawCurvedBoxEx(Imports.curvedBox, regRec, new int[]{1,1,1,1}, highlight);
        //Raylib.DrawTexture(Imports.regex, (int)(rec.x+rec.width)-48, (int)rec.y, fgCol);
        Raylib.DrawTexturePro(Imports.icons, new Rectangle(48,0,48,24), new Rectangle(rec.x+rec.width-48,rec.y,48,24), new Vector2(0,0), 0, fgCol);
        
        float i = 0;
        foreach (var result in results)
        {
            Rectangle irec = new Rectangle(rec.x, by+rec.y+24+(i*h), rec.width, h);
            Vector2 nameSize = Raylib.MeasureTextEx(Imports.firaCode, result.name, 16, 0);
            if (Functions.IsMouseOnRect(irec))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    if (result.function != null)
                    {
                        result.function(result.name);
                        showMenu = false;
                    }
                }
                Raylib.DrawRectangleRec(irec, highlight);
            }

            Raylib.DrawTextEx(Imports.firaCode, result.name, new Vector2(irec.x, irec.y + (irec.height - nameSize.Y)/2), 16, 0, fgCol);
            i++;
        }
        if (Functions.IsMouseOnRect(rec)) Functions.SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT);
    }

    public static void GoToLine(string input = "")
    {
        string[] splitIn = Regex.Split(input, " at line |, index ");
        TabManager.Current().pointer.X = Convert.ToSingle(splitIn[2]);
        TabManager.Current().pointer.Y = Convert.ToSingle(splitIn[1]);
        int Z = 0;
        for (int i = 0; i < TabManager.Current().pointer.Y; i++) Z += TabManager.Current().splitStr[i].Length + 1;
        Z += (int)TabManager.Current().pointer.X;
        TabManager.Current().pointer.Z = Z;

        TabManager.Current().scroll.Y = -TabManager.Current().pointer.Y;

        TabManager.Current().secondPointer = TabManager.Current().pointer;
    }

    static string findMatch(string str)
    {
        if (useRegex)
        {
            try{return Regex.Match(str, searchString, caseSensitive?RegexOptions.None:RegexOptions.IgnoreCase).Value;}
            catch{return"";}
            
        }
        else
        {
            string STR = str.ToUpper();
            string SEARCHSTRING = searchString.ToUpper();

            if (!caseSensitive)
            {
                if (STR.Contains(SEARCHSTRING)) return searchString;
            }
            else 
            {
                if (str.Contains(searchString)) return searchString;
            }
        }
        return "";
    }
}