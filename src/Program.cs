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
Console.ReadKey();