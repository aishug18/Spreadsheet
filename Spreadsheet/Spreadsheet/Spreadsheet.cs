// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
//     - Updated return types
//     - Updated documentation

// <author> Aishwarya Golakoti </author>
// <date> 18 September 2024 </date>
// For CS 3500
namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

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
    private DependencyGraph dg = new();

    /// <summary>
    ///     Keeps track of non-empty cells.
    ///     Maps cell names to Cell objects.
    /// </summary>
    private Dictionary<string, Cell> cells = new();

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
        foreach (var cell in cells)
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
        if (!IsValidName(name)) // name is invalid
            throw new InvalidNameException();
        
        if(cells.ContainsKey(name.ToUpper()))
            return cells[name.ToUpper()].Contents;
        else
            return string.Empty;
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
    public IList<string> SetCellContents(string name, double number)
    {
        if (!IsValidName(name)) // name is invalid
            throw new InvalidNameException();

        if (cells.TryGetValue(name, out Cell? cell)) // if cell already exists and has a value
        {
            // TWO OPTIONS

            // option 1:
            //cells[name.ToUpper()] = new Cell(number);

            // option 2 (preferred):
            cell.Contents = number;
        }
        else    // cell doesn't have any value
        {
            cells.Add(name.ToUpper(), new Cell(number));
        }

        return GetCellsToRecalculate(name.ToUpper()).ToList();
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
    public IList<string> SetCellContents(string name, string text)
    {
        if (!IsValidName(name)) // name is invalid
            throw new InvalidNameException();

        // worry about empty strings here to delete the cell

        if (text == string.Empty)
        {

        }

        if (cells.TryGetValue(name, out Cell? cell)) // if cell already exists and has a value
        {
            // TWO OPTIONS

            // option 1:
            cells[name.ToUpper()] = new Cell(text);

            // option 2:
            cell.Contents = text;
        }
        else    // cell doesn't have any value
        {
            cells.Add(name.ToUpper(), new Cell(text));
        }

        return GetCellsToRecalculate(name.ToUpper()).ToList();

        throw new NotImplementedException();
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
    public IList<string> SetCellContents(string name, Formula formula)
    {
        if (!IsValidName(name)) // name is invalid
            throw new InvalidNameException();

        // create a new cell
        //Cell cell = new(name, 3.0);


        // update dependency graph with new dependencies if they exist
        HashSet<string> variables = (HashSet<string>)formula.GetVariables();
        foreach (string variable in variables)
        {
            dg.AddDependency(variable, name.ToUpper());
        }

        return GetCellsToRecalculate(name.ToUpper()).ToList();
        throw new NotImplementedException();
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
    ///   FIXME: You should fully comment what is going on below using XML tags as appropriate,
    ///   as well as inline comments in the code.
    /// </summary>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        visited.Add(name);
        foreach (string n in GetDirectDependents(name))
        {
            if (n.Equals(start))
            {
                throw new CircularException();
            }
            else if (!visited.Contains(n))
            {
                Visit(start, n, visited, changed);
            }
        }

        changed.AddFirst(name);
    }

    /// <summary>
    ///     figure out if a name is a valid cell name.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private static bool IsValidName(string token)
    {
        string standaloneVarPattern = $"^{@"[a-zA-Z]+\d+"}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///     Class that represents a cell in the spreadsheet.
    ///     Only contains the contents for now.
    /// </summary>
    private class Cell
    {
        public object Contents {  get; set; }

        // constructor for when the contents is a double.
        public Cell(double contents)
        {
            Contents = contents;
        }
        
        // constructor for when the contents is a string.
        public Cell(string contents)
        {
            Contents = contents;
        }

        // constructor for when the contents is a Formula.
        public Cell(Formula contents)
        {
            Contents = contents;
        }

    }
}
