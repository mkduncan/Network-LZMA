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
    class Client
    {
        private static String clientMessage =
            "This text was originally compressed from the CLIENT and sent to the server. " +
            "If this text is readable, then the SERVER has decompressed this text. " +
            "This serves to be a proof of concept of transmitting compressed data and receiving decompressed data. " +
            "This message is intended to be long to show how much data can be saved through using compression. " +
            "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z 0 1 2 3 4 5 6 7 8 9";

        public static void StartClient()
        {
            try
            {
                IPHostEntry localInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress localAddress = localInfo.AddressList[0];
                IPEndPoint remoteEndPoint = new IPEndPoint(localAddress, 12345);
                Socket sender = new Socket(localAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                List<Byte> data = new List<Byte>();
                Byte[] decompressedMessage = null, compressedMessage = null, response = null;
                int messageBytes = 0, index;
                
                try
                {
                    sender.Connect(remoteEndPoint);
                    Console.WriteLine("Client: connected to server \"{0}\"", sender.RemoteEndPoint.ToString());

                    decompressedMessage = Encoding.ASCII.GetBytes(clientMessage);
                    Console.WriteLine("Client: compressing message of {0} bytes to send to server.", decompressedMessage.Length);

                    compressedMessage = Compressor.Compress(decompressedMessage);

                    Console.WriteLine("Client: sending compressed message of {0} bytes to server.", compressedMessage.Length);
                    sender.Send(compressedMessage);

                    response = new Byte[1024];
                    messageBytes = sender.Receive(response);

                    Console.WriteLine("Client: received compressed message of {0} bytes from server.", messageBytes);

                    for(index = 0; index < messageBytes; ++index)
                        data.Add(response[index]);

                    compressedMessage = data.ToArray();
                    decompressedMessage = Decompressor.Decompress(compressedMessage);

                    Console.WriteLine("Client: decompressed message of {0} bytes:", decompressedMessage.Length);
                    Console.WriteLine("    {0}", Encoding.ASCII.GetString(decompressedMessage));

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

                catch(ArgumentNullException exception)
                {
                    Console.WriteLine(exception.ToString());
                }

                catch(SocketException exception)
                {
                    Console.WriteLine(exception.ToString());
                }

                catch(Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }

            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}
