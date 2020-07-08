using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AlyCommon
{
    [Serializable]
    public class SocketMessage
    {
        public Guid Key { get; set; }

        public int Length { get; set; }

        public byte[] Body { get; set; }

          public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(Key.ToByteArray());
                ms.Write(BitConverter.GetBytes(Length));
                ms.Write(Body);
                return ms.ToArray();
            }
        }
    }
}
