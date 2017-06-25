// Code for WindowsInstructed.com

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;


namespace TAPS
{
    class StreamSocketClass
    {

        public static bool IsServer { get; set; }
        // Change this. True = server, false = client
        private string ServerPort = "12345";

        private StreamSocket ConnectionSocket;


        public void DataListener_OpenListenPorts()
        {
            StreamSocketListener DataListener = new StreamSocketListener();
            DataListener.BindServiceNameAsync(ServerPort).AsTask().Wait();
            DataListener.ConnectionReceived += DataListener_ConnectionReceived;
        }

        private async void DataListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {

            DataReader DataListener_Reader;
            StringBuilder DataListener_StrBuilder;
            string DataReceived;
            JObject JsonResponse;

            using (DataListener_Reader = new DataReader(args.Socket.InputStream))
            {
                DataListener_StrBuilder = new StringBuilder();
                DataListener_Reader.InputStreamOptions = InputStreamOptions.Partial;
                DataListener_Reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                DataListener_Reader.ByteOrder = ByteOrder.LittleEndian;
                await DataListener_Reader.LoadAsync(256);
                while (DataListener_Reader.UnconsumedBufferLength > 0)
                {
                    DataListener_StrBuilder.Append(DataListener_Reader.ReadString(DataListener_Reader.UnconsumedBufferLength));
                    await DataListener_Reader.LoadAsync(256);
                }
                DataListener_Reader.DetachStream();
                DataReceived = DataListener_StrBuilder.ToString();
                JsonResponse = JObject.Parse(DataReceived);
                Debug.WriteLine(JsonResponse);
            }

            if (DataReceived != null)
            {
                // Server
                if (IsServer)
                {
                    Debug.WriteLine("[SERVER] I've received " + DataReceived + " from " + args.Socket.Information.RemoteHostName);
                    // Sending reply
                    SentResponse(args.Socket.Information.RemoteAddress, "Hello Client!");
                }
                // Client
                else
                {
                    Debug.WriteLine("[CLIENT] I've received " + DataReceived + " from " + args.Socket.Information.RemoteHostName);
                }
            }
            else
            {
                Debug.WriteLine("Received data was empty. Check if you sent data.");
            }

        }

        public async void SentResponse(HostName Adress, string MessageToSent)
        {
            try
            {
                // Try connect
                Debug.WriteLine("Attempting to connect. " + Environment.NewLine);
                ConnectionSocket = new StreamSocket();
                // Wait on connection
                await ConnectionSocket.ConnectAsync(Adress, ServerPort);
                // Create a DataWriter
                DataWriter SentResponse_Writer = new DataWriter(ConnectionSocket.OutputStream);
                string content = MessageToSent;
                byte[] data = Encoding.UTF8.GetBytes(content);
                // Write the bytes
                SentResponse_Writer.WriteBytes(data);
                // Store the written data
                await SentResponse_Writer.StoreAsync();
                SentResponse_Writer.DetachStream();
                // Dispose the data
                SentResponse_Writer.Dispose();
                Debug.WriteLine("Connection has been made and your message " + MessageToSent + " has been sent." + Environment.NewLine);
                // Dispose the connection.
                ConnectionSocket.Dispose();
                ConnectionSocket = new StreamSocket();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Failed to connect " + exception.Message);
                ConnectionSocket.Dispose();
                ConnectionSocket = null;

            }
        }
    }
}