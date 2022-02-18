using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = SynchronizationContext.Current?.GetType().FullName;

            AccessUIFromATask();
        }

        void AccessUIFromATask()
        {
            var t = Task.Run(() =>
            {
                try
                {
                    Thread.Sleep(1000);

                    MessageBox.Show("We're in a task now, and SynchronizationContext is: " + SynchronizationContext.Current?.GetType().FullName);

                    textBox1.Text = "From Task!";
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "EXCEPTION!!!");
                }
            });
        }
    }
}
