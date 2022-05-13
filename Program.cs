using System;
using System.Data;
using System.Collections.Generic;

namespace spargo_test
{
    class Program
    {
        static void Main(string[] args)
        {
            //=================Строка подключения к базе в коде, нужно изменить================    
            Spargo.connect = @"data source=localhost\SQLEXPRESS8;User ID=sa;Password=aA12345678;database=spargo_test";
            //=================Строка подключения к базе в коде, нужно изменить================
            if (args.Length < 2)
            {
                help();
                return;
            }
            try
            {
                List<string> views = new List<string>() { "article", "pharmacy", "stock", "batch", "v_batch", "v_article_total" };
                Dictionary<string, List<string>> procedures = new Dictionary<string, List<string>>()
            {
                {"p_article_add", new List<string>{"@ar_name"}},
                {"p_pharmacy_add", new List<string>{"@ph_name", "@ph_address", "@ph_phone"}},
                {"p_stock_add", new List<string>{"@st_ph","@st_name"}},
                {"p_batch_add", new List<string>{"@bt_ar", "@bt_st", "@bt_num"}},

                {"p_article_del", new List<string>{"@ar_id"}},
                {"p_pharmacy_del", new List<string>{"@ph_id"}},
                {"p_stock_del", new List<string>{"@st_id"}},
                {"p_batch_del", new List<string>{"@bt_id"}},

                {"p_total_articles", new List<string>{"@ph_id"}}
            };

                Dictionary<string, object> prepare_param(string proc_name)
                {
                    if (!procedures.ContainsKey(proc_name))
                    {
                        throw new Exception("Недоустимое имя объекта");
                    }
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    int i = 2;
                    foreach (var p in procedures[proc_name])
                    {
                        if (i < args.Length)
                            result.Add(p, args[i++]);
                        else
                            result.Add(p, DBNull.Value);
                    }
                    return result;
                };

                string command = args[0];
                Spargo spargo = new Spargo();
                if (command == "insert")
                {
                    string p_name = $"p_{args[1]}_add";
                    Dictionary<string, object> param = prepare_param(p_name);
                    spargo.exec_proc(p_name, param);
                    return;
                }

                if (command == "delete")
                {
                    string p_name = $"p_{args[1]}_del";
                    Dictionary<string, object> param = prepare_param(p_name);
                    spargo.exec_proc(p_name, param);
                    return;
                }

                if (command == "total")
                {
                    string p_name = "p_total_articles";
                    Dictionary<string, object> param = prepare_param(p_name);
                    spargo.exec_proc(p_name, param);
                    return;
                }




                if (command == "select")
                {
                    string tablename = args[1];
                    if (views.IndexOf(tablename) == -1)
                    {
                        Console.WriteLine("Ошибка: Недоустимое имя объекта");
                        return;
                    }
                    spargo.select(tablename);
                    return;
                }

                throw new Exception("Неизвестная комманда");

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        static void help()
        {
            Console.WriteLine("Команды:");
            Console.WriteLine("select [article/pharmacy/stock/batch/v_batch/v_article_total]");
            Console.WriteLine("");
            Console.WriteLine("insert article [ar_name]");
            Console.WriteLine("insert pharmacy [ph_name] [ph_address] [ph_phone]");
            Console.WriteLine("insert stock [st_ph] [st_name]");
            Console.WriteLine("insert batch [bt_ar] [bt_st] [bt_num]");

            Console.WriteLine("delete article [ar_id]");
            Console.WriteLine("delete pharmacy [ph_id]");
            Console.WriteLine("delete stock [st_id]");
            Console.WriteLine("delete batch [bt_id]");

            Console.WriteLine("");
            Console.WriteLine("total articles [ph_id]");
            Console.WriteLine("");
            Console.WriteLine("Описание полей:");
            Console.WriteLine("[ph_id], [st_ph] - id аптеки (pharmacy)");
            Console.WriteLine("[ar_id], [bt_ar] - id товара (article)");
            Console.WriteLine("[st_id], [bt_st] - id склада (stock)");
            Console.WriteLine("[bt_id] - id партии  (batch)");
            return;
        }
    }
}
