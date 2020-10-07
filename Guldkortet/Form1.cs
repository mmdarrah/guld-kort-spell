using System;
using System.Collections.Generic;
using System.Drawing;
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
        //the client
        private TcpClient client;
        //connect the client
        private TcpListener listener;

        //list of string from the class list user
        Userlist<string> userlist = new Userlist<string>("", "", "");
        List<Userlist<string>> userInfo = new List<Userlist<string>>();
        //list of string from the class list user
        List<Cardlist<string>> cardInfo = new List<Cardlist<string>>();

        public Form1()
        {
            InitializeComponent();
            //This metod will run when the program run
            ReadFromTextFile();
            // The main picture
            pictureBox3.Load(Application.StartupPath + "\\Grafik\\nos.png");
        }
        // a privet async function to handel the connection
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
                // Creating a list of strings
                List<string> items = new List<string>();
                // StreamReader to load a text, line by line
                StreamReader reader = new StreamReader("kundlista.txt", Encoding.Default, false);
                string item = "";
                // Continue to read until you reach end of file
                while ((item = reader.ReadLine()) != null)
                {
                    items.Add(item);
                }
                //Lopping 
                foreach (string a in items)
                {
                    //Put each part in the correct variable
                    string[] vektor = a.Split(new string[] { "###" }, StringSplitOptions.None);
                    string SerialNumber = vektor[0];
                    string Name = vektor[1];
                    string Location = vektor[2];
                    //Put all variables in a list
                    userlist = new Userlist<string>(SerialNumber, Name, Location);
                    userInfo.Add(userlist);

                }
            }

            else
            {
                //in case of error
                MessageBox.Show("Textfilen hittades inte");

            }

            //If the cards list text file exists
            if (File.Exists("kortlista.txt"))
            {
                // Creating a list of strings
                List<string> items = new List<string>();
                // StreamReader to load a text, line by line
                StreamReader reader = new StreamReader("kortlista.txt", Encoding.Default, false);

                string item = "";
                // Continue to read until you reach end of file
                while ((item = reader.ReadLine()) != null)
                {
                    items.Add(item);
                }
                //Lopping 
                foreach (string a in items)
                {
                    //Put each part in the correct variable
                    string[] vektor = a.Split(new string[] { "###" }, StringSplitOptions.None);
                    string CardNumber = vektor[0];
                    string Reward = vektor[1];
                    //According to the reward the case will fire
                    switch (Reward)
                    {
                        case "Dunderkatt":
                            Dunderkatt<string> Dund = new Dunderkatt<string>(CardNumber);
                            cardInfo.Add(Dund);
                            break;
                        case "Kristallhäst":
                            Kristallhäst<string> Krist = new Kristallhäst<string>(CardNumber);
                            cardInfo.Add(Krist);
                            break;
                        case "Överpanda":
                            Överpanda<string> över = new Överpanda<string>(CardNumber);
                            cardInfo.Add(över);
                            break;
                        case "Eldtomat":
                            Eldtomat<string> Eldto = new Eldtomat<string>(CardNumber);
                            cardInfo.Add(Eldto);
                            break;

                    }

                }

            }
            else
            {
                //in case of error
                MessageBox.Show("Textfilen hittades inte");

            }
        }

        // This button is responsible for connecting to the server
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
                button1.Text = "Ansluten";
            }
            catch (Exception error)
            {
                //in case of error
                MessageBox.Show(error.Message, this.Text);
                button1.BackColor = Color.Red;
                button1.Text = "Inte ansluten";
                return;
            }



        }

        // This button is responsible for check the prize
        private async void button2_Click(object sender, EventArgs e)
        {
           
            try
            {

                //Check if the client is connected so we can send the message
                if (client.Connected)
                {
                    client.NoDelay = true;
                    byte[] buffer = new byte[2000 * 2000];
                    string[] vektore = textBox1.Text.Split(new string[] { "-" }, StringSplitOptions.None);
                    bool user = false;// if the user is correct we will change it to true
                    bool card = false;// if the card is correct we will change it to true


                    //To compaere the data from the card list and the user list we need two loops
                    for (int c = 0; c < cardInfo.Count; c++)
                    {
                        for (int u = 0; u < userInfo.Count; u++)
                        {
                            if (userInfo[u].serialNumber == vektore[0] && cardInfo[c].cardNumber == vektore[1])
                            {
                                //ToString method will be send to the client
                                buffer = Encoding.Unicode.GetBytes(userInfo[u].ToString() + cardInfo[c].ToString());
                                await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
                                //We change the variables to true
                                user = true;
                                card = true;
                                //Two pictures will pop up when the user wins a prize the first is the same for all prizes, the second is unique according to the prize
                                pictureBox2.Load(Application.StartupPath + "\\Grafik\\grattis.png");
                                pictureBox1.Load(Application.StartupPath + "\\Grafik\\" + cardInfo[c].reward + ".png");

                            }
                        }
                    }
                    //A loop to find when the user is correct but the card is not
                    for (int i = 0; i < userInfo.Count; i++)

                    {
                        if (userInfo[i].serialNumber == vektore[0] && card == false)
                        {
                            buffer = Encoding.Unicode.GetBytes(userInfo[i].ToString() + " ditt kodkort är inte rätt försök igen!");
                            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
                            user = true;
                            card = false;
                        }
                    }


                    //A loop to find when the user is false but the card is true
                    for (int i = 0; i < cardInfo.Count; i++)
                    {
                        if (user == false && cardInfo[i].cardNumber == vektore[1])
                        {
                            buffer = Encoding.Unicode.GetBytes(cardInfo[i].ToString() + " men du har inget konto hos oss!");
                            await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
                            user = false;
                            card = true;
                        }
                    }



                    //Both user and card is not exist
                    if (user == false && card == false)
                    {
                        buffer = Encoding.Unicode.GetBytes("Kunden och kortet finns inte. Var god försök igen!");
                        await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
                    }



                }
            }
            catch
            {
                //fel meddelande ska visas om någon gick fel
                MessageBox.Show("Klienten ansluter inte till servern!");
                return;
            }
        }

        // This button is responsible for delet the data inside the textBox
        private async void button3_Click(object sender, EventArgs e)
        {
            //Delete text inside testBox1
            textBox1.Text = "";
            //Delete the prize images
            pictureBox2.Image = null;
            pictureBox1.Image = null;
            //Add color when click for half second
            button3.BackColor = Color.LightSalmon;
            await Task.Delay(500);
            button3.BackColor = SystemColors.Control;
        }

        //The field where the data from server appear
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //A unique picture according to the prize
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //A congratulation picture appear every time the user win a prize
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            //The main picture
        }
    }
}





//cards class list
class Cardlist<T>
{
    // Class properties all are public strings
    public string cardNumber;
    public string reward;
    public Cardlist(string CardNumber)
    {
        cardNumber = CardNumber;
    }
}



// Subclass with one more propertie "reward"
class Dunderkatt<T> : Cardlist<T>
{
    public Dunderkatt(string CardNumber) : base(CardNumber)
    {
        reward = "Dunderkatt";
    }
    //ToString method
    public override string ToString()
    {
        return "Grattis, du har vunnit " + reward + " guldkort.";
    }
}



// Subclass with one more propertie "reward"
class Kristallhäst<T> : Cardlist<T>
{
    public Kristallhäst(string CardNumber) : base(CardNumber)
    {
        reward = "Kristallhäst";
    }
    //ToString method
    public override string ToString()
    {
        return "Grattis, du har vunnit " + reward + " guldkort.";
    }
}


// Subclass with one more propertie "reward"
class Överpanda<T> : Cardlist<T>
{
    public Överpanda(string CardNumber) : base(CardNumber)
    {
        reward = "Överpanda";
    }
    //ToString method
    public override string ToString()
    {
        return "Grattis, du har vunnit " + reward + " guldkort.";
    }
}


// Subclass with one more propertie "reward"
class Eldtomat<T> : Cardlist<T>
{
    public Eldtomat(string CardNumber) : base(CardNumber)
    {
        reward = "Eldtomat";
    }
    //ToString method
    public override string ToString()
    {
        return "Grattis, du har vunnit " + reward + " guldkort.";
    }
}


//cards class list
class Userlist<T>
{
    // Class properties all are public strings
    public string serialNumber;
    public string name;
    public string location;
    public Userlist(string SerialNumber, string Name, string Location)
    {
        serialNumber = SerialNumber;
        name = Name;
        location = Location;
    }
    //ToString method
    public override string ToString()
    {
        return "Hej " + name + "! ";
    }
}