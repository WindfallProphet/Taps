// Code for WindowsInstructed.com

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;


namespace TAPS
{
    class Client
    {

        private string ServerPort = "12345";
        private StreamSocket ConnectionSocket;
        public async void SentResponse(HostName Adress, string MessageToSend)
        {
            try
            {
                //Try to connect
                Debug.WriteLine("Attempting to connect...");
                ConnectionSocket = new StreamSocket();
                //Await connection
                await ConnectionSocket.ConnectAsync(Adress, ServerPort);
                //Create a DataWriter
                DataWriter SentResponse_Writer = new DataWriter(ConnectionSocket.OutputStream);
                string content = MessageToSend;
                byte[] data = Encoding.UTF8.GetBytes(content);
                //Write the bytes
                SentResponse_Writer.WriteBytes(data);
                //Store the written data
                await SentResponse_Writer.StoreAsync();
                SentResponse_Writer.DetachStream();
                //Dispose of the data
                SentResponse_Writer.Dispose();
                Debug.WriteLine("Connection has been made and your message('{0}') has been sent.", MessageToSend);
                //Now end ("dispose") the connection
                ConnectionSocket.Dispose();
                ConnectionSocket = new StreamSocket();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Failed to connect {0}", exception.Message);
                ConnectionSocket.Dispose();
                ConnectionSocket = null;
            }
        }
    }
}
