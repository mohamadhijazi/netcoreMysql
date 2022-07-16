namespace Net.Core.DAO.Migrations
{
    using FluentMigrator;

    [Migration(202207161800)]
    public class SP1 : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {
            Execute.Sql(@"
CREATE TABLE `Customers` ( 
`ID` INT NOT NULL AUTO_INCREMENT,
  `Surname` VARCHAR(256) NOT NULL,
  
  `Name` VARCHAR(256) NOT NULL,
  PRIMARY KEY (  `ID`));

CREATE TABLE `Account` ( 
`ID` INT NOT NULL AUTO_INCREMENT,
  `CustomerID` INT NOT NULL,
  
  `Balance` Float NULL,
  PRIMARY KEY (  `ID`));

CREATE TABLE `Transaction` ( 
`ID` INT NOT NULL AUTO_INCREMENT,
  `AccountID` INT NOT NULL,
  
  `Amount` Float NULL,
  PRIMARY KEY (  `ID`));
");

            Execute.Sql(@"

CREATE PROCEDURE sp_GetCustomers()
BEGIN
    select * from Customers;
END 
    

");
         
        }

    }
}
