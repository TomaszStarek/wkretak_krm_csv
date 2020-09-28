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
//using EasyModbus;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace WindowsFormsApp5
{


    public partial class Form1 : Form
    {
        SerialPort port, port2;
        string lineReadIn;



        // this will prevent cross-threading between the serial port
        // received data thread & the display of that data on the central thread
        private delegate void preventCrossThreading(string x);
        private preventCrossThreading accessControlFromCentralThread;

        public Form1()
        {
            InitializeComponent();
            textBox1.Select();
            textBox1.KeyDown += textBox2_KeyDown;
            // textBox1.PreviewKeyDown += textBox2_PreviewKeyDown;
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



            const string com2 = "COM8";
            port2 = new SerialPort(com2, 9600, Parity.None, 8, StopBits.One);

            //   port.ErrorReceived += new SerialErrorReceivedEventHandler();
            try
            {
                port2.Open();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Error: Port " + com2 + " jest zajęty");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Uart exception: " + ex);
            }




            if (port2.IsOpen)
            {
                // set the 'invoke' delegate and attach the 'receive-data' function
                // to the serial port 'receive' event.
                // accessControlFromCentralThread = displayTextReadIn;
                port2.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived2);
                
            }


            if (port.IsOpen)
            {
                // set the 'invoke' delegate and attach the 'receive-data' function
                // to the serial port 'receive' event.
              // accessControlFromCentralThread = displayTextReadIn;
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            }

            try
            {
                port2.Write("$");
                UpdateControl(label7, Color.Red, "Wyłączony", true);
            }
            catch
            {
                MessageBox.Show("Brak połączenia z arduino", "Info", MessageBoxButtons.OK);
            }



        }


        const int pierwsza_srub = 1;
        const int druga_srub = 2;
        const int trzecia_srub = 3;
        const int czwarta_srub = 4;
        const int piata_srub = 5;
        const int szosta_srub = 6;


        string wydruk;
        //string pomocnicza;
        string[] result = new string[100];

        struct wkrecenie
        {
            public string numer;
          //  public string rezultat;

        }
        wkrecenie[] proba = new wkrecenie[7];
        
       
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


            if (result[4] == "O")
                zerowanie_ekranu();

            wyswietlanko();
            porownanie();


            if (result.Length >= 14)
                tworzeniepliku("1");






           
            lineReadIn = string.Empty;
           // pomocnicza = sn;

        }// end function 'port_dataReceived'

        string text2;

        private void port_DataReceived2(object sender, SerialDataReceivedEventArgs rcvdData)
        {

            while (port2.BytesToRead > 0)
            {
                //   lineReadIn += port.ReadExisting();

                lineReadIn += port2.ReadExisting();
                lineReadIn += Environment.NewLine;

                // lineReadIn += "\r\n";
                //   lineReadIn += lineReadIn;
                Thread.Sleep(25);
            }




            // display what we've acquired.

            text2 = lineReadIn;
            lineReadIn = string.Empty;
            // pomocnicza = sn;

        }// end function 'port_dataReceived'





        private void zerowanie_ekranu()
        {
            

            switch ((Convert.ToInt32(result[6])))
            {
                case pierwsza_srub:
                    proba[1].numer = sn;
                    break;
                case druga_srub:
                    proba[2].numer = sn;
                    break;
                case trzecia_srub:
                    proba[3].numer = sn;
                    break;
                case czwarta_srub:
                    proba[4].numer = sn;
                    break;
                case piata_srub:
                    proba[5].numer = sn;
                    break;
                case szosta_srub:
                    proba[6].numer = sn;
                    break;

                default:
                    // MessageBox.Show("Wszystko jest OK", "Info", MessageBoxButtons.OK);                   
                    break;
                    //== proba[2].numer == proba[3].numer == proba[4].numer == proba[5].numer == proba[6].numer


            }


        }


        private void porownanie()
        {
            if (proba[1].numer == proba[2].numer && proba[2].numer == proba[3].numer && proba[3].numer == proba[4].numer && proba[4].numer == proba[5].numer && proba[5].numer == proba[6].numer && !String.IsNullOrEmpty(proba[6].numer))
            {
               // label8.Visible = true;
               if(sn != "")
                UpdateControl(label8, SystemColors.HighlightText, "zeskanuj kolejną płytę", true);
                try
                {
                    port2.Write("$");
                    UpdateControl(label7, Color.Red, "Wyłączony", true);
                }
                catch
                {
                    MessageBox.Show("Brak połączenia z arduino", "Info", MessageBoxButtons.OK);
                }
                //   kolejnaplyta();
                for (int i = 1; i < 7; i++)
                    proba[i].numer = null;
                
            }
        }

    //public static class ThreadHelperClass
    //{
    //    delegate void SetTextCallback(Form f, Control ctrl, string text);

    //    /// <summary>
    //    /// Set text property of various controls
    //    /// </summary>
    //    /// <param name="form1">The calling form</param>
    //    /// <param name="ctrl"></param>
    //    /// <param name="text"></param>
    //    public static void SetText(Form form, Control ctrl, string text)
    //    {
    //        // InvokeRequired required compares the thread ID of the 
    //        // calling thread to the thread ID of the creating thread. 
    //        // If these threads are different, it returns true. 
    //        if (ctrl.InvokeRequired)
    //        {
    //            SetTextCallback d = new SetTextCallback(SetText);
    //            form.Invoke(d, new object[] { form, ctrl, text });
    //        }
    //        else
    //        {
    //            ctrl.Text = text;
    //        }
    //    }
    //}

    public bool ControlInvokeRequired(Control c, Action a)
        {
            if (c.InvokeRequired) c.Invoke(new MethodInvoker(delegate { a(); }));
            else return false;

            return true;
        }
        public void UpdateControl(Control myControl, Color c,  String s, bool widzialnosc)
        {
            //Check if invoke requied if so return - as i will be recalled in correct thread
            if (ControlInvokeRequired(myControl, () => UpdateControl(myControl,c,s,widzialnosc))) return;
            myControl.Text = s;
            myControl.BackColor = c;
            myControl.Visible = widzialnosc;
        }




        private void kolejnaplyta()
        {
                UpdateControl(s1, SystemColors.Control, string.Empty, true);
                UpdateControl(s2, SystemColors.Control, string.Empty, true);
                UpdateControl(s3, SystemColors.Control, string.Empty, true);
                UpdateControl(s4, SystemColors.Control, string.Empty, true);
                UpdateControl(s5, SystemColors.Control, string.Empty, true);
                UpdateControl(s6, SystemColors.Control, string.Empty, true);         
        }

    private void wyswietlanko()
    {
        if (result[6] == "1")
            {
              //  s1.BackColor = SystemColors.Control;
              //  s1.Text = string.Empty;
                UpdateControl(s1, SystemColors.Control, string.Empty, true);

                string pomocnicza = string.Empty;
                for (int i = 1; i< 15; i++)
                {
                    pomocnicza += result[i] + Environment.NewLine;   
                }
              //  ThreadHelperClass.SetText(this, s1, pomocnicza);

                if (result[4] == "O")
                    UpdateControl(s1, Color.Green, pomocnicza, true);
                else
                    UpdateControl(s1, Color.Red, pomocnicza, true);
            }

            if (result[6] == "2")
            {
                UpdateControl(s2, SystemColors.Control, string.Empty, true);

                string pomocnicza = string.Empty;
                for (int i = 1; i< 15; i++)
                {
                    pomocnicza += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    UpdateControl(s2, Color.Green, pomocnicza, true);
                else
                    UpdateControl(s2, Color.Red, pomocnicza, true);
            }

            if (result[6] == "3")
            {
                UpdateControl(s3, SystemColors.Control, string.Empty, true);

                string pomocnicza = string.Empty;
                for (int i = 1; i< 15; i++)
                {
                    pomocnicza += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    UpdateControl(s3, Color.Green, pomocnicza, true);
                else
                    UpdateControl(s3, Color.Red, pomocnicza, true);
            }

            if (result[6] == "4")
            {
                UpdateControl(s4, SystemColors.Control, string.Empty, true);

                string pomocnicza = string.Empty;
                for (int i = 1; i < 15; i++)
                {
                    pomocnicza += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    UpdateControl(s4, Color.Green, pomocnicza, true);
                else
                    UpdateControl(s4, Color.Red, pomocnicza, true);
            }

            if (result[6] == "5")
            {
                UpdateControl(s5, SystemColors.Control, string.Empty, true);

                string pomocnicza = string.Empty;
                for (int i = 1; i < 15; i++)
                {
                    pomocnicza += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    UpdateControl(s5, Color.Green, pomocnicza, true);
                else
                    UpdateControl(s5, Color.Red, pomocnicza, true);
            }

            if (result[6] == "6")
            {
                UpdateControl(s6, SystemColors.Control, string.Empty, true);

                string pomocnicza = string.Empty;
                for (int i = 1; i < 15; i++)
                {
                    pomocnicza += result[i] + Environment.NewLine;
                }

                if (result[4] == "O")
                    UpdateControl(s6, Color.Green, pomocnicza, true);
                else
                    UpdateControl(s6, Color.Red, pomocnicza, true);
            }
}






        // this, hopefully, will prevent cross threading.
        //private void displayTextReadIn(string ToBeDisplayed)          //wyswietlanie sygnalu na drugim texboxie
        //{
        //    if (s1.InvokeRequired)
        //        s1.Invoke(accessControlFromCentralThread, ToBeDisplayed);
        //    else
        //        s1.Text = ToBeDisplayed;

        //}

        //       private void operacje_na_danych(string ToBeDisplayed)          //wyswietlanie sygnalu na drugim texboxie
        //       {
        //           if (s1.InvokeRequired)
        //               s1.Invoke(accessControlFromCentralThread, ToBeDisplayed);
        //           else
        //           {
        ////               Dane(ToBeDisplayed);
        //               s1.Text = ToBeDisplayed;
        //           }

        //       }

        DateTime stop;
        //---------------------------------------------------------------------------------------------


        string sn;
  

        private void textBox2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Determine whether the key entered is the F1 key. If it is, display Help.
            if (e.KeyCode == Keys.Enter)
            {
                sn = textBox1.Text;
                if (sprawdzeniekrok(sn) == 1)
                {
                    UpdateControl(textBox2, SystemColors.Window, sn, true);
                    UpdateControl(textBox1, SystemColors.Window, string.Empty, true);
                    UpdateControl(label8, SystemColors.Control, string.Empty, false);
                    try
                    {
                        port2.Write("b");
                        UpdateControl(label7, Color.Green, "Włączony", true);
                    }
                    catch
                    {
                        MessageBox.Show("Brak połączenia z arduino", "Info", MessageBoxButtons.OK);
                    }
                }
                else
                    textBox1.Text = String.Empty;
            }
        }
        // Display a pop-up Help topic to assist the user.
        //  Help.ShowPopup(textBox2, "Enter your name.", new Point(textBox2.Bottom, textBox2.Right));



        const int M_NIENARODZONY = 1;
        const int M_BRAK_KROKU = 2;
        const int M_FAIL = 3;
        const int M_BRAK_POLACZENIA_Z_MES = 4;


        public int sprawdzanieMES(string SerialTxt)
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

                 var Test = Result.Tables[0].TableName;
                if (Test != "BoardHistory") return M_NIENARODZONY; //numer produktu nie widnieje w systemie MES
                               

                var data = (from row in Result.Tables["BoardHistory"].AsEnumerable()
                            where row.Field<string>("Test_Process").ToUpper() == "FVT / FVT_INNER_SEAL".ToUpper() || row.Field<string>("Test_Process").ToUpper() == "FVT / FVT_INNER_SEAL".ToUpper()
                            select new
                            {
                                TestProcess = row.Field<string>("Test_Process"),
                                TestType = row.Field<string>("TestType"),
                                TestStatus = row.Field<string>("TestStatus"),
                                StartDateTime = row.Field<DateTime>("StartDateTime"),
                                StopDateTime = row.Field<DateTime>("StopDateTime"),
                            }).FirstOrDefault();


                if (data != null)
                {
                    //sprawdzamy PASS w poprzednim kroku
                    if ("PASS" == data.TestStatus.ToUpper()) return 0; //wszystko jest OK
                    else return M_FAIL;
                }
                else return M_BRAK_KROKU; //brak poprzedniego kroku
            }
        }




        private int sprawdzeniekrok(string sn)
        {
            int Result;

            Result = sprawdzanieMES(sn); //przykladowy numer seryjny 9100000668
            switch (Result)
            {
                case M_BRAK_POLACZENIA_Z_MES:
                    MessageBox.Show("Brak połączenia z MES.", "Info", MessageBoxButtons.OK);
                    //label8.Text = "Brak połączenia z MES.";
                    break;

                case M_NIENARODZONY:
                    MessageBox.Show("Numer nienarodzony w MES.", "Info", MessageBoxButtons.OK);
                    //label8.Text = "Numer nienarodzony w MES.";
                    break;

                case M_BRAK_KROKU:
                    MessageBox.Show("Brak poprzedniego kroku.", "Info", MessageBoxButtons.OK);
                   // label8.Text = "Brak poprzedniego kroku.";
                    break;

                case M_FAIL:
                    MessageBox.Show("Poprzedni krok = FAIL.", "Info", MessageBoxButtons.OK);
                  //  label8.Text = "Poprzedni krok = FAIL.";
                    break;

                default:
                   //  MessageBox.Show("Wszystko jest OK", "Info", MessageBoxButtons.OK);                   
                    return 1;
            }           
            return 0;
        }


        private void tworzeniepliku(string serial)
        {
            string sciezka = ("C:/tars/");      //definiowanieścieżki do której zapisywane logi
            stop = DateTime.Now;
            if (Directory.Exists(sciezka))       //sprawdzanie czy sciezka istnieje
            {
                ;
            }
            else
                System.IO.Directory.CreateDirectory(sciezka); //jeśli nie to ją tworzy
            if(sn!=null)
            sn = Regex.Replace(sn, @"\s+", string.Empty);
            result[6] = Regex.Replace(result[6], @"\s+", string.Empty);
            using (StreamWriter sw = new StreamWriter("C:/tars/" + sn + "-" + result[6] + "-" + "(" + stop.Day + "-" + stop.Month + "-" + stop.Year  + " "+ stop.Hour + "-" + stop.Minute + "-" + stop.Second + ")" + ".Tars"))
            {
                

                sw.WriteLine("S{0}", sn);
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
            //porownanie();


            kolejnaplyta();
            //UpdateControl(s1, SystemColors.Control, string.Empty, true);
            //UpdateControl(s2, SystemColors.Control, string.Empty, true);
            //UpdateControl(s3, SystemColors.Control, string.Empty, true);
            //UpdateControl(s4, SystemColors.Control, string.Empty, true);
            //UpdateControl(s5, SystemColors.Control, string.Empty, true);
            //UpdateControl(s6, SystemColors.Control, string.Empty, true);

            //

            //UpdateControl(textBox1, SystemColors.Window, sn, true);

            // if (textBox1.TextLength > 27)
            // textBox1.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                port2.Write("b");
                UpdateControl(label7, Color.Green, "Włączony", true);
            }
            catch
            {
                MessageBox.Show("Brak połączenia z arduino", "Info", MessageBoxButtons.OK);
            }
            
            Thread.Sleep(200);
           // label7.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                port2.Write("$");
                UpdateControl(label7, Color.Red, "Wyłączony", true);
            }
            catch
            {
                MessageBox.Show("Brak połączenia z arduino", "Info", MessageBoxButtons.OK);
            }
            
            Thread.Sleep(200);
            // label7.Text = "";
        }

        private void label7_Click(object sender, EventArgs e)
        {
            UpdateControl(s1, SystemColors.Control, string.Empty, true);

            string pomocnicza = string.Empty;
            for (int i = 1; i < 15; i++)
            {
                pomocnicza += i + Environment.NewLine;
            }
            //  ThreadHelperClass.SetText(this, s1, pomocnicza);

            if (result[4] == "O")
                UpdateControl(s1, Color.Green, pomocnicza, true);
            else
                UpdateControl(s2, Color.Green, pomocnicza, true);
        }
    }
}
