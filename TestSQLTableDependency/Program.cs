using System;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace TestSQLTableDependency
{
    /*
    * - install SQLTableDependency Nuget
    * - Create Customer Table in sql server
    * - if u get permission error u have to change your sql server to own that database manually in SQL Server -> Security -> your user login -> Property -> User Mapping or use sql command "EXEC sp_changedbowner 'sa', 'true'" to your database
    */
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Data Source=DESKTOP-H8VJ0NI;Database=MVCAPITest;Persist Security Info=false;Integrated Security=false;User Id=sa;Password=123";

            using (var tableDependency = new SqlTableDependency<Customer>(connectionString, "Customers"))//Customers = Customers Table In SQL Server
            {
                tableDependency.OnChanged += TableDependency_Changed;
                tableDependency.OnError += TableDependency_OnError;

                tableDependency.Start();
                Console.WriteLine("Wait for receiving notification...");
                Console.WriteLine("Press a key to stop");
                Console.ReadKey();
                tableDependency.Stop();
            }
        }

        static void TableDependency_Changed(object sender, RecordChangedEventArgs<Customer> e)
        {
            Console.WriteLine(Environment.NewLine);
            if(e.ChangeType != ChangeType.None)
            {
                var changedEntity = e.Entity;
                Console.WriteLine("DML Operation : "+e.ChangeType);
                Console.WriteLine("ID : "+changedEntity.Id);
                Console.WriteLine("Name : " + changedEntity.Name);
                Console.WriteLine("Surname : " + changedEntity.Surname);
                Console.WriteLine(Environment.NewLine);
            }
        }

        static void TableDependency_OnError(object sender, ErrorEventArgs e)
        {

        }
    }

    class Customer
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Surname { set; get; }
        public bool isActive { set; get; }
    }
}
