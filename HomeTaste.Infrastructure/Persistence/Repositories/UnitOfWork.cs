using Dapper;
using HomeTaste.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace HomeTaste.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private IDbContextTransaction? _transaction;

        // Get the current DbContext's connection (SqlConnection for SQL Server)
        private IDbConnection Connection => _context.Database.GetDbConnection();
        private Stack<string> _savepoints = new();

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        //public IRepository<T> Repository<T>() where T : class
        //{
        //    var type = typeof(T);

        //    if (!_repositories.ContainsKey(type))
        //    {
        //        var repositoryInstance = new GenericRepository<T>(_context);
        //        _repositories.Add(type, repositoryInstance);
        //    }

        //    return (IRepository<T>)_repositories[type];
        //}

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            //if (_repositories.ContainsKey(type))
            //    return (IRepository<T>)_repositories[type];
            if (_repositories.TryGetValue(type, out var repo))
                return (IRepository<T>)repo;

            var repoInstance = new GenericRepository<T>(_context);
            _repositories[type] = repoInstance;
            return repoInstance;
        }


        // Begin a transaction
        public async Task BeginTransaction()
        {
            if (_transaction != null)
                throw new InvalidOperationException("A database transaction is already in progress. Cannot start a new one.");

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        // Commit the transaction
        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Cannot commit because no transaction has been started.");

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch (Exception)
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                _transaction.Dispose();  // Clean up
                _transaction = null!;
            }
        }

        // Rollback the transaction
        public async Task RollbackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Cannot rollback because no transaction has been started.");

            await _transaction.RollbackAsync();
            _transaction.Dispose();  // Clean up
            _transaction = null!;
        }


        // Begin a nested transaction (Savepoint)
        public async Task BeginNestedTransaction(string savepointName = null!)
        {
            if (_transaction == null)
                throw new InvalidOperationException("Cannot start a nested transaction because the main transaction has not been started.");

            // Generate a unique name for the savepoint if not provided
            savepointName ??= Guid.NewGuid().ToString();

            // Save the savepoint name
            _savepoints.Push(savepointName);

            // Create the savepoint
            await _context.Database.ExecuteSqlRawAsync($"SAVEPOINT {savepointName}");
        }

        // Rollback to a specific nested transaction (Savepoint)
        public async Task RollbackToSavepointAsync(string savepointName = null!)
        {
            if (_transaction == null)
                throw new InvalidOperationException("Cannot roll back to a savepoint because no transaction has been started.");

            // Use the latest savepoint if not provided
            savepointName ??= _savepoints.Peek();

            // Rollback to the given savepoint
            await _context.Database.ExecuteSqlRawAsync($"ROLLBACK TO SAVEPOINT {savepointName}");

            // Remove the savepoint from the stack
            _savepoints.Pop();
        }



        public bool IsTransactionActive()
        {
            return _transaction != null;
        }
        public IEnumerable<string> GetActiveSavepoints()
        {
            return _savepoints.ToList();
        }
        public void ClearSavepoints()
        {
            _savepoints.Clear();
        }
        public bool HasNestedTransactions()
        {
            return _savepoints.Count > 0;
        }

        public object GetTransactionDetails()
        {
            if (_transaction == null)
                return null!;

            //return new
            //{
            //    TransactionId = _transaction.TransactionId,
            //    StartTime = DateTime.Now // Can be replaced by a timestamp if available
            //};

            var connection = _context.Database.GetDbConnection();
            // Optional: Track when the transaction actually started (if needed)
            DateTime transactionStartTime = DateTime.Now;
            var transactionDetails = new
            {
                TransactionId = _transaction.TransactionId,
                StartTime = DateTime.Now,  // You can modify to capture a start time if needed
                transactionStartTime = DateTime.Now, // Track when the transaction actually started (if needed)
                ConnectionString = connection?.ConnectionString,  // The connection string being used
                DatabaseType = connection?.GetType().Name,  // Type of the connection, e.g., SqlConnection
                ServerVersion = connection?.ServerVersion,  // Database server version (optional)
                DatabaseName = connection?.Database,  // Database being used
                TransactionState = _transaction != null ? "Active" : "Unknown",  // Current transaction state
                Timeout = _context.Database.GetCommandTimeout(),  // Timeout for commands in the current transaction
                ConnectionState = connection?.State.ToString(), // Connection state (open/closed)                
                PendingChangesCount = _context.ChangeTracker.Entries().Count(), // Number of pending changes in the context                
            };

            return transactionDetails;
        }

        public async Task BeginTransactionWithTimeout(TimeSpan timeout)
        {
            if (_transaction != null)
                throw new InvalidOperationException("A database transaction is already in progress.");

            // Set the timeout for the current transaction
            _context.Database.SetCommandTimeout(timeout);

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public string GetTransactionQueryLogs()
        {
            // This would typically involve using a profiler or logging all SQL executed in the context.
            return _context.Database.GetDbConnection().ToString(); // Simplified for demonstration
        }





        //Dapper

        // This method will execute a Dapper query that returns a single value
        // It returns the first column of the first row from the result set returned by the query
        public async Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null)
        {
            // Execute the Dapper query and return a scalar value
            return await Connection.ExecuteScalarAsync<T>(sql, parameters);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
        {            
            // Execute Dapper query using the current transaction
            return await Connection.QueryAsync<T>(sql, parameters);
        }


        // This method will return the first result or default (null) if no records are found
        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
        {            
            // Execute Dapper query and return the first result or default (null)
            return await Connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }

        // This method will return a single result or default (null) if no records are found
        // Throws an exception if more than one record is returned
        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null)
        {            
            // Execute Dapper query and return a single result or default (null)
            return await Connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
        }

        // Execute a Dapper command (non-query)
        public async Task ExecuteAsync(string sql, object? parameters = null, bool useTransaction = false)
        {
            if (useTransaction)
            {
                if (_transaction == null)
                    throw new InvalidOperationException("No transaction has been started.");

                // Execute with transaction if one exists
                await Connection.ExecuteAsync(sql, parameters, transaction: _transaction.GetDbTransaction());
            }
            else
            {
                // Execute without transaction if none exists
                await Connection.ExecuteAsync(sql, parameters);
            }
        }




        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
            // Clear the savepoints
            _savepoints.Clear();

            // Dispose of the DbContext
            _context.Dispose();
        }
    }
}
