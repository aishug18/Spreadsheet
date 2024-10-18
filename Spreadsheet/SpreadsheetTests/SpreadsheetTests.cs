// <author> Aishwarya Golakoti </author>
// <date> 18 October 2024 </date>
// For CS3500
using CS3500.Formula;
using CS3500.Spreadsheet;
using System.Text.Json;

namespace SpreadsheetTests
{
    /// <summary>
    ///     This is a test class for Spreadsheet and contains all Spreadsheet Unit Tests.
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        // ---- Tests for both constructors ----

        [TestMethod]
        public void TestDefaultConstructor_EmptySpreadsheet()
        {
            Spreadsheet s = new();
            HashSet<string> expected = new();
            Assert.IsTrue(expected.SetEquals(s.GetNamesOfAllNonemptyCells()));
            Assert.IsFalse(s.Changed);
        }

        [TestMethod]
        public void TestConstructorWithFile_NoErrors_Successful()
        {
            string sheet = "{\"Cells\": {\"A3\": {\"StringForm\": \"4.9\"}, \"B7\": {\"StringForm\": \"hello\"}, " +
                "\"C9\": {\"StringForm\": \"=64*53+A3\"}}}";
            File.WriteAllText("file.txt", sheet);

            Spreadsheet s = new("file.txt");

            Assert.IsFalse(s.Changed);
            HashSet<string> expected = new HashSet<string> { "A3", "B7", "C9" };
            Assert.IsTrue(expected.SetEquals(s.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        ///     Checks that when the deserializer returns a null value, an exception is thrown. When it's null, it 
        ///     means the JSON is in invalid format. In this case, the string "sheet" that represents a JSON string
        ///     is missing a set of curly braces, which is needed to denote something as a Dictionary.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorWithFile_Null_Exception()
        {
            string sheet = "{\"Cells\": \"A3\": {\"Property\": \"4.9\"}, \"B7\": {\"StringForm\": \"hello\"}, " +
                "\"C9\": {\"StringForm\": \"=64*53+A3\"}}";
            File.WriteAllText("file.txt", sheet);

            _ = new Spreadsheet("file.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorWithFile_FilenameError_Exception()
        {
            _ = new Spreadsheet("/some/nonsense/path.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorWithFile_InvalidCellNameError_Exception()
        {
            string sheet = "{\"Cells\": {\"A3\": {\"StringForm\": \"4.9\"}, \"B7\": {\"StringForm\": \"hello\"}, " +
                "\"C9B\": {\"StringForm\": \"=64*53+A3\"}}}";
            File.WriteAllText("file.txt", sheet);

            _ = new Spreadsheet("file.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorWithFile_InvalidFormulaError_Exception()
        {
            string sheet = "{\"Cells\": {\"A3\": {\"StringForm\": \"4.9\"}, \"B7\": {\"StringForm\": \"hello\"}, " +
                "\"C9\": {\"StringForm\": \"=64*53#+A3\"}}}";
            File.WriteAllText("file.txt", sheet);

            _ = new Spreadsheet("file.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorWithFile_CircularDependencyError_Exception()
        {
            string sheet = "{\"Cells\": {\"A3\": {\"StringForm\": \"=22/2+C9\"}, \"B7\": {\"StringForm\": \"hello\"}, " +
                "\"C9\": {\"StringForm\": \"=64*53+A3\"}, \"B3\": {\"StringForm\": \"=A3/2+C9\"}}}";
            File.WriteAllText("file.txt", sheet);

            _ = new Spreadsheet("file.txt");
        }

        // ---- Tests for Save ----

        [TestMethod]
        public void TestSave_NoErrors_Successful()
        {
            Spreadsheet s1 = new();
            s1.SetContentsOfCell("a3", "4.9");
            s1.SetContentsOfCell("b7", "hello");
            s1.SetContentsOfCell("c9", "=64*53 + a3");

            Assert.IsTrue(s1.Changed);
            s1.Save("file.txt");
            Assert.IsFalse(s1.Changed);

            Spreadsheet s2 = new("file.txt");

            Assert.AreEqual(JsonSerializer.Serialize(s1), JsonSerializer.Serialize(s2));
            Assert.IsTrue(s1.GetNamesOfAllNonemptyCells().SetEquals(s2.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        ///     Checks that when saving to an already existing file, the contents of that file are overwritten.
        /// </summary>
        [TestMethod]
        public void TestSave_ExistingFile_OverwriteFile()
        {
            string sheet = "{\"Cells\": {\"D8\": {\"StringForm\": \"9.10\"}, \"C3\": {\"StringForm\": \"30.8\"}, " +
                "\"P9\": {\"StringForm\": \"NEVERmore\"}, \"H5\": {\"StringForm\": \"=39*C3+2\"}}}";
            File.WriteAllText("existing_file.txt", sheet);

            Spreadsheet oldSpreadsheet = new("existing_file.txt");

            Spreadsheet newSpreadsheet = new();
            newSpreadsheet.SetContentsOfCell("a3", "4.9");
            newSpreadsheet.SetContentsOfCell("b7", "hello");
            newSpreadsheet.SetContentsOfCell("c9", "=64*53 + a3");

            Assert.IsTrue(newSpreadsheet.Changed);
            newSpreadsheet.Save("existing_file.txt");   // save to existing file, so overwrite it
            Assert.IsFalse(newSpreadsheet.Changed);

            Assert.AreNotEqual(JsonSerializer.Serialize(oldSpreadsheet), JsonSerializer.Serialize(newSpreadsheet));
        }

        /// <summary>
        ///     Checks that when saving to a file that has an invalid name, an exception is thrown.
        /// </summary>
        [TestMethod]
        public void TestSave_Error_Exception()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");

            Assert.IsTrue(s.Changed);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => { s.Save("/some/nonsense/path.txt"); });
            Assert.IsTrue(s.Changed);
        }

        // ---- Tests for indexer ----

        [TestMethod]
        public void TestIndexer_EmptySpreadsheet_EmptyString()
        {
            Spreadsheet s = new();
            Assert.AreEqual(string.Empty, s["a7"]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestIndexer_InvalidName_ThrowException()
        {
            Spreadsheet s = new();
            _ = s["b8d"];
        }

        [TestMethod]
        public void TestIndexer_ValidName_String()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            Assert.AreEqual("hello", s["b7"]);
        }

        [TestMethod]
        public void TestIndexer_ValidName_Double()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            Assert.AreEqual(4.9, s["a3"]);
        }

        [TestMethod]
        public void TestIndexer_ValidName_Formula()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            Assert.AreEqual(3396.9, s["c9"]);
        }

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
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
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

        /// <summary>
        ///     Tests that GetCellContents returns a string as the contents.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents_ValidName_String()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            Assert.AreEqual("hello", s.GetCellContents("b7"));
        }

        /// <summary>
        ///     Tests that GetCellContents returns a double as the contents.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents_ValidName_Double()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            Assert.AreEqual(4.9, s.GetCellContents("a3"));
        }

        /// <summary>
        ///     Tests that GetCellContents returns a Formula as the contents.
        /// </summary>
        [TestMethod]
        public void TestGetCellContents_ValidName_Formula()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            Assert.AreEqual(new Formula("64*53+A3"), s.GetCellContents("c9"));
        }

        // ---- Tests for GetCellValue ----

        [TestMethod]
        public void TestGetCellValue_EmptySpreadsheet_EmptyString()
        {
            Spreadsheet s = new();
            Assert.AreEqual(string.Empty, s.GetCellValue("a7"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValue_InvalidName_ThrowException()
        {
            Spreadsheet s = new();
            s.GetCellValue("b8D");
        }

        /// <summary>
        ///     Tests that GetCellValue returns a string as the value.
        /// </summary>
        [TestMethod]
        public void TestGetCellValue_ValidName_String()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            Assert.AreEqual("hello", s.GetCellValue("b7"));
        }

        /// <summary>
        ///     Tests that GetCellValue returns a double as the value when the contents are a double.
        /// </summary>
        [TestMethod]
        public void TestGetCellValue_ValidNameDoubleContents_Double()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            Assert.AreEqual(4.9, s.GetCellValue("a3"));
        }

        /// <summary>
        ///     Tests that GetCellValue returns a double as the value when the contents is a Formula.
        /// </summary>
        [TestMethod]
        public void TestGetCellValue_ValidNameFormulaContents_Double()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            Assert.AreEqual(3396.9, s.GetCellValue("c9"));
        }

        /// <summary>
        ///     Tests that GetCellValue returns a FormulaError as the value when the contents is a Formula.
        /// </summary>
        [TestMethod]
        public void TestGetCellValue_ValidNameFormulaContents_FormulaError()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + D8");
            Assert.IsInstanceOfType<FormulaError>(s.GetCellValue("c9"));
        }

        // ---- Tests for SetContentsOfCell ----

        /// <summary>
        ///     Tests that SetContentsOfCell sets the contents of an empty cell to a double, and the
        ///     returned List only contains the name of the cell.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_EmptySpreadsheetDouble_ListWithOneItem()
        {
            Spreadsheet s = new();
            List<string> expectedList = new List<string> { "A1" };
            List<string> actualList = (List<string>)s.SetContentsOfCell("a1", "9.3");
            CollectionAssert.AreEqual(expectedList, actualList);
            Assert.IsTrue(s.Changed);

            Assert.AreEqual(9.3, s.GetCellContents("A1"));
            Assert.AreEqual(9.3, s.GetCellValue("A1"));

            HashSet<string> expectedSet = new HashSet<string> { "A1" };
            Assert.IsTrue(expectedSet.SetEquals(s.GetNamesOfAllNonemptyCells()));
        }

        [TestMethod]
        public void TestSetContentsOfCell_InvalidName_ThrowException()
        {
            Spreadsheet s = new();
            Assert.ThrowsException<InvalidNameException>(() => { s.SetContentsOfCell("bj8s*", "9.3"); });
            Assert.IsFalse(s.Changed);
        }

        /// <summary>
        ///     Tests that SetContentsOfCell sets the contents of an already existing cell without
        ///     dependents to a double.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_ExistingCellNoDependentsDouble_CorrectlyUpdated()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "9.3");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");

            List<string> expected = new List<string> { "B7" };
            List<string> actual = (List<string>)s.SetContentsOfCell("b7", "19.3");
            CollectionAssert.AreEqual(expected, actual);
            Assert.IsTrue(s.Changed);

            Assert.AreEqual(19.3, s.GetCellContents("b7"));
            Assert.AreEqual(19.3, s.GetCellValue("b7"));

            HashSet<string> expectedSet = new HashSet<string> { "A3", "B7", "C9" };
            Assert.IsTrue(expectedSet.SetEquals(s.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        ///     Tests that SetContentsOfCell sets the contents of an already existing cell with 
        ///     dependents to a double. Also checks that the Changed property is updated appropriately.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_ExistingCellWithDependentsDouble_CorrectlyUpdated()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            s.SetContentsOfCell("b3", "=a3/2 + c9");

            List<string> expected = new List<string> { "A3", "C9", "B3" };
            List<string> actual = (List<string>)s.SetContentsOfCell("a3", "19.8");
            CollectionAssert.AreEqual(expected, actual);
            Assert.IsTrue(s.Changed);

            Assert.AreEqual(19.8, s.GetCellContents("a3"));
            Assert.AreEqual(19.8, s.GetCellValue("a3"));

            Assert.IsTrue(s.Changed);
            s.Save("file.txt");
            Assert.IsFalse(s.Changed);
        }

        /// <summary>
        ///     Tests that SetContentsOfCell sets the contents of an empty cell to a string.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_EmptySpreadsheetString_ListWithOneItem()
        {
            Spreadsheet s = new();
            List<string> expected = new List<string> { "A1" };
            List<string> actual = (List<string>)s.SetContentsOfCell("a1", "hello");
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual("hello", s.GetCellContents("a1"));
            Assert.AreEqual("hello", s.GetCellValue("a1"));
        }

        /// <summary>
        ///     Tests that SetContentsOfCell sets the contents of an already existing cell with 
        ///     dependents to a string.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_ExistingCellWithDependentsString_CorrectlyUpdated()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            s.SetContentsOfCell("b3", "=a3/2 + c9");

            List<string> expected = new List<string> { "A3", "C9", "B3" };
            List<string> actual = (List<string>)s.SetContentsOfCell("a3", "yellow");
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual("yellow", s.GetCellContents("a3"));
            Assert.AreEqual("yellow", s.GetCellValue("a3"));
        }

        /// <summary>
        ///     Tests that when setting the contents of an already existing cell to an empty string, the cell
        ///     gets deleted from the backing Dictionary and its contents are updated correctly.
        /// </summary>
        [TestMethod]
        public void TestSetCellContentsString_SetToEmpty_DeletesCell()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            s.SetContentsOfCell("b3", "=a3/2 + c9");

            List<string> expected = new List<string> { "A3", "C9", "B3" };
            List<string> actual = (List<string>)s.SetContentsOfCell("A3", "");
            CollectionAssert.AreEqual(expected, actual);
            Assert.IsTrue(s.Changed);

            Assert.AreEqual(string.Empty, s.GetCellContents("a3"));
            Assert.AreEqual(string.Empty, s.GetCellContents("a3"));

            HashSet<string> expectedSet = new HashSet<string> { "B7", "C9", "B3" };
            Assert.IsTrue(expectedSet.SetEquals(s.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        ///     Tests that SetContentsOfCell sets the contents of an empty cell to a Formula and the value
        ///     of that cell is the correct double.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_EmptySpreadsheetFormula_CalculatesCorrectValue()
        {
            Spreadsheet s = new();
            List<string> expected = new List<string> { "A1" };
            List<string> actual = (List<string>)s.SetContentsOfCell("a1", "=4+2");
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual(new Formula("4+2"), s.GetCellContents("a1"));
            Assert.AreEqual(6.0, s.GetCellValue("a1"));
        }

        /// <summary>
        ///     Checks that a CircularException was thrown when changing the contents of a cell results in a
        ///     circular dependency, and that the spreadsheet is not changed.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_CircularDependency_ThrowExceptionNoChange()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            s.SetContentsOfCell("b3", "=a3/2 + c9");

            Assert.IsTrue(s.Changed);
            s.Save("file.txt");
            Assert.IsFalse(s.Changed);

            Assert.ThrowsException<CircularException>(() => { s.SetContentsOfCell("a3", "=22/2 + c9"); });
            Assert.IsFalse(s.Changed);

            Assert.AreEqual(4.9, s.GetCellContents("A3"));
            Assert.AreEqual(4.9, s.GetCellValue("A3"));
        }

        /// <summary>
        ///     Tests that SetContentsOfCell sets the contents of an already existing cell with no
        ///     dependents to a Formula, and the value is a double.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_ExistingCellNoDependentsFormula_CorrectlyUpdates()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");

            List<string> expected = new List<string> { "C9" };
            List<string> actual = (List<string>)s.SetContentsOfCell("c9", "=73*2 + 3");
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual(new Formula("73*2 + 3"), s.GetCellContents("c9"));
            Assert.AreEqual(149.0, s.GetCellValue("c9"));
        }

        /// <summary>
        ///     Tests that SetContentsOfCell sets the contents of an already existing cell with 
        ///     dependents to a Formula, but evaluated the Formula causes a FormulaError.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_ExistingCellWithDependents_FormulaErrorAsValue()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "hello");
            s.SetContentsOfCell("c9", "=64*53 + a3");
            s.SetContentsOfCell("b3", "=a3/2 + c9");

            List<string> expected = new List<string> { "A3", "C9", "B3" };
            List<string> actual = (List<string>)s.SetContentsOfCell("a3", "=22/2 + B7");
            CollectionAssert.AreEqual(expected, actual);
            Assert.IsTrue(s.Changed);

            Assert.AreEqual(new Formula("22/2 + B7"), s.GetCellContents("a3"));
            Assert.IsInstanceOfType<FormulaError>(s.GetCellValue("a3"));
        }

        // ---- Additional Tests ----

        /// <summary>
        ///     Checks that when changing the contents of a cell from a formula with variables to a
        ///     double, the dependencies are correctly removed from the backing DependencyGraph.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_FormulaToNumber_CorrectList()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "=a3 + 27");
            s.SetContentsOfCell("c9", "=64*53 + b7");
            s.SetContentsOfCell("b3", "=c9-60 + b7");

            CollectionAssert.AreEqual(new List<string> { "B7", "C9", "B3" }, (List<string>)s.SetContentsOfCell("b7", "2"));

            s.SetContentsOfCell("b7", "=a3 + 27");   // reset the spreadsheet

            List<string> expected = new List<string> { "C9", "B3" };
            List<string> actual = (List<string>)s.SetContentsOfCell("c9", "89");
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual(89.0, s.GetCellContents("c9"));
            Assert.AreEqual(89.0, s.GetCellValue("c9"));
            Assert.AreEqual(60.9, s.GetCellValue("b3"));

            CollectionAssert.AreEqual(new List<string> { "B7", "B3" }, (List<string>)s.SetContentsOfCell("b7", "30"));
            Assert.AreEqual(59.0, s.GetCellValue("b3"));
        }

        /// <summary>
        ///     Checks that when changing the contents of a cell from a formula with variables to an
        ///     empty string (essentially deleting the cell), the dependencies are correctly removed 
        ///     from the backing DependencyGraph.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_FormulaToEmptyString_CorrectList()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "=a3 + 27");
            s.SetContentsOfCell("c9", "=64*53 + b7");
            s.SetContentsOfCell("b3", "=c9-60 + b7");

            CollectionAssert.AreEqual(new List<string> { "B7", "C9", "B3" }, (List<string>)s.SetContentsOfCell("b7", "2"));

            s.SetContentsOfCell("b7", "=a3 + 27");   // reset the spreadsheet

            List<string> expected = new List<string> { "C9", "B3" };
            List<string> actual = (List<string>)s.SetContentsOfCell("c9", string.Empty);
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual(string.Empty, s.GetCellContents("c9"));
            Assert.AreEqual(string.Empty, s.GetCellValue("c9"));
            Assert.IsInstanceOfType<FormulaError>(s.GetCellValue("b3"));

            CollectionAssert.AreEqual(new List<string> { "B7", "B3" }, (List<string>)s.SetContentsOfCell("b7", "30"));
            Assert.IsInstanceOfType<FormulaError>(s.GetCellValue("b3"));
        }

        /// <summary>
        ///     Checks that when changing the contents of a cell from a formula with variables to a
        ///     string, the dependencies are correctly removed from the backing DependencyGraph.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_FormulaToString_CorrectList()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "=a3 + 27");
            s.SetContentsOfCell("c9", "=64*53 + b7");
            s.SetContentsOfCell("b3", "=c9-60 + b7");

            CollectionAssert.AreEqual(new List<string> { "B7", "C9", "B3" }, (List<string>)s.SetContentsOfCell("b7", "2"));

            s.SetContentsOfCell("b7", "=a3 + 27");   // reset the spreadsheet

            List<string> expected = new List<string> { "C9", "B3" };
            List<string> actual = (List<string>)s.SetContentsOfCell("c9", "hello");
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual("hello", s.GetCellContents("c9"));
            Assert.AreEqual("hello", s.GetCellValue("c9"));
            Assert.IsInstanceOfType<FormulaError>(s.GetCellValue("b3"));

            CollectionAssert.AreEqual(new List<string> { "B7", "B3" }, (List<string>)s.SetContentsOfCell("b7", "nevermore"));
            Assert.IsInstanceOfType<FormulaError>(s.GetCellValue("b3"));
        }

        /// <summary>
        ///     Checks that when changing the contents of a cell from a formula with variables to a
        ///     formula with other variables, the dependencies are correctly removed from the backing DependencyGraph.
        /// </summary>
        [TestMethod]
        public void TestSetContentsOfCell_FormulaToFormula_CorrectList()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("a3", "4.9");
            s.SetContentsOfCell("b7", "=a3 + 27");
            s.SetContentsOfCell("c9", "=64*53 + b7");
            s.SetContentsOfCell("b3", "=c9-60 + b7");
            s.SetContentsOfCell("d9", "32.8");
            s.SetContentsOfCell("e8", "6.96");
            s.SetContentsOfCell("l2", "3.2");

            CollectionAssert.AreEqual(new List<string> { "B7", "C9", "B3" }, (List<string>)s.SetContentsOfCell("b7", "2"));

            s.SetContentsOfCell("b7", "=a3 + 27");   // reset the spreadsheet

            List<string> expected = new List<string> { "C9", "B3" };
            List<string> actual = (List<string>)s.SetContentsOfCell("c9", "=(d9+e8) * 6");
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual(new Formula("(d9+e8) * 6"), s.GetCellContents("c9"));
            Assert.AreEqual(238.56, s.GetCellValue("c9"));
            Assert.AreEqual(210.46, s.GetCellValue("b3"));

            CollectionAssert.AreEqual(new List<string> { "B7", "B3" }, (List<string>)s.SetContentsOfCell("b7", "=l2*5"));
            Assert.AreEqual(16.0, s.GetCellValue("b7"));
            Assert.AreEqual(194.56, s.GetCellValue("b3"));

            CollectionAssert.AreEqual(new List<string> { "A3" }, (List<string>)s.SetContentsOfCell("a3", "hello"));
        }

    }
}