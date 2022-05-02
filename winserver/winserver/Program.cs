using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetFwTypeLib;

namespace winserver
{
    class Program
    {
        static HttpListener listener = new HttpListener();

        static void Main(string[] args)
        {
            int port = 8605;
            string ip = GetIPAddress();

            // Add port first for foreign access
            AddPortToFirewall("WoWs Info", port);

            var address = $"http://localhost:{port}/";
            Console.WriteLine("Starting server...");
            // Add the address you want to use
            listener.Prefixes.Add(address);
            listener.Start(); // start server (Run application as Administrator!)
            Console.WriteLine("Server is now online at " + address);
            Process.Start(new ProcessStartInfo {
                FileName = address,
                UseShellExecute = true
            });
            Console.WriteLine("Github: https://github.com/HenryQuan/winserver");

            var response = new Thread(ResponseThread);
            response.Start(); // start the response thread
        }


        /// <summary>
        /// Responce to request
        /// Reference: https://www.codeproject.com/Tips/485182/Create-a-local-server-in-Csharp
        /// </summary>
        static void ResponseThread()
        {
            var rand = new Random();
            while (true)
            {
                var context = listener.GetContext();
                byte[] buffer = Encoding.UTF8.GetBytes($"Hello World from C#\nYour lucky number is {rand.Next(888888)}"); // get the bytes to response
                var response = context.Response;
                response.ContentLength64 = buffer.Length;
                response.KeepAlive = false; // set the KeepAlive bool to false
                var output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length); // write bytes to the output stream
                output.Close();
            }
        }

        /// <summary>
        /// Get your local ip address
        /// Reference: https://stackoverflow.com/questions/6803073/get-local-ip-address
        /// </summary>
        /// <returns>local ip</returns>
        static string GetIPAddress()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                // It does not have to be valid since there is no real connection
                socket.Connect("8.8.8.8", 65530);
                // This gives the local address that would be used to connect to the specified remote host
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        /// <summary>
        /// Add port to windows firewall
        /// Reference: https://social.msdn.microsoft.com/Forums/vstudio/en-US/a3e390d1-4383-4f23-bad9-b725bef33499/add-firewall-rule-programatically?forum=wcf
        /// </summary>
        static void AddPortToFirewall(string name, int port)
        {
            try
            {
                Type TicfMgr = Type.GetTypeFromProgID("HNetCfg.FwMgr");
                INetFwMgr icfMgr = (INetFwMgr)Activator.CreateInstance(TicfMgr);

                // add a new port
                Type TportClass = Type.GetTypeFromProgID("HNetCfg.FWOpenPort");
                INetFwOpenPort portClass = (INetFwOpenPort)Activator.CreateInstance(TportClass);

                // Get the current profile
                INetFwProfile profile = icfMgr.LocalPolicy.CurrentProfile;

                // Set the port properties
                portClass.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
                portClass.Enabled = true;
                portClass.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                // WoWs Info - 8605
                portClass.Name = name;
                portClass.Port = port;

                // Add the port to the ICF Permissions List
                profile.GloballyOpenPorts.Add(portClass);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to add port to firewall. This is the error message.\n");
                Console.WriteLine(e.Message);
            }
        }
    }
}
