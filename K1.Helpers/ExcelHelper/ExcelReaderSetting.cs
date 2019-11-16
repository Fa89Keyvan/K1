namespace K1.Libs.Utils.ExcelHelper
{
    public class ExcelReaderSetting
    {
        public string Password { get; set; }

        /// <summary>
        /// Default: , ; TAB | # 
        /// </summary>
        public char[] CsvSeparators { get; set; }

        public bool? UseColumnDataType { get; set; }

        /// <summary>
        /// select sheets by its names to read
        /// by defalut read all sheets
        /// </summary>
        public string[] SheetsNames { get; set; }

        /// <summary>
        /// select sheets by its index to read
        /// by defalut read all sheets
        /// </summary>
        public int[] SheetsIndexes { get; set; }

    }
}
