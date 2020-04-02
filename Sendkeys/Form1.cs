using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
namespace Sendkeys
{
    public partial class Form1 : Form
    {
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        public bool keysEnabled;
        public static string[] splitRead = new string[12];
        public static int[] vals = new int[12];
        public static int[] oldVals = new int[12];
        public static string[] data = new string[] { "0", "0", "0" };
        public static string[] oldData = new string[] { "0", "0","0" };
        public static string[] data2 = new string[] { "0", "0" };  // arrays for additional axes. This is my crap codiing, and will be out of bounds exce
        public static string[] oldData2 = new string[] { "0", "0" };
        public static string[] data3 = new string[] { "0", "0" };
        public static string[] oldData3 = new string[] { "0", "0" };
        public static string[] ports = new string[5] {"COM11", "", "", "", ""};
        public static string[,] sliderKeysUp = new string[5,2];
        public static string[,] sliderKeysDown = new string[5, 2];
        public static string[] rotaryUp = new string[5];
        public static string[] rotaryDown = new string[5];
        public static string program ="unity";


        Thread MainThread;
       public SerialPort sp;
       public SerialPort sp2;
       public SerialPort sp3;
       public SerialPort sp4;
        public SerialPort sp5;

       
        public Form1()
        {
            InitializeComponent();
            myTimer.Tick += new EventHandler(TimerEventProcessor);

           
            myTimer.Interval = 5;
            myTimer.Start();
            MainThread = new System.Threading.Thread(Loop);
           // Thread readThread = new Thread(Read);
            MainThread.Start();
            

            



        }

        private void TimerEventProcessor(object sender, EventArgs e)
        {
            updateText();
        }

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
           string lpWindowName);

       
        [DllImport("USER32.DLL")]
        static extern int SetForegroundWindow(IntPtr point);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        Process p = Process.GetProcessesByName(program).FirstOrDefault();
        
        private void button1_Click(object sender, EventArgs e)
        {
           // sp.Close();
            keysEnabled = !keysEnabled;
            if (keysEnabled)
            {
                button1.BackColor = Color.Green;
                button1.Text = "KEYS ENABLED";
            }
            else
            {
                button1.BackColor = Color.Red;
                button1.Text = "KEYS DISABLED";
            }
            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);  // set notepad as main window. Can do this with serial read.
               
            }

          
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)  // event handler on serial portrecieve.
        {
            SerialPort sport = (SerialPort)sender;
            string indata = sport.ReadLine();
            Debug.Print(indata);
           
            string[] splits = indata.Split(' ');
            Debug.Print(splits.Length.ToString());
           
            for (int i = 0; i< 3; i++)
            {
                data[i] = splits[i];
                
                
            }
            //updateText();
            
        }
        private void DataReceivedHandler2(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sport = (SerialPort)sender;
            string indata = sport.ReadLine();


            string[] splits = indata.Split(' ');
            Debug.Print(splits.Length.ToString());

            for (int i = 0; i < 2; i++)
            {
                data2[i] = splits[i];


            }
            //updateText();

        }
        private void DataReceivedHandler3(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sport = (SerialPort)sender;
            string indata = sport.ReadLine();


            string[] splits = indata.Split(' ');
            Debug.Print(splits.Length.ToString());

            for (int i = 0; i < 2; i++)
            {
                data3[i] = splits[i];


            }
            //updateText();

        }

        public void Loop()
        { // we are on a new thread here. use this for serial read
            while (true)
            {
                
                if (keysEnabled)
                {
                    if (p != null)
                    {
                        /////////////////////////////////////////////////////////////////////////  First Slider up
                        if (Int16.Parse(data[0]) > Int16.Parse(oldData[0]))
                        {
                            
                            if (sliderKeysUp[0, 0] != null)
                            {
                                SendKeys.SendWait(sliderKeysUp[0, 0]);
                            }
                            else
                            {
                                SendKeys.SendWait("^+(l)"); //Put your default shortcuts in here.  ^ctl, +shift, %alt (but symbols are different in unity)
                            }
                            oldData[0] = data[0];
                        }
                        else if ((Int16.Parse(data[0]) < Int16.Parse(oldData[0]))) ///////////////// first slider down

                        {
                            if (sliderKeysDown[0, 0] != null)
                            {
                                SendKeys.SendWait(sliderKeysDown[0, 0]);
                            }
                            else
                            {
                                SendKeys.SendWait("^+(h)");
                            }
                            oldData[0] = data[0];
                        }

                        if (Int16.Parse(data[1]) > Int16.Parse(oldData[1])) // second slider up
                        {
                            if (sliderKeysUp[0, 1] != null)
                            {
                                SendKeys.SendWait(sliderKeysUp[0, 1]);
                            }
                            else
                            {
                                SendKeys.SendWait("^%(l)");
                            }
                            oldData[1] = data[1];
                        }
                        else if ((Int16.Parse(data[1]) < Int16.Parse(oldData[1]))) // second slider down
                        {
                            if (sliderKeysDown[0, 1] != null)
                            {
                                SendKeys.SendWait(sliderKeysDown[0, 1]);
                            }
                            else
                            {
                                SendKeys.SendWait("^%(h)");
                            }
                            oldData[1] = data[1];
                        }
                        if (Int16.Parse(data[2]) > Int16.Parse(oldData[2])) // rotary up. Will need to remove the 24 steps in the arduino code.
                        {
                            if (rotaryUp[0] != null)
                            {
                                SendKeys.SendWait(rotaryUp[0]);
                            }
                            else
                            {
                                SendKeys.SendWait( "{UP}");
                            }
                            oldData[2] = data[2];

                        }
                        if (Int16.Parse(data[2]) < Int16.Parse(oldData[2])) // rotary down. Will need to remove the 24 steps in the arduino code.
                        {
                            if (rotaryDown[0] != null)
                            {
                                SendKeys.SendWait(rotaryDown[0]);
                            }
                            else
                            {
                                SendKeys.SendWait("{DOWN}");
                            }
                            oldData[2] = data[2];

                        }
                        /////////////////////////////////////////////////////////////////// MoveX and MoveX fine
                        if (Int16.Parse(data2[0]) > Int16.Parse(oldData2[0]))
                        {
                            SendKeys.SendWait("^+(,)");
                            oldData2[0] = data2[0];
                        }
                        else if ((Int16.Parse(data2[0]) < Int16.Parse(oldData2[0])))
                        {
                            SendKeys.SendWait("^+(;)");
                            oldData2[0] = data2[0];
                        }

                        if (Int16.Parse(data2[1]) > Int16.Parse(oldData2[1]))
                        {
                            SendKeys.SendWait("^(,)");
                            oldData2[1] = data2[1];
                        }
                        else if ((Int16.Parse(data2[1]) < Int16.Parse(oldData2[1])))
                        {
                            SendKeys.SendWait("^(;)");
                            oldData2[1] = data2[1];
                        }
                        /////////////////////////////////////////////MoveY
                         if (Int16.Parse(data3[1]) > Int16.Parse(oldData3[1]))
                        {
                            SendKeys.SendWait("^+(i)");
                            oldData3[1] = data3[1];
                        }
                        else if ((Int16.Parse(data3[1]) < Int16.Parse(oldData3[1])))
                        {
                            SendKeys.SendWait("^+(j)");
                            oldData3[1] = data3[1];
                        }

                        if (Int16.Parse(data3[0]) > Int16.Parse(oldData3[0]))
                        {
                            SendKeys.SendWait("^(i)");
                            oldData3[0] = data3[0];
                        }
                        else if ((Int16.Parse(data3[0]) < Int16.Parse(oldData3[0])))
                        {
                            SendKeys.SendWait("^(j)");
                            oldData3[0] = data3[0];
                        }

                    }
                }
                
            }
        }


        public void updateText()
        {
            textBox1.Text = $"{data[0]} {data[1]} {data[2]}";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ports[0] = textBox2.Text;
            Debug.Print(ports[0]);
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            sp = new SerialPort(ports[0], 115200);
            sp.ReadTimeout = 500;
            sp.WriteTimeout = 500;
            sp.Parity = Parity.None;
            sp.DataBits = 8;
            sp.StopBits = StopBits.One;
            sp.RtsEnable = true;
            sp.Handshake = Handshake.None;
            sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            try
            {
                sp.Open();
                Debug.Print("OPEN SUCCESSFUL");
                button2.BackColor = Color.Green;
            }
            catch (SystemException f)
            {
                MessageBox.Show(f.ToString());
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            sliderKeysUp[0,0] = textBox7.Text;
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            sliderKeysUp[0, 1] = textBox17.Text;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            sliderKeysDown[0, 0] = textBox8.Text;
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            sliderKeysDown[0, 1] = textBox18.Text;
        }

        private void textBox36_TextChanged(object sender, EventArgs e)
        {
            rotaryUp[0] = textBox36.Text;
        }

        private void textBox41_TextChanged(object sender, EventArgs e)
        {
            rotaryDown[0] = textBox41.Text;
        }

        private void textBox40_TextChanged(object sender, EventArgs e)
        {
            program = textBox40.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sp2 = new SerialPort(ports[1], 115200);
            sp2.ReadTimeout = 500;
            sp2.WriteTimeout = 500;
            sp2.Parity = Parity.None;
            sp2.DataBits = 8;
            sp2.StopBits = StopBits.One;
            sp2.RtsEnable = true;
            sp2.Handshake = Handshake.None;
            sp2.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler2);
            try
            {
                sp2.Open();
                Debug.Print("OPEN SUCCESSFUL");
                button3.BackColor = Color.Green;
            }
            catch (SystemException f)
            {
                MessageBox.Show(f.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            sp3 = new SerialPort(ports[2], 115200);
            sp3.ReadTimeout = 500;
            sp3.WriteTimeout = 500;
            sp3.Parity = Parity.None;
            sp3.DataBits = 8;
            sp3.StopBits = StopBits.One;
            sp3.RtsEnable = true;
            sp3.Handshake = Handshake.None;
            sp3.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler3);
            try
            {
                sp3.Open();
                Debug.Print("OPEN SUCCESSFUL");
                button4.BackColor = Color.Green;
            }
            catch (SystemException f)
            {
                MessageBox.Show(f.ToString());
            }
        }
    }
}
