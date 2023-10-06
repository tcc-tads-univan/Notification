using System.Data;

namespace Notification.Api.Database.Interfaces
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnection();
    }
}
