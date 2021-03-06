﻿// Original Work Copyright (c) Ethan Moffat 2014-2016
// This file is subject to the GPL v2 License
// For additional details, see the LICENSE file

using System;
using System.IO;
using System.Text;
using EOLib.Data.Map;

namespace EOLib.IO
{
	public enum NPCType
	{
		NPC,
		Passive,
		Aggressive,
		Unknown1,
		Unknown2,
		Unknown3,
		Shop,
		Inn,
		Unknown4,
		Bank,
		Barber,
		Guild,
		Priest,
		Law,
		Skills,
		Quest,
		NONE
	}

	public class NPCRecord : IDataRecord, IMapElement
	{
		public int ID { get; set; }

		public int NameCount { get { return 1; } }
		public string Name { get; set; }

		public int Graphic { get; set; }

		public short Boss { get; set; }
		public short Child { get; set; }
		public NPCType Type { get; set; }

		public short VendorID { get; set; }

		public int HP { get; set; }
		public ushort Exp { get; set; }
		public short MinDam { get; set; }
		public short MaxDam { get; set; }

		public short Accuracy { get; set; }
		public short Evade { get; set; }
		public short Armor { get; set; }

		public NPCRecord(int id)
		{
			ID = id;
		}

		public override string ToString()
		{
			return ID + ": " + Name;
		}

		public void SetNames(params string[] names)
		{
			if (names.Length != NameCount)
				throw new ArgumentException("Error: item record has invalid number of names");

			Name = names[0];
		}

		public void DeserializeFromByteArray(int version, byte[] rawData)
		{
			if (version != 0)
				throw new FileLoadException("Unable to Load file with invalid version: " + version);

			Graphic = Packet.DecodeNumber(rawData[0], rawData[1]);
			Boss = (short) Packet.DecodeNumber(rawData[3], rawData[4]);
			Child = (short) Packet.DecodeNumber(rawData[5], rawData[6]);
			Type = (NPCType) Packet.DecodeNumber(rawData[7], rawData[8]);
			VendorID = (short) Packet.DecodeNumber(rawData[9], rawData[10]);
			HP = Packet.DecodeNumber(rawData[11], rawData[12], rawData[13]);
			MinDam = (short) Packet.DecodeNumber(rawData[16], rawData[17]);
			MaxDam = (short) Packet.DecodeNumber(rawData[18], rawData[19]);
			Accuracy = (short) Packet.DecodeNumber(rawData[20], rawData[21]);
			Evade = (short) Packet.DecodeNumber(rawData[22], rawData[23]);
			Armor = (short) Packet.DecodeNumber(rawData[24], rawData[25]);
			Exp = (ushort) Packet.DecodeNumber(rawData[36], rawData[37]);
		}

		public byte[] SerializeToByteArray()
		{

			byte[] ret = new byte[ClassFile.DATA_SIZE + 1 + Name.Length];
			for (int i = 0; i < ret.Length; ++i)
				ret[i] = 254;

			using (MemoryStream mem = new MemoryStream(ret))
			{
				mem.WriteByte(Packet.EncodeNumber(Name.Length, 1)[0]);
				byte[] name = Encoding.ASCII.GetBytes(Name);
				mem.Write(name, 0, name.Length);

				mem.Write(Packet.EncodeNumber(Graphic, 2), 0, 2);

				mem.Seek(3, SeekOrigin.Begin);
				mem.Write(Packet.EncodeNumber(Boss, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(Child, 2), 0, 2);
				mem.Write(Packet.EncodeNumber((short)Type, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(VendorID, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(HP, 3), 0, 3);

				mem.Seek(16, SeekOrigin.Begin);
				mem.Write(Packet.EncodeNumber(MinDam, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(MaxDam, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(Accuracy, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(Evade, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(Armor, 2), 0, 2);

				mem.Seek(36, SeekOrigin.Begin);
				mem.Write(Packet.EncodeNumber(Exp, 2), 0, 2);
			}

			return ret;
		}
	}

	public class NPCFile : EODataFile
	{
		public const int DATA_SIZE = 39;

		public NPCFile()
			: base(new NPCRecordFactory())
		{
			Load(FilePath = Constants.NPCFilePath);
		}

		public NPCFile(string path)
			: base(new NPCRecordFactory())
		{
			Load(FilePath = path);
		}

		protected override int GetDataSize()
		{
			return DATA_SIZE;
		}
	}
}
