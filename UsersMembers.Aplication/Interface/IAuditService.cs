namespace UsersMembers.Application.Interface
{
    public interface IAuditService
    {
        Task LogAsync(
            string userId,
            string action,
            string entityName,
            string entityId,
            string? dataBefore,
            string? dataAfter,
            string? ipAddress = null,
            CancellationToken cancellationToken = default);

        Task<TResult> ExecuteWithAuditAsync<TResult>(
            string action,
            string entityName,
            string entityId,
            Func<Task<TResult>> func,
            string? ipAddress = null,
            CancellationToken cancellationToken = default);
    }
}
