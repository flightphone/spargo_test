using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace spargo_test
{
    public class Spargo
    {
        public static string connect = "";

        public void select(string tablename)
        {
            string sql = $"select * from {tablename}";
            SqlDataAdapter da = new SqlDataAdapter(sql, Spargo.connect);
            DataTable resultTable = new DataTable();
            da.Fill(resultTable);
            print(resultTable);
        }

        public void exec_proc(string procname, Dictionary<string, object> par)
        {
            SqlDataAdapter da = new SqlDataAdapter(procname, Spargo.connect);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            foreach (var p in par)
                da.SelectCommand.Parameters.AddWithValue(p.Key, p.Value);
            DataTable resultTable = new DataTable();
            da.Fill(resultTable);
            if (resultTable.Columns.Count > 0)
                print(resultTable);
            else
                Console.WriteLine("successful");
        }
        public void print(DataTable table)
        {
            DataColumn[] temp = new DataColumn[table.Columns.Count];
            table.Columns.CopyTo(temp, 0);
            string print_string = string.Join(" | ", temp.Select((a) => a.Caption));
            Console.WriteLine(print_string);
            foreach (DataRow data in table.Rows)
            {
                print_string = string.Join(" | ", temp.Select((a) => data[a].ToString()));
                Console.WriteLine(print_string);
            }
        }
    }
}