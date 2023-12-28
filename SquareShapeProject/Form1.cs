using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SquareShapeProject
{
    public partial class Form1 : Form
    {
        private Color squareShapeColor = Color.Green;
        public Form1()
        {
            InitializeComponent();
            DrawSquareShape();
        }

        private void DrawSquareShape()
        {
            this.pictureBox1.BackColor = squareShapeColor;         
        }

        private void RecolorSquareShape(string color)
        {
            Color newColor = ColorTranslator.FromHtml(color);
            squareShapeColor = newColor;
            BeginInvoke(new MethodInvoker(() =>
            {
                DrawSquareShape();
            }));
        }

        private void StartHttpServer(string url, string newColor)
        {
            Thread thread = new Thread(async () =>
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var jsonObject = JsonConvert.SerializeObject(newColor);
                    var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        RecolorSquareShape(newColor);
                    }
                }
            });
            thread.Start();
        }

        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BackColor = squareShapeColor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://localhost:44343/Color/changeColor/";
            StartHttpServer(url, this.textBox1.Text);
        }
    }
}
