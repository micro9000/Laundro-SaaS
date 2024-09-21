using DbUp.Engine;
using Laundro.Core.Constants;
using Laundro.Core.Models;
using System.Data;
using System.Linq;

namespace Laundro.DatabaseMigration.Mock.Common;
public class Mock0002_RoleAssignment : IScript
{
    public string ProvideScript(Func<IDbCommand> dbCommandFactory)
    {
        List<UserWithRole> userWithRoles = new List<UserWithRole>
        {
            new UserWithRole("ranielgarcia101@gmail.com", [Roles.store_owner_admin])
        };

        var emails = userWithRoles.Select(u => u.email).ToList();
        var roleNames = userWithRoles.SelectMany(u => u.roles.Select(r => r.ToString())).ToList();

        var dbUsers = GetUsers(emails, dbCommandFactory);
        var dbRoles = GetRoles(roleNames, dbCommandFactory);

        foreach(var user in userWithRoles)
        {
            var userRoles = user.roles.Select(r => r.ToString()).ToList();
            var userDb = dbUsers.FirstOrDefault(u => u.Email == user.email);
            var rolesDb = dbRoles.Where(r => userRoles.Contains(r.SystemKey)).ToList();

            if (!userDb.IsActive)
            {
                using (var cmd = dbCommandFactory())
                {
                    cmd.CommandText = $"UPDATE Users SET IsActive=1 WHERE Id={userDb.Id}";
                    cmd.ExecuteNonQuery();
                }
            }

            if (userDb is not null && rolesDb.Any())
            {
                using (var cmd = dbCommandFactory())
                {
                    var insertValues = rolesDb.Select(r => $"({userDb?.Id}, {r.Id})");
                    cmd.CommandText = $@"
MERGE UserRoles AS [Target]
USING (VALUES {string.Join(",", insertValues)}) AS Seed (UserId, RoleId)
    ON Target.UserId = Seed.UserId AND Target.RoleId = Seed.RoleId
WHEN NOT MATCHED THEN
    INSERT (UserId, RoleId) VALUES (Seed.UserId, Seed.RoleId)
WHEN MATCHED THEN
    UPDATE SET IsActive=1;
";

                    cmd.ExecuteNonQuery();
                }
            }

            
        }

        return string.Empty;
    }

    private IEnumerable<Role> GetRoles(IEnumerable<string> roleNames, Func<IDbCommand> dbCommandFactory)
    {
        var roles = new List<Role>();
        using (var cmd = dbCommandFactory())
        {
            var queryParams = roleNames.Select(r => $"'{r}'");
            cmd.CommandText = $"SELECT Id, SystemKey, IsActive FROM Roles WHERE IsActive=1 AND SystemKey IN ({string.Join(",", queryParams)})";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(reader.GetOrdinal("Id"));
                    var systemKey = reader.GetString(reader.GetOrdinal("SystemKey"));
                    var isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                    roles.Add(new Role { SystemKey = systemKey, Id = id, IsActive = isActive });
                }
            }
        }
        return roles;
    }

    private IEnumerable<User> GetUsers(IEnumerable<string> emails, Func<IDbCommand> dbCommandFactory)
    {
        var users = new List<User>();
        using (var cmd = dbCommandFactory())
        {
            var queryParams = emails.Select(e => $"'{e}'");
            cmd.CommandText = $"SELECT Id, Email, IsActive FROM Users WHERE Email IN ({string.Join(",", queryParams)})";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(reader.GetOrdinal("Id"));
                    var email = reader.GetString(reader.GetOrdinal("Email"));
                    var isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                    users.Add(new User { Email = email, Id = id, IsActive = isActive });
                }
            }
        }
        return users;
    }


    public record UserWithRole(string email, Roles[] roles);
}
