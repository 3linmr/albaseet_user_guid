using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper.Logic
{
    public static class DataTableHelper
    {
        public static List<T> ToCollection<T>(this DataTable dt)
        {
            List<T> lst = new System.Collections.Generic.List<T>();
            Type tClass = typeof(T);
            PropertyInfo[] pClass = tClass.GetProperties();
            List<DataColumn> dc = dt.Columns.Cast<DataColumn>().ToList();
            T cn;
            foreach (DataRow item in dt.Rows)
            {
                cn = (T)Activator.CreateInstance(tClass)!;
                foreach (PropertyInfo pc in pClass)
                {
                    try
                    {
                        DataColumn d = dc?.Find(c => c.ColumnName == pc.Name)!;
                        if (d != null)
                            pc.SetValue(cn, item[pc.Name], null);
                    }
                    catch(Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                lst.Add(cn);
            }
            return lst;
        }

        public static DataTable ListToDataTable<T>(this List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            int columnIndex = 0;
            foreach (var prop in props)
            {
                tb?.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                tb?.Columns[prop.Name]?.SetOrdinal(columnIndex);
                columnIndex++;
            }
            if (items != null)
            {
                foreach (var item in items)
                {
                    var values = new object[props.Length];
                    for (var i = 0; i < props.Length; i++)
                    {
                        values[i] = props[i].GetValue(item, null)!;
                    }
                    tb.Rows.Add(values);
                }
            }
            return tb ?? new DataTable();
        }

        public static DataTable ObjectToDataTable<T>(this T obj)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            int columnIndex = 0;
            foreach (var prop in props)
            {
                tb?.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                tb?.Columns[prop.Name]?.SetOrdinal(columnIndex);
                columnIndex++;
            }
            if (obj != null)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(obj, null)!;
                }
                tb?.Rows.Add(values);
            }
            return tb ?? new DataTable();
        }

    }
}