﻿// Original Work Copyright (c) Ethan Moffat 2014-2016
// This file is subject to the GPL v2 License
// For additional details, see the LICENSE file

using System;
using System.IO;
using System.Text;

namespace EOLib.IO
{
	#region Enums

	public enum EquipLocation
	{
		Boots = 0,
		Accessory,
		Gloves,
		Belt,
		Armor,
		Necklace,
		Hat,
		Shield,
		Weapon,
		Ring1,
		Ring2,
		Armlet1,
		Armlet2,
		Bracer1,
		Bracer2,
		PAPERDOLL_MAX = 15
	}

	public enum ItemType : byte
	{
		Static,
		UnknownType1,
		Money,
		Heal,
		Teleport,
		Spell,
		EXPReward,
		StatReward,
		SkillReward,
		Key,
		Weapon,
		Shield,
		Armor,
		Hat,
		Boots,
		Gloves,
		Accessory,
		Belt,
		Necklace,
		Ring,
		Armlet,
		Bracer,
		Beer,
		EffectPotion,
		HairDye,
		CureCurse
	}

	public enum ItemSubType : byte
	{
		None,
		Ranged,
		Arrows,
		Wings,
		//The following 2 values require modded pubs in order to work properly.
		FaceMask, //ADDED: *this* client will interpret this value as a hat/mask, so all hair should be shown (ie frog head/dragon mask)
		HideHair //ADDED: *this* client will interpret this value as a hat/helmet, so no hair should be shown (ie helmy, H.O.D., etc)
	}

	public enum ItemSpecial : byte
	{
		Normal,
		Rare, // ?
		UnknownSpecial2,
		Unique, // ?
		Lore,
		Cursed
	}

	public enum ItemSize : byte
	{
		Size1x1,
		Size1x2,
		Size1x3,
		Size1x4,
		Size2x1,
		Size2x2,
		Size2x3,
		Size2x4
	}

	#endregion

	public class ItemRecord : IDataRecord
	{
		public int ID { get; set; }

		public int NameCount { get { return 1; } }
		public string Name { get; set; }

		public short Graphic { get; set; }
		public ItemType Type { get; set; }
		public ItemSubType SubType { get; set; }

		public ItemSpecial Special { get; set; }
		public short HP { get; set; }
		public short TP { get; set; }
		public short MinDam { get; set; }
		public short MaxDam { get; set; }
		public short Accuracy { get; set; }
		public short Evade { get; set; }
		public short Armor { get; set; }

		public byte Str { get; set; }
		public byte Int { get; set; }
		public byte Wis { get; set; }
		public byte Agi { get; set; }
		public byte Con { get; set; }
		public byte Cha { get; set; }

		public byte Light { get; set; }
		public byte Dark { get; set; }
		public byte Earth { get; set; }
		public byte Air { get; set; }
		public byte Water { get; set; }
		public byte Fire { get; set; }

		//Three item-specific parameters
		//eoserv uses unions, but they can just point to the same private data member
		private int itemSpecificParam1;
		private byte itemSpecificParam2;
		private byte itemSpecificParam3;

		public int ScrollMap { get { return itemSpecificParam1; } set { itemSpecificParam1 = value; } }
		public int DollGraphic { get { return itemSpecificParam1; } set { itemSpecificParam1 = value; } }
		public int ExpReward { get { return itemSpecificParam1; } set { itemSpecificParam1 = value; } }
		public int HairColor { get { return itemSpecificParam1; } set { itemSpecificParam1 = value; } }
		public int Effect { get { return itemSpecificParam1; } set { itemSpecificParam1 = value; } }
		public int Key { get { return itemSpecificParam1; } set { itemSpecificParam1 = value; } }

		/// <summary>
		/// 0 == female and 1 == male
		/// </summary>
		public byte Gender { get { return itemSpecificParam2; } set { itemSpecificParam2 = value; } }
		public byte ScrollX { get { return itemSpecificParam2; } set { itemSpecificParam2 = value; } }

		public byte ScrollY { get { return itemSpecificParam3; } set { itemSpecificParam3 = value; } }
		public byte DualWieldDollGraphic { get { return itemSpecificParam3; } set { itemSpecificParam3 = value; } }

		public short LevelReq { get; set; }
		public short ClassReq { get; set; }
		public short StrReq { get; set; }
		public short IntReq { get; set; }
		public short WisReq { get; set; }
		public short AgiReq { get; set; }
		public short ConReq { get; set; }
		public short ChaReq { get; set; }

		public byte Weight { get; set; }

		public ItemSize Size { get; set; }

		public ItemRecord(int id)
		{
			ID = id;
		}

		public override string ToString()
		{
			return ID + " : " + Name;
		}

		public void SetNames(params string[] names)
		{
			if(names.Length != NameCount)
				throw new ArgumentException("Error: item record has invalid number of names");

			Name = names[0];
		}

		public void DeserializeFromByteArray(int version, byte[] rawData)
		{
			//version 0/1 support: 
			// 0 : Original EO spec
			// 1 : Ethan's updates with the 2 added SubTypes for FaceMask and HatNoHair

			if (version == 0 || version == 1)
			{
				Graphic = (short) Packet.DecodeNumber(rawData[0], rawData[1]);
				Type = (ItemType) Packet.DecodeNumber(rawData[2]);
				SubType = (ItemSubType) Packet.DecodeNumber(rawData[3]);

				Special = (ItemSpecial) Packet.DecodeNumber(rawData[4]);
				HP = (short) Packet.DecodeNumber(rawData[5], rawData[6]);
				TP = (short) Packet.DecodeNumber(rawData[7], rawData[8]);
				MinDam = (short) Packet.DecodeNumber(rawData[9], rawData[10]);
				MaxDam = (short) Packet.DecodeNumber(rawData[11], rawData[12]);
				Accuracy = (short) Packet.DecodeNumber(rawData[13], rawData[14]);
				Evade = (short) Packet.DecodeNumber(rawData[15], rawData[16]);
				Armor = (short) Packet.DecodeNumber(rawData[17], rawData[18]);

				Str = (byte) Packet.DecodeNumber(rawData[20]);
				Int = (byte) Packet.DecodeNumber(rawData[21]);
				Wis = (byte) Packet.DecodeNumber(rawData[22]);
				Agi = (byte) Packet.DecodeNumber(rawData[23]);
				Con = (byte) Packet.DecodeNumber(rawData[24]);
				Cha = (byte) Packet.DecodeNumber(rawData[25]);

				Light = (byte) Packet.DecodeNumber(rawData[26]);
				Dark = (byte) Packet.DecodeNumber(rawData[27]);
				Earth = (byte) Packet.DecodeNumber(rawData[28]);
				Air = (byte) Packet.DecodeNumber(rawData[29]);
				Water = (byte) Packet.DecodeNumber(rawData[30]);
				Fire = (byte) Packet.DecodeNumber(rawData[31]);

				ScrollMap = Packet.DecodeNumber(rawData[32], rawData[33], rawData[34]);
				ScrollX = (byte) Packet.DecodeNumber(rawData[35]);
				ScrollY = (byte) Packet.DecodeNumber(rawData[36]);

				LevelReq = (short) Packet.DecodeNumber(rawData[37], rawData[38]);
				ClassReq = (short) Packet.DecodeNumber(rawData[39], rawData[40]);

				StrReq = (short) Packet.DecodeNumber(rawData[41], rawData[42]);
				IntReq = (short) Packet.DecodeNumber(rawData[43], rawData[44]);
				WisReq = (short) Packet.DecodeNumber(rawData[45], rawData[46]);
				AgiReq = (short) Packet.DecodeNumber(rawData[47], rawData[48]);
				ConReq = (short) Packet.DecodeNumber(rawData[49], rawData[50]);
				ChaReq = (short) Packet.DecodeNumber(rawData[51], rawData[52]);

				Weight = (byte) Packet.DecodeNumber(rawData[55]);
				Size = (ItemSize) Packet.DecodeNumber(rawData[57]);

				if (ID == 365 && Name == "Gun")
					SubType = ItemSubType.Ranged;
			}
			else
			{
				throw new FileLoadException("Unable to Load file with invalid version: " + version);
			}
		}

		public byte[] SerializeToByteArray()
		{
			byte[] ret = new byte[ItemFile.DATA_SIZE + 1 + Name.Length];
			for (int i = 0; i < ret.Length; ++i)
				ret[i] = 254;

			using (MemoryStream mem = new MemoryStream(ret))
			{
				mem.WriteByte(Packet.EncodeNumber(Name.Length, 1)[0]);
				byte[] name = Encoding.ASCII.GetBytes(Name);
				mem.Write(name, 0, name.Length);

				mem.Write(Packet.EncodeNumber(Graphic, 2), 0, 2);
				mem.WriteByte(Packet.EncodeNumber((byte)Type, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber((byte)SubType, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber((byte)Special, 1)[0]);

				mem.Write(Packet.EncodeNumber(HP, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(TP, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(MinDam, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(MaxDam, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(Accuracy, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(Evade, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(Armor, 2), 0, 2);

				mem.Seek(21 + Name.Length, SeekOrigin.Begin);
				mem.WriteByte(Packet.EncodeNumber(Str, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Int, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Wis, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Agi, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Con, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Cha, 1)[0]);

				mem.WriteByte(Packet.EncodeNumber(Light, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Dark, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Earth, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Air, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Water, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(Fire, 1)[0]);

				mem.Write(Packet.EncodeNumber(ScrollMap, 3), 0, 3);
				mem.WriteByte(Packet.EncodeNumber(ScrollX, 1)[0]);
				mem.WriteByte(Packet.EncodeNumber(ScrollY, 1)[0]);

				mem.Write(Packet.EncodeNumber(LevelReq, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(ClassReq, 2), 0, 2);

				mem.Write(Packet.EncodeNumber(StrReq, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(IntReq, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(WisReq, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(AgiReq, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(ConReq, 2), 0, 2);
				mem.Write(Packet.EncodeNumber(ChaReq, 2), 0, 2);

				mem.Seek(56 + Name.Length, SeekOrigin.Begin);
				mem.WriteByte(Packet.EncodeNumber(Weight, 1)[0]);
				mem.Seek(58 + Name.Length, SeekOrigin.Begin);
				mem.WriteByte(Packet.EncodeNumber((byte)Size, 1)[0]);
			}

			return ret;
		}

		public EquipLocation GetEquipLocation()
		{
			switch (Type)
			{
				case ItemType.Accessory:
					return EquipLocation.Accessory;
				case ItemType.Armlet:
					return EquipLocation.Armlet1;
				case ItemType.Armor:
					return EquipLocation.Armor;
				case ItemType.Belt:
					return EquipLocation.Belt;
				case ItemType.Boots:
					return EquipLocation.Boots;
				case ItemType.Bracer:
					return EquipLocation.Bracer1;
				case ItemType.Gloves:
					return EquipLocation.Gloves;
				case ItemType.Hat:
					return EquipLocation.Hat;
				case ItemType.Necklace:
					return EquipLocation.Necklace;
				case ItemType.Ring:
					return EquipLocation.Ring1;
				case ItemType.Shield:
					return EquipLocation.Shield;
				case ItemType.Weapon:
					return EquipLocation.Weapon;
				default:
					return EquipLocation.PAPERDOLL_MAX;
			}
		}
	}

	public class ItemFile : EODataFile
	{
		public const int DATA_SIZE = 58;

		public ItemFile()
			: base(new ItemRecordFactory())
		{
			Load(FilePath = Constants.ItemFilePath);
		}

		public ItemFile(string path)
			: base(new ItemRecordFactory())
		{
			Load(FilePath = path);
		}

		public ItemRecord GetItemRecordByID(int id)
		{
			return (ItemRecord)Data.Find(_rec => ((ItemRecord)_rec).ID == id);
		}

		public ItemRecord GetItemRecordByDollGraphic(ItemType type, short dollGraphic)
		{
			return (ItemRecord)Data.Find(_rec => ((ItemRecord)_rec).DollGraphic == dollGraphic && ((ItemRecord)_rec).Type == type);
		}

		protected override int GetDataSize()
		{
			return DATA_SIZE;
		}
	}
}
