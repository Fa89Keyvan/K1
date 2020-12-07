using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;
using NPOI.XSSF.UserModel;

namespace K1.Libs.Utils.ExcelHelper
{
    public class ExcelHelper
    {
        private const string DefaultSheetNameTemplate = "Sheet{0}";
        //static ExcelHelper() => Encoding.RegisterProvider(EncodingProvider.Instance);

        public DataSet Read(FileStream excelFileStream, ExcelReaderSetting setting = null)
        {
            #region ' Input validations '

            if (excelFileStream == null || excelFileStream.Length <= 0)
            {
                return null;
            }

            #endregion

            ExcelReaderConfiguration excelReaderConfiguration = null;
            ExcelDataSetConfiguration excelDataSetConfiguration = null;

            if (setting != null)
            {
                excelReaderConfiguration = new ExcelReaderConfiguration();
                excelDataSetConfiguration = new ExcelDataSetConfiguration();

                if (string.IsNullOrWhiteSpace(setting.Password) == false)
                {
                    excelReaderConfiguration.Password = setting.Password;
                }

                if (setting.CsvSeparators != null && setting.CsvSeparators.Length > 0)
                {
                    excelReaderConfiguration.AutodetectSeparators = setting.CsvSeparators;
                }

                if (setting.UseColumnDataType.HasValue)
                {
                    excelDataSetConfiguration.UseColumnDataType = setting.UseColumnDataType.Value;
                }

                excelDataSetConfiguration.FilterSheet = (tableReader, sheetIndex) =>
                {
                    bool readThisSheet = true;

                    if (setting.SheetsNames != null && setting.SheetsIndexes.Length > 0)
                    {
                        readThisSheet = setting.SheetsNames.Contains(tableReader.Name);
                    }

                    if (setting.SheetsIndexes != null && setting.SheetsIndexes.Length > 0)
                    {
                        readThisSheet = setting.SheetsIndexes.Contains(sheetIndex);
                    }

                    return readThisSheet;
                };
            }

            var reader = ExcelReaderFactory.CreateReader(excelFileStream, excelReaderConfiguration);

            return reader.AsDataSet(excelDataSetConfiguration);
        }

        public void WriteToFileStream(FileStream excelFileStream, DataSet dataSet, bool useColumnCaptionsAsHeader = true)
        {
            #region ' Validation '

            if (dataSet == null || dataSet.Tables == null || dataSet.Tables.Count == 0 || excelFileStream == null)
            {
                return;
            }
            
            #endregion

            var xssfWorkBook = new XSSFWorkbook();

            for (short t = 0; t < dataSet.Tables.Count; t++)
            {
                _write(xssfWorkBook, dataSet.Tables[t], useColumnCaptionsAsHeader);
            }
        }

        public void WriteToFileStream(FileStream fileStreamToWrite,DataTable dataTable, bool useColumnCaptionsAsHeader = true)
        {
            #region ' Validation '

            if (fileStreamToWrite == null || dataTable == null || dataTable.Columns.Count == 0)
            {
                return;
            }

            #endregion

            var xssfWorkBook = new XSSFWorkbook();
            _write(xSSFWorkbookToWrite: xssfWorkBook, dataTable: dataTable, useColumnCaptionsAsHeader: useColumnCaptionsAsHeader);

            if(xssfWorkBook == null)
            {
                return;
            }

            xssfWorkBook.Write(fileStreamToWrite);
        }

        private void _write(XSSFWorkbook xSSFWorkbookToWrite, DataTable dataTable, bool useColumnCaptionsAsHeader = true)
        {

            #region ' Validation '

            if (dataTable == null || dataTable.Columns.Count == 0 || xSSFWorkbookToWrite == null)
            {
                return;
            }

            #endregion

            var xSSFWorkbook = xSSFWorkbookToWrite;
            string sheetName = string.Format(DefaultSheetNameTemplate, "1");

            if (string.IsNullOrWhiteSpace(dataTable.TableName) == false)
            {
                sheetName = dataTable.TableName;
            }

            var sheet = xSSFWorkbook.CreateSheet(sheetName);
            byte startRowIndex = 0;
            
            //Headers
            if (useColumnCaptionsAsHeader)
            {
                startRowIndex++;

                var headerRow = sheet.CreateRow(0);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var cell = headerRow.CreateCell(i);
                    cell.SetCellValue(dataTable.Columns[i].Caption);
                    
                }
            }
            
            //Rows
            if(dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                for (int r = 0; r < dataTable.Rows.Count; r++)
                {
                    var row  = sheet.CreateRow(r + startRowIndex);

                    for (short c = 0; c < dataTable.Columns.Count; c++)
                    {
                        var cell = row.CreateCell(c);
                        cell.SetCellValue(dataTable.Rows[r][c].ToString());
                    }
                }
            }

        }
    }
}
