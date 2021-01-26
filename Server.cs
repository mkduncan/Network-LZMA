/*********************************************************************************
** MIT License
**
** Copyright(c) 2018 duncanmk
**
** Permission is hereby granted, free of charge, to any person obtaining a copy
** of this software and associated documentation files(the "Software"), to deal
** in the Software without restriction, including without limitation the rights
** to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
** copies of the Software, and to permit persons to whom the Software is
** furnished to do so, subject to the following conditions :
**
** The above copyright notice and this permission notice shall be included in all
** copies or substantial portions of the Software.
**
** THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
** IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
** FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
** AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
** LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
** OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
** SOFTWARE.
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Network_LZMA
{
    class Server
    {
        private static String serverMessage =
            "This text was originally compressed from the SERVER and sent to the client. " +
            "If this text is readable, then the CLIENT has decompressed this text." +
            "This serves to be a proof of concept of transmitting compressed data and receiving decompressed data." +
            "This message is intended to be long to show how much data can be saved through using compression. " +
            "9 8 7 6 5 4 3 2 1 0 Z Y X W V U T S R Q P O N M L K J I H G F E D C B A";

        public static void Listen()
        {
            IPHostEntry localInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localAddress = localInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(localAddress, 12345);
            Socket listener = new Socket(localAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp), handler = null;
            List<Byte> compressedData = null;
            Byte[] bufferedData = null, decompressedData = null;
            int receivedBytes = 0, index;

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(8);

                while(true)
                {
                    Console.WriteLine("Server: waiting for a connection...");

                    handler = listener.Accept();
                    compressedData = new List<byte>();

                    while(true)
                    {
                        bufferedData = new byte[1024];
                        receivedBytes = handler.Receive(bufferedData);

                        for(index = 0; index < receivedBytes; ++index)
                            compressedData.Add(bufferedData[index]);

                        if(compressedData.IndexOf((Byte)'\0') != -1)
                            break;
                    }

                    compressedData.TrimExcess();
                    bufferedData = compressedData.ToArray();
                    Console.WriteLine("Server: received compressed message with {0} bytes.", bufferedData.Length);

                    decompressedData = Decompressor.Decompress(bufferedData);
                    Console.WriteLine("Server: decompressed message to {0} bytes:", decompressedData.Length);
                    Console.WriteLine("  {0}", Encoding.ASCII.GetString(decompressedData));
                    
                    decompressedData = Encoding.ASCII.GetBytes(serverMessage);

                    Console.WriteLine("Server: compressing message from {0} bytes.", serverMessage.Length);
                    bufferedData = Compressor.Compress(decompressedData);

                    Console.WriteLine("Server: sending compressed message with {0} bytes.", bufferedData.Length);
                    
                    handler.Send(bufferedData);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                    Console.WriteLine();
                }
            }

            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
