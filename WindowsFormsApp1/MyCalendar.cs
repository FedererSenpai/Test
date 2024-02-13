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
    public partial class MyCalendar : UserControl
    {
        private DateTime date;
        public MyCalendar()
        {
            InitializeComponent();
            Date = DateTime.Now;
            CreateMonth();
        }

        public DateTime Date { get => date; set
            {
                date = value; 
                label1.Text = date.ToString("MMMM 'de' yyyy");
                monthCalendar1.SelectionStart = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1);
                monthCalendar1.SelectionEnd = new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, DateTime.DaysInMonth(date.AddMonths(-1).Year, date.AddMonths(-1).Month));
                monthCalendar2.SelectionStart = new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, 1);
                monthCalendar2.SelectionEnd = new DateTime(date.AddMonths(1).Year, date.AddMonths(1).Month, DateTime.DaysInMonth(date.AddMonths(1).Year, date.AddMonths(1).Month));
            }
        }

        private void CreateMonth()
        {
            tableLayoutPanel3.Controls.Clear();
            DateTime mydate = new DateTime(date.Year, date.Month, 1);
            int column = 0;
            int row = 0;
            for(int i = mydate.DayOfWeek.ToInt(); i > 0; i--)
            {
                tableLayoutPanel3.Controls.Add(new MyDay(mydate.AddDays(i * -1), false, string.Empty), column, row);
                column++;
            }
            while (mydate.Month == date.Month)
            {
                tableLayoutPanel3.Controls.Add(new MyDay(mydate, true, "1h 30m"), column, row);
                mydate = mydate.AddDays(1);
                column++;
                if (column == 7)
                {
                    column = 0;
                    row++;
                }
            }
            while(row < 6)
            {
                tableLayoutPanel3.Controls.Add(new MyDay(mydate, false, string.Empty), column, row);
                mydate = mydate.AddDays(1);
                column++;
                if (column == 7)
                {
                    column = 0;
                    row++;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Date = date.AddMonths(1);
            CreateMonth();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Date = date.AddMonths(-1);
            CreateMonth();
        }
    }
}
