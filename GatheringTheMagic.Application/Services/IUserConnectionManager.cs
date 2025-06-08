namespace GatheringTheMagic.Application.Services;

public interface IUserConnectionManager
{
    void AddUser(string userName, string connectionId);
    void RemoveConnection(string connectionId);
    string GetUserByConnectionId(string connectionId);
    string GetConnectionId(string userName);
    IReadOnlyList<string> GetAllUsers();
}
