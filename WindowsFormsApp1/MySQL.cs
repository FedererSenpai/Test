using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class MySQL
    {
        mysql
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

                //148D DI -> 155A
                //AbreConexion(con);
                if (!con.ConnectionString.ToLower().Contains("localhost") && frmTecladoVenta != null)
                {
                    EjecutaConsultasHilo consulta = new EjecutaConsultasHilo(con, null, cmd, frmTecladoVenta);
                    registros = Convert.ToInt32(consulta.cmdExecuteNonQuery());
                }
                else
                    registros = Convert.ToInt32(cmd.ExecuteNonQuery());


            }
            catch (AbrirConexionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                registros = -1;
                Log.EscribirError(ex.StackTrace, ex.Message);
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

                //148D DI -> 155A
                //AbreConexion(con);
                if (!con.ConnectionString.ToLower().Contains("localhost") && frmTecladoVenta != null)
                {
                    EjecutaConsultasHilo consulta = new EjecutaConsultasHilo(con, null, cmd, frmTecladoVenta);
                    i = consulta.cmdExecuteScalar();
                }
                else
                    i = cmd.ExecuteScalar();
            }
            catch (AbrirConexionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Log.EscribirError(ex.StackTrace, ex.Message);
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

                switch (Globales.Config.CadenaConexion.Provider)
                {
                    case provider_sqlclient:
                        da = new SqlDataAdapter((SqlCommand)cmd);
                        break;
                    case provider_oledb:
                        da = new OleDbDataAdapter((OleDbCommand)cmd);
                        break;
                    case provider_odbc:
                        da = new OdbcDataAdapter((OdbcCommand)cmd);
                        break;
                    case provider_mysql:
                        da = new MySqlDataAdapter((MySqlCommand)cmd);
                        break;
                    default:
                        //ResourceManager LocRM = Mensajes.ResourceManager;
                        //string sError= (LocRM.GetString("ProveedorIncorrecto")).ToString();
                        throw (new Exception(MensajesCS1100.ProveedorIncorrecto));
                }

                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                //148D DI -> 155A
                //AbreConexion(con);
                if (!con.ConnectionString.ToLower().Contains("localhost") && frmTecladoVenta != null)
                {
                    EjecutaConsultasHilo consulta = new EjecutaConsultasHilo(con, da, cmd, frmTecladoVenta);
                    ds = consulta.Fill();
                }
                else
                    da.Fill(ds);
                cmd.Parameters.Clear();
            }
            catch (AbrirConexionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                ds.Tables.Add();
                Log.EscribirError(ex.StackTrace, ex.Message);
                throw ex;
            }

            //connection.Close();

            return ds;
        }

    public static IDbConnection CreateConnection(IDbConnection con)
    {
        bool relojAct = false;
        bool abrirConexion = false;
        try
        {
            string proveedor = Globales.Config.CadenaConexion.Provider;

            if (con != null)
            {
                abrirConexion = true;
                if (con.State == ConnectionState.Open)
                {
                    //con este ping solucionamos el problema de desconexión temporal o problema de timeout
                    if (proveedor == provider_mysql)
                    {
                        //lo dejo para que de momento pase por solo el ping
                        if (con.ConnectionString.ToLower().Contains("localhost") || frmTecladoVenta == null)
                        {
                            ((MySqlConnection)con).Ping();
                        }
                        else
                        {

                            if (Globales.formPerdidaConexionActivo)
                            {
                                throw new Exception(AccesoMensajes.ObtenerTraduccion("FormPerdidaConexionActivo")); //"Sin conexión con la maestra. Formulario Perdida de conexión abierto"
                            }
                            //System.Threading.ParameterizedThreadStart param = new System.Threading.ParameterizedThreadStart(hiloPingMySQL);
                            //System.Threading.Thread ping = new System.Threading.Thread(param);
                            //ping.Start(con);
                            PingMySQLHilo ping = new PingMySQLHilo(con);
                            ping.Ping();
                            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                            sw.Reset();
                            sw.Start();
                            while (ping.Finalizado == false && sw.Elapsed.TotalSeconds < 10)
                            {
                                if (sw.Elapsed.TotalMilliseconds > 100)
                                {
                                    System.Threading.Thread.Sleep(1);
                                    Application.DoEvents();
                                }

                                if (sw.Elapsed.TotalSeconds > 2 && relojAct == false)
                                {
                                    Globales.conexionConMaestra = false;
                                    frmEspera.MostrarReloj(AccesoMensajes.ObtenerTraduccion("buscandoServidor"));
                                    relojAct = true;
                                }
                            }
                            sw.Stop();
                            if (relojAct)
                            {
                                frmEspera.OcultarReloj();
                                relojAct = false;
                            }
                            if (!(ping.Correcto && sw.Elapsed.TotalSeconds < 10))
                            {

                                //if (!sond.Conectado)
                                //{
                                if (!Globales.formPerdidaConexionActivo)
                                {
                                    Globales.formPerdidaConexionActivo = true;
                                    if (perd == null)
                                        perd = new PerdidaConexion(Globales.Config.CadenaConexion.Server, Globales.Config.CadenaConexion.Port);
                                    perd.Opacity = 0;
                                    perd.ShowDialog(frmTecladoVenta);
                                    Globales.formPerdidaConexionActivo = false;
                                }
                                else
                                {
                                    throw new Exception(AccesoMensajes.ObtenerTraduccion("FormPerdidaConexionActivo")); //"Sin conexión con la maestra. Formulario Perdida de conexión abierto"
                                }
                                //}
                                if (con == null)
                                {
                                    Globales.Config.CadenaConexion.Remove("provider");
                                    switch (proveedor)
                                    {
                                        case provider_sqlclient:
                                            con = new SqlConnection(Globales.Config.CadenaConexion.ToString());
                                            break;
                                        case provider_oledb:
                                            con = new OleDbConnection(Globales.Config.CadenaConexion.ToString());
                                            break;
                                        case provider_odbc:
                                            con = new OdbcConnection(Globales.Config.CadenaConexion.ToString());
                                            break;
                                        case provider_mysql:
                                            con = new MySqlConnection(Globales.Config.CadenaConexion.ToString());
                                            break;
                                        default:
                                            throw (new Exception(MensajesCS1100.ProveedorIncorrecto));
                                    }
                                    Globales.Config.CadenaConexion.Provider = proveedor;

                                }
                                AbreConexion(con);
                            }
                            else if (Globales.conexionConMaestra == false)
                                Globales.conexionConMaestra = true;

                        }


                    }
                    return (con);
                }
                else
                {
                    AbreConexion(con);
                    return con;
                }
            }

            Globales.Config.CadenaConexion.Remove("provider");

            switch (proveedor)
            {
                case provider_sqlclient:
                    con = new SqlConnection(Globales.Config.CadenaConexion.ToString());
                    break;
                case provider_oledb:
                    con = new OleDbConnection(Globales.Config.CadenaConexion.ToString());
                    break;
                case provider_odbc:
                    con = new OdbcConnection(Globales.Config.CadenaConexion.ToString());
                    break;
                case provider_mysql:
                    con = new MySqlConnection(Globales.Config.CadenaConexion.ToString());
                    break;
                default:
                    throw (new Exception(MensajesCS1100.ProveedorIncorrecto));
            }
            Globales.Config.CadenaConexion.Provider = proveedor;
            if (abrirConexion)
                AbreConexion(con);
            return (con);
        }
        catch (AbrirConexionException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (relojAct)
            {
                frmEspera.OcultarReloj();
                relojAct = false;
            }
        }
    }
    }
}
