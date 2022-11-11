// (^|[^/])//[^/] search for comments this way
global using Raylib_cs;
global using System.Numerics;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using TinyDialogsNet;
int w = 800;
int h = 450;
//int result = Dialogs.MessageBox(Dialogs.MessageBoxButtons.YesNo, Dialogs.MessageBoxIconType.Warning, Dialogs.MessageBoxDefaultButton.OkYes, "r r ", " r  r e");
//DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
OptionsMenu.config = JsonSerializer.Deserialize<Config>(File.ReadAllText("assets/config.json"))!;
TabManager.OpenLastOpenedFiles();
//&& !t.Value.ContainsKey("\\#[A-Fa-f0-9]+")) t.Value.Add("\\#[A-Fa-f0-9]+", "self"); 
string windowTitle = TabManager.Current().filePath;

//OptionsMenu.OpenFile(TabManager.GetCurrent().data.lastOpenedFile);

bool closeWindow = false;
bool isDraggable = false;
bool isResizable = false;


Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_UNDECORATED);
Raylib.InitWindow(w, h, windowTitle);
Imports.LoadAll();
Raylib.SetTargetFPS(60);
Raylib.SetExitKey(0);
Raylib.SetWindowIcon(Imports.iconBig);
Vector2 dragOffset = Vector2.Zero;
Vector2 windowPosition = Raylib.GetWindowPosition();
Vector2 windowSize = new Vector2(w,h);
Raylib.SetTextureFilter(Imports.font.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);

int tabHeight = 24;

TabManager.Current().strHistory.Add(new StrState(){str=TabManager.Current().str, pointer=TabManager.Current().pointer});
while (!Raylib.WindowShouldClose() && !closeWindow)
{
    // file dropping
    if (Raylib.IsFileDropped())
    {
        string[] droppedFiles = Raylib.GetDroppedFiles();
        for (int i = 0; i < droppedFiles.Length; i++) OptionsMenu.OpenFile(droppedFiles[i]);
        Raylib.ClearDroppedFiles();
    }
    if (OptionsMenu.showMenu && !Functions.IsMouseOnRect(OptionsMenu.rec) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) OptionsMenu.showMenu = false;
    if (SearchMenu.showMenu && !Functions.IsMouseOnRect(SearchMenu.rec) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) SearchMenu.showMenu = false;
    bool noMenuIsOpen = !OptionsMenu.showMenu && !SearchMenu.showMenu;
    // cap the strHistory size to 100
    if (TabManager.Current().strHistory.Count > 1000) TabManager.Current().strHistory.RemoveAt(0);
    // set the window title
    windowTitle = Path.GetFileName(TabManager.Current().filePath) + (TabManager.Current().str!=TabManager.Current().savedStr?"*":"");

    // TabManager.GetCurrent().scroll functionality
    if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
    {
        TabManager.Current().scroll.X += (int)Raylib.GetMouseWheelMove();
        TabManager.Current().scroll.X = Math.Min(0,TabManager.Current().scroll.X);
        TabManager.Current().scroll.X = Math.Max(-TabManager.Current().splitStr.Max().Length+1,TabManager.Current().scroll.X);
    }
    else
    {
        TabManager.Current().scroll.Y += (int)Raylib.GetMouseWheelMove();
        TabManager.Current().scroll.Y = Math.Min(0,TabManager.Current().scroll.Y);
        TabManager.Current().scroll.Y = Math.Max(-TabManager.Current().splitStr.Length+1,TabManager.Current().scroll.Y);
    }
    
    // defining useful variables
    TabManager.Current().splitStr = TabManager.Current().str.Split("\n");
    w = Raylib.GetScreenWidth();
    h = Raylib.GetScreenHeight();

    // change the window title seen in the window manager
    if (!Raylib.IsCursorOnScreen()) Raylib.SetWindowTitle(windowTitle);
   

    // resize button
    Rectangle resizeRec = new Rectangle(w-24,h-24, 24,24);
    if (Functions.IsMouseOnRect(resizeRec) && !Raylib.IsWindowMaximized())
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            isResizable = true;
        }
        Functions.SetMouseCursor(MouseCursor.MOUSE_CURSOR_RESIZE_NWSE);
    }

    // move button
    int optionsWidth = 48;
    int winButtonsWidth = 72;
    if (Functions.IsMouseOnRect(new Rectangle(optionsWidth, 0, w-(optionsWidth+winButtonsWidth), 24)) && !Raylib.IsWindowMaximized())
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            dragOffset = Raylib.GetMousePosition();
            isDraggable = true;
        }
    }
    // reset isDraggable and isResizable
    if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
    {
        isDraggable = false;
        isResizable = false;
    }

    if (isDraggable)
    {
        windowPosition += Raylib.GetMousePosition() - dragOffset;
        Raylib.SetWindowPosition((int)windowPosition.X, (int)windowPosition.Y);
    }
    else if (isResizable)
    {
        windowSize += Raylib.GetMouseDelta();
        windowSize = Vector2.Max(new Vector2(300,72 + OptionsMenu.config.fontSize), windowSize);
        Raylib.SetWindowSize((int)windowSize.X, (int)windowSize.Y);
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Functions.HexToColor(OptionsMenu.config.colors["UI border"]));
    Rectangle textArea = new Rectangle(4, 24+tabHeight, w, h);
    textArea.height -= textArea.y+24;
    textArea.width -= textArea.x*2;
    Rectangle tabArea = new Rectangle(textArea.x, textArea.y - tabHeight, textArea.width, tabHeight);
    Functions.DrawCurvedBoxEx(Imports.curvedBox, textArea, new int[]{0,0,1,1}, Functions.HexToColor(OptionsMenu.config.colors["text background"]));
    Functions.DrawCurvedBoxEx(Imports.curvedBox, tabArea, new int[]{1,1,0,0}, Functions.HexToColor(OptionsMenu.config.colors["tab background"]));

    Rectangle newTab = new Rectangle(tabArea.x, tabArea.y, 24, 24);
    if (Functions.IsMouseOnRect(newTab) && noMenuIsOpen)
    {
        Functions.DrawCurvedBoxEx(Imports.curvedBox, Functions.AddRect(newTab, new Rectangle(2,2,-4,-4)), new int[]{1,1,1,1}, Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]));
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            //tabs++;
            TabManager.NewTab();
        }
    }
    //Raylib.DrawTextureEx(Imports.plus, new Vector2(tabArea.x, tabArea.y), 0, 1, Functions.HexToColor(OptionsMenu.config.colors["UI elements"]));
    Raylib.DrawTexturePro(Imports.icons, new Rectangle(ButtonPlaces.plus, 0, 24, 24), new Rectangle((int)tabArea.x, (int)tabArea.y,24,24), Vector2.Zero, 0, Functions.HexToColor(OptionsMenu.config.colors["UI elements"]));
    float tabX = tabArea.x + 24;
    // tabs
    for (int i = 0; i < TabManager.tabs.Count; i++)
    {
        string tabTitle = TabManager.tabs[i].windowTitle;
        if (tabTitle == "") tabTitle = "new file";
        tabTitle += (TabManager.tabs[i].str!=TabManager.tabs[i].savedStr?"*":"");
        Vector2 textSize = Raylib.MeasureTextEx(Imports.firaCode, tabTitle, 16, 0);
        int tabWidth = (int)textSize.X + 24 + Imports.curvedBox.height*2;
        Rectangle tab = new Rectangle(tabX, tabArea.y, tabWidth, tabHeight);
        Color tabCol = Functions.HexToColor(OptionsMenu.config.colors["tab foreground"]);
        if (Functions.IsMouseOnRect(tab) && noMenuIsOpen)
        {
            tabCol =  Functions.HexToColor(OptionsMenu.config.colors["tab background"]);
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) TabManager.current = i;
        }
        if (i == TabManager.current) tabCol = Functions.HexToColor(OptionsMenu.config.colors["text background"]);
        else tabCol = Functions.HexToColor(OptionsMenu.config.colors["tab foreground"]);
        Functions.DrawCurvedBoxEx(Imports.curvedBox, tab, new int[]{1,1,0,0}, tabCol);
        
        Rectangle closeTab = new Rectangle(tab.x + tab.width - 24, tab.y, 24, 24);
        if (Functions.IsMouseOnRect(closeTab) && noMenuIsOpen)
        {
            Functions.DrawCurvedBoxEx(Imports.curvedBox, Functions.AddRect(closeTab, new Rectangle(2,2,-4,-4)), new int[]{1,1,1,1}, Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]));
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
            {
                //OptionsMenu.AreAllFilesSaved(i);
                if (OptionsMenu.IsFileSaved(i)) TabManager.CloseTab(i);
            }
        }
        // close button
        Raylib.DrawTexturePro(Imports.icons, new Rectangle(ButtonPlaces.close, 0, 24, 24), closeTab, Vector2.Zero, 0, Functions.HexToColor(OptionsMenu.config.colors["UI elements"]));
        // tab text
        Raylib.DrawTextEx(Imports.firaCode, tabTitle, new Vector2(tabX + Imports.curvedBox.height, tabArea.y + (tabHeight-textSize.Y)/2), 16, 0, Functions.HexToColor(OptionsMenu.config.colors["text foreground"]));
        tabX += tabWidth+1;
    }

    //Raylib.DrawTexture(Imports.resize, w-24, h-24, Functions.HexToColor(OptionsMenu.config.colors["UI elements"]));
    Raylib.DrawTexturePro(Imports.icons, new Rectangle(ButtonPlaces.resize, 0, 24, 24), new Rectangle(w-24, h-24,24,24), Vector2.Zero, 0, Functions.HexToColor(OptionsMenu.config.colors["UI elements"]));
        
    
    // alter the text based on input
    if (noMenuIsOpen) TabManager.Current().AlterTextMultiline();
    
    #region drawing text
    Raylib.BeginScissorMode((int)textArea.x, (int)textArea.y, (int)textArea.width, (int)textArea.height);
    Vector2 textPos = new Vector2(textArea.x+TabManager.Current().scroll.X*OptionsMenu.config.fontSize, textArea.y+(TabManager.Current().scroll.Y*OptionsMenu.config.fontSize));
    Vector2 linePosition = new Vector2(0,0);
    int xOffset = 8;// + OptionsMenu.config.fontSize/2;
    Color color = Functions.HexToColor(OptionsMenu.config.colors["text foreground"]);
    for (int i = 0; i < TabManager.Current().splitStr.Length; i++)
    {
        int lineNumbersWidth = (OptionsMenu.config.lineNumbers?OptionsMenu.config.fontSize/2 * (TabManager.Current().splitStr.Length-1).ToString().Length:0);

        // allows you to change the style of a line
        
        // the position of the text
        linePosition = textPos + new Vector2(
            xOffset + lineNumbersWidth + OptionsMenu.config.fontSize/2,
            (i)*OptionsMenu.config.fontSize
        );
        //linePosition.Y -= 4;

        // drawing the line of text
        string completed = "";
        for (int chr = 0; chr < TabManager.Current().splitStr[i].Length; chr++)
        {
            bool drawBackground = false;
            string chrStr = TabManager.Current().splitStr[i][chr].ToString();
            
            Color bg = Functions.HexToColor(OptionsMenu.config.colors["text background"]);
            Color col = color;
            foreach (var t in OptionsMenu.config.syntax)
            {
                if (Path.GetExtension(TabManager.Current().filePath) == t.Key)
                {
                    foreach (var m in t.Value)
                    {
                        MatchCollection regex = Regex.Matches(TabManager.Current().splitStr[i], m.Key, RegexOptions.Singleline);
                        List<int> matches = splitMatches(regex);
                        foreach (int k in matches)
                        {
                            if (chr == k)
                            {
                                if (m.Value == "self")
                                {
                                    string correctMatch = "#f0f";
                                    foreach (Match match in regex) if (k >= match.Index && k  <= match.Index+match.Value.Length-1) correctMatch = match.Value;
                                    bg = Functions.HexToColor(correctMatch);
                                    // get the average of bg
                                    int avg = (bg.r + bg.g + bg.b) / 3;
                                    if (avg > 128) col = Color.BLACK;
                                    else col = Color.WHITE;
                                    //Color bg = Functions.HexToColor(OptionsMenu.config.colors["text background"]);
                                    drawBackground = true;//(Math.Abs(col.r - bg.r) + Math.Abs(col.g - bg.g) + Math.Abs(col.b - bg.b) < 100) || col.a < 50;
                                }
                                else col = Functions.HexToColor(m.Value);
                            }
                        }
                    }
                }
            }
            Vector2 charPos = linePosition + new Vector2(Raylib.MeasureTextEx(Imports.font, completed, OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing).X, 0);// - /*Vector2.UnitY*OptionsMenu.config.fontSize*/;
            //Color invert = new Color(255-col.r, 255-col.g, 255-col.b, 128);
            if (drawBackground)
            {
                Rectangle background = new Rectangle(
                    (int)charPos.X,
                    (int)charPos.Y,
                    (int)Raylib.MeasureTextEx(Imports.font, chrStr, OptionsMenu.config.fontSize, 0).X + OptionsMenu.config.fontSpacing,
                    (int)OptionsMenu.config.fontSize
                );
                Raylib.DrawRectangleRec(background, bg);
                //Raylib.DrawTextEx(Imports.font, chrStr, charPos, OptionsMenu.config.fontSize + newLineHeight, OptionsMenu.config.fontSpacing, invert);
            }
            //Raylib.DrawTextEx(Imports.font, chrStr, charPos, OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing, col);
            Raylib.DrawTextCodepoint(Imports.font, (int)chrStr[0], charPos, OptionsMenu.config.fontSize, col);
            completed += chrStr;
        }
        //Raylib.DrawTextEx(Imports.font, TabManager.GetCurrent().splitStr[i], linePosition, OptionsMenu.config.fontSize + newLineHeight, OptionsMenu.config.fontSpacing, color);

        // line numbers
        if (OptionsMenu.config.lineNumbers)
        {
            // the position of the numbers
            Vector2 numberPosition = new Vector2(
                xOffset + lineNumbersWidth - Raylib.MeasureTextEx(Imports.firaCode, i.ToString(), OptionsMenu.config.fontSize, 0).X,
                linePosition.Y
            );
            //Raylib.DrawRectangleRec(new Rectangle(0, linePosition.Y, lineNumbersWidth, Options.config.fontSize + newLineHeight), Functions.HexToColor(Options.config.colors["text"]));
            // drawing the line numbers
            Raylib.DrawRectangleV(new Vector2(4, linePosition.Y), new Vector2(lineNumbersWidth+8, OptionsMenu.config.fontSize), Functions.HexToColor(OptionsMenu.config.colors["text background"]));
            Raylib.DrawTextEx(Imports.firaCode, i.ToString(), numberPosition, OptionsMenu.config.fontSize, 0, Functions.HexToColor(OptionsMenu.config.colors["line numbers"]));
        }
    }
    //Raylib.DrawRectangleRec(Functions.AddRect(textArea, new Rectangle(0,0,-(textArea.width-lineNumbersWidth-4),0)), Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]));
        
    
    // move the text pointer to the mouse position if the left mouse button is pressed
    #region text pointer
    Vector2 mousePos = Raylib.GetMousePosition();
    if (!Functions.IsMouseOnRect(textArea)) goto skipPointer;
    #region Y
        Dictionary<float, int> charHeightsDict = new Dictionary<float, int>();
        float cy = textPos.Y + OptionsMenu.config.fontSize/2;
        for (int i = 0; i < TabManager.Current().splitStr.Length; i++)
        {
            charHeightsDict.Add(cy, i);
            if (i < TabManager.Current().splitStr.Length) cy += OptionsMenu.config.fontSize;
        }
        var closestY = charHeightsDict.First();
        foreach (var charHeight in charHeightsDict)
        {
            if (Math.Abs(charHeight.Key - mousePos.Y) < Math.Abs(closestY.Key - mousePos.Y)) closestY = charHeight;
        }
    #endregion

    #region X
        Dictionary<float, int> charWidthsDict = new Dictionary<float, int>();
        float cx = linePosition.X;
        string line = TabManager.Current().splitStr[closestY.Value];
        for (int i = 0; i <= line.Length; i++)
        {
            charWidthsDict.Add(cx, i);
            if (i < line.Length)
                cx += Raylib.MeasureTextEx(Imports.font, line[i].ToString(), OptionsMenu.config.fontSize, 0).X + OptionsMenu.config.fontSpacing;;
        }
        var closestX = charWidthsDict.First();
        foreach (var charWidth in charWidthsDict)
        {
            if (Math.Abs(charWidth.Key - mousePos.X) < Math.Abs(closestX.Key - mousePos.X)) closestX = charWidth;
        }
    #endregion
    if (noMenuIsOpen && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
        TabManager.Current().pointer = new Vector3(closestX.Value, closestY.Value, XYToI((int)closestX.Value, (int)closestY.Value));
    if (noMenuIsOpen && Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
        TabManager.Current().secondPointer = new Vector3(closestX.Value, closestY.Value, XYToI((int)closestX.Value, (int)closestY.Value));
    
    skipPointer:;
    // draw text pointer
    Raylib.DrawRectangleRec(
        new Rectangle(
            linePosition.X + Raylib.MeasureTextEx(Imports.font, TabManager.Current().splitStr[(int)TabManager.Current().pointer.Y].Substring(0,(int)TabManager.Current().pointer.X), OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing).X,
            textPos.Y + TabManager.Current().pointer.Y*OptionsMenu.config.fontSize,
            2,
            OptionsMenu.config.fontSize
        ),
        color
    );
    Raylib.DrawRectangleRec(
        new Rectangle(
            linePosition.X + Raylib.MeasureTextEx(Imports.font, TabManager.Current().splitStr[(int)TabManager.Current().secondPointer.Y].Substring(0,(int)TabManager.Current().secondPointer.X), OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing).X,
            textPos.Y + TabManager.Current().secondPointer.Y*OptionsMenu.config.fontSize,
            2,
            OptionsMenu.config.fontSize
        ),
        color
    );

    if (TabManager.Current().pointer != TabManager.Current().secondPointer)
    {
        Vector3 p = TabManager.Current().pointer;
        Vector3 sp = TabManager.Current().secondPointer;
        /*if (p.Z > sp.Z)
        {
            p = TabManager.Current().secondPointer;
            sp = TabManager.Current().pointer;
        }*/
        var pr = new Rectangle(
            linePosition.X + Raylib.MeasureTextEx(Imports.font, TabManager.Current().splitStr[(int)p.Y].Substring(0,(int)p.X), OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing).X,
            textPos.Y + p.Y*OptionsMenu.config.fontSize,
            2,
            OptionsMenu.config.fontSize
        );
        var spr =  new Rectangle(
            linePosition.X + Raylib.MeasureTextEx(Imports.font, TabManager.Current().splitStr[(int)sp.Y].Substring(0,(int)sp.X), OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing).X,
            textPos.Y + sp.Y*OptionsMenu.config.fontSize,
            2,
            OptionsMenu.config.fontSize
        );
        if (p.Z < sp.Z)
        {
            List<Rectangle> rects = new List<Rectangle>();
            Rectangle thisRec = new Rectangle(linePosition.X,textPos.Y + p.Y*OptionsMenu.config.fontSize,0,OptionsMenu.config.fontSize);
            string str = TabManager.Current().str;
            for (int i = (int)p.Z; i <= sp.Z; i++)
            {
                if (i == p.Z) thisRec.x += Raylib.MeasureTextEx(Imports.font, str.Split("\n")[(int)p.Y].Substring(0,(int)p.X), OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing).X;
                if (i == sp.Z)
                {
                    thisRec.width -= OptionsMenu.config.fontSize/2;
                    //thisRec.width += OptionsMenu.config.fontSpacing;
                }
                if (i == sp.Z || str[i] == '\n')
                {
                    thisRec.width += OptionsMenu.config.fontSize/2;
                    rects.Add(thisRec);
                    thisRec.x = linePosition.X - OptionsMenu.config.fontSpacing;
                    thisRec.y += OptionsMenu.config.fontSize;
                    thisRec.width = 0;
                }
                else
                {
                    thisRec.width += Raylib.MeasureTextEx(Imports.font, str[i].ToString(), OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing).X + OptionsMenu.config.fontSpacing;
                }
            }
            foreach (var l in rects)
            {
                Raylib.DrawRectangleRec(l, Functions.HexToColor(OptionsMenu.config.colors["text highlight"]));
            }
        }
        else if (p.Z > sp.Z)
        {
            sp = p;
            p  = TabManager.Current().secondPointer;
            List<Rectangle> rects = new List<Rectangle>();
            Rectangle thisRec = new Rectangle(linePosition.X,textPos.Y + p.Y*OptionsMenu.config.fontSize,0,OptionsMenu.config.fontSize);
            string str = TabManager.Current().str;
            for (int i = (int)p.Z; i <= sp.Z; i++)
            {
                if (i == p.Z) thisRec.x += Raylib.MeasureTextEx(Imports.font, str.Split("\n")[(int)p.Y].Substring(0,(int)p.X), OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing).X;
                if (i == sp.Z)
                {
                    thisRec.width -= OptionsMenu.config.fontSize/2;
                    //thisRec.width += OptionsMenu.config.fontSpacing;
                }
                if (i == sp.Z || str[i] == '\n')
                {
                    thisRec.width += OptionsMenu.config.fontSize/2;
                    rects.Add(thisRec);
                    thisRec.x = linePosition.X - OptionsMenu.config.fontSpacing;
                    thisRec.y += OptionsMenu.config.fontSize;
                    thisRec.width = 0;
                }
                else
                {
                    thisRec.width += Raylib.MeasureTextEx(Imports.font, str[i].ToString(), OptionsMenu.config.fontSize, OptionsMenu.config.fontSpacing).X + OptionsMenu.config.fontSpacing;
                }
            }
            foreach (var l in rects)
            {
                Raylib.DrawRectangleRec(l, Functions.HexToColor(OptionsMenu.config.colors["text highlight"]));
            }
        }
        //Raylib.DrawLineEx(new Vector2(pr.x, pr.y + pr.height/2), new Vector2(spr.x, spr.y + spr.height/2), 24, color);
    }
    
    #endregion

    
    bool mouseCursorBool = Functions.IsMouseOnRect(textArea);
    if (mouseCursorBool) Functions.SetMouseCursor(MouseCursor.MOUSE_CURSOR_IBEAM);
    else Functions.SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT);
    
    Raylib.EndScissorMode();
    #endregion
    // top bar
    
    /*Raylib.DrawRectangle(0,0,w,(int)firaCodeSize,Functions.HexToColor(Options.config.colors["UI border"]));
    #region side borders
        float blw = 4; // border line width;
        float cns = curvedNegative.width; // curve negative size;
        Raylib.DrawTextureQuad(curvedNegative, new Vector2(1,-1), Vector2.Zero, new Rectangle(w-cns-blw, h-24-cns, cns, cns), Functions.HexToColor(Options.config.colors["UI border"]));
        Raylib.DrawTextureQuad(curvedNegative, new Vector2(1, 1), Vector2.Zero, new Rectangle(w-cns-blw, 24, cns, cns), Functions.HexToColor(Options.config.colors["UI border"]));
        Raylib.DrawLineEx(new Vector2(w-blw/2,0),new Vector2(w-blw/2,h), blw,Functions.HexToColor(Options.config.colors["UI border"]));
        Raylib.DrawLineEx(new Vector2(blw/2,0),new Vector2(blw/2,h), blw,Functions.HexToColor(Options.config.colors["UI border"]));
        Raylib.DrawTextureQuad(curvedNegative, new Vector2(-1,-1), Vector2.Zero, new Rectangle(blw, h-24-cns, cns, cns), Functions.HexToColor(Options.config.colors["UI border"]));
        Raylib.DrawTextureQuad(curvedNegative, new Vector2(-1, 1), Vector2.Zero, new Rectangle(blw, 24, cns, cns), Functions.HexToColor(Options.config.colors["UI border"]));
    #endregion
    */
    #region file name and pointer info
        // drawing the pointers X, Y and Index info or error message if there is one
        string pointerText = TabManager.Current().pointer.X +","+TabManager.Current().pointer.Y+"  ";
        //pointerText = (errorMessage==""?pointerText:errorMessage)+"  ";
        float pointerTextWidth = Raylib.MeasureTextEx(Imports.firaCode, pointerText, 24, 0).X;
        Raylib.DrawTextEx(Imports.firaCode, pointerText, new Vector2(w-pointerTextWidth,h-24),24, 0, Functions.HexToColor(OptionsMenu.config.colors["UI elements"]));

        // draw the file name at the bottom left of the screen
        Raylib.DrawTextEx(Imports.firaCode, windowTitle, new Vector2(4,h-24),24, 0, Functions.HexToColor(OptionsMenu.config.colors["UI elements"]));

        // drawing the logo in the middle of the screen
        Raylib.DrawTexture(Imports.icon, (w-Imports.icon.width)/2,0, Functions.HexToColor(OptionsMenu.config.colors["UI elements"]));
    #endregion
    
    #region close, min & max buttons
    string buttonsColor = OptionsMenu.config.colors["UI elements"];
    Rectangle close = new Rectangle(w-24,0, 24,24);
    Rectangle max = new Rectangle(w-48,0, 24,24);
    Rectangle min = new Rectangle(w-72,0, 24,24);
    if (Functions.IsMouseOnRect(close)) // close button
    {
        Functions.DrawCurvedBoxEx(Imports.curvedBox, Functions.AddRect(close, new Rectangle(2,2,-4,-4)), new int[]{1,1,1,1}, Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]));
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            closeWindow = OptionsMenu.AreAllFilesSaved();
        }
        //buttonsColor = Options.config.colors["text"];
    }
    else if (Functions.IsMouseOnRect(max)) // max button
    {
        Functions.DrawCurvedBoxEx(Imports.curvedBox, Functions.AddRect(max, new Rectangle(2,2,-4,-4)), new int[]{1,1,1,1}, Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]));
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            if (Raylib.IsWindowMaximized()) {
                Raylib.RestoreWindow();
                Raylib.SetWindowPosition((int)windowPosition.X, (int)windowPosition.Y);
                isDraggable = false;
            }
            else {
                windowPosition = Raylib.GetWindowPosition();
                Raylib.MaximizeWindow();
                Raylib.SetWindowPosition(0,0);
            }
        }
        //buttonsColor = Options.config.colors["text"];
    }
    else if (Functions.IsMouseOnRect(min)) // min button
    {
        Functions.DrawCurvedBoxEx(Imports.curvedBox, Functions.AddRect(min, new Rectangle(2,2,-4,-4)), new int[]{1,1,1,1}, Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]));
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) Raylib.MinimizeWindow();
        //buttonsColor = Options.config.colors["text"];
    }

    //Raylib.DrawTexture(Imports.buttons, w-Imports.buttons.width, 0, Functions.HexToColor(buttonsColor));
    Raylib.DrawTexturePro(Imports.icons, new Rectangle(ButtonPlaces.minimize,0,72,24), new Rectangle(w-72,0,72,24), new Vector2(0,0), 0, Functions.HexToColor(buttonsColor));
    #endregion

    // options button
    Rectangle optionsRec = new Rectangle(0,0, 24,24);
    if (Functions.IsMouseOnRect(optionsRec))
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            SearchMenu.showMenu = false;
            OptionsMenu.showMenu = !OptionsMenu.showMenu;
        }
    }
    OptionsMenu.Options();
    if (OptionsMenu.showMenu) Functions.DrawCurvedBoxEx(Imports.curvedBox, Functions.AddRect(optionsRec, new Rectangle(2,2,-4,-4)), new int[]{1,1,1,1}, Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]));
    
    Rectangle searchRec = new Rectangle(24,0, 24,24);
    if (Functions.IsMouseOnRect(searchRec))
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
        {
            SearchMenu.showMenu = !SearchMenu.showMenu;
            OptionsMenu.showMenu = false;
        }
    }
    SearchMenu.Search();
    if (SearchMenu.showMenu) Functions.DrawCurvedBoxEx(Imports.curvedBox, Functions.AddRect(searchRec, new Rectangle(2,2,-4,-4)), new int[]{1,1,1,1}, Functions.HexToColor(OptionsMenu.config.colors["UI highlight"]));
    
    
    //Raylib.DrawTexture(Imports.options, 0, 0, Functions.HexToColor(OptionsMenu.config.colors["UI elements"]));
    Raylib.DrawTexturePro(Imports.icons, new Rectangle(0,0,48,24), new Rectangle(0,0,48,24), new Vector2(0,0), 0, Functions.HexToColor(buttonsColor));
    /*7string[] cols = {
        "143cdc",
        "3c14dc",
        "3cdc14",

        "14dc3c",
        "dc143c",
        "dc3c14",
    };
    for (int i = 0; i < cols.Length; i++)
    {
        int s = 50;
        int y = i%2 == 0?s:-16;
        Raylib.DrawRectangle(100 + i*s, 100, s, s, Functions.HexToColor(cols[i]));
        Raylib.DrawText(cols[i], 100 + i*s, 100+y, 15, Functions.HexToColor(cols[i]));
    }*/

    //Functions.DrawCurvedBoxEx(Imports.curvedBox, new Rectangle(100, 100, 150, 100), new int[]{1,2,2,1}, Color.WHITE);
    //Raylib.DrawFPS(w-100, h-100);
    Raylib.EndDrawing();
    Functions.UpdateMouseCursor();
    TabManager.Current().prevStr = TabManager.Current().str;
}
//if (TabManager.Current().filePath != "") TextBoxInfo.data.lastOpenedFile = TabManager.Current().filePath;
TextBoxInfo.data.lastOpenedFiles.Clear();
foreach (var tab in TabManager.tabs)
{
    if (tab.filePath != "")TextBoxInfo.data.lastOpenedFiles.Add(tab.filePath);
}
File.WriteAllText("assets/data.json", System.Text.Json.JsonSerializer.Serialize(TextBoxInfo.data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

// close all tabs
for (int i=0; i < TabManager.tabs.Count; i++) TabManager.CloseTab(i);
Raylib.CloseWindow();
Imports.UnloadAll();

static int XYToI(int x, int y)
{
    int retI = 0;
    for (int i = 0; i < y; i++)
    {
        retI += TabManager.Current().splitStr[i].Length + 1;
    }
    retI += x;
    return retI;
}

static List<int> splitMatches(MatchCollection m)
{
    List<int> ret = new List<int>();
    foreach (Match match in m)
    {
        if (match.Value.Length > 1)
        {
            for (int o = 0; o < match.Value.Length; o++) ret.Add(match.Index + o);
        }
        else if (match.Value.Length == 1) ret.Add(match.Index);
    }
    return ret;
}