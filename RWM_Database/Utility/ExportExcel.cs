using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Utility
{
    public class ExportExcel
    {
        public static Stylesheet CreateStyleSheet()
        {
            var stylesheet = new Stylesheet();

            // Default Font
            var fonts = new Fonts() { Count = 1, KnownFonts = BooleanValue.FromBoolean(true) };
            var font = new Font
            {
                FontSize = new FontSize() { Val = 13 },
                FontName = new FontName() { Val = "Calibri" },
                FontFamilyNumbering = new FontFamilyNumbering() { Val = 2 },
                FontScheme = new FontScheme() { Val = new EnumValue<FontSchemeValues>(FontSchemeValues.Minor) }
            };
            fonts.Append(font);
            stylesheet.Append(fonts);

            // Default Fill
            var fills = new Fills() { Count = 1 };
            var fill = new Fill();
            fill.PatternFill = new PatternFill() { PatternType = new EnumValue<PatternValues>(PatternValues.None) };
            fills.Append(fill);
            stylesheet.Append(fills);

            // Default Border
            var borders = new Borders() { Count = 1 };
            var border = new Border
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);
            stylesheet.Append(borders);

            // Default cell format and a date cell format
            var cellFormats = new CellFormats() { Count = 2 };
            var cellFormatDefault = new CellFormat { NumberFormatId = 0, FormatId = 0, FontId = 0, BorderId = 0, FillId = 0 };
            cellFormats.Append(cellFormatDefault);

            var cellFormatDate = new CellFormat { NumberFormatId = 22, FormatId = 0, FontId = 0, BorderId = 0, FillId = 0, ApplyNumberFormat = BooleanValue.FromBoolean(true) };
            cellFormats.Append(cellFormatDate);

            stylesheet.Append(cellFormats);

            return stylesheet;
        }

        public static void ExportTable(DataTable table, string destination, List<string> omitColumns)
        {
            using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();

                    WorkbookStylesPart stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = CreateStyleSheet();
                    stylesPart.Stylesheet.Save();

                var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();

               
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);
                
    

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                  
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                

                sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    foreach (System.Data.DataColumn column in table.Columns)
                    {
                    if (omitColumns.Contains(column.ColumnName)) continue;
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }


                    sheetData.AppendChild(headerRow);

                    foreach (System.Data.DataRow dsrow in table.Rows)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                             if (omitColumns.Contains(col)) continue;
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString()); //
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                    workbookPart.Workbook.Save();

            }
        }
    }
}
