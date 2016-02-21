// -----------------------------------------------------------------------
// <copyright file="DBConnection.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides a class for managing thread-safe connections to the ACMA database
    /// </summary>
    public class DBConnection : IDisposable
    {
        /// <summary>
        /// The SqlConnection in use on this DataContext
        /// </summary>
        private SqlConnection connection;

        /// <summary>
        /// Indicates if the object has been disposed
        /// </summary>
        private bool disposed;

        private string connectionString;

        /// <summary>
        /// Initializes a new instance of the DBConnection class
        /// </summary>
        /// <param name="connection">The SqlConnection to use</param>
        public DBConnection(SqlConnection connection)
        {
            this.connection = connection;

            if (this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
            }
        }

        /// <summary>
        /// Initializes a new instance of the DBConnection class
        /// </summary>
        /// <param name="connectionString">The connection string to use</param>
        public DBConnection(string serverName, string databaseName)
        {
            this.connectionString = "Server='" + serverName + "';Initial Catalog='" + databaseName + "';Integrated Security=True;MultipleActiveResultSets=True";

            this.connection = new SqlConnection(this.ConnectionString);
            this.connection.Open();
        }

        public DBConnection(string connectionString)
        {
            this.connectionString = connectionString;

            this.connection = new SqlConnection(this.ConnectionString);
            this.connection.Open();
        }

        /// <summary>
        /// Finalizes an instance of the DBConnection class
        /// </summary>
        ~DBConnection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the current SqlConnection for this object
        /// </summary>
        private SqlConnection Connection
        {
            get
            {
                if (this.connection == null)
                {
                    this.connection = new SqlConnection(this.ConnectionString);
                    this.connection.Open();
                }

                return this.connection;
            }
        }

        public SqlConnection GetSqlConnection()
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Gets the connection string
        /// </summary>
        private string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }

        public void ResetConnection()
        {
            this.connection = new SqlConnection(this.connectionString);
            this.connection.Open();
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        /// <param name="disposing">A value indicating if the disposal is coming from a call to IDisposable.Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.connection != null)
                    {
                        if (this.connection.State != ConnectionState.Closed)
                        {
                            this.connection.Close();
                        }
                    }
                }
            }

            this.disposed = true;
        }
    }
}
