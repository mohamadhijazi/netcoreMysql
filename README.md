# netcoreMysql
.net core Mysql Dapper FluentMigrator

-First Create database using Mysql (Test using 10.4.22-MariaDB)
-Update the connection string user, password in appsettings.json
-start the .net core web application
home page will open by default listing current Customers (2 created on startup)
in top navigation we can click on second link to open the form that allow creation on account by customer ID "[Create Account]"

-http://localhost:52126/swagger/index.html swagger link cusromer POST
-in homepage click on [Details] will open customer information and transactions, 
-in details page click on [Create Account For this Customer] to creata account and transaction for current row user
