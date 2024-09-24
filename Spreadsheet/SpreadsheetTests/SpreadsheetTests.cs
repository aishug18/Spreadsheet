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
            List<string> expectedList = new List<string> { "A1" };
            List<string> actualList = (List<string>)s.SetCellContents("a1", 9.3);
            Assert.AreEqual(expectedList, actualList);

            Assert.AreEqual(9.3, s.GetCellContents("A1"));

            HashSet<string> expectedSet = new HashSet<string> { "A1" };
            Assert.IsTrue(expectedSet.SetEquals(s.GetNamesOfAllNonemptyCells()));
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
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));

            List<string> expected = new List<string> { "B7" };
            List<string> actual = (List<string>)s.SetCellContents("b7", 19.8);
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(19.8, s.GetCellContents("b7"));

            HashSet<string> expectedSet = new HashSet<string> { "A1", "B7", "C9" };
            Assert.IsTrue(expectedSet.SetEquals(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        public void TestSetCellContentsDouble_ValidNameWithDependents_CorrectList()
        {
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));
            s.SetCellContents("b3", new Formula("a3/2 + c9"));

            List<string> expected = new List<string> { "A3", "C9", "B3" };
            List<string> actual = (List<string>)s.SetCellContents("a3", 19.8);
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(19.8, s.GetCellContents("a3"));
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
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));

            List<string> expected = new List<string> { "B7" };
            List<string> actual = (List<string>)s.SetCellContents("b7", "tomato");
            Assert.AreEqual(expected, actual);

            Assert.AreEqual("tomato", s.GetCellContents("b7"));
        }

        [TestMethod]
        public void TestSetCellContentsString_ValidNameWithDependents_CorrectList()
        {
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));
            s.SetCellContents("b3", new Formula("a3/2 + c9"));

            List<string> expected = new List<string> { "A3", "C9", "B3" };
            List<string> actual = (List<string>)s.SetCellContents("a3", "yellow");
            Assert.AreEqual(expected, actual);

            Assert.AreEqual("yellow", s.GetCellContents("a3"));
        }

        // ---- Tests for SetCellContents Formula ----

        [TestMethod]
        public void TestSetCellContentsFormula_EmptySpreadsheet_ListWithOneItem()
        {
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
        public void TestSetCellContentsFormula_CircularDependency_ThrowException()
        {
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));
            s.SetCellContents("b3", new Formula("a3/2 + c9"));

            Assert.ThrowsException<CircularException>(() => { s.SetCellContents("a3", new Formula("22/2 + c9")); });

            Assert.AreEqual(4.9, s.GetCellContents("A3"));
        }

        [TestMethod]
        public void TestSetCellContentsFormula_ValidNameNoDependents_ListWithOneItem()
        {
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));

            List<string> expected = new List<string> { "C9" };
            List<string> actual = (List<string>)s.SetCellContents("c9", new Formula("73*2 + 3"));
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(new Formula("73*2 + 3"), s.GetCellContents("c9"));
        }

        [TestMethod]
        public void TestSetCellContentsFormula_ValidNameWithDependents_CorrectList()
        {
            Spreadsheet s = new();
            s.SetCellContents("a3", 4.9);
            s.SetCellContents("b7", "hello");
            s.SetCellContents("c9", new Formula("64*53 + a3"));
            s.SetCellContents("b3", new Formula("a3/2 + c9"));

            List<string> expected = new List<string> { "A3", "C9", "B3" };
            List<string> actual = (List<string>)s.SetCellContents("a3", new Formula("22/2 + 5"));
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(new Formula("22/2 + 5"), s.GetCellContents("a3"));
        }


    }
}