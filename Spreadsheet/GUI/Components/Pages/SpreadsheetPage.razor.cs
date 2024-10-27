// <copyright file="SpreadsheetPage.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <author> Aishwarya Golakoti </author>
// <date> 27 October 2024 </date>
// For CS 3500
//
// used ChatGPT for help mostly with syntax and GUI-related logic. These include:
//  - the async stuff to ensure A1 is first selected and the user can type things into the input box without
//      selecting anything when the page is first opened or when a new spreadsheet is loaded into the page.
//  - the error box and how to make that appear on the page.
//  - when clicking on a cell, that cell in the grid is outlined to show the user visually which cell they 
//      selected.
//  - the color picker and how to change the colors of the cells.

namespace GUI.Client.Pages;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using CS3500.Spreadsheet;
using CS3500.Formula;

/// <summary>
///     This class contains the behind-the-scenes code for the Spreadsheet GUI page. It helps in displaying
///     the correct values with the correct format in the spreadsheet page.
/// </summary>
public partial class SpreadsheetPage
{
    /// <summary>
    /// Based on your computer, you could shrink/grow this value based on performance.
    /// </summary>
    private const int ROWS = 50;

    /// <summary>
    /// Number of columns, which will be labeled A-Z.
    /// </summary>
    private const int COLS = 26;

    /// <summary>
    /// Provides an easy way to convert from an index to a letter (0 -> A)
    /// </summary>
    private char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    /// <summary>
    ///   Gets or sets the name of the file to be saved
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";

    /// <summary>
    ///   <para> Gets or sets the data for all of the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[ROWS, COLS];

    /// <summary>
    ///     Backing spreadsheet to store all of the data.
    /// </summary>
    private Spreadsheet spreadsheet = new();

    /// <summary>
    ///     The selected cell in the grid, a letter followed by numbers. Initially set to "A1" when
    ///     application is first opened.
    /// </summary>
    private string SelectedCell = "A1";

    /// <summary>
    ///     The contents of the selected cell.
    /// </summary>
    private string CurrentContents = string.Empty;
    private string CurrentContents1 = "hello";

    /// <summary>
    ///     The value of the selected cell.
    /// </summary>
    private string CurrentValue = string.Empty;

    /// <summary>
    ///     Reference to the input box where the user can type in the contents of the selected cell.
    /// </summary>
    private ElementReference TextArea;

    /// <summary>
    ///     The row component of the selected cell's coordinates, from 0 to ROWS-1.
    /// </summary>
    private int SelectedRow = 0;

    /// <summary>
    ///     The column component of the selected cell's coordinates, from 0 to COLS-1.
    /// </summary>
    private int SelectedCol = 0;

    /// <summary>
    ///     True if an invalid formula was entered (which causes an exception), false if otherwise.
    /// </summary>
    private bool ShowError = false;


    /// <summary>
    /// Handler for when a cell is clicked
    /// </summary>
    /// <param name="row">The row component of the cell's coordinates</param>
    /// <param name="col">The column component of the cell's coordinates</param>
    private void CellClicked( int row, int col )
    {
        SelectedCell = Alphabet[col] + (row + 1).ToString();
        SelectedRow = row;
        SelectedCol = col;

        CurrentContents = GetCurrentContents();
        CurrentValue = DisplayValue(spreadsheet.GetCellValue(SelectedCell));

        SelectedColor = CellColors[row, col];

        TextArea.FocusAsync();
        StateHasChanged();
    }

    /// <summary>
    ///     Updates the backing spreadsheet and all of the things that are displayed on the page when a cell's
    ///     contents are changed.
    /// </summary>
    /// <param name="e"></param>
    private void CellContentChanged(ChangeEventArgs e)
    {
        try
        {
            IList<string> cellsToUpdate = spreadsheet.SetContentsOfCell(SelectedCell, e.Value!.ToString() ?? "");
            RefreshCells(cellsToUpdate);

            string displayValue = DisplayValue(spreadsheet.GetCellValue(SelectedCell));

            CellsBackingStore[SelectedRow, SelectedCol] = displayValue;
            CurrentContents = GetCurrentContents();
            CurrentValue = displayValue;
        }
        catch (Exception)
        {
            ShowError = true;   // an invalid formula is inputted, so show the error box
        }
        finally
        {
            StateHasChanged();
        }
    }

    /// <summary>
    ///     Returns the current contents of the selected cell, ensuring that if the contents is a formula, 
    ///     then the "=" remains in the contents.
    /// </summary>
    /// <returns> the current contents of SelectedCell</returns>
    private string GetCurrentContents()
    {
        if (spreadsheet.GetCellContents(SelectedCell) is Formula)
        {
            return "=" + spreadsheet.GetCellContents(SelectedCell).ToString();
        }
        else
        {
            return spreadsheet.GetCellContents(SelectedCell).ToString() ?? "";
        }
    }

    /// <summary>
    ///     Returns the value that should be displayed on the spreadsheet page.
    /// </summary>
    /// <param name="value"> the value of the cell, which is either a string, a double, or a FormulaError</param>
    /// <returns> the value that should be displayed </returns>
    private string DisplayValue(object value)
    {
        if (value is FormulaError)
        {
            return "Formula Error";
        }
        else
        {
            return value.ToString() ?? "";
        }
    }

    /// <summary>
    ///     Update the display values of cells whose values are changed.
    ///     Essentially "refreshes" the spreadsheet to reflect the new changes.
    /// </summary>
    /// <param name="list"> the list of cells that need their values that are displayed to be updated </param>
    private void RefreshCells(IList<string> list)
    {
        foreach(string cell in list)
        {
            int row = Int32.Parse(cell.Substring(1)) - 1;
            int col = Array.IndexOf(Alphabet, cell.First());

            CellsBackingStore[row, col] = DisplayValue(spreadsheet.GetCellValue(cell));
        }
        StateHasChanged();
    }

    /// <summary>
    ///     When a new spreadsheet is loaded into the page, this method ensures that the new displayed values 
    ///     reflect the new changes and gets rid of the existing spreadsheet.
    ///     Essentially "refreshes" the spreadsheet to reflect the new changes.
    /// </summary>
    private void RefreshSpreadsheet()
    {
        for(int row = 0; row < ROWS; row++)
        {
            for(int col = 0; col < COLS; col++)
            {
                CellsBackingStore[row, col] = DisplayValue(spreadsheet.GetCellValue(Alphabet[col] + (row + 1).ToString()));
                CellColors[row, col] = spreadsheet.GetCellColor(Alphabet[col] + (row + 1).ToString());
            }
        }

        // ensures the current selected cell is A1
        SelectedCell = "A1";
        SelectedRow = 0;
        SelectedCol = 0;
        SelectedColor = CellColors[0, 0];

        CurrentContents = GetCurrentContents();
        CurrentValue = DisplayValue(spreadsheet.GetCellValue(SelectedCell));

        // focus on the Contents input box
        InvokeAsync(async () => await TextArea.FocusAsync());
    }

    /// <summary>
    /// Saves the current spreadsheet, by providing a download of a file
    /// containing the json representation of the spreadsheet.
    /// </summary>
    private async void SaveFile()
    {
        await JSRuntime.InvokeVoidAsync( "downloadFile", FileSaveName, 
            spreadsheet.JsonSpreadsheetStringForm() );
    }

    /// <summary>
    /// This method will run when the file chooser is used, for loading a file.
    /// Uploads a file containing a json representation of a spreadsheet, and 
    /// replaces the current sheet with the loaded one.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser( EventArgs args )
    {
        try
        {
            string fileContent = string.Empty;

            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
            if ( eventArgs.FileCount == 1 )
            {
                var file = eventArgs.File;
                if ( file is null )
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                // fileContent will contain the contents of the loaded file
                fileContent = await reader.ReadToEndAsync();

                spreadsheet.ReplaceSpreadsheet( fileContent );
                RefreshSpreadsheet();

                StateHasChanged();
            }
        }
        catch ( Exception e )
        {
            Debug.WriteLine( "an error occurred while loading the file..." + e );
        }
    }

}
