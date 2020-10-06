using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Guldkortet
{
    public partial class Form1 : Form
    {

        //the port number
        private int port = 12345;
        //the clinic
        private TcpClient client;
        //connect the client
        private TcpListener listener;

        

        public Form1()
        {
            InitializeComponent();
            //This metod will run when the program run
            ReadFromTextFile();
        }


        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                //The IP address that the client should connect
                IPAddress address = IPAddress.Parse("127.0.0.1");
                //TCPListener object
                listener = new TcpListener(address, port);
                //start the listener
                listener.Start();
                //Give the button a color to know that there is a connection
                button1.BackColor = Color.LightGreen;
                button1.Enabled = false;
                StartReception();
                //textBox1.Text = "Test connection";
                button1.Text = "Connected";
            }
            catch (Exception error)
            {
                //in case of error
                MessageBox.Show(error.Message, this.Text);
                button1.BackColor = Color.Red;
                button1.Text = "Not connected";
                return;
            }
            


        }

        private async void StartReception()
        {
            try
            {
                //Connect to the client
                client = await listener.AcceptTcpClientAsync();
            }
            catch (Exception error)
            {
                //in case of error
                MessageBox.Show(error.Message, this.Text);
                return;
            }
            StartReading(client);
        }

        private async void StartReading(TcpClient k)
        {
            try
            {
                //Receive a message from the client
                byte[] indata = new byte[1024];
                int n = await k.GetStream().ReadAsync(indata, 0, indata.Length);
                string message = Encoding.Unicode.GetString(indata, 0, n);
                //The message should displayed in textbox1
                textBox1.AppendText(message);
                

            }
            catch (Exception error)
            {
                //in case of error
                MessageBox.Show(error.Message, this.Text);
                return;
            }
            StartReading(k);
        }
        public void ReadFromTextFile()
        {

            //If the customer list text file exists
            if (File.Exists("kundlista.txt"))
            {
                
            }
        }
    }
}




