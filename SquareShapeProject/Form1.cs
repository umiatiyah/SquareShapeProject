using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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

        private async Task StartHttpServer(string url, string newColor)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var jsonObject = JsonConvert.SerializeObject(newColor);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content).ConfigureAwait(false);
                if (httpResponse.IsSuccessStatusCode)
                {
                    RecolorSquareShape(newColor);
                }
                else
                {
                    ShowDialogResult("Bad Request");
                }
            }
            catch
            {
                ShowDialogResult("Service Error / Service Off");
            }
        }

        private void ShowDialogResult(string message)
        {
            Form form = new Form();
            Label label = new Label();
            form.Text = message;
            label.Text = message;
            label.AutoSize = true;
            form.AutoSize = true;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Controls.Add(label);
            form.ShowDialog();
        }
        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BackColor = squareShapeColor;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string color = this.textBox1.Text;
            if (!string.IsNullOrEmpty(color))
            {
                string url = $"{ConfigurationManager.AppSettings["ColorApi"]}/changeColor";
                await StartHttpServer(url, this.textBox1.Text);
            }
        }
    }
}
