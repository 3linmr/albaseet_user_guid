using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ExcelDataReader;

namespace Shared.Helper.Logic
{
    public static class ExcelHelpers
    {
        public static DataTable ReadCsvFile(String path)
        {
            DataTable dtCsv = new DataTable();
            string Fulltext;

            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                    string[] rows = Fulltext.Split('\n'); //split full file text into rows  
                    for (int i = 0; i < rows.Count() - 1; i++)
                    {
                        string[] rowValues = rows[i].Split(';'); //split each row with comma to get individual values  
                        {
                            if (i == 0)
                            {
                                for (int j = 0; j < rowValues.Count(); j++)
                                {
                                    dtCsv.Columns.Add(rowValues[j]); //add headers  
                                }
                            }
                            else
                            {
                                DataRow dr = dtCsv.NewRow();
                                for (int k = 0; k < rowValues.Count(); k++)
                                {
                                    dr[k] = rowValues[k].ToString();
                                }
                                dtCsv.Rows.Add(dr); //add other rows  
                            }
                        }
                    }
                }
            }
            return dtCsv;
        }

        //public static DataTable ReadExcel(string fileName)
        //{
        //    using var stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
        //    using var reader = ExcelReaderFactory.CreateReader(stream);
        //    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
        //    {
        //        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
        //        {
        //            UseHeaderRow = true
        //        }
        //    });
        //    return result.Tables[0];
        //}
    }
}
