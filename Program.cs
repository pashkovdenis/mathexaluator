using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        { 
            Console.WriteLine("Math Expression evaluator");
 
            while (true)
            { 
                var input = Console.ReadLine();

                var reader = new StringReader(input); 
                var tokenizer = new Tokenizer(reader); 
                var operands = new Stack<Operand>();
                  
                while (tokenizer.Token != Token.EOF)
                { 
                    if (operands.Count == 0)
                    {
                        operands.Push(new Operand(tokenizer.Number, tokenizer.Token)); 
                    
                    } else
                    {
                        if (tokenizer.Token == Token.OpenParens || tokenizer.Token == Token.CloseParens)
                        { 
                            operands.Push(new Operand()); 
                            tokenizer.NextToken(); 
                            continue; 
                        }  
                        if (tokenizer.Token != Token.Number)
                        {
                            var o = operands.Pop();
                            operands.Push(new Operand(o.Value, tokenizer.Token));
                        } 
                        if (tokenizer.Token == Token.Number)
                        {
                            var o = operands.Pop();
                            operands.Push(new Operand( Calculate(o.Token, o.Value, tokenizer.Number), Token.Number));  
                        } 
                    } 
                    tokenizer.NextToken(); 
                }

                  
                // Calculate operands 
                if (operands.Count > 0)
                {
                    var oper = operands.Pop();
                    double total = oper.Value;
                    Token prev = oper.Token; 
                    foreach (var o in operands)
                    {
                        total = Calculate(prev, total, o.Value);
                        prev = o.Token;
                    } 
                    Console.WriteLine(total);
                }
        
            } 
        
        }
         
        static double Calculate(Token t, double left, double right)
        {
            return t switch
            {
                Token.Multiply => left * right,
                Token.Subtract => left - right,
                Token.Divide => left/right,
                 _ => left + right, 
            };
        } 
        
    }
     
    public readonly struct Operand
    { 
        public double Value { get; }  
        public Token Token { get; }  
        public Operand(double val, Token token)
        {
            Value = val;
            Token = token;  
        } 
    }
     
    public class Tokenizer
    {
        public Tokenizer(TextReader reader)
        {
            _reader = reader;
            NextChar();
            NextToken();
        }

        TextReader _reader;
        char _currentChar;
        Token _currentToken;
        double _number;
        string _identifier;

        public Token Token
        {
            get { return _currentToken; }
        }

        public double Number
        {
            get { return _number; }
        }

        public string Identifier
        {
            get { return _identifier; }
        }

        // Read the next character from the input strem
        // and store it in _currentChar, or load '\0' if EOF
        void NextChar()
        {
            int ch = _reader.Read();
            _currentChar = ch < 0 ? '\0' : (char)ch;
        }

        // Read the next token from the input stream
        public void NextToken()
        {
            // Skip whitespace
            while (char.IsWhiteSpace(_currentChar))
            {
                NextChar();
            }

            // Special characters
            switch (_currentChar)
            {
                case '\0':
                    _currentToken = Token.EOF;
                    return;

                case '+':
                    NextChar();
                    _currentToken = Token.Add;
                    return;

                case '-':
                    NextChar();
                    _currentToken = Token.Subtract;
                    return;

                case '*':
                    NextChar();
                    _currentToken = Token.Multiply;
                    return;

                case '/':
                    NextChar();
                    _currentToken = Token.Divide;
                    return;

                case '(':
                    NextChar();
                    _currentToken = Token.OpenParens;
                    return;

                case ')':
                    NextChar();
                    _currentToken = Token.CloseParens;
                    return;

                case ',':
                    NextChar();
                    _currentToken = Token.Comma;
                    return;
            }

            // Number?
            if (char.IsDigit(_currentChar) || _currentChar == '.')
            {
                // Capture digits/decimal point
                var sb = new StringBuilder();
                bool haveDecimalPoint = false;
                while (char.IsDigit(_currentChar) || (!haveDecimalPoint && _currentChar == '.'))
                {
                    sb.Append(_currentChar);
                    haveDecimalPoint = _currentChar == '.';
                    NextChar();
                }

                // Parse it
                _number = double.Parse(sb.ToString(), CultureInfo.InvariantCulture);
                _currentToken = Token.Number;
                return;
            }

            // Identifier - starts with letter or underscore
            if (char.IsLetter(_currentChar) || _currentChar == '_')
            {
                var sb = new StringBuilder();

                // Accept letter, digit or underscore
                while (char.IsLetterOrDigit(_currentChar) || _currentChar == '_')
                {
                    sb.Append(_currentChar);
                    NextChar();
                }

                // Setup token
                _identifier = sb.ToString();
                _currentToken = Token.Identifier;
                return;
            }

        }
    }

    public enum Token
    {
        EOF,
        Add,
        Subtract,
        Multiply,
        Divide,
        OpenParens,
        CloseParens,
        Comma,
        Identifier,
        Number,
    }


}
