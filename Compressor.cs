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

using System.IO;
using SevenZip;
using SevenZip.Compression.LZMA;

namespace Network_LZMA
{
    class Compressor
    {
        public static byte[] Compress(byte[] inputData)
        {
            CoderPropID[] propertyIDs =
            {
                CoderPropID.DictionarySize,
                CoderPropID.PosStateBits,
                CoderPropID.LitContextBits,
                CoderPropID.LitPosBits,
                CoderPropID.Algorithm,
                CoderPropID.NumFastBytes,
                CoderPropID.MatchFinder,
                CoderPropID.EndMarker
            };

            object[] properties = { 1 << 23, 2, 3, 0, 2, 128, "BT4", false };
            MemoryStream inputStream = new MemoryStream(inputData), outputStream = new MemoryStream();
            Encoder encoder = new Encoder();
            int index = 0;

            encoder.SetCoderProperties(propertyIDs, properties);
            encoder.WriteCoderProperties(outputStream);

            for(; index < 8; ++index)
                outputStream.WriteByte((byte)(inputStream.Length >> (index << 3)));

            encoder.Code(inputStream, outputStream, -1, -1, null);
            return outputStream.ToArray();
        }
    }
}
