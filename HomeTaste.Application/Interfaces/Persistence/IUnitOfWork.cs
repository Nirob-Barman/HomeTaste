namespace HomeTaste.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransaction();
        Task CommitAsync();
        Task RollbackAsync();


        // Additional methods for transaction management
        bool IsTransactionActive();
        IEnumerable<string> GetActiveSavepoints();
        void ClearSavepoints();
        bool HasNestedTransactions();
        object GetTransactionDetails();
        Task BeginTransactionWithTimeout(TimeSpan timeout);
        string GetTransactionQueryLogs();


        // Nested transaction management
        Task BeginNestedTransaction(string savepointName = null!);
        Task RollbackToSavepointAsync(string savepointName = null!);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null);
        Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null);


        //Dapper
        Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
        Task ExecuteAsync(string sql, object? parameters = null, bool useTransaction = false);
    }
}
