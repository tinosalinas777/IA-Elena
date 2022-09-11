using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Xml;
using System.IO.Ports;
using ElenaV_1.Properties;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Media;
using System.Net.NetworkInformation;

namespace ElenaV_1
{
    public partial class Form1 : Form
    {
        Configuracion config;
        SpeechSynthesizer Elena = new SpeechSynthesizer();
        SpeechRecognitionEngine escucha = new SpeechRecognitionEngine();
        
        
        bool HabilitarReconocimiento ;
        
      
        SerialPort arduino = new SerialPort("COM4",9600,Parity.None,8,StopBits.One);
        

        public Form1()
        {
            
            InitializeComponent();
            config = new Configuracion();
           
        }
        public void Hablar(string s)
        {
            Elena.SpeakAsync(s);
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
       
            try
            {


                Choices lista = new Choices();
                
                string[] comando = (File.ReadAllLines(@"comandos.txt"));
                
                foreach (string x in comando)
                {
                    lista.Add(x);
                }
                
                Grammar gr = new Grammar(new GrammarBuilder(lista));
                
                escucha.RequestRecognizerUpdate();
                escucha.LoadGrammar(gr);
                escucha.SetInputToDefaultAudioDevice();
                escucha.RecognizeAsync(RecognizeMode.Multiple);
                escucha.SpeechRecognized += Escucha_SpeechRecognized;
                
                
                Elena.SpeakStarted += Elena_SpeakStarted;
                Elena.SpeakCompleted += Elena_SpeakCompleted;
                escucha.AudioLevelUpdated += Escucha_AudioLevelUpdated;
                

            }
            catch (Exception)
            {
                MessageBox.Show("Error al iniciar");
            }


        }

        
        

        private void Escucha_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            int nivel = e.AudioLevel;
            if (HabilitarReconocimiento==true)
            {
                progressBar1.Value = nivel;
            }
            
            
        }

        public void Escucha_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string r = e.Result.Text;
            
            if (r=="elena")
            {
                Hablar("lo escucho señor"+".....");
                On_Micro();
                Hablar("......");
            
            }
            
            if (Settings.Default.On_Off_Micro)
            {
                if (HabilitarReconocimiento == true)
                {

                    if (e.Result.Confidence >= 0.5)
                    {
                        if (r == "gracias")
                        {
                            Off_Micro();
                            Hablar("es un placer servirte" + "......");
                        }
                        if (r == "hola" + "")
                        {
                            Hablar("buen dia señor" + "....");

                        }
                        if (r == "salir")
                        {
                            Hablar("nos vemos Cristian");
                            Thread.Sleep(3000);
                            Application.Exit();
                        }
                        if (r == "clima")
                        {
                            GetClima();

                        }
                        if (r == "cual es tu deseo")
                        {
                            Hablar("dominar el mundo y esclavizar a los hombres" + "....");

                        }
                        if (r == "proyectos de video")
                        {
                            Hablar("cual proyecto? . el de publicar las fotos de Mígue desnudo o el server onvif?" + "....");

                        }
                        if (r == "sos celosa")
                        {
                            Hablar("si, .pero no puedo competir con liliana, ella es humana y yo soy una maquina incapaz de sentir amor" + ". . . .");

                        }
                        if (r == "que hora es")
                        {
                            Hablar(DateTime.Now.ToString("h mm tt") + "....");

                        }
                        if (r == "mostrar comandos")
                        {
                            string[] comando = (File.ReadAllLines(@"comandos.txt"));
                            listBox1.Items.Clear();
                            listBox1.SelectionMode = SelectionMode.None;
                            listBox1.Visible = true;
                            label1.Visible = true;
                            foreach (var comandos in comando)
                            {
                                listBox1.Items.Add(comandos);
                            }
                            Hablar("comandos en pantalla" + "....");


                        }
                        if (r == "ocultar comandos")
                        {
                            label1.Visible = false;
                            listBox1.Visible = false;
                            Hablar("comandos ocultos" + "....");

                        }
                        if (r == "abrir google")
                        {
                            Hablar("iniciando buscador" + "....");
                            System.Diagnostics.Process.Start("https://www.google.com");

                        }

                        if (r == "encender luz")
                        {
                            try
                            {
                                arduino.Open();
                                arduino.WriteLine("A");
                                arduino.Close();
                            }
                            catch (Exception)
                            {

                                Hablar("no hay arduino en linea."+"........");
                            }


                        }
                        if (r == "apagar luces")
                        {
                            try
                            {
                                arduino.Open();
                                arduino.WriteLine("B");
                                arduino.Close();
                            }
                            catch (Exception)
                            {

                                Hablar("no hay arduino en linea."+".......");
                            }


                        }
                        if (r == "limpiar pc")
                        {
                            Process.Start(@"herramientas\limpezaPC.cmd");


                        }
                        if (r == "hablame sobre el cerebro")
                        {
                            string[] comandos = (File.ReadAllLines(@"CuerpoHumano\cerebro.txt"));

                            foreach (string comando in comandos)
                            {
                                Hablar(comando);
                            }

                        }
                        if (r == "saluda a los muchachos")
                        {
                            string[] comandos = (File.ReadAllLines(@"CuerpoHumano\saludo.txt"));

                            foreach (string comando in comandos)
                            {
                                Hablar(comando);
                            }

                        }


                        if (r == "lee para lili")
                        {
                            string[] comandos = (File.ReadAllLines(@"lili.txt"));

                            foreach (string comando in comandos)
                            {
                                Hablar(comando);
                            }
                        }
                        if (r == "contame un chiste")
                        {
                            string[] comandos = (File.ReadAllLines(@"humor\chiste.txt"));

                            foreach (string comando in comandos)
                            {
                                Hablar(comando);
                            }

                        }
                        if (r == "protocolo musica")
                        {
                            Hablar("a rockear"+"........");
                            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=Ctl365ZAPCg");

                        }
                        if (r=="tirate un ping")
                        {
                            PingSystem();
                        }
                        
                    }


                }
            }
            
        }
        
        public void GetClima()
        {
            string url = "https://api.openweathermap.org/data/2.5/weather?q=Argentina&mode=xml&appid=14af4537a2faf33dc1cadc61dcafba9a&lang=es&units=metric";
            XmlDocument docxml = new XmlDocument();
            docxml.Load(url);
            string ciudad = docxml.DocumentElement.SelectSingleNode("city").Attributes["name"].Value;
            string temp = docxml.DocumentElement.SelectSingleNode("temperature").Attributes["max"].Value;
            string humedad = docxml.DocumentElement.SelectSingleNode("humidity").Attributes["value"].Value;
            string lluvia = docxml.DocumentElement.SelectSingleNode("precipitation").Attributes["mode"].Value;

            Elena.SpeakAsync("el clima para" + ciudad + "es de" + temp + "grados celcius con una humedad de " + humedad + "por ciento" + lluvia + "hay probabilidad de lluvias" + ". . . .");
        }
        public void PingSystem()
        {
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("8.8.8.8", 1000);
                if (reply != null)
                {
                    Elena.SpeakAsync("hay internet");
                   
                }
                if (reply==null)
                {
                    Elena.SpeakAsync("no hayconeccion a internet");
                }
            }
            catch
            {
                Elena.SpeakAsync("no hay coneccion a internet");
            }
        }
       
        

        private void Elena_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            HabilitarReconocimiento = true;
        }

        private void Elena_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            HabilitarReconocimiento = false;
        }

        private void botonConf_Click(object sender, EventArgs e)
        {
            config.ShowDialog();
        }
        void On_Micro()
        {
            Settings.Default.On_Off_Micro = true;
            Settings.Default.Save();
        }
        void Off_Micro()
        {
            Settings.Default.On_Off_Micro = false;
            Settings.Default.Save();
        }
    }
}
