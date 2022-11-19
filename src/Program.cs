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

Console.WriteLine($"Hello, {System.Environment.UserName}. This is Monkey programming language.");
Console.WriteLine($"Feel free to type in commands:");

while (true)
{
    Console.Write(">>");
    string? line = Console.ReadLine();
    if (line == null) continue;

    var l = new Lexer(line);
    for (var tok = l.NextToken(); tok.Type != TokenType.EOF; tok = l.NextToken())
    {
        tok.Print();
    }
}