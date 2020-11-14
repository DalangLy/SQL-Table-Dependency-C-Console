using System;
using System.Linq.Expressions;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Where;

namespace TestSQLTableDependency
{
    /*
    * - install SQLTableDependency Nuget
    * - Create Customer Table in sql server
    *   CREATE TABLE [dbo].[Customers](
	*       [Id] [int] IDENTITY(1,1) NOT NULL,
	*       [Name] [nvarchar](50) NULL,
	*       [Surname] [nvarchar](50) NULL,
	*       [Expired] [bit] NULL
    *   ) ON [PRIMARY]
    *   GO
    *   
    *   
    * - if u get permission error u have to change your sql server to own that database manually in SQL Server -> Security -> your user login -> Property -> User Mapping or use sql command "EXEC sp_changedbowner 'sa', 'true'" to your database
    */
    class Program
    {
        static void Main(string[] args)
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; //if you are using asp.net framework web api
            var connectionString = @"Data Source=DESKTOP-H8VJ0NI;Database=MVCAPITest;Persist Security Info=false;Integrated Security=false;User Id=sa;Password=123";

            //this is optional u can remove this: this use to map class attributes with sql table attribute when both are not the same  name
            var mapper = new ModelToTableMapper<Customer>();
            mapper.AddMapping(c => c.Id, "Id");
            mapper.AddMapping(c => c.Name, "Name");
            mapper.AddMapping(c => c.Surname, "Surname");
            mapper.AddMapping(c => c.Expired, "Expired");

            Expression<Func<Customer, bool>> expression = p => p.Expired == false;//using linq expression to only monitor table row with value in "Expired" column = false;
            ITableDependencyFilter whereCondition = new SqlTableDependencyFilter<Customer>(expression);

            var columnsToTrackChanging = new UpdateOfModel<Customer>();
            columnsToTrackChanging.Add(c => c.Name);//monitor only changing of "Name" Column

            using (var tableDependency = new SqlTableDependency<Customer>(connectionString, "Customers", updateOf: columnsToTrackChanging, mapper: mapper, notifyOn: DmlTriggerType.Update, filter: whereCondition))//Customers = Customers Table In SQL Server
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
            if(e.ChangeType == ChangeType.Update)
            {
                var changedEntity = e.Entity;
                Console.WriteLine("DML Operation : "+e.ChangeType);
                Console.WriteLine("ID : "+changedEntity.Id);
                Console.WriteLine("Name : " + changedEntity.Name);
                Console.WriteLine("Surname : " + changedEntity.Surname);
                Console.WriteLine("Expired : "+ changedEntity.Expired);
                Console.WriteLine(Environment.NewLine);
            }
            if(e.ChangeType == ChangeType.Insert)
            {
                Console.WriteLine("Inserted");
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
        public bool Expired { set; get; }
    }
}
