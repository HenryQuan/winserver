using System.Net;
using System;
using System.Threading;
using System.Text;
using NetFwTypeLib;
using System.Diagnostics;

namespace winserver
{
    class Program
    {
        static HttpListener listener = new HttpListener();

        static void Main(string[] args)
        {
            int port = 8605;

            // Add port first for foreign access
            AddPortToFirewall("WoWs Info", port);

            var address = $"http://192.168.1.102:{port}/";
            Console.WriteLine("Starting server...");
            // Add the address you want to use
            listener.Prefixes.Add(address);
            listener.Start(); // start server (Run application as Administrator!)
            Console.WriteLine("Server is now online at " + address);
            Process.Start(address);

            var response = new Thread(ResponseThread);
            response.Start(); // start the response thread
        }

        static void ResponseThread()
        {
            while (true)
            {
                var context = listener.GetContext();
                byte[] responseArray = Encoding.UTF8.GetBytes("Hello World From C#"); // get the bytes to response
                context.Response.OutputStream.Write(responseArray, 0, responseArray.Length); // write bytes to the output stream
                context.Response.KeepAlive = false; // set the KeepAlive bool to false
                context.Response.Close(); // close the connection
                Console.WriteLine("200 OK");
            }
        }

        /// <summary>
        /// Add port to windows firewall
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
                portClass.Scope = NetFwTypeLib.NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
                portClass.Enabled = true;
                portClass.Protocol = NetFwTypeLib.NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
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
                Console.WriteLine("\nPlease feel free to open an issue to discuss this it with me.");
                Process.Start("https://github.com/HenryQuan/winserver");
            }
            
        }
    }
}
