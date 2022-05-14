using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;


namespace spargo_test
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
            IConfigurationRoot root = builder.Build();
            //=================Строка подключения к базе в файле appsettings.json===============    
            Spargo.connect = root["connect"];
            //=================Строка подключения к базе в файле appsettings.json===============
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
                        throw new Exception("Недопустимое имя объекта");
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
                        throw new Exception("Недопустимое имя объекта");
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

            Console.WriteLine(@"Команды:

Вывод на печать таблиц:
select [article/pharmacy/stock/batch/v_batch/v_article_total]

Добавление:            
insert article [ar_name]
insert pharmacy [ph_name] [ph_address] [ph_phone]
insert stock [st_ph] [st_name]
insert batch [bt_ar] [bt_st] [bt_num]

Удаление:
delete article [ar_id]
delete pharmacy [ph_id]
delete stock [st_id]
delete batch [bt_id]

Товары в аптеке:
total article [ph_id]

Описание полей:
[ph_id], [st_ph] - id аптеки (pharmacy)
[ar_id], [bt_ar] - id товара (article)
[st_id], [bt_st] - id склада (stock)
[bt_id] - id партии  (batch)");

            return;
        }
    }
}
