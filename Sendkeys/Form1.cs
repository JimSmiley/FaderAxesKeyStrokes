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
        public static string[] data = new string[] { "0", "0", "0", "0", "0"};  
    
        
        public static string[] oldData = new string[] { "0", "0","0", "0", "0" };


        public static string ports = "COM11"; // rename your default port here
        public static string sliderOneUp = "{LEFT}"; // default shortcut
        public static string sliderOneDown = "{RIGHT}"; // default shortcut
        public static string sliderTwoUp = "{UP}"; // default shortcut
        public static string sliderTwoDown = "{DOWN}"; // default shortcut
        public static string rotaryUp = "{UP}"; // default shortcut
        public static string rotaryDown = "{DOWN}";
        public static string rotaryButton = "a"; // default shortcut
        public static string topButton = "b"; // default shortcut
       
        public static string program ="unity";


        Thread MainThread;
       public SerialPort sp;
    

       
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
            updateText(); // just monitors the serial strings and displays in textbox. 
        }
        
        
        ///////////// Here is all the bring window to front stuff that is no longer working. Probably a better way to do this....

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
           string lpWindowName);

       
        [DllImport("USER32.DLL")]
        static extern int SetForegroundWindow(IntPtr point);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        Process p = Process.GetProcessesByName(program).FirstOrDefault();  // this may kill the if statement in the loop. Not sure why it was null for you...
        
        private void button1_Click(object sender, EventArgs e)  // keys enable toggle
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
            else
            {
                Debug.Write("P is NULL!!!");
            }

          
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)  // event handler on serial portrecieve.
        {
            SerialPort sport = (SerialPort)sender;
            string indata = sport.ReadLine();
           // Debug.Print(indata);
           
            string[] splits = indata.Split(' ');
          //  Debug.Print(splits.Length.ToString());
           
            for (int i = 0; i< 5; i++)
            {
                data[i] = splits[i];
                
                
            }
            //updateText(); // can't call from this thread?
            
        }




        public void Loop()
        { // we are on a new thread here. use this for serial read
            while (true)
            {

                if (keysEnabled)
                {
                    if (p != null)  // get rid of this?
                    {
                        /////////////////////////////////////////////////////////////////////////  First Slider up
                        if (Int16.Parse(data[0]) > Int16.Parse(oldData[0]))
                        {
                            if (Int16.Parse(data[0]) - Int16.Parse(oldData[0]) < 50) // removes little jitter after scrolling
                            {

                                SendKeys.SendWait(sliderOneUp);
                                Debug.Write("SentKEYS!!");
                            }
                            oldData[0] = data[0];
                        }
                        else if ((Int16.Parse(data[0]) < Int16.Parse(oldData[0]))) ///////////////// first slider down

                        {
                            if (Int16.Parse(oldData[0]) - Int16.Parse(data[0]) < 50)

                                SendKeys.SendWait(sliderOneDown);
                        }

                        oldData[0] = data[0];
                    }

                    if (Int16.Parse(data[1]) > Int16.Parse(oldData[1])) // second slider up
                    {
                        if (Int16.Parse(data[1]) - Int16.Parse(oldData[1]) < 50)
                        {

                            SendKeys.SendWait(sliderTwoUp);


                        }
                        oldData[1] = data[1];
                    }
                    else if ((Int16.Parse(data[1]) < Int16.Parse(oldData[1]))) // second slider down
                    {
                        if (Int16.Parse(oldData[1]) - Int16.Parse(data[1]) < 50)
                        {

                            SendKeys.SendWait(sliderTwoDown);

                        }
                        oldData[1] = data[1];
                    }
                    if (Int16.Parse(data[2]) > Int16.Parse(oldData[2])) // rotary up. Will need to remove the 24 steps in the arduino code.
                    {

                        SendKeys.SendWait(rotaryUp);


                        oldData[2] = data[2];

                    }
                    if (Int16.Parse(data[2]) < Int16.Parse(oldData[2])) // rotary down. Will need to remove the 24 steps in the arduino code.
                    {

                        SendKeys.SendWait(rotaryDown);


                        oldData[2] = data[2];

                    }
                    if (Int16.Parse(data[3]) != Int16.Parse(oldData[3])) // button Press
                    {
                        if (Int16.Parse(data[3]) == 0)
                        {
                            SendKeys.SendWait(rotaryButton);
                        }

                        oldData[3] = data[3];
                    }




                    if (Int16.Parse(data[4]) != Int16.Parse(oldData[4])) // button Press
                    {
                        if (Int16.Parse(data[4]) == 0)
                        {
                            SendKeys.SendWait(topButton);
                        }
                        oldData[4] = data[4];


                    } //



                }
            }
        }

        public void updateText()   // displays values in textbox
        {
            textBox1.Text = $"{data[0]} {data[1]} {data[2]} {data [3]} {data[4] }";
        }

        private void textBox2_TextChanged(object sender, EventArgs e)  // change port
        {
            ports = textBox2.Text;
            
        }

        
        private void button2_Click(object sender, EventArgs e)  //open port button
        {
            sp = new SerialPort(ports, 115200);
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
            sliderOneUp = textBox7.Text;
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            sliderTwoUp = textBox17.Text;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            sliderOneDown = textBox8.Text;
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            sliderTwoDown = textBox18.Text;
        }

        private void textBox36_TextChanged(object sender, EventArgs e)
        {
            rotaryUp= textBox36.Text;
        }

        private void textBox41_TextChanged(object sender, EventArgs e)
        {
            rotaryDown = textBox41.Text;
        }

        private void textBox40_TextChanged(object sender, EventArgs e)
        {
            program = textBox40.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            rotaryButton = textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            topButton = textBox4.Text;
        }
    }
}
