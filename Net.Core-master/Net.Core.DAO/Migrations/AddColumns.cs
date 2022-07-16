namespace Net.Core.DAO.Migrations
{
    using FluentMigrator;

    [Migration(202207161500)]
    public class AddColumns : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Alter.Table("identityuser").AddColumn("Address").AsCustom("VARCHAR(255)").Nullable();
        }

    }
}
