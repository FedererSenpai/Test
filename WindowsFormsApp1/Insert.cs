using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Insert : Form
    {
        BindingSource bs = new BindingSource();
        public Insert()
        {
            InitializeComponent();
            //while(true)
            //{
            //    RandomString(0,15,false,true);
            //}
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            DataRow dr = ds.Tables[0].NewRow();
            ds.Tables[0].Columns.Add(new DataColumn("Tipo"));
            dr[0] = "Text";
            ds.Tables[0].Rows.Add(dr);
            dr = ds.Tables[0].NewRow();
            dr[0] = "Num";
            ds.Tables[0].Rows.Add(dr);
            dr = ds.Tables[0].NewRow();
            dr[0] = "Bool";
            ds.Tables[0].Rows.Add(dr);
            this.Tipo.ValueMember = ds.Tables[0].Columns["Tipo"].ToString();
            this.Tipo.DisplayMember = ds.Tables[0].Columns["Tipo"].ToString();
            this.Tipo.DataSource = ds.Tables[0];

            this.dataGridView1.DataSource = bs;
            this.Tipo.DataPropertyName = "Tipo";
            this.Campo.DataPropertyName = "Campo";

        }

        public int RandomInt(int max, int min)
        {
            int i = 0;
            Random r = new Random();
            i = r.Next(min, max);
            return i;
        }

        public double RandomDouble(double max, double min, int decimales)
        {
            double d = 0.0;
            Random r = new Random();
            d = Math.Round(min + (max - min) * r.NextDouble(), decimales, MidpointRounding.AwayFromZero);
            return d;
        }

        public string RandomString(int minchar, int maxchar, bool constchar, bool rellenar)
        {
            string s = string.Empty;
            Random r = new Random();
            int max = maxchar;
            if(!constchar)
            {
                max = r.Next(minchar, maxchar + 1);
            }
            for (int i = 0; i < max; i++)
            {
                s += (char)r.Next(33,127);
            }
            if(rellenar)
            {
                for (int j = s.Length; j < maxchar; j++)
                    s += '*';
            }
            return s;
        }

        public bool RandomBool()
        {
            bool b = false;
            Random r = new Random();
            b = Convert.ToBoolean(r.Next(0, 2));
            return b;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].GetType() == typeof(DataGridViewButtonColumn) && (dataGridView1.Columns[e.ColumnIndex] as DataGridViewButtonColumn).Text == "Editar")
            {
                if (e.RowIndex > -1 && dataGridView1.Rows[e.RowIndex].Cells[1].Value != null)
                {
                    new InsertText((dataGridView1.Rows[e.RowIndex].DataBoundItem as DatosInsert)).ShowDialog();
                    switch (dataGridView1.Rows[e.RowIndex].Cells[1].Value)
                    {
                        case "Text":
                            break;
                        case "Num":
                            break;
                        case "Bool":
                            break;
                    }
                }
            }
            //(dataGridView1.Rows[e.RowIndex].DataBoundItem as DatosInsert).ToString();
            return;
        }

        private void button1_click(object sender, EventArgs e)
        {
            bs.Add(new DatosInsert());
        }

        //PTE: Bindings controles
        //PTE: Meter botón para borrar linea.
        //PTE: Meter elegir numero de inserts.
        //PTE: Crear insert
        //PTE: Crear Editar Num
        //PTE: Crear Editar Bool
        
    }
}
