using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;

namespace TempCaicedonia
{
    public partial class Form1 : Form
    {

        Point offset;
        bool isMainPanelDragged = false;
        System.IO.Ports.SerialPort puerto;
        string datos_puerto;
        int tiempoEspera = 0;
        String[] listado_puerto = System.IO.Ports.SerialPort.GetPortNames();
        bool IsOpen = false;
        String SerialDispositivo = "";
        double Ydata = 0;
        private double[] dataArray = new double[60];

        public Form1()
        {
            InitializeComponent();

            foreach (var item in listado_puerto)
            {
                bunifuDropdown1.AddItem(item);
            }
            bunifuDropdown1.selectedIndex = 0;

        }
        public void serial()
        {

            try
            {
                this.puerto = new System.IO.Ports.SerialPort("" + bunifuDropdown1.selectedValue, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                Console.WriteLine("OPEN PORT= " + bunifuDropdown1.selectedValue);
                this.puerto.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(recepcion);

            }
            catch (Exception)
            {
                showMessage("Error de puerto COM", "Verifique:" + System.Environment.NewLine + "- Voltage" + System.Environment.NewLine + "- Conexion del puerto");
            }
        }
        private void showMessage(string title, string body)
        {
            Form2 form2 = new Form2();
            form2.dataMessage(title, body);
            form2.ShowDialog();
        }
        public void recepcion(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
               // Thread.Sleep(1);
                datos_puerto = this.puerto.ReadLine();
                Console.WriteLine(datos_puerto);
               
                if (datos_puerto.StartsWith("T"))
                { this.Invoke(new EventHandler(tempertatura)); }
              

            }

            catch (Exception) { }


        }
       
        public void tempertatura(object s, EventArgs e)
        {
            datos_puerto = datos_puerto.Remove(0, 1).Replace(".", ",");
            label3.Text = datos_puerto;
            Ydata = Convert.ToDouble(datos_puerto);
            bunifuCircleProgressbar1.Value = Convert.ToInt32(Ydata);
            tiempoEspera = 0;
        }
       
        public void serial(object s, EventArgs e)
        {

            datos_puerto = datos_puerto.Remove(0, 1).Replace("\r", "");
            label17.Text = datos_puerto;
            if (label17.Text != "SERIAL")
            {

                SerialDispositivo = label17.Text;

            }

        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuSwitch1_Click_1(object sender, EventArgs e)
        {
           
            if (bunifuSwitch1.Value == true)
            {
                try
                {
                    serial();
                    puerto.Open();
                    pictureBox2.Image = TempCaicedonia.Properties.Resources.icons8_connected_64px;
                    IsOpen = true;
                    timer1.Start();



                }
                catch (Exception)
                {
                    bunifuSwitch1.Value = false;
                    showMessage("Error de puerto COM", "Verifique:" + System.Environment.NewLine + "- Voltage" + System.Environment.NewLine + "- Conexion del puerto");


                }

            }

            else
            {
                puerto.Close();
                Console.WriteLine("CLOSED PORT");
                IsOpen = false;
                pictureBox2.Image = TempCaicedonia.Properties.Resources.icons8_disconnected_64px;
                label17.Text = "SERIAL";
                timer1.Stop();

            }
          
        }

        private void heartBeat()
        {

            puerto.Write(".");

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMainPanelDragged = true;
                Point pointStartPosition = this.PointToScreen(new Point(e.X, e.Y));
                offset = new Point();
                offset.X = this.Location.X - pointStartPosition.X;
                offset.Y = this.Location.Y - pointStartPosition.Y;
            }
            else
            {
                isMainPanelDragged = false;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMainPanelDragged)
            {
                Point newPoint = panel1.PointToScreen(new Point(e.X, e.Y));
                newPoint.Offset(offset);
                this.Location = newPoint;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            isMainPanelDragged = false;
        }

        private void UpdateCpuChart()
        {
            chart2.Series[0].Points.Clear();

            for (int i = 0; i < dataArray.Length - 1; ++i)
            {
                chart2.Series[0].Points.AddY(dataArray[i]);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            dataArray[dataArray.Length - 1] = Ydata;

            Array.Copy(dataArray, 1, dataArray, 0, dataArray.Length - 1);

            if (chart2.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate { UpdateCpuChart(); });
            }
                /////////SE COMPRUEBA EL HEARBEAT///////////
                if (IsOpen)
                {
                    try
                    {
                        heartBeat();
                    }
                    catch (Exception)
                    {
                        puerto.Close();
                        bunifuSwitch1.Value = false;
                        IsOpen = false;
                        label3.Text = "Temp.";
                        label4.Text = "Temp.";
                        showMessage("CONEXIÓN PERDIDA", "Acciones:" + System.Environment.NewLine + "- Reinicie Puerto" + System.Environment.NewLine + "- Conexion del puerto");

                    }
                }
            

            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                pictureBox10.Image = TempCaicedonia.Properties.Resources.wifi;
                label15.Text = "RED";
            }
            catch (Exception)
            {
                pictureBox10.Image = TempCaicedonia.Properties.Resources.noWifi;
                label15.Text = "NO RED";
            }
        }
        private void bunifuSlider1_ValueChanged(object sender, EventArgs e)
        {
            label2.Text = bunifuSlider1.Value.ToString();
            puerto.Write("P");//PWM DATA
            puerto.WriteLine(label2.Text);
            Console.Write("P");
            Console.WriteLine(label2.Text);
        }

        private void bunifuSwitch2_Click(object sender, EventArgs e)
        {
            if (bunifuSwitch2.Value == true)
            {
                puerto.Write("D");//DIGITAL DATA
                puerto.WriteLine("1");
            }
            if (bunifuSwitch2.Value == false)
            {
                puerto.Write("D");//DIGITAL DATA
                puerto.WriteLine("0");
            }
        }

        public class Persona
        {
            public string user_code { get; set; }
            public string device_code { get; set; }
            public double temp { get; set; }
            public string fecha { get; set; }
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            puerto.Write("T");//PANTALLA DATA
            puerto.WriteLine(bunifuMaterialTextbox1.Text);
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            puerto.Write("C");//CLEAR PANTALLA DATA
        }

        private void bunifuFlatButton3_Click(object sender, EventArgs e)
        {
            chart2.Series[0].Points.Clear();
            Array.Clear(dataArray, 0, dataArray.Length);
        }

        private void bunifuSwitch3_Click(object sender, EventArgs e)
        {
            if (bunifuSwitch3.Value == true)
            {
                puerto.Write("E");//DIGITAL DATA
                puerto.WriteLine("1");
            }
            if (bunifuSwitch3.Value == false)
            {
                puerto.Write("E");//DIGITAL DATA
                puerto.WriteLine("0");
            }
        }

        private void bunifuSwitch4_Click(object sender, EventArgs e)
        {
            if (bunifuSwitch4.Value == true)
            {
                puerto.Write("F");//DIGITAL DATA
                puerto.WriteLine("1");
            }
            if (bunifuSwitch4.Value == false)
            {
                puerto.Write("F");//DIGITAL DATA
                puerto.WriteLine("0");
            }
        }

        private void bunifuSwitch16_Click(object sender, EventArgs e)
        {
            if (bunifuSwitch16.Value == true)
            {
                puerto.Write("Q");//RELE DATA
                puerto.WriteLine("1");
            }
            if (bunifuSwitch16.Value == false)
            {
                puerto.Write("Q");//RELE DATA
                puerto.WriteLine("0");
            }
        }

        private void bunifuSwitch15_Click(object sender, EventArgs e)
        {
            if (bunifuSwitch15.Value == true)
            {
                puerto.Write("R");//RELE DATA
                puerto.WriteLine("1");
            }
            if (bunifuSwitch15.Value == false)
            {
                puerto.Write("R");//RELE DATA
                puerto.WriteLine("0");
            }
        }
    }
}
