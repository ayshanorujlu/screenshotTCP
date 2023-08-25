using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Text;

IPAddress iPAddress = IPAddress.Parse("10.2.24.26");
IPEndPoint iPEndPoint = new(iPAddress, 27001);

TcpListener listener = new(iPEndPoint);

listener.Start();

Console.WriteLine("Listening...");

while (true)
{
    var client = listener.AcceptTcpClient();
    Console.WriteLine($"{client.Client.LocalEndPoint} connected");
    Task.Run(() =>
    {

        while (true)
        {
            var stream = client.GetStream();
            var bitmap = ScreenShot();
            ImageConverter converter = new ImageConverter();
            var bytes = (byte[])converter.ConvertTo(bitmap, typeof(byte[]))!;
            stream.Write(bytes);
            Console.WriteLine("Screenshoted!");

            Console.WriteLine($"{bytes.Length} Sent");
            stream.Close();
        }
    });
}

Bitmap ScreenShot()
{
    Bitmap memoryImage;
    memoryImage = new Bitmap(1920, 1080);
    Size s = new Size(memoryImage.Width, memoryImage.Height);

    Graphics memoryGraphics = Graphics.FromImage(memoryImage);

    memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);

    return memoryImage;
}