using System;
using System.Data.SqlClient;
using System.IO;

namespace SQL
{
    class Program
    {
        static void Main(string[] args)
        {
            string sqlServer = string.Empty;
            string database = "master";  // 默认连接到 master 数据库
            bool showHelp = false;  // 是否显示帮助信息
            string filePath = string.Empty; // 文件路径，用于批量执行 SQL 命令

            // 解析命令行参数
            foreach (var arg in args)
            {
                if (arg.StartsWith("--server"))
                {
                    sqlServer = arg.Split('=')[1];
                }
                else if (arg.Equals("-h") || arg.Equals("--help"))
                {
                    showHelp = true;
                }
                else if (arg.StartsWith("--noshell-file"))
                {
                    filePath = arg.Split('=')[1]; // 获取文件路径
                }
            }

            // 如果需要显示帮助信息，则输出帮助说明并退出
            if (showHelp)
            {
                ShowHelp();
                return;
            }

            if (string.IsNullOrEmpty(sqlServer))
            {
                Console.WriteLine("Usage: mssql.exe --server <sqlServerAddress> --noshell-file <filePath>");
                Environment.Exit(0);
            }

            string conString = $"Server=tcp:{sqlServer},1433; Database={database}; Integrated Security=True;";

            SqlConnection con = new SqlConnection(conString);

            try
            {
                // 尝试连接数据库
                con.Open();
                Console.WriteLine("Auth success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Auth failed: " + ex.Message);
                Environment.Exit(0);
            }

            // 执行权限查询
            ExecuteQuery(con, "SELECT SYSTEM_USER;", "Logged in as");
            ExecuteQuery(con, "SELECT USER_NAME();", "Mapped to the user");
            ExecuteQuery(con, "SELECT IS_SRVROLEMEMBER('public');", "User is a member of public role");
            ExecuteQuery(con, "SELECT IS_SRVROLEMEMBER('sysadmin');", "User is a member of sysadmin role");

            // 如果指定了文件路径，执行文件中的 SQL 查询
            if (!string.IsNullOrEmpty(filePath))
            {
                ExecuteCommandsFromFile(con, filePath);
            }
            else
            {
                // 启动交互式查询模式
                InteractiveQuery(con);
            }

            // 关闭连接
            con.Close();
            Console.WriteLine("Connection closed.");
        }

        // 执行 SQL 查询并输出结果
        static void ExecuteQuery(SqlConnection con, string query, string resultMessage)
        {
            try
            {
                SqlCommand command = new SqlCommand(query, con);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine(resultMessage + ":");
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader[i].ToString() + "\t");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine(resultMessage + ": No rows returned.");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
            }
        }

        // 从文件中读取 SQL 命令并逐行执行
        static void ExecuteCommandsFromFile(SqlConnection con, string filePath)
        {
            try
            {
                // 读取文件中的所有命令
                string[] commands = File.ReadAllLines(filePath);
                foreach (var command in commands)
                {
                    if (!string.IsNullOrWhiteSpace(command))
                    {
                        Console.WriteLine($"Executing command: {command}");
                        ExecuteQuery(con, command, "Query Result");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file or executing commands: {ex.Message}");
            }
        }

        // 启动交互式查询模式
        static void InteractiveQuery(SqlConnection con)
        {
            string userQuery;
            while (true)
            {
                Console.WriteLine("\nsuse like mssqlclient(Enter 'exit' quit)：");
                userQuery = Console.ReadLine();

                // 检查用户输入是否为空
                if (string.IsNullOrWhiteSpace(userQuery))
                {
                    Console.WriteLine("The query statement cannot be empty");
                    continue;
                }

                if (userQuery.ToLower() == "exit")
                {
                    break;
                }

                ExecuteQuery(con, userQuery, "Query Result");
            }
        }

        // 显示帮助信息
        static void ShowHelp()
        {
            Console.WriteLine("Usage: mssql.exe --server <sqlServerAddress> --noshell-file <filePath>");
            Console.WriteLine("Options:");
            Console.WriteLine("  --server=<serverAddress>    Target SQL server address");
            Console.WriteLine("  --noshell-file=<filePath>   Path to the file containing SQL commands to execute");
            Console.WriteLine("  -h, --help                  Display this help message");
            Console.WriteLine("Example:");
            Console.WriteLine("  mssql.exe --server 127.0.0.1 --noshell-file test.txt");
        }
    }
}
