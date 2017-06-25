using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Networking;
using System.Diagnostics;


namespace TAPS
{

    public sealed partial class MainPage : Page
    {
        StreamSocketClass SocketManager = new StreamSocketClass();
        public MainPage()
        {
            this.InitializeComponent();
            // Declaring IsServer (True = server, False = client)
            StreamSocketClass.IsServer = true;
            // Declaring HostName of Server
            HostName ServerAdress = new HostName("DESKTOP-A1SAQ5U");
            // Open Listening ports and start listening.
            SocketManager.DataListener_OpenListenPorts();
            // Server
            if (StreamSocketClass.IsServer)
            {
                Debug.WriteLine("[SERVER] Ready to receive");
            }
            // Client
            else
            {
                SocketManager.SentResponse(ServerAdress, "Hello WindowsInstructed");
            }

        }
    }
}