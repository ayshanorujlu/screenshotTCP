using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;

namespace ScreenShotAPP.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        BitmapImage? imageSource;

        public BitmapImage? PhotoSource
        {
            get { return imageSource; }
            set { Set(ref imageSource, value); }
        }


        public Dispatcher? Dispatcher { get; set; } = Dispatcher.CurrentDispatcher;

        public IPAddress? IpAdress { get; set; }

        public IPEndPoint? IpAdressEndPoint { get; set; }

        public TcpClient? Client { get; set; }

        public MainViewModel()
        {
            PhotoSource = new();
            IpAdress = IPAddress.Parse("10.2.24.26");
            IpAdressEndPoint = new(IpAdress, 27001);
            Client = new TcpClient();
        }


        public RelayCommand StartCommand
        {
            get => new(() =>
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            Client = new();
                            Client.Connect(IpAdressEndPoint!);
                            if (Client.Connected)
                            {
                                try
                                {
                                    using (NetworkStream networkStream = Client.GetStream())
                                    {
                                        byte[] imageData = new byte[4096];
                                        int bytesRead;
                                        using (MemoryStream memoryStream = new MemoryStream())
                                        {

                                            while ((bytesRead = networkStream.Read(imageData, 0, imageData.Length)) > 0)
                                            {
                                                memoryStream.Write(imageData, 0, bytesRead);
                                            }

                                            memoryStream.Seek(0, SeekOrigin.Begin);
                                            BitmapImage bitmapImage = new();
                                            bitmapImage.BeginInit();
                                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                            bitmapImage.StreamSource = memoryStream;
                                            bitmapImage.EndInit();
                                            bitmapImage.Freeze();

                                            Dispatcher!.Invoke(() => PhotoSource = bitmapImage);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        Task.Delay(1000).Wait();
                    }
                });
            });
        }
    }
}