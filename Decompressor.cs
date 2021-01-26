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
using System.IO;
using SevenZip.Compression.LZMA;

namespace Network_LZMA
{
    class Decompressor
    {
        public static byte[] Decompress(byte[] inputData)
        {
            MemoryStream inputStream = new MemoryStream(inputData), outputStream = new MemoryStream();
            Decoder decoder = new Decoder();
            long outputBytes = 0;
            int index = 0, byteValue;
            byte[] properties = new byte[5];

            inputStream.Seek(0, SeekOrigin.Begin);

            if(inputStream.Read(properties, 0, 5) != 5)
                throw new Exception("Error: unable to read input data stream to decompressor.\n");

            for(; index < 8; ++index)
            {
                byteValue = inputStream.ReadByte();

                if(byteValue < 0)
                    throw new Exception("Error: unable to read input data stream to decompressor.\n");

                outputBytes |= ((long)((byte)byteValue)) << (index << 3);
            }

            decoder.SetDecoderProperties(properties);
            decoder.Code(inputStream, outputStream, inputStream.Length - inputStream.Position, outputBytes, null);

            return outputStream.ToArray();
        }
    }
}
