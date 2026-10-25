using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

/// <summary>
/// Small helper around System.Data.SQLite.
/// Values are always passed as parameters: write @p0, @p1... in the SQL and
/// pass the values in the same order.
/// </summary>
public class SQLiteDatabase
{
    private readonly String dbConnection;

    public SQLiteDatabase(String inputFile)
    {
        SQLiteConnectionStringBuilder connBuilder = new SQLiteConnectionStringBuilder();
        connBuilder.DataSource = inputFile;
        connBuilder.Version = 3;
        connBuilder.PageSize = 4096;
        connBuilder.CacheSize = 10000;
        connBuilder.JournalMode = SQLiteJournalModeEnum.Wal;
        connBuilder.Pooling = true;
        connBuilder.SyncMode = SynchronizationModes.Normal;
        connBuilder.LegacyFormat = false;
        connBuilder.DefaultTimeout = 500;

        dbConnection = connBuilder.ToString();
    }

    private SQLiteConnection Open()
    {
        SQLiteConnection cnn = new SQLiteConnection(dbConnection);
        cnn.Open();
        return cnn;
    }

    private static SQLiteCommand Command(SQLiteConnection cnn, String sql, object[] args)
    {
        SQLiteCommand cmd = new SQLiteCommand(sql, cnn);
        if (args != null)
        {
            for (int i = 0; i < args.Length; i++)
            {
                cmd.Parameters.AddWithValue("@p" + i, args[i] ?? DBNull.Value);
            }
        }
        return cmd;
    }

    public DataTable GetDataTable(String sql, params object[] args)
    {
        DataTable dt = new DataTable();
        using (SQLiteConnection cnn = Open())
        using (SQLiteCommand cmd = Command(cnn, sql, args))
        using (SQLiteDataReader reader = cmd.ExecuteReader())
        {
            dt.Load(reader);
        }
        return dt;
    }

    public int ExecuteNonQuery(String sql, params object[] args)
    {
        using (SQLiteConnection cnn = Open())
        using (SQLiteCommand cmd = Command(cnn, sql, args))
        {
            return cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Runs an INSERT and returns the id of the new row.
    /// </summary>
    public long ExecuteInsert(String sql, params object[] args)
    {
        using (SQLiteConnection cnn = Open())
        {
            using (SQLiteCommand cmd = Command(cnn, sql, args))
            {
                cmd.ExecuteNonQuery();
            }
            return cnn.LastInsertRowId;
        }
    }

    /// <summary>
    /// Runs several statements (without parameters) inside one transaction.
    /// </summary>
    public int ExecuteNonQueryBulk(String[] sqls)
    {
        int rowsUpdated = 0;
        using (SQLiteConnection cnn = Open())
        using (SQLiteTransaction transaction = cnn.BeginTransaction())
        {
            foreach (String sql in sqls)
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, cnn, transaction))
                {
                    rowsUpdated += cmd.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
        return rowsUpdated;
    }

    public String ExecuteScalar(String sql, params object[] args)
    {
        using (SQLiteConnection cnn = Open())
        using (SQLiteCommand cmd = Command(cnn, sql, args))
        {
            object value = cmd.ExecuteScalar();
            return (value != null && value != DBNull.Value) ? value.ToString() : "";
        }
    }

    /// <summary>
    /// True if the file opens as a SQLite database and contains the given table.
    /// </summary>
    public bool HasTable(String table)
    {
        try
        {
            return ExecuteScalar("SELECT name FROM sqlite_master WHERE type='table' AND name=@p0", table) == table;
        }
        catch (Exception ex)
        {
            WDS.Globals.AddToLogFile("Database file check failed (table '" + table + "'): " + ex);
            return false;
        }
    }
}
