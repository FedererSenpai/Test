using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Insert : Base
    {
        BindingSource bs = new BindingSource();
        private static Random r = new Random((int)DateTime.Now.Ticks);

        public Insert()
        {
            InitializeComponent();
            FolderPath = Path.Combine(Application.StartupPath, "Result");
            //while(true)
            //{
            //    RandomString(0,15,false,true);
            //}
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            DataRow dr = ds.Tables[0].NewRow();
            ds.Tables[0].Columns.Add(new DataColumn("Tipo"));
            dr[0] = DatosInsert.typo.Text.ToString();
            ds.Tables[0].Rows.Add(dr);
            dr = ds.Tables[0].NewRow();
            dr[0] = DatosInsert.typo.Num.ToString();
            ds.Tables[0].Rows.Add(dr);
            dr = ds.Tables[0].NewRow();
            dr[0] = DatosInsert.typo.Bool.ToString();
            ds.Tables[0].Rows.Add(dr);
            dr = ds.Tables[0].NewRow();
            dr[0] = DatosInsert.typo.Custom.ToString();
            ds.Tables[0].Rows.Add(dr); this.Tipo.ValueMember = ds.Tables[0].Columns["Tipo"].ToString();
            this.Tipo.DisplayMember = ds.Tables[0].Columns["Tipo"].ToString();
            this.Tipo.DataSource = ds.Tables[0];

            this.dataGridView1.DataSource = bs;
            this.Tipo.DataPropertyName = "Tipo";
            this.Campo.DataPropertyName = "Campo";

        }

        public int RandomInt(int max, int min)
        {
            int i = 0;
            i = r.Next(min, max);
            return i;
        }

        public double RandomDouble(double max, double min, int decimales)
        {
            double d = 0.0;
            d = Math.Round(min + (max - min) * r.NextDouble(), decimales, MidpointRounding.AwayFromZero);
            return d;
        }

        public string RandomString(int minchar, int maxchar, bool constchar, bool rellenar)
        {
            string s = string.Empty;
            int max = maxchar;
            if (!constchar)
            {
                max = r.Next(minchar, maxchar + 1);
            }
            for (int i = 0; i < max; i++)
            {
                s += (char)r.Next(33, 127);
            }
            if (rellenar)
            {
                for (int j = s.Length; j < maxchar; j++)
                    s += '*';
            }
            return s;
        }

        public bool RandomBool()
        {
            bool b = false;
            b = Convert.ToBoolean(r.Next(0, 2));
            return b;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].GetType() == typeof(DataGridViewButtonColumn) && (dataGridView1.Columns[e.ColumnIndex] as DataGridViewButtonColumn).Text == "Editar")
            {
                if (e.RowIndex > -1 && dataGridView1.Rows[e.RowIndex].Cells[1].Value != null)
                {
                    switch (dataGridView1.Rows[e.RowIndex].Cells[1].Value)
                    {
                        case "Text":
                            new InsertText((dataGridView1.Rows[e.RowIndex].DataBoundItem as DatosInsert)).ShowDialog();
                            break;
                        case "Num":
                            new InsertNum((dataGridView1.Rows[e.RowIndex].DataBoundItem as DatosInsert)).ShowDialog();
                            break;
                        case "Bool":
                            new InsertBool((dataGridView1.Rows[e.RowIndex].DataBoundItem as DatosInsert)).ShowDialog();
                            break;
                    }
                }
            (dataGridView1.Rows[e.RowIndex].DataBoundItem as DatosInsert).ToString();
                return;
            }
        }

        private string[] CrearFilaCustom(DatosInsert di, int filas)
        {
            string[] valores = Enumerable.Repeat("'"+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+"'", filas).ToArray();//new string[filas];
            return valores;
        }

        private string[] CrearFilaNum(DatosInsert di, int filas)
        {
            string[] valores = new string[filas];
            string valor;
            int decimales = 0;
            if (di.NumDecimal)
                decimales = di.Decimalesmax;

            if (di.Aleatorio)
            {
                if (di.Autoincremento)
                {
                    for (int i = 0; i < valores.Length; i++)
                    {
                        valores[i] = (di.Minimo + i).ToString();
                    }
                }
                else
                {
                    for (int i = 0; i < valores.Length; i++)
                    {
                        di.Maximo = 100;
                        valores[i] = GenerarNumero(di.Minimo, di.Maximo, decimales, di.NoCero, di.Rango);
                    }
                }
            }
            else
            {
                valor = $"{di.Texto}";
                valores = Enumerable.Repeat(valor, filas).ToArray();
            }
            return valores;
        }

        private string[] CrearFilaBool(DatosInsert di, int filas)
        {
            string[] valores = new string[filas];
            string valor;
            if (di.Aleatorio)
            {
                if (di.Repetir)
                {
                    valor = GenerarBool();
                    valores = Enumerable.Repeat(valor, filas).ToArray();
                }
                else
                {
                    for (int i = 0; i < filas; i++)
                    {
                        valor = GenerarBool();
                        valores[i] = valor;
                    }
                }
            }
            else
            {
                valor = di.TrueFalse.ToString();
                valores = Enumerable.Repeat(valor, filas).ToArray();
            }
            return valores;
        }

        private string[] CrearFilaString(DatosInsert di, int filas)
        {
            string[] valores = new string[filas];
            string valor;
            string minusculas = "qwertyuiopasdfghjklñzxcvbnm";
            string mayusculas = "QWERTYUIOPASDFGHJKLÑZXCVBNM";
            string numeros = "0123456789";
            string especiales = @"$%#@!*?;:^&,.-_{}[]+ç|/~¬¿¡()";//no esta el caracter \
            string todos = string.Empty;
            if (di.Aleatorio)
            {
                if (di.Minusculas)
                    todos += minusculas;
                if (di.Mayusculas)
                    todos += mayusculas;
                if (di.Numeros)
                    todos += numeros;
                if (di.Caracteresespeciales)
                    todos += especiales;

                if (string.IsNullOrEmpty(todos))
                {
                    valor = "''";
                    valores = Enumerable.Repeat(valor, filas).ToArray();
                    return valores;
                }

                if (di.Repetir)
                {
                    if (di.Fijo)
                    {
                        valor = $"'{GenerarString(todos, di.Longitud)}'";
                        valores = Enumerable.Repeat(valor, filas).ToArray();
                    }
                    else
                    {
                        valor = $"'{GenerarString(todos, di.Minimo, di.Maximo)}'";
                        valores = Enumerable.Repeat(valor, filas).ToArray();
                    }
                }
                else
                {
                    if (di.Fijo)
                    {
                        for (int i = 0; i < filas; i++)
                        {
                            valor = GenerarString(todos, di.Longitud);
                            valores[i] = $"'{valor}'";
                        }
                    }
                    else
                    {
                        for (int i = 0; i < filas; i++)
                        {
                            valor = GenerarString(todos, di.Minimo, di.Maximo);
                            valores[i] = $"'{valor}'";
                        }
                    }
                }
            }
            else
            {
                valor = $"'{di.Texto}'";
                valores = Enumerable.Repeat(valor, filas).ToArray();
            }
            if(di.Primermayuscula)
            {
                for(int i = 0; i<valores.Length;i++)
                {
                    valores[i] = valores[i].Substring(0, 2).ToUpper() + valores[i].Substring(2).ToLower();
                }
            }
            return valores;
        }

        public string GenerarString(string cadena, int longitudMin, int longitudMax)
        {
            int longitud = r.Next(longitudMin, longitudMax);
            string resultado = string.Empty;
            for (int i = 0; i < longitud; i++)
            {
                resultado += cadena[r.Next(cadena.Length)];
            }
            return resultado;
        }

        public string GenerarString(string cadena, int longitud)
        {
            string resultado = string.Empty;
            if (!string.IsNullOrEmpty(cadena))
            {
                for (int i = 0; i < longitud; i++)
                {
                    resultado += cadena[r.Next(cadena.Length)];
                }
            }
            return resultado;
        }

        public string GenerarNumero(int minimo, int maximo, int decimales, bool cero, bool rango)
        {
            string resultado = string.Empty;
            decimal potencia = (decimal)Math.Pow(10, decimales);
            resultado = (r.Next(minimo * (int)potencia, maximo * (int)potencia) / potencia).ToString().Replace(",", ".");
            if (cero && decimal.Parse(resultado) == 0)
                resultado = GenerarNumero(minimo, maximo, decimales, cero, rango);
            return resultado;
        }
        private string GenerarBool()
        {
            string resultado = string.Empty;
            resultado = Convert.ToBoolean(r.Next(2)).ToString();
            return resultado;
        }

        private void GenerarSql(string[][] valores, int filas)
        {
            string sql = GenerarCabeceraSql(textbox1.Text);
            for (int i = 0; i < filas; i++)
            {
                for(int j = 0; j < valores.Length; j++)
                {
                    sql += $"{valores[j][i]}, ";
                }
                sql = sql.Substring(0, sql.Length - 2) + "), (";
            }
            sql = sql.Substring(0, sql.Length - 3) + ";";
            sql.ToFile(Path.Combine(Application.StartupPath, "Result", "sql.txt"));       
        }

        private string GenerarCabeceraSql(string tabla)
        {
            string sql = "INSERT INTO " + tabla + " (";
            foreach (DatosInsert di in bs)
            {
                sql += $"{di.Campo},";
            }
            sql = sql.Substring(0, sql.Length - 1) + ") VALUES (";
            return sql;
        }

        private void button1_click(object sender, EventArgs e)
        {
            bs.Add(new DatosInsert());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int columnas = this.dataGridView1.Rows.Count;
            int filas = (int)this.numericUpDown1.Value;
            string[][] valores = new string[columnas][];
            int cont = 0;
            foreach (DatosInsert di in bs)
            {
                if(di.Tipo.Equals(DatosInsert.typo.Text.ToString()))
                {
                    valores[cont] = CrearFilaString(di, filas);
                    cont++;
                }
                else if (di.Tipo.Equals(DatosInsert.typo.Num.ToString()))
                {
                    valores[cont] = CrearFilaNum(di, filas);
                    cont++;
                }
                else if(di.Tipo.Equals(DatosInsert.typo.Bool.ToString()))
                {
                    valores[cont] = CrearFilaBool(di, filas);
                    cont++;
                }
                else if (di.Tipo.Equals(DatosInsert.typo.Custom.ToString()))
                {
                    valores[cont] = CrearFilaCustom(di, filas);
                    cont++;
                }
            }
            GenerarSql(valores, filas);
            ExtensionMethods.WriteToFile(Path.Combine(Application.StartupPath, "Result", "Design.json"),bs.Cast<DatosInsert>().ToList().ToJson());
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 48 || e.KeyChar > 57)
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                bs.RemoveAt(dataGridView1.CurrentRow.Index);
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            BBDD bbdd = new BBDD();
            if (bbdd.ShowDialog() == DialogResult.OK)
            {
                textbox1.Text = bbdd.Datatable;
                DataTable dt = MySQL.GetColumns(bbdd.Database, bbdd.Datatable);
                bbdd.Close();
                bs.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    DatosInsert di = new DatosInsert();
                    di.Tipo = GetTipo(dr["COLUMN_TYPE"].ToString());
                    di.Campo = dr["COLUMN_NAME"].ToString();
                    di.Autoincremento = dr["ORDINAL_POSITION"].ToString().Equals("1") && dr["COLUMN_KEY"].ToString().Equals("PRI");
                    di.Longitud = Math.Min(10,GetLength(dr["COLUMN_TYPE"].ToString()));
                    di.Maximo = di.Longitud;
                    di.Fijo = false;
                    di.NumDecimal = dr["COLUMN_TYPE"].ToString().Contains("double") || dr["COLUMN_TYPE"].ToString().Contains("decimal");
                    di.Decimalesmax = 3;
                    di.Minusculas = true;
                    di.Mayusculas = true;
                    di.Numeros = true;
                    di.Caracteresespeciales = true;
                    bs.Add(di);
                }
            }
        }

        private int GetDecimals(string type)
        {
            if (type.Contains('(') && type.Contains(')') && type.Contains(','))
                return Convert.ToInt16(type.Split('(', ')')[1].Split(',')[1]);
            else
                return 0;

        }

        private int GetLength(string type)
        {
            if (type.Contains('(') && type.Contains(')'))
                return Convert.ToInt16(type.Split('(', ')')[1].Split(',')[0]);
            else
                return new Random().Next(0,1000);
        }

        private string GetTipo(string type)
        {
            switch(type)
            {
                case string a when a.Contains("char"):
                case string b when b.Contains("text"):
                    return DatosInsert.typo.Text.ToString();
                case string a when a.Contains("tiny"):
                    return DatosInsert.typo.Bool.ToString();
                case string a when a.Contains("int"):
                case string b when b.Contains("double"):
                case string c when c.Contains("decimal"):
                    return DatosInsert.typo.Num.ToString();
                default:
                    return DatosInsert.typo.Custom.ToString();
            }
        }
        //PTE: Bindings controles
        //PTE: Crear insert
        //PTE: Crear Editar Num
    }
}
