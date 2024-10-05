namespace Laundro.Core.Authentication;
public class UserAccountState
{
    public enum UserStateInDb
    {
        NoRecord = 0,
        Inactive = 1,
        Active = 2
    }

    public UserStateInDb DbState { get; set; } = UserStateInDb.NoRecord;

    public UserContext? UserContext { get; set; }
}
