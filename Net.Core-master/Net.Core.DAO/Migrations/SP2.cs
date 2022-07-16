namespace Net.Core.DAO.Migrations
{
    using FluentMigrator;

    [Migration(202207161801)]
    public class SP2 : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
          
            Execute.Sql(@"

CREATE PROCEDURE sp_GetCustomerByID(IN parm_ID INT)
BEGIN
    select * from Customers where ID=parm_ID;
END 
    

");
        }

    }
}
