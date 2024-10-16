// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
//     - Updated return types
//     - Updated documentation

// <author> Aishwarya Golakoti </author>
// <date> 18 October 2024 </date>
// For CS 3500
namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
/// <para>
/// Thrown to indicate that a read or write attempt has failed with
/// an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    /// <para>
    ///     Creates the exception with a message defining what went wrong.
    /// </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg) : base(msg)
    {
    }
}

/// <summary>
///   <para>
///     A Spreadsheet object represents the state of a simple spreadsheet. A
///     spreadsheet represents an infinite number of named cells.
///   </para>
///   <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
///   </para>
///   <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
///   </para>
///   <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
///   </para>
///   <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
///   </para>
///   <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
///   </para>
///   <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
///   </para>
///   <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.) We are not concerned with cell values yet, only with their contents,
///     but for context:
///   </para>
///   <list type="number">
///     <item>If a cell's contents is a string, its value is that string.</item>
///     <item>If a cell's contents is a double, its value is that double.</item>
///     <item>
///         <para>
///             If a cell's contents is a Formula, its value is either a double or a FormulaError,
///             as reported by the Evaluate method of the Formula class.  For this assignment,
///             you are not dealing with values yet.
///         </para>
///     </item>
///   </list>
///   <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself,
///     either directly or indirectly.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
///   </para>
/// </summary>
public class Spreadsheet
{
    /// <summary>
    ///     A Dependency Graph to keep track of relationships among cells.
    /// </summary>
    private DependencyGraph dg;

    /// <summary>
    ///     Keeps track of all non-empty cells. Maps cell names to Cell objects.
    /// </summary>
    private Dictionary<string, Cell> Cells;

    /// <summary>
    ///     True if this spreadsheet has been changed since it was
    ///     created or saved (whichever happened most recently),
    ///     False otherwise.
    /// </summary>
    public bool Changed { get; private set; }

    /// <summary>
    ///     Constructs an empty spreadsheet.
    /// </summary>
    public Spreadsheet()
    {
        dg = new();
        Cells = new();
    }

    /// <summary>
    ///     Constructs a spreadsheet using the saved data in the file refered to by
    ///     the given filename.
    /// <see cref="Save(string)"/>
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    ///     Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        dg = new();
        Cells = new();

        try
        {
            using (StreamReader obj = new(filename) )
            {

            }
        }
        catch(Exception e)
        {
            throw new SpreadsheetReadWriteException(e.Message);
        }
        
        
        throw new NotImplementedException();
    }

    /// <summary>
    ///     <para>
    ///         Writes the contents of this spreadsheet to the named file using a JSON format.
    ///         If the file already exists, overwrite it.
    ///     </para>
    ///     <para>
    ///         The output JSON should look like the following.
    ///     </para>
    ///     <para>
    ///         For example, consider a spreadsheet that contains a cell "A1"
    ///         with contents being the double 5.0, and a cell "B3" with contents
    ///         being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    ///     </para>
    ///     <para>
    ///         This method would produce the following JSON string:
    ///     </para>
    ///     <code>
    ///     {
    ///         "Cells": {
    ///             "A1": {
    ///                 "StringForm": "5"
    ///             },
    ///             "B3": {
    ///                 "StringForm": "=A1+2"
    ///             },
    ///             "C4": {
    ///                 "StringForm": "hello"
    ///             }
    ///         }
    ///     }
    ///     </code>
    ///     <para>
    ///         You can achieve this by making sure your data structure is a dictionary
    ///         and that the contained objects (Cells) have property named "StringForm"
    ///         (if this name does not match your existing code, use the JsonPropertyName
    ///         attribute).
    ///     </para>
    ///     <para>
    ///         There can be 0 cells in the dictionary, resulting in { "Cells" : {} }
    ///     </para>
    ///     <para>
    ///         Further, when writing the value of each cell...
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             If the contents is a string, the value of StringForm is that string
    ///         </item>
    ///         <item>
    ///             If the contents is a double d, the value of StringForm is d.ToString()
    ///         </item>
    ///         <item>
    ///             If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///     If there are any problems opening, writing, or closing the file,
    ///     the method should throw a SpreadsheetReadWriteException with an
    ///     explanatory message.
    /// </exception>
    public void Save(string filename)
    {

        Changed = false;
        throw new NotImplementedException();
    }

    /// <summary>
    /// <para>
    ///     Return the value of the named cell, as defined by <see cref="GetCellValue(string)"/>.
    /// </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///     <see cref="GetCellValue(string)"/>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name]
    {
        get { return GetCellValue(name); }
    }

    /// <summary>
    ///   Provides a copy of the normalized names of all of the cells in the spreadsheet
    ///   that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        HashSet<string> names = new();
        foreach (var cell in Cells)
        {
            names.Add(cell.Key);
        }

        return names;
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        if (!IsValidName(name))
            throw new InvalidNameException();

        if (Cells.TryGetValue(name.ToUpper(), out Cell? cell))
        {
            return cell.Contents;
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    ///     <para>
    ///         Return the value of the named cell.
    ///     </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///     Returns the value (as opposed to the contents) of the named cell. The return
    ///     value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string name)
    {
        if (!IsValidName(name))
            throw new InvalidNameException();

        if (Cells.TryGetValue(name.ToUpper(), out Cell? cell))
        {
            return cell.Value;
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    ///     <para>
    ///         Set the contents of the named cell to be the provided string
    ///         which will either represent (1) a string, (2) a number, or
    ///         (3) a formula (based on the prepended '=' character).
    ///     </para>
    ///     <para>
    ///         Rules of parsing the input string:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <para>
    ///                 If 'content' parses as a double, the contents of the named
    ///                 cell becomes that double.
    ///             </para>
    ///         </item>
    ///         <item>
    ///             If the string does not begin with an '=', the contents of the
    ///             named cell becomes 'content'.
    ///         </item>
    ///         <item>
    ///             <para>
    ///                 If 'content' begins with the character '=', an attempt is made
    ///                 to parse the remainder of content into a Formula f using the Formula
    ///                 constructor. There are then three possibilities:
    ///             </para>
    ///             <list type="number">
    ///                 <item>
    ///                     If the remainder of content cannot be parsed into a Formula, a
    ///                     CS3500.Formula.FormulaFormatException is thrown.
    ///                 </item>
    ///                 <item>
    ///                     Otherwise, if changing the contents of the named cell to be f
    ///                     would cause a circular dependency, a CircularException is thrown,
    ///                     and no change is made to the spreadsheet.
    ///                 </item>
    ///                 <item>
    ///                     Otherwise, the contents of the named cell becomes f.
    ///                 </item>
    ///             </list>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <returns>
    ///     <para>
    ///         The method returns a list consisting of the name plus the names
    ///         of all other cells whose value depends, directly or indirectly,
    ///         on the named cell. The order of the list should be any order
    ///         such that if cells are re-evaluated in that order, their dependencies
    ///         are satisfied by the time they are evaluated.
    ///     </para>
    /// <example>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.
    /// </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        if (!IsValidName(name))
            throw new InvalidNameException();
        
        Changed = true;
        IList<string> list;

        // if contents is a double
        if (double.TryParse(content, out double number))
        {
            list = SetCellContents(name.ToUpper(), number);
        }
        // if contents is a formula
        else if (content.StartsWith('='))
        {
            list = SetCellContents(name.ToUpper(), new Formula(content.Substring(1)));
        }
        // if contents is a string
        else
        {
            list = SetCellContents(name.ToUpper(), content);
        }

        Recalculate(list);

        return list;
    }

    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all of the cells, i.e., if you re-evaluate each cells in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///     evaluated, followed by B1, followed by C1.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        if (Cells.TryGetValue(name, out Cell? cell))
        {
            // remove old dependencies if required
            if(cell.Contents is Formula)
            {
                RemoveDependencies(name, (Formula)cell.Contents);
            }

            cell.Contents = number;
            cell.Value = number;
            cell.StringForm = number.ToString();
        }
        else
        {
            Cells.Add(name, new Cell(number));
        }

        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        if (text == string.Empty)   // remove the now empty cell from Dictionary
        {
            if (Cells.TryGetValue(name, out Cell? cell) && cell.Contents is Formula)
            {
                RemoveDependencies(name, (Formula)cell.Contents);
            }

            Cells.Remove(name);

        }
        else if (Cells.TryGetValue(name, out Cell? cell))
        {
            // remove old dependencies if required
            if (cell.Contents is Formula)
            {
                RemoveDependencies(name, (Formula)cell.Contents);
            }

            cell.Contents = text;
            cell.Value = text;
            cell.StringForm = text;
        }
        else
        {
            Cells.Add(name, new Cell(text));
        }

        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException, and no
    ///     change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        // update dependency graph with new dependencies if they exist
        HashSet<string> variables = (HashSet<string>)formula.GetVariables();
        foreach (string variable in variables)
        {
            dg.AddDependency(variable, name);
        }

        // store result to check if a CircularException occurs
        IList<string> result = GetCellsToRecalculate(name).ToList();

        if (Cells.TryGetValue(name, out Cell? cell))
        {
            // remove old dependencies if required
            if (cell.Contents is Formula)
            {
                RemoveDependencies(name, (Formula)cell.Contents);
            }
            cell.Contents = formula;
            cell.StringForm = "=" + formula.ToString();
        }
        else
        {
            Cells.Add(name, new Cell(formula));
        }

        return result;
    }

    private double Lookup(string variable)
    {
        if(Cells.TryGetValue(variable, out Cell? cell) && (cell.Value is double value))
        {
            return value;
        }
        else    // cell doesn't exist or the value is a string or a FormulaError
        {
            throw new ArgumentException();
        }
    }

    private void Recalculate(IList<string> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if (Cells.TryGetValue(list[i], out Cell? cell) && (cell.Contents is Formula f))
            {
                cell.Value = f.Evaluate(Lookup);
            }
        }
    }

    /// <summary>
    ///     Removes dependencies from the dependency graph given a cell name and a Formula.
    /// </summary>
    /// <param name="name"> name of the cell </param>
    /// <param name="formula">
    ///     previous contents of the cell used to find the variables and remove dependencies
    /// </param>
    private void RemoveDependencies(string name, Formula formula)
    {
        HashSet<string> variables = (HashSet<string>)formula.GetVariables();
        foreach (string variable in variables)
        {
            dg.RemoveDependency(variable, name);
        }
    }

    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        return dg.GetDependents(name.ToUpper());
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    ///   A helper for the GetCellsToRecalculate method.
    ///   Gets all of the direct and indirect dependents of start and adds it to a list in the order 
    ///   of what must be calculated first.
    /// </summary>
    /// <param name="start"> the cell to get all of the direct and indirect dependents of</param>
    /// <param name="name"> the current cell looked at to get the direct dependents</param>
    /// <param name="visited"> a Set of already visited cells</param>
    /// <param name="changed"> a List of what cells need to be recalculated in the required order</param>
    /// <exception cref="CircularException"> thrown if a circular dependency occurs</exception>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        visited.Add(name);
        foreach (string n in GetDirectDependents(name)) // get dependents of the current cell
        {
            if (n.Equals(start))    // a circular dependency occurs
            {
                throw new CircularException();
            }
            else if (!visited.Contains(n))  // haven't visited the cell yet
            {
                Visit(start, n, visited, changed);  // repeat process for the new cell to get the direct dependents of it
            }
        }

        changed.AddFirst(name); // add the cell that must be recalculated
    }

    /// <summary>
    ///     Helper method to determine if a name is a valid cell name, i.e., that it is a valid variable.
    /// </summary>
    /// <param name="name"> the name of the cell to check if it is valid</param>
    /// <returns> true if name is a valid cell name </returns>
    private static bool IsValidName(string name)
    {
        string standaloneVarPattern = $"^{@"[a-zA-Z]+\d+"}$";
        return Regex.IsMatch(name, standaloneVarPattern);
    }

    /// <summary>
    ///     Class that represents a cell in the spreadsheet.
    ///     Stores the contents, which is of type double, string, or Formula,
    ///     the value, which is of type double, string, or FormulaError, and
    ///     the StringForm, which is the string form of the contents of the cell.
    /// </summary>
    private class Cell
    {
        /// <summary>
        ///     Stores the contents of the cell, either a double, a string, or a Formula
        /// </summary>
        public object Contents {  get; set; }

        /// <summary>
        ///     Stores the value of the cell, either a double, a string, or a FormulaError
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     Stores the StringForm of a cell, either a string, or ("=" + formula.ToString()).
        /// </summary>
        public object StringForm { get; set; }

        /// <summary>
        ///     Constructs a cell when the contents is a double.
        /// </summary>
        /// <param name="contents"></param>
        public Cell(double contents)
        {
            Contents = contents;
            Value = contents;
            StringForm = contents.ToString();
        }
        
        /// <summary>
        ///     Constructs a cell when the contents is a string.
        /// </summary>
        /// <param name="contents"></param>
        public Cell(string contents)
        {
            Contents = contents;
            Value = contents;
            StringForm = contents;
        }

        /// <summary>
        ///     Constructs a cell when the contents is a Formula.
        /// </summary>
        /// <param name="contents"></param>
        public Cell(Formula contents)
        {
            Contents = contents;
            Value = new FormulaError("");
            StringForm = "=" + contents.ToString();
        }

    }
}
