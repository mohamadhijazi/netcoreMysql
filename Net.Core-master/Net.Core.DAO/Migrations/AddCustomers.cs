namespace Net.Core.DAO.Migrations
{
    using FluentMigrator;

    [Migration(202207162200)]
    public class CreateCustomers : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.Sql(@"
INSERT INTO `customers`(`Surname`, `Name`) VALUES ('Hijazi','Mohammad');
INSERT INTO `customers`(`Surname`, `Name`) VALUES ('Youssef','Mike');

");

            
        }

    }
}
