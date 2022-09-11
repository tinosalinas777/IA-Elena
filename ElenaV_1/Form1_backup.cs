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
using System.IO;
using ElenaV_1.Properties;

namespace ElenaV_1
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer Elena = new SpeechSynthesizer();
        SpeechRecognitionEngine escucha = new SpeechRecognitionEngine();
        bool HabilitarReconocimiento = true;
        string VozSeleccionada="Microsoft Helena Desktop";
        

        public Form1()
        {
           
            
            InitializeComponent();
            CargarVoces();
        }
        public void Hablar(string s)
        {
            Elena.SpeakAsync(s);
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            
            if (Settings.Default.VozDefault != null)
            {
                Elena.SelectVoice(Settings.Default.VozDefault);
                VozSeleccionada = Settings.Default.VozDefault;
            }

            Choices lista = new Choices();
            string[] comando = new string[] { "abrir google","hola", "como te llamas", "clima", "que hora es", "sos celosa", "salir", "mostrar comandos","ocultar comandos" };
            lista.Add(comando);
            Grammar gr = new Grammar(new GrammarBuilder(lista));

            try
            {
                
                escucha.RequestRecognizerUpdate();
                escucha.SetInputToDefaultAudioDevice();
                escucha.LoadGrammar(gr);
                escucha.SpeechRecognized += Escucha_SpeechRecognized;
                Elena.SpeakStarted += Elena_SpeakStarted;
                Elena.SpeakCompleted += Elena_SpeakCompleted;
                escucha.AudioLevelUpdated += Escucha_AudioLevelUpdated;
                escucha.RecognizeAsync(RecognizeMode.Multiple);

            }
            catch (Exception)
            {
                MessageBox.Show("Error al iniciar");
            }


        }
        void VozDeSalida(string t)
        {
            if (VozSeleccionada!=null)
            {
                Elena.SelectVoice(VozSeleccionada);
                Hablar(t);
            }
            MessageBox.Show("Debe selecionar una voz");
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
            if (HabilitarReconocimiento==true)
            {
                string r = e.Result.Text;
                if (r == "hola")
                {
                    VozDeSalida("buen dia señor"+"....");
                }
                if (r == "salir")
                {
                    Application.Exit();
                }
                if (r=="clima")
                {
                    GetClima();
                }
                if (r=="como te llamas")
                {
                    Hablar("me llamo elena y soy un programa con inteligencia artificial" + "....");
                }
                if (r=="sos celosa")
                {
                    Hablar("si, pero no puedo competir con liliana ella te da amor y yo soy una maquina incapaz de sentir amor" + ". . . .");
                }
                if (r=="que hora es")
                {
                    Hablar(DateTime.Now.ToString("h mm tt")+"....");
                }
                if (r=="mostrar comandos")
                {
                    string[] comando = new string[] { "abrir google", "hola", "como te llamas", "clima", "que hora es", "sos celosa", "salir", "mostrar comandos", "ocultar comandos" };
                    listBox1.Items.Clear();
                    listBox1.SelectionMode = SelectionMode.None;
                    listBox1.Visible = true;
                    label1.Visible = true;
                    foreach (var comandos in comando)
                    {
                        listBox1.Items.Add(comandos);
                    }
                    Hablar("comandos en pantalla"+"....");
                    
                }
                if (r=="ocultar comandos")
                {
                    label1.Visible = false;
                    listBox1.Visible = false;
                    Hablar("comandos ocultos");
                }
                if (r=="abrir google")
                {
                    Hablar("iniciando buscador"+"....");
                    System.Diagnostics.Process.Start("https://www.google.com");
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
        public void CargarVoces()
        {
            foreach (InstalledVoice voz in Elena.GetInstalledVoices())
            {
                if (Elena.GetInstalledVoices()!=null)
                {
                    comboBox1.Items.Add(voz.VoiceInfo.Name);
                }
                

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

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            VozSeleccionada = comboBox1.Text;
            Settings.Default.VozDefault = comboBox1.Text;
            Settings.Default.Save();
        }
    }
}
