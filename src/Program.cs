TestLexer();
TestParser();
TestUnlessMacro();

REPL();

void TestLexer()
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"TestLexer:");
    Console.ResetColor();

    string x = @"
    let five = 5; 
    let ten = 10;
    let add = fn(x, y) {
        x + y;
    };
    let result = add(five, ten);

    if (5 < 10) {
        return true;
    } else {
        return false;
    }

    10 == 10;
    10 != 9;
    ";

    var m = new Lexer(x);
    List<Token> tokens = new List<Token>();
    while (m.ReadPosition <= x.Length)
    {
        var tok = m.NextToken();
        tok.Print();
        tokens.Add(tok);
    }
}

void TestParser()
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"TestParser:");
    Console.ResetColor();

    string x = @"
    let five = 5; 
    return true;

    5 + 5 * 10;

    !foo;
    5 + -10;

    5 + 5;
    5 - 5;
    5 * 5;
    5 / 5;
    5 > 5;
    5 < 5;
    5 == 5;
    5 != 5;

    bool b; 

    (5 + 5) * 10;

    if (b) { 1+2; } else { 3+4;}

    let add = fn(x, y) {
      x + y;
    };

    ";

    var l = new Lexer(x);
    var p = new Parser(l);
    var program = p.ParseProgram();
    program.PrintStaments();
}

void TestUnlessMacro()
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"TestUnlessMacro:");
    Console.ResetColor();

    Evaluator eval = new();
    MonkeyEnvironment env = new();
    MonkeyEnvironment macroEnv = new();

    string unlessMacro = "let unless = macro(condition, consequence, alternative){ quote( if(!(unquote(condition))) {unquote(consequence);} else {unquote(alternative);} ); };";
    ParseLine(unlessMacro, eval, env, macroEnv);
    string unlessUseage = "unless(10>5, puts(\"note greater\"), puts(\"greater\"));";
    ParseLine(unlessUseage, eval, env, macroEnv);
}

void REPL()
{
    //    string MONKEY_FACE = """            __,__
    //   .--.  .- "     " -.  .--.
    //  / .. \/  .-. .-.  \/ .. \
    // | | '|  /   Y   \  |' | |
    // | \   \  \ 0 | 0 /  /   / |
    //  \ '- ,\.-"""""""-./, -' /
    //   '' - ' /_   ^ ^   _\ ' - ''
    //       |  \._ _./  |
    //       \   \ '~' /   /
    //        '._ ' -= -' _.'
    //           '-----'
    //""";

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Hello, {System.Environment.UserName}. This is Monkey programming language.");
    Console.WriteLine($"Feel free to type in commands:");
    //Console.WriteLine(MONKEY_FACE);
    Console.ResetColor();

    Evaluator eval = new();
    MonkeyEnvironment env = new();
    MonkeyEnvironment macroEnv = new();

    while (true)
    {
        Console.Write(">>");
        string? line = Console.ReadLine();
        if (line == null) continue;

        ParseLine(line, eval, env, macroEnv);
    }
}

void ParseLine(string line, Evaluator eval, MonkeyEnvironment env, MonkeyEnvironment macroEnv)
{
    var l = new Lexer(" " + line);
    var p = new Parser(l);
    var program = p.ParseProgram();
    program.PrintStaments();

    eval.DefineMacros(program, macroEnv);
    var expanded = eval.ExpandMacros(program, macroEnv);
    IMonkeyObject? obj = eval.Eval(program, env);
    if (obj != null)
    {
        Console.WriteLine($"evaluator result : {obj.Inspect()}");
    }
}
