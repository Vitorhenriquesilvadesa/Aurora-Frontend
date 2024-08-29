using AuroraCompiler.Core;
using AuroraCompiler.Types;

namespace AuroraCompiler.Lexer
{
    using Spec;

    public class Scanner
    {
        private readonly ScanSpecification _specification;
        private int _start;
        private int _current;
        private int _line;
        private int _column;
        private string _source;
        private List<Token> _tokens;
        public Stack<string> ErrorStack { get; private set; }
        public bool HasError { get; private set; }

        public Scanner(ScanSpecification specification)
        {
            _specification = specification;
        }

        public Scanner()
        {
            _specification = new ScanSpecification(logLexingPass: Spec.Disabled);
        }

        public ScannedData Scan(string source)
        {
            _source = source;
            ResetInternalState();
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            return new ScannedData(_tokens);
        }

        private void ScanToken()
        {
            char c = Peek();
            switch (c)
            {
                case '(':
                    MakeToken(TokenType.LeftParen, "(", NullLiteral.Instance);
                    break;

                case ')':
                    MakeToken(TokenType.RightParen, ")", NullLiteral.Instance);
                    break;

                case '[':
                    MakeToken(TokenType.LeftBracket, "[", NullLiteral.Instance);
                    break;

                case ']':
                    MakeToken(TokenType.RightBracket, "]", NullLiteral.Instance);
                    break;

                case '{':
                    MakeToken(TokenType.LeftBrace, "{", NullLiteral.Instance);
                    break;

                case '}':
                    MakeToken(TokenType.RightBrace, "}", NullLiteral.Instance);
                    break;

                case '.':
                    MakeToken(TokenType.Dot, ".", NullLiteral.Instance);
                    break;

                case ',':
                    MakeToken(TokenType.Comma, ",", NullLiteral.Instance);
                    break;

                case ';':
                    MakeToken(TokenType.Semicolon, ";", NullLiteral.Instance);
                    break;

                case ':':
                    MakeToken(TokenType.Colon, ":", NullLiteral.Instance);
                    break;

                case '@':
                    MakeToken(TokenType.AtSign, "@", NullLiteral.Instance);
                    break;

                case '$':
                    MakeToken(TokenType.Dollar, "$", NullLiteral.Instance);
                    break;

                case '#':
                    MakeToken(TokenType.Hash, "#", NullLiteral.Instance);
                    break;

                case '|':
                    MakeToken(TokenType.Pipe, "|", NullLiteral.Instance);
                    break;

                case '%':
                    MakeToken(TokenType.Mod, "%", NullLiteral.Instance);
                    break;

                case '+':
                    MakeToken(TokenType.Plus, "+", NullLiteral.Instance);
                    break;

                case '-':
                    MakeToken(TokenType.Minus, "-", NullLiteral.Instance);
                    break;

                case '*':
                    MakeToken(TokenType.Star, "*", NullLiteral.Instance);
                    break;

                case '/':
                    if (Match('/'))
                    {
                        SingleLineComment();
                    }
                    else
                    {
                        if (Match('*'))
                        {
                            
                        }
                        else if (Match('='))
                        {
                            MakeToken(TokenType.SlashEqual, "/=", NullLiteral.Instance);
                        }
                    }

                    break;

                case '!':
                    if (Match('='))
                    {
                        MakeToken(TokenType.EqualEqual, "!=", NullLiteral.Instance);
                    }
                    else
                    {
                        MakeToken(TokenType.Mark, "!", NullLiteral.Instance);
                    }
                    break;

                case '=':
                    if (Match('='))
                        MakeToken(TokenType.EqualEqual, "==", NullLiteral.Instance);
                    else
                        MakeToken(TokenType.Equal, "=", NullLiteral.Instance);
                    break;

                case '<':
                    if (Match('<'))
                        if (Match('='))
                        {
                            MakeToken(TokenType.LeftShiftEqual, "<<=", NullLiteral.Instance);
                        }
                        else
                        {
                            MakeToken(TokenType.LeftShit, "<<", NullLiteral.Instance);
                        }
                    else
                    {
                        if (Match('='))
                        {
                            MakeToken(TokenType.LessEqual, "<=", NullLiteral.Instance);
                        }
                        else
                        {
                            MakeToken(TokenType.Less, "<", NullLiteral.Instance);
                        }
                    }

                    break;

                case '>':
                    if (Match('>'))
                        if (Match('='))
                        {
                            MakeToken(TokenType.RightShiftEqual, ">>=", NullLiteral.Instance);
                        }
                        else
                        {
                            MakeToken(TokenType.RightShift, ">>", NullLiteral.Instance);
                        }
                    else
                    {
                        if (Match('='))
                        {
                            MakeToken(TokenType.GreaterEqual, ">=", NullLiteral.Instance);
                        }
                        else
                        {
                            MakeToken(TokenType.Greater, ">", NullLiteral.Instance);
                        }
                    }

                    break;

                case '?':
                    if (Match('?'))
                    {
                        if (Match('='))
                        {
                            MakeToken(TokenType.QuestionQuestionEqual, "??=", NullLiteral.Instance);
                        }
                        else
                        {
                            MakeToken(TokenType.QuestionQuestion, "??", NullLiteral.Instance);
                        }
                    }
                    else
                    {
                        MakeToken(TokenType.Question, "?", NullLiteral.Instance);
                    }

                    break;

                case '"':
                    // Handle string literals here
                    break;

                default:
                    // Handle unknown tokens
                    break;
            }
        }

        private void SingleLineComment()
        {
            while (!Check('\n'))
            {
                Advance();
            }
            
            SyncCursors();
        }

        private void ResetInternalState()
        {
            _start = 0;
            _current = 0;
            _line = 1;
            _column = 1;
            _tokens = new List<Token>();
            ErrorStack = new Stack<string>();
            HasError = false;
        }

        private void MakeToken(TokenType type, string lexeme, object literal)
        {
            Token token = new Token(_line, _column, type, lexeme, literal);
            _tokens.Add(token);
            int delta = _current - _start;
            SyncCursors();
            _column += delta;
        }

        private void SyncCursors()
        {
            _start = _current;
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        private bool IsAtEnd()
        {
            return Peek() == '\0' || _current >= _source.Length;
        }

        private char Peek()
        {
            return _source[_current];
        }

        private void Advance()
        {
            _current++;
        }

        private bool Check(char c)
        {
            return Peek() == c;
        }

        private bool Match(char c)
        {
            if (_current == _source.Length) return false;
            if (c != _source[_current + 1]) return false;

            Advance();
            return true;
        }
    }
}