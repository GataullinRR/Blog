using Microsoft.EntityFrameworkCore;
using System;

namespace DBModels
{
    public class SaveChangesResult
    {
        public int RowsUpdated { get; }
        public Exception Exception { get; }
        public bool IsSuccessful => Exception == null;
        public DbUpdateException UpdateException => Exception as DbUpdateException;
        public DbUpdateConcurrencyException ConcurrencyException => Exception as DbUpdateConcurrencyException;

        public SaveChangesResult(int rowsUpdated)
            :this(rowsUpdated, null)
        {

        }
        public SaveChangesResult(Exception exception) 
            : this(0, exception ?? throw new ArgumentNullException(nameof(exception)))
        {

        }

        SaveChangesResult(int rowsUpdated, Exception exception)
        {
            RowsUpdated = rowsUpdated;
            Exception = exception;
        }
    }
}
