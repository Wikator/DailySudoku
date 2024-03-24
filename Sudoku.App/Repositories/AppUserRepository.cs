using System.Security.Cryptography;
using System.Text;
using Neo4j.Driver;
using Sudoku.App.Models;
using Sudoku.App.Models.FormModels;
using Sudoku.App.Repositories.Contracts;
using Sudoku.App.Services.Contracts;

namespace Sudoku.App.Repositories;

public class AppUserRepository(INeo4JDataAccess dataAccess) : IAppUserRepository
{
    private INeo4JDataAccess DataAccess { get; } = dataAccess;
    
    public async Task<CurrentUser> RegisterAsync(RegisterModel registerModel)
    {
        using var hmac = new HMACSHA512();
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerModel.Password));
        var passwordSalt = hmac.Key;

        // language=Cypher
        const string query = """
                             CREATE (u:User {
                                 id: $id,
                                 email: $email,
                                 userName: $userName,
                                 passwordHash: $passwordHash,
                                 passwordSalt: $passwordSalt
                             })
                             RETURN
                                u.id as Id,
                                u.email as Email,
                                u.userName as UserName
                             """;
        
        var parameters = new
        {
            id = Guid.NewGuid().ToString(),
            email = registerModel.Email,
            userName = registerModel.UserName,
            passwordHash = Convert.ToBase64String(passwordHash),
            passwordSalt = Convert.ToBase64String(passwordSalt)
        };
        
        var record = await DataAccess.ExecuteWriteSingleAsync(query, parameters);
        return new CurrentUser
        {
            Id = Guid.Parse(record["Id"].As<string>()),
            Email = record["Email"].As<string>(),
            UserName = record["UserName"].As<string>()
        };
    }

    public async Task<CurrentUser?> LoginAsync(LoginModel loginModel)
    {
        try
        {
            // language=Cypher
            const string query = """
                                 MATCH (u:User)
                                 WHERE u.email = $email
                                 RETURN
                                    u.id as Id,
                                    u.userName as UserName,
                                    u.passwordHash as PasswordHash,
                                    u.passwordSalt as PasswordSalt
                                 """;
        
            var parameters = new { email = loginModel.Email };
            var record = await DataAccess.ExecuteReadSingleAsync(query, parameters);
            
            var passwordHash = Convert.FromBase64String(record["PasswordHash"].As<string>());
            var passwordSalt = Convert.FromBase64String(record["PasswordSalt"].As<string>());
            
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginModel.Password));
            
            if (!computedHash.SequenceEqual(passwordHash))
                return null;
            
            return new CurrentUser
            {
                Id = Guid.Parse(record["Id"].As<string>()),
                Email = loginModel.Email,
                UserName = record["UserName"].As<string>()
            };
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        // language=Cypher
        const string query = "MATCH (u:User) WHERE u.email = $email RETURN count(u) > 0 as Exists";
        var parameters = new { email };
        
        var record = await DataAccess.ExecuteReadSingleAsync(query, parameters);
        return record["Exists"].As<bool>();
    }

    public async Task<bool> UserNameExistsAsync(string userName)
    {
        // language=Cypher
        const string query = "MATCH (u:User) WHERE u.userName = $userName RETURN count(u) > 0 as Exists";
        var parameters = new { userName };
        
        var record = await DataAccess.ExecuteReadSingleAsync(query, parameters);
        return record["Exists"].As<bool>();
    }
}