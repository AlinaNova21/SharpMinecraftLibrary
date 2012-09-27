using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using MinecraftLibrary;

namespace TestProject1
{
    public class TestPacket : Packet
    {
        public TestPacket(Stream str)
        {
            this.str = str;
        }
        public override void Read()
        {
            throw new NotImplementedException();
        }
        public override void Write()
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class PacketTests
    {
        [TestMethod]
        public void P00KeepAlive()
        {
            MemoryStream str1 = new MemoryStream();
            MemoryStream str2 = new MemoryStream();

            TestPacket tp = new TestPacket(str1);
            tp.Byte = 0;
            tp.Int = 12345;

            Packet_KeepAlive p1 = new Packet_KeepAlive();
            p1.Stream = str2;
            p1.ID = 12345;
            p1.Write();

            byte[] expect = str1.ToArray();
            byte[] actual = str2.ToArray();
            
            Assert.AreEqual(str1.Length, str2.Length,"Arrays have different lengths!");
            CollectionAssert.AreEqual(expect, actual,"Arrays have different contents!");
        }
        [TestMethod]
        public void P02HandShake()
        {
            MemoryStream str1 = new MemoryStream();
            MemoryStream str2 = new MemoryStream();

            TestPacket tp = new TestPacket(str1);
            tp.Byte = 2;
            tp.Byte = 22;
            tp.String = "ags131";
            tp.String = "localhost";
            tp.Int = 25565;

            Packet_Handshake p1 = new Packet_Handshake();
            p1.Stream = str2;
            p1.Host = "localhost";
            p1.Port = 25565;
            p1.ProtocolVersion = 22;
            p1.Username = "ags131";
            p1.Write();

            byte[] expect = str1.ToArray();
            byte[] actual = str2.ToArray();

            Assert.AreEqual(str1.Length, str2.Length, "Arrays have different lengths!");
            CollectionAssert.AreEqual(expect, actual, "Arrays have different contents!");
        }
    }
}
