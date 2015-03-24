using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Compression;
using Microsoft.Win32;
using Newtonsoft.Json;
using VOTCServer.Core;
using VOTCServer.Scripts;

namespace VOTCServer.HTTP
{
    class Client : IDisposable
    {
        #region Private
        private readonly NetworkStream _networkStream;
        private readonly MemoryStream _memoryStream = new MemoryStream();
        private readonly StreamReader _streamReader;
        private const string Identifier = "HTTP:VOTC";
        #endregion

        public Client(System.Net.Sockets.Socket socket)
        {
            _networkStream = new NetworkStream(socket, true);
            _streamReader = new StreamReader(_memoryStream);
        }

        public async void Do()
        {
            var maxBuffer = new byte[12 * 1024 * 1014];
            try
            {
                for (;;)
                {
                    var bytesRead = await _networkStream.ReadAsync(maxBuffer, 0, maxBuffer.Length);

                    if (bytesRead == 0)
                        return;

                    _memoryStream.Seek(0, SeekOrigin.End);
                    await _memoryStream.WriteAsync(maxBuffer, 0, bytesRead);
                    var get = await ProcessHeader();
                    if (get)
                        break;
                    var data = new byte[bytesRead];
                    Buffer.BlockCopy(maxBuffer, 0, data, 0, bytesRead);

                    var incommingData = Encoding.UTF8.GetString(data);
                    if (incommingData.Contains("POST"))
                        continue;
                    try
                    {
                        var decompressed = Bit.Decompress(data);
                        incommingData = Encoding.UTF8.GetString(decompressed);

                        ProcessSubmission(incommingData);
                    }
                    catch (Exception e)
                    {
                        Kernel.WriteLine(e, ConsoleColor.Red);
                    }
                }
            }
            catch (Exception e)
            {
                Kernel.WriteLine(e, ConsoleColor.Red);
            }
        }

        private static void ProcessSubmission(string incommingData)
        {
            incommingData = Uri.UnescapeDataString(incommingData);
            new Script(incommingData.SplitByLine(), incommingData.Contains("Socket") || incommingData.Contains("Listen"));
        }

        private async Task<bool> ProcessHeader()
        {
            try
            {
                for (;;)
                {
                    _memoryStream.Seek(0, SeekOrigin.Begin);
                    var line = await _streamReader.ReadLineAsync();

                    if (line == null)
                        break;

                    if (line.StartsWith("GET "))
                    {
                        var file = line.Split(' ')[1].TrimStart('/');
                        if (string.IsNullOrWhiteSpace(file))
                            file = "index.html";
                        SendFile(file);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async void SendFile(string file)
        {
            file = Uri.UnescapeDataString(file);
            try
            {
                byte[] data;
                string responseCode;
                var contentType = "";
                try
                {
                    if (File.Exists(@"C:\WebRoot\VOTC\downloads\" + file))
                    {
                        data = File.ReadAllBytes(@"C:\WebRoot\VOTC\downloads\" + file);
                        Kernel.DownloadCount++;
                        contentType = GetContentType("application/octet-stream");
                        responseCode = "200 OFC MEN";
                    }
                    else
                    {
                        if (file.Contains(".download"))
                        {
                            var builder = new StringBuilder();
                            var array = file.Split('.');
                            Kernel.AllScripts[array[0] + ".cs"].Downloads++;
                            var serialized = JsonConvert.SerializeObject(Kernel.AllScripts[array[0] + ".cs"]);
                            builder.Append(serialized);
                            data = Encoding.UTF8.GetBytes(builder.ToString().Replace("Â", ""));
                            responseCode = "200 OK";
                        }
                        else if (file.Contains(".badge"))
                        {
                            var array = file.Split('.');
                            var image = Kernel.AllScripts[array[0] + ".cs"].StoreBadge;
                            responseCode = "200 OK";
                            data = Encoding.UTF8.GetBytes(image);
                        }
                        else if (file.Contains(".header"))
                        {
                            var array = file.Split('.');
                            var image = Kernel.AllScripts[array[0] + ".cs"].HeaderImage;
                            responseCode = "200 OK";
                            data = Encoding.UTF8.GetBytes(image);
                        }
                        else if (file.Contains(".gethash"))
                        {
                            var array = file.Split('.');
                            responseCode = "200 OK";
                            data = Encoding.ASCII.GetBytes(Kernel.AllScripts[array[0] + ".cs"].Updated.ToUniversalTime().ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            switch (file)
                            {
                                case "track.bitflash":
                                    {
                                        if (Kernel.CommandsReceived.Count < 1)
                                        {
                                            data = Encoding.UTF8.GetBytes("No Data");
                                            responseCode = "200 OK";
                                            break;
                                        }

                                        var builder = new StringBuilder();
                                        builder.AppendLine("<h1>VOTC Realtime Stats</h1>");
                                        for (var I = Kernel.CommandsReceived.Count - 1; I > 0; I--)
                                        {
                                            builder.AppendLine("<p>" + Kernel.CommandsReceived[I] + "</p>");
                                        }
                                        data = Encoding.UTF8.GetBytes(builder.ToString().Replace("Â", ""));
                                        responseCode = "200 OK";
                                        break;
                                    }
                                case "getmostdownloaded.bitflash":
                                    {
                                        var builder = new StringBuilder();
                                        foreach (var script in Kernel.AllScripts.OrderByDescending(o => o.Value.Downloads))
                                        {
                                            builder.Append(script.Value.Name + "§");
                                        }
                                        data = Encoding.UTF8.GetBytes(builder.ToString().Replace("Â", ""));
                                        responseCode = "200 OK";
                                        break;
                                    }
                                case "getnew.bitflash":
                                    {
                                        var builder = new StringBuilder();
                                        foreach (var script in Kernel.AllScripts.Where(o => o.Value.Category == Category.New))
                                        {
                                            builder.Append(script.Value.Name + "§");
                                        }
                                        data = Encoding.UTF8.GetBytes(builder.ToString().Replace("Â", ""));
                                        responseCode = "200 OK";
                                        break;
                                    }
                                case "getharmful.bitflash":
                                    {
                                        var builder = new StringBuilder();
                                        foreach (var script in Kernel.AllScripts.Where(o => o.Value.Category == Category.Harmful))
                                        {
                                            builder.Append(script.Value.Name + "§");
                                        }
                                        data = Encoding.UTF8.GetBytes(builder.ToString().Replace("Â", ""));
                                        responseCode = "200 OK";
                                        break;
                                    }
                                case "getpopular.bitflash":
                                    {
                                        var builder = new StringBuilder();
                                        foreach (var script in Kernel.AllScripts.Where(o => o.Value.Category == Category.Popular))
                                        {
                                            builder.Append(script.Value.Name + "§");
                                        }
                                        data = Encoding.UTF8.GetBytes(builder.ToString().Replace("Â", ""));
                                        responseCode = "200 OK";
                                        break;
                                    }
                                case "getfeatured.bitflash":
                                    {
                                        var builder = new StringBuilder();
                                        foreach (var script in Kernel.AllScripts.Where(o => o.Value.Category == Category.Featured))
                                        {
                                            builder.Append(script.Value.Name + "§");
                                        }
                                        data = Encoding.UTF8.GetBytes(builder.ToString().Replace("Â", ""));
                                        responseCode = "200 OK";
                                        break;
                                    }
                                default:
                                    {
                                        var builder = new StringBuilder();

                                        var serialized = JsonConvert.SerializeObject(Kernel.AllScripts[file + ".cs"]);
                                        builder.Append(serialized);
                                        data = Encoding.UTF8.GetBytes(builder.ToString().Replace("Â", ""));
                                        responseCode = "200 OK";
                                        break;
                                    }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    data =
                        Encoding.UTF8.GetBytes("<html><body><h1>500 Internal server error</h1><pre>" +
                                               exception.Message + "</pre></body></html>");
                    responseCode = "500 GOD DAMN IT";
                }
                var header = string.Format("HTTP/1.1 {0}\r\n"
                                           + "Server: {1}\r\n"
                                           + "Content-Length: {2}\r\n"
                                           + "Content-Type: {3}\r\n"
                                           + "Keep-Alive: Close\r\n"
                                           + "\r\n", responseCode,
                    Identifier, data.Length,
                    contentType);

                var headerBytes = Encoding.UTF8.GetBytes(header);
                await _networkStream.WriteAsync(headerBytes, 0, headerBytes.Length);
                await _networkStream.WriteAsync(data, 0, data.Length);
                await _networkStream.FlushAsync();
                _networkStream.Dispose();
            }
            catch (Exception e)
            {
                Kernel.WriteLine(e, ConsoleColor.Red);
            }
        }

        private string GetContentType(string extension)
        {
            if (Regex.IsMatch(extension, "^[a-z0-9]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                return (Registry.GetValue(@"HKEY_CLASSES_ROOT\." + extension, "Content Type", null) as string) ?? "application/octet-stream";
            return "application/octet-stream";
        }

        public void Dispose()
        {
            _streamReader.Dispose();
            _networkStream.Dispose();
            _memoryStream.Dispose();
        }
    }
}
