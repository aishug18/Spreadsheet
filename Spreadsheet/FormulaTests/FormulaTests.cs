// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Aishwarya Golakoti </authors>
// <date> _ </date>

namespace CS3500.FormulaTests;

using CS3500.Formula;
using System.Runtime.CompilerServices;

/// <summary>
///   <para>
///     This class contains unit tests for the Formula class.
///   </para>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.
    ///       </item>
    ///     </list>
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        _ = new Formula(string.Empty);
    }

    /// <summary>
    ///     This test ensures that the Formula constructor will NOT throw an exception when there
    ///     is at least one token in the formula passed to the constructor.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOneToken_Valid()
    {
        _ = new Formula("1");
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    ///     This test ensures that positive whole numbers are valid tokens.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenPositiveWholeNumber_Valid()
    {
        _ = new Formula("23");
    }

    /// <summary>
    ///     This test ensures that positive decimals are valid tokens.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenPositiveDecimal_Valid()
    {
        _ = new Formula("3.12");
    }

    /// <summary>
    ///     This test ensures that numbers in scientific notation, with a lowercase "e" and a positive exponent, are valid tokens.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenScientificNotationLowercaseE_Valid()
    {
        _ = new Formula("2e5");
    }

    /// <summary>
    ///     This test ensures that numbers in scientific notation, with an uppercase "e" and a positive exponent, are valid tokens.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenScientificNotationUppercaseE_Valid()
    {
        _ = new Formula("2.4E5");
    }

    /// <summary>
    ///     This test ensures that numbers in scientific notation, with a negative exponent, are valid tokens.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenScientificNotationNegativeExponent_Valid()
    {
        _ = new Formula("8.2E-6");
    }

    /// <summary>
    ///     This test ensures that negative numbers are not valid tokens and will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTokenNegativeNumber_Invalid()
    {
        _ = new Formula("-3");
    }

    /// <summary>
    ///     This test ensures that the operator "+" (add) is a valid token.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenAdd_Valid()
    {
        _ = new Formula("1+1");
    }

    /// <summary>
    ///     This test ensures that the operator "-" (subtract) is a valid token.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenSubtract_Valid()
    {
        _ = new Formula("1-1");
    }

    /// <summary>
    ///     This test ensures that the operator "*" (multiply) is a valid token.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenMultiply_Valid()
    {
        _ = new Formula("2*3");
    }

    /// <summary>
    ///     This test ensures that the operator "/" (divide) is a valid token.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenDivide_Valid()
    {
        _ = new Formula("2/3");
    }

    /// <summary>
    ///     This test ensures that the mod symbol "%", which is often used in coding, is not
    ///     a valid symbol and will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTokenModSymbol_Invalid()
    {
        _ = new Formula("5%2");
    }

    /// <summary>
    ///     This test ensures that opening and closing parentheses are valid tokens.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokensOpeningAndClosingParentheses_Valid()
    {
        _ = new Formula("(4+5)");
    }

    /// <summary>
    ///     This test ensures that any symbols other than +, -, *, /, (, ) are invalid, such as "$", and will
    ///     throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTokenOtherSymbol_Invalid()
    {
        _ = new Formula("5$2");
    }

    /// <summary>
    ///     This test ensures that variables with one lowercase letter followed by one digit are valid tokens.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenVariableOneLetterOneDigit_Valid()
    {
        _ = new Formula("a4");
    }

    /// <summary>
    ///     This test ensures that variables with a few letters (lowercase and uppercase) followed by a few digits are valid tokens.
    ///     No exception should be thrown.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestTokenVariableFewLettersFewDigits_Valid()
    {
        _ = new Formula("abD421");
    }

    /// <summary>
    ///     This test ensures that a letter that is not followed by a digit is not a variable and hence not a
    ///     valid token, therefore should throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTokenVariableOnlyLetter_Invalid()
    {
        _ = new Formula("a");
    }

    /// <summary>
    ///     This test ensures that a formula with a digit followed by a letter is not a variable and hence not
    ///     a valid token, therefore should throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTokenVariableDigitLetter_Invalid()
    {
        _ = new Formula("1a");
    }

    /// <summary>
    ///     This test ensures that a formula with a letter, then a digit, then another letter is not a variable
    ///     and hence not a valid token, therefore should throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestTokenVariableLetterDigitLetter_Invalid()
    {
        _ = new Formula("a6B");
    }

    // --- Tests for Closing Parenthesis Rule ---

    /// <summary>
    ///     This test checks that the number of closing parentheses should never exceed the number of opening
    ///     parentheses seen so far when reading tokens from left to right, and should cause no errors (no
    ///     exception is thrown).
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestClosingParentheses_Valid()
    {
        _ = new Formula("(1+7)*4");
    }

    /// <summary>
    ///     This test ensures that the number of closing parentheses exceeding the number of opening
    ///     parentheses when reading tokens from left to right should throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestMoreClosingParentheses_Invalid()
    {
        _ = new Formula("(2+4) - 7)");
    }

    // --- Tests for Balanced Parentheses Rule ---

    /// <summary>
    ///     This test checks that the total number of opening parentheses is equal to the total number of closing
    ///     parentheses in the formula, and should cause no errors (no exception is thrown).
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestBalancedParentheses_Valid()
    {
        _ = new Formula("(7+(4*8)/2)");
    }

    /// <summary>
    ///     This test determines that the total number of opening parentheses is not equal to the total number of closing
    ///     parentheses in the formula, and should throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestUnbalancedParentheses_Invalid()
    {
        _ = new Formula("(6+(2/2)");
    }

    // --- Tests for First Token Rule ---

    /// <summary>
    ///     This test ensures that the first token being a number in the formula will not throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    /// <summary>
    ///     This test ensures that the first token being a variable in the formula will not throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenVariable_Valid()
    {
        _ = new Formula("a4-3");
    }

    /// <summary>
    ///     This test ensures that the first token being an opening parenthesis in the formula will not throw
    ///     an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenOpeningParenthesis_Valid()
    {
        _ = new Formula("(2*7) - B6");
    }

    /// <summary>
    ///     This test ensures that the first token being an operator will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenOperator_Invalid()
    {
        _ = new Formula("+5");
    }

    /// <summary>
    ///     This test ensures that the first token being a closing parenthesis will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenClosingParenthesis_Invalid()
    {
        _ = new Formula(")7+2");
    }

    // --- Tests for  Last Token Rule ---

    /// <summary>
    ///     This test ensures that the last token being a number will not throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenNumber_Valid()
    {
        _ = new Formula("6*8");
    }

    /// <summary>
    ///     This test ensures that the last token being a variable will not throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenVariable_Valid()
    {
        _ = new Formula("5+a4");
    }

    /// <summary>
    ///     This test ensures that the last token being a closing parenthesis will not throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenClosingParenthesis_Valid()
    {
        _ = new Formula("7+(2-3)");
    }

    /// <summary>
    ///     This test ensures that the last token being an operator will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenOperator_Invalid()
    {
        _ = new Formula("7+");
    }

    /// <summary>
    ///     This test ensures that the last token being an opening parenthesis will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenOpeningParenthesis_Invalid()
    {
        _ = new Formula("(4+6)(");
    }

    // --- Tests for Parentheses/Operator Following Rule ---

    /// <summary>
    ///     This test ensures that an opening parenthesis followed by a number will not throw an exception.
    ///     This also ensures that a number followed by an operator is valid.
    ///     This also ensures that a number followed by a closing parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOpeningParenthesisFollowedByNumber_Valid()
    {
        _ = new Formula("(2+3)");
    }

    /// <summary>
    ///     This test ensures that an opening parenthesis followed by a variable will not throw an exception.
    ///     This also ensures that a variable followed by an operator is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOpeningParenthesisFollowedByVariable_Valid()
    {
        _ = new Formula("(D6*4)");
    }

    /// <summary>
    ///     This test ensures that an opening parenthesis followed by an opening parenthesis will not throw an exception.
    ///     This also ensures that a closing parenthesis followed by an operator is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOpeningParenthesisFollowedByOpeningParenthesis_Valid()
    {
        _ = new Formula("((4+2)*4)");
    }

    /// <summary>
    ///     This test ensures that an opening parenthesis followed by a closing parenthesis will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisFollowedByClosingParenthesis_Invalid()
    {
        _ = new Formula("()");
    }

    /// <summary>
    ///     This test ensures that an opening parenthesis followed by an operator will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpeningParenthesisFollowedByOperator_Invalid()
    {
        _ = new Formula("(+2)");
    }

    /// <summary>
    ///     This test ensures that an operator followed by an opening parenthesis will not throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowedByOpeningParenthesis_Valid()
    {
        _ = new Formula("6+(4/2)");
    }

    /// <summary>
    ///     This test ensures that an operator followed by a number will not throw an exception.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowedByNumber_Valid()
    {
        _ = new Formula("4/2");
    }

    /// <summary>
    ///     This test ensures that an operator followed by a variable will not throw an exception.
    ///     This also ensures that a variable followed by a closing parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowedByVariable_Valid()
    {
        _ = new Formula("(4*z4)");
    }

    /// <summary>
    ///     This test ensures that an operator followed by an operator will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowedByOperator_Invalid()
    {
        _ = new Formula("4++6");
    }

    /// <summary>
    ///     This test ensures that an operator followed by a closing parenthesis will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowedByClosingParenthesis_Invalid()
    {
        _ = new Formula("(9+)");
    }

    // --- Tests for Extra Following Rule ---

    /// <summary>
    ///     This test ensures that a closing parenthesis followed by a closing parenthesis will not throw an exception.
    ///     This also ensures that a variable followed by a closing parenthesis is valid.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestClosingParenthesisFollowedByClosingParenthesis_Valid()
    {
        _ = new Formula("(2*(7+a4))");
    }

    /// <summary>
    ///     This test ensures that a number followed by a number will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowedByNumber_Invalid()
    {
        _ = new Formula("4 7");
    }

    /// <summary>
    ///     This test ensures that a number followed by a variable will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowedByVariable_Invalid()
    {
        _ = new Formula("8 A7");
    }

    /// <summary>
    ///     This test ensures that a number followed by an opening parenthesis will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowedByOpeningParenthesis_Invalid()
    {
        _ = new Formula("6(4+2)");
    }

    /// <summary>
    ///     This test ensures that a variable followed by a number will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableFollowedByNumber_Invalid()
    {
        _ = new Formula("R3 9");
    }

    /// <summary>
    ///     This test ensures that a variable followed by a variable will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableFollowedByVariable_Invalid()
    {
        _ = new Formula("S2 b8");
    }

    /// <summary>
    ///     This test ensures that a variable followed by an opening parenthesis will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableFollowedByOpeningParenthesis_Invalid()
    {
        _ = new Formula("k3(2+4)");
    }

    /// <summary>
    ///     This test ensures that a closing parenthesis followed by a number will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisFollowedByNumber_Invalid()
    {
        _ = new Formula("(3+2)7");
    }

    /// <summary>
    ///     This test ensures that a closing parenthesis followed by a variable will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisFollowedByVariable_Invalid()
    {
        _ = new Formula("(3*4)s1");
    }

    /// <summary>
    ///     This test ensures that a closing parenthesis followed by an opening parenthesis will throw an exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisFollowedByOpeningParenthesis_Invalid()
    {
        _ = new Formula("8)(");
    }

    // ----- Tests for GetVariables Method -----

    [TestMethod]
    public void FormulaGetVariables_TestLowercaseVariables_UppercaseVariables()
    {
        // input lowercase variables, should get uppercase variables
        Formula formula = new Formula("a3+bd89*(4-h2)/l9");
        HashSet<string> actual = (HashSet<string>)formula.GetVariables();
        HashSet<string> expected = new HashSet<string> { "A3", "BD89", "H2", "L9"};
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void FormulaGetVariables_TestUppercaseVariables_UppercaseVariables()
    {
        // input uppercase variables, should get uppercase variables
        Formula formula = new Formula("T3+WD89/K9");
        HashSet<string> actual = (HashSet<string>)formula.GetVariables();
        HashSet<string> expected = new HashSet<string> { "T3", "WD89", "K9"};
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void FormulaGetVariables_TestDuplicateVariables_NoDuplicateVariables()
    {
        // input duplicate variables, should get no duplicate variables
        Formula formula = new Formula("a3+bd89*(4-a3)/l9 - BD89");
        HashSet<string> actual = (HashSet<string>)formula.GetVariables();
        HashSet<string> expected = new HashSet<string> { "A3", "BD89", "L9" };
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void FormulaGetVariables_TestNoVariables_EmptyList()
    {
        // input no variables, should get empty list
        Formula formula = new Formula("3+3-9");
        HashSet<string> actual = (HashSet<string>)formula.GetVariables();
        HashSet<string> expected = new HashSet<string> { };
        Assert.IsTrue(expected.SetEquals(actual));
    }

    // ----- Tests for ToString Method -----

    /// <summary>
    ///     This test checks if a formula with lowercase variables, spaces, numbers in scientific notation, and decimals
    ///     with unnecessary digits is turned into a formula with uppercase variables, no spaces, and numbers in their default
    ///     form.
    /// </summary>
    [TestMethod]
    public void FormulaToString_TestLowercaseVarSpacesScientificNotationDecimals_ProperCanonicalFormula()
    {
        Formula formula = new Formula("a3+   5.100*(4  - 4e2)/    9E3");
        string actual = formula.ToString();
        string expected = "A3+5.1*(4-400)/9000";
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FormulaToString_TestCanonicalFormula_SameFormula()
    {
        Formula formula = new Formula("G3-7.9+300/BD6");
        string actual = formula.ToString();
        string expected = "G3-7.9+300/BD6";
        Assert.AreEqual(expected, actual);
    }

    // ----- more tests for new methods for PS4 -----
    // DO NOT FORGET NECESSARY COMMENTS

    // ----- Tests for == -----

    /// <summary>
    ///     Test the == operator, checks that two different formula objects are not equal, return false.
    /// </summary>
    [TestMethod]
    public void OperatorEquals_TestNotEqual_False()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("k3+9 / (7+2)");
        Assert.IsFalse(f1 == f2);
    }

    /// <summary>
    ///     Test the == operator, checks that two formula objects that are the same exactly (when inputted)
    ///     are equal, return true.
    /// </summary>
    [TestMethod]
    public void OperatorEquals_TestExactlyEqual_True()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("a1+4 / 7");
        Assert.IsTrue(f1 == f2);
    }

    /// <summary>
    ///     Test the == operator, checks that two formula objects that are not the same exactly but the same
    ///     canonically are equal, return true.
    /// </summary>
    [TestMethod]
    public void OperatorEquals_TestEqualCanonically_True()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("A1+4/7");
        Assert.IsTrue(f1 == f2);
    }

    // ----- Tests for != -----

    /// <summary>
    ///     Test the != operator, checks that two different formula objects are not equal, return true.
    /// </summary>
    [TestMethod]
    public void OperatorNotEquals_TestNotEqual_True()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("k3+9 / (7+2)");
        Assert.IsTrue(f1 != f2);
    }

    /// <summary>
    ///     Test the != operator, checks that two formula objects that are the same exactly (when inputted)
    ///     are equal, return false.
    [TestMethod]
    public void OperatorNotEquals_TestExactlyEqual_False()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("a1+4 / 7");
        Assert.IsFalse(f1 != f2);
    }

    /// <summary>
    ///     Test the != operator, checks that two formula objects that are not the same exactly but the same
    ///     canonically are equal, return false.
    /// </summary>
    [TestMethod]
    public void OperatorNotEquals_TestEqualCanonically_False()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("A1+4/7");
        Assert.IsFalse(f1 != f2);
    }

    // ----- Tests for Equals -----

    /// <summary>
    ///     Test Equals method, checks that if the input is null, return false.
    /// </summary>
    [TestMethod]
    public void Equals_TestNullParameter_False()
    {
        Formula f1 = new Formula("4+3");
        Formula f2 = null;
        Assert.IsFalse(f1.Equals(f2));
    }

    /// <summary>
    ///     Test Equals method, checks that if the input is not a Formula object, return false.
    /// </summary>
    [TestMethod]
    public void Equals_TestNotFormulaParameter_False()
    {
        Formula f1 = new Formula("4+3");
        List<string> list = new List<string> { "4+3" };
        Assert.IsFalse(f1.Equals(list));
    }

    [TestMethod]
    public void Equals_TestDifferentFormulas_False()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("k3+9 / (7+2)");
        Assert.IsFalse(f1.Equals(f2));
    }

    /// <summary>
    ///     Test Equals method, checks that two Formula objects have exactly the same formulas.
    /// </summary>
    [TestMethod]
    public void Equals_TestExactlySameFormulas_True()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("a1+4 / 7");
        Assert.IsTrue(f1.Equals(f2));
    }

    /// <summary>
    ///     Test Equals method, checks that two Formula objects have not exactly the same formulas but the 
    ///     same canonical formulas.
    /// </summary>
    [TestMethod]
    public void Equals_TestCanonicallySameFormulas_True()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("A1+4/7");
        Assert.IsTrue(f1.Equals(f2));
    }

    // ----- Tests for Evaluate -----

    /// <summary>
    ///     Test Evaluate method, using a formula with no variables, so the input delegate just returns 0.
    /// </summary>
    [TestMethod]
    public void Evaluate_FormulaNoVars_EvaluateCorrectly()
    {
        Formula f1 = new Formula("4+5 / (7-4)");
        Assert.AreEqual(3, f1.Evaluate(a => 0));
    }

    /// <summary>
    ///     Test Evaluate method, using a formula with variables, checks that the delegate is used properly.
    /// </summary>
    [TestMethod]
    public void Evaluate_FormulaWithVars_EvaluateCorrectly()
    {
        Formula f1 = new Formula("(4*b2) / (a1 - 2)");
        Assert.AreEqual(4, f1.Evaluate(a => { if (a == "A1") return 5; else return 3; }));
    }

    /// <summary>
    ///     Test Evaluate method, using a formula with variables, checks that Evaluate returns a FormulaError
    ///     because a variable in the formula doesn't have a value.
    /// </summary>
    [TestMethod]
    public void Evaluate_FormulaWithVarsWithNoValue_ReturnFormulaError()
    {
        Formula f1 = new Formula("(4*b2) / (a1 - 2)");
        Assert.IsInstanceOfType<FormulaError>(f1.Evaluate(a => { if (a == "A1") return 5; else throw new ArgumentException(); }));
    }

    /// <summary>
    ///     Test Evaluate method, using a formula with no variables, checks that dividing by 0 returns a FormulaError.
    /// </summary>
    [TestMethod]
    public void Evaluate_FormulaNoVarsDivideByZero_ReturnFormulaError()
    {
        Formula f1 = new Formula("5 - 2/0");
        Assert.IsInstanceOfType<FormulaError>(f1.Evaluate(a => 0));
    }

    /// <summary>
    ///     Test Evaluate method, using a formula with no variables, checks that a variable=0 and dividing by
    ///     the variable returns a FormulaError.
    /// </summary>
    [TestMethod]
    public void Evaluate_FormulaVarDivideByZero_ReturnFormulaError()
    {
        Formula f1 = new Formula("5 - 2/a2");
        Assert.IsInstanceOfType<FormulaError>(f1.Evaluate(a => { if (a == "A2") return 0; else throw new ArgumentException(); }));
    }

    /// <summary>
    ///     Test Evaluate method, using a formula with no variables, checks that dividing by 0 after ) returns
    ///     a FormulaError.
    /// </summary>
    [TestMethod]
    public void Evaluate_FormulaCloseParenThenDivideByZero_ReturnFormulaError()
    {
        Formula f1 = new Formula("(5 - 2)/0");
        Assert.IsInstanceOfType<FormulaError>(f1.Evaluate(a => 0));
    }

    // ----- Tests for GetHashCode -----

    /// <summary>
    ///     Test GetHashCode, checks that if f1.Equals(f2), then their hash codes are equal.
    /// </summary>
    [TestMethod]
    public void GetHashCode_TwoEqualFormulas_EqualHashCode()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("a1+4 / 7");
        Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
    }

    /// <summary>
    ///     Test GetHashCode, checks that if !f1.Equals(f2), then their hash codes are not equal.
    /// </summary>
    [TestMethod]
    public void GetHashCode_TwoDifferentFormulas_NotEqualHashCode()
    {
        Formula f1 = new Formula("a1+4 / 7");
        Formula f2 = new Formula("(5 - 2)/a2");
        Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
    }
}