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
    public partial class Smite : Form
    {
        int maxhealth = 10000;
        int health;
        readonly int smite = 900;
        bool killed = false;
        Random time = new Random();
        Random damage = new Random();
        Stopwatch sw = new Stopwatch();
        private readonly object healthLock = new object();

        public Smite()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Smitear);
            health = maxhealth;
            progressBar1.Maximum = maxhealth;
            progressBar1.Value = maxhealth;
        }

        
        private void Smitear(Object o, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.D)
            {
                //TakeDamage(smite);
                if(health < smite)
                {
                    sw.Stop();
                    MessageBox.Show("Tu tiempo de reaccion ha sido de " + sw.ElapsedMilliseconds + "ms.");
                }
                else
                {
                    MessageBox.Show("Has fallado el smite");
                }
                TakeDamage(smite);
            }
        }

        private void TakeDamage(int damage)
        {
            lock (healthLock)
            {
                health = Math.Max(health - damage, 0);
                if (health <= 0)
                {
                    killed = true;
                }
            }
        }

        private async Task Fight()
        {
            while (!killed)
            {
                await Task.Delay(time.Next(0, 500));
                TakeDamage(damage.Next(0, 500));
                label1.Text = health.ToString();
                progressBar1.Value = health;
                if (health < smite)
                    sw.Start();
            }
            if(sw.IsRunning)
            MessageBox.Show("Has fallado el smite");
        }

        private async void button1_MouseClickAsync(object sender, MouseEventArgs e)
        {
            await Fight();
        }
    }
}
