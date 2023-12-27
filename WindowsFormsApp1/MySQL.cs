using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace WindowsFormsApp1
{
    
    public static class MySQL
    {
        private static MySqlConnectionStringBuilder cadenaConexion = new MySqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["cnnString"].ConnectionString);

        public static MySqlConnectionStringBuilder CadenaConexion { get => cadenaConexion; }
        private static IDbConnection connection = null;
        private static int idConexion = int.MinValue;
        private const string bd_defecto = "sys_datos";
        private const string puerto_defecto = "3306";
        private const string usuario_defecto = "user";
        private const string password_defecto = "dibal";

        public static IDbConnection Connection
        {
            get
            {
                //148D DI -> 155A
                if (connection == null)
                    connection = CreateConnection(connection);
                //connection = CreateConnection(connection);
                //AbreConexion(connection);               
                return connection;
            }
            set
            {
                connection = value;
            }
        }

        public static string Bd_defecto => bd_defecto;

        public static string Puerto_defecto => puerto_defecto;

        public static string Usuario_defecto => usuario_defecto;

        public static string Password_defecto => password_defecto;

        public static int EjecutaNonQuery(IDbConnection con, string sql)
        {
            int registros;
            try
            {
                // Creamos una conexión por si es nula
                con = CreateConnection(con);

                // Creamos un comando
                IDbCommand cmd = CreateCommand();

                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                    registros = Convert.ToInt32(cmd.ExecuteNonQuery());


            }
            catch (Exception ex)
            {
                registros = -1;
                throw ex;

            }

            return registros;
        }

        public static object EjecutaScalar(IDbConnection con, string sql)
        {
            object i = 0;
            try
            {
                // Creamos una conexión por si es nula
                con = CreateConnection(con);

                // Creamos un comando
                IDbCommand cmd = CreateCommand();

                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                    i = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
            }
            return i;
        }

        public static DataSet EjecutaQuery(IDbConnection con, string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                //// Creamos una conexión
                con = CreateConnection(con);

                // Creamos un comando
                IDbCommand cmd = CreateCommand();

                // Creamos un DataAdpater generico y lo asignamos a tipo de proveedor
                DbDataAdapter da;

                        da = new MySqlDataAdapter((MySqlCommand)cmd);

                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                    da.Fill(ds);
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                ds.Tables.Add();
                throw ex;
            }

            //connection.Close();

            return ds;
        }

        public static IDbCommand CreateCommand()
        {
            IDbCommand objectCmd;

                    objectCmd = new MySqlCommand();
            return (objectCmd);
        }

        public static IDbConnection CreateConnection(IDbConnection con)
    {
        try
        {

            if (con != null)
            {
                if (con.State == ConnectionState.Open)
                {
                    //con este ping solucionamos el problema de desconexión temporal o problema de timeout
                                //}
                                if (con == null)
                                {
                                            con = new MySqlConnection(CadenaConexion.ToString());

                                }
                                AbreConexion(con);

                    return (con);
                }
                else
                {
                    AbreConexion(con);
                    return con;
                }
            }

                    con = new MySqlConnection(CadenaConexion.ToString());
            return (con);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
        public static void AbreConexion(IDbConnection con)
        {
            int i = 1;
            while (i <= 5)
            {
                try
                {
                    if (con.State != ConnectionState.Open)
                    {
                        con.Close();

                        idConexion = int.MinValue;

                        con.Open();

                    }
                    break;
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static DataTable GetColumns(string database, string datatable)
        {
            string sql = string.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}'; ", database, datatable);
            return EjecutaQuery(Connection, sql).Tables[0];
        }
    }
}
