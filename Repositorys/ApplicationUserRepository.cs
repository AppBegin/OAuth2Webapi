using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using CryptoHelper;

namespace auth
{
    public interface IApplicationUserRepository
    {
        bool Add(ApplicationUser user);
        ApplicationUser FindByPhone(string phone);
        bool CheckPhone(string phone);
        bool CheckPassword(string phone,string password);
        bool CheckVerifycode(string phone,string code);
        bool ResetPassword(string phone,string verifycode,string password);
    }

    public class ApplicationUserRepository : IApplicationUserRepository
    {
        string connectionString;
        
        public ApplicationUserRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("sqlserver");
        }
        
        internal IDbConnection Connection
        {
            get
            {
                var connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
        }

        public bool Add(ApplicationUser user)
        {
            DateTime now = DateTime.Now;
            user.password = Crypto.HashPassword(user.password);
            user.createdAt = now;
            string sqlquery = "INSERT INTO ApplicationUser(phone,password,userName,code,createdAt)VALUES(@phone,@password,@userName,@code,@createdAt)";
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Execute(sqlquery, user);
            }
            return true;
        }

        public ApplicationUser FindByPhone(string phone)
        {
            string sqlquery = "SELECT * FROM ApplicationUser WHERE phone = @phone";
            using (IDbConnection dbConnection = Connection)
            {
                return dbConnection.Query<ApplicationUser>(sqlquery,new { phone = phone }).SingleOrDefault();
            }
        }

        public bool CheckPhone(string phone)
        {
            string sqlquery = "SELECT * FROM ApplicationUser WHERE phone = @phone";
            using (IDbConnection dbConnection = Connection)
            {
                var user = dbConnection.Query<ApplicationUser>(sqlquery,new { phone = phone }).SingleOrDefault();
                return (user.phone == phone);
            }
        } 

        public bool CheckPassword(string phone,string password)
        {
            string sqlquery = "SELECT * FROM ApplicationUser WHERE phone = @phone";
            using (IDbConnection dbConnection = Connection)
            {
                var user = dbConnection.Query<ApplicationUser>(sqlquery,new { phone = phone }).SingleOrDefault();
                return Crypto.VerifyHashedPassword(user.password,password);
            }
        } 

        public bool CheckVerifycode(string phone,string verifycode)
        {
            Random rd = new Random();
            var newcode = rd.Next(100000, 999999).ToString();
            string sqlquery = "update ApplicationUser set ifValidate = 1 , code = @newcode WHERE phone = @phone and code = @verifycode";
            using (IDbConnection dbConnection = Connection)
            {
                int rtn = dbConnection.Execute(sqlquery,new { newcode = newcode, phone = phone, verifycode=verifycode });
                return (rtn>0);
            }
        } 

        public bool ResetPassword(string phone,string verifycode,string password)
        {
            Random rd = new Random();
            var newcode = rd.Next(100000, 999999).ToString();
            password = Crypto.HashPassword(password);
            string sqlquery = "update ApplicationUser set password = @password,code = @newcode WHERE phone = @phone and code = @verifycode";
            using (IDbConnection dbConnection = Connection)
            {
                int rtn = dbConnection.Execute(sqlquery,new { password = password, newcode = newcode, phone = phone, verifycode = verifycode });
                return (rtn>0);
            }
        } 
    }
}