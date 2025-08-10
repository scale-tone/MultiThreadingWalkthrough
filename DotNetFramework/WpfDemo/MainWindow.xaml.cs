using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        async Task AsyncTest()
        {
            Debug.WriteLine($">> 1: {Thread.CurrentThread.ManagedThreadId}");

            // await Task.Delay(2000);
            Thread.Sleep(2000);

            Debug.WriteLine($">> 2: {Thread.CurrentThread.ManagedThreadId}");

            // await Task.Delay(2000);
            Thread.Sleep(2000);

            Debug.WriteLine($">> 3: {Thread.CurrentThread.ManagedThreadId}");

            //await Task.Delay(2000);
            Thread.Sleep(2000);

            Debug.WriteLine($">> 4: {Thread.CurrentThread.ManagedThreadId}");
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            textBox1.Text = $"1: {Thread.CurrentThread.ManagedThreadId}";

            await this.AsyncTest();

            textBox1.Text = $"2: {Thread.CurrentThread.ManagedThreadId}";

            await this.AsyncTest();

            textBox1.Text = $"3: {Thread.CurrentThread.ManagedThreadId}";
            */

            AccessUIFromATask();

            //await UpdateStatusBar();
        }

        async Task UpdateStatusBar()
        {
            textBox1.Text = DateTime.Now.ToString();

            Thread.Sleep(1000);

            await this.UpdateStatusBar();
        }

        void AccessUIFromATask()
        {
            var t = Task.Run(() =>
            {
                try
                {
                    Thread.Sleep(1000);

                    MessageBox.Show("We're in a task now!");

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
