using System.Collections.Generic;
using System.Data;
using System.IO;

namespace K1.Libs.Utils.ExcelHelper
{
    public interface IExcelHelper
    {
        DataSet Read(FileStream excelFile);
        IEnumerable<TModel> Read<TModel>(FileStream excelFile);
    }
}
