﻿using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SquareShapeProject.Models;
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
        private readonly string url = $"{ConfigurationManager.AppSettings["ColorApi"]}";
        private readonly HubConnection _hubConnection;
        public Form1()
        {
            InitializeComponent();
            DrawSquareShape();
            _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{url}/colorhub")
            .Build();
            _hubConnection.On<string>("ChangeColor", colorRes =>
            {
                RecolorSquareShape(colorRes);
            });
            Thread thread = new Thread(async () =>
            {
                await _hubConnection.StartAsync();
            });
            thread.Start();
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
                ColorModel colorModel = new ColorModel();
                colorModel.ColorName = newColor;
                var jsonObject = JsonConvert.SerializeObject(colorModel);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content).ConfigureAwait(false);
                string apiResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                var res = JsonConvert.DeserializeObject<ColorResponse>(apiResponse);
                if (httpResponse.IsSuccessStatusCode)
                {
                    RecolorSquareShape(res.ColorName);
                }
                else
                {
                    ShowDialogResult($"Bad Request! {apiResponse}");
                }
            }
            catch(Exception ex)
            {
                ShowDialogResult($"Service Error/Service Off! {ex.Message}");
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
                await StartHttpServer($"{url}/Color/changeColor", this.textBox1.Text);
            }
        }
    }
}
