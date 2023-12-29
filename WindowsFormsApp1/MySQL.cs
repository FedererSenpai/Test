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

        public static int EjecutaNonQuery(string sql)
        {
            return EjecutaNonQuery(Connection, sql);
        }

        public static IDbConnection NewConnection(string server ,string database)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(string.Empty) { Database = database, Server = server };
            return new MySqlConnection(builder.ToString());
        }

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

    public class MySqlConnectionStringBuilder   // por Javier Sanz
    {
        // Si queremos hacer databinding con instancias de esta clase no podemos heredar, hay que delegar.
        // Ver http://www.winterdom.com/weblog/2006/07/22/ICustomTypeDescriptorAndDataBindingInNET20.aspx
        readonly DbConnectionStringBuilder _stringBuilder = new DbConnectionStringBuilder();

        public MySqlConnectionStringBuilder(string cadena)
        {
            _stringBuilder.ConnectionString = cadena;//DesEncriptada;
            if (!_stringBuilder.ContainsKey("port")) this.Port = 3306;
            //148D DI -> 155A
            //if (!_stringBuilder.ContainsKey("connection timeout")) this.ConnectionTimeout = 180000;
            if (!_stringBuilder.ContainsKey("database")) this.Database = MySQL.Bd_defecto;
            if (!_stringBuilder.ContainsKey("user id")) this.UserID = MySQL.Usuario_defecto;
            if (!_stringBuilder.ContainsKey("password")) this.Password = MySQL.Password_defecto;
            if (!_stringBuilder.ContainsKey("server")) this.Server = "localhost";

            //148D DI -> 155A
            //a las balanzas esclavas establecemos el timeout en pocos segundos para que si pierde la conexión en el momento de hacer el open
            //no se quede indefinidamente esperando
            if (!this.Server.ToLower().Contains("localhost"))
                this.ConnectionTimeout = 10;
        }
        public int Port
        {
            get { return int.Parse(_stringBuilder["port"].ToString()); }
            set { _stringBuilder["port"] = value.ToString(); }
        }
        public int ConnectionTimeout
        {
            get { return int.Parse(_stringBuilder["connection timeout"].ToString()); }
            set { _stringBuilder["connection timeout"] = value.ToString(); }
        }
        public string Database
        {
            get { return _stringBuilder["database"].ToString(); }
            set { _stringBuilder["database"] = value; }
        }
        public string UserID
        {
            get { return _stringBuilder["user id"].ToString(); }
            set { _stringBuilder["user id"] = value; }
        }
        public string Password
        {
            get { return _stringBuilder["password"].ToString(); }
            set { _stringBuilder["password"] = value; }
        }
        public string Server
        {
            get { return _stringBuilder["server"].ToString(); }
            set { _stringBuilder["server"] = value; }
        }

        public void Remove(string s)
        {
            this._stringBuilder.Remove(s);
        }
        public override string ToString()
        {
            return this._stringBuilder.ToString();
        }
    }
}
