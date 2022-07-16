namespace Net.Core.DAO.Migrations
{
    using FluentMigrator;

    [Migration(202207162310)]
    public class SPCustomerbalance : Migration
    {
        public override void Down()
        {

        }

        public override void Up()
        {

            Execute.Sql(@"
            CREATE PROCEDURE sp_GetCustomerBalanceByID(IN parm_ID INT)
            BEGIN
            SELECT customers.ID, Name, Surname, Sum(transaction.Amount) as balance from customers
            INNER join account on account.CustomerID=customers.ID
            INNER join transaction on transaction.AccountID=account.ID
            where customers.ID=parm_ID;

            END


            ");
            Execute.Sql(@"


            CREATE PROCEDURE sp_GetCustomerTransactionsByID(IN parm_ID INT)
            BEGIN
            SELECT `transaction`.ID as ID, `transaction`.AccountID as AccountID, `transaction`.Amount as Amount from customers
            INNER join account on account.CustomerID=customers.ID
            INNER join transaction on transaction.AccountID=account.ID
            where customers.ID=parm_ID;

            END


            ");
            Execute.Sql(@"


CREATE PROCEDURE sp_CreateAccount(IN parm_ID INT)
BEGIN
INSERT INTO account (customerID) VALUES (parm_ID);
SELECT LAST_INSERT_ID();
END

");
            Execute.Sql(@"

CREATE PROCEDURE sp_Transaction(IN parm_ID INT,IN parm_Amount float)
BEGIN
INSERT INTO transaction(AccountID,Amount) VALUES (parm_ID,parm_Amount);
SELECT LAST_INSERT_ID();
END 

");
        }

    }
}
