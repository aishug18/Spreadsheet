// <copyright file="SpreadsheetPage.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace GUI.Client.Pages;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using CS3500.Spreadsheet;
using CS3500.Formula;

/// <summary>
/// TODO: Fill in
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

    private Spreadsheet spreadsheet = new();

    private string SelectedCell = string.Empty;
    private string CurrentContents = string.Empty;
    private string CurrentValue = string.Empty;
    private ElementReference TextArea;
    private int SelectedRow = 0;
    private int SelectedCol = 0;
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

        CurrentContents = spreadsheet.GetCellContents(SelectedCell).ToString() ?? "";
        CurrentValue = DisplayValue(spreadsheet.GetCellValue(SelectedCell));

        TextArea.FocusAsync();
        StateHasChanged();
    }

    /// <summary>
    ///     method that takes action when the contents of a cell is changed.
    /// </summary>
    /// <param name="e"></param>
    private void CellContentChanged(ChangeEventArgs e)
    {
        try
        {
            // process input
            spreadsheet.SetContentsOfCell(SelectedCell, e.Value!.ToString() ?? "");

            string displayValue = DisplayValue(spreadsheet.GetCellValue(SelectedCell));

            CellsBackingStore[SelectedRow, SelectedCol] = displayValue;

            CurrentContents = spreadsheet.GetCellContents(SelectedCell).ToString() ?? "";
            CurrentValue = displayValue;
        }
        catch (Exception)
        {
            // display error box
            ShowError = true;
        }
        finally
        {
            StateHasChanged();
        }
    }

    /// <summary>
    ///     method to return the value that should be displayed on the spreadsheet
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
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
    ///     method to hide error box
    /// </summary>
    private void HideError()
    {
        ShowError = false;
        StateHasChanged();
    }

    /// <summary>
    ///     method to change text color if input results in formula error
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string GetTextClass(string value)
    {
        // If the value is "Formula Error", return the class for red text
        if (value == "Formula Error")
        {
            return "red-text";
        }

        // Otherwise return an empty string (no special class)
        return "";
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

                // TODO: Use the loaded fileContent to replace the current spreadsheet
                spreadsheet.ReplaceSpreadsheet( fileContent );

                StateHasChanged();
            }
        }
        catch ( Exception e )
        {
            Debug.WriteLine( "an error occurred while loading the file..." + e );
        }
    }

}
