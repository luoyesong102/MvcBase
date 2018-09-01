using System;
using System.Data;
using System.Xml;
//using System.Data.OracleClient;
using System.Collections;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
//使用ODP.NET访问oracle效率比data.OracleClient高

namespace Esmart.Framework.DB
{
	/// <summary>
	/// The OracleHelp class is intended to encapsulate high performance, scalable best practices for 
	/// common uses of OracleClient
	/// </summary>
    public sealed class OracleHelp
    {
       
        #region private utility methods & constructors
        
        // Since this class provides only static methods, make the default constructor private to prevent 
        // instances from being created with "new OracleHelp()"
        private OracleHelp() {}

        /// <summary>
        /// This method is used to attach array of OracleParameters to a OracleCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of OracleParameters to be added to command</param>
        private static void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
        {
			if( command == null ) throw new ArgumentNullException( "command" );
			if( commandParameters != null )
			{
				foreach (OracleParameter p in commandParameters)
				{
					if( p != null )
					{
						// Check for derived output value with no value assigned
						if ( ( p.Direction == ParameterDirection.InputOutput || 
							p.Direction == ParameterDirection.Input ) && 
							(p.Value == null))
						{
							p.Value = DBNull.Value;
						}
						command.Parameters.Add(p);
					}
				}
			}
        }

        /// <summary>
        /// This method assigns dataRow column values to an array of OracleParameters
        /// </summary>
        /// <param name="commandParameters">Array of OracleParameters to be assigned values</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>
        private static void AssignParameterValues(OracleParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null)) 
            {
                // Do nothing if we get no data
                return;
            }

			int i = 0;
            // Set the parameters values
            foreach(OracleParameter commandParameter in commandParameters)
            {
				// Check the parameter name
				if( commandParameter.ParameterName == null || 
					commandParameter.ParameterName.Length <= 1 )
					throw new Exception( 
						string.Format( 
							"Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.", 
							i, commandParameter.ParameterName ) );
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
				i++;
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of OracleParameters
        /// </summary>
        /// <param name="commandParameters">Array of OracleParameters to be assigned values</param>
        /// <param name="parameterValues">Array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(OracleParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null)) 
            {
                // Do nothing if we get no data
                return;
            }

            //xakep 加入 判断最后一位参数是否是out类型并且是字符集 
            

            // We must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length && !(commandParameters.Length > 0 && commandParameters[commandParameters.Length - 1].Direction == ParameterDirection.Output && commandParameters[commandParameters.Length - 1].OracleDbType == OracleDbType.RefCursor))
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            // Iterate through the OracleParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property
                if (commandParameters.Length == parameterValues.Length)
                {
                    if (parameterValues[i] is IDbDataParameter)
                    {
                        IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                        if (paramInstance.Value == null)
                        {
                            commandParameters[i].Value = DBNull.Value;
                        }
                        else
                        {
                            commandParameters[i].Value = paramInstance.Value;
                        }
                    }
                    else if (parameterValues[i] == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = parameterValues[i];
                    }
                }
                else
                {
                    if (i == j-1)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        if (parameterValues[i] is IDbDataParameter)
                        {
                            IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                            if (paramInstance.Value == null)
                            {
                                commandParameters[i].Value = DBNull.Value;
                            }
                            else
                            {
                                commandParameters[i].Value = paramInstance.Value;
                            }
                        }
                        else if (parameterValues[i] == null)
                        {
                            commandParameters[i].Value = DBNull.Value;
                        }
                        else
                        {
                            commandParameters[i].Value = parameterValues[i];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command
        /// </summary>
        /// <param name="command">The OracleCommand to be prepared</param>
        /// <param name="connection">A valid OracleConnection, on which to execute this command</param>
        /// <param name="transaction">A valid OracleTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
        private static void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters, out bool mustCloseConnection )
        {
			if( command == null ) throw new ArgumentNullException( "command" );
			if( commandText == null || commandText.Length == 0 ) throw new ArgumentNullException( "commandText" );

            // If the provided connection is not open, we will open it
			if (connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				connection.Open();
			}
			else
			{
				mustCloseConnection = false;
			}

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or Oracle statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
				if( transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
                // command.Transaction = transaction;//使用Oracle.DataAccess时，注释掉这句就可以了，如果是Data.OracleClient不用注释掉
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        #endregion private utility methods & constructors

        #region ExecuteNonQuery

        /// <summary>
        /// Execute a OracleCommand (that returns no resultset and takes no parameters) against the database specified in 
        /// the connection string
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteNonQuery(connectionString, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );

            // Create & open a OracleConnection, and dispose of it after we are done
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns no resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="spName">The name of the stored prcedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            //}
        }

        /// <summary>
        /// Execute a OracleCommand (that returns no resultset and takes no parameters) against the provided OracleConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteNonQuery(connection, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns no resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {	
			if( connection == null ) throw new ArgumentNullException( "connection" );

            // Create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
			bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection );
    		
            // Finally, execute the command
            int retval = cmd.ExecuteNonQuery();
    		
            // Detach the OracleParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
			if( mustCloseConnection )
				connection.Close();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns no resultset) against the specified OracleConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleConnection connection, string spName, params object[] parameterValues)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            //}
        }

        /// <summary>
        /// Execute a OracleCommand (that returns no resultset and takes no parameters) against the provided OracleTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteNonQuery(transaction, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns no resultset) against the specified OracleTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
		{
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );

			// Create a command and prepare it for execution
			OracleCommand cmd = new OracleCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection );
    			
			// Finally, execute the command
			int retval = cmd.ExecuteNonQuery();
    			
			// Detach the OracleParameters from the command object, so they can be used again
			cmd.Parameters.Clear();
			return retval;
		}

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns no resultset) against the specified 
        /// OracleTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(OracleTransaction transaction, string spName, params object[] parameterValues)
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

			// If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            //}
        }

        #endregion ExecuteNonQuery

        #region ExecuteDataset

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteDataset(connectionString, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );

            // Create & open a OracleConnection, and dispose of it after we are done
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );
			
			// If we receive parameter values, we need to figure out where they go
           // if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            //}
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteDataset(connection, commandType, commandText, (OracleParameter[])null);
        }
		
        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
		{
			if( connection == null ) throw new ArgumentNullException( "connection" );

			// Create a command and prepare it for execution
			OracleCommand cmd = new OracleCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection );
    			
			// Create the DataAdapter & DataSet
			using( OracleDataAdapter da = new OracleDataAdapter(cmd) )
			{
				DataSet ds = new DataSet();

				// Fill the DataSet using default values for DataTable names, etc
				da.Fill(ds);
				
				// Detach the OracleParameters from the command object, so they can be used again
				cmd.Parameters.Clear();

				if( mustCloseConnection )
					connection.Close();

				// Return the dataset
				return ds;
			}	
		}
		
        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(OracleConnection connection, string spName, params object[] parameterValues)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            //}
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(OracleTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteDataset(transaction, commandType, commandText, (OracleParameter[])null);
        }
		
        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
		{
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );

			// Create a command and prepare it for execution
			OracleCommand cmd = new OracleCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection );
    			
			// Create the DataAdapter & DataSet
			using( OracleDataAdapter da = new OracleDataAdapter(cmd) )
			{
				DataSet ds = new DataSet();

				// Fill the DataSet using default values for DataTable names, etc
				da.Fill(ds);
    			
				// Detach the OracleParameters from the command object, so they can be used again
				cmd.Parameters.Clear();

				// Return the dataset
				return ds;
			}	
		}
		
        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified 
        /// OracleTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataset(OracleTransaction transaction, string spName, params object[] parameterValues)
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );
			
			// If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            //}
        }

        #endregion ExecuteDataset
		
        #region ExecuteReader

        /// <summary>
        /// This enum is used to indicate whether the connection was provided by the caller, or created by OracleHelp, so that
        /// we can set the appropriate CommandBehavior when calling ExecuteReader()
        /// </summary>
        private enum OracleConnectionOwnership	
        {
            /// <summary>Connection is owned and managed by OracleHelp</summary>
            Internal, 
            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }

        /// <summary>
        /// Create and prepare a OracleCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        /// 
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">A valid OracleConnection, on which to execute this command</param>
        /// <param name="transaction">A valid OracleTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="connectionOwnership">Indicates whether the connection parameter was provided by the caller, or created by OracleHelp</param>
        /// <returns>OracleDataReader containing the results of the command</returns>
        private static OracleDataReader ExecuteReader(OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters, OracleConnectionOwnership connectionOwnership)
        {	
			if( connection == null ) throw new ArgumentNullException( "connection" );

			bool mustCloseConnection = false;
            // Create a command and prepare it for execution
            OracleCommand cmd = new OracleCommand();
			try
			{
				PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection );
			
				// Create a reader
				OracleDataReader dataReader;

				// Call ExecuteReader with the appropriate CommandBehavior
				if (connectionOwnership == OracleConnectionOwnership.External)
				{
					dataReader = cmd.ExecuteReader();
				}
				else
				{
					dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				}
			
				// Detach the OracleParameters from the command object, so they can be used again.
				// HACK: There is a problem here, the output parameter values are fletched 
				// when the reader is closed, so if the parameters are detached from the command
				// then the OracleReader can�t set its values. 
				// When this happen, the parameters can�t be used again in other command.
				bool canClear = true;
				foreach(OracleParameter commandParameter in cmd.Parameters)
				{
					if (commandParameter.Direction != ParameterDirection.Input)
						canClear = false;
				}
            
				if (canClear)
				{
					cmd.Parameters.Clear();
				}

				return dataReader;
			}
			catch
			{
				if( mustCloseConnection )
					connection.Close();
				throw;
			}
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteReader(connectionString, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
            OracleConnection connection = null;
            try
            {
				connection = new OracleConnection(connectionString);
				connection.Open();

                // Call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(connection, null, commandType, commandText, commandParameters,OracleConnectionOwnership.Internal);
            }
            catch
            {
                // If we fail to return the OracleDatReader, we need to close the connection ourselves
                if( connection != null ) connection.Close();
                throw;
            }
            
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );
			
			// If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //     Otherwise we can just call the SP without params
            //    return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            //}
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(OracleConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteReader(connection, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            // Pass through the call to the private overload using a null transaction value and an externally owned connection
            return ExecuteReader(connection, (OracleTransaction)null, commandType, commandText, commandParameters, OracleConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(OracleConnection connection, string spName, params object[] parameterValues)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            //}
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(OracleTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteReader(transaction, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///   OracleDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );

			// Pass through to private overload, indicating that the connection is owned by the caller
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, OracleConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified
        /// OracleTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  OracleDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReader(OracleTransaction transaction, string spName, params object[] parameterValues)
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            //}
        }

        #endregion ExecuteReader

        #region ExecuteScalar
		
        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteScalar(connectionString, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
            // Create & open a OracleConnection, and dispose of it after we are done
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );
			
			// If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            //}
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset and takes no parameters) against the provided OracleConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(OracleConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteScalar(connection, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
		{
			if( connection == null ) throw new ArgumentNullException( "connection" );

			// Create a command and prepare it for execution
			OracleCommand cmd = new OracleCommand();

			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection );
    			
			// Execute the command & return the results
			object retval = cmd.ExecuteScalar();
    			
			// Detach the OracleParameters from the command object, so they can be used again
			cmd.Parameters.Clear();

			if( mustCloseConnection )
				connection.Close();

			return retval;
		}

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the specified OracleConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(OracleConnection connection, string spName, params object[] parameterValues)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            //}
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset and takes no parameters) against the provided OracleTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteScalar(transaction, commandType, commandText, (OracleParameter[])null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a 1x1 resultset) against the specified OracleTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(OracleTransaction transaction, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
		{
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );

			// Create a command and prepare it for execution
			OracleCommand cmd = new OracleCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection );
    			
			// Execute the command & return the results
			object retval = cmd.ExecuteScalar();
    			
			// Detach the OracleParameters from the command object, so they can be used again
			cmd.Parameters.Clear();
			return retval;
		}

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the specified
        /// OracleTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(OracleTransaction transaction, string spName, params object[] parameterValues)
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // PPull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            //}
        }

        #endregion ExecuteScalar	


        #region FillDataset
        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)</param>
        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( dataSet == null ) throw new ArgumentNullException( "dataSet" );
            
			// Create & open a OracleConnection, and dispose of it after we are done
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                FillDataset(connection, commandType, commandText, dataSet, tableNames);
            }
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        public static void FillDataset(string connectionString, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params OracleParameter[] commandParameters)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( dataSet == null ) throw new ArgumentNullException( "dataSet" );
            // Create & open a OracleConnection, and dispose of it after we are done
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, 24);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>    
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        public static void FillDataset(string connectionString, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( dataSet == null ) throw new ArgumentNullException( "dataSet" );
            // Create & open a OracleConnection, and dispose of it after we are done
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                FillDataset (connection, spName, dataSet, tableNames, parameterValues);
            }
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>    
        public static void FillDataset(OracleConnection connection, CommandType commandType, 
            string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        public static void FillDataset(OracleConnection connection, CommandType commandType, 
            string commandText, DataSet dataSet, string[] tableNames,
            params OracleParameter[] commandParameters)
        {
            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  FillDataset(conn, "GetOrders", ds, new string[] {"orders"}, 24, 36);
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        public static void FillDataset(OracleConnection connection, string spName, 
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if ( connection == null ) throw new ArgumentNullException( "connection" );
			if (dataSet == null ) throw new ArgumentNullException( "dataSet" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

			// If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
            //}    
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset and takes no parameters) against the provided OracleTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        public static void FillDataset(OracleTransaction transaction, CommandType commandType, 
            string commandText,
            DataSet dataSet, string[] tableNames)
        {
            FillDataset (transaction, commandType, commandText, dataSet, tableNames, null);    
        }

        /// <summary>
        /// Execute a OracleCommand (that returns a resultset) against the specified OracleTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
        public static void FillDataset(OracleTransaction transaction, CommandType commandType, 
            string commandText, DataSet dataSet, string[] tableNames,
            params OracleParameter[] commandParameters)
        {
            FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified 
        /// OracleTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  FillDataset(trans, "GetOrders", ds, new string[]{"orders"}, 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        public static void FillDataset(OracleTransaction transaction, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues) 
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
			if( dataSet == null ) throw new ArgumentNullException( "dataSet" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If we receive parameter values, we need to figure out where they go
            //if ((parameterValues != null) && (parameterValues.Length > 0)) 
            //{
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of OracleParameters
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            //}
            //else 
            //{
            //    // Otherwise we can just call the SP without params
            //    FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            //}    
        }

        /// <summary>
        /// Private helper method that execute a OracleCommand (that returns a resultset) against the specified OracleTransaction and OracleConnection
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(conn, trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid OracleConnection</param>
        /// <param name="transaction">A valid OracleTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-Oracle command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="commandParameters">An array of OracleParamters used to execute the command</param>
		private static void FillDataset(OracleConnection connection, OracleTransaction transaction, CommandType commandType, 
			string commandText, DataSet dataSet, string[] tableNames,
			params OracleParameter[] commandParameters)
		{
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( dataSet == null ) throw new ArgumentNullException( "dataSet" );

			// Create a command and prepare it for execution
			OracleCommand command = new OracleCommand();
			bool mustCloseConnection = false;
			PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection );
    			
			// Create the DataAdapter & DataSet
			using( OracleDataAdapter dataAdapter = new OracleDataAdapter(command) )
			{
                
				// Add the table mappings specified by the user
				if (tableNames != null && tableNames.Length > 0)
				{
					string tableName = "Table";
					for (int index=0; index < tableNames.Length; index++)
					{
						if( tableNames[index] == null || tableNames[index].Length == 0 ) throw new ArgumentException( "The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames" );
						dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                        //程飞对应网上所说，在下面加上了
                        tableName = "Table";
						tableName += (index + 1).ToString();
					}
				}
                
				// Fill the DataSet using default values for DataTable names, etc
				dataAdapter.Fill(dataSet);

				// Detach the OracleParameters from the command object, so they can be used again
				command.Parameters.Clear();
			}

			if( mustCloseConnection )
				connection.Close();
		}
		#endregion
        
        #region UpdateDataset
        /// <summary>
        /// Executes the respective command for each inserted, updated, or deleted row in the DataSet.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order");
        /// </remarks>
        /// <param name="insertCommand">A valid transact-Oracle statement or stored procedure to insert new records into the data source</param>
        /// <param name="deleteCommand">A valid transact-Oracle statement or stored procedure to delete records from the data source</param>
        /// <param name="updateCommand">A valid transact-Oracle statement or stored procedure used to update records in the data source</param>
        /// <param name="dataSet">The DataSet used to update the data source</param>
        /// <param name="tableName">The DataTable used to update the data source.</param>
        public static void UpdateDataset(OracleCommand insertCommand, OracleCommand deleteCommand, OracleCommand updateCommand, DataSet dataSet, string tableName)
        {
			if( insertCommand == null ) throw new ArgumentNullException( "insertCommand" );
			if( deleteCommand == null ) throw new ArgumentNullException( "deleteCommand" );
			if( updateCommand == null ) throw new ArgumentNullException( "updateCommand" );
			if( tableName == null || tableName.Length == 0 ) throw new ArgumentNullException( "tableName" ); 

            // Create a OracleDataAdapter, and dispose of it after we are done
            using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
            {
                // Set the data adapter commands
                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;

                // Update the dataset changes in the data source
                dataAdapter.Update (dataSet, tableName); 

                // Commit all the changes made to the DataSet
                dataSet.AcceptChanges();
            }
        }
        #endregion

        #region CreateCommand
        /// <summary>
        /// Simplify the creation of a Oracle command object by allowing
        /// a stored procedure and optional parameters to be provided
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  OracleCommand command = CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName");
        /// </remarks>
        /// <param name="connection">A valid OracleConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="sourceColumns">An array of string to be assigned as the source columns of the stored procedure parameters</param>
        /// <returns>A valid OracleCommand object</returns>
        public static OracleCommand CreateCommand(OracleConnection connection, string spName, params string[] sourceColumns) 
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // Create a OracleCommand
            OracleCommand cmd = new OracleCommand( spName, connection );
            cmd.CommandType = CommandType.StoredProcedure;

            // If we receive parameter values, we need to figure out where they go
            if ((sourceColumns != null) && (sourceColumns.Length > 0)) 
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided source columns to these parameters based on parameter order
                for (int index=0; index < sourceColumns.Length; index++)
                    commandParameters[index].SourceColumn = sourceColumns[index];

                // Attach the discovered parameters to the OracleCommand object
                AttachParameters (cmd, commandParameters);
            }

            return cmd;
        }
        #endregion

        #region ExecuteNonQueryTypedParams
        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns no resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQueryTypedParams(String connectionString, String spName, DataRow dataRow)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );
			
			// If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                                
                return OracleHelp.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns no resultset) against the specified OracleConnection 
        /// using the dataRow column values as the stored procedure's parameters values.  
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="connection">A valid OracleConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQueryTypedParams(OracleConnection connection, String spName, DataRow dataRow)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                                
                return OracleHelp.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns no resultset) against the specified
        /// OracleTransaction using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="transaction">A valid OracleTransaction object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQueryTypedParams(OracleTransaction transaction, String spName, DataRow dataRow)
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // Sf the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                                
                return OracleHelp.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteDatasetTypedParams
        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDatasetTypedParams(string connectionString, String spName, DataRow dataRow)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            //If the row has values, the store procedure parameters must be initialized
            if ( dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                
                return OracleHelp.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the dataRow column values as the store procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="connection">A valid OracleConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDatasetTypedParams(OracleConnection connection, String spName, DataRow dataRow)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If the row has values, the store procedure parameters must be initialized
            if( dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                
                return OracleHelp.ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified OracleTransaction 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="transaction">A valid OracleTransaction object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDatasetTypedParams(OracleTransaction transaction, String spName, DataRow dataRow)
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If the row has values, the store procedure parameters must be initialized
            if( dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                
                return OracleHelp.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion

        #region ExecuteReaderTypedParams
        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReaderTypedParams(String connectionString, String spName, DataRow dataRow)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );
			
			// If the row has values, the store procedure parameters must be initialized
            if ( dataRow != null && dataRow.ItemArray.Length > 0 )
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                
                return OracleHelp.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }

                
        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified OracleConnection 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connection">A valid OracleConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReaderTypedParams(OracleConnection connection, String spName, DataRow dataRow)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If the row has values, the store procedure parameters must be initialized
            if( dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                
                return OracleHelp.ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }
        
        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a resultset) against the specified OracleTransaction 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="transaction">A valid OracleTransaction object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A OracleDataReader containing the resultset generated by the command</returns>
        public static OracleDataReader ExecuteReaderTypedParams(OracleTransaction transaction, String spName, DataRow dataRow)
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If the row has values, the store procedure parameters must be initialized
            if( dataRow != null && dataRow.ItemArray.Length > 0 )
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                
                return OracleHelp.ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteScalarTypedParams
        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a OracleConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalarTypedParams(String connectionString, String spName, DataRow dataRow)
        {
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );
			
			// If the row has values, the store procedure parameters must be initialized
            if( dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connectionString, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                
                return OracleHelp.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the specified OracleConnection 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connection">A valid OracleConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalarTypedParams(OracleConnection connection, String spName, DataRow dataRow)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If the row has values, the store procedure parameters must be initialized
            if( dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(connection, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                
                return OracleHelp.ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a OracleCommand (that returns a 1x1 resultset) against the specified OracleTransaction
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="transaction">A valid OracleTransaction object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalarTypedParams(OracleTransaction transaction, String spName, DataRow dataRow)
        {
			if( transaction == null ) throw new ArgumentNullException( "transaction" );
			if( transaction != null && transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            // If the row has values, the store procedure parameters must be initialized
            if( dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = OracleHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                
                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);
                
                return OracleHelp.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return OracleHelp.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion


    }

	/// <summary>
	/// OracleHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
	/// ability to discover parameters for stored procedures at run-time.
	/// </summary>
	public sealed class OracleHelperParameterCache
	{
		#region private methods, variables, and constructors

		//Since this class provides only static methods, make the default constructor private to prevent 
		//instances from being created with "new OracleHelperParameterCache()"
		private OracleHelperParameterCache() {}

		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Resolve at run time the appropriate set of OracleParameters for a stored procedure
        /// </summary>
        /// <param name="connection">A valid OracleConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
        /// <returns>The parameter array discovered.</returns>
		private static OracleParameter[] DiscoverSpParameterSet(OracleConnection connection, string spName, bool includeReturnValueParameter)
		{
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

			OracleCommand cmd = new OracleCommand(spName, connection);
			cmd.CommandType = CommandType.StoredProcedure;

			connection.Open();
			OracleCommandBuilder.DeriveParameters(cmd);
			connection.Close();

			if (!includeReturnValueParameter) 
			{
                
               //  cmd.Parameters.RemoveAt(0);  返回数据集，大多为out类型，此处应不执行
                
			}
                
			OracleParameter[] discoveredParameters = new OracleParameter[cmd.Parameters.Count];

			cmd.Parameters.CopyTo(discoveredParameters, 0);

			// Init the parameters with a DBNull value
			foreach (OracleParameter discoveredParameter in discoveredParameters)
			{
				discoveredParameter.Value = DBNull.Value;
			}
			return discoveredParameters;
		}

		/// <summary>
		/// Deep copy of cached OracleParameter array
		/// </summary>
		/// <param name="originalParameters"></param>
		/// <returns></returns>
		private static OracleParameter[] CloneParameters(OracleParameter[] originalParameters)
		{
			OracleParameter[] clonedParameters = new OracleParameter[originalParameters.Length];

			for (int i = 0, j = originalParameters.Length; i < j; i++)
			{
				clonedParameters[i] = (OracleParameter)((ICloneable)originalParameters[i]).Clone();
			}

			return clonedParameters;
		}

		#endregion private methods, variables, and constructors

		#region caching functions

		/// <summary>
		/// Add parameter array to the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a OracleConnection</param>
		/// <param name="commandText">The stored procedure name or T-Oracle command</param>
		/// <param name="commandParameters">An array of OracleParamters to be cached</param>
		public static void CacheParameterSet(string connectionString, string commandText, params OracleParameter[] commandParameters)
		{
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( commandText == null || commandText.Length == 0 ) throw new ArgumentNullException( "commandText" );

			string hashKey = connectionString + ":" + commandText;

			paramCache[hashKey] = commandParameters;
		}

		/// <summary>
		/// Retrieve a parameter array from the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a OracleConnection</param>
		/// <param name="commandText">The stored procedure name or T-Oracle command</param>
		/// <returns>An array of OracleParamters</returns>
		public static OracleParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( commandText == null || commandText.Length == 0 ) throw new ArgumentNullException( "commandText" );

			string hashKey = connectionString + ":" + commandText;

			OracleParameter[] cachedParameters = paramCache[hashKey] as OracleParameter[];
			if (cachedParameters == null)
			{			
				return null;
			}
			else
			{
				return CloneParameters(cachedParameters);
			}
		}

		#endregion caching functions

		#region Parameter Discovery Functions

		/// <summary>
		/// Retrieves the set of OracleParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OracleConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of OracleParameters</returns>
		public static OracleParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return GetSpParameterSet(connectionString, spName, false);  
		}

		/// <summary>
		/// Retrieves the set of OracleParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OracleConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OracleParameters</returns>
		public static OracleParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			if( connectionString == null || connectionString.Length == 0 ) throw new ArgumentNullException( "connectionString" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            using(OracleConnection connection = new OracleConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
		}

        /// <summary>
        /// Retrieves the set of OracleParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid OracleConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of OracleParameters</returns>
        internal static OracleParameter[] GetSpParameterSet(OracleConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary>
        /// Retrieves the set of OracleParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid OracleConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of OracleParameters</returns>
        internal static OracleParameter[] GetSpParameterSet(OracleConnection connection, string spName, bool includeReturnValueParameter)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
            using (OracleConnection clonedConnection = (OracleConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of OracleParameters appropriate for the stored procedure
        /// </summary>
        /// <param name="connection">A valid OracleConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of OracleParameters</returns>
        private static OracleParameter[] GetSpParameterSetInternal(OracleConnection connection, string spName, bool includeReturnValueParameter)
        {
			if( connection == null ) throw new ArgumentNullException( "connection" );
			if( spName == null || spName.Length == 0 ) throw new ArgumentNullException( "spName" );

            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter":"");

            OracleParameter[] cachedParameters;
        	
            cachedParameters = paramCache[hashKey] as OracleParameter[];
            if (cachedParameters == null)
            {	
                OracleParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
		        paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }
        	
            return CloneParameters(cachedParameters);
        }
        
		#endregion Parameter Discovery Functions

	}
}
