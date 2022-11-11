public class TextBoxInfo
{
    
    public static Data data = JsonSerializer.Deserialize<Data>(File.ReadAllText("assets/data.json"))!;
    public string filePath;
    public string windowTitle;
    public string str = "";
    public string savedStr;
    //Options.OpenFile(data.lastOpenedFile);
    public string prevStr;
    
    public Vector3 pointer = Vector3.Zero;
    public Vector3 secondPointer = Vector3.Zero;
    public string[] splitStr;
    public List<StrState> strHistory = new List<StrState>();

    public Vector2 scroll = Vector2.Zero;
    static int[] allowedChars = {
        162,
        331,
        332,
        333,
        334,
        118,
        258,
    };

    public TextBoxInfo(string filePath, string str)
    {
        this.str = str;
        savedStr = str;
        prevStr = str;
        splitStr = str.Split("\n");
        this.filePath = filePath;
        windowTitle = Path.GetFileName(filePath);
        if (strHistory != null)strHistory.Add(new StrState(){str=str, pointer=pointer});
    }

    public void AlterTextMultiline()
    {
        int key = Functions.GetKeyPressed();
        if (key == 0) return;

        bool isShiftOn = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT);
        bool isCtrltOn = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL);

        if (key == 259) // backspace
        {
            if (pointer.Z <= 0) goto no;
            if (secondPointer != pointer) goto different;
            str = str.Remove((int)pointer.Z-1, 1);

            if (pointer.X == 0)
            {
                pointer.Y--;
                pointer.X = splitStr[(int)pointer.Y].Length;
            }
            else pointer.X--;
            pointer.Z--;
            secondPointer = pointer;
            if (strHistory != null)strHistory.Add(new StrState(){str=str, pointer=pointer});
            return;

            different:
            if (pointer.Z < secondPointer.Z)
            {
                str = str.Remove((int)pointer.Z, (int)(secondPointer.Z-(pointer.Z)));
                secondPointer = pointer;
            }
            else
            {
                str = str.Remove((int)secondPointer.Z, (int)(pointer.Z-(secondPointer.Z)));
                pointer = secondPointer;
            }
            if (strHistory != null)strHistory.Add(new StrState(){str=str, pointer=pointer});
            return;
        }
        else if (key == 262) // right
        {
            if (secondPointer != pointer)
            {
                if (pointer.X > secondPointer.X) secondPointer = pointer;
                else pointer = secondPointer;
                return;
            }
            if (pointer.Z == str.Length) goto no;

            if (pointer.X == splitStr[(int)pointer.Y].Length)
            {
                pointer.Y++;
                pointer.X = 0;
            }
            else pointer.X++;
            pointer.Z++;
            secondPointer = pointer;
        }
        else if (key == 263) // left
        {
            if (secondPointer != pointer)
            {
                if (pointer.X < secondPointer.X) secondPointer = pointer;
                else pointer = secondPointer;
                return;
            }
            if (pointer.Z == 0) goto no;

            if (pointer.X == 0)
            {
                pointer.Y--;
                pointer.X = splitStr[(int)pointer.Y].Length;
            }
            else pointer.X--;
            pointer.Z--;
            secondPointer = pointer;
        }
        else if (key == 264) // down
        {
            if (secondPointer != pointer)
            {
                if (pointer.Y > secondPointer.Y) secondPointer = pointer;
                else pointer = secondPointer;
                return;
            }
            if (pointer.Y == splitStr.Length-1) goto no;

            pointer.Y++;
            if (pointer.X > splitStr[(int)pointer.Y].Length) pointer.X = splitStr[(int)pointer.Y].Length;
            int ci = 0; // current I
            for (int i = 0; i < pointer.Y; i++) ci += splitStr[i].Length + 1;
            pointer.Z = ci + splitStr[(int)pointer.Y].Remove((int)pointer.X).Length;
            secondPointer = pointer;
        }
        else if (key == 265) // up
        {
            if (secondPointer != pointer)
            {
                if (pointer.Y < secondPointer.Y) secondPointer = pointer;
                else pointer = secondPointer;
                return;
            }
            if (pointer.Y == 0) goto no;

            pointer.Y--;
            if (pointer.X > splitStr[(int)pointer.Y].Length) pointer.X = splitStr[(int)pointer.Y].Length;
            int ci = 0; // current I
            for (int i = 0; i < pointer.Y; i++) ci += splitStr[i].Length + 1;
            pointer.Z = ci + splitStr[(int)pointer.Y].Remove((int)pointer.X).Length;
            secondPointer = pointer;
        }
        else if (Functions.getCharSet.Contains((char)key) || allowedChars.Contains(key))
        {
            string chr = " ";
            bool doubled = false;
            if (key == 10)
            {
                chr = "\n";
                pointer.Y++;
                pointer.X = 0;
            }
            else
            {
                chr = ""+((char)key);
                if (key == 162)
                {
                    if (isShiftOn) chr = "|";
                    else chr = "\\";
                }
                if (key == 331) chr = "/";
                if (key == 332) chr = "*";
                if (key == 333) chr = "-";
                if (key == 334) chr = "+";
                if (key == 334) chr = "+";
                if (key == 258) chr = new string(' ', OptionsMenu.config.tabLength);
                char next = str.Length>0?str[Math.Min((int)pointer.Z+1, str.Length-1)]:' ';
                if (next != ' ' && next != '\n') goto forgetDoubling;
                if (chr == "\"")
                {
                    chr = "\"\"";
                    doubled = true;
                }
                if (chr == "{")
                {
                    chr = "{}";
                    doubled = true;
                }
                if (chr == "(")
                {
                    chr = "()";
                    doubled = true;
                }
                if (chr == "[")
                {
                    chr = "[]";
                    doubled = true;
                }
                forgetDoubling:
                //if (key == 118 && isCtrltOn)
                //    chr = Clipboard.GetText();
                    
                pointer.X+=chr.Length;
            }
            str = str.Insert((int)pointer.Z,chr);
            //pointer.Y += chr.Where(c => c == '\n').Count();
            pointer.Z+=chr.Length;
            if (doubled)
            {
                pointer.X--;
                pointer.Z--;
            }
            if (strHistory != null)strHistory.Add(new StrState(){str=str, pointer=pointer});
        }
        no:
        splitStr = str.Split("\n");
        secondPointer = pointer;
    }
    public static void AlterTextMonoline(ref int pointer, ref string str)
    {
        int key = Functions.GetKeyPressed();
        if (key == 0) return;

        bool isShiftOn = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT);
        bool isCtrltOn = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_CONTROL);

        if (key == 259) // backspace
        {
            if (pointer <= 0) goto no;
            str = str.Remove(pointer-1, 1);
            pointer--;
        }
        else if (key == 262) // right
        {
            if (pointer == str.Length) goto no;
            pointer++;
        }
        else if (key == 263) // left
        {
            if (pointer == 0) goto no;
            
            pointer--;
        }
        else if (Functions.getCharSet.Contains((char)key) || allowedChars.Contains(key))
        {
            string chr = " ";
            if (key != 10)
            {
                chr = ""+((char)key);
                if (key == 162)
                {
                    if (isShiftOn) chr = "|";
                    else chr = "\\";
                }
                if (key == 331) chr = "/";
                if (key == 332) chr = "*";
                if (key == 333) chr = "-";
                if (key == 334) chr = "+";

                str = str.Insert(pointer,chr);
                pointer++;
            }
        }
        no:
        return;
    }
}