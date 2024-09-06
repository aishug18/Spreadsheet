// <copyright file="Formula_PS2.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
//  <para>
//      This code is provided to start your assignment. It was written
//      by Profs Joe, Danny, and Jim. You should keep this attribution
//      at the top of your code where you have your header comment, along
//      with the other required information.
//  </para>
//  <para>
//      You should remove/add/adjust comments in your file as appropriate
//      to represent your work and any changes you make.
//  </para>
// </summary>
namespace CS3500.Formula;

using System.Text.RegularExpressions;

/// <summary>
///     <para>
///         This class represents formulas written in standard infix notation using standard precedence
///         rules. The allowed symbols are non-negative numbers written using double-precision
///         floating-point syntax; variables that consist of one or more letters followed by
///         one or more numbers; parentheses; and the four operator symbols +, -, *,and /.
///     </para>
///     <para>
///         Spaces are significant only insofar that they delimit tokens. For example, "xy" is
///         a single variable, "x y" consists of two variables "x" and "y"; "x23" is a single variable;
///         and "x 23" consists of a variable "x" and a number "23". Otherwise, spaces are to be removed.
///     </para>
///     <para>
///         For Assignment Two, you are to implement the following functionality:
///     </para>
///     <list type="bullet">
///         <item>
///             Formula Constructor which checks the syntax of a formula.
///         </item>
///         <item>
///             Get Variables
///         </item>
///         <item>
///             ToString
///         </item>
///     </list>
/// </summary>
public class Formula
{

    /// <summary>
    ///     All variables are letters followed by numbers. This pattern
    ///     represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    ///     This variable stores the string representation of the canonical formula (uppercase
    ///     variables, no spaces, etc).
    /// </summary>
    private string canonicalFormula;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Formula"/> class.
    ///     <para>
    ///         Creates a Formula from a string that consists of an infix expression written as
    ///         described in the class comment. If the expression is syntactically incorrect,
    ///         throws a FormulaFormatException with an explanatory Message. See the assignment
    ///         specifications for the syntax rules you are to implement.
    ///     </para>
    ///     <para>
    ///         Non Exhaustive Example Errors:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             Invalid variable name, e.g., x, x1x (Note: x1 is valid, but would be normalized to X1)
    ///         </item>
    ///         <item>
    ///             Empty formula, e.g., string.Empty
    ///         </item>
    ///         <item>
    ///             Mismatched Parentheses, e.g., "(("
    ///         </item>
    ///         <item>
    ///             Invalid Following Rule, e.g., "2x+5"
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        List<string> tokens = GetTokens(formula);

        if(tokens.Count == 0)
            throw new FormulaFormatException("there must be at least one token");

        string prevToken = "";
        canonicalFormula = "";
        int countCloseParen = 0;
        int countOpenParen = 0;
        string operators = @"[+\-*/]";

        string firstToken = tokens.First();
        if(!double.TryParse(firstToken, out double resultFirst) && !IsVar(firstToken) && !firstToken.Equals("("))
            throw new FormulaFormatException("the first token is not a number, variable, or opening parenthesis");

        string lastToken = tokens.Last();
        if (!double.TryParse(lastToken, out double resultLast) && !IsVar(lastToken) && !lastToken.Equals(")"))
            throw new FormulaFormatException("the last token is not a number, variable, or closing parenthesis");

        foreach (string t in tokens)
        {
            // if token is a number
            if (double.TryParse(t, out double number) && NumVarCloseParenFollowedByNumVarOpenParen(prevToken))
                canonicalFormula += number.ToString();

            // if token is a variable
            else if (IsVar(t) && NumVarCloseParenFollowedByNumVarOpenParen(prevToken))
                canonicalFormula += t.ToUpper();

            // if token is an operator
            else if (Regex.IsMatch(t, operators) && OperOpenParenFollowedByOperCloseParen(prevToken))
                canonicalFormula += t;

            // if token is a closing parenthesis
            else if (t.Equals(")") && OperOpenParenFollowedByOperCloseParen(prevToken))
            {
                canonicalFormula += t;
                countCloseParen++;
            }

            // if token is an opening parenthesis
            else if (t.Equals("(") && NumVarCloseParenFollowedByNumVarOpenParen(prevToken))
            {
                canonicalFormula += t;
                countOpenParen++;
            }

            // if token is anything else
            else
                throw new FormulaFormatException("a token in the formula is not a valid token");

            if (countCloseParen > countOpenParen)
                throw new FormulaFormatException("the number of closing parentheses seen so far is greater then the number of opening parentheses");

            prevToken = t;
        }

        if (countCloseParen != countOpenParen)
            throw new FormulaFormatException("the total number of opening parentheses is not equal to the total number of closing parentheses");
    }

    /// <summary>
    ///     Called if the current token is a number, variable, or open parenthesis.
    ///     If the previous token is a number, variable, or close parenthesis, then the formula breaks the following rules and 
    ///     is invalid, so an FormulaFormatException is thrown.
    ///     Otherwise, the token is valid and returns true.
    /// </summary>
    /// <param name="prevToken"> the previous token. </param>
    /// <returns> true if the following rules are passed, else throws a FormulaFormatException. </returns>
    private static bool NumVarCloseParenFollowedByNumVarOpenParen(string prevToken)
    {
        if (double.TryParse(prevToken, out double result) || IsVar(prevToken) || prevToken.Equals(")"))
            throw new FormulaFormatException("a number/variable/closing parenthesis is followed by a number/variable/opening parenthesis");
        
        else
            return true;

    }

    /// <summary>
    ///     Called if the current token is an operator or close parenthesis.
    ///     If the previous token is an operator or open parenthesis, then the formula breaks the following rules and 
    ///     is invalid, so an FormulaFormatException is thrown.
    ///     Otherwise, the token is valid and returns true.
    /// </summary>
    /// <param name="prevToken"> the previous token. </param>
    /// <returns> true if the following rules are passed, else throws a FormulaFormatException. </returns>
    private static bool OperOpenParenFollowedByOperCloseParen(string prevToken)
    {
        string operators = @"[+\-*/]";  // regex pattern for operators +, -, *, /

        if (Regex.IsMatch(prevToken, operators) || prevToken.Equals("("))
            throw new FormulaFormatException("an operator/opening parenthesis is followed by an operator/closing parenthesis");
        
        else
            return true;
    }

   
    /// <summary>
    ///     <para>
    ///         Returns a set of all the variables in the formula.
    ///     </para>
    ///     <remarks>
    ///         Important: no variable may appear more than once in the returned set, even
    ///         if it is used more than once in the Formula.
    ///     </remarks>
    ///     <para>
    ///         For example, if N is a method that converts all the letters in a string to upper case:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>new("x1+y1*z1").GetVariables() should enumerate "X1", "Y1", and "Z1".</item>
    ///         <item>new("x1+X1" ).GetVariables() should enumerate "X1".</item>
    ///     </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> variables = new HashSet<string>();

        List<string> tokens = GetTokens(canonicalFormula);

        foreach(string t in tokens)
        {
            if (IsVar(t))
                variables.Add(t);
        }
        
        return variables;
    }

    /// <summary>
    ///     <para>
    ///         Returns a string representation of a canonical form of the formula.
    ///     </para>
    ///     <para>
    ///         The string will contain no spaces.
    ///     </para>
    ///     <para>
    ///         If the string is passed to the Formula constructor, the new Formula f
    ///         will be such that this.ToString() == f.ToString().
    ///     </para>
    ///     <para>
    ///         All of the variables in the string will be normalized. This
    ///         means capital letters.
    ///     </para>
    ///     <para>
    ///         For example:
    ///     </para>
    ///     <code>
    ///         new("x1 + y1").ToString() should return "X1+Y1"
    ///         new("X1 + 5.0000").ToString() should return "X1+5".
    ///     </code>
    ///     <para>
    ///         This code should execute in O(1) time.
    ///     <para>
    /// </summary>
    /// <returns>
    ///     A canonical version (string) of the formula. All "equal" formulas
    ///     should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return canonicalFormula;
    }

    /// <summary>
    ///     Reports whether "token" is a variable. It must be one or more letters
    ///     followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable.
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///     <para>
    ///         Given an expression, enumerates the tokens that compose it.
    ///     </para>
    ///     <para>
    ///         Tokens returned are:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>left paren</item>
    ///         <item>right paren</item>
    ///         <item>one of the four operator symbols</item>
    ///         <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///         <item>a double literal</item>
    ///         <item>and anything that doesn't match one of the above patterns</item>
    ///     </list>
    ///     <para>
    ///         There are no empty tokens; white space is ignored (except to separate other tokens).
    ///     </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];
        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";
        
        // Overall pattern
        string pattern = string.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                lpPattern,
                                rpPattern,
                                opPattern,
                                VariableRegExPattern,
                                doublePattern,
                                spacePattern);
        
        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }
        return results;
    }
}

/// <summary>
///     Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///     <para>
    ///         Constructs a FormulaFormatException containing the explanatory message.
    ///     </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
        // All this does is call the base constructor. No extra code needed.
    }
}