// <author> Aishwarya Golakoti </author>
// <date> 24 September 2024 </date>
// For CS3500
using CS3500.Formula;
using CS3500.Spreadsheet;

namespace SpreadsheetTests
{
    /// <summary>
    ///     This is a test class for Spreadsheet and contains all Spreadsheet Unit Tests.
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        // ---- Tests for GetNamesOfAllNonemptyCells ----

        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells_EmptySpreadsheet_EmptySet()
        {
            Spreadsheet s = new();
            HashSet<string> expected = new();
            Assert.IsTrue(expected.SetEquals(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells_NonemptySpreadsheet_CorrectSet()
        {
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));
            HashSet<string> expected = new HashSet<string>{"A3", "B7", "C9"};
            Assert.IsTrue(expected.SetEquals(s.GetNamesOfAllNonemptyCells()));
        }

        // ---- Tests for GetCellContents ----

        [TestMethod]
        public void TestGetCellContents_EmptySpreadsheet_EmptyString()
        {
            Spreadsheet s = new();
            Assert.AreEqual(string.Empty, s.GetCellContents("a7"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContents_InvalidName_ThrowException()
        {
            Spreadsheet s = new();
            s.GetCellContents("b8D");
        }

        [TestMethod]
        public void TestGetCellContents_ValidName_String()
        {
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));
            Assert.AreEqual("hello", s.GetCellContents("b7"));
        }

        [TestMethod]
        public void TestGetCellContents_ValidName_Double()
        {
            // valid name, return a double
            // valid name, return a string
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));
            Assert.AreEqual(4.9, s.GetCellContents("a3"));
        }

        [TestMethod]
        public void TestGetCellContents_ValidName_Formula()
        {
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));
            Assert.AreEqual(new Formula("64*53+A3"), s.GetCellContents("c9"));
        }

        // ---- Tests for SetCellContents double ----

        [TestMethod]
        public void TestSetCellContentsDouble_EmptySpreadsheet_ListWithOneItem()
        {
            Spreadsheet s = new();
            List<string> expected = new List<string> { "A1" };
            List<string> actual = (List<string>)s.SetCellContents("a1", 9.3);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(9.3, s.GetCellContents("a1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsDouble_InvalidName_ThrowException()
        {
            Spreadsheet s = new();
            s.SetCellContents("bj8s*", 9.3);
        }

        [TestMethod]
        public void TestSetCellContentsDouble_ValidNameNoDependents_ListWithOneItem()
        {
            // valid name, no dependents so return a list with only the updated cell's name.
            // updates the contents of the cell correctly (check with GetCellContents method)
        }

        [TestMethod]
        public void TestSetCellContentsDouble_ValidNameSomeDependents_CorrectList()
        {
            // valid name, a couple dependents, returns correct list
            // updates the contents of the cell correctly (check with GetCellContents method)
        }

        [TestMethod]
        public void TestSetCellContentsDouble_ValidNameManyDependents_CorrectList()
        {
            // valid name, a lot more dependents, returns correct list
            // updates the contents of the cell correctly (check with GetCellContents method)
        }

        // ---- Tests for SetCellContents string ----

        [TestMethod]
        public void TestSetCellContentsString_EmptySpreadsheet_ListWithOneItem()
        {
            Spreadsheet s = new();
            List<string> expected = new List<string> { "A1" };
            List<string> actual = (List<string>)s.SetCellContents("a1", "hello");
            Assert.AreEqual(expected, actual);
            Assert.AreEqual("hello", s.GetCellContents("a1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsString_InvalidName_ThrowException()
        {
            Spreadsheet s = new();
            s.SetCellContents("bj8s*", "hello");
        }

        [TestMethod]
        public void TestSetCellContentsString_ValidNameNoDependents_ListWithOneItem()
        {
            // valid name, no dependents so return a list with only the updated cell's name.
            // updates the contents of the cell correctly (check with GetCellContents method)
        }

        [TestMethod]
        public void TestSetCellContentsString_ValidNameSomeDependents_CorrectList()
        {
            // valid name, a couple dependents, returns correct list
            // updates the contents of the cell correctly (check with GetCellContents method)
        }

        [TestMethod]
        public void TestSetCellContentsString_ValidNameManyDependents_CorrectList()
        {
            // valid name, a lot more dependents, returns correct list
            // updates the contents of the cell correctly (check with GetCellContents method)
        }

        // ---- Tests for SetCellContents Formula ----

        [TestMethod]
        public void TestSetCellContentsFormula_EmptySpreadsheet_ListWithOneItem()
        {
            // empty spreadsheet, return a list with only the updated cell's name.
            Spreadsheet s = new();
            List<string> expected = new List<string> { "A1" };
            List<string> actual = (List<string>)s.SetCellContents("a1", new Formula("4+2"));
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(new Formula("4+2"), s.GetCellContents("a1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsFormula_InvalidName_ThrowException()
        {
            Spreadsheet s = new();
            s.SetCellContents("bj8s*", new Formula("4+2"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsFormula_CircularDependency_ThrowException()
        {
            // changing the contents of the cell will cause a circular dependency, so check that it does throw.
            // check that the spreadsheet is not changed? (is it possible to check this?)
        }

        [TestMethod]
        public void TestSetCellContentsFormula_ValidNameNoDependents_ListWithOneItem()
        {
            // valid name, no dependents so return a list with only the updated cell's name.
            // updates the contents of the cell correctly (check with GetCellContents method)
        }

        [TestMethod]
        public void TestSetCellContentsFormula_ValidNameSomeDependents_CorrectList()
        {
            // valid name, a couple dependents, returns correct list
            // updates the contents of the cell correctly (check with GetCellContents method)
        }

        [TestMethod]
        public void TestSetCellContentsFormula_ValidNameManyDependents_CorrectList()
        {
            // valid name, a lot more dependents, returns correct list
            // updates the contents of the cell correctly (check with GetCellContents method)
        }


    }
}