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
using EasyModbus;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace WindowsFormsApp5
{
    

    public partial class Form1 : Form
    {
        SerialPort port;
        string lineReadIn;

        

        // this will prevent cross-threading between the serial port
        // received data thread & the display of that data on the central thread
        private delegate void preventCrossThreading(string x);
        private preventCrossThreading accessControlFromCentralThread;

        public Form1()
        {
            InitializeComponent();
            
            // create and open the serial port (configured to my machine)
            // this is a Down-n-Dirty mechanism devoid of try-catch blocks and
            // other niceties associated with polite programming
            const string com = "COM7";
            port = new SerialPort(com, 9600, Parity.None, 8, StopBits.One);

            //   port.ErrorReceived += new SerialErrorReceivedEventHandler();
            try
            {
                port.Open();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Error: Port " + com + " jest zajęty");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Uart exception: " + ex);
            }


 

            if (port.IsOpen)
            {
                // set the 'invoke' delegate and attach the 'receive-data' function
                // to the serial port 'receive' event.
                accessControlFromCentralThread = displayTextReadIn;
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            }
        }

      

        private void button1_Click(object sender, EventArgs e)
        {



        }

        private void zerowanieRS()
        {

            // clear the RCVbox text string and write the VER command
            s1.Text = lineReadIn = string.Empty;
            port.Write("VER\r");
        }

        string wydruk;
        string[] result = new string[100];

        // this is called when the serial port has receive-data for us.
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs rcvdData)
        {

            while (port.BytesToRead > 0)
            {
                //   lineReadIn += port.ReadExisting();

                lineReadIn += port.ReadExisting();
                lineReadIn += Environment.NewLine;

                // lineReadIn += "\r\n";
                //   lineReadIn += lineReadIn;
                Thread.Sleep(25);
            }
            
            
            

            // display what we've acquired.

            wydruk = lineReadIn;
            
            result = wydruk.Split(',');

            for (int i = 0; i < 15; i++)
            {
                result[i] = Regex.Replace(result[i], @"\s+", string.Empty);
                
            }

            if (result[6] == "1")
            {
               // Thread.Sleep(3000);
                s1.Text = string.Empty;
                s2.Text = string.Empty;
                s3.Text = string.Empty;
                s4.Text = string.Empty;
                s5.Text = string.Empty;
                s6.Text = string.Empty;

                s1.BackColor = SystemColors.Control;
                s2.BackColor = SystemColors.Control;
                s3.BackColor = SystemColors.Control;
                s4.BackColor = SystemColors.Control;
                s5.BackColor = SystemColors.Control;
                s6.BackColor = SystemColors.Control;

            }


            if (result[6] == "1")
            {
                for (int i = 0; i < 15; i++)
                {
                    s1.Text += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    s1.BackColor = Color.Green;
                else
                    s1.BackColor = Color.Red;
            }

            if (result[6] == "2")
            {
                for (int i = 0; i < 15; i++)
                {
                    s2.Text += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    s2.BackColor = Color.Green;
                else
                    s2.BackColor = Color.Red;
            }

            if (result[6] == "3")
            {
                for (int i = 0; i < 15; i++)
                {
                    s3.Text += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    s3.BackColor = Color.Green;
                else
                    s3.BackColor = Color.Red;
            }

            if (result[6] == "4")
            {
                for (int i = 0; i < 15; i++)
                {
                    s4.Text += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    s4.BackColor = Color.Green;
                else
                    s4.BackColor = Color.Red;
            }

            if (result[6] == "5")
            {
                for (int i = 0; i < 15; i++)
                {
                    s5.Text += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    s5.BackColor = Color.Green;
                else
                    s5.BackColor = Color.Red;
            }

            if (result[6] == "6")
            {
                for (int i = 0; i < 15; i++)
                {
                    s6.Text += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                {
                    s6.BackColor = Color.Green;
                }
                else
                    s6.BackColor = Color.Red;
            }



            if (result.Length >= 14)
                tworzeniepliku("1");

            lineReadIn = string.Empty;


        }// end function 'port_dataReceived'



        // this, hopefully, will prevent cross threading.
        private void displayTextReadIn(string ToBeDisplayed)          //wyswietlanie sygnalu na drugim texboxie
        {
            if (s1.InvokeRequired)
                s1.Invoke(accessControlFromCentralThread, ToBeDisplayed);
            else
                s1.Text = ToBeDisplayed;
            
        }

        private void operacje_na_danych(string ToBeDisplayed)          //wyswietlanie sygnalu na drugim texboxie
        {
            if (s1.InvokeRequired)
                s1.Invoke(accessControlFromCentralThread, ToBeDisplayed);
            else
            {
 //               Dane(ToBeDisplayed);
                s1.Text = ToBeDisplayed;
            }

        }

        DateTime stop;
        //---------------------------------------------------------------------------------------------

        

        const int M_BRAK_POLACZENIA_Z_MES = 4;




        public int Test1(string SerialTxt)
        {
            using (MESwebservice.BoardsSoapClient wsMES = new MESwebservice.BoardsSoapClient("BoardsSoap"))
            {
                DataSet Result;
                try
                {
                    Result = wsMES.GetBoardHistoryDS(@"itron", SerialTxt);
                }
                catch
                {
                    return M_BRAK_POLACZENIA_Z_MES;
                }

                
            }
                return 1;
        }




        private int sprawdzeniekrok(string sn)
        {
            int Result;

            Result = Test1(sn); //przykladowy numer seryjny 9100000668
            switch (Result)
            {
                case M_BRAK_POLACZENIA_Z_MES:
                   // MessageBox.Show("Brak połączenia z MES.", "Info", MessageBoxButtons.OK);
                    break;

                default:
                   // MessageBox.Show("Wszystko jest OK", "Info", MessageBoxButtons.OK);                   
                    return 1;
            }
            MessageBox.Show("Brak połączenia z MES.", "Info", MessageBoxButtons.OK);
            return 0;
        }


        private void tworzeniepliku(string sn)
        {
            string sciezka = ("C:/tars/");      //definiowanieścieżki do której zapisywane logi
            stop = DateTime.Now;
            if (Directory.Exists(sciezka))       //sprawdzanie czy sciezka istnieje
            {
                ;
            }
            else
                System.IO.Directory.CreateDirectory(sciezka); //jeśli nie to ją tworzy

            result[0] = Regex.Replace(result[0], @"\s+", string.Empty);
            result[6] = Regex.Replace(result[6], @"\s+", string.Empty);
            using (StreamWriter sw = new StreamWriter("C:/tars/" + result[0] + "-" + result[6] + "-" + "(" + stop.Day + "-" + stop.Month + "-" + stop.Year  + " "+ stop.Hour + "-" + stop.Minute + "-" + stop.Second + ")" + ".Tars"))
            {
                

                sw.WriteLine("S{0}", result[0]);
                sw.WriteLine("CITRON");
                sw.WriteLine("NPLKWIM0P19B3M01_" + result[6]);
                sw.WriteLine("PQC_SCREW" + result[6]);
                sw.WriteLine("Ooperator");

                result[1] = Regex.Replace(result[1], @"\s+", string.Empty);
                sw.WriteLine("[" + result[1].Remove(10) + " " + result[1].Remove(0, 10));
                //  sw.WriteLine("]" + stop.Year + "-" + stop.Month + "-" + stop.Day + " " + stop.Hour + ":" + stop.Minute + ":" + stop.Second);
                sw.WriteLine("]" + stop.ToString("yyyy-MM-dd HH:mm:ss"));
                if (result[4] == "O")
                    sw.WriteLine("TP");
                else
                    sw.WriteLine("TF");

                sw.WriteLine("MResult_" + result[6]);
                sw.WriteLine("d" + result[5].Trim());
                sw.WriteLine("MCount_" + result[6]);
                sw.WriteLine("d" + result[6].Trim());
                sw.WriteLine("MBatch_" + result[6]);
                sw.WriteLine("d" + result[7].Trim());
                sw.WriteLine("MTorque Actual_" + result[6]);
                sw.WriteLine("d" + result[8]);
                result[9] = Regex.Replace(result[9], @"\s+", string.Empty);
                sw.WriteLine("MTorque Min_" + result[6]);
                sw.WriteLine("d" + result[9]);
                sw.WriteLine("MTorque Max_" + result[6]);
                sw.WriteLine("d" + (result[10])  );
                sw.WriteLine("MTorque Units_" + result[6]);
                sw.WriteLine("d" + result[11]);
                sw.WriteLine("MAngle_" + result[6]);
                sw.WriteLine("d" + result[12]);
                sw.WriteLine("MAngle Min_" + result[6]);
                sw.WriteLine("d" + result[13] );
                sw.WriteLine("MAngle Max_" + result[6]);
                sw.WriteLine("d" + result[14]);


                //for (int i = 0; i > 15; i++)
                //    result[i] = string.Empty;

            }


        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //1
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label2.Text = "0:" + result[0] + "\n" + "1:" + result[1] + "\n" + "2:" + result[2] + "\n" + "3:" + result[3] + "\n" + "4:" + result[4] + "\n" + "5:" + result[5] + "\n" + "6:" + result[6] + "\n" + "7:" + result[7] + "\n";
        }

    }
}
