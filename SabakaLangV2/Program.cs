using SabakaLangV2.Lexer;

Lexer lexer = new Lexer("int picun = 5; print(picun);");

foreach (var token in lexer.Tokenize())
{
    Console.WriteLine(token.Type);
}