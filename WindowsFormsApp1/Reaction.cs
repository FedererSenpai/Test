using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Reaction : Form
    {
        Random random = new Random();
        Stopwatch stopwatch = new Stopwatch();
        int TimesTested = 0;
        int ReactionTime1 = 0;
        int ReactionTime2 = 0;
        int ReactionTime3 = 0;
        bool ButtonPressed = false;
        int ReactionTime { get; set; }

        public Reaction()
        {
            InitializeComponent();
            button1.BackColor = Color.Red;

        }

        public void button1_Click_1(object sender, EventArgs e)
        {
            if (TimesTested < 3)
            {
                ReactionTest();
            }
            else
            {
                MessageBox.Show("Your times were: " + Convert.ToString(ReactionTime1) + ", " + Convert.ToString(ReactionTime2) + ", " + Convert.ToString(ReactionTime3));
            }
        }

        public void btnReactionButton_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            stopwatch.Stop();
            button1.BackColor = Color.Red;
            button1.Text = "Your time was " + stopwatch.ElapsedMilliseconds + "ms milliseconds";
            ReactionTime = Convert.ToInt32(stopwatch.ElapsedMilliseconds);

            if (TimesTested == 0)
            {
                ReactionTime1 = ReactionTime;
                TimesTested = TimesTested + 1;
            }
            else if (TimesTested == 1)
            {
                ReactionTime2 = ReactionTime;
                TimesTested = TimesTested + 1;
            }
            else if (TimesTested == 2)
            {
                ReactionTime3 = ReactionTime;
                TimesTested = TimesTested + 1;
            }
        }

        public async void ReactionTest()
        {
            stopwatch.Reset();
            button1.Text = "3";
            await Task.Delay(1000);
            button1.Text = "2";
            await Task.Delay(1000);
            button1.Text = "1";
            await Task.Delay(1000);
            button1.Text = "0";
            await Task.Delay(1000);
            button1.Text = "Press this button when it turns green";
            await Task.Delay(random.Next(3000, 10000));
            button1.Visible = true;
            stopwatch.Start();
            button1.BackColor = Color.Green;
            button1.Text = "Click Now!";
            button1.Enabled = true;
        }
    }
}
