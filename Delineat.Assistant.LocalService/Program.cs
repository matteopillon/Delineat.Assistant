using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Delineat.Assistant.LocalService
{
    class Program
    {

        public static void Main(string[] args)
        {
            JObject data;
            data = Read();

            var processed = ProcessMessage(data);

            Write(processed);

        }

        public static ProcessResult ProcessMessage(JObject data)
        {

            var messageAction = data["action"].Value<string>();
            switch ((messageAction ?? string.Empty).ToLower())
            {
                case "openfolder":
                    return OpenFolder(data["path"].Value<string>());

                case "openfile":
                    return OpenFile(data["path"].Value<string>());

            }

            return new ProcessResult(false, $"Operazione non '{messageAction}'  riconosciuta");
        }

        public static JObject Read()
        {
            var stdin = Console.OpenStandardInput();
            var length = 0;

            var lengthBytes = new byte[4];
            stdin.Read(lengthBytes, 0, 4);
            length = BitConverter.ToInt32(lengthBytes, 0);

            var buffer = new char[length];
            using (var reader = new StreamReader(stdin))
            {
                while (reader.Peek() >= 0)
                {
                    reader.Read(buffer, 0, buffer.Length);
                }
            }

            return (JObject)JsonConvert.DeserializeObject<JObject>(new string(buffer));
        }

        public static void Write(ProcessResult data)
        {

         
            var bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

            var stdout = Console.OpenStandardOutput();
            stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
            stdout.Write(bytes, 0, bytes.Length);
            stdout.Flush();
        }

        [DllImport("User32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        private static void StartProcess(string path)
        {
            Process procRun = Process.Start(path);
            if (procRun != null)
            {
                var handle = procRun.MainWindowHandle;
                SwitchToThisWindow(handle, true);
            }
        }

        private static ProcessResult OpenFile(string path)
        {

            if (!string.IsNullOrWhiteSpace(path))
            {
                if (File.Exists(path))
                {
                    StartProcess(path);
                    return new ProcessResult(true);
                }
                else
                {
                    return new ProcessResult(false, $"File {path} non trovato");
                }
            }

            return new ProcessResult(false, "Percorso del file non valorizzato");

        }


        private static ProcessResult OpenFolder(string path)
        {
            //var storageFile = await Download();
            if (!string.IsNullOrWhiteSpace(path))
            {
                var directory = Path.GetDirectoryName(path);
                if (Directory.Exists(directory))
                {
                    StartProcess(directory);
                    return new ProcessResult(true);
                }

                else
                {
                    return new ProcessResult(false, $"Directory {path} non trovata");
                }
            }
            return new ProcessResult(false, $"Percorso della directory non valorizzato");
        }

    }
}
