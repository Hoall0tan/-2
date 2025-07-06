using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssemblyCSharp.Mod.Xmap;

// Token: 0x0200000F RID: 15
internal class AutoBroly
{
	// Token: 0x0600004E RID: 78 RVA: 0x00003EA5 File Offset: 0x000020A5
	private static void Wait(int time)
	{
		AutoBroly.IsWait = true;
		AutoBroly.TimeStartWait = mSystem.currentTimeMillis();
		AutoBroly.TimeWait = (long)time;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003EBE File Offset: 0x000020BE
	private static bool IsWaiting()
	{
		if (AutoBroly.IsWait && mSystem.currentTimeMillis() - AutoBroly.TimeStartWait >= AutoBroly.TimeWait)
		{
			AutoBroly.IsWait = false;
		}
		return AutoBroly.IsWait;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000093B8 File Offset: 0x000075B8
	public static bool IsBoss()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00009424 File Offset: 0x00007624
	public static void SearchBoss()
	{
		int currentZone = TileMap.zoneID;
		int num = GameScr.gI().zones.Length;
		if (AutoBroly.IsBoss())
		{
			AutoBroly.visitedZones.Clear();
			return;
		}
		AutoBroly.visitedZones.Add(currentZone);
		List<int> list = (from z in Enumerable.Range(0, num)
			where z != currentZone && !AutoBroly.visitedZones.Contains(z)
			select z).ToList<int>();
		if (list.Count == 0)
		{
			AutoBroly.visitedZones.Clear();
			return;
		}
		int num2 = list[AutoBroly.random.Next(list.Count)];
		Service.gI().requestChangeZone(num2, -1);
	}

	// Token: 0x06000052 RID: 82 RVA: 0x000094C8 File Offset: 0x000076C8
	public static void FocusSuperBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHP > 0L && global::Char.myCharz().charFocus != @char)
			{
				global::Char.myCharz().npcFocus = null;
				global::Char.myCharz().mobFocus = null;
				global::Char.myCharz().charFocus = @char;
				return;
			}
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00009560 File Offset: 0x00007760
	public static void Update()
	{
		AutoPean.Update();
		if (!AutoBroly.IsWaiting())
		{
			if (global::Char.myCharz().cHP <= 0L || global::Char.myCharz().meDead)
			{
				if (AutoBroly.IsBoss())
				{
					AutoBroly.Map = TileMap.mapID;
					AutoBroly.Khu = TileMap.zoneID;
				}
				Service.gI().returnTownFromDead();
				AutoBroly.Wait(3000);
				return;
			}
			if (AutoBroly.Map != -1 && AutoBroly.Khu != -1 && TileMap.mapID == AutoBroly.Map && TileMap.zoneID == AutoBroly.Khu && !AutoBroly.IsBoss())
			{
				AutoBroly.Map = -1;
				AutoBroly.Khu = -1;
				AutoBroly.RemoveMyIDFromCanhBoss();
			}
			if (AutoBroly.IsBoss())
			{
				if (DataAccount.Type > 1)
				{
					AutoBroly.Map = TileMap.mapID;
					AutoBroly.Khu = TileMap.zoneID;
				}
				AutoBroly.TrangThai = "SP: " + TileMap.mapNames[TileMap.mapID].ToString() + " - " + TileMap.zoneID.ToString();
				if (AutoBroly.visitedZones.Count > 0)
				{
					AutoBroly.visitedZones.Clear();
				}
			}
			else
			{
				AutoBroly.TrangThai = "Không có thông tin ";
			}
			if (AutoBroly.Map != -1 && TileMap.mapID != AutoBroly.Map && !Pk9rXmap.IsXmapRunning)
			{
				XmapController.StartRunToMapId(AutoBroly.Map);
				AutoBroly.Wait(3000);
				return;
			}
			if (TileMap.mapID == AutoBroly.Map && TileMap.zoneID != AutoBroly.Khu && AutoBroly.Khu != -1)
			{
				Service.gI().requestChangeZone(AutoBroly.Khu, -1);
				AutoBroly.Wait(2000);
				return;
			}
			if (TileMap.mapID == AutoBroly.Map && TileMap.zoneID == AutoBroly.Khu && AutoBroly.IsBoss())
			{
				AutoBroly.FocusSuperBroly();
			}
			if (!AutoBroly.IsBoss() && AutoBroly.isDoKhu)
			{
				AutoBroly.SearchBoss_Smart();
				AutoBroly.Wait(2000);
				return;
			}
			if (DataAccount.Type == 1)
			{
				if (AutoBroly.NhayNe == 0 && AutoBroly.IsBoss())
				{
					AutoBroly.NhayNe = 1;
					AutoBroly.NhayCuoiMap();
					AutoBroly.Wait(1000);
					return;
				}
				if (!AutoBroly.IsBoss() && AutoBroly.NhayNe == 1)
				{
					AutoBroly.NhayNe = 0;
				}
			}
			if (DataAccount.Type == 3)
			{
				if (AutoBroly.NhayNe == 0 && !AutoBroly.IsBoss())
				{
					AutoBroly.NhayNe = 1;
					AutoBroly.NhayCuoiMap();
					AutoBroly.Wait(1000);
					return;
				}
				if (!AutoBroly.IsBoss() && AutoBroly.NhayNe == 1)
				{
					AutoBroly.NhayNe = 0;
				}
			}
			AutoBroly.AutoKichSP();
			AutoBroly.Wait(500);
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x000097B8 File Offset: 0x000079B8
	public static void NhayCuoiMap()
	{
		if (mSystem.currentTimeMillis() - AutoBroly.lastMoveTime >= 10000L)
		{
			global::Char @char = global::Char.myCharz();
			int num = @char.cx + (AutoBroly.isMovingRight ? AutoBroly.moveDistance : (-AutoBroly.moveDistance));
			int cy = @char.cy;
			num = AutoBroly.MyMax(50, AutoBroly.MyMin(num, 3800));
			KsSupper.TelePortTo(num, cy);
			AutoBroly.isMovingRight = !AutoBroly.isMovingRight;
			AutoBroly.lastMoveTime = mSystem.currentTimeMillis();
		}
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00009830 File Offset: 0x00007A30
	public static bool IsBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && !@char.cName.Contains("Super"))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00009890 File Offset: 0x00007A90
	public static void Painting(mGraphics g)
	{
		string text = TileMap.mapNames[TileMap.mapID];
		string text2 = " - " + TileMap.zoneID.ToString();
		string text3 = NinjaUtil.getMoneys(global::Char.myCharz().cHP).ToString() + " / " + NinjaUtil.getMoneys(global::Char.myCharz().cHPFull).ToString();
		string text4 = NinjaUtil.getMoneys(global::Char.myCharz().cMP).ToString() + " / " + NinjaUtil.getMoneys(global::Char.myCharz().cMPFull).ToString();
		if (AutoBroly.IsBoss())
		{
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
				if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L)
				{
					string text5 = string.Concat(new string[]
					{
						@char.cName,
						" [ ",
						NinjaUtil.getMoneys(@char.cHP),
						" / ",
						NinjaUtil.getMoneys(@char.cHPFull),
						" ]"
					});
					mFont.tahoma_7b_yellow.drawString(g, text5, 20, GameCanvas.h - (GameCanvas.h - GameCanvas.h / 3), 0);
				}
			}
		}
		if (AutoBroly.IsBroly())
		{
			for (int j = 0; j < GameScr.vCharInMap.size(); j++)
			{
				global::Char char2 = (global::Char)GameScr.vCharInMap.elementAt(j);
				if (char2 != null && char2.cName.Contains("Broly") && !char2.cName.Contains("Super"))
				{
					string text6 = string.Concat(new string[]
					{
						char2.cName,
						" [ ",
						NinjaUtil.getMoneys(char2.cHP),
						" / ",
						NinjaUtil.getMoneys(char2.cHPFull),
						" ]"
					});
					mFont.tahoma_7b_white.drawString(g, text6, 20, GameCanvas.h - (GameCanvas.h - GameCanvas.h / 3), 0);
				}
			}
		}
		mFont.tahoma_7b_white.drawString(g, "HP: " + text3, 30, GameCanvas.h - (GameCanvas.h - 25), 0);
		mFont.tahoma_7b_white.drawString(g, "MP: " + text4, 30, GameCanvas.h - (GameCanvas.h - 35), 0);
		mFont.tahoma_7b_white.drawString(g, text + " " + text2 + " ", 30, GameCanvas.h - (GameCanvas.h - 10), 0);
		mFont.tahoma_7b_white.drawString(g, AutoBroly.Map.ToString() + " " + AutoBroly.Khu.ToString() + " ", GameCanvas.w - 30, GameCanvas.h - (GameCanvas.h - 10), 0);
		if (DataAccount.Type == 3)
		{
			int mySearchOrder = AutoBroly.GetMySearchOrder();
			int totalSearchers = AutoBroly.GetTotalSearchers();
			if (mySearchOrder != -1 && totalSearchers > 0)
			{
				string text7 = string.Format("Acc dò: {0}/{1}", mySearchOrder + 1, totalSearchers);
				mFont.tahoma_7b_white.drawString(g, text7, 30, GameCanvas.h - (GameCanvas.h - 45), 0);
			}
		}
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003EEC File Offset: 0x000020EC
	private static int MyMax(int a, int b)
	{
		if (a <= b)
		{
			return b;
		}
		return a;
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00003EF5 File Offset: 0x000020F5
	private static int MyMin(int a, int b)
	{
		if (a >= b)
		{
			return b;
		}
		return a;
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00009C9C File Offset: 0x00007E9C
	private static double MySqrt(double x)
	{
		if (x < 0.0)
		{
			return double.NaN;
		}
		double num = x;
		double num2;
		do
		{
			num2 = num;
			num = (num + x / num) / 2.0;
		}
		while (global::System.Math.Abs(num - num2) > 1E-07);
		return num;
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00009CE8 File Offset: 0x00007EE8
	public static void AutoKichSP()
	{
		if (DataAccount.Type != 1)
		{
			return;
		}
		global::Char @char = global::Char.myCharz();
		global::Char char2 = null;
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char char3 = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (char3 != null && char3.cName.Contains("Broly") && !char3.cName.Contains("Super"))
			{
				char2 = char3;
				break;
			}
		}
		if (char2 == null)
		{
			return;
		}
		int num = @char.cx - char2.cx;
		int num2 = @char.cy - char2.cy;
		int num3 = (int)AutoBroly.MySqrt((double)(num * num + num2 * num2));
		long num4 = mSystem.currentTimeMillis();
		int num5 = 0;
		int num6 = 3840;
		int num7 = 100;
		int num8 = 10;
		if (num3 <= num7)
		{
			if (num4 - AutoBroly.lastAutoKichSPTime >= 500L)
			{
				int num9 = @char.cx;
				if (@char.cx <= num5 + num8 && @char.cx <= char2.cx)
				{
					num9 = char2.cx + num7;
					if (num9 > num6 - num8)
					{
						num9 = num6 - num8;
					}
				}
				else if (@char.cx >= num6 - num8 && @char.cx >= char2.cx)
				{
					num9 = char2.cx - num7;
					if (num9 < num5 + num8)
					{
						num9 = num5 + num8;
					}
				}
				else if (@char.cx < char2.cx)
				{
					num9 = char2.cx - num7;
					if (num9 < num5 + num8)
					{
						num9 = num5 + num8;
					}
				}
				else if (@char.cx > char2.cx)
				{
					num9 = char2.cx + num7;
					if (num9 > num6 - num8)
					{
						num9 = num6 - num8;
					}
				}
				KsSupper.TelePortTo(num9, @char.cy);
				AutoBroly.lastAutoKichSPTime = num4;
				return;
			}
		}
		else
		{
			AutoBroly.lastAutoKichSPTime = num4;
		}
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00009EAC File Offset: 0x000080AC
	public static void SearchBoss_Smart()
	{
		int currentZone = TileMap.zoneID;
		int num = GameScr.gI().zones.Length;
		List<int> list = AutoBroly.ReadCanhBossFile();
		List<int> list2 = (from x in AutoBroly.AllOnlineAccIDs.Except(list)
			orderby x
			select x).ToList<int>();
		int tongSoAcc = list2.Count;
		if (tongSoAcc == 0)
		{
			return;
		}
		int myOrder = list2.IndexOf(AutoBroly.MyQLTK_ID);
		if (myOrder == -1)
		{
			return;
		}
		List<int> list3 = (from z in Enumerable.Range(2, num - 2)
			where z != currentZone && !AutoBroly.visitedZones.Contains(z)
			where (z - 2) % tongSoAcc == myOrder
			select z).ToList<int>();
		if (AutoBroly.IsBoss())
		{
			AutoBroly.AddMyIDToCanhBoss();
			AutoBroly.visitedZones.Clear();
			return;
		}
		AutoBroly.RemoveMyIDFromCanhBoss();
		AutoBroly.visitedZones.Add(currentZone);
		if (list3.Count == 0)
		{
			AutoBroly.visitedZones.Clear();
			return;
		}
		int num2 = list3.FirstOrDefault((int z) => z < num);
		if (num2 == 0)
		{
			return;
		}
		Service.gI().chat("Tôi là acc dò số " + (myOrder + 1).ToString() + "/" + tongSoAcc.ToString());
		Service.gI().requestChangeZone(num2, -1);
	}

	// Token: 0x0600005E RID: 94 RVA: 0x0000A018 File Offset: 0x00008218
	public static List<int> ReadCanhBossFile()
	{
		List<int> list;
		try
		{
			if (!File.Exists(AutoBroly.QLTK_ID_File))
			{
				list = new List<int>();
			}
			else
			{
				list = (from id in File.ReadAllLines(AutoBroly.QLTK_ID_File).Select(delegate(string line)
					{
						int num;
						if (!int.TryParse(line, out num))
						{
							return -1;
						}
						return num;
					})
					where id >= 0
					select id).ToList<int>();
			}
		}
		catch
		{
			list = new List<int>();
		}
		return list;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x0000A0B0 File Offset: 0x000082B0
	public static void WriteCanhBossFile(List<int> ids)
	{
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(AutoBroly.QLTK_ID_File));
			File.WriteAllLines(AutoBroly.QLTK_ID_File, ids.Select((int id) => id.ToString()).ToArray<string>());
		}
		catch
		{
		}
	}

	// Token: 0x06000060 RID: 96 RVA: 0x0000A118 File Offset: 0x00008318
	public static void AddMyIDToCanhBoss()
	{
		List<int> list = AutoBroly.ReadCanhBossFile();
		if (!list.Contains(AutoBroly.MyQLTK_ID))
		{
			list.Add(AutoBroly.MyQLTK_ID);
			AutoBroly.WriteCanhBossFile(list);
		}
	}

	// Token: 0x06000061 RID: 97 RVA: 0x0000A14C File Offset: 0x0000834C
	public static void RemoveMyIDFromCanhBoss()
	{
		List<int> list = AutoBroly.ReadCanhBossFile();
		if (list.Contains(AutoBroly.MyQLTK_ID))
		{
			list.Remove(AutoBroly.MyQLTK_ID);
			AutoBroly.WriteCanhBossFile(list);
		}
	}

	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000062 RID: 98 RVA: 0x00003EFE File Offset: 0x000020FE
	public static int MyQLTK_ID
	{
		get
		{
			return DataAccount.ID;
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x0000A180 File Offset: 0x00008380
	private static int GetMyOrder()
	{
		return (from id in AutoBroly.AllOnlineAccIDs.Except(AutoBroly.ReadCanhBossFile())
			orderby id
			select id).ToList<int>().IndexOf(AutoBroly.MyQLTK_ID);
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003F05 File Offset: 0x00002105
	public static int GetTotalSearchers()
	{
		return AutoBroly.cachedOnlineIDs.Count;
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00003F11 File Offset: 0x00002111
	public static int GetMySearchOrder()
	{
		return AutoBroly.cachedOnlineIDs.IndexOf(DataAccount.ID);
	}

	// Token: 0x06000066 RID: 102 RVA: 0x0000A1D0 File Offset: 0x000083D0
	private static void UpdateOnlineStatus()
	{
		long num = mSystem.currentTimeMillis();
		if (num - AutoBroly.lastCheckOnlineTime < 60000L)
		{
			return;
		}
		AutoBroly.lastCheckOnlineTime = num;
		IEnumerable<int> enumerable = AutoBroly.ReadOnlineFile();
		List<int> list = AutoBroly.ReadCanhBossFile();
		AutoBroly.cachedOnlineIDs = (from id in enumerable.Except(list)
			orderby id
			select id).ToList<int>();
	}

	// Token: 0x06000067 RID: 103 RVA: 0x0000A238 File Offset: 0x00008438
	public static List<int> ReadOnlineFile()
	{
		List<int> list;
		try
		{
			string text = "Nro_244_Data\\Resources\\Data\\online.txt";
			if (!File.Exists(text))
			{
				list = new List<int>();
			}
			else
			{
				list = (from id in File.ReadAllLines(text).Select(delegate(string line)
					{
						int num;
						if (!int.TryParse(line.Trim(), out num))
						{
							return -1;
						}
						return num;
					})
					where id != -1
					select id).Distinct<int>().ToList<int>();
			}
		}
		catch
		{
			list = new List<int>();
		}
		return list;
	}

	// Token: 0x06000068 RID: 104 RVA: 0x0000A2D0 File Offset: 0x000084D0
	public static void AddIDToOnlineFile()
	{
		if (DataAccount.Type != 3)
		{
			return;
		}
		try
		{
			string text = "Nro_244_Data\\Resources\\Data\\online.txt";
			Directory.CreateDirectory(Path.GetDirectoryName(text));
			List<int> list;
			if (!File.Exists(text))
			{
				list = new List<int>();
			}
			else
			{
				list = (from id in File.ReadAllLines(text).Select(delegate(string line)
					{
						int num;
						if (!int.TryParse(line.Trim(), out num))
						{
							return -1;
						}
						return num;
					})
					where id != -1
					select id).ToList<int>();
			}
			if (!list.Contains(DataAccount.ID))
			{
				File.AppendAllText(text, DataAccount.ID.ToString() + Environment.NewLine);
			}
		}
		catch
		{
		}
	}

	// Token: 0x06000069 RID: 105 RVA: 0x0000A39C File Offset: 0x0000859C
	public static void RemoveIDFromOnlineFile()
	{
		try
		{
			string text = "Nro_244_Data\\Resources\\Data\\online.txt";
			if (File.Exists(text))
			{
				List<string> list = File.ReadAllLines(text).Where(delegate(string line)
				{
					int num;
					return int.TryParse(line.Trim(), out num) && num != DataAccount.ID;
				}).ToList<string>();
				File.WriteAllLines(text, list.ToArray());
			}
		}
		catch
		{
		}
	}

	// Token: 0x0600006A RID: 106 RVA: 0x0000A408 File Offset: 0x00008608
	public static void RemoveFromCanhBoss()
	{
		try
		{
			if (File.Exists(AutoBroly.canhbossPath))
			{
				List<string> list = File.ReadAllLines(AutoBroly.canhbossPath).ToList<string>();
				if (list.Contains(DataAccount.ID.ToString()))
				{
					list.Remove(DataAccount.ID.ToString());
					File.WriteAllLines(AutoBroly.canhbossPath, list.ToArray());
				}
			}
		}
		catch
		{
		}
	}

	// Token: 0x0400004D RID: 77
	public static string TrangThai = "Không có thông tin";

	// Token: 0x0400004E RID: 78
	public static int Map = -1;

	// Token: 0x0400004F RID: 79
	public static int Khu = -1;

	// Token: 0x04000050 RID: 80
	private static bool IsWait;

	// Token: 0x04000051 RID: 81
	private static long TimeStartWait;

	// Token: 0x04000052 RID: 82
	private static long TimeWait;

	// Token: 0x04000053 RID: 83
	public static bool isDoKhu = false;

	// Token: 0x04000054 RID: 84
	private static HashSet<int> visitedZones = new HashSet<int>();

	// Token: 0x04000055 RID: 85
	private static Random random = new Random();

	// Token: 0x04000056 RID: 86
	public static int NhayNe = 0;

	// Token: 0x04000057 RID: 87
	private static long lastAutoKichSPTime = 0L;

	// Token: 0x04000058 RID: 88
	public static List<int> AllOnlineAccIDs = new List<int>();

	// Token: 0x04000059 RID: 89
	private static string QLTK_ID_File = "Nro_244_Data\\Resources\\Data\\canhboss.txt";

	// Token: 0x0400005A RID: 90
	private static bool IsCanhBoss = false;

	// Token: 0x0400005B RID: 91
	private static bool isMovingRight = true;

	// Token: 0x0400005C RID: 92
	private static long lastMoveTime = 0L;

	// Token: 0x0400005D RID: 93
	private static int moveDistance = 30;

	// Token: 0x0400005E RID: 94
	private static long lastCheckOnlineTime = 0L;

	// Token: 0x0400005F RID: 95
	private static List<int> cachedOnlineIDs = new List<int>();

	// Token: 0x04000060 RID: 96
	private static string canhbossPath = "Nro_244_Data\\Resources\\Data\\canhboss.txt";
}
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

// Token: 0x02000088 RID: 136
public class DataAccount
{
	// Token: 0x0600065A RID: 1626 RVA: 0x0006DED4 File Offset: 0x0006C0D4
	public static void Doc()
	{
		try
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			bool flag = commandLineArgs.Length == 0;
			if (!flag)
			{
				string text = null;
				string text2 = null;
				string text3 = null;
				string text4 = null;
				string text5 = null;
				string text6 = null;
				string text7 = null;
				for (int i = 0; i < commandLineArgs.Length; i++)
				{
					string text8 = commandLineArgs[i];
					string text9 = text8;
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text9);
					if (num <= 1405861912U)
					{
						if (num != 915181618U)
						{
							if (num != 1135419332U)
							{
								if (num == 1405861912U)
								{
									if (text9 == "--acc")
									{
										bool flag2 = i + 1 < commandLineArgs.Length;
										if (flag2)
										{
											text = commandLineArgs[i + 1];
											i++;
										}
									}
								}
							}
							else if (text9 == "--server")
							{
								bool flag3 = i + 1 < commandLineArgs.Length;
								if (flag3)
								{
									text3 = commandLineArgs[i + 1];
									i++;
								}
							}
						}
						else if (text9 == "--pass")
						{
							bool flag4 = i + 1 < commandLineArgs.Length;
							if (flag4)
							{
								text2 = commandLineArgs[i + 1];
								i++;
							}
						}
					}
					else if (num <= 2635071343U)
					{
						if (num != 2357181198U)
						{
							if (num == 2635071343U)
							{
								if (text9 == "--prx")
								{
									bool flag5 = i + 1 < commandLineArgs.Length;
									if (flag5)
									{
										text5 = commandLineArgs[i + 1];
										i++;
									}
								}
							}
						}
						else if (text9 == "--team")
						{
							bool flag6 = i + 1 < commandLineArgs.Length;
							if (flag6)
							{
								text7 = commandLineArgs[i + 1];
								i++;
							}
						}
					}
					else if (num != 4071210487U)
					{
						if (num == 4142942274U)
						{
							if (text9 == "--id")
							{
								bool flag7 = i + 1 < commandLineArgs.Length;
								if (flag7)
								{
									text4 = commandLineArgs[i + 1];
									i++;
								}
							}
						}
					}
					else if (text9 == "--type")
					{
						bool flag8 = i + 1 < commandLineArgs.Length;
						if (flag8)
						{
							text6 = commandLineArgs[i + 1];
							i++;
						}
					}
				}
				DataAccount.Account = text;
				DataAccount.PassWord = text2;
				DataAccount.Server = int.Parse(text3) - 1;
				DataAccount.Team = int.Parse(text7);
				DataAccount.Type = int.Parse(text6.Split(new char[] { '.' })[0]);
				DataAccount.Proxy = text5;
				DataAccount.ID = int.Parse(text4);
				ThreadStart threadStart;
				if ((threadStart = DataAccount.<>O.<0>__Login) == null)
				{
					threadStart = (DataAccount.<>O.<0>__Login = new ThreadStart(DataAccount.Login));
				}
				new Thread(threadStart).Start();
			}
		}
		catch
		{
			DataAccount.startOKDlgWithStar("");
		}
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x0006E1E0 File Offset: 0x0006C3E0
	public static void startOKDlgWithStar(string message)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = new string[]
		{
			"................................:-....:-==**=:..................:...................................", ".................................=:.        ..-+=-:..............=-:................................", ".................................-=.            ..-+-............=-:=-..............................", "..................................=:              ...++:.........--..-+.............................", "..................................-+.                 .-+-:......=-.  .*-...........................", "...................................*:                   .:==:....=:    .==..........................", "...................................-+.                    ..-=:..+.     .==.........................", "....................::::--------::::=.                       .::-+.       -+:.......................", "..............:====--:::..............                         .=:         :+:......................", "...........--:....                                             ...         .-=.....::...............",
			"..........:+-.                                       .:.....                .=-....=+:..............", "............:+=.                                     ..::::...               .*:..:=.+-.............", "..............:+:.                                    ...:...:..              .=.-*. .*:............", "...............:=-.                                      ........              :==.   -=............", ".................-+.                                       ...  .             .:..    :=:...........", ".................:::.   .....                                                         .=:...........", "...........:-=*+:.  ..::::.....                                                       .=:...........", ".......:=+=:..       ...:....                                                         .+:...........", ".....-+-..             ......                                                         .=:...........", "...:=..                                                                               :=............",
			"...:-=+=:.                                              ........                      --............", "........:=+:.                                    ......:::::::::::........            -:-+*++===-...", "...........:+=.                         ......::::::::::::::::::::::::::::::....    .-=-::::=*-.....", ".............:-.                    .....::::::::::::::::::::-------:::::::::::......::::-+=:.......", "...........:=-.                 ....:::::::::::::::::-=*##%%%%%%%%%%%%##*=::::::::.::::-*-..........", "...........+-.              ....::::::::::::::-=*##%%%%%%%%%%%%%%%%%%%%%%%%%#+-:::::::=+:...........", ".........:*:             ...:::::::::::::-+*%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%*-:::+=.............", "........:+:           ...:::::::::::-+*#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#---..............", "........=-.        ...:::::::::-+#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%+...............", ".......-=. .--:.  ..:::::-=*#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%*:..............",
			".......*::+=::+  ..:::=*#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%*:..............", ".......==-...-*..::-#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%+...............", ".......-:....-+.::+#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%+...............", ".............:*::*:-*%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%*:..............", ".............:#:=-..-#%%%%%%%#*++====+++***########%%%%%%%%%%%%%%%%%%%%%%%%####*=:..:-..............", "..............-*=:...*%#**:..:.......................................................+..............", "...............:-....:=-*:..................................:....::.................-+..............", "......................:+++...................................:+**:.................:+:..............", "........................=#-.......................................................:+:...............", ".........................-**++++-...............................................-+-.................",
			"..........................=*+*-:*#+-:........................................-#*:...................", "..........................:+=:+=*=#@@@%%@%%%%%%%%%###*++====----------:::-=*%%%#-...................", "...........................:...-:.:+%@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@@%@@@@@%=:....................", "...................................:#@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@@@@@@@%*:......................", "..................................=%@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%%%%%%%@%#:.......:=+:.............", ".................................=%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@%*:....=+==:+=:...........", "................................:%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@%*:...-##+:=*-...........", "...............................:#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@%+...=*++-=+-...........", "..............................:*%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@@%-..-+-::.:=...........", "..............................-#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#@@@@@@#::*.:=..--...........",
			"................-=------=---::-%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@@@@%-:+.--..::..........."
		};
		foreach (string text in array)
		{
			stringBuilder.Append(text).Append("\n");
		}
		string text2 = message + "\n" + stringBuilder.ToString();
		GameCanvas.startOKDlg(text2, true);
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0006E408 File Offset: 0x0006C608
	public static void Login()
	{
		Thread.Sleep(1000);
		for (;;)
		{
			try
			{
				bool flag = string.IsNullOrEmpty(global::Char.myCharz().cName);
				if (flag)
				{
					Thread.Sleep(1000);
					while (!ServerListScreen.loadScreen)
					{
						Thread.Sleep(10);
					}
					Thread.Sleep(500);
					ServerListScreen.ipSelect = DataAccount.Server;
					bool flag2 = ServerListScreen.nameServer[ServerListScreen.ipSelect].Replace(" ", "") != DataAccount.Server.ToString();
					if (flag2)
					{
						GameCanvas.serverScreen.selectServer();
						while (!ServerListScreen.loadScreen)
						{
							Thread.Sleep(10);
						}
						while (!Session_ME.gI().isConnected())
						{
							Thread.Sleep(100);
						}
						Thread.Sleep(100);
						while (!ServerListScreen.loadScreen)
						{
							Thread.Sleep(10);
						}
					}
					Thread.Sleep(1000);
					GameCanvas.serverScreen.perform(3, null);
					Thread.Sleep(30000);
				}
			}
			catch
			{
			}
			Thread.Sleep(5000);
		}
	}

	// Token: 0x04000B7A RID: 2938
	public static string Account;

	// Token: 0x04000B7B RID: 2939
	public static string PassWord;

	// Token: 0x04000B7C RID: 2940
	public static int Server;

	// Token: 0x04000B7D RID: 2941
	public static int Type;

	// Token: 0x04000B7E RID: 2942
	public static string Proxy;

	// Token: 0x04000B7F RID: 2943
	public static int ID;

	// Token: 0x04000B80 RID: 2944
	public static int Team;

	// Token: 0x02000089 RID: 137
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x04000B81 RID: 2945
		public static ThreadStart <0>__Login;
	}
}
using System;
using System.IO;
using System.Threading;
using AssemblyCSharp.Mod.Xmap;

// Token: 0x02000017 RID: 23
internal class DovaBaoKhu
{
	// Token: 0x06000097 RID: 151 RVA: 0x00004015 File Offset: 0x00002215
	private static void Wait(int time)
	{
		DovaBaoKhu.IsWait = true;
		DovaBaoKhu.TimeStartWait = mSystem.currentTimeMillis();
		DovaBaoKhu.TimeWait = (long)time;
	}

	// Token: 0x06000098 RID: 152 RVA: 0x0000402E File Offset: 0x0000222E
	private static bool IsWaiting()
	{
		if (DovaBaoKhu.IsWait && mSystem.currentTimeMillis() - DovaBaoKhu.TimeStartWait >= DovaBaoKhu.TimeWait)
		{
			DovaBaoKhu.IsWait = false;
		}
		return DovaBaoKhu.IsWait;
	}

	// Token: 0x06000099 RID: 153 RVA: 0x0000AAE8 File Offset: 0x00008CE8
	public static void GetMapBoss()
	{
		if (DovaBaoKhu.TbBoss != null && DovaBaoKhu.TbBoss.Contains("BOSS") && DovaBaoKhu.TbBoss.Contains("Broly"))
		{
			DovaBaoKhu.TbBoss = DovaBaoKhu.TbBoss.Replace("Boss ", "");
			DovaBaoKhu.TbBoss = DovaBaoKhu.TbBoss.Replace(" vừa xuất hiện tại", "|");
			DovaBaoKhu.TbBoss = DovaBaoKhu.TbBoss.Replace("Khu ", "|");
			AutoBroly.Map = DovaBaoKhu.MapID(DovaBaoKhu.TbBoss.Split(new char[] { '|' })[1].Trim());
			DovaBaoKhu.TbBoss = "";
		}
	}

	// Token: 0x0600009A RID: 154 RVA: 0x000093B8 File Offset: 0x000075B8
	public static bool IsBoss()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00004054 File Offset: 0x00002254
	public static void Out()
	{
		File.Create(string.Format("Nro_244_Data/Resources/Status/xong{0}", DataAccount.ID)).Close();
		Thread.Sleep(10000);
		Main.exit();
		DovaBaoKhu.IsPet = false;
	}

	// Token: 0x0600009C RID: 156 RVA: 0x0000ABA0 File Offset: 0x00008DA0
	public static int MapID(string a)
	{
		for (int i = 0; i < TileMap.mapNames.Length; i++)
		{
			if (TileMap.mapNames[i].Equals(a))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600009D RID: 157 RVA: 0x0000ABD4 File Offset: 0x00008DD4
	public static void Update()
	{
		if (!DovaBaoKhu.IsWaiting())
		{
			if (global::Char.myCharz().cHP <= 0L || global::Char.myCharz().meDead)
			{
				if (DovaBaoKhu.IsBoss())
				{
					AutoBroly.Map = TileMap.mapID;
					AutoBroly.Khu = TileMap.zoneID;
				}
				Service.gI().returnTownFromDead();
				DovaBaoKhu.Wait(3000);
				return;
			}
			if (global::Char.myCharz().havePet)
			{
				AutoBroly.TrangThai = "Đã có Đệ";
				if (!DovaBaoKhu.IsPet)
				{
					DovaBaoKhu.IsPet = true;
					new Thread(new ThreadStart(DovaBaoKhu.Out)).Start();
				}
				DovaBaoKhu.Wait(500);
				return;
			}
			if (DovaBaoKhu.IsBoss())
			{
				if (!DovaBaoKhu.IsInvalidBossHP())
				{
					DovaBaoKhu.timeInvalidHP = 0L;
					string text = string.Format("Super Broly - {0} khu {1} - [{2:yyyy-MM-dd HH:mm:ss}]", TileMap.mapNames[TileMap.mapID], TileMap.zoneID, DateTime.Now);
					File.WriteAllText("Nro_244_Data//Resources//thongbao", text);
					DovaBaoKhu.Wait(3000);
					return;
				}
				if (DovaBaoKhu.timeInvalidHP == 0L)
				{
					DovaBaoKhu.timeInvalidHP = mSystem.currentTimeMillis();
				}
				else if (mSystem.currentTimeMillis() - DovaBaoKhu.timeInvalidHP >= 5000L)
				{
					AutoBroly.Map = -1;
					AutoBroly.Khu = -1;
					Service.gI().requestChangeZone(TileMap.zoneID + 1, -1);
					DovaBaoKhu.timeInvalidHP = 0L;
					DovaBaoKhu.Wait(3000);
					return;
				}
			}
			if (File.Exists("Nro_244_Data//Resources//dokhu"))
			{
				if (!DovaBaoKhu.IsBoss() && !Pk9rXmap.IsXmapRunning)
				{
					DovaBaoKhu.GetMapBoss();
				}
				if (!DovaBaoKhu.IsBoss())
				{
					AutoBroly.RemoveFromCanhBoss();
					AutoBroly.TrangThai = "Đang dò boss";
					if (TileMap.mapID == AutoBroly.Map)
					{
						AutoBroly.SearchBoss_Smart();
					}
					DovaBaoKhu.Wait(2000);
					return;
				}
			}
			DovaBaoKhu.Wait(500);
		}
	}

	// Token: 0x0600009F RID: 159 RVA: 0x0000AD84 File Offset: 0x00008F84
	private static bool IsInvalidBossHP()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super"))
			{
				long cHP = @char.cHP;
				if (cHP >= 10L && cHP <= 16070767L)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0400007F RID: 127
	private static bool IsWait;

	// Token: 0x04000080 RID: 128
	private static long TimeStartWait;

	// Token: 0x04000081 RID: 129
	private static long TimeWait;

	// Token: 0x04000082 RID: 130
	public static string TbBoss;

	// Token: 0x04000083 RID: 131
	public static bool isDoKhu;

	// Token: 0x04000084 RID: 132
	public static bool IsPet;

	// Token: 0x04000085 RID: 133
	private static long timeInvalidHP;
}
using System;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class GameMidlet
{
	// Token: 0x06000465 RID: 1125 RVA: 0x00004CD7 File Offset: 0x00002ED7
	public GameMidlet()
	{
		this.initGame();
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x000527AC File Offset: 0x000509AC
	public void initGame()
	{
		GameMidlet.instance = this;
		MotherCanvas.instance = new MotherCanvas();
		Session_ME.gI().setHandler(Controller.gI());
		Session_ME2.gI().setHandler(Controller.gI());
		Session_ME2.isMainSession = false;
		GameMidlet.instance = this;
		GameMidlet.gameCanvas = new GameCanvas();
		GameMidlet.gameCanvas.start();
		SplashScr.loadImg();
		SplashScr.loadSplashScr();
		GameCanvas.currentScreen = new SplashScr();
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0005281C File Offset: 0x00050A1C
	public void exit()
	{
		try
		{
			if (DataAccount.Type == 3)
			{
				AutoBroly.RemoveIDFromOnlineFile();
			}
		}
		catch
		{
		}
		if (Main.typeClient == 6)
		{
			mSystem.exitWP();
			return;
		}
		GameCanvas.bRun = false;
		mSystem.gcc();
		this.notifyDestroyed();
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00004CE5 File Offset: 0x00002EE5
	public static void sendSMS(string data, string to, Command successAction, Command failAction)
	{
		Cout.println("SEND SMS");
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x00004CF1 File Offset: 0x00002EF1
	public static void flatForm(string url)
	{
		Cout.LogWarning("PLATFORM REQUEST: " + url);
		Application.OpenURL(url);
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00004D09 File Offset: 0x00002F09
	public void notifyDestroyed()
	{
		Main.exit();
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00004D10 File Offset: 0x00002F10
	public void platformRequest(string url)
	{
		Cout.LogWarning("PLATFORM REQUEST: " + url);
		Application.OpenURL(url);
	}

	// Token: 0x0400076C RID: 1900
	public static string IP = "112.213.94.23";

	// Token: 0x0400076D RID: 1901
	public static int PORT = 14445;

	// Token: 0x0400076E RID: 1902
	public static string IP2;

	// Token: 0x0400076F RID: 1903
	public static int PORT2;

	// Token: 0x04000770 RID: 1904
	public static sbyte PROVIDER;

	// Token: 0x04000771 RID: 1905
	public static int LANGUAGE;

	// Token: 0x04000772 RID: 1906
	public static string VERSION = "2.4.3";

	// Token: 0x04000773 RID: 1907
	public static int intVERSION = 243;

	// Token: 0x04000774 RID: 1908
	public static GameCanvas gameCanvas;

	// Token: 0x04000775 RID: 1909
	public static GameMidlet instance;

	// Token: 0x04000776 RID: 1910
	public static bool isConnect2;

	// Token: 0x04000777 RID: 1911
	public static bool isBackWindowsPhone;
}
using System;
using System.Threading;
using AssemblyCSharp.Mod.Xmap;
using Assets.src.g;

// Token: 0x02000060 RID: 96
public class GameScr : mScreen, IChatable
{
	// Token: 0x06000474 RID: 1140 RVA: 0x00053274 File Offset: 0x00051474
	public GameScr()
	{
		bool flag = GameCanvas.w == 128 || GameCanvas.h <= 208;
		if (flag)
		{
			GameScr.indexSize = 20;
		}
		this.cmdback = new Command(string.Empty, 11021);
		this.cmdMenu = new Command("menu", 11000);
		this.cmdFocus = new Command(string.Empty, 11001);
		this.cmdMenu.img = GameScr.imgMenu;
		this.cmdMenu.w = mGraphics.getImageWidth(this.cmdMenu.img) + 20;
		this.cmdMenu.isPlaySoundButton = false;
		this.cmdFocus.img = GameScr.imgFocus;
		bool isTouch = GameCanvas.isTouch;
		if (isTouch)
		{
			this.cmdMenu.x = 0;
			this.cmdMenu.y = 50;
			this.cmdFocus = null;
		}
		else
		{
			this.cmdMenu.x = 0;
			this.cmdMenu.y = GameScr.gH - 30;
			this.cmdFocus.x = GameScr.gW - 32;
			this.cmdFocus.y = GameScr.gH - 32;
		}
		this.right = this.cmdFocus;
		GameScr.isPaintRada = 1;
		bool isTouch2 = GameCanvas.isTouch;
		if (isTouch2)
		{
			GameScr.isHaveSelectSkill = true;
		}
		this.cmdDoiCo = new Command("Đổi cờ", GameCanvas.gI(), 100001, null);
		this.cmdLogOut = new Command("Logout", GameCanvas.gI(), 100002, null);
		this.cmdChatTheGioi = new Command("chat world", GameCanvas.gI(), 100003, null);
		this.cmdshowInfo = new Command("InfoLog", GameCanvas.gI(), 100004, null);
		this.cmdDoiCo.setType();
		this.cmdLogOut.setType();
		this.cmdChatTheGioi.setType();
		this.cmdshowInfo.setType();
		this.cmdChatTheGioi.x = GameCanvas.w - this.cmdChatTheGioi.w;
		this.cmdshowInfo.x = GameCanvas.w - this.cmdshowInfo.w;
		this.cmdLogOut.x = GameCanvas.w - this.cmdLogOut.w;
		this.cmdDoiCo.x = GameCanvas.w - this.cmdDoiCo.w;
		this.cmdChatTheGioi.y = this.cmdChatTheGioi.h + mFont.tahoma_7_white.getHeight();
		this.cmdshowInfo.y = this.cmdChatTheGioi.h * 2 + mFont.tahoma_7_white.getHeight();
		this.cmdLogOut.y = this.cmdChatTheGioi.h * 3 + mFont.tahoma_7_white.getHeight();
		this.cmdDoiCo.y = this.cmdChatTheGioi.h * 4 + mFont.tahoma_7_white.getHeight();
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x000535EC File Offset: 0x000517EC
	public static void loadBg()
	{
		GameScr.fra_PVE_Bar_0 = new FrameImage(mSystem.loadImage("/mainImage/i_pve_bar_0.png"), 6, 15);
		GameScr.fra_PVE_Bar_1 = new FrameImage(mSystem.loadImage("/mainImage/i_pve_bar_1.png"), 38, 21);
		GameScr.imgVS = mSystem.loadImage("/mainImage/i_vs.png");
		GameScr.imgHP_NEW = mSystem.loadImage("/mainImage/i_hp.png");
		GameScr.imgKhung = mSystem.loadImage("/mainImage/i_khung.png");
		GameScr.imgMenu = GameCanvas.loadImage("/mainImage/myTexture2dmenu.png");
		GameScr.imgFocus = GameCanvas.loadImage("/mainImage/myTexture2dfocus.png");
		GameScr.imgHP_tm_do = GameCanvas.loadImage("/mainImage/tm-do.png");
		GameScr.imgHP_tm_vang = GameCanvas.loadImage("/mainImage/tm-vang.png");
		GameScr.imgHP_tm_xam = GameCanvas.loadImage("/mainImage/tm-xam.png");
		GameScr.imgHP_tm_xanh = GameCanvas.loadImage("/mainImage/tm-xanh.png");
		GameScr.imgNR1 = GameCanvas.loadImage("/mainImage/myTexture2dPea_0.png");
		GameScr.imgNR2 = GameCanvas.loadImage("/mainImage/myTexture2dPea_1.png");
		GameScr.imgNR3 = GameCanvas.loadImage("/mainImage/myTexture2dPea_2.png");
		GameScr.imgNR4 = GameCanvas.loadImage("/mainImage/myTexture2dPea_3.png");
		GameScr.flyTextX = new int[5];
		GameScr.flyTextY = new int[5];
		GameScr.flyTextDx = new int[5];
		GameScr.flyTextDy = new int[5];
		GameScr.flyTextState = new int[5];
		GameScr.flyTextString = new string[5];
		GameScr.flyTextYTo = new int[5];
		GameScr.flyTime = new int[5];
		GameScr.flyTextColor = new int[8];
		for (int i = 0; i < 5; i++)
		{
			GameScr.flyTextState[i] = -1;
		}
		sbyte[] array = Rms.loadRMS("NRdataVersion");
		sbyte[] array2 = Rms.loadRMS("NRmapVersion");
		sbyte[] array3 = Rms.loadRMS("NRskillVersion");
		sbyte[] array4 = Rms.loadRMS("NRitemVersion");
		bool flag = array != null;
		if (flag)
		{
			GameScr.vcData = array[0];
		}
		bool flag2 = array2 != null;
		if (flag2)
		{
			GameScr.vcMap = array2[0];
		}
		bool flag3 = array3 != null;
		if (flag3)
		{
			GameScr.vcSkill = array3[0];
		}
		bool flag4 = array4 != null;
		if (flag4)
		{
			GameScr.vcItem = array4[0];
		}
		GameScr.imgNut = GameCanvas.loadImage("/mainImage/myTexture2dnut.png");
		GameScr.imgNutF = GameCanvas.loadImage("/mainImage/myTexture2dnutF.png");
		MobCapcha.init();
		GameScr.isAnalog = ((Rms.loadRMSInt("analog") == 1) ? 1 : 0);
		GameScr.gamePad = new GamePad();
		GameScr.arrow = GameCanvas.loadImage("/mainImage/myTexture2darrow3.png");
		GameScr.imgTrans = GameCanvas.loadImage("/bg/trans.png");
		GameScr.imgRoomStat = GameCanvas.loadImage("/mainImage/myTexture2dstat.png");
		GameScr.frBarPow0 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor20.png");
		GameScr.frBarPow1 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor21.png");
		GameScr.frBarPow2 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor22.png");
		GameScr.frBarPow20 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor00.png");
		GameScr.frBarPow21 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor01.png");
		GameScr.frBarPow22 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor02.png");
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x00004D5B File Offset: 0x00002F5B
	public void initSelectChar()
	{
		this.readPart();
		SmallImage.init();
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x000538B8 File Offset: 0x00051AB8
	public static void paintOngMauPercent(Image img0, Image img1, Image img2, float x, float y, int size, float pixelPercent, mGraphics g)
	{
		int clipX = g.getClipX();
		int clipY = g.getClipY();
		int clipWidth = g.getClipWidth();
		int clipHeight = g.getClipHeight();
		g.setClip((int)x, (int)y, (int)pixelPercent, 13);
		int num = size / 15 - 2;
		for (int i = 0; i < num; i++)
		{
			g.drawImage(img1, x + (float)((i + 1) * 15), y, 0);
		}
		g.drawImage(img0, x, y, 0);
		g.drawImage(img1, x + (float)size - 30f, y, 0);
		g.drawImage(img2, x + (float)size - 15f, y, 0);
		g.setClip(clipX, clipY, clipWidth, clipHeight);
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00053978 File Offset: 0x00051B78
	public void initTraining()
	{
		bool isCreateChar = CreateCharScr.isCreateChar;
		if (isCreateChar)
		{
			CreateCharScr.isCreateChar = false;
			this.right = null;
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x000539A0 File Offset: 0x00051BA0
	public bool isMapDocNhan()
	{
		return TileMap.mapID >= 53 && TileMap.mapID <= 62;
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x000539D4 File Offset: 0x00051BD4
	public bool isMapFize()
	{
		return TileMap.mapID >= 63;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x000539FC File Offset: 0x00051BFC
	public override void switchToMe()
	{
		GameScr.vChatVip.removeAllElements();
		ServerListScreen.isWait = false;
		bool flag = BackgroudEffect.isHaveRain();
		if (flag)
		{
			SoundMn.gI().rain();
		}
		LoginScr.isContinueToLogin = false;
		global::Char.isLoadingMap = false;
		bool flag2 = !GameScr.isPaintOther;
		if (flag2)
		{
			Service.gI().finishLoadMap();
		}
		bool flag3 = TileMap.isTrainingMap();
		if (flag3)
		{
			this.initTraining();
		}
		GameScr.info1.isUpdate = true;
		GameScr.info2.isUpdate = true;
		this.resetButton();
		GameScr.isLoadAllData = true;
		GameScr.isPaintOther = false;
		base.switchToMe();
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00053A98 File Offset: 0x00051C98
	public static int getMaxExp(int level)
	{
		int num = 0;
		for (int i = 0; i <= level; i++)
		{
			num += (int)GameScr.exps[i];
		}
		return num;
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x00053AD0 File Offset: 0x00051CD0
	public static void resetAllvector()
	{
		GameScr.vCharInMap.removeAllElements();
		Teleport.vTeleport.removeAllElements();
		GameScr.vItemMap.removeAllElements();
		Effect2.vEffect2.removeAllElements();
		Effect2.vAnimateEffect.removeAllElements();
		Effect2.vEffect2Outside.removeAllElements();
		Effect2.vEffectFeet.removeAllElements();
		Effect2.vEffect3.removeAllElements();
		GameScr.vMobAttack.removeAllElements();
		GameScr.vMob.removeAllElements();
		GameScr.vNpc.removeAllElements();
		global::Char.myCharz().vMovePoints.removeAllElements();
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00003E4C File Offset: 0x0000204C
	public void loadSkillShortcut()
	{
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00053B68 File Offset: 0x00051D68
	public void onOSkill(sbyte[] oSkillID)
	{
		Cout.println("GET onScreenSkill!");
		GameScr.onScreenSkill = new Skill[10];
		bool flag = oSkillID == null;
		if (flag)
		{
			this.loadDefaultonScreenSkill();
		}
		else
		{
			for (int i = 0; i < oSkillID.Length; i++)
			{
				for (int j = 0; j < global::Char.myCharz().vSkillFight.size(); j++)
				{
					Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(j);
					bool flag2 = skill.template.id == oSkillID[i];
					if (flag2)
					{
						GameScr.onScreenSkill[i] = skill;
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00053C10 File Offset: 0x00051E10
	public void onKSkill(sbyte[] kSkillID)
	{
		Cout.println("GET KEYSKILL!");
		GameScr.keySkill = new Skill[10];
		bool flag = kSkillID == null;
		if (flag)
		{
			this.loadDefaultKeySkill();
		}
		else
		{
			for (int i = 0; i < kSkillID.Length; i++)
			{
				for (int j = 0; j < global::Char.myCharz().vSkillFight.size(); j++)
				{
					Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(j);
					bool flag2 = skill.template.id == kSkillID[i];
					if (flag2)
					{
						GameScr.keySkill[i] = skill;
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00053CB8 File Offset: 0x00051EB8
	public void onCSkill(sbyte[] cSkillID)
	{
		Cout.println("GET CURRENTSKILL!");
		bool flag = cSkillID == null || cSkillID.Length == 0;
		if (flag)
		{
			bool flag2 = global::Char.myCharz().vSkillFight.size() > 0;
			if (flag2)
			{
				global::Char.myCharz().myskill = (Skill)global::Char.myCharz().vSkillFight.elementAt(0);
			}
		}
		else
		{
			for (int i = 0; i < global::Char.myCharz().vSkillFight.size(); i++)
			{
				Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(i);
				bool flag3 = skill.template.id == cSkillID[0];
				if (flag3)
				{
					global::Char.myCharz().myskill = skill;
					break;
				}
			}
		}
		bool flag4 = global::Char.myCharz().myskill != null;
		if (flag4)
		{
			Service.gI().selectSkill((int)global::Char.myCharz().myskill.template.id);
			this.saveRMSCurrentSkill(global::Char.myCharz().myskill.template.id);
		}
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00053DC8 File Offset: 0x00051FC8
	private void loadDefaultonScreenSkill()
	{
		Cout.println("LOAD DEFAULT ONmScreen SKILL");
		int num = 0;
		while (num < GameScr.onScreenSkill.Length && num < global::Char.myCharz().vSkillFight.size())
		{
			Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(num);
			GameScr.onScreenSkill[num] = skill;
			num++;
		}
		this.saveonScreenSkillToRMS();
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00053E34 File Offset: 0x00052034
	private void loadDefaultKeySkill()
	{
		Cout.println("LOAD DEFAULT KEY SKILL");
		int num = 0;
		while (num < GameScr.keySkill.Length && num < global::Char.myCharz().vSkillFight.size())
		{
			Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(num);
			GameScr.keySkill[num] = skill;
			num++;
		}
		this.saveKeySkillToRMS();
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00053EA0 File Offset: 0x000520A0
	public void doSetOnScreenSkill(SkillTemplate skillTemplate)
	{
		Skill skill = global::Char.myCharz().getSkill(skillTemplate);
		MyVector myVector = new MyVector();
		for (int i = 0; i < 10; i++)
		{
			object obj = new object[]
			{
				skill,
				i.ToString() + string.Empty
			};
			Command command = new Command(mResources.into_place + (i + 1).ToString(), 11120, obj);
			Skill skill2 = GameScr.onScreenSkill[i];
			bool flag = skill2 != null;
			if (flag)
			{
				command.isDisplay = true;
			}
			myVector.addElement(command);
		}
		GameCanvas.menu.startAt(myVector, 0);
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00053F4C File Offset: 0x0005214C
	public void doSetKeySkill(SkillTemplate skillTemplate)
	{
		Cout.println("DO SET KEY SKILL");
		Skill skill = global::Char.myCharz().getSkill(skillTemplate);
		string[] array = ((!TField.isQwerty) ? mResources.key_skill : mResources.key_skill_qwerty);
		MyVector myVector = new MyVector();
		for (int i = 0; i < 10; i++)
		{
			MyVector myVector2 = myVector;
			object obj = new object[]
			{
				skill,
				i.ToString() + string.Empty
			};
			myVector2.addElement(new Command(array[i], 11121, obj));
		}
		GameCanvas.menu.startAt(myVector, 0);
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00053FE4 File Offset: 0x000521E4
	public void saveonScreenSkillToRMS()
	{
		sbyte[] array = new sbyte[GameScr.onScreenSkill.Length];
		for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
		{
			bool flag = GameScr.onScreenSkill[i] == null;
			if (flag)
			{
				array[i] = -1;
			}
			else
			{
				array[i] = GameScr.onScreenSkill[i].template.id;
			}
		}
		Service.gI().changeOnKeyScr(array);
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00054050 File Offset: 0x00052250
	public void saveKeySkillToRMS()
	{
		sbyte[] array = new sbyte[GameScr.keySkill.Length];
		for (int i = 0; i < GameScr.keySkill.Length; i++)
		{
			bool flag = GameScr.keySkill[i] == null;
			if (flag)
			{
				array[i] = -1;
			}
			else
			{
				array[i] = GameScr.keySkill[i].template.id;
			}
		}
		Service.gI().changeOnKeyScr(array);
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00003E4C File Offset: 0x0000204C
	public void saveRMSCurrentSkill(sbyte id)
	{
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x000540BC File Offset: 0x000522BC
	public void addSkillShortcut(Skill skill)
	{
		Cout.println("ADD SKILL SHORTCUT TO SKILL " + skill.template.id.ToString());
		for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
		{
			bool flag = GameScr.onScreenSkill[i] == null;
			if (flag)
			{
				GameScr.onScreenSkill[i] = skill;
				break;
			}
		}
		for (int j = 0; j < GameScr.keySkill.Length; j++)
		{
			bool flag2 = GameScr.keySkill[j] == null;
			if (flag2)
			{
				GameScr.keySkill[j] = skill;
				break;
			}
		}
		bool flag3 = global::Char.myCharz().myskill == null;
		if (flag3)
		{
			global::Char.myCharz().myskill = skill;
		}
		this.saveKeySkillToRMS();
		this.saveonScreenSkillToRMS();
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0000F54C File Offset: 0x0000D74C
	public bool isBagFull()
	{
		for (int i = global::Char.myCharz().arrItemBag.Length - 1; i >= 0; i--)
		{
			bool flag = global::Char.myCharz().arrItemBag[i] == null;
			if (flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x00004D6B File Offset: 0x00002F6B
	public void createConfirm(string[] menu, Npc npc)
	{
		this.resetButton();
		this.isLockKey = true;
		this.left = new Command(menu[0], 130011, npc);
		this.right = new Command(menu[1], 130012, npc);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00054180 File Offset: 0x00052380
	public void createMenu(string[] menu, Npc npc)
	{
		MyVector myVector = new MyVector();
		for (int i = 0; i < menu.Length; i++)
		{
			myVector.addElement(new Command(menu[i], 11057, npc));
		}
		GameCanvas.menu.startAt(myVector, 2);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x000541CC File Offset: 0x000523CC
	public void readPart()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_part"));
			int num = (int)dataInputStream.readShort();
			GameScr.parts = new Part[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)dataInputStream.readByte();
				GameScr.parts[i] = new Part(num2);
				for (int j = 0; j < GameScr.parts[i].pi.Length; j++)
				{
					GameScr.parts[i].pi[j] = new PartImage();
					GameScr.parts[i].pi[j].id = dataInputStream.readShort();
					GameScr.parts[i].pi[j].dx = dataInputStream.readByte();
					GameScr.parts[i].pi[j].dy = dataInputStream.readByte();
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("LOI TAI readPart " + ex.ToString());
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex2)
			{
				Res.outz2("LOI TAI readPart 2" + ex2.StackTrace);
			}
		}
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x0005432C File Offset: 0x0005252C
	public void readEfect()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_effect"));
			int num = (int)dataInputStream.readShort();
			GameScr.efs = new EffectCharPaint[num];
			for (int i = 0; i < num; i++)
			{
				GameScr.efs[i] = new EffectCharPaint();
				GameScr.efs[i].idEf = (int)dataInputStream.readShort();
				GameScr.efs[i].arrEfInfo = new EffectInfoPaint[(int)dataInputStream.readByte()];
				for (int j = 0; j < GameScr.efs[i].arrEfInfo.Length; j++)
				{
					GameScr.efs[i].arrEfInfo[j] = new EffectInfoPaint();
					GameScr.efs[i].arrEfInfo[j].idImg = (int)dataInputStream.readShort();
					GameScr.efs[i].arrEfInfo[j].dx = (int)dataInputStream.readByte();
					GameScr.efs[i].arrEfInfo[j].dy = (int)dataInputStream.readByte();
				}
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham Eff: " + ex.ToString());
			}
		}
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0005448C File Offset: 0x0005268C
	public void readArrow()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_arrow"));
			int num = (int)dataInputStream.readShort();
			GameScr.arrs = new Arrowpaint[num];
			for (int i = 0; i < num; i++)
			{
				GameScr.arrs[i] = new Arrowpaint();
				GameScr.arrs[i].id = (int)dataInputStream.readShort();
				GameScr.arrs[i].imgId[0] = (int)dataInputStream.readShort();
				GameScr.arrs[i].imgId[1] = (int)dataInputStream.readShort();
				GameScr.arrs[i].imgId[2] = (int)dataInputStream.readShort();
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham readArrow: " + ex.ToString());
			}
		}
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x00054588 File Offset: 0x00052788
	public void readDart()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_dart"));
			int num = (int)dataInputStream.readShort();
			GameScr.darts = new DartInfo[num];
			for (int i = 0; i < num; i++)
			{
				GameScr.darts[i] = new DartInfo();
				GameScr.darts[i].id = dataInputStream.readShort();
				GameScr.darts[i].nUpdate = dataInputStream.readShort();
				GameScr.darts[i].va = (int)(dataInputStream.readShort() * 256);
				GameScr.darts[i].xdPercent = dataInputStream.readShort();
				int num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].tail = new short[num2];
				for (int j = 0; j < num2; j++)
				{
					GameScr.darts[i].tail[j] = dataInputStream.readShort();
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].tailBorder = new short[num2];
				for (int k = 0; k < num2; k++)
				{
					GameScr.darts[i].tailBorder[k] = dataInputStream.readShort();
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].xd1 = new short[num2];
				for (int l = 0; l < num2; l++)
				{
					GameScr.darts[i].xd1[l] = dataInputStream.readShort();
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].xd2 = new short[num2];
				for (int m = 0; m < num2; m++)
				{
					GameScr.darts[i].xd2[m] = dataInputStream.readShort();
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].head = new short[num2][];
				for (int n = 0; n < num2; n++)
				{
					short num3 = dataInputStream.readShort();
					GameScr.darts[i].head[n] = new short[(int)num3];
					for (int num4 = 0; num4 < (int)num3; num4++)
					{
						GameScr.darts[i].head[n][num4] = dataInputStream.readShort();
					}
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].headBorder = new short[num2][];
				for (int num5 = 0; num5 < num2; num5++)
				{
					short num6 = dataInputStream.readShort();
					GameScr.darts[i].headBorder[num5] = new short[(int)num6];
					for (int num7 = 0; num7 < (int)num6; num7++)
					{
						GameScr.darts[i].headBorder[num5][num7] = dataInputStream.readShort();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham ReadDart: " + ex.ToString());
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex2)
			{
				Cout.LogError("Loi ham reaaDart: " + ex2.ToString());
			}
		}
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x000548E0 File Offset: 0x00052AE0
	public void readSkill()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_skill"));
			int num = (int)dataInputStream.readShort();
			int num2 = Skills.skills.size();
			GameScr.sks = new SkillPaint[num2];
			for (int i = 0; i < num; i++)
			{
				short num3 = dataInputStream.readShort();
				bool flag = num3 == 1111;
				if (flag)
				{
					num3 = (short)(num - 1);
				}
				GameScr.sks[(int)num3] = new SkillPaint();
				GameScr.sks[(int)num3].id = (int)num3;
				GameScr.sks[(int)num3].effectHappenOnMob = (int)dataInputStream.readShort();
				bool flag2 = GameScr.sks[(int)num3].effectHappenOnMob <= 0;
				if (flag2)
				{
					GameScr.sks[(int)num3].effectHappenOnMob = 80;
				}
				GameScr.sks[(int)num3].numEff = (int)dataInputStream.readByte();
				GameScr.sks[(int)num3].skillStand = new SkillInfoPaint[(int)dataInputStream.readByte()];
				for (int j = 0; j < GameScr.sks[(int)num3].skillStand.Length; j++)
				{
					GameScr.sks[(int)num3].skillStand[j] = new SkillInfoPaint();
					GameScr.sks[(int)num3].skillStand[j].status = (int)dataInputStream.readByte();
					GameScr.sks[(int)num3].skillStand[j].effS0Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e0dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e0dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].effS1Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e1dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e1dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].effS2Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e2dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e2dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].arrowId = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].adx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].ady = (int)dataInputStream.readShort();
				}
				GameScr.sks[(int)num3].skillfly = new SkillInfoPaint[(int)dataInputStream.readByte()];
				for (int k = 0; k < GameScr.sks[(int)num3].skillfly.Length; k++)
				{
					GameScr.sks[(int)num3].skillfly[k] = new SkillInfoPaint();
					GameScr.sks[(int)num3].skillfly[k].status = (int)dataInputStream.readByte();
					GameScr.sks[(int)num3].skillfly[k].effS0Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e0dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e0dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].effS1Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e1dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e1dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].effS2Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e2dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e2dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].arrowId = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].adx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].ady = (int)dataInputStream.readShort();
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham readSkill: " + ex.ToString());
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex2)
			{
				Cout.LogError("Loi ham readskill: " + ex2.ToString());
			}
		}
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x00054DCC File Offset: 0x00052FCC
	public void readOk()
	{
		try
		{
			Res.outz("<readOk><vsData<" + GameScr.vsData.ToString() + "==" + GameScr.vcData.ToString());
			Res.outz("<readOk><vsMap<" + GameScr.vsMap.ToString() + "==" + GameScr.vcMap.ToString());
			Res.outz("<readOk><vsSkill<" + GameScr.vsSkill.ToString() + "==" + GameScr.vcSkill.ToString());
			Res.outz("<readOk><vsItem<" + GameScr.vsItem.ToString() + "==" + GameScr.vcItem.ToString());
			bool flag = GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem;
			if (flag)
			{
				Res.outz(string.Concat(new string[]
				{
					GameScr.vsData.ToString(),
					",",
					GameScr.vsMap.ToString(),
					",",
					GameScr.vsSkill.ToString(),
					",",
					GameScr.vsItem.ToString()
				}));
				GameScr.gI().readDart();
				GameScr.gI().readEfect();
				GameScr.gI().readArrow();
				GameScr.gI().readSkill();
				Service.gI().clientOk();
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham readOk: " + ex.ToString());
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x00054F8C File Offset: 0x0005318C
	public static GameScr gI()
	{
		bool flag = GameScr.instance == null;
		if (flag)
		{
			GameScr.instance = new GameScr();
		}
		return GameScr.instance;
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x00004DA4 File Offset: 0x00002FA4
	public static void clearGameScr()
	{
		GameScr.instance = null;
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x00004DAD File Offset: 0x00002FAD
	public void loadGameScr()
	{
		GameScr.loadSplash();
		Res.init();
		this.loadInforBar();
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00054FBC File Offset: 0x000531BC
	public void doMenuInforMe()
	{
		GameScr.scrMain.clear();
		GameScr.scrInfo.clear();
		GameScr.isViewNext = false;
		this.cmdBag = new Command(mResources.MENUME[0], 1100011);
		this.cmdSkill = new Command(mResources.MENUME[1], 1100012);
		this.cmdTiemnang = new Command(mResources.MENUME[2], 1100013);
		this.cmdInfo = new Command(mResources.MENUME[3], 1100014);
		this.cmdtrangbi = new Command(mResources.MENUME[4], 1100015);
		MyVector myVector = new MyVector();
		myVector.addElement(this.cmdBag);
		myVector.addElement(this.cmdSkill);
		myVector.addElement(this.cmdTiemnang);
		myVector.addElement(this.cmdInfo);
		myVector.addElement(this.cmdtrangbi);
		GameCanvas.menu.startAt(myVector, 3);
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x000550B0 File Offset: 0x000532B0
	public void doMenusynthesis()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command(mResources.SYNTHESIS[0], 110002));
		myVector.addElement(new Command(mResources.SYNTHESIS[1], 1100032));
		myVector.addElement(new Command(mResources.SYNTHESIS[2], 1100033));
		GameCanvas.menu.startAt(myVector, 3);
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x0005511C File Offset: 0x0005331C
	public static void loadCamera(bool fullmScreen, int cx, int cy)
	{
		GameScr.gW = GameCanvas.w;
		GameScr.cmdBarH = 39;
		GameScr.gH = GameCanvas.h;
		GameScr.cmdBarW = GameScr.gW;
		GameScr.cmdBarX = 0;
		GameScr.cmdBarY = GameCanvas.h - Paint.hTab - GameScr.cmdBarH;
		GameScr.girlHPBarY = 0;
		GameScr.csPadMaxH = GameCanvas.h / 6;
		bool flag = GameScr.csPadMaxH < 48;
		if (flag)
		{
			GameScr.csPadMaxH = 48;
		}
		GameScr.gW2 = GameScr.gW >> 1;
		GameScr.gH2 = GameScr.gH >> 1;
		GameScr.gW3 = GameScr.gW / 3;
		GameScr.gH3 = GameScr.gH / 3;
		GameScr.gW23 = GameScr.gH - 120;
		GameScr.gH23 = GameScr.gH * 2 / 3;
		GameScr.gW34 = 3 * GameScr.gW / 4;
		GameScr.gH34 = 3 * GameScr.gH / 4;
		GameScr.gW6 = GameScr.gW / 6;
		GameScr.gH6 = GameScr.gH / 6;
		GameScr.gssw = GameScr.gW / (int)TileMap.size + 2;
		GameScr.gssh = GameScr.gH / (int)TileMap.size + 2;
		bool flag2 = GameScr.gW % 24 != 0;
		if (flag2)
		{
			GameScr.gssw++;
		}
		GameScr.cmxLim = (TileMap.tmw - 1) * (int)TileMap.size - GameScr.gW;
		GameScr.cmyLim = (TileMap.tmh - 1) * (int)TileMap.size - GameScr.gH;
		bool flag3 = cx == -1 && cy == -1;
		if (flag3)
		{
			GameScr.cmx = (GameScr.cmtoX = global::Char.myCharz().cx - GameScr.gW2 + GameScr.gW6 * global::Char.myCharz().cdir);
			GameScr.cmy = (GameScr.cmtoY = global::Char.myCharz().cy - GameScr.gH23);
		}
		else
		{
			GameScr.cmx = (GameScr.cmtoX = cx - GameScr.gW23 + GameScr.gW6 * global::Char.myCharz().cdir);
			GameScr.cmy = (GameScr.cmtoY = cy - GameScr.gH23);
		}
		GameScr.firstY = GameScr.cmy;
		bool flag4 = GameScr.cmx < 24;
		if (flag4)
		{
			GameScr.cmx = (GameScr.cmtoX = 24);
		}
		bool flag5 = GameScr.cmx > GameScr.cmxLim;
		if (flag5)
		{
			GameScr.cmx = (GameScr.cmtoX = GameScr.cmxLim);
		}
		bool flag6 = GameScr.cmy < 0;
		if (flag6)
		{
			GameScr.cmy = (GameScr.cmtoY = 0);
		}
		bool flag7 = GameScr.cmy > GameScr.cmyLim;
		if (flag7)
		{
			GameScr.cmy = (GameScr.cmtoY = GameScr.cmyLim);
		}
		GameScr.gssx = GameScr.cmx / (int)TileMap.size - 1;
		bool flag8 = GameScr.gssx < 0;
		if (flag8)
		{
			GameScr.gssx = 0;
		}
		GameScr.gssy = GameScr.cmy / (int)TileMap.size;
		GameScr.gssxe = GameScr.gssx + GameScr.gssw;
		GameScr.gssye = GameScr.gssy + GameScr.gssh;
		bool flag9 = GameScr.gssy < 0;
		if (flag9)
		{
			GameScr.gssy = 0;
		}
		bool flag10 = GameScr.gssye > TileMap.tmh - 1;
		if (flag10)
		{
			GameScr.gssye = TileMap.tmh - 1;
		}
		TileMap.countx = (GameScr.gssxe - GameScr.gssx) * 4;
		bool flag11 = TileMap.countx > TileMap.tmw;
		if (flag11)
		{
			TileMap.countx = TileMap.tmw;
		}
		TileMap.county = (GameScr.gssye - GameScr.gssy) * 4;
		bool flag12 = TileMap.county > TileMap.tmh;
		if (flag12)
		{
			TileMap.county = TileMap.tmh;
		}
		TileMap.gssx = (global::Char.myCharz().cx - 2 * GameScr.gW) / (int)TileMap.size;
		bool flag13 = TileMap.gssx < 0;
		if (flag13)
		{
			TileMap.gssx = 0;
		}
		TileMap.gssxe = TileMap.gssx + TileMap.countx;
		bool flag14 = TileMap.gssxe > TileMap.tmw;
		if (flag14)
		{
			TileMap.gssxe = TileMap.tmw;
		}
		TileMap.gssy = (global::Char.myCharz().cy - 2 * GameScr.gH) / (int)TileMap.size;
		bool flag15 = TileMap.gssy < 0;
		if (flag15)
		{
			TileMap.gssy = 0;
		}
		TileMap.gssye = TileMap.gssy + TileMap.county;
		bool flag16 = TileMap.gssye > TileMap.tmh;
		if (flag16)
		{
			TileMap.gssye = TileMap.tmh;
		}
		ChatTextField.gI().parentScreen = GameScr.instance;
		ChatTextField.gI().tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
		ChatTextField.gI().initChatTextField();
		bool isTouch = GameCanvas.isTouch;
		if (isTouch)
		{
			GameScr.yTouchBar = GameScr.gH - 88;
			GameScr.xC = GameScr.gW - 40;
			GameScr.yC = 2;
			bool flag17 = GameCanvas.w <= 240;
			if (flag17)
			{
				GameScr.xC = GameScr.gW - 35;
				GameScr.yC = 5;
			}
			GameScr.xF = GameScr.gW - 55;
			GameScr.yF = GameScr.yTouchBar + 35;
			GameScr.xTG = GameScr.gW - 37;
			GameScr.yTG = GameScr.yTouchBar - 1;
			bool flag18 = GameCanvas.w >= 450;
			if (flag18)
			{
				GameScr.yTG -= 12;
				GameScr.yHP -= 7;
				GameScr.xF -= 10;
				GameScr.yF -= 5;
				GameScr.xTG -= 10;
			}
		}
		GameScr.setSkillBarPosition();
		GameScr.disXC = ((GameCanvas.w <= 200) ? 30 : 40);
		bool flag19 = Rms.loadRMSInt("viewchat") == -1;
		if (flag19)
		{
			GameCanvas.panel.isViewChatServer = true;
		}
		else
		{
			GameCanvas.panel.isViewChatServer = Rms.loadRMSInt("viewchat") == 1;
		}
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x000556CC File Offset: 0x000538CC
	public static void setSkillBarPosition()
	{
		Skill[] array = ((!GameCanvas.isTouch) ? GameScr.keySkill : GameScr.onScreenSkill);
		GameScr.xS = new int[array.Length];
		GameScr.yS = new int[array.Length];
		bool flag = GameCanvas.isTouchControlSmallScreen && GameScr.isUseTouch;
		if (flag)
		{
			GameScr.xSkill = 23;
			GameScr.ySkill = 52;
			GameScr.padSkill = 5;
			for (int i = 0; i < GameScr.xS.Length; i++)
			{
				GameScr.xS[i] = i * (25 + GameScr.padSkill);
				GameScr.yS[i] = GameScr.ySkill;
				bool flag2 = GameScr.xS.Length > 5 && i >= GameScr.xS.Length / 2;
				if (flag2)
				{
					GameScr.xS[i] = (i - GameScr.xS.Length / 2) * (25 + GameScr.padSkill);
					GameScr.yS[i] = GameScr.ySkill - 32;
				}
			}
			GameScr.xHP = array.Length * (25 + GameScr.padSkill);
			GameScr.yHP = GameScr.ySkill;
		}
		else
		{
			GameScr.wSkill = 30;
			bool flag3 = GameCanvas.w <= 320;
			if (flag3)
			{
				GameScr.ySkill = GameScr.gH - GameScr.wSkill - 6;
				GameScr.xSkill = GameScr.gW2 - array.Length * GameScr.wSkill / 2 - 25;
			}
			else
			{
				GameScr.wSkill = 40;
				GameScr.xSkill = 10;
				GameScr.ySkill = GameCanvas.h - GameScr.wSkill + 7;
			}
			for (int j = 0; j < GameScr.xS.Length; j++)
			{
				GameScr.xS[j] = j * GameScr.wSkill;
				GameScr.yS[j] = GameScr.ySkill;
				bool flag4 = GameScr.xS.Length > 5 && j >= GameScr.xS.Length / 2;
				if (flag4)
				{
					GameScr.xS[j] = (j - GameScr.xS.Length / 2) * GameScr.wSkill;
					GameScr.yS[j] = GameScr.ySkill - 32;
				}
			}
			GameScr.xHP = array.Length * GameScr.wSkill;
			GameScr.yHP = GameScr.ySkill;
		}
		bool flag5 = !GameCanvas.isTouch;
		if (!flag5)
		{
			GameScr.xSkill = 17;
			GameScr.ySkill = GameCanvas.h - 40;
			bool flag6 = GameScr.gamePad.isSmallGamePad && GameScr.isAnalog == 1;
			if (flag6)
			{
				GameScr.xHP = array.Length * GameScr.wSkill;
				GameScr.yHP = GameScr.ySkill;
			}
			else
			{
				GameScr.xHP = GameCanvas.w - 45;
				GameScr.yHP = GameCanvas.h - 45;
			}
			GameScr.setTouchBtn();
			for (int k = 0; k < GameScr.xS.Length; k++)
			{
				GameScr.xS[k] = k * GameScr.wSkill;
				GameScr.yS[k] = GameScr.ySkill;
				bool flag7 = GameScr.xS.Length > 5 && k >= GameScr.xS.Length / 2;
				if (flag7)
				{
					GameScr.xS[k] = (k - GameScr.xS.Length / 2) * GameScr.wSkill;
					GameScr.yS[k] = GameScr.ySkill - 32;
				}
			}
		}
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x000559FC File Offset: 0x00053BFC
	private static void updateCamera()
	{
		bool flag = GameScr.isPaintOther;
		if (!flag)
		{
			bool flag2 = GameScr.cmx != GameScr.cmtoX || GameScr.cmy != GameScr.cmtoY;
			if (flag2)
			{
				GameScr.cmvx = GameScr.cmtoX - GameScr.cmx << 2;
				GameScr.cmvy = GameScr.cmtoY - GameScr.cmy << 2;
				GameScr.cmdx += GameScr.cmvx;
				GameScr.cmx += GameScr.cmdx >> 4;
				GameScr.cmdx &= 15;
				GameScr.cmdy += GameScr.cmvy;
				GameScr.cmy += GameScr.cmdy >> 4;
				GameScr.cmdy &= 15;
				bool flag3 = GameScr.cmx < 24;
				if (flag3)
				{
					GameScr.cmx = 24;
				}
				bool flag4 = GameScr.cmx > GameScr.cmxLim;
				if (flag4)
				{
					GameScr.cmx = GameScr.cmxLim;
				}
				bool flag5 = GameScr.cmy < 0;
				if (flag5)
				{
					GameScr.cmy = 0;
				}
				bool flag6 = GameScr.cmy > GameScr.cmyLim;
				if (flag6)
				{
					GameScr.cmy = GameScr.cmyLim;
				}
			}
			GameScr.gssx = GameScr.cmx / (int)TileMap.size - 1;
			bool flag7 = GameScr.gssx < 0;
			if (flag7)
			{
				GameScr.gssx = 0;
			}
			GameScr.gssy = GameScr.cmy / (int)TileMap.size;
			GameScr.gssxe = GameScr.gssx + GameScr.gssw;
			GameScr.gssye = GameScr.gssy + GameScr.gssh;
			bool flag8 = GameScr.gssy < 0;
			if (flag8)
			{
				GameScr.gssy = 0;
			}
			bool flag9 = GameScr.gssye > TileMap.tmh - 1;
			if (flag9)
			{
				GameScr.gssye = TileMap.tmh - 1;
			}
			TileMap.gssx = (global::Char.myCharz().cx - 2 * GameScr.gW) / (int)TileMap.size;
			bool flag10 = TileMap.gssx < 0;
			if (flag10)
			{
				TileMap.gssx = 0;
			}
			TileMap.gssxe = TileMap.gssx + TileMap.countx;
			bool flag11 = TileMap.gssxe > TileMap.tmw;
			if (flag11)
			{
				TileMap.gssxe = TileMap.tmw;
				TileMap.gssx = TileMap.gssxe - TileMap.countx;
			}
			TileMap.gssy = (global::Char.myCharz().cy - 2 * GameScr.gH) / (int)TileMap.size;
			bool flag12 = TileMap.gssy < 0;
			if (flag12)
			{
				TileMap.gssy = 0;
			}
			TileMap.gssye = TileMap.gssy + TileMap.county;
			bool flag13 = TileMap.gssye > TileMap.tmh;
			if (flag13)
			{
				TileMap.gssye = TileMap.tmh;
				TileMap.gssy = TileMap.gssye - TileMap.county;
			}
			GameScr.scrMain.updatecm();
			GameScr.scrInfo.updatecm();
		}
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00055CB0 File Offset: 0x00053EB0
	public bool testAct()
	{
		for (sbyte b = 2; b < 9; b += 2)
		{
			bool flag = GameCanvas.keyHold[(int)b];
			if (flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00055CE8 File Offset: 0x00053EE8
	public void clanInvite(string strInvite, int clanID, int code)
	{
		ClanObject clanObject = new ClanObject();
		clanObject.code = code;
		clanObject.clanID = clanID;
		this.startYesNoPopUp(strInvite, new Command(mResources.YES, 12002, clanObject), new Command(mResources.NO, 12003, clanObject));
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x00055D34 File Offset: 0x00053F34
	public void playerMenu(global::Char c)
	{
		this.auto = 0;
		GameCanvas.clearKeyHold();
		bool flag = global::Char.myCharz().charFocus.charID < 0 || global::Char.myCharz().charID < 0;
		if (!flag)
		{
			MyVector vPlayerMenu = GameCanvas.panel.vPlayerMenu;
			bool flag2 = vPlayerMenu.size() > 0;
			if (!flag2)
			{
				bool flag3 = global::Char.myCharz().taskMaint != null && global::Char.myCharz().taskMaint.taskId > 1;
				if (flag3)
				{
					vPlayerMenu.addElement(new Command(mResources.make_friend, 11112, global::Char.myCharz().charFocus));
					vPlayerMenu.addElement(new Command(mResources.trade, 11113, global::Char.myCharz().charFocus));
				}
				bool flag4 = global::Char.myCharz().clan != null && global::Char.myCharz().role < 2 && global::Char.myCharz().charFocus.clanID == -1;
				if (flag4)
				{
					vPlayerMenu.addElement(new Command(mResources.CHAR_ORDER[4], 110391));
				}
				bool flag5 = global::Char.myCharz().charFocus.statusMe != 14 && global::Char.myCharz().charFocus.statusMe != 5;
				if (flag5)
				{
					bool flag6 = global::Char.myCharz().taskMaint != null && global::Char.myCharz().taskMaint.taskId >= 14;
					if (flag6)
					{
						vPlayerMenu.addElement(new Command(mResources.CHAR_ORDER[0], 2003));
					}
				}
				else
				{
					bool flag7 = global::Char.myCharz().myskill.template.type != 4;
					if (flag7)
					{
					}
				}
				bool flag8 = global::Char.myCharz().clan != null && global::Char.myCharz().clan.ID == global::Char.myCharz().charFocus.clanID && global::Char.myCharz().charFocus.statusMe != 14 && global::Char.myCharz().taskMaint != null && global::Char.myCharz().taskMaint.taskId >= 14;
				if (flag8)
				{
					vPlayerMenu.addElement(new Command(mResources.CHAR_ORDER[1], 2004));
				}
				int num = global::Char.myCharz().nClass.skillTemplates.Length;
				for (int i = 0; i < num; i++)
				{
					SkillTemplate skillTemplate = global::Char.myCharz().nClass.skillTemplates[i];
					Skill skill = global::Char.myCharz().getSkill(skillTemplate);
					bool flag9 = skill != null && skillTemplate.isBuffToPlayer() && skill.point >= 1;
					if (flag9)
					{
						vPlayerMenu.addElement(new Command(skillTemplate.name, 12004, skill));
					}
				}
			}
		}
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x00055FFC File Offset: 0x000541FC
	public bool isAttack()
	{
		bool flag = this.checkClickToBotton(global::Char.myCharz().charFocus);
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = this.checkClickToBotton(global::Char.myCharz().mobFocus);
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				bool flag4 = this.checkClickToBotton(global::Char.myCharz().npcFocus);
				if (flag4)
				{
					flag2 = false;
				}
				else
				{
					bool isShow = ChatTextField.gI().isShow;
					if (isShow)
					{
						flag2 = false;
					}
					else
					{
						bool flag5 = InfoDlg.isLock || global::Char.myCharz().isLockAttack || global::Char.isLockKey;
						if (flag5)
						{
							flag2 = false;
						}
						else
						{
							bool flag6 = global::Char.myCharz().myskill != null && global::Char.myCharz().myskill.template.id == 6 && global::Char.myCharz().itemFocus != null;
							if (flag6)
							{
								this.pickItem();
								flag2 = false;
							}
							else
							{
								bool flag7 = global::Char.myCharz().myskill != null && global::Char.myCharz().myskill.template.type == 2 && global::Char.myCharz().npcFocus == null && global::Char.myCharz().myskill.template.id != 6;
								if (flag7)
								{
									bool flag8 = !this.checkSkillValid();
									flag2 = !flag8;
								}
								else
								{
									bool flag9 = global::Char.myCharz().skillPaint != null || (global::Char.myCharz().mobFocus == null && global::Char.myCharz().npcFocus == null && global::Char.myCharz().charFocus == null && global::Char.myCharz().itemFocus == null);
									if (flag9)
									{
										flag2 = false;
									}
									else
									{
										bool flag10 = global::Char.myCharz().mobFocus != null;
										if (flag10)
										{
											bool flag11 = global::Char.myCharz().mobFocus.isBigBoss() && global::Char.myCharz().mobFocus.status == 4;
											if (flag11)
											{
												global::Char.myCharz().mobFocus = null;
												global::Char.myCharz().currentMovePoint = null;
											}
											GameScr.isAutoPlay = true;
											bool flag12 = !this.isMeCanAttackMob(global::Char.myCharz().mobFocus);
											if (flag12)
											{
												Res.outz("can not attack");
												flag2 = false;
											}
											else
											{
												bool flag13 = this.mobCapcha != null;
												if (flag13)
												{
													flag2 = false;
												}
												else
												{
													bool flag14 = global::Char.myCharz().myskill == null;
													if (flag14)
													{
														flag2 = false;
													}
													else
													{
														bool flag15 = global::Char.myCharz().isSelectingSkillUseAlone();
														if (flag15)
														{
															flag2 = false;
														}
														else
														{
															int num = -1;
															int num2 = Res.abs(global::Char.myCharz().cx - GameScr.cmx) * mGraphics.zoomLevel;
															bool flag16 = global::Char.myCharz().charFocus != null;
															if (flag16)
															{
																num = Res.abs(global::Char.myCharz().cx - global::Char.myCharz().charFocus.cx) * mGraphics.zoomLevel;
															}
															else
															{
																bool flag17 = global::Char.myCharz().mobFocus != null;
																if (flag17)
																{
																	num = Res.abs(global::Char.myCharz().cx - global::Char.myCharz().mobFocus.x) * mGraphics.zoomLevel;
																}
															}
															bool flag18 = (global::Char.myCharz().mobFocus.status == 1 || global::Char.myCharz().mobFocus.status == 0 || global::Char.myCharz().myskill.template.type == 4 || num == -1 || num > num2) && global::Char.myCharz().myskill.template.type == 4;
															if (flag18)
															{
																bool flag19 = global::Char.myCharz().mobFocus.x < global::Char.myCharz().cx;
																if (flag19)
																{
																	global::Char.myCharz().cdir = -1;
																}
																else
																{
																	global::Char.myCharz().cdir = 1;
																}
																this.doSelectSkill(global::Char.myCharz().myskill, true);
															}
															bool flag20 = !this.checkSkillValid();
															if (flag20)
															{
																flag2 = false;
															}
															else
															{
																bool flag21 = global::Char.myCharz().cx < global::Char.myCharz().mobFocus.getX();
																if (flag21)
																{
																	global::Char.myCharz().cdir = 1;
																}
																else
																{
																	global::Char.myCharz().cdir = -1;
																}
																int num3 = global::Math.abs(global::Char.myCharz().cx - global::Char.myCharz().mobFocus.getX());
																int num4 = global::Math.abs(global::Char.myCharz().cy - global::Char.myCharz().mobFocus.getY());
																global::Char.myCharz().cvx = 0;
																bool flag22 = num3 <= global::Char.myCharz().myskill.dx && num4 <= global::Char.myCharz().myskill.dy;
																if (flag22)
																{
																	bool flag23 = global::Char.myCharz().myskill.template.id == 20;
																	if (flag23)
																	{
																		flag2 = true;
																	}
																	else
																	{
																		bool flag24 = num4 > num3 && Res.abs(global::Char.myCharz().cy - global::Char.myCharz().mobFocus.getY()) > 30 && global::Char.myCharz().mobFocus.getTemplate().type == 4;
																		if (flag24)
																		{
																			global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().cx + global::Char.myCharz().cdir, global::Char.myCharz().mobFocus.getY());
																			global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
																			GameCanvas.clearKeyHold();
																			GameCanvas.clearKeyPressed();
																			flag2 = false;
																		}
																		else
																		{
																			int num5 = 20;
																			bool flag25 = false;
																			bool flag26 = global::Char.myCharz().mobFocus is BigBoss || global::Char.myCharz().mobFocus is BigBoss2;
																			if (flag26)
																			{
																				flag25 = true;
																			}
																			bool flag27 = global::Char.myCharz().myskill.dx > 100;
																			if (flag27)
																			{
																				num5 = 60;
																				bool flag28 = num3 < 20;
																				if (flag28)
																				{
																					global::Char.myCharz().createShadow(global::Char.myCharz().cx, global::Char.myCharz().cy, 10);
																				}
																			}
																			bool flag29 = false;
																			bool flag30 = (TileMap.tileTypeAtPixel(global::Char.myCharz().cx, global::Char.myCharz().cy + 3) & 2) == 2;
																			if (flag30)
																			{
																				int num6 = ((global::Char.myCharz().cx > global::Char.myCharz().mobFocus.getX()) ? 1 : (-1));
																				bool flag31 = (TileMap.tileTypeAtPixel(global::Char.myCharz().mobFocus.getX() + num5 * num6, global::Char.myCharz().cy + 3) & 2) != 2;
																				if (flag31)
																				{
																					flag29 = true;
																				}
																			}
																			bool flag32 = num3 <= num5 && !flag29;
																			if (flag32)
																			{
																				bool flag33 = global::Char.myCharz().cx > global::Char.myCharz().mobFocus.getX();
																				if (flag33)
																				{
																					int num7 = global::Char.myCharz().mobFocus.getX() + num5 + (flag25 ? 30 : 0);
																					int i = global::Char.myCharz().mobFocus.getX();
																					bool flag34 = false;
																					while (i < num7)
																					{
																						bool flag35 = TileMap.tileTypeAtPixel(i, global::Char.myCharz().cy + 3) == 8 || TileMap.tileTypeAtPixel(i, global::Char.myCharz().cy + 3) == 4;
																						if (flag35)
																						{
																							flag34 = true;
																							break;
																						}
																						i += 24;
																					}
																					bool flag36 = flag34;
																					if (flag36)
																					{
																						global::Char.myCharz().cx = i - 24;
																					}
																					else
																					{
																						global::Char.myCharz().cx = num7;
																					}
																					global::Char.myCharz().cdir = -1;
																				}
																				else
																				{
																					int num8 = global::Char.myCharz().mobFocus.getX() - num5 - (flag25 ? 30 : 0);
																					int j = global::Char.myCharz().mobFocus.getX();
																					bool flag37 = false;
																					while (j > num8)
																					{
																						bool flag38 = TileMap.tileTypeAtPixel(j, global::Char.myCharz().cy + 3) == 8 || TileMap.tileTypeAtPixel(j, global::Char.myCharz().cy + 3) == 4;
																						if (flag38)
																						{
																							flag37 = true;
																							break;
																						}
																						j -= 24;
																					}
																					bool flag39 = flag37;
																					if (flag39)
																					{
																						global::Char.myCharz().cx = j + 24;
																					}
																					else
																					{
																						global::Char.myCharz().cx = num8;
																					}
																					global::Char.myCharz().cdir = 1;
																				}
																				Service.gI().charMove();
																			}
																			GameCanvas.clearKeyHold();
																			GameCanvas.clearKeyPressed();
																			flag2 = true;
																		}
																	}
																}
																else
																{
																	bool flag40 = false;
																	bool flag41 = global::Char.myCharz().mobFocus is BigBoss || global::Char.myCharz().mobFocus is BigBoss2;
																	if (flag41)
																	{
																		flag40 = true;
																	}
																	int num9 = (global::Char.myCharz().myskill.dx - ((!flag40) ? 20 : 50)) * ((global::Char.myCharz().cx > global::Char.myCharz().mobFocus.getX()) ? 1 : (-1));
																	bool flag42 = num3 <= global::Char.myCharz().myskill.dx;
																	if (flag42)
																	{
																		num9 = 0;
																	}
																	global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().mobFocus.getX() + num9, global::Char.myCharz().mobFocus.getY());
																	global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
																	GameCanvas.clearKeyHold();
																	GameCanvas.clearKeyPressed();
																	flag2 = false;
																}
															}
														}
													}
												}
											}
										}
										else
										{
											bool flag43 = global::Char.myCharz().npcFocus != null;
											if (flag43)
											{
												bool isHide = global::Char.myCharz().npcFocus.isHide;
												if (isHide)
												{
													flag2 = false;
												}
												else
												{
													bool flag44 = global::Char.myCharz().cx < global::Char.myCharz().npcFocus.cx;
													if (flag44)
													{
														global::Char.myCharz().cdir = 1;
													}
													else
													{
														global::Char.myCharz().cdir = -1;
													}
													bool flag45 = global::Char.myCharz().cx < global::Char.myCharz().npcFocus.cx;
													if (flag45)
													{
														global::Char.myCharz().npcFocus.cdir = -1;
													}
													else
													{
														global::Char.myCharz().npcFocus.cdir = 1;
													}
													int num10 = global::Math.abs(global::Char.myCharz().cx - global::Char.myCharz().npcFocus.cx);
													int num11 = global::Math.abs(global::Char.myCharz().cy - global::Char.myCharz().npcFocus.cy);
													bool flag46 = num11 > 40;
													if (flag46)
													{
														global::Char.myCharz().cy = global::Char.myCharz().npcFocus.cy - 40;
													}
													bool flag47 = num10 < 60;
													if (flag47)
													{
														GameCanvas.clearKeyHold();
														GameCanvas.clearKeyPressed();
														bool flag48 = this.tMenuDelay == 0;
														if (flag48)
														{
															bool flag49 = global::Char.myCharz().taskMaint != null && global::Char.myCharz().taskMaint.taskId == 0;
															if (flag49)
															{
																bool flag50 = global::Char.myCharz().taskMaint.index < 4 && global::Char.myCharz().npcFocus.template.npcTemplateId == 4;
																if (flag50)
																{
																	return false;
																}
																bool flag51 = global::Char.myCharz().taskMaint.index < 3 && global::Char.myCharz().npcFocus.template.npcTemplateId == 3;
																if (flag51)
																{
																	return false;
																}
															}
															this.tMenuDelay = 50;
															InfoDlg.showWait();
															Service.gI().charMove();
															Service.gI().openMenu(global::Char.myCharz().npcFocus.template.npcTemplateId);
														}
													}
													else
													{
														int num12 = (20 + Res.r.nextInt(20)) * ((global::Char.myCharz().cx > global::Char.myCharz().npcFocus.cx) ? 1 : (-1));
														global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().npcFocus.cx + num12, global::Char.myCharz().cy);
														global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
														GameCanvas.clearKeyHold();
														GameCanvas.clearKeyPressed();
													}
													flag2 = false;
												}
											}
											else
											{
												bool flag52 = global::Char.myCharz().charFocus != null;
												if (flag52)
												{
													bool flag53 = this.mobCapcha != null;
													if (flag53)
													{
														flag2 = false;
													}
													else
													{
														bool flag54 = global::Char.myCharz().cx < global::Char.myCharz().charFocus.cx;
														if (flag54)
														{
															global::Char.myCharz().cdir = 1;
														}
														else
														{
															global::Char.myCharz().cdir = -1;
														}
														int num13 = global::Math.abs(global::Char.myCharz().cx - global::Char.myCharz().charFocus.cx);
														int num14 = global::Math.abs(global::Char.myCharz().cy - global::Char.myCharz().charFocus.cy);
														bool flag55 = global::Char.myCharz().isMeCanAttackOtherPlayer(global::Char.myCharz().charFocus) || global::Char.myCharz().isSelectingSkillBuffToPlayer();
														if (flag55)
														{
															bool flag56 = global::Char.myCharz().myskill == null;
															if (flag56)
															{
																flag2 = false;
															}
															else
															{
																bool flag57 = !this.checkSkillValid();
																if (flag57)
																{
																	flag2 = false;
																}
																else
																{
																	bool flag58 = global::Char.myCharz().cx < global::Char.myCharz().charFocus.cx;
																	if (flag58)
																	{
																		global::Char.myCharz().cdir = 1;
																	}
																	else
																	{
																		global::Char.myCharz().cdir = -1;
																	}
																	global::Char.myCharz().cvx = 0;
																	bool flag59 = num13 <= global::Char.myCharz().myskill.dx && num14 <= global::Char.myCharz().myskill.dy;
																	if (flag59)
																	{
																		bool flag60 = global::Char.myCharz().myskill.template.id == 20;
																		if (flag60)
																		{
																			flag2 = true;
																		}
																		else
																		{
																			int num15 = 20;
																			bool flag61 = global::Char.myCharz().myskill.dx > 60;
																			if (flag61)
																			{
																				num15 = 60;
																				bool flag62 = num13 < 20;
																				if (flag62)
																				{
																					global::Char.myCharz().createShadow(global::Char.myCharz().cx, global::Char.myCharz().cy, 10);
																				}
																			}
																			bool flag63 = false;
																			bool flag64 = (TileMap.tileTypeAtPixel(global::Char.myCharz().cx, global::Char.myCharz().cy + 3) & 2) == 2;
																			if (flag64)
																			{
																				int num16 = ((global::Char.myCharz().cx > global::Char.myCharz().charFocus.cx) ? 1 : (-1));
																				bool flag65 = (TileMap.tileTypeAtPixel(global::Char.myCharz().charFocus.cx + num15 * num16, global::Char.myCharz().cy + 3) & 2) != 2;
																				if (flag65)
																				{
																					flag63 = true;
																				}
																			}
																			bool flag66 = num13 <= num15 && !flag63;
																			if (flag66)
																			{
																				bool flag67 = global::Char.myCharz().cx > global::Char.myCharz().charFocus.cx;
																				if (flag67)
																				{
																					global::Char.myCharz().cx = global::Char.myCharz().charFocus.cx + num15;
																					global::Char.myCharz().cdir = -1;
																				}
																				else
																				{
																					global::Char.myCharz().cx = global::Char.myCharz().charFocus.cx - num15;
																					global::Char.myCharz().cdir = 1;
																				}
																				Service.gI().charMove();
																			}
																			GameCanvas.clearKeyHold();
																			GameCanvas.clearKeyPressed();
																			flag2 = true;
																		}
																	}
																	else
																	{
																		int num17 = (global::Char.myCharz().myskill.dx - 20) * ((global::Char.myCharz().cx > global::Char.myCharz().charFocus.cx) ? 1 : (-1));
																		bool flag68 = num13 <= global::Char.myCharz().myskill.dx;
																		if (flag68)
																		{
																			num17 = 0;
																		}
																		global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().charFocus.cx + num17, global::Char.myCharz().charFocus.cy);
																		global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
																		GameCanvas.clearKeyHold();
																		GameCanvas.clearKeyPressed();
																		flag2 = false;
																	}
																}
															}
														}
														else
														{
															bool flag69 = num13 < 60 && num14 < 40;
															if (flag69)
															{
																this.playerMenu(global::Char.myCharz().charFocus);
																bool flag70 = !GameCanvas.isTouch && global::Char.myCharz().charFocus.charID >= 0 && TileMap.mapID != 51 && TileMap.mapID != 52 && this.popUpYesNo == null;
																if (flag70)
																{
																	GameCanvas.panel.setTypePlayerMenu(global::Char.myCharz().charFocus);
																	GameCanvas.panel.show();
																	Service.gI().getPlayerMenu(global::Char.myCharz().charFocus.charID);
																	Service.gI().messagePlayerMenu(global::Char.myCharz().charFocus.charID);
																}
															}
															else
															{
																int num18 = (20 + Res.r.nextInt(20)) * ((global::Char.myCharz().cx > global::Char.myCharz().charFocus.cx) ? 1 : (-1));
																global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().charFocus.cx + num18, global::Char.myCharz().charFocus.cy);
																global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
																GameCanvas.clearKeyHold();
																GameCanvas.clearKeyPressed();
															}
															flag2 = false;
														}
													}
												}
												else
												{
													bool flag71 = global::Char.myCharz().itemFocus != null;
													if (flag71)
													{
														this.pickItem();
														flag2 = false;
													}
													else
													{
														flag2 = true;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return flag2;
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x00057148 File Offset: 0x00055348
	public bool isMeCanAttackMob(Mob m)
	{
		bool flag = m == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = global::Char.myCharz().cTypePk == 5;
			if (flag3)
			{
				flag2 = true;
			}
			else
			{
				bool flag4 = global::Char.myCharz().isAttacPlayerStatus() && !m.isMobMe;
				if (flag4)
				{
					flag2 = false;
				}
				else
				{
					bool flag5 = global::Char.myCharz().mobMe != null && m.Equals(global::Char.myCharz().mobMe);
					if (flag5)
					{
						flag2 = false;
					}
					else
					{
						global::Char @char = GameScr.findCharInMap(m.mobId);
						bool flag6 = @char == null;
						if (flag6)
						{
							flag2 = true;
						}
						else
						{
							bool flag7 = @char.cTypePk == 5;
							if (flag7)
							{
								flag2 = true;
							}
							else
							{
								bool flag8 = global::Char.myCharz().isMeCanAttackOtherPlayer(@char);
								flag2 = flag8;
							}
						}
					}
				}
			}
		}
		return flag2;
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x00057218 File Offset: 0x00055418
	private bool checkSkillValid()
	{
		bool flag = global::Char.myCharz().myskill != null && ((global::Char.myCharz().myskill.template.manaUseType != 1 && global::Char.myCharz().cMP < (long)global::Char.myCharz().myskill.manaUse) || (global::Char.myCharz().myskill.template.manaUseType == 1 && global::Char.myCharz().cMP < global::Char.myCharz().cMPFull * (long)global::Char.myCharz().myskill.manaUse / 100L));
		bool flag2;
		if (flag)
		{
			GameScr.info1.addInfo(mResources.NOT_ENOUGH_MP, 0);
			this.auto = 0;
			flag2 = false;
		}
		else
		{
			bool flag3 = global::Char.myCharz().myskill == null || (global::Char.myCharz().myskill.template.maxPoint > 0 && global::Char.myCharz().myskill.point == 0);
			if (flag3)
			{
				GameCanvas.startOKDlg(mResources.SKILL_FAIL);
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
		}
		return flag2;
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x00057328 File Offset: 0x00055528
	private bool checkSkillValid2()
	{
		bool flag = global::Char.myCharz().myskill != null && ((global::Char.myCharz().myskill.template.manaUseType != 1 && global::Char.myCharz().cMP < (long)global::Char.myCharz().myskill.manaUse) || (global::Char.myCharz().myskill.template.manaUseType == 1 && global::Char.myCharz().cMP < global::Char.myCharz().cMPFull * (long)global::Char.myCharz().myskill.manaUse / 100L));
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = global::Char.myCharz().myskill == null || (global::Char.myCharz().myskill.template.maxPoint > 0 && global::Char.myCharz().myskill.point == 0);
			flag2 = !flag3;
		}
		return flag2;
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x00057414 File Offset: 0x00055614
	public void resetButton()
	{
		GameCanvas.menu.showMenu = false;
		ChatTextField.gI().close();
		ChatTextField.gI().center = null;
		this.isLockKey = false;
		this.typeTrade = 0;
		GameScr.indexMenu = 0;
		GameScr.indexSelect = 0;
		this.indexItemUse = -1;
		GameScr.indexRow = -1;
		GameScr.indexRowMax = 0;
		GameScr.indexTitle = 0;
		this.typeTrade = (this.typeTradeOrder = 0);
		mSystem.endKey();
		bool flag = global::Char.myCharz().cHP <= 0L || global::Char.myCharz().statusMe == 14 || global::Char.myCharz().statusMe == 5;
		if (flag)
		{
			bool meDead = global::Char.myCharz().meDead;
			if (meDead)
			{
				this.cmdDead = new Command(mResources.DIES[0], 11038);
				this.center = this.cmdDead;
				global::Char.myCharz().cHP = 0L;
			}
			GameScr.isHaveSelectSkill = false;
		}
		else
		{
			GameScr.isHaveSelectSkill = true;
		}
		GameScr.scrMain.clear();
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x00004DC3 File Offset: 0x00002FC3
	public override void keyPress(int keyCode)
	{
		base.keyPress(keyCode);
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x00057518 File Offset: 0x00055718
	public override void updateKey()
	{
		bool flag = Controller.isStopReadMessage || global::Char.myCharz().isTeleport || global::Char.myCharz().isPaintNewSkill || InfoDlg.isLock;
		if (!flag)
		{
			bool flag2 = GameCanvas.isTouch && !ChatTextField.gI().isShow && !GameCanvas.menu.showMenu;
			if (flag2)
			{
				this.updateKeyTouchControl();
			}
			this.checkAuto();
			GameCanvas.debug("F2", 0);
			bool flag3 = ChatPopup.currChatPopup != null;
			if (flag3)
			{
				Command cmdNextLine = ChatPopup.currChatPopup.cmdNextLine;
				bool flag4 = (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(cmdNextLine)) && cmdNextLine != null;
				if (flag4)
				{
					GameCanvas.isPointerJustRelease = false;
					GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
					mScreen.keyTouch = -1;
					if (cmdNextLine != null)
					{
						cmdNextLine.performAction();
					}
				}
			}
			else
			{
				bool flag5 = !ChatTextField.gI().isShow;
				if (flag5)
				{
					bool flag6 = (GameCanvas.keyPressed[12] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.left)) && this.left != null;
					if (flag6)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.isPointerClick = false;
						GameCanvas.keyPressed[12] = false;
						mScreen.keyTouch = -1;
						bool flag7 = this.left != null;
						if (flag7)
						{
							this.left.performAction();
						}
					}
					bool flag8 = (GameCanvas.keyPressed[13] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.right)) && this.right != null;
					if (flag8)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.isPointerClick = false;
						GameCanvas.keyPressed[13] = false;
						mScreen.keyTouch = -1;
						bool flag9 = this.right != null;
						if (flag9)
						{
							this.right.performAction();
						}
					}
					bool flag10 = (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.center)) && this.center != null;
					if (flag10)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
						mScreen.keyTouch = -1;
						bool flag11 = this.center != null;
						if (flag11)
						{
							this.center.performAction();
						}
					}
				}
				else
				{
					bool flag12 = ChatTextField.gI().left != null && (GameCanvas.keyPressed[12] || mScreen.getCmdPointerLast(ChatTextField.gI().left)) && ChatTextField.gI().left != null;
					if (flag12)
					{
						ChatTextField.gI().left.performAction();
					}
					bool flag13 = ChatTextField.gI().right != null && (GameCanvas.keyPressed[13] || mScreen.getCmdPointerLast(ChatTextField.gI().right)) && ChatTextField.gI().right != null;
					if (flag13)
					{
						ChatTextField.gI().right.performAction();
					}
					bool flag14 = ChatTextField.gI().center != null && (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(ChatTextField.gI().center)) && ChatTextField.gI().center != null;
					if (flag14)
					{
						ChatTextField.gI().center.performAction();
					}
				}
			}
			GameCanvas.debug("F6", 0);
			this.updateKeyAlert();
			GameCanvas.debug("F7", 0);
			bool flag15 = global::Char.myCharz().currentMovePoint != null;
			if (flag15)
			{
				for (int i = 0; i < GameCanvas.keyPressed.Length; i++)
				{
					bool flag16 = GameCanvas.keyPressed[i];
					if (flag16)
					{
						global::Char.myCharz().currentMovePoint = null;
						break;
					}
				}
			}
			GameCanvas.debug("F8", 0);
			bool flag17 = ChatTextField.gI().isShow && GameCanvas.keyAsciiPress != 0;
			if (flag17)
			{
				ChatTextField.gI().keyPressed(GameCanvas.keyAsciiPress);
				GameCanvas.keyAsciiPress = 0;
			}
			else
			{
				bool flag18 = this.isLockKey;
				if (flag18)
				{
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
				else
				{
					bool flag19 = GameCanvas.menu.showMenu || this.isOpenUI() || global::Char.isLockKey;
					if (!flag19)
					{
						bool flag20 = GameCanvas.keyPressed[10];
						if (flag20)
						{
							GameCanvas.keyPressed[10] = false;
							this.doUseHP();
							GameCanvas.clearKeyPressed();
						}
						bool flag21 = GameCanvas.keyPressed[11] && this.mobCapcha == null;
						if (flag21)
						{
							bool flag22 = this.popUpYesNo != null;
							if (flag22)
							{
								this.popUpYesNo.cmdYes.performAction();
							}
							else
							{
								bool flag23 = GameScr.info2.info.info != null && GameScr.info2.info.info.charInfo != null;
								if (flag23)
								{
									GameCanvas.panel.setTypeMessage();
									GameCanvas.panel.show();
								}
							}
							GameCanvas.keyPressed[11] = false;
							GameCanvas.clearKeyPressed();
						}
						bool flag24 = GameCanvas.keyAsciiPress != 0 && TField.isQwerty && GameCanvas.keyAsciiPress == 32;
						if (flag24)
						{
							this.doUseHP();
							GameCanvas.keyAsciiPress = 0;
							GameCanvas.clearKeyPressed();
						}
						bool flag25 = GameCanvas.keyAsciiPress != 0 && this.mobCapcha == null && TField.isQwerty && GameCanvas.keyAsciiPress == 121;
						if (flag25)
						{
							bool flag26 = this.popUpYesNo != null;
							if (flag26)
							{
								this.popUpYesNo.cmdYes.performAction();
								GameCanvas.keyAsciiPress = 0;
								GameCanvas.clearKeyPressed();
							}
							else
							{
								bool flag27 = GameScr.info2.info.info != null && GameScr.info2.info.info.charInfo != null;
								if (flag27)
								{
									GameCanvas.panel.setTypeMessage();
									GameCanvas.panel.show();
									GameCanvas.keyAsciiPress = 0;
									GameCanvas.clearKeyPressed();
								}
							}
						}
						bool flag28 = GameCanvas.keyPressed[10] && this.mobCapcha == null;
						if (flag28)
						{
							GameCanvas.keyPressed[10] = false;
							GameScr.info2.doClick(10);
							GameCanvas.clearKeyPressed();
						}
						this.checkDrag();
						bool flag29 = !global::Char.myCharz().isFlyAndCharge;
						if (flag29)
						{
							this.checkClick();
						}
						bool flag30 = global::Char.myCharz().cmdMenu != null && global::Char.myCharz().cmdMenu.isPointerPressInside();
						if (flag30)
						{
							global::Char.myCharz().cmdMenu.performAction();
						}
						bool flag31 = global::Char.myCharz().skillPaint != null;
						if (!flag31)
						{
							bool flag32 = GameCanvas.keyAsciiPress != 0;
							if (flag32)
							{
								bool flag33 = this.mobCapcha == null;
								if (flag33)
								{
									bool isQwerty = TField.isQwerty;
									if (isQwerty)
									{
										bool flag34 = GameCanvas.keyPressed[1];
										if (flag34)
										{
											bool flag35 = GameScr.keySkill[0] != null;
											if (flag35)
											{
												this.doSelectSkill(GameScr.keySkill[0], true);
											}
										}
										else
										{
											bool flag36 = GameCanvas.keyPressed[2];
											if (flag36)
											{
												bool flag37 = GameScr.keySkill[1] != null;
												if (flag37)
												{
													this.doSelectSkill(GameScr.keySkill[1], true);
												}
											}
											else
											{
												bool flag38 = GameCanvas.keyPressed[3];
												if (flag38)
												{
													bool flag39 = GameScr.keySkill[2] != null;
													if (flag39)
													{
														this.doSelectSkill(GameScr.keySkill[2], true);
													}
												}
												else
												{
													bool flag40 = GameCanvas.keyPressed[4];
													if (flag40)
													{
														bool flag41 = GameScr.keySkill[3] != null;
														if (flag41)
														{
															this.doSelectSkill(GameScr.keySkill[3], true);
														}
													}
													else
													{
														bool flag42 = GameCanvas.keyPressed[5];
														if (flag42)
														{
															bool flag43 = GameScr.keySkill[4] != null;
															if (flag43)
															{
																this.doSelectSkill(GameScr.keySkill[4], true);
															}
														}
														else
														{
															bool flag44 = GameCanvas.keyPressed[6];
															if (flag44)
															{
																bool flag45 = GameScr.keySkill[5] != null;
																if (flag45)
																{
																	this.doSelectSkill(GameScr.keySkill[5], true);
																}
															}
															else
															{
																bool flag46 = GameCanvas.keyPressed[7];
																if (flag46)
																{
																	bool flag47 = GameScr.keySkill[6] != null;
																	if (flag47)
																	{
																		this.doSelectSkill(GameScr.keySkill[6], true);
																	}
																}
																else
																{
																	bool flag48 = GameCanvas.keyPressed[8];
																	if (flag48)
																	{
																		bool flag49 = GameScr.keySkill[7] != null;
																		if (flag49)
																		{
																			this.doSelectSkill(GameScr.keySkill[7], true);
																		}
																	}
																	else
																	{
																		bool flag50 = GameCanvas.keyPressed[9];
																		if (flag50)
																		{
																			bool flag51 = GameScr.keySkill[8] != null;
																			if (flag51)
																			{
																				this.doSelectSkill(GameScr.keySkill[8], true);
																			}
																		}
																		else
																		{
																			bool flag52 = GameCanvas.keyPressed[0];
																			if (flag52)
																			{
																				bool flag53 = GameScr.keySkill[9] != null;
																				if (flag53)
																				{
																					this.doSelectSkill(GameScr.keySkill[9], true);
																				}
																			}
																			else
																			{
																				bool flag54 = GameCanvas.keyAsciiPress == 105;
																				if (flag54)
																				{
																					MyVector myVector = new MyVector();
																					myVector.addElement(new Command("Mặc set 1", 999901));
																					myVector.addElement(new Command("Mặc set 2", 999902));
																					GameCanvas.menu.startAt(myVector, 3);
																				}
																				else
																				{
																					bool flag55 = GameCanvas.keyAsciiPress == 114;
																					if (flag55)
																					{
																						ChatTextField.gI().startChat(this, string.Empty);
																					}
																					else
																					{
																						bool flag56 = GameCanvas.keyAsciiPress == 120;
																						if (flag56)
																						{
																							XmapController.ShowXmapMenu();
																						}
																						else
																						{
																							bool flag57 = GameCanvas.keyAsciiPress == 98;
																							if (flag57)
																							{
																								bool flag58 = DataAccount.Type == 1;
																								if (flag58)
																								{
																									SocketOutPut.Send(string.Format("bom|{0}", DataAccount.Team));
																								}
																							}
																							else
																							{
																								bool flag59 = GameCanvas.keyAsciiPress == 103;
																								if (flag59)
																								{
																									bool flag60 = DataAccount.Type == 1;
																									if (flag60)
																									{
																										SocketOutPut.Send(string.Format("sp|{0}|{1}|{2}", TileMap.mapID, TileMap.zoneID, DataAccount.Team));
																									}
																								}
																								else
																								{
																									bool flag61 = GameCanvas.keyAsciiPress == 100;
																									if (flag61)
																									{
																										bool flag62 = DataAccount.Type == 1;
																										if (flag62)
																										{
																											SocketOutPut.Send("dokhu");
																										}
																									}
																									else
																									{
																										bool flag63 = GameCanvas.keyAsciiPress == 109;
																										if (flag63)
																										{
																											Service.gI().openUIZone();
																											GameCanvas.panel.setTypeZone();
																											GameCanvas.panel.show();
																										}
																										else
																										{
																											bool flag64 = GameCanvas.keyAsciiPress == 102;
																											if (flag64)
																											{
																												bool flag65 = DataAccount.Type == 1;
																												if (flag65)
																												{
																													SocketOutPut.Send(string.Format("hopthe|{0}", DataAccount.Team));
																												}
																											}
																											else
																											{
																												bool flag66 = GameCanvas.keyAsciiPress == 119;
																												if (flag66)
																												{
																													bool flag67 = DataAccount.Type == 1;
																													if (flag67)
																													{
																														SocketOutPut.Send(string.Format("bohuyet|{0}", DataAccount.Team));
																													}
																												}
																												else
																												{
																													bool flag68 = GameCanvas.keyAsciiPress == 113;
																													if (flag68)
																													{
																														bool flag69 = DataAccount.Type == 1;
																														if (flag69)
																														{
																															SocketOutPut.Send(string.Format("ttnl|{0}", DataAccount.Team));
																														}
																													}
																													else
																													{
																														bool flag70 = GameCanvas.keyAsciiPress == 106;
																														if (flag70)
																														{
																															bool flag71 = GameScr.getX(0) > 0 && GameScr.getY(0) > 0;
																															if (flag71)
																															{
																																GameScr.MoveMyChar(GameScr.getX(0), GameScr.getY(0));
																															}
																														}
																														else
																														{
																															bool flag72 = GameCanvas.keyAsciiPress == 107;
																															if (flag72)
																															{
																																bool flag73 = GameScr.getX(1) > 0 && GameScr.getY(1) > 0;
																																if (flag73)
																																{
																																	GameScr.MoveMyChar(GameScr.getX(1), GameScr.getY(1));
																																	Service.gI().getMapOffline();
																																	Service.gI().requestChangeMap();
																																}
																															}
																															else
																															{
																																bool flag74 = GameCanvas.keyAsciiPress == 108;
																																if (flag74)
																																{
																																	bool flag75 = GameScr.getX(2) > 0 && GameScr.getY(2) > 0;
																																	if (flag75)
																																	{
																																		GameScr.MoveMyChar(GameScr.getX(2), GameScr.getY(2));
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
									else
									{
										bool flag76 = !GameCanvas.isMoveNumberPad;
										if (flag76)
										{
											ChatTextField.gI().startChat(GameCanvas.keyAsciiPress, this, string.Empty);
										}
										else
										{
											bool flag77 = GameCanvas.keyAsciiPress == 55;
											if (flag77)
											{
												bool flag78 = GameScr.keySkill[0] != null;
												if (flag78)
												{
													this.doSelectSkill(GameScr.keySkill[0], true);
												}
											}
											else
											{
												bool flag79 = GameCanvas.keyAsciiPress == 56;
												if (flag79)
												{
													bool flag80 = GameScr.keySkill[1] != null;
													if (flag80)
													{
														this.doSelectSkill(GameScr.keySkill[1], true);
													}
												}
												else
												{
													bool flag81 = GameCanvas.keyAsciiPress == 57;
													if (flag81)
													{
														bool flag82 = GameScr.keySkill[(!Main.isPC) ? 2 : 21] != null;
														if (flag82)
														{
															this.doSelectSkill(GameScr.keySkill[2], true);
														}
													}
													else
													{
														bool flag83 = GameCanvas.keyAsciiPress == 48;
														if (flag83)
														{
															ChatTextField.gI().startChat(this, string.Empty);
														}
													}
												}
											}
										}
									}
								}
								else
								{
									char[] array = this.keyInput.ToCharArray();
									MyVector myVector2 = new MyVector();
									for (int j = 0; j < array.Length; j++)
									{
										myVector2.addElement(array[j].ToString() + string.Empty);
									}
									myVector2.removeElementAt(0);
									string text = ((char)GameCanvas.keyAsciiPress).ToString() + string.Empty;
									bool flag84 = text.Equals(string.Empty) || text == null || text.Equals("\n");
									if (flag84)
									{
										text = "-";
									}
									myVector2.insertElementAt(text, myVector2.size());
									this.keyInput = string.Empty;
									for (int k = 0; k < myVector2.size(); k++)
									{
										this.keyInput += ((string)myVector2.elementAt(k)).ToUpper();
									}
									Service.gI().mobCapcha((char)GameCanvas.keyAsciiPress);
								}
								GameCanvas.keyAsciiPress = 0;
							}
							bool flag85 = global::Char.myCharz().statusMe == 1;
							if (flag85)
							{
								GameCanvas.debug("F10", 0);
								bool flag86 = !this.doSeleckSkillFlag;
								if (flag86)
								{
									bool flag87 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
									if (flag87)
									{
										GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
										this.doFire(false, false);
									}
									else
									{
										bool flag88 = GameCanvas.keyHold[(!Main.isPC) ? 2 : 21];
										if (flag88)
										{
											bool flag89 = !global::Char.myCharz().isLockMove;
											if (flag89)
											{
												this.setCharJump(0);
											}
										}
										else
										{
											bool flag90 = GameCanvas.keyHold[1] && this.mobCapcha == null;
											if (flag90)
											{
												bool flag91 = !Main.isPC;
												if (flag91)
												{
													global::Char.myCharz().cdir = -1;
													bool flag92 = !global::Char.myCharz().isLockMove;
													if (flag92)
													{
														this.setCharJump(-4);
													}
												}
											}
											else
											{
												bool flag93 = GameCanvas.keyHold[(!Main.isPC) ? 5 : 25] && this.mobCapcha == null;
												if (flag93)
												{
													bool flag94 = !Main.isPC;
													if (flag94)
													{
														global::Char.myCharz().cdir = 1;
														bool flag95 = !global::Char.myCharz().isLockMove;
														if (flag95)
														{
															this.setCharJump(4);
														}
													}
												}
												else
												{
													bool flag96 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
													if (flag96)
													{
														GameScr.isAutoPlay = false;
														global::Char.myCharz().isAttack = false;
														bool flag97 = global::Char.myCharz().cdir == 1;
														if (flag97)
														{
															global::Char.myCharz().cdir = -1;
														}
														else
														{
															bool flag98 = !global::Char.myCharz().isLockMove;
															if (flag98)
															{
																bool flag99 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0;
																if (flag99)
																{
																	Service.gI().charMove();
																}
																global::Char.myCharz().statusMe = 2;
																global::Char.myCharz().cvx = -global::Char.myCharz().cspeed;
															}
														}
														global::Char.myCharz().holder = false;
													}
													else
													{
														bool flag100 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
														if (flag100)
														{
															GameScr.isAutoPlay = false;
															global::Char.myCharz().isAttack = false;
															bool flag101 = global::Char.myCharz().cdir == -1;
															if (flag101)
															{
																global::Char.myCharz().cdir = 1;
															}
															else
															{
																bool flag102 = !global::Char.myCharz().isLockMove;
																if (flag102)
																{
																	bool flag103 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0;
																	if (flag103)
																	{
																		Service.gI().charMove();
																	}
																	global::Char.myCharz().statusMe = 2;
																	global::Char.myCharz().cvx = global::Char.myCharz().cspeed;
																}
															}
															global::Char.myCharz().holder = false;
														}
													}
												}
											}
										}
									}
								}
							}
							else
							{
								bool flag104 = global::Char.myCharz().statusMe == 2;
								if (flag104)
								{
									GameCanvas.debug("F11", 0);
									bool flag105 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
									if (flag105)
									{
										GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
										this.doFire(false, true);
									}
									else
									{
										bool flag106 = GameCanvas.keyHold[(!Main.isPC) ? 2 : 21];
										if (flag106)
										{
											bool flag107 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0;
											if (flag107)
											{
												Service.gI().charMove();
											}
											global::Char.myCharz().cvy = -10;
											global::Char.myCharz().statusMe = 3;
											global::Char.myCharz().cp1 = 0;
										}
										else
										{
											bool flag108 = GameCanvas.keyHold[1] && this.mobCapcha == null;
											if (flag108)
											{
												bool isPC = Main.isPC;
												if (isPC)
												{
													bool flag109 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0;
													if (flag109)
													{
														Service.gI().charMove();
													}
													global::Char.myCharz().cdir = -1;
													global::Char.myCharz().cvy = -10;
													global::Char.myCharz().cvx = -4;
													global::Char.myCharz().statusMe = 3;
													global::Char.myCharz().cp1 = 0;
												}
											}
											else
											{
												bool flag110 = GameCanvas.keyHold[3] && this.mobCapcha == null;
												if (flag110)
												{
													bool flag111 = !Main.isPC;
													if (flag111)
													{
														bool flag112 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0;
														if (flag112)
														{
															Service.gI().charMove();
														}
														global::Char.myCharz().cdir = 1;
														global::Char.myCharz().cvy = -10;
														global::Char.myCharz().cvx = 4;
														global::Char.myCharz().statusMe = 3;
														global::Char.myCharz().cp1 = 0;
													}
												}
												else
												{
													bool flag113 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
													if (flag113)
													{
														GameScr.isAutoPlay = false;
														bool flag114 = global::Char.myCharz().cdir == 1;
														if (flag114)
														{
															global::Char.myCharz().cdir = -1;
														}
														else
														{
															global::Char.myCharz().cvx = -global::Char.myCharz().cspeed + global::Char.myCharz().cBonusSpeed;
														}
													}
													else
													{
														bool flag115 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
														if (flag115)
														{
															GameScr.isAutoPlay = false;
															bool flag116 = global::Char.myCharz().cdir == -1;
															if (flag116)
															{
																global::Char.myCharz().cdir = 1;
															}
															else
															{
																global::Char.myCharz().cvx = global::Char.myCharz().cspeed + global::Char.myCharz().cBonusSpeed;
															}
														}
													}
												}
											}
										}
									}
								}
								else
								{
									bool flag117 = global::Char.myCharz().statusMe == 3;
									if (flag117)
									{
										GameScr.isAutoPlay = false;
										GameCanvas.debug("F12", 0);
										bool flag118 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
										if (flag118)
										{
											GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
											this.doFire(false, true);
										}
										bool flag119 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] || (GameCanvas.keyHold[1] && this.mobCapcha == null);
										if (flag119)
										{
											bool flag120 = global::Char.myCharz().cdir == 1;
											if (flag120)
											{
												global::Char.myCharz().cdir = -1;
											}
											else
											{
												global::Char.myCharz().cvx = -global::Char.myCharz().cspeed;
											}
										}
										else
										{
											bool flag121 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] || (GameCanvas.keyHold[3] && this.mobCapcha == null);
											if (flag121)
											{
												bool flag122 = global::Char.myCharz().cdir == -1;
												if (flag122)
												{
													global::Char.myCharz().cdir = 1;
												}
												else
												{
													global::Char.myCharz().cvx = global::Char.myCharz().cspeed;
												}
											}
										}
										bool flag123 = (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] || ((GameCanvas.keyHold[1] || GameCanvas.keyHold[3]) && this.mobCapcha == null)) && global::Char.myCharz().canFly && global::Char.myCharz().cMP > 0L && global::Char.myCharz().cp1 < 8 && global::Char.myCharz().cvy > -4;
										if (flag123)
										{
											global::Char.myCharz().cp1++;
											global::Char.myCharz().cvy = -7;
										}
									}
									else
									{
										bool flag124 = global::Char.myCharz().statusMe == 4;
										if (flag124)
										{
											GameCanvas.debug("F13", 0);
											bool flag125 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
											if (flag125)
											{
												GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
												this.doFire(false, true);
											}
											bool flag126 = GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] && global::Char.myCharz().cMP > 0L && global::Char.myCharz().canFly;
											if (flag126)
											{
												GameScr.isAutoPlay = false;
												bool flag127 = (global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0) && (Res.abs(global::Char.myCharz().cx - global::Char.myCharz().cxSend) > 96 || Res.abs(global::Char.myCharz().cy - global::Char.myCharz().cySend) > 24);
												if (flag127)
												{
													Service.gI().charMove();
												}
												global::Char.myCharz().cvy = -10;
												global::Char.myCharz().statusMe = 3;
												global::Char.myCharz().cp1 = 0;
											}
											bool flag128 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
											if (flag128)
											{
												GameScr.isAutoPlay = false;
												bool flag129 = global::Char.myCharz().cdir == 1;
												if (flag129)
												{
													global::Char.myCharz().cdir = -1;
												}
												else
												{
													global::Char.myCharz().cp1++;
													global::Char.myCharz().cvx = -global::Char.myCharz().cspeed;
													bool flag130 = global::Char.myCharz().cp1 > 5 && global::Char.myCharz().cvy > 6;
													if (flag130)
													{
														global::Char.myCharz().statusMe = 10;
														global::Char.myCharz().cp1 = 0;
														global::Char.myCharz().cvy = 0;
													}
												}
											}
											else
											{
												bool flag131 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
												if (flag131)
												{
													GameScr.isAutoPlay = false;
													bool flag132 = global::Char.myCharz().cdir == -1;
													if (flag132)
													{
														global::Char.myCharz().cdir = 1;
													}
													else
													{
														global::Char.myCharz().cp1++;
														global::Char.myCharz().cvx = global::Char.myCharz().cspeed;
														bool flag133 = global::Char.myCharz().cp1 > 5 && global::Char.myCharz().cvy > 6;
														if (flag133)
														{
															global::Char.myCharz().statusMe = 10;
															global::Char.myCharz().cp1 = 0;
															global::Char.myCharz().cvy = 0;
														}
													}
												}
											}
										}
										else
										{
											bool flag134 = global::Char.myCharz().statusMe == 10;
											if (flag134)
											{
												GameCanvas.debug("F14", 0);
												bool flag135 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
												if (flag135)
												{
													GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
													this.doFire(false, true);
												}
												bool flag136 = global::Char.myCharz().canFly && global::Char.myCharz().cMP > 0L;
												if (flag136)
												{
													bool flag137 = GameCanvas.keyHold[(!Main.isPC) ? 2 : 21];
													if (flag137)
													{
														GameScr.isAutoPlay = false;
														bool flag138 = (global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0) && (Res.abs(global::Char.myCharz().cx - global::Char.myCharz().cxSend) > 96 || Res.abs(global::Char.myCharz().cy - global::Char.myCharz().cySend) > 24);
														if (flag138)
														{
															Service.gI().charMove();
														}
														global::Char.myCharz().cvy = -10;
														global::Char.myCharz().statusMe = 3;
														global::Char.myCharz().cp1 = 0;
													}
													else
													{
														bool flag139 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
														if (flag139)
														{
															GameScr.isAutoPlay = false;
															bool flag140 = global::Char.myCharz().cdir == 1;
															if (flag140)
															{
																global::Char.myCharz().cdir = -1;
															}
															else
															{
																global::Char.myCharz().cvx = -(global::Char.myCharz().cspeed + 1);
															}
														}
														else
														{
															bool flag141 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
															if (flag141)
															{
																bool flag142 = global::Char.myCharz().cdir == -1;
																if (flag142)
																{
																	global::Char.myCharz().cdir = 1;
																}
																else
																{
																	global::Char.myCharz().cvx = global::Char.myCharz().cspeed + 1;
																}
															}
														}
													}
												}
											}
											else
											{
												bool flag143 = global::Char.myCharz().statusMe == 7;
												if (flag143)
												{
													GameCanvas.debug("F15", 0);
													bool flag144 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
													if (flag144)
													{
														GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
													}
													bool flag145 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
													if (flag145)
													{
														GameScr.isAutoPlay = false;
														bool flag146 = global::Char.myCharz().cdir == 1;
														if (flag146)
														{
															global::Char.myCharz().cdir = -1;
														}
														else
														{
															global::Char.myCharz().cvx = -global::Char.myCharz().cspeed + 2;
														}
													}
													else
													{
														bool flag147 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
														if (flag147)
														{
															GameScr.isAutoPlay = false;
															bool flag148 = global::Char.myCharz().cdir == -1;
															if (flag148)
															{
																global::Char.myCharz().cdir = 1;
															}
															else
															{
																global::Char.myCharz().cvx = global::Char.myCharz().cspeed - 2;
															}
														}
													}
												}
											}
										}
									}
								}
							}
							GameCanvas.debug("F17", 0);
							bool flag149 = GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] && GameCanvas.keyAsciiPress != 56;
							if (flag149)
							{
								GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] = false;
								global::Char.myCharz().delayFall = 0;
							}
							bool flag150 = GameCanvas.keyPressed[10];
							if (flag150)
							{
								GameCanvas.keyPressed[10] = false;
								this.doUseHP();
							}
							GameCanvas.debug("F20", 0);
							GameCanvas.clearKeyPressed();
							GameCanvas.debug("F23", 0);
							this.doSeleckSkillFlag = false;
						}
					}
				}
			}
		}
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x000515EC File Offset: 0x0004F7EC
	public bool isVsMap()
	{
		return true;
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x00059180 File Offset: 0x00057380
	private void checkDrag()
	{
		bool flag = GameScr.isAnalog == 1 || GameScr.gamePad.disableCheckDrag();
		if (!flag)
		{
			global::Char.myCharz().cmtoChar = true;
			bool flag2 = GameScr.isUseTouch;
			if (!flag2)
			{
				bool isPointerJustDown = GameCanvas.isPointerJustDown;
				if (isPointerJustDown)
				{
					GameCanvas.isPointerJustDown = false;
					this.isPointerDowning = true;
					this.ptDownTime = 0;
					this.ptLastDownX = (this.ptFirstDownX = GameCanvas.px);
					this.ptLastDownY = (this.ptFirstDownY = GameCanvas.py);
				}
				bool flag3 = this.isPointerDowning;
				if (flag3)
				{
					int num = GameCanvas.px - this.ptLastDownX;
					int num2 = GameCanvas.py - this.ptLastDownY;
					bool flag4 = !this.isChangingCameraMode && (Res.abs(GameCanvas.px - this.ptFirstDownX) > 15 || Res.abs(GameCanvas.py - this.ptFirstDownY) > 15);
					if (flag4)
					{
						this.isChangingCameraMode = true;
					}
					this.ptLastDownX = GameCanvas.px;
					this.ptLastDownY = GameCanvas.py;
					this.ptDownTime++;
					bool flag5 = this.isChangingCameraMode;
					if (flag5)
					{
						global::Char.myCharz().cmtoChar = false;
						GameScr.cmx -= num;
						GameScr.cmy -= num2;
						bool flag6 = GameScr.cmx < 24;
						if (flag6)
						{
							int num3 = (24 - GameScr.cmx) / 3;
							bool flag7 = num3 != 0;
							if (flag7)
							{
								GameScr.cmx += num - num / num3;
							}
						}
						bool flag8 = GameScr.cmx < (this.isVsMap() ? 24 : 0);
						if (flag8)
						{
							GameScr.cmx = (this.isVsMap() ? 24 : 0);
						}
						bool flag9 = GameScr.cmx > GameScr.cmxLim;
						if (flag9)
						{
							int num4 = (GameScr.cmx - GameScr.cmxLim) / 3;
							bool flag10 = num4 != 0;
							if (flag10)
							{
								GameScr.cmx += num - num / num4;
							}
						}
						bool flag11 = GameScr.cmx > GameScr.cmxLim + ((!this.isVsMap()) ? 24 : 0);
						if (flag11)
						{
							GameScr.cmx = GameScr.cmxLim + ((!this.isVsMap()) ? 24 : 0);
						}
						bool flag12 = GameScr.cmy < 0;
						if (flag12)
						{
							int num5 = -GameScr.cmy / 3;
							bool flag13 = num5 != 0;
							if (flag13)
							{
								GameScr.cmy += num2 - num2 / num5;
							}
						}
						bool flag14 = GameScr.cmy < -((!this.isVsMap()) ? 24 : 0);
						if (flag14)
						{
							GameScr.cmy = -((!this.isVsMap()) ? 24 : 0);
						}
						bool flag15 = GameScr.cmy > GameScr.cmyLim;
						if (flag15)
						{
							GameScr.cmy = GameScr.cmyLim;
						}
						GameScr.cmtoX = GameScr.cmx;
						GameScr.cmtoY = GameScr.cmy;
					}
				}
				bool flag16 = this.isPointerDowning && GameCanvas.isPointerJustRelease;
				if (flag16)
				{
					this.isPointerDowning = false;
					this.isChangingCameraMode = false;
					bool flag17 = Res.abs(GameCanvas.px - this.ptFirstDownX) > 15 || Res.abs(GameCanvas.py - this.ptFirstDownY) > 15;
					if (flag17)
					{
						GameCanvas.isPointerJustRelease = false;
					}
				}
			}
		}
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x000594C4 File Offset: 0x000576C4
	private void checkClick()
	{
		bool flag = this.isCharging();
		if (!flag)
		{
			bool flag2 = this.popUpYesNo != null && this.popUpYesNo.cmdYes != null && this.popUpYesNo.cmdYes.isPointerPressInside();
			if (flag2)
			{
				this.popUpYesNo.cmdYes.performAction();
			}
			else
			{
				bool flag3 = this.checkClickToCapcha();
				if (!flag3)
				{
					long num = mSystem.currentTimeMillis();
					bool flag4 = this.lastSingleClick != 0L;
					if (flag4)
					{
						this.lastSingleClick = 0L;
						GameCanvas.isPointerJustDown = false;
						bool flag5 = !this.disableSingleClick;
						if (flag5)
						{
							this.checkSingleClick();
							GameCanvas.isPointerJustRelease = false;
							this.isWaitingDoubleClick = true;
							this.timeStartDblClick = mSystem.currentTimeMillis();
						}
					}
					bool flag6 = this.isWaitingDoubleClick;
					if (flag6)
					{
						this.timeEndDblClick = mSystem.currentTimeMillis();
						bool flag7 = this.timeEndDblClick - this.timeStartDblClick < 300L && GameCanvas.isPointerJustRelease;
						if (flag7)
						{
							this.isWaitingDoubleClick = false;
							this.checkDoubleClick();
						}
					}
					bool isPointerJustRelease = GameCanvas.isPointerJustRelease;
					if (isPointerJustRelease)
					{
						this.disableSingleClick = this.checkSingleClickEarly();
						this.lastSingleClick = num;
						this.lastClickCMX = GameScr.cmx;
						this.lastClickCMY = GameScr.cmy;
						GameCanvas.isPointerJustRelease = false;
					}
				}
			}
		}
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x00059618 File Offset: 0x00057818
	private IMapObject findClickToItem(int px, int py)
	{
		IMapObject mapObject = null;
		int num = 0;
		int num2 = 30;
		MyVector[] array = new MyVector[]
		{
			GameScr.vMob,
			GameScr.vNpc,
			GameScr.vItemMap,
			GameScr.vCharInMap
		};
		for (int i = 0; i < array.Length; i++)
		{
			for (int j = 0; j < array[i].size(); j++)
			{
				IMapObject mapObject2 = (IMapObject)array[i].elementAt(j);
				bool flag = mapObject2.isInvisible();
				if (!flag)
				{
					bool flag2 = mapObject2 is Mob;
					if (flag2)
					{
						Mob mob = (Mob)mapObject2;
						bool flag3 = mob.isMobMe && mob.Equals(global::Char.myCharz().mobMe);
						if (flag3)
						{
							goto IL_015F;
						}
					}
					int x = mapObject2.getX();
					int y = mapObject2.getY();
					int w = mapObject2.getW();
					int h = mapObject2.getH();
					bool flag4 = !this.inRectangle(px, py, x - w / 2 - num2, y - h - num2, w + num2 * 2, h + num2 * 2);
					if (!flag4)
					{
						bool flag5 = mapObject == null;
						if (flag5)
						{
							mapObject = mapObject2;
							num = Res.abs(px - x) + Res.abs(py - y);
							bool flag6 = i == 1;
							if (flag6)
							{
								return mapObject;
							}
						}
						else
						{
							int num3 = Res.abs(px - x) + Res.abs(py - y);
							bool flag7 = num3 < num;
							if (flag7)
							{
								mapObject = mapObject2;
								num = num3;
							}
						}
					}
				}
				IL_015F:;
			}
		}
		return mapObject;
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x000597C0 File Offset: 0x000579C0
	private Mob findClickToMOB(int px, int py)
	{
		int num = 30;
		Mob mob = null;
		int num2 = 0;
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob2 = (Mob)GameScr.vMob.elementAt(i);
			bool flag = mob2.isInvisible();
			if (!flag)
			{
				bool flag2 = mob2 != null;
				if (flag2)
				{
					Mob mob3 = mob2;
					bool flag3 = mob3.isMobMe && mob3.Equals(global::Char.myCharz().mobMe);
					if (flag3)
					{
						goto IL_0110;
					}
				}
				int x = mob2.getX();
				int y = mob2.getY();
				int w = mob2.getW();
				int h = mob2.getH();
				bool flag4 = !this.inRectangle(px, py, x - w / 2 - num, y - h - num, w + num * 2, h + num * 2);
				if (!flag4)
				{
					bool flag5 = mob == null;
					if (flag5)
					{
						mob = mob2;
						num2 = Res.abs(px - x) + Res.abs(py - y);
					}
					else
					{
						int num3 = Res.abs(px - x) + Res.abs(py - y);
						bool flag6 = num3 < num2;
						if (flag6)
						{
							mob = mob2;
							num2 = num3;
						}
					}
				}
			}
			IL_0110:;
		}
		return mob;
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00059900 File Offset: 0x00057B00
	private bool inRectangle(int xClick, int yClick, int x, int y, int w, int h)
	{
		return xClick >= x && xClick <= x + w && yClick >= y && yClick <= y + h;
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00059930 File Offset: 0x00057B30
	private bool checkSingleClickEarly()
	{
		int num = GameCanvas.px + GameScr.cmx;
		int num2 = GameCanvas.py + GameScr.cmy;
		global::Char.myCharz().cancelAttack();
		IMapObject mapObject = this.findClickToItem(num, num2);
		bool flag = mapObject != null;
		bool flag5;
		if (flag)
		{
			bool flag2 = global::Char.myCharz().isAttacPlayerStatus() && global::Char.myCharz().charFocus != null && !mapObject.Equals(global::Char.myCharz().charFocus) && !mapObject.Equals(global::Char.myCharz().charFocus.mobMe) && mapObject is global::Char;
			if (flag2)
			{
				global::Char @char = (global::Char)mapObject;
				bool flag3 = @char.cTypePk != 5 && !@char.isAttacPlayerStatus();
				if (flag3)
				{
					this.checkClickMoveTo(num, num2, 2);
					return false;
				}
			}
			bool flag4 = global::Char.myCharz().mobFocus == mapObject || global::Char.myCharz().itemFocus == mapObject;
			if (flag4)
			{
				this.doDoubleClickToObj(mapObject);
				flag5 = true;
			}
			else
			{
				bool flag6 = TileMap.mapID == 51 && mapObject.Equals(global::Char.myCharz().npcFocus);
				if (flag6)
				{
					this.checkClickMoveTo(num, num2, 3);
					flag5 = false;
				}
				else
				{
					bool flag7 = global::Char.myCharz().skillPaint != null || global::Char.myCharz().arr != null || global::Char.myCharz().dart != null || global::Char.myCharz().skillInfoPaint() != null;
					if (flag7)
					{
						flag5 = false;
					}
					else
					{
						global::Char.myCharz().focusManualTo(mapObject);
						mapObject.stopMoving();
						flag5 = false;
					}
				}
			}
		}
		else
		{
			flag5 = false;
		}
		return flag5;
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x00059AC8 File Offset: 0x00057CC8
	private void checkDoubleClick()
	{
		int num = GameCanvas.px + this.lastClickCMX;
		int num2 = GameCanvas.py + this.lastClickCMY;
		int cy = global::Char.myCharz().cy;
		bool flag = this.isLockKey;
		if (!flag)
		{
			IMapObject mapObject = this.findClickToItem(num, num2);
			bool flag2 = mapObject != null;
			if (flag2)
			{
				bool flag3 = mapObject is Mob && !this.isMeCanAttackMob((Mob)mapObject);
				if (flag3)
				{
					this.checkClickMoveTo(num, num2, 4);
				}
				else
				{
					bool flag4 = this.checkClickToBotton(mapObject) || (!mapObject.Equals(global::Char.myCharz().npcFocus) && this.mobCapcha != null);
					if (!flag4)
					{
						bool flag5 = global::Char.myCharz().isAttacPlayerStatus() && global::Char.myCharz().charFocus != null && !mapObject.Equals(global::Char.myCharz().charFocus) && !mapObject.Equals(global::Char.myCharz().charFocus.mobMe) && mapObject is global::Char;
						if (flag5)
						{
							global::Char @char = (global::Char)mapObject;
							bool flag6 = @char.cTypePk != 5 && !@char.isAttacPlayerStatus();
							if (flag6)
							{
								this.checkClickMoveTo(num, num2, 5);
								return;
							}
						}
						bool flag7 = TileMap.mapID == 51 && mapObject.Equals(global::Char.myCharz().npcFocus);
						if (flag7)
						{
							this.checkClickMoveTo(num, num2, 6);
						}
						else
						{
							this.doDoubleClickToObj(mapObject);
						}
					}
				}
			}
			else
			{
				bool flag8 = !this.checkClickToPopup(num, num2) && !this.checkClipTopChatPopUp(num, num2) && !Main.isPC;
				if (flag8)
				{
					this.checkClickMoveTo(num, num2, 7);
				}
			}
		}
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00059C84 File Offset: 0x00057E84
	private bool checkClickToBotton(IMapObject Object)
	{
		bool flag = Object == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			int i = Object.getY();
			int num = global::Char.myCharz().cy;
			bool flag3 = i < num;
			if (flag3)
			{
				while (i < num)
				{
					num -= 5;
					bool flag4 = TileMap.tileTypeAt(global::Char.myCharz().cx, num, 8192);
					if (flag4)
					{
						this.auto = 0;
						global::Char.myCharz().cancelAttack();
						global::Char.myCharz().currentMovePoint = null;
						return true;
					}
				}
			}
			flag2 = false;
		}
		return flag2;
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x00059D14 File Offset: 0x00057F14
	private void doDoubleClickToObj(IMapObject obj)
	{
		bool flag = (obj.Equals(global::Char.myCharz().npcFocus) || this.mobCapcha == null) && !this.checkClickToBotton(obj);
		if (flag)
		{
			this.checkEffToObj(obj, false);
			global::Char.myCharz().cancelAttack();
			global::Char.myCharz().currentMovePoint = null;
			global::Char.myCharz().cvx = (global::Char.myCharz().cvy = 0);
			obj.stopMoving();
			this.auto = 10;
			this.doFire(false, true);
			this.clickToX = obj.getX();
			this.clickToY = obj.getY();
			this.clickOnTileTop = false;
			this.clickMoving = true;
			this.clickMovingRed = true;
			this.clickMovingTimeOut = 20;
			this.clickMovingP1 = 30;
		}
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x00059DE0 File Offset: 0x00057FE0
	private void checkSingleClick()
	{
		int num = GameCanvas.px + this.lastClickCMX;
		int num2 = GameCanvas.py + this.lastClickCMY;
		bool flag = !this.isLockKey && !this.checkClickToPopup(num, num2) && !this.checkClipTopChatPopUp(num, num2);
		if (flag)
		{
			this.checkClickMoveTo(num, num2, 0);
		}
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x00059E38 File Offset: 0x00058038
	private bool checkClipTopChatPopUp(int xClick, int yClick)
	{
		bool flag = this.Equals(GameScr.info2) && GameScr.gI().popUpYesNo != null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = GameScr.info2.info.info != null && GameScr.info2.info.info.charInfo != null;
			if (flag3)
			{
				int num = Res.abs(GameScr.info2.cmx) + GameScr.info2.info.X - 40;
				int num2 = Res.abs(GameScr.info2.cmy) + GameScr.info2.info.Y;
				bool flag4 = this.inRectangle(xClick - GameScr.cmx, yClick - GameScr.cmy, num, num2, 200, GameScr.info2.info.H);
				if (flag4)
				{
					GameScr.info2.doClick(10);
					return true;
				}
			}
			flag2 = false;
		}
		return flag2;
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x00059F34 File Offset: 0x00058134
	private bool checkClickToPopup(int xClick, int yClick)
	{
		for (int i = 0; i < PopUp.vPopups.size(); i++)
		{
			PopUp popUp = (PopUp)PopUp.vPopups.elementAt(i);
			bool flag = this.inRectangle(xClick, yClick, popUp.cx, popUp.cy, popUp.cw, popUp.ch);
			if (flag)
			{
				bool flag2 = popUp.cy <= 24 && TileMap.isInAirMap() && global::Char.myCharz().cTypePk != 0;
				bool flag3;
				if (flag2)
				{
					flag3 = false;
				}
				else
				{
					bool flag4 = popUp.isPaint;
					if (!flag4)
					{
						goto IL_0086;
					}
					popUp.doClick(10);
					flag3 = true;
				}
				return flag3;
			}
			IL_0086:;
		}
		return false;
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x00059FEC File Offset: 0x000581EC
	private void checkClickMoveTo(int xClick, int yClick, int index)
	{
		bool flag = GameScr.gamePad.disableClickMove();
		if (!flag)
		{
			global::Char.myCharz().cancelAttack();
			bool flag2 = xClick < TileMap.pxw && xClick > TileMap.pxw - 32;
			if (flag2)
			{
				global::Char.myCharz().currentMovePoint = new MovePoint(TileMap.pxw, yClick);
			}
			else
			{
				bool flag3 = xClick < 32 && xClick > 0;
				if (flag3)
				{
					global::Char.myCharz().currentMovePoint = new MovePoint(0, yClick);
				}
				else
				{
					bool flag4 = xClick < TileMap.pxw && xClick > TileMap.pxw - 48;
					if (flag4)
					{
						global::Char.myCharz().currentMovePoint = new MovePoint(TileMap.pxw, yClick);
					}
					else
					{
						bool flag5 = xClick < 48 && xClick > 0;
						if (flag5)
						{
							global::Char.myCharz().currentMovePoint = new MovePoint(0, yClick);
						}
						else
						{
							this.clickToX = xClick;
							this.clickToY = yClick;
							this.clickOnTileTop = false;
							global::Char.myCharz().delayFall = 0;
							int num = ((!global::Char.myCharz().canFly || global::Char.myCharz().cMP <= 0L) ? 1000 : 0);
							bool flag6 = this.clickToY > global::Char.myCharz().cy && Res.abs(this.clickToX - global::Char.myCharz().cx) < 12;
							if (!flag6)
							{
								int num2 = 0;
								while (num2 < 60 + num && this.clickToY + num2 < TileMap.pxh - 24)
								{
									bool flag7 = TileMap.tileTypeAt(this.clickToX, this.clickToY + num2, 2);
									if (flag7)
									{
										this.clickToY = TileMap.tileYofPixel(this.clickToY + num2);
										this.clickOnTileTop = true;
										break;
									}
									num2 += 24;
								}
								for (int i = 0; i < 40 + num; i += 24)
								{
									bool flag8 = TileMap.tileTypeAt(this.clickToX, this.clickToY - i, 2);
									if (flag8)
									{
										this.clickToY = TileMap.tileYofPixel(this.clickToY - i);
										this.clickOnTileTop = true;
										break;
									}
								}
								this.clickMoving = true;
								this.clickMovingRed = false;
								this.clickMovingP1 = ((!this.clickOnTileTop) ? 30 : ((yClick >= this.clickToY) ? this.clickToY : yClick));
								global::Char.myCharz().delayFall = 0;
								bool flag9 = !this.clickOnTileTop && this.clickToY < global::Char.myCharz().cy - 50;
								if (flag9)
								{
									global::Char.myCharz().delayFall = 20;
								}
								this.clickMovingTimeOut = 30;
								this.auto = 0;
								bool holder = global::Char.myCharz().holder;
								if (holder)
								{
									global::Char.myCharz().removeHoleEff();
								}
								global::Char.myCharz().currentMovePoint = new MovePoint(this.clickToX, this.clickToY);
								global::Char.myCharz().cdir = ((global::Char.myCharz().cx - global::Char.myCharz().currentMovePoint.xEnd <= 0) ? 1 : (-1));
								global::Char.myCharz().endMovePointCommand = null;
								GameScr.isAutoPlay = false;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0005A308 File Offset: 0x00058508
	private void checkAuto()
	{
		long num = mSystem.currentTimeMillis();
		bool flag = GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] || GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] || GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] || GameCanvas.keyPressed[1] || GameCanvas.keyPressed[3];
		if (flag)
		{
			this.auto = 0;
			GameScr.isAutoPlay = false;
		}
		bool flag2 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] && !this.isPaintPopup();
		if (flag2)
		{
			bool flag3 = this.auto == 0;
			if (flag3)
			{
				bool flag4 = num - this.lastFire < 800L && this.checkSkillValid2() && (global::Char.myCharz().mobFocus != null || (global::Char.myCharz().charFocus != null && global::Char.myCharz().isMeCanAttackOtherPlayer(global::Char.myCharz().charFocus)));
				if (flag4)
				{
					Res.outz("toi day");
					this.auto = 10;
					GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
				}
			}
			else
			{
				this.auto = 0;
				GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = (GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = false);
			}
			this.lastFire = num;
		}
		bool flag5 = GameCanvas.gameTick % 5 == 0 && this.auto > 0 && global::Char.myCharz().currentMovePoint == null;
		if (flag5)
		{
			bool flag6 = global::Char.myCharz().myskill != null && (global::Char.myCharz().myskill.template.isUseAlone() || global::Char.myCharz().myskill.paintCanNotUseSkill);
			if (flag6)
			{
				return;
			}
			bool flag7 = (global::Char.myCharz().mobFocus != null && global::Char.myCharz().mobFocus.status != 1 && global::Char.myCharz().mobFocus.status != 0 && global::Char.myCharz().charFocus == null) || (global::Char.myCharz().charFocus != null && global::Char.myCharz().isMeCanAttackOtherPlayer(global::Char.myCharz().charFocus));
			if (flag7)
			{
				bool paintCanNotUseSkill = global::Char.myCharz().myskill.paintCanNotUseSkill;
				if (paintCanNotUseSkill)
				{
					return;
				}
				this.doFire(false, true);
			}
		}
		bool flag8 = this.auto > 1;
		if (flag8)
		{
			this.auto--;
		}
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0005A588 File Offset: 0x00058788
	public void doUseHP()
	{
		bool flag = global::Char.myCharz().stone || global::Char.myCharz().blindEff || global::Char.myCharz().holdEffID > 0;
		if (!flag)
		{
			long num = mSystem.currentTimeMillis();
			bool flag2 = num - this.lastUsePotion >= 10000L;
			if (flag2)
			{
				bool flag3 = !global::Char.myCharz().doUsePotion();
				if (flag3)
				{
					GameScr.info1.addInfo(mResources.HP_EMPTY, 0);
				}
				else
				{
					ServerEffect.addServerEffect(11, global::Char.myCharz(), 5);
					ServerEffect.addServerEffect(104, global::Char.myCharz(), 4);
					this.lastUsePotion = num;
					SoundMn.gI().eatPeans();
				}
			}
		}
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0005A638 File Offset: 0x00058838
	public void activeSuperPower(int x, int y)
	{
		bool flag = !this.isSuperPower;
		if (flag)
		{
			SoundMn.gI().bigeExlode();
			this.isSuperPower = true;
			this.tPower = 0;
			this.dxPower = 0;
			this.xPower = x - GameScr.cmx;
			this.yPower = y - GameScr.cmy;
		}
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0005A690 File Offset: 0x00058890
	public void activeRongThanEff(bool isMe)
	{
		this.activeRongThan = true;
		this.isUseFreez = true;
		this.isMeCallRongThan = true;
		if (isMe)
		{
			Effect effect = new Effect(20, global::Char.myCharz().cx, global::Char.myCharz().cy - 77, 2, 8, 1);
			EffecMn.addEff(effect);
		}
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x00004DCE File Offset: 0x00002FCE
	public void hideRongThanEff()
	{
		this.activeRongThan = false;
		this.isUseFreez = true;
		this.isMeCallRongThan = false;
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x00004DE6 File Offset: 0x00002FE6
	public void doiMauTroi()
	{
		this.isRongThanXuatHien = true;
		this.mautroi = mGraphics.blendColor(0.4f, 0, GameCanvas.colorTop[GameCanvas.colorTop.Length - 1]);
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0005A6E4 File Offset: 0x000588E4
	public void callRongThan(int x, int y)
	{
		Res.outz("VE RONG THAN O VI TRI x= " + x.ToString() + " y=" + y.ToString());
		this.doiMauTroi();
		Effect effect = new Effect((!this.isRongNamek) ? 17 : 25, x, y - 77, 2, -1, 1);
		EffecMn.addEff(effect);
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0005A740 File Offset: 0x00058940
	public void hideRongThan()
	{
		this.isRongThanXuatHien = false;
		EffecMn.removeEff(17);
		bool flag = this.isRongNamek;
		if (flag)
		{
			this.isRongNamek = false;
			EffecMn.removeEff(25);
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0005A778 File Offset: 0x00058978
	private void autoPlay()
	{
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0005A788 File Offset: 0x00058988
	private void doFire(bool isFireByShortCut, bool skipWaypoint)
	{
		GameScr.tam++;
		Waypoint waypoint = global::Char.myCharz().isInEnterOfflinePoint();
		Waypoint waypoint2 = global::Char.myCharz().isInEnterOnlinePoint();
		bool flag = !skipWaypoint && waypoint != null && (global::Char.myCharz().mobFocus == null || (global::Char.myCharz().mobFocus != null && global::Char.myCharz().mobFocus.templateId == 0));
		if (flag)
		{
			waypoint.popup.command.performAction();
		}
		else
		{
			bool flag2 = !skipWaypoint && waypoint2 != null && (global::Char.myCharz().mobFocus == null || (global::Char.myCharz().mobFocus != null && global::Char.myCharz().mobFocus.templateId == 0));
			if (flag2)
			{
				waypoint2.popup.command.performAction();
			}
			else
			{
				bool flag3 = (TileMap.mapID == 51 && global::Char.myCharz().npcFocus != null) || global::Char.myCharz().statusMe == 14;
				if (!flag3)
				{
					global::Char.myCharz().cvx = (global::Char.myCharz().cvy = 0);
					bool flag4 = global::Char.myCharz().isSelectingSkillUseAlone() && global::Char.myCharz().focusToAttack();
					if (flag4)
					{
						bool flag5 = this.checkSkillValid();
						if (flag5)
						{
							global::Char.myCharz().currentFireByShortcut = isFireByShortCut;
							global::Char.myCharz().useSkillNotFocus();
						}
					}
					else
					{
						bool flag6 = this.isAttack();
						if (flag6)
						{
							bool flag7 = global::Char.myCharz().isUseChargeSkill() && global::Char.myCharz().focusToAttack();
							if (flag7)
							{
								bool flag8 = this.checkSkillValid();
								if (flag8)
								{
									global::Char.myCharz().currentFireByShortcut = isFireByShortCut;
									global::Char.myCharz().sendUseChargeSkill();
								}
								else
								{
									global::Char.myCharz().stopUseChargeSkill();
								}
							}
							else
							{
								bool flag9 = TileMap.tileTypeAt(global::Char.myCharz().cx, global::Char.myCharz().cy, 2);
								global::Char.myCharz().setSkillPaint(GameScr.sks[(int)global::Char.myCharz().myskill.skillId], (!flag9) ? 1 : 0);
								bool flag10 = flag9;
								if (flag10)
								{
									global::Char.myCharz().delayFall = 20;
								}
								global::Char.myCharz().currentFireByShortcut = isFireByShortCut;
							}
						}
					}
					bool flag11 = global::Char.myCharz().isSelectingSkillBuffToPlayer();
					if (flag11)
					{
						this.auto = 0;
					}
				}
			}
		}
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0005A9E4 File Offset: 0x00058BE4
	private void askToPick()
	{
		Npc npc = new Npc(5, 0, -100, 100, 5, GameScr.info1.charId[global::Char.myCharz().cgender][2]);
		string nhatvatpham = mResources.nhatvatpham;
		string[] array = new string[]
		{
			mResources.YES,
			mResources.NO
		};
		npc.idItem = 673;
		GameScr.gI().createMenu(array, npc);
		ChatPopup.addChatPopupWithIcon(nhatvatpham, 100000, npc, 5820);
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0005AA60 File Offset: 0x00058C60
	private void pickItem()
	{
		bool flag = global::Char.myCharz().itemFocus == null;
		if (!flag)
		{
			bool flag2 = global::Char.myCharz().cx < global::Char.myCharz().itemFocus.x;
			if (flag2)
			{
				global::Char.myCharz().cdir = 1;
			}
			else
			{
				global::Char.myCharz().cdir = -1;
			}
			int num = global::Math.abs(global::Char.myCharz().cx - global::Char.myCharz().itemFocus.x);
			int num2 = global::Math.abs(global::Char.myCharz().cy - global::Char.myCharz().itemFocus.y);
			bool flag3 = num <= 40 && num2 < 40;
			if (flag3)
			{
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				bool flag4 = global::Char.myCharz().itemFocus.template.id != 673;
				if (flag4)
				{
					Service.gI().pickItem(global::Char.myCharz().itemFocus.itemMapID);
				}
				else
				{
					this.askToPick();
				}
			}
			else
			{
				global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().itemFocus.x, global::Char.myCharz().itemFocus.y);
				global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
			}
		}
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0005ABC0 File Offset: 0x00058DC0
	public bool isCharging()
	{
		return global::Char.myCharz().isFlyAndCharge || global::Char.myCharz().isUseSkillAfterCharge || global::Char.myCharz().isStandAndCharge || global::Char.myCharz().isWaitMonkey || this.isSuperPower || global::Char.myCharz().isFreez;
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0005AC24 File Offset: 0x00058E24
	public void doSelectSkill(Skill skill, bool isShortcut)
	{
		bool flag = global::Char.myCharz().isCreateDark || this.isCharging() || global::Char.myCharz().taskMaint.taskId <= 1;
		if (!flag)
		{
			global::Char.myCharz().myskill = skill;
			bool flag2 = this.lastSkill != skill && this.lastSkill != null;
			if (flag2)
			{
				Service.gI().selectSkill((int)skill.template.id);
				this.saveRMSCurrentSkill(skill.template.id);
				this.resetButton();
				this.lastSkill = skill;
				this.selectedIndexSkill = -1;
				GameScr.gI().auto = 0;
			}
			else
			{
				bool flag3 = global::Char.myCharz().isUseSkillSpec();
				if (flag3)
				{
					Res.outz(">>>use skill spec: " + skill.template.id.ToString());
					global::Char.myCharz().sendNewAttack((short)skill.template.id);
					this.saveRMSCurrentSkill(skill.template.id);
					this.resetButton();
					this.lastSkill = skill;
					this.selectedIndexSkill = -1;
					GameScr.gI().auto = 0;
				}
				else
				{
					bool flag4 = global::Char.myCharz().isSelectingSkillUseAlone();
					if (flag4)
					{
						Res.outz("use skill not focus");
						this.doUseSkillNotFocus(skill);
						this.lastSkill = skill;
					}
					else
					{
						this.selectedIndexSkill = -1;
						bool flag5 = skill == null;
						if (!flag5)
						{
							Res.outz("only select skill");
							bool flag6 = this.lastSkill != skill;
							if (flag6)
							{
								Service.gI().selectSkill((int)skill.template.id);
								this.saveRMSCurrentSkill(skill.template.id);
								this.resetButton();
							}
							bool flag7 = global::Char.myCharz().charFocus != null || !global::Char.myCharz().isSelectingSkillBuffToPlayer();
							if (flag7)
							{
								bool flag8 = global::Char.myCharz().focusToAttack();
								if (flag8)
								{
									this.doFire(isShortcut, true);
									this.doSeleckSkillFlag = true;
								}
								this.lastSkill = skill;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0005AE38 File Offset: 0x00059038
	public void doUseSkill(Skill skill, bool isShortcut)
	{
		bool flag = (TileMap.mapID == 112 || TileMap.mapID == 113) && global::Char.myCharz().cTypePk == 0;
		if (!flag)
		{
			bool flag2 = global::Char.myCharz().isSelectingSkillUseAlone();
			if (flag2)
			{
				Res.outz("HERE");
				this.doUseSkillNotFocus(skill);
			}
			else
			{
				this.selectedIndexSkill = -1;
				bool flag3 = skill != null;
				if (flag3)
				{
					Service.gI().selectSkill((int)skill.template.id);
					this.saveRMSCurrentSkill(skill.template.id);
					this.resetButton();
					global::Char.myCharz().myskill = skill;
					this.doFire(isShortcut, true);
				}
			}
		}
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0005AEE8 File Offset: 0x000590E8
	public void doUseSkillNotFocus(Skill skill)
	{
		bool flag = ((TileMap.mapID != 112 && TileMap.mapID != 113) || global::Char.myCharz().cTypePk != 0) && this.checkSkillValid();
		if (flag)
		{
			this.selectedIndexSkill = -1;
			bool flag2 = skill != null;
			if (flag2)
			{
				Service.gI().selectSkill((int)skill.template.id);
				this.saveRMSCurrentSkill(skill.template.id);
				this.resetButton();
				global::Char.myCharz().myskill = skill;
				global::Char.myCharz().useSkillNotFocus();
				global::Char.myCharz().currentFireByShortcut = true;
				this.auto = 0;
			}
		}
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0005AF8C File Offset: 0x0005918C
	public void sortSkill()
	{
		for (int i = 0; i < global::Char.myCharz().vSkillFight.size() - 1; i++)
		{
			Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(i);
			for (int j = i + 1; j < global::Char.myCharz().vSkillFight.size(); j++)
			{
				Skill skill2 = (Skill)global::Char.myCharz().vSkillFight.elementAt(j);
				bool flag = skill2.template.id < skill.template.id;
				if (flag)
				{
					Skill skill3 = skill2;
					skill2 = skill;
					skill = skill3;
					global::Char.myCharz().vSkillFight.setElementAt(skill, i);
					global::Char.myCharz().vSkillFight.setElementAt(skill2, j);
				}
			}
		}
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x0005B060 File Offset: 0x00059260
	public void updateKeyTouchCapcha()
	{
		bool flag = this.isNotPaintTouchControl();
		if (!flag)
		{
			for (int i = 0; i < this.strCapcha.Length; i++)
			{
				this.keyCapcha[i] = -1;
				bool flag2 = !GameCanvas.isTouchControl;
				if (!flag2)
				{
					int num = (GameCanvas.w - this.strCapcha.Length * GameScr.disXC) / 2;
					int num2 = this.strCapcha.Length * GameScr.disXC;
					int num3 = GameCanvas.h - 40;
					int num4 = GameScr.disXC;
					bool flag3 = !GameCanvas.isPointerHoldIn(num, num3, num2, num4);
					if (!flag3)
					{
						int num5 = (GameCanvas.px - num) / GameScr.disXC;
						bool flag4 = i == num5;
						if (flag4)
						{
							this.keyCapcha[i] = 1;
						}
						bool flag5 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease && i == num5;
						if (flag5)
						{
							char[] array = this.keyInput.ToCharArray();
							MyVector myVector = new MyVector();
							for (int j = 0; j < array.Length; j++)
							{
								myVector.addElement(array[j].ToString() + string.Empty);
							}
							myVector.removeElementAt(0);
							myVector.insertElementAt(this.strCapcha[i].ToString() + string.Empty, myVector.size());
							this.keyInput = string.Empty;
							for (int k = 0; k < myVector.size(); k++)
							{
								this.keyInput += ((string)myVector.elementAt(k)).ToUpper();
							}
							Service.gI().mobCapcha(this.strCapcha[i]);
						}
					}
				}
			}
		}
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x0005B248 File Offset: 0x00059448
	public bool checkClickToCapcha()
	{
		bool flag = this.mobCapcha == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			int num = (GameCanvas.w - 5 * GameScr.disXC) / 2;
			int num2 = 5 * GameScr.disXC;
			int num3 = GameCanvas.h - 40;
			int num4 = GameScr.disXC;
			bool flag3 = GameCanvas.isPointerHoldIn(num, num3, num2, num4);
			flag2 = flag3;
		}
		return flag2;
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0005B2B0 File Offset: 0x000594B0
	public void checkMouseChat()
	{
		bool flag = GameCanvas.isMouseFocus(GameScr.xC, GameScr.yC, 34, 34);
		if (flag)
		{
			bool flag2 = !TileMap.isOfflineMap();
			if (flag2)
			{
				mScreen.keyMouse = 15;
			}
		}
		else
		{
			bool flag3 = GameCanvas.isMouseFocus(GameScr.xHP, GameScr.yHP, 40, 40);
			if (flag3)
			{
				bool flag4 = global::Char.myCharz().statusMe != 14;
				if (flag4)
				{
					mScreen.keyMouse = 10;
				}
			}
			else
			{
				bool flag5 = GameCanvas.isMouseFocus(GameScr.xF, GameScr.yF, 40, 40);
				if (flag5)
				{
					bool flag6 = global::Char.myCharz().statusMe != 14;
					if (flag6)
					{
						mScreen.keyMouse = 5;
					}
				}
				else
				{
					bool flag7 = this.cmdMenu != null && GameCanvas.isMouseFocus(this.cmdMenu.x, this.cmdMenu.y, this.cmdMenu.w / 2, this.cmdMenu.h);
					if (flag7)
					{
						mScreen.keyMouse = 1;
					}
					else
					{
						mScreen.keyMouse = -1;
					}
				}
			}
		}
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0005B3C0 File Offset: 0x000595C0
	private void updateKeyTouchControl()
	{
		bool flag = this.isNotPaintTouchControl();
		if (!flag)
		{
			mScreen.keyTouch = -1;
			bool isTouchControl = GameCanvas.isTouchControl;
			if (isTouchControl)
			{
				bool flag2 = GameCanvas.isPointerHoldIn(0, 0, 60, 50) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
				if (flag2)
				{
					bool flag3 = global::Char.myCharz().cmdMenu != null;
					if (flag3)
					{
						global::Char.myCharz().cmdMenu.performAction();
					}
					global::Char.myCharz().currentMovePoint = null;
					GameCanvas.clearAllPointerEvent();
					this.flareFindFocus = true;
					this.flareTime = 5;
					return;
				}
				bool isPC = Main.isPC;
				if (isPC)
				{
					this.checkMouseChat();
				}
				bool flag4 = !TileMap.isOfflineMap() && GameCanvas.isPointerHoldIn(GameScr.xC, GameScr.yC, 34, 34);
				if (flag4)
				{
					mScreen.keyTouch = 15;
					GameCanvas.isPointerJustDown = false;
					this.isPointerDowning = false;
					bool flag5 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
					if (flag5)
					{
						ChatTextField.gI().startChat(this, string.Empty);
						SoundMn.gI().buttonClick();
						global::Char.myCharz().currentMovePoint = null;
						GameCanvas.clearAllPointerEvent();
						return;
					}
				}
				bool flag6 = global::Char.myCharz().cmdMenu != null && GameCanvas.isPointerHoldIn(global::Char.myCharz().cmdMenu.x - 17, global::Char.myCharz().cmdMenu.y - 17, 34, 34);
				if (flag6)
				{
					mScreen.keyTouch = 20;
					GameCanvas.isPointerJustDown = false;
					this.isPointerDowning = false;
					bool flag7 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
					if (flag7)
					{
						GameCanvas.clearAllPointerEvent();
						global::Char.myCharz().cmdMenu.performAction();
						return;
					}
				}
				this.updateGamePad();
				bool flag8 = ((GameScr.isAnalog != 0) ? GameCanvas.isPointerHoldIn(GameScr.xHP, GameScr.yHP + 10, 34, 34) : GameCanvas.isPointerHoldIn(GameScr.xHP, GameScr.yHP + 10, 40, 40)) && global::Char.myCharz().statusMe != 14 && this.mobCapcha == null;
				if (flag8)
				{
					mScreen.keyTouch = 10;
					GameCanvas.isPointerJustDown = false;
					this.isPointerDowning = false;
					bool flag9 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
					if (flag9)
					{
						GameCanvas.keyPressed[10] = true;
						GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
					}
				}
				bool flag10 = ((GameScr.isAnalog != 0) ? GameCanvas.isPointerHoldIn(GameScr.xHP + 5, GameScr.yHP - 6 - 34 + 10, 34, 34) : GameCanvas.isPointerHoldIn(GameScr.xHP + 5, GameScr.yHP - 6 - 40 + 10, 40, 40)) && global::Char.myCharz().statusMe != 14 && this.mobCapcha == null;
				if (flag10)
				{
					bool flag11 = GameScr.isPickNgocRong;
					if (flag11)
					{
						mScreen.keyTouch = 14;
						GameCanvas.isPointerJustDown = false;
						this.isPointerDowning = false;
						bool flag12 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
						if (flag12)
						{
							GameCanvas.keyPressed[14] = true;
							GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
							GameScr.isPickNgocRong = false;
							Service.gI().useItem(-1, -1, -1, -1);
						}
					}
					else
					{
						bool flag13 = GameScr.isudungCapsun4;
						if (flag13)
						{
							mScreen.keyTouch = 14;
							GameCanvas.isPointerJustDown = false;
							this.isPointerDowning = false;
							bool flag14 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
							if (flag14)
							{
								GameCanvas.keyPressed[14] = true;
								GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
								for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
								{
									Item item = global::Char.myCharz().arrItemBag[i];
									bool flag15 = item == null;
									if (!flag15)
									{
										Res.err("find " + item.template.id.ToString());
										bool flag16 = item.template.id == 194;
										if (flag16)
										{
											GameScr.isudungCapsun4 = item.quantity > 0;
											bool flag17 = GameScr.isudungCapsun4;
											if (flag17)
											{
												Service.gI().useItem(0, 1, (sbyte)i, -1);
												break;
											}
										}
									}
								}
							}
						}
						else
						{
							bool flag18 = GameScr.isudungCapsun3;
							if (flag18)
							{
								mScreen.keyTouch = 14;
								GameCanvas.isPointerJustDown = false;
								this.isPointerDowning = false;
								bool flag19 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
								if (flag19)
								{
									GameCanvas.keyPressed[14] = true;
									GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
									for (int j = 0; j < global::Char.myCharz().arrItemBag.Length; j++)
									{
										Item item2 = global::Char.myCharz().arrItemBag[j];
										bool flag20 = item2 != null && item2.template.id == 193;
										if (flag20)
										{
											GameScr.isudungCapsun3 = item2.quantity > 0;
											bool flag21 = GameScr.isudungCapsun3;
											if (flag21)
											{
												Service.gI().useItem(0, 1, (sbyte)j, -1);
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool flag22 = this.mobCapcha != null;
			if (flag22)
			{
				this.updateKeyTouchCapcha();
			}
			else
			{
				bool flag23 = GameScr.isHaveSelectSkill;
				if (flag23)
				{
					bool flag24 = this.isCharging();
					if (flag24)
					{
						return;
					}
					this.keyTouchSkill = -1;
					bool flag25 = false;
					bool flag26 = GameScr.onScreenSkill.Length > 5 && (GameCanvas.isPointerHoldIn(GameScr.xSkill + GameScr.xS[0] - GameScr.wSkill / 2 + 12, GameScr.yS[0] - GameScr.wSkill / 2 + 12, 5 * GameScr.wSkill, GameScr.wSkill) || GameCanvas.isPointerHoldIn(GameScr.xSkill + GameScr.xS[5] - GameScr.wSkill / 2 + 12, GameScr.yS[5] - GameScr.wSkill / 2 + 12, 5 * GameScr.wSkill, GameScr.wSkill));
					if (flag26)
					{
						flag25 = true;
					}
					bool flag27 = flag25 || GameCanvas.isPointerHoldIn(GameScr.xSkill + GameScr.xS[0] - GameScr.wSkill / 2 + 12, GameScr.yS[0] - GameScr.wSkill / 2 + 12, 5 * GameScr.wSkill, GameScr.wSkill) || (!GameCanvas.isTouchControl && GameCanvas.isPointerHoldIn(GameScr.xSkill + GameScr.xS[0] - GameScr.wSkill / 2 + 12, GameScr.yS[0] - GameScr.wSkill / 2 + 12, GameScr.wSkill, GameScr.onScreenSkill.Length * GameScr.wSkill));
					if (flag27)
					{
						GameCanvas.isPointerJustDown = false;
						this.isPointerDowning = false;
						int num = (GameCanvas.pxLast - (GameScr.xSkill + GameScr.xS[0] - GameScr.wSkill / 2 + 12)) / GameScr.wSkill;
						bool flag28 = flag25 && GameCanvas.pyLast < GameScr.yS[0];
						if (flag28)
						{
							num += 5;
						}
						this.keyTouchSkill = num;
						bool flag29 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
						if (flag29)
						{
							GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
							this.selectedIndexSkill = num;
							bool flag30 = GameScr.indexSelect < 0;
							if (flag30)
							{
								GameScr.indexSelect = 0;
							}
							bool flag31 = !Main.isPC;
							if (flag31)
							{
								bool flag32 = this.selectedIndexSkill > GameScr.onScreenSkill.Length - 1;
								if (flag32)
								{
									this.selectedIndexSkill = GameScr.onScreenSkill.Length - 1;
								}
							}
							else
							{
								bool flag33 = this.selectedIndexSkill > GameScr.keySkill.Length - 1;
								if (flag33)
								{
									this.selectedIndexSkill = GameScr.keySkill.Length - 1;
								}
							}
							Skill skill = (Main.isPC ? GameScr.keySkill[this.selectedIndexSkill] : GameScr.onScreenSkill[this.selectedIndexSkill]);
							bool flag34 = skill != null;
							if (flag34)
							{
								this.doSelectSkill(skill, true);
							}
						}
					}
				}
			}
			bool isPointerJustRelease = GameCanvas.isPointerJustRelease;
			if (isPointerJustRelease)
			{
				bool flag35 = GameCanvas.keyHold[1] || GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] || GameCanvas.keyHold[3] || GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] || GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
				if (flag35)
				{
					GameCanvas.isPointerJustRelease = false;
				}
				GameCanvas.keyHold[1] = false;
				GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = false;
				GameCanvas.keyHold[3] = false;
				GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = false;
				GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = false;
			}
		}
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x00004E10 File Offset: 0x00003010
	public void setCharJumpAtt()
	{
		global::Char.myCharz().cvy = -10;
		global::Char.myCharz().statusMe = 3;
		global::Char.myCharz().cp1 = 0;
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0005BC54 File Offset: 0x00059E54
	public void setCharJump(int cvx)
	{
		bool flag = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0;
		if (flag)
		{
			Service.gI().charMove();
		}
		global::Char.myCharz().cvy = -10;
		global::Char.myCharz().cvx = cvx;
		global::Char.myCharz().statusMe = 3;
		global::Char.myCharz().cp1 = 0;
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0005BCD4 File Offset: 0x00059ED4
	public void updateOpen()
	{
		bool flag = this.isstarOpen;
		if (flag)
		{
			bool flag2 = this.moveUp > -3;
			if (flag2)
			{
				this.moveUp -= 4;
			}
			else
			{
				this.moveUp = -2;
			}
			bool flag3 = this.moveDow < GameCanvas.h + 3;
			if (flag3)
			{
				this.moveDow += 4;
			}
			else
			{
				this.moveDow = GameCanvas.h + 2;
			}
			bool flag4 = this.moveUp <= -2 && this.moveDow >= GameCanvas.h + 2;
			if (flag4)
			{
				this.isstarOpen = false;
			}
		}
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00003E4C File Offset: 0x0000204C
	public void initCreateCommand()
	{
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00003E4C File Offset: 0x0000204C
	public void checkCharFocus()
	{
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0005BD7C File Offset: 0x00059F7C
	public void updateXoSo()
	{
		bool flag = this.tShow == 0;
		if (!flag)
		{
			GameScr.currXS = mSystem.currentTimeMillis();
			bool flag2 = GameScr.currXS - GameScr.lastXS > 1000L;
			if (flag2)
			{
				GameScr.lastXS = mSystem.currentTimeMillis();
				GameScr.secondXS++;
			}
			bool flag3 = GameScr.secondXS > 20;
			if (flag3)
			{
				for (int i = 0; i < this.winnumber.Length; i++)
				{
					this.randomNumber[i] = this.winnumber[i];
				}
				this.tShow--;
				bool flag4 = this.tShow == 0;
				if (flag4)
				{
					this.yourNumber = string.Empty;
					GameScr.info1.addInfo(this.strFinish, 0);
					GameScr.secondXS = 0;
				}
			}
			else
			{
				bool flag5 = this.moveIndex > this.winnumber.Length - 1;
				if (flag5)
				{
					this.tShow--;
					bool flag6 = this.tShow == 0;
					if (flag6)
					{
						this.yourNumber = string.Empty;
						GameScr.info1.addInfo(this.strFinish, 0);
					}
				}
				else
				{
					bool flag7 = this.moveIndex < this.randomNumber.Length;
					if (flag7)
					{
						bool flag8 = this.tMove[this.moveIndex] == 15;
						if (flag8)
						{
							bool flag9 = this.randomNumber[this.moveIndex] == this.winnumber[this.moveIndex] - 1;
							if (flag9)
							{
								this.delayMove[this.moveIndex] = 10;
							}
							bool flag10 = this.randomNumber[this.moveIndex] == this.winnumber[this.moveIndex];
							if (flag10)
							{
								this.tMove[this.moveIndex] = -1;
								this.moveIndex++;
							}
						}
						else
						{
							bool flag11 = GameCanvas.gameTick % 5 == 0;
							if (flag11)
							{
								this.tMove[this.moveIndex]++;
							}
						}
					}
					for (int j = 0; j < this.winnumber.Length; j++)
					{
						bool flag12 = this.tMove[j] == -1;
						if (!flag12)
						{
							this.moveCount[j]++;
							bool flag13 = this.moveCount[j] > this.tMove[j] + this.delayMove[j];
							if (flag13)
							{
								this.moveCount[j] = 0;
								this.randomNumber[j]++;
								bool flag14 = this.randomNumber[j] >= 10;
								if (flag14)
								{
									this.randomNumber[j] = 0;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x00004E35 File Offset: 0x00003035
	public void MyDoDoubleClickToObj(IMapObject obj)
	{
		this.doDoubleClickToObj(obj);
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0005C034 File Offset: 0x0005A234
	public static int getX(sbyte type)
	{
		int i = 0;
		while (i < TileMap.vGo.size())
		{
			Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
			bool flag = waypoint.maxX < 60 && type == 0;
			int num;
			if (flag)
			{
				num = 15;
			}
			else
			{
				bool flag2 = (int)waypoint.minX <= TileMap.pxw - 60 && waypoint.maxX >= 60 && type == 1;
				if (flag2)
				{
					num = (int)((waypoint.minX + waypoint.maxX) / 2);
				}
				else
				{
					bool flag3 = (int)waypoint.minX > TileMap.pxw - 60 && type == 2;
					if (!flag3)
					{
						i++;
						continue;
					}
					num = TileMap.pxw - 15;
				}
			}
			return num;
		}
		return 0;
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0005C0FC File Offset: 0x0005A2FC
	public static int getY(sbyte type)
	{
		int i = 0;
		while (i < TileMap.vGo.size())
		{
			Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
			bool flag = waypoint.maxX < 60 && type == 0;
			int num;
			if (flag)
			{
				num = (int)waypoint.maxY;
			}
			else
			{
				bool flag2 = (int)waypoint.minX <= TileMap.pxw - 60 && waypoint.maxX >= 60 && type == 1;
				if (flag2)
				{
					num = (int)waypoint.maxY;
				}
				else
				{
					bool flag3 = (int)waypoint.minX > TileMap.pxw - 60 && type == 2;
					if (!flag3)
					{
						i++;
						continue;
					}
					num = (int)waypoint.maxY;
				}
			}
			return num;
		}
		return 0;
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0005C1BC File Offset: 0x0005A3BC
	private static void MoveMyChar(int x, int y)
	{
		global::Char.myCharz().cx = x;
		global::Char.myCharz().cy = y;
		Service.gI().charMove();
		bool flag = ItemTime.isExistItem(4387);
		if (!flag)
		{
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y + 1;
			Service.gI().charMove();
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y;
			Service.gI().charMove();
		}
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0005C240 File Offset: 0x0005A440
	public override void update()
	{
		bool flag = !Pk9rXmap.IsXmapRunning;
		if (flag)
		{
			Status.Update();
			AutoBroly.Update();
			ChucNangPhu.Update();
			ChucNangPhu2.Update();
			ChucNangPhu3.Update();
			ChucNangPhu4.Update();
		}
		bool flag2 = GameCanvas.gameTick % 20 == 0;
		if (flag2)
		{
			SocketInPut.Start();
		}
		bool flag3 = DataAccount.Type > 1;
		if (flag3)
		{
			KsSupper.Update();
			bool flag4 = DataAccount.Type == 3;
			if (flag4)
			{
				DovaBaoKhu.Update();
			}
		}
		bool flag5 = GameCanvas.keyPressed[16];
		if (flag5)
		{
			GameCanvas.keyPressed[16] = false;
			global::Char.myCharz().findNextFocusByKey();
		}
		bool flag6 = GameCanvas.keyPressed[13] && !GameCanvas.panel.isShow;
		if (flag6)
		{
			GameCanvas.keyPressed[13] = false;
			global::Char.myCharz().findNextFocusByKey();
		}
		bool flag7 = GameCanvas.keyPressed[17];
		if (flag7)
		{
			GameCanvas.keyPressed[17] = false;
			global::Char.myCharz().searchItem();
			bool flag8 = global::Char.myCharz().itemFocus != null;
			if (flag8)
			{
				this.pickItem();
			}
		}
		bool flag9 = GameCanvas.gameTick % 100 == 0 && TileMap.mapID == 137;
		if (flag9)
		{
			GameScr.shock_scr = 30;
		}
		bool flag10 = GameScr.isAutoPlay && GameCanvas.gameTick % 20 == 0;
		if (flag10)
		{
			this.autoPlay();
		}
		this.updateXoSo();
		mSystem.checkAdComlete();
		SmallImage.update();
		try
		{
			bool isContinueToLogin = LoginScr.isContinueToLogin;
			if (isContinueToLogin)
			{
				LoginScr.isContinueToLogin = false;
			}
			bool flag11 = GameScr.tickMove == 1;
			if (flag11)
			{
				GameScr.lastTick = mSystem.currentTimeMillis();
			}
			bool flag12 = GameScr.tickMove == 100;
			if (flag12)
			{
				GameScr.tickMove = 0;
				GameScr.currTick = mSystem.currentTimeMillis();
				int num = (int)(GameScr.currTick - GameScr.lastTick) / 1000;
				Service.gI().checkMMove(num);
			}
			bool flag13 = GameScr.lockTick > 0;
			if (flag13)
			{
				GameScr.lockTick--;
				bool flag14 = GameScr.lockTick == 0;
				if (flag14)
				{
					Controller.isStopReadMessage = false;
				}
			}
			this.checkCharFocus();
			GameCanvas.debug("E1", 0);
			GameScr.updateCamera();
			GameCanvas.debug("E2", 0);
			ChatTextField.gI().update();
			GameCanvas.debug("E3", 0);
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				((global::Char)GameScr.vCharInMap.elementAt(i)).update();
			}
			for (int j = 0; j < Teleport.vTeleport.size(); j++)
			{
				((Teleport)Teleport.vTeleport.elementAt(j)).update();
			}
			global::Char.myCharz().update();
			bool flag15 = global::Char.myCharz().statusMe == 1;
			if (flag15)
			{
			}
			bool flag16 = this.popUpYesNo != null;
			if (flag16)
			{
				this.popUpYesNo.update();
			}
			EffecMn.update();
			GameCanvas.debug("E5x", 0);
			for (int k = 0; k < GameScr.vMob.size(); k++)
			{
				((Mob)GameScr.vMob.elementAt(k)).update();
			}
			GameCanvas.debug("E6", 0);
			for (int l = 0; l < GameScr.vNpc.size(); l++)
			{
				((Npc)GameScr.vNpc.elementAt(l)).update();
			}
			this.nSkill = GameScr.onScreenSkill.Length;
			for (int m = GameScr.onScreenSkill.Length - 1; m >= 0; m--)
			{
				Skill skill = GameScr.onScreenSkill[m];
				bool flag17 = skill != null;
				if (flag17)
				{
					this.nSkill = m + 1;
					break;
				}
				this.nSkill--;
			}
			GameScr.setSkillBarPosition();
			GameCanvas.debug("E7", 0);
			GameCanvas.gI().updateDust();
			GameCanvas.debug("E8", 0);
			GameScr.updateFlyText();
			PopUp.updateAll();
			GameScr.updateSplash();
			this.updateSS();
			GameCanvas.updateBG();
			GameCanvas.debug("E9", 0);
			this.updateClickToArrow();
			GameCanvas.debug("E10", 0);
			for (int n = 0; n < GameScr.vItemMap.size(); n++)
			{
				((ItemMap)GameScr.vItemMap.elementAt(n)).update();
			}
			GameCanvas.debug("E11", 0);
			GameCanvas.debug("E13", 0);
			for (int num2 = Effect2.vRemoveEffect2.size() - 1; num2 >= 0; num2--)
			{
				Effect2.vEffect2.removeElement(Effect2.vRemoveEffect2.elementAt(num2));
				Effect2.vRemoveEffect2.removeElementAt(num2);
			}
			for (int num3 = 0; num3 < Effect2.vEffect2.size(); num3++)
			{
				Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(num3);
				effect.update();
			}
			for (int num4 = 0; num4 < Effect2.vEffect2Outside.size(); num4++)
			{
				Effect2 effect2 = (Effect2)Effect2.vEffect2Outside.elementAt(num4);
				effect2.update();
			}
			for (int num5 = 0; num5 < Effect2.vAnimateEffect.size(); num5++)
			{
				Effect2 effect3 = (Effect2)Effect2.vAnimateEffect.elementAt(num5);
				effect3.update();
			}
			for (int num6 = 0; num6 < Effect2.vEffectFeet.size(); num6++)
			{
				Effect2 effect4 = (Effect2)Effect2.vEffectFeet.elementAt(num6);
				effect4.update();
			}
			for (int num7 = 0; num7 < Effect2.vEffect3.size(); num7++)
			{
				Effect2 effect5 = (Effect2)Effect2.vEffect3.elementAt(num7);
				effect5.update();
			}
			BackgroudEffect.updateEff();
			GameScr.info1.update();
			GameScr.info2.update();
			GameCanvas.debug("E15", 0);
			bool flag18 = GameScr.currentCharViewInfo != null && !GameScr.currentCharViewInfo.Equals(global::Char.myCharz());
			if (flag18)
			{
				GameScr.currentCharViewInfo.update();
			}
			this.runArrow++;
			bool flag19 = this.runArrow > 3;
			if (flag19)
			{
				this.runArrow = 0;
			}
			bool flag20 = this.isInjureHp;
			if (flag20)
			{
				this.twHp += 1L;
				bool flag21 = this.twHp == 20L;
				if (flag21)
				{
					this.twHp = 0L;
					this.isInjureHp = false;
				}
			}
			else
			{
				bool flag22 = this.dHP > global::Char.myCharz().cHP;
				if (flag22)
				{
					long num8 = this.dHP - global::Char.myCharz().cHP >> 1;
					bool flag23 = num8 < 1L;
					if (flag23)
					{
						num8 = 1L;
					}
					this.dHP -= num8;
				}
				else
				{
					this.dHP = global::Char.myCharz().cHP;
				}
			}
			bool flag24 = this.isInjureMp;
			if (flag24)
			{
				this.twMp += 1L;
				bool flag25 = this.twMp == 20L;
				if (flag25)
				{
					this.twMp = 0L;
					this.isInjureMp = false;
				}
			}
			else
			{
				bool flag26 = this.dMP > global::Char.myCharz().cMP;
				if (flag26)
				{
					long num9 = this.dMP - global::Char.myCharz().cMP >> 1;
					bool flag27 = num9 < 1L;
					if (flag27)
					{
						num9 = 1L;
					}
					this.dMP -= num9;
				}
				else
				{
					this.dMP = global::Char.myCharz().cMP;
				}
			}
			bool flag28 = this.tMenuDelay > 0;
			if (flag28)
			{
				this.tMenuDelay--;
			}
			bool flag29 = this.isRongThanMenu();
			if (flag29)
			{
				int num10 = 100;
				while (this.yR - num10 < GameScr.cmy)
				{
					GameScr.cmy--;
				}
			}
			for (int num11 = 0; num11 < global::Char.vItemTime.size(); num11++)
			{
				((ItemTime)global::Char.vItemTime.elementAt(num11)).update();
			}
			for (int num12 = 0; num12 < GameScr.textTime.size(); num12++)
			{
				((ItemTime)GameScr.textTime.elementAt(num12)).update();
			}
			this.updateChatVip();
		}
		catch (Exception)
		{
		}
		int num13 = GameCanvas.gameTick % 4000;
		bool flag30 = num13 == 1000;
		if (flag30)
		{
			GameScr.checkRemoveImage();
		}
		EffectManager.update();
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00003E4C File Offset: 0x0000204C
	public void updateKeyChatPopUp()
	{
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0005CB44 File Offset: 0x0005AD44
	public bool isRongThanMenu()
	{
		return this.isMeCallRongThan;
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x0005CB68 File Offset: 0x0005AD68
	public void paintEffect(mGraphics g)
	{
		for (int i = 0; i < Effect2.vEffect2.size(); i++)
		{
			Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
			bool flag = effect != null && !(effect is ChatPopup);
			if (flag)
			{
				effect.paint(g);
			}
		}
		bool flag2 = !GameCanvas.lowGraphic;
		if (flag2)
		{
			for (int j = 0; j < Effect2.vAnimateEffect.size(); j++)
			{
				Effect2 effect2 = (Effect2)Effect2.vAnimateEffect.elementAt(j);
				effect2.paint(g);
			}
		}
		for (int k = 0; k < Effect2.vEffect2Outside.size(); k++)
		{
			Effect2 effect3 = (Effect2)Effect2.vEffect2Outside.elementAt(k);
			effect3.paint(g);
		}
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0005CC4C File Offset: 0x0005AE4C
	public void paintBgItem(mGraphics g, int layer)
	{
		for (int i = 0; i < TileMap.vCurrItem.size(); i++)
		{
			BgItem bgItem = (BgItem)TileMap.vCurrItem.elementAt(i);
			bool flag = bgItem.idImage != -1 && (int)bgItem.layer == layer;
			if (flag)
			{
				bgItem.paint(g);
			}
		}
		bool flag2 = TileMap.mapID == 48 && layer == 3 && GameCanvas.bgW != null && GameCanvas.bgW[0] != 0;
		if (flag2)
		{
			for (int j = 0; j < TileMap.pxw / GameCanvas.bgW[0] + 1; j++)
			{
				g.drawImage(GameCanvas.imgBG[0], j * GameCanvas.bgW[0], TileMap.pxh - GameCanvas.bgH[0] - 70, 0);
			}
		}
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0005CD24 File Offset: 0x0005AF24
	public void paintBlackSky(mGraphics g)
	{
		bool flag = !GameCanvas.lowGraphic;
		if (flag)
		{
			g.fillTrans(GameScr.imgTrans, 0, 0, GameCanvas.w, GameCanvas.h);
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0005CD58 File Offset: 0x0005AF58
	public void paintCapcha(mGraphics g)
	{
		MobCapcha.paint(g, global::Char.myCharz().cx, global::Char.myCharz().cy);
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		bool flag = GameCanvas.menu.showMenu || GameCanvas.panel.isShow || ChatPopup.currChatPopup != null || !GameCanvas.isTouch;
		if (!flag)
		{
			for (int i = 0; i < this.strCapcha.Length; i++)
			{
				int num = (GameCanvas.w - this.strCapcha.Length * GameScr.disXC) / 2 + i * GameScr.disXC + GameScr.disXC / 2;
				bool flag2 = this.keyCapcha[i] == -1;
				if (flag2)
				{
					g.drawImage(GameScr.imgNut, num, GameCanvas.h - 25, 3);
					mFont.tahoma_7b_dark.drawString(g, this.strCapcha[i].ToString() + string.Empty, num, GameCanvas.h - 30, 2);
				}
				else
				{
					g.drawImage(GameScr.imgNutF, num, GameCanvas.h - 25, 3);
					mFont.tahoma_7b_green2.drawString(g, this.strCapcha[i].ToString() + string.Empty, num, GameCanvas.h - 30, 2);
				}
			}
		}
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0005CEC4 File Offset: 0x0005B0C4
	public override void paint(mGraphics g)
	{
		GameScr.countEff = 0;
		bool flag = !GameScr.isPaint;
		if (!flag)
		{
			GameCanvas.debug("PA1", 1);
			bool flag2 = this.isUseFreez && ChatPopup.currChatPopup == null;
			if (flag2)
			{
				this.dem++;
				bool flag3 = (this.dem < 30 && this.dem >= 0 && GameCanvas.gameTick % 4 == 0) || (this.dem >= 30 && this.dem <= 50 && GameCanvas.gameTick % 3 == 0) || this.dem > 50;
				if (flag3)
				{
					bool flag4 = this.dem <= 50;
					if (flag4)
					{
						return;
					}
					bool flag5 = this.isUseFreez;
					if (flag5)
					{
						this.isUseFreez = false;
						this.dem = 0;
						bool flag6 = this.activeRongThan;
						if (flag6)
						{
							this.callRongThan(this.xR, this.yR);
						}
						else
						{
							this.hideRongThan();
						}
					}
					this.paintInfoBar(g);
					g.translate(-GameScr.cmx, -GameScr.cmy);
					g.translate(0, GameCanvas.transY);
					global::Char.myCharz().paint(g);
					mSystem.paintFlyText(g);
					GameScr.resetTranslate(g);
					this.paintSelectedSkill(g);
					return;
				}
			}
			GameCanvas.debug("PA2", 1);
			GameCanvas.paintBGGameScr(g);
			this.paint_ios_bg(g);
			bool flag7 = (this.isRongThanXuatHien || this.isFireWorks) && TileMap.bgID != 3;
			if (flag7)
			{
				this.paintBlackSky(g);
			}
			GameCanvas.debug("PA3", 1);
			bool flag8 = GameScr.shock_scr > 0;
			if (flag8)
			{
				g.translate(-GameScr.cmx + GameScr.shock_x[GameScr.shock_scr % GameScr.shock_x.Length], -GameScr.cmy + GameScr.shock_y[GameScr.shock_scr % GameScr.shock_y.Length]);
				GameScr.shock_scr--;
			}
			else
			{
				g.translate(-GameScr.cmx, -GameScr.cmy);
			}
			bool flag9 = this.isSuperPower;
			if (flag9)
			{
				int num = ((GameCanvas.gameTick % 3 != 0) ? (-3) : 3);
				g.translate(num, 0);
			}
			BackgroudEffect.paintBehindTileAll(g);
			EffecMn.paintLayer1(g);
			TileMap.paintTilemap(g);
			TileMap.paintOutTilemap(g);
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
				bool flag10 = @char.isMabuHold && TileMap.mapID == 128;
				if (flag10)
				{
					@char.paintHeadWithXY(g, @char.cx, @char.cy, 0);
				}
			}
			bool flag11 = global::Char.myCharz().isMabuHold && TileMap.mapID == 128;
			if (flag11)
			{
				global::Char.myCharz().paintHeadWithXY(g, global::Char.myCharz().cx, global::Char.myCharz().cy, 0);
			}
			this.paintBgItem(g, 2);
			bool flag12 = global::Char.myCharz().cmdMenu != null && GameCanvas.isTouch;
			if (flag12)
			{
				bool flag13 = mScreen.keyTouch == 20;
				if (flag13)
				{
					g.drawImage(GameScr.imgChat2, global::Char.myCharz().cmdMenu.x + GameScr.cmx, global::Char.myCharz().cmdMenu.y + GameScr.cmy, mGraphics.HCENTER | mGraphics.VCENTER);
				}
				else
				{
					g.drawImage(GameScr.imgChat, global::Char.myCharz().cmdMenu.x + GameScr.cmx, global::Char.myCharz().cmdMenu.y + GameScr.cmy, mGraphics.HCENTER | mGraphics.VCENTER);
				}
			}
			GameCanvas.debug("PA4", 1);
			GameCanvas.debug("PA5", 1);
			BackgroudEffect.paintBackAll(g);
			EffectManager.lowEffects.paintAll(g);
			for (int j = 0; j < Effect2.vEffectFeet.size(); j++)
			{
				Effect2 effect = (Effect2)Effect2.vEffectFeet.elementAt(j);
				effect.paint(g);
			}
			for (int k = 0; k < Teleport.vTeleport.size(); k++)
			{
				((Teleport)Teleport.vTeleport.elementAt(k)).paintHole(g);
			}
			for (int l = 0; l < GameScr.vNpc.size(); l++)
			{
				Npc npc = (Npc)GameScr.vNpc.elementAt(l);
				bool flag14 = npc.cHP > 0L;
				if (flag14)
				{
					npc.paintShadow(g);
				}
			}
			for (int m = 0; m < GameScr.vNpc.size(); m++)
			{
				((Npc)GameScr.vNpc.elementAt(m)).paint(g);
			}
			g.translate(0, GameCanvas.transY);
			GameCanvas.debug("PA7", 1);
			GameCanvas.debug("PA8", 1);
			for (int n = 0; n < GameScr.vCharInMap.size(); n++)
			{
				global::Char char2 = null;
				try
				{
					char2 = (global::Char)GameScr.vCharInMap.elementAt(n);
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi ham paint char gamesc: " + ex.ToString());
				}
				bool flag15 = char2 != null && (!GameCanvas.panel.isShow || !GameCanvas.panel.isTypeShop()) && char2.isShadown;
				if (flag15)
				{
					char2.paintShadow(g);
				}
			}
			global::Char.myCharz().paintShadow(g);
			EffecMn.paintLayer2(g);
			for (int num2 = 0; num2 < GameScr.vMob.size(); num2++)
			{
				((Mob)GameScr.vMob.elementAt(num2)).paint(g);
			}
			for (int num3 = 0; num3 < Teleport.vTeleport.size(); num3++)
			{
				((Teleport)Teleport.vTeleport.elementAt(num3)).paint(g);
			}
			for (int num4 = 0; num4 < GameScr.vCharInMap.size(); num4++)
			{
				global::Char char3 = null;
				try
				{
					char3 = (global::Char)GameScr.vCharInMap.elementAt(num4);
				}
				catch (Exception)
				{
				}
				bool flag16 = char3 != null && (!GameCanvas.panel.isShow || !GameCanvas.panel.isTypeShop());
				if (flag16)
				{
					char3.paint(g);
				}
			}
			global::Char.myCharz().paint(g);
			bool flag17 = global::Char.myCharz().skillPaint != null && global::Char.myCharz().skillInfoPaint() != null && global::Char.myCharz().indexSkill < global::Char.myCharz().skillInfoPaint().Length;
			if (flag17)
			{
				global::Char.myCharz().paintCharWithSkill(g);
				global::Char.myCharz().paintMount2(g);
			}
			for (int num5 = 0; num5 < GameScr.vCharInMap.size(); num5++)
			{
				global::Char char4 = null;
				try
				{
					char4 = (global::Char)GameScr.vCharInMap.elementAt(num5);
				}
				catch (Exception ex2)
				{
					Cout.LogError("Loi ham paint char gamescr: " + ex2.ToString());
				}
				bool flag18 = char4 != null && (!GameCanvas.panel.isShow || !GameCanvas.panel.isTypeShop()) && char4.skillPaint != null && char4.skillInfoPaint() != null && char4.indexSkill < char4.skillInfoPaint().Length;
				if (flag18)
				{
					char4.paintCharWithSkill(g);
					char4.paintMount2(g);
				}
			}
			for (int num6 = 0; num6 < GameScr.vItemMap.size(); num6++)
			{
				((ItemMap)GameScr.vItemMap.elementAt(num6)).paint(g);
			}
			g.translate(0, -GameCanvas.transY);
			GameCanvas.debug("PA9", 1);
			GameScr.paintSplash(g);
			GameCanvas.debug("PA10", 1);
			GameCanvas.debug("PA11", 1);
			GameCanvas.debug("PA13", 1);
			this.paintEffect(g);
			this.paintBgItem(g, 3);
			for (int num7 = 0; num7 < GameScr.vNpc.size(); num7++)
			{
				Npc npc2 = (Npc)GameScr.vNpc.elementAt(num7);
				npc2.paintName(g);
			}
			EffecMn.paintLayer3(g);
			for (int num8 = 0; num8 < GameScr.vNpc.size(); num8++)
			{
				Npc npc3 = (Npc)GameScr.vNpc.elementAt(num8);
				bool flag19 = npc3.chatInfo != null;
				if (flag19)
				{
					if (npc3 != null)
					{
						npc3.chatInfo.paint(g, npc3.cx, npc3.cy - npc3.ch - GameCanvas.transY, npc3.cdir);
					}
				}
			}
			for (int num9 = 0; num9 < GameScr.vCharInMap.size(); num9++)
			{
				global::Char char5 = null;
				try
				{
					char5 = (global::Char)GameScr.vCharInMap.elementAt(num9);
				}
				catch (Exception)
				{
				}
				bool flag20 = char5 != null && char5.chatInfo != null;
				if (flag20)
				{
					char5.chatInfo.paint(g, char5.cx, char5.cy - char5.ch, char5.cdir);
				}
			}
			bool flag21 = global::Char.myCharz().chatInfo != null;
			if (flag21)
			{
				global::Char.myCharz().chatInfo.paint(g, global::Char.myCharz().cx, global::Char.myCharz().cy - global::Char.myCharz().ch, global::Char.myCharz().cdir);
			}
			EffectManager.mid_2Effects.paintAll(g);
			EffectManager.midEffects.paintAll(g);
			BackgroudEffect.paintFrontAll(g);
			for (int num10 = 0; num10 < TileMap.vCurrItem.size(); num10++)
			{
				BgItem bgItem = (BgItem)TileMap.vCurrItem.elementAt(num10);
				bool flag22 = bgItem.idImage != -1 && bgItem.layer > 3;
				if (flag22)
				{
					bgItem.paint(g);
				}
			}
			PopUp.paintAll(g);
			bool flag23 = TileMap.mapID == 120;
			if (flag23)
			{
				bool flag24 = this.percentMabu != 100;
				if (flag24)
				{
					int num11 = (int)this.percentMabu * mGraphics.getImageWidth(GameScr.imgHPLost) / 100;
					int num12 = (int)this.percentMabu;
					g.drawImage(GameScr.imgHPLost, TileMap.pxw / 2 - mGraphics.getImageWidth(GameScr.imgHPLost) / 2, 220, 0);
					g.setClip(TileMap.pxw / 2 - mGraphics.getImageWidth(GameScr.imgHPLost) / 2, 220, num11, 10);
					g.drawImage(GameScr.imgHP, TileMap.pxw / 2 - mGraphics.getImageWidth(GameScr.imgHPLost) / 2, 220, 0);
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				}
				bool flag25 = this.mabuEff;
				if (flag25)
				{
					this.tMabuEff++;
					bool flag26 = GameCanvas.gameTick % 3 == 0;
					if (flag26)
					{
						Effect effect2 = new Effect(19, Res.random(TileMap.pxw / 2 - 50, TileMap.pxw / 2 + 50), 340, 2, 1, -1);
						EffecMn.addEff(effect2);
					}
					bool flag27 = GameCanvas.gameTick % 15 == 0;
					if (flag27)
					{
						Effect effect3 = new Effect(18, Res.random(TileMap.pxw / 2 - 5, TileMap.pxw / 2 + 5), Res.random(300, 320), 2, 1, -1);
						EffecMn.addEff(effect3);
					}
					bool flag28 = this.tMabuEff == 100;
					if (flag28)
					{
						this.activeSuperPower(TileMap.pxw / 2, 300);
					}
					bool flag29 = this.tMabuEff == 110;
					if (flag29)
					{
						this.tMabuEff = 0;
						this.mabuEff = false;
					}
				}
			}
			BackgroudEffect.paintFog(g);
			bool flag30 = true;
			for (int num13 = 0; num13 < BackgroudEffect.vBgEffect.size(); num13++)
			{
				BackgroudEffect backgroudEffect = (BackgroudEffect)BackgroudEffect.vBgEffect.elementAt(num13);
				bool flag31 = backgroudEffect.typeEff == 0;
				if (flag31)
				{
					flag30 = false;
					break;
				}
			}
			bool flag32 = mGraphics.zoomLevel <= 1 || Main.isIpod || Main.isIphone4;
			if (flag32)
			{
				flag30 = false;
			}
			bool flag33 = flag30 && !this.isRongThanXuatHien;
			if (flag33)
			{
				int num14 = TileMap.pxw / (mGraphics.getImageWidth(TileMap.imgLight) + 50);
				bool flag34 = num14 <= 0;
				if (flag34)
				{
					num14 = 1;
				}
				bool flag35 = TileMap.tileID != 28;
				if (flag35)
				{
					for (int num15 = 0; num15 < num14; num15++)
					{
						int num16 = 100 + num15 * (mGraphics.getImageWidth(TileMap.imgLight) + 50) - GameScr.cmx / 2;
						int num17 = -20;
						int imageWidth = mGraphics.getImageWidth(TileMap.imgLight);
						bool flag36 = num16 + imageWidth >= GameScr.cmx && num16 <= GameScr.cmx + GameCanvas.w && num17 + mGraphics.getImageHeight(TileMap.imgLight) >= GameScr.cmy && num17 <= GameScr.cmy + GameCanvas.h;
						if (flag36)
						{
							g.drawImage(TileMap.imgLight, 100 + num15 * (mGraphics.getImageWidth(TileMap.imgLight) + 50) - GameScr.cmx / 2, num17, 0);
						}
					}
				}
			}
			mSystem.paintFlyText(g);
			GameCanvas.debug("PA14", 1);
			GameCanvas.debug("PA15", 1);
			GameCanvas.debug("PA16", 1);
			this.paintArrowPointToNPC(g);
			GameCanvas.debug("PA17", 1);
			bool flag37 = !GameScr.isPaintOther && GameScr.isPaintRada == 1 && !GameCanvas.panel.isShow;
			if (flag37)
			{
				this.paintInfoBar(g);
			}
			GameScr.resetTranslate(g);
			this.paint_xp_bar(g);
			bool flag38 = !GameScr.isPaintOther;
			if (flag38)
			{
				AutoBroly.Painting(g);
				GameCanvas.debug("PA21", 1);
				GameCanvas.debug("PA18", 1);
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				bool flag39 = (TileMap.mapID == 128 || TileMap.mapID == 127) && GameScr.mabuPercent != 0;
				if (flag39)
				{
					int num18 = 30;
					int num19 = 200;
					g.setColor(0);
					g.fillRect(num18 - 27, num19 - 112, 54, 8);
					g.setColor(16711680);
					g.setClip(num18 - 25, num19 - 110, (int)GameScr.mabuPercent, 4);
					g.fillRect(num18 - 25, num19 - 110, 50, 4);
					g.setClip(0, 0, 3000, 3000);
					mFont.tahoma_7b_white.drawString(g, "Mabu", num18, num19 - 112 + 10, 2, mFont.tahoma_7b_dark);
				}
				bool isFusion = global::Char.myCharz().isFusion;
				if (isFusion)
				{
					global::Char.myCharz().tFusion++;
					bool flag40 = GameCanvas.gameTick % 3 == 0;
					if (flag40)
					{
						g.setColor(16777215);
						g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
					}
					bool flag41 = global::Char.myCharz().tFusion >= 100;
					if (flag41)
					{
						global::Char.myCharz().fusionComplete();
					}
				}
				for (int num20 = 0; num20 < GameScr.vCharInMap.size(); num20++)
				{
					global::Char char6 = null;
					try
					{
						char6 = (global::Char)GameScr.vCharInMap.elementAt(num20);
					}
					catch (Exception)
					{
					}
					bool flag42 = char6 != null && char6.isFusion && global::Char.isCharInScreen(char6);
					if (flag42)
					{
						char6.tFusion++;
						bool flag43 = GameCanvas.gameTick % 3 == 0;
						if (flag43)
						{
							g.setColor(16777215);
							g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
						}
						bool flag44 = char6.tFusion >= 100;
						if (flag44)
						{
							char6.fusionComplete();
						}
					}
				}
				GameCanvas.paintz.paintTabSoft(g);
				GameCanvas.debug("PA19", 1);
				GameCanvas.debug("PA20", 1);
				GameScr.resetTranslate(g);
				this.paintSelectedSkill(g);
				GameCanvas.debug("PA22", 1);
				GameScr.resetTranslate(g);
				bool flag45 = GameCanvas.isTouch && GameCanvas.isTouchControl;
				if (flag45)
				{
					this.paintTouchControl(g);
				}
				GameScr.resetTranslate(g);
				this.paintChatVip(g);
				bool flag46 = !GameCanvas.panel.isShow && GameCanvas.currentDialog == null && ChatPopup.currChatPopup == null && ChatPopup.serverChatPopUp == null && GameCanvas.currentScreen.Equals(GameScr.instance);
				if (flag46)
				{
					base.paint(g);
					bool flag47 = mScreen.keyMouse == 1 && this.cmdMenu != null;
					if (flag47)
					{
						g.drawImage(ItemMap.imageFlare, this.cmdMenu.x + 7, this.cmdMenu.y + 15, 3);
					}
				}
				GameScr.resetTranslate(g);
				int num21 = 100 + ((global::Char.vItemTime.size() != 0) ? (GameScr.textTime.size() * 12) : 0);
				bool flag48 = global::Char.myCharz().clan != null;
				if (flag48)
				{
					int num22 = 0;
					int num23 = 0;
					int num24 = (GameCanvas.h - 100 - 60) / 12;
					for (int num25 = 0; num25 < GameScr.vCharInMap.size(); num25++)
					{
						global::Char char7 = (global::Char)GameScr.vCharInMap.elementAt(num25);
						bool flag49 = char7.clanID == -1 || char7.clanID != global::Char.myCharz().clan.ID;
						if (!flag49)
						{
							bool flag50 = char7.isOutX() && char7.cx < global::Char.myCharz().cx;
							if (flag50)
							{
								int num26 = num24;
								bool flag51 = global::Char.vItemTime.size() != 0;
								if (flag51)
								{
									num26 -= GameScr.textTime.size();
								}
								bool flag52 = num22 <= num26;
								if (flag52)
								{
									mFont.tahoma_7_green.drawString(g, char7.cName, 20, num21 - 12 + num22 * 12, mFont.LEFT, mFont.tahoma_7_grey);
									char7.paintHp(g, 10, num21 + num22 * 12 - 5);
									num22++;
								}
							}
							else
							{
								bool flag53 = char7.isOutX() && char7.cx > global::Char.myCharz().cx && num23 <= num24;
								if (flag53)
								{
									mFont.tahoma_7_green.drawString(g, char7.cName, GameCanvas.w - 25, num21 - 12 + num23 * 12, mFont.RIGHT, mFont.tahoma_7_grey);
									char7.paintHp(g, GameCanvas.w - 15, num21 + num23 * 12 - 5);
									num23++;
								}
							}
						}
					}
				}
				ChatTextField.gI().paint(g);
				bool flag54 = GameScr.isNewClanMessage && !GameCanvas.panel.isShow && GameCanvas.gameTick % 4 == 0;
				if (flag54)
				{
					g.drawImage(ItemMap.imageFlare, this.cmdMenu.x + 15, this.cmdMenu.y + 30, mGraphics.BOTTOM | mGraphics.HCENTER);
				}
				bool flag55 = this.isSuperPower;
				if (flag55)
				{
					this.dxPower += 5;
					bool flag56 = this.tPower >= 0;
					if (flag56)
					{
						this.tPower += this.dxPower;
					}
					Res.outz("x power= " + this.xPower.ToString());
					bool flag57 = this.tPower < 0;
					if (flag57)
					{
						this.tPower--;
						bool flag58 = this.tPower == -20;
						if (flag58)
						{
							this.isSuperPower = false;
							this.tPower = 0;
							this.dxPower = 0;
						}
					}
					else
					{
						bool flag59 = (this.xPower - this.tPower > 0 || this.tPower < TileMap.pxw) && this.tPower > 0;
						if (flag59)
						{
							g.setColor(16777215);
							bool flag60 = !GameCanvas.lowGraphic;
							if (flag60)
							{
								g.fillArg(0, 0, GameCanvas.w, GameCanvas.h, 0, 0);
							}
							else
							{
								g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
							}
						}
						else
						{
							this.tPower = -1;
						}
					}
				}
				for (int num27 = 0; num27 < global::Char.vItemTime.size(); num27++)
				{
					((ItemTime)global::Char.vItemTime.elementAt(num27)).paint(g, this.cmdMenu.x + 32 + num27 * 24, 55);
				}
				for (int num28 = 0; num28 < GameScr.textTime.size(); num28++)
				{
					((ItemTime)GameScr.textTime.elementAt(num28)).paintText(g, this.cmdMenu.x + ((global::Char.vItemTime.size() == 0) ? 25 : 5), ((global::Char.vItemTime.size() == 0) ? 45 : 90) + num28 * 12);
				}
				this.paintXoSo(g);
				bool flag61 = mResources.language == 1;
				if (flag61)
				{
					long num29 = mSystem.currentTimeMillis() - GameScr.deltaTime;
					mFont.tahoma_7b_white.drawString(g, NinjaUtil.getDate2(num29), 10, GameCanvas.h - 65, 0, mFont.tahoma_7b_dark);
				}
				bool flag62 = !this.yourNumber.Equals(string.Empty);
				if (flag62)
				{
					for (int num30 = 0; num30 < this.strPaint.Length; num30++)
					{
						mFont.tahoma_7b_white.drawString(g, this.strPaint[num30], 5, 85 + num30 * 18, 0, mFont.tahoma_7b_dark);
					}
				}
			}
			int num31 = 0;
			int num32 = GameCanvas.hw;
			bool flag63 = num32 > 200;
			if (flag63)
			{
				num32 = 200;
			}
			this.paintPhuBanBar(g, num31 + GameCanvas.w / 2, 0, num32);
			EffectManager.hiEffects.paintAll(g);
			bool flag64 = GameScr.nCT_timeBallte > mSystem.currentTimeMillis() && TileMap.mapID == 170 && GameScr.isPaint_CT && GameScr.nCT_nBoyBaller / 2 > 0;
			if (flag64)
			{
				try
				{
					this.paint_CT(g, num31 + GameCanvas.w / 2, 0, num32);
				}
				catch (Exception)
				{
				}
			}
			bool flag65 = TileMap.mapID == 172;
			if (flag65)
			{
				string text = string.Concat(new string[]
				{
					mResources.WAIT,
					"  ",
					GameScr.nUSER_CT.ToString(),
					"/",
					GameScr.nUSER_MAX_CT.ToString()
				});
				mFont.tahoma_7b_dark.drawString(g, string.Concat(new string[]
				{
					mResources.WAIT,
					"  ",
					GameScr.nUSER_CT.ToString(),
					"/",
					GameScr.nUSER_MAX_CT.ToString()
				}), GameCanvas.w - 10, 40, 1);
			}
		}
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0005E6A8 File Offset: 0x0005C8A8
	private void paintXoSo(mGraphics g)
	{
		bool flag = this.tShow != 0;
		if (flag)
		{
			string text = string.Empty;
			for (int i = 0; i < this.winnumber.Length; i++)
			{
				text = text + this.randomNumber[i].ToString() + " ";
			}
			PopUp.paintPopUp(g, 20, 45, 95, 35, 16777215, false);
			mFont.tahoma_7b_dark.drawString(g, mResources.kquaVongQuay, 68, 50, 2);
			mFont.tahoma_7b_dark.drawString(g, text + string.Empty, 68, 65, 2);
		}
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x0005E74C File Offset: 0x0005C94C
	private void checkEffToObj(IMapObject obj, bool isnew)
	{
		bool flag = obj == null || this.tDoubleDelay > 0;
		if (!flag)
		{
			this.tDoubleDelay = 10;
			int x = obj.getX();
			int num = Res.abs(global::Char.myCharz().cx - x);
			int num2 = ((num <= 80) ? 1 : ((num > 80 && num <= 200) ? 2 : ((num <= 200 || num > 400) ? 4 : 3)));
			bool flag2 = !isnew;
			if (flag2)
			{
				bool flag3 = obj.Equals(global::Char.myCharz().mobFocus) || (obj.Equals(global::Char.myCharz().charFocus) && global::Char.myCharz().isMeCanAttackOtherPlayer(global::Char.myCharz().charFocus));
				if (flag3)
				{
					ServerEffect.addServerEffect(135, obj.getX(), obj.getY(), num2);
				}
				else
				{
					bool flag4 = obj.Equals(global::Char.myCharz().npcFocus) || obj.Equals(global::Char.myCharz().itemFocus) || obj.Equals(global::Char.myCharz().charFocus);
					if (flag4)
					{
						ServerEffect.addServerEffect(136, obj.getX(), obj.getY(), num2);
					}
				}
			}
			else
			{
				ServerEffect.addServerEffect(136, obj.getX(), obj.getY(), num2);
			}
		}
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0005E8A8 File Offset: 0x0005CAA8
	private void updateClickToArrow()
	{
		bool flag = this.tDoubleDelay > 0;
		if (flag)
		{
			this.tDoubleDelay--;
		}
		bool flag2 = this.clickMoving;
		if (flag2)
		{
			this.clickMoving = false;
			IMapObject mapObject = this.findClickToItem(this.clickToX, this.clickToY);
			bool flag3 = mapObject == null || (mapObject != null && mapObject.Equals(global::Char.myCharz().npcFocus) && TileMap.mapID == 51);
			if (flag3)
			{
				ServerEffect.addServerEffect(134, this.clickToX, this.clickToY + GameCanvas.transY / 2, 3);
			}
		}
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0005E948 File Offset: 0x0005CB48
	private void paintWaypointArrow(mGraphics g)
	{
		int num = 10;
		Task taskMaint = global::Char.myCharz().taskMaint;
		bool flag = taskMaint != null && taskMaint.taskId == 0 && ((taskMaint.index != 1 && taskMaint.index < 6) || taskMaint.index == 0);
		if (!flag)
		{
			for (int i = 0; i < TileMap.vGo.size(); i++)
			{
				Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
				bool flag2 = waypoint.minY == 0 || (int)waypoint.maxY >= TileMap.pxh - 24;
				if (flag2)
				{
					bool flag3 = (int)waypoint.maxY <= TileMap.pxh / 2;
					if (flag3)
					{
						int num2 = (int)(waypoint.minX + (waypoint.maxX - waypoint.minX) / 2);
						int num3 = (int)(waypoint.minY + (waypoint.maxY - waypoint.minY) / 2) + this.runArrow;
						bool isTouch = GameCanvas.isTouch;
						if (isTouch)
						{
							num3 = (int)(waypoint.maxY + (waypoint.maxY - waypoint.minY)) + this.runArrow + num;
						}
						g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 6, num2, num3, StaticObj.VCENTER_HCENTER);
					}
					else
					{
						bool flag4 = (int)waypoint.minY >= TileMap.pxh / 2;
						if (flag4)
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 4, (int)(waypoint.minX + (waypoint.maxX - waypoint.minX) / 2), (int)(waypoint.minY - 12) - this.runArrow, StaticObj.VCENTER_HCENTER);
						}
					}
				}
				else
				{
					bool flag5 = waypoint.minX >= 0 && waypoint.minX < 24;
					if (flag5)
					{
						bool flag6 = !GameCanvas.isTouch;
						if (flag6)
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 2, (int)(waypoint.maxX + 12) + this.runArrow, (int)(waypoint.maxY - 12), StaticObj.VCENTER_HCENTER);
						}
						else
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 2, (int)(waypoint.maxX + 12) + this.runArrow, (int)(waypoint.maxY - 32), StaticObj.VCENTER_HCENTER);
						}
					}
					else
					{
						bool flag7 = (int)waypoint.minX <= TileMap.tmw * 24 && (int)waypoint.minX >= TileMap.tmw * 24 - 48;
						if (flag7)
						{
							bool flag8 = !GameCanvas.isTouch;
							if (flag8)
							{
								g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 0, (int)(waypoint.minX - 12) - this.runArrow, (int)(waypoint.maxY - 12), StaticObj.VCENTER_HCENTER);
							}
							else
							{
								g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 0, (int)(waypoint.minX - 12) - this.runArrow, (int)(waypoint.maxY - 32), StaticObj.VCENTER_HCENTER);
							}
						}
						else
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 4, (int)(waypoint.minX + (waypoint.maxX - waypoint.minX) / 2), (int)(waypoint.maxY - 48) - this.runArrow, StaticObj.VCENTER_HCENTER);
						}
					}
				}
			}
		}
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0005EC9C File Offset: 0x0005CE9C
	public static Npc findNPCInMap(short id)
	{
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			bool flag = npc.template.npcTemplateId == (int)id;
			if (flag)
			{
				return npc;
			}
		}
		return null;
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x0005ECF4 File Offset: 0x0005CEF4
	public static global::Char findCharInMap(int charId)
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char.charID == charId;
			if (flag)
			{
				return @char;
			}
		}
		return null;
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0005ED48 File Offset: 0x0005CF48
	public static Mob findMobInMap(sbyte mobIndex)
	{
		return (Mob)GameScr.vMob.elementAt((int)mobIndex);
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0005ED6C File Offset: 0x0005CF6C
	public static Mob findMobInMap(int mobId)
	{
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob = (Mob)GameScr.vMob.elementAt(i);
			bool flag = mob.mobId == mobId;
			if (flag)
			{
				return mob;
			}
		}
		return null;
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0005EDC0 File Offset: 0x0005CFC0
	public static Npc getNpcTask()
	{
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			bool flag = npc.template.npcTemplateId == (int)GameScr.getTaskNpcId();
			if (flag)
			{
				return npc;
			}
		}
		return null;
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0005EE1C File Offset: 0x0005D01C
	private void paintArrowPointToNPC(mGraphics g)
	{
		try
		{
			bool flag = ChatPopup.currChatPopup != null;
			if (!flag)
			{
				int taskNpcId = (int)GameScr.getTaskNpcId();
				bool flag2 = taskNpcId == -1;
				if (!flag2)
				{
					Npc npc = null;
					for (int i = 0; i < GameScr.vNpc.size(); i++)
					{
						Npc npc2 = (Npc)GameScr.vNpc.elementAt(i);
						bool flag3 = npc2.template.npcTemplateId == taskNpcId;
						if (flag3)
						{
							bool flag4 = npc == null;
							if (flag4)
							{
								npc = npc2;
							}
							else
							{
								bool flag5 = Res.abs(npc2.cx - global::Char.myCharz().cx) < Res.abs(npc.cx - global::Char.myCharz().cx);
								if (flag5)
								{
									npc = npc2;
								}
							}
						}
					}
					bool flag6 = npc == null || npc.statusMe == 15 || (npc.cx > GameScr.cmx && npc.cx < GameScr.cmx + GameScr.gW && npc.cy > GameScr.cmy && npc.cy < GameScr.cmy + GameScr.gH) || GameCanvas.gameTick % 10 < 5;
					if (!flag6)
					{
						int num = npc.cx - global::Char.myCharz().cx;
						int num2 = npc.cy - global::Char.myCharz().cy;
						int num3 = 0;
						int num4 = 0;
						int num5 = 0;
						bool flag7 = num > 0 && num2 >= 0;
						if (flag7)
						{
							bool flag8 = Res.abs(num) >= Res.abs(num2);
							if (flag8)
							{
								num3 = GameScr.gW - 10;
								num4 = GameScr.gH / 2 + 30;
								bool isTouch = GameCanvas.isTouch;
								if (isTouch)
								{
									num4 = GameScr.gH / 2 + 10;
								}
								num5 = 0;
							}
							else
							{
								num3 = GameScr.gW / 2;
								num4 = GameScr.gH - 10;
								num5 = 5;
							}
						}
						else
						{
							bool flag9 = num >= 0 && num2 < 0;
							if (flag9)
							{
								bool flag10 = Res.abs(num) >= Res.abs(num2);
								if (flag10)
								{
									num3 = GameScr.gW - 10;
									num4 = GameScr.gH / 2 + 30;
									bool isTouch2 = GameCanvas.isTouch;
									if (isTouch2)
									{
										num4 = GameScr.gH / 2 + 10;
									}
									num5 = 0;
								}
								else
								{
									num3 = GameScr.gW / 2;
									num4 = 10;
									num5 = 6;
								}
							}
						}
						bool flag11 = num < 0 && num2 >= 0;
						if (flag11)
						{
							bool flag12 = Res.abs(num) >= Res.abs(num2);
							if (flag12)
							{
								num3 = 10;
								num4 = GameScr.gH / 2 + 30;
								bool isTouch3 = GameCanvas.isTouch;
								if (isTouch3)
								{
									num4 = GameScr.gH / 2 + 10;
								}
								num5 = 3;
							}
							else
							{
								num3 = GameScr.gW / 2;
								num4 = GameScr.gH - 10;
								num5 = 5;
							}
						}
						else
						{
							bool flag13 = num <= 0 && num2 < 0;
							if (flag13)
							{
								bool flag14 = Res.abs(num) >= Res.abs(num2);
								if (flag14)
								{
									num3 = 10;
									num4 = GameScr.gH / 2 + 30;
									bool isTouch4 = GameCanvas.isTouch;
									if (isTouch4)
									{
										num4 = GameScr.gH / 2 + 10;
									}
									num5 = 3;
								}
								else
								{
									num3 = GameScr.gW / 2;
									num4 = 10;
									num5 = 6;
								}
							}
						}
						GameScr.resetTranslate(g);
						g.drawRegion(GameScr.arrow, 0, 0, 13, 16, num5, num3, num4, StaticObj.VCENTER_HCENTER);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham arrow to npc: " + ex.ToString());
		}
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x00004E40 File Offset: 0x00003040
	public static void resetTranslate(mGraphics g)
	{
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		g.setClip(0, -200, GameCanvas.w, 200 + GameCanvas.h);
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0005F1B8 File Offset: 0x0005D3B8
	private void paintTouchControl(mGraphics g)
	{
		bool flag = this.isNotPaintTouchControl();
		if (!flag)
		{
			GameScr.resetTranslate(g);
			bool flag2 = !TileMap.isOfflineMap() && !this.isVS();
			if (flag2)
			{
				bool flag3 = mScreen.keyTouch == 15 || mScreen.keyMouse == 15;
				if (flag3)
				{
					g.drawImage((!Main.isPC) ? GameScr.imgChat2 : GameScr.imgChatsPC2, GameScr.xC + 17, GameScr.yC + 17 + mGraphics.addYWhenOpenKeyBoard, mGraphics.HCENTER | mGraphics.VCENTER);
				}
				else
				{
					g.drawImage((!Main.isPC) ? GameScr.imgChat : GameScr.imgChatPC, GameScr.xC + 17, GameScr.yC + 17 + mGraphics.addYWhenOpenKeyBoard, mGraphics.HCENTER | mGraphics.VCENTER);
				}
			}
			bool flag4 = GameScr.isUseTouch;
			if (flag4)
			{
			}
		}
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x0005F29C File Offset: 0x0005D49C
	public void paintImageBarRight(mGraphics g, global::Char c)
	{
		int num = (int)(c.cHP * GameScr.hpBarW / c.cHPFull);
		int num2 = (int)(c.cMP * (long)GameScr.mpBarW / c.cMPFull);
		int num3 = (int)(this.dHP * GameScr.hpBarW / c.cHPFull);
		int num4 = (int)(this.dMP * (long)GameScr.mpBarW / c.cMPFull);
		g.setClip(GameCanvas.w / 2 + 58 - mGraphics.getImageWidth(GameScr.imgPanel), 0, 95, 100);
		g.drawRegion(GameScr.imgPanel, 0, 0, mGraphics.getImageWidth(GameScr.imgPanel), mGraphics.getImageHeight(GameScr.imgPanel), 2, GameCanvas.w / 2 + 60, 0, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip((int)((long)(GameCanvas.w / 2 + 60 - 83) - GameScr.hpBarW + GameScr.hpBarW - (long)num3), 5, num3, 10);
		g.drawImage(GameScr.imgHPLost, GameCanvas.w / 2 + 60 - 83, 5, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		g.setClip((int)((long)(GameCanvas.w / 2 + 60 - 83) - GameScr.hpBarW + GameScr.hpBarW - (long)num), 5, num, 10);
		g.drawImage(GameScr.imgHP, GameCanvas.w / 2 + 60 - 83, 5, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		g.setClip((int)((long)(GameCanvas.w / 2 + 60 - 83 - GameScr.mpBarW) + GameScr.hpBarW - (long)num4), 20, num4, 6);
		g.drawImage(GameScr.imgMPLost, GameCanvas.w / 2 + 60 - 83, 20, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		g.setClip((int)((long)(GameCanvas.w / 2 + 60 - 83 - GameScr.mpBarW) + GameScr.hpBarW - (long)num2), 20, num2, 6);
		g.drawImage(GameScr.imgMP, GameCanvas.w / 2 + 60 - 83, 20, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0005F4E0 File Offset: 0x0005D6E0
	private void paintImageBar(mGraphics g, bool isLeft, global::Char c)
	{
		bool flag = c != null;
		if (flag)
		{
			bool flag2 = c.charID == global::Char.myCharz().charID;
			int num;
			int num2;
			int num3;
			int num4;
			if (flag2)
			{
				num = (int)(this.dHP * GameScr.hpBarW / c.cHPFull);
				num2 = (int)(this.dMP * (long)GameScr.mpBarW / c.cMPFull);
				num3 = (int)(c.cHP * GameScr.hpBarW / c.cHPFull);
				num4 = (int)(c.cMP * (long)GameScr.mpBarW / c.cMPFull);
			}
			else
			{
				num = (int)(c.dHP * GameScr.hpBarW / c.cHPFull);
				num2 = c.perCentMp * GameScr.mpBarW / 100;
				num3 = (int)(c.cHP * GameScr.hpBarW / c.cHPFull);
				num4 = c.perCentMp * GameScr.mpBarW / 100;
			}
			bool flag3 = global::Char.myCharz().secondPower > 0;
			if (flag3)
			{
				int num5 = (int)global::Char.myCharz().powerPoint * GameScr.spBarW / (int)global::Char.myCharz().maxPowerPoint;
				g.drawImage(GameScr.imgPanel2, 58, 29, 0);
				g.setClip(83, 31, num5, 10);
				g.drawImage(GameScr.imgSP, 83, 31, 0);
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				mFont.tahoma_7_white.drawString(g, string.Concat(new string[]
				{
					global::Char.myCharz().strInfo,
					":",
					global::Char.myCharz().powerPoint.ToString(),
					"/",
					global::Char.myCharz().maxPowerPoint.ToString()
				}), 115, 29, 2);
			}
			bool flag4 = c.charID != global::Char.myCharz().charID;
			if (flag4)
			{
				g.setClip(mGraphics.getImageWidth(GameScr.imgPanel) - 95, 0, 95, 100);
			}
			g.drawImage(GameScr.imgPanel, 0, 0, 0);
			if (isLeft)
			{
				g.setClip(83, 5, num, 10);
			}
			else
			{
				g.setClip((int)(83L + GameScr.hpBarW - (long)num), 5, num, 10);
			}
			g.drawImage(GameScr.imgHPLost, 83, 5, 0);
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			if (isLeft)
			{
				g.setClip(83, 5, num3, 10);
			}
			else
			{
				g.setClip((int)(83L + GameScr.hpBarW - (long)num3), 5, num3, 10);
			}
			g.drawImage(GameScr.imgHP, 83, 5, 0);
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			if (isLeft)
			{
				g.setClip(83, 20, num2, 6);
			}
			else
			{
				g.setClip(83 + GameScr.mpBarW - num2, 20, num2, 6);
			}
			g.drawImage(GameScr.imgMPLost, 83, 20, 0);
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			if (isLeft)
			{
				g.setClip(83, 20, num2, 6);
			}
			else
			{
				g.setClip(83 + GameScr.mpBarW - num4, 20, num4, 6);
			}
			g.drawImage(GameScr.imgMP, 83, 20, 0);
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			bool flag5 = global::Char.myCharz().cMP == 0L && GameCanvas.gameTick % 10 > 5;
			if (flag5)
			{
				g.setClip(83, 20, 2, 6);
				g.drawImage(GameScr.imgMPLost, 83, 20, 0);
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			}
		}
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00003E4C File Offset: 0x0000204C
	public void getInjure()
	{
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x0005F884 File Offset: 0x0005DA84
	public void starVS()
	{
		this.curr = (this.last = mSystem.currentTimeMillis());
		this.secondVS = 180;
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0005F8B4 File Offset: 0x0005DAB4
	private global::Char findCharVS1()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char.cTypePk != 0;
			if (flag)
			{
				return @char;
			}
		}
		return null;
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x0005F908 File Offset: 0x0005DB08
	private global::Char findCharVS2()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char.cTypePk != 0 && @char != this.findCharVS1();
			if (flag)
			{
				return @char;
			}
		}
		return null;
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0005F96C File Offset: 0x0005DB6C
	private void paintInfoBar(mGraphics g)
	{
		GameScr.resetTranslate(g);
		bool flag = TileMap.mapID == 130 && this.findCharVS1() != null && this.findCharVS2() != null;
		if (flag)
		{
			g.translate(GameCanvas.w / 2 - 62, 0);
			this.paintImageBar(g, true, this.findCharVS1());
			g.translate(-(GameCanvas.w / 2 - 65), 0);
			this.paintImageBarRight(g, this.findCharVS2());
			this.findCharVS1().paintHeadWithXY(g, 137, 25, 0);
			this.findCharVS2().paintHeadWithXY(g, GameCanvas.w - 15 - 122, 25, 2);
		}
		else
		{
			bool flag2 = this.isVS() && global::Char.myCharz().charFocus != null;
			if (flag2)
			{
				g.translate(GameCanvas.w / 2 - 62, 0);
				this.paintImageBar(g, true, global::Char.myCharz().charFocus);
				g.translate(-(GameCanvas.w / 2 - 65), 0);
				this.paintImageBarRight(g, global::Char.myCharz());
				global::Char.myCharz().paintHeadWithXY(g, 137, 25, 0);
				global::Char.myCharz().charFocus.paintHeadWithXY(g, GameCanvas.w - 15 - 122, 25, 2);
			}
			else
			{
				bool flag3 = GameScr.ispaintPhubangBar() && GameScr.isSmallScr();
				if (flag3)
				{
					GameScr.paintHPBar_NEW(g, 1, 1, global::Char.myCharz());
				}
				else
				{
					this.paintImageBar(g, true, global::Char.myCharz());
					bool flag4 = global::Char.myCharz().isInEnterOfflinePoint() != null || global::Char.myCharz().isInEnterOnlinePoint() != null;
					if (flag4)
					{
						mFont.tahoma_7_green2.drawString(g, mResources.enter, this.imgScrW / 2, 8 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
					}
					else
					{
						bool flag5 = global::Char.myCharz().mobFocus != null;
						if (!flag5)
						{
							bool flag6 = global::Char.myCharz().npcFocus != null;
							if (!flag6)
							{
								bool flag7 = global::Char.myCharz().charFocus != null;
								if (flag7)
								{
								}
							}
						}
					}
				}
			}
		}
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		bool flag8 = this.isVS() && this.secondVS > 0;
		if (flag8)
		{
			this.curr = mSystem.currentTimeMillis();
			bool flag9 = this.curr - this.last >= 1000L;
			if (flag9)
			{
				this.last = mSystem.currentTimeMillis();
				this.secondVS--;
			}
			mFont.tahoma_7b_white.drawString(g, this.secondVS.ToString() + string.Empty, GameCanvas.w / 2, 13, 2, mFont.tahoma_7b_dark);
		}
		bool flag10 = this.flareFindFocus;
		if (flag10)
		{
			g.drawImage(ItemMap.imageFlare, 40, 35, mGraphics.BOTTOM | mGraphics.HCENTER);
			this.flareTime--;
			bool flag11 = this.flareTime < 0;
			if (flag11)
			{
				this.flareTime = 0;
				this.flareFindFocus = false;
			}
		}
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x0005FC7C File Offset: 0x0005DE7C
	public bool isVS()
	{
		return TileMap.isVoDaiMap() && (global::Char.myCharz().cTypePk != 0 || (TileMap.mapID == 130 && this.findCharVS1() != null && this.findCharVS2() != null));
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x0005FCD4 File Offset: 0x0005DED4
	private void paintSelectedSkill(mGraphics g)
	{
		bool flag = this.mobCapcha != null;
		if (flag)
		{
			this.paintCapcha(g);
		}
		else
		{
			bool flag2 = GameCanvas.currentDialog != null || ChatPopup.currChatPopup != null || GameCanvas.menu.showMenu || this.isPaintPopup() || GameCanvas.panel.isShow || global::Char.myCharz().taskMaint.taskId == 0 || ChatTextField.gI().isShow || GameCanvas.currentScreen == MoneyCharge.instance;
			if (!flag2)
			{
				long num = mSystem.currentTimeMillis();
				long num2 = num - this.lastUsePotion;
				int num3 = 0;
				bool flag3 = num2 < 10000L;
				if (flag3)
				{
					num3 = (int)(num2 * 20L / 10000L);
				}
				bool flag4 = !GameCanvas.isTouch;
				if (flag4)
				{
					g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgSkill : GameScr.imgSkill2, GameScr.xSkill + GameScr.xHP - 1, GameScr.yHP - 1, 0);
					SmallImage.drawSmallImage(g, 542, GameScr.xSkill + GameScr.xHP + 3, GameScr.yHP + 3, 0, 0);
					mFont.number_gray.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xSkill + GameScr.xHP + 22, GameScr.yHP + 15, 1);
					bool flag5 = num2 < 10000L;
					if (flag5)
					{
						g.setColor(2721889);
						num3 = (int)(num2 * 20L / 10000L);
						g.fillRect(GameScr.xSkill + GameScr.xHP + 3, GameScr.yHP + 3 + num3, 20, 20 - num3);
					}
				}
				else
				{
					bool flag6 = global::Char.myCharz().statusMe != 14;
					if (flag6)
					{
						bool isSmallGamePad = GameScr.gamePad.isSmallGamePad;
						if (isSmallGamePad)
						{
							bool flag7 = GameScr.isAnalog != 1;
							if (flag7)
							{
								g.setColor(9670800);
								g.fillRect(GameScr.xHP + 9, GameScr.yHP + 10 + 10, 22, 20);
								g.setColor(16777215);
								g.fillRect(GameScr.xHP + 9, GameScr.yHP + 10 + ((num3 != 0) ? (20 - num3) : 0) + 10, 22, (num3 == 0) ? 20 : num3);
								g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgHP1 : GameScr.imgHP2, GameScr.xHP, GameScr.yHP + 10, 0);
								mFont.tahoma_7_red.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xHP + 20, GameScr.yHP + 15 + 10, 2);
								bool flag8 = GameScr.isPickNgocRong;
								if (flag8)
								{
									g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNR1 : GameScr.imgNR2, GameScr.xHP + 5, GameScr.yHP - 6 - 40 + 10, 0);
								}
								else
								{
									bool flag9 = GameScr.isudungCapsun4;
									if (flag9)
									{
										g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNutF : GameScr.imgNut, GameScr.xHP + 5, GameScr.yHP - 6 - 40 + 10, 0);
										SmallImage.drawSmallImage(g, 1088, GameScr.xHP - 7 + 5, GameScr.yHP - 6 - 40 - 7 + 10, 0, 0);
									}
									else
									{
										bool flag10 = GameScr.isudungCapsun3;
										if (flag10)
										{
											g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNutF : GameScr.imgNut, GameScr.xHP + 5, GameScr.yHP - 6 - 40 + 10, 0);
											SmallImage.drawSmallImage(g, 1087, GameScr.xHP - 7 + 5, GameScr.yHP - 6 - 40 - 7 + 10, 0, 0);
										}
									}
								}
							}
							else
							{
								bool flag11 = GameScr.isAnalog == 1;
								if (flag11)
								{
									int num4 = 10;
									g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgSkill : GameScr.imgSkill2, GameScr.xSkill + GameScr.xHP - 1, GameScr.yHP - 1 + num4, 0);
									SmallImage.drawSmallImage(g, 542, GameScr.xSkill + GameScr.xHP + 3, GameScr.yHP + 3 + num4, 0, 0);
									mFont.number_gray.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xSkill + GameScr.xHP + 22, GameScr.yHP + 13 + num4, 1);
									bool flag12 = num2 < 10000L;
									if (flag12)
									{
										g.setColor(2721889);
										num3 = (int)(num2 * 20L / 10000L);
										g.fillRect(GameScr.xSkill + GameScr.xHP + 3, GameScr.yHP + 3 + num3 + num4, 20, 20 - num3);
									}
									bool flag13 = GameScr.isPickNgocRong;
									if (flag13)
									{
										g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNR3 : GameScr.imgNR4, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
									}
									else
									{
										bool flag14 = GameScr.isudungCapsun4;
										if (flag14)
										{
											g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
											SmallImage.drawSmallImage(g, 1088, GameScr.xHP + 20 - 7 + 5, GameScr.yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
										}
										else
										{
											bool flag15 = GameScr.isudungCapsun3;
											if (flag15)
											{
												g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
												SmallImage.drawSmallImage(g, 1087, GameScr.xHP + 20 - 7 + 5, GameScr.yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
											}
										}
									}
								}
							}
						}
						else
						{
							bool flag16 = GameScr.isAnalog != 1;
							if (flag16)
							{
								g.setColor(9670800);
								g.fillRect(GameScr.xHP + 9, GameScr.yHP + 10 - 6, 22, 20);
								g.setColor(16777215);
								g.fillRect(GameScr.xHP + 9, GameScr.yHP + 10 + ((num3 != 0) ? (20 - num3) : 0) - 6, 22, (num3 == 0) ? 20 : num3);
								g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgHP1 : GameScr.imgHP2, GameScr.xHP, GameScr.yHP - 6, 0);
								mFont.tahoma_7_red.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xHP + 20, GameScr.yHP + 15 - 6, 2);
								bool flag17 = GameScr.isPickNgocRong;
								if (flag17)
								{
									g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNR1 : GameScr.imgNR2, GameScr.xHP, GameScr.yHP - 6 - 40, 0);
								}
								else
								{
									bool flag18 = GameScr.isudungCapsun4;
									if (flag18)
									{
										g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20, GameScr.yHP + 20 - 6 - 40, mGraphics.HCENTER | mGraphics.VCENTER);
										SmallImage.drawSmallImage(g, 1088, GameScr.xHP + 20 - 7, GameScr.yHP + 20 - 6 - 40 - 7, 0, 0);
									}
									else
									{
										bool flag19 = GameScr.isudungCapsun3;
										if (flag19)
										{
											g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20, GameScr.yHP + 20 - 6 - 40, mGraphics.HCENTER | mGraphics.VCENTER);
											SmallImage.drawSmallImage(g, 1087, GameScr.xHP + 20 - 7, GameScr.yHP + 20 - 6 - 40 - 7, 0, 0);
										}
									}
								}
							}
							else
							{
								g.setColor(9670800);
								g.fillRect(GameScr.xHP + 10, GameScr.yHP + 10 - 6 + 10, 20, 18);
								g.setColor(16777215);
								g.fillRect(GameScr.xHP + 10, GameScr.yHP + 10 + ((num3 != 0) ? (20 - num3) : 0) - 6 + 10, 20, (num3 == 0) ? 18 : num3);
								g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgHP3 : GameScr.imgHP4, GameScr.xHP + 20, GameScr.yHP + 20 - 6 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
								mFont.tahoma_7_red.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xHP + 20, GameScr.yHP + 15 - 6 + 10, 2);
								bool flag20 = GameScr.isPickNgocRong;
								if (flag20)
								{
									g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNR3 : GameScr.imgNR4, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
								}
								else
								{
									bool flag21 = GameScr.isudungCapsun4;
									if (flag21)
									{
										g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
										SmallImage.drawSmallImage(g, 1088, GameScr.xHP + 20 - 7 + 5, GameScr.yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
									}
									else
									{
										bool flag22 = GameScr.isudungCapsun3;
										if (flag22)
										{
											g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
											SmallImage.drawSmallImage(g, 1087, GameScr.xHP + 20 - 7 + 5, GameScr.yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
										}
									}
								}
							}
						}
					}
				}
				bool flag23 = GameScr.isHaveSelectSkill;
				if (flag23)
				{
					Skill[] array = (Main.isPC ? GameScr.keySkill : ((!GameCanvas.isTouch) ? GameScr.keySkill : GameScr.onScreenSkill));
					bool flag24 = mScreen.keyTouch == 10;
					if (flag24)
					{
					}
					bool flag25 = !GameCanvas.isTouch;
					if (flag25)
					{
						g.setColor(11152401);
						g.fillRect(GameScr.xSkill + GameScr.xHP + 2, GameScr.yHP - 10 + 6, 20, 10);
						mFont.tahoma_7_white.drawString(g, "*", GameScr.xSkill + GameScr.xHP + 12, GameScr.yHP - 8 + 6, mFont.CENTER);
					}
					int num5 = (Main.isPC ? array.Length : ((!GameCanvas.isTouch) ? array.Length : this.nSkill));
					for (int i = 0; i < num5; i++)
					{
						bool isPC = Main.isPC;
						if (isPC)
						{
							string[] array3;
							if (!TField.isQwerty)
							{
								string[] array2 = new string[5];
								array2[0] = "";
								array2[1] = "";
								array2[2] = "";
								array2[3] = "";
								array3 = array2;
								array2[4] = "";
							}
							else
							{
								string[] array4 = new string[10];
								array4[0] = "";
								array4[1] = "";
								array4[2] = "";
								array4[3] = "";
								array4[4] = "";
								array4[5] = "";
								array4[6] = "";
								array4[7] = "";
								array4[8] = "";
								array3 = array4;
								array4[9] = "";
							}
							string[] array5 = array3;
							int num6 = -13;
							bool flag26 = num5 > 5 && i < 5;
							if (flag26)
							{
								num6 = 27;
							}
							mFont.tahoma_7b_dark.drawString(g, array5[i], GameScr.xSkill + GameScr.xS[i] + 14, GameScr.yS[i] + num6, mFont.CENTER);
							mFont.tahoma_7b_white.drawString(g, array5[i], GameScr.xSkill + GameScr.xS[i] + 14, GameScr.yS[i] + num6 + 1, mFont.CENTER);
						}
						else
						{
							bool flag27 = !GameCanvas.isTouch;
							if (flag27)
							{
								string[] array7;
								if (!TField.isQwerty)
								{
									string[] array6 = new string[5];
									array6[0] = "7";
									array6[1] = "8";
									array6[2] = "9";
									array6[3] = "1";
									array7 = array6;
									array6[4] = "3";
								}
								else
								{
									string[] array8 = new string[5];
									array8[0] = "Q";
									array8[1] = "W";
									array8[2] = "E";
									array8[3] = "R";
									array7 = array8;
									array8[4] = "T";
								}
								string[] array9 = array7;
								g.setColor(11152401);
								g.fillRect(GameScr.xSkill + GameScr.xS[i] + 2, GameScr.yS[i] - 10 + 8, 20, 10);
								mFont.tahoma_7_white.drawString(g, array9[i], GameScr.xSkill + GameScr.xS[i] + 12, GameScr.yS[i] - 10 + 6, mFont.CENTER);
							}
						}
						Skill skill = array[i];
						bool flag28 = skill != global::Char.myCharz().myskill;
						if (flag28)
						{
							g.drawImage(GameScr.imgSkill, GameScr.xSkill + GameScr.xS[i] - 1, GameScr.yS[i] - 1, 0);
						}
						bool flag29 = skill == null;
						if (!flag29)
						{
							bool flag30 = skill == global::Char.myCharz().myskill;
							if (flag30)
							{
								g.drawImage(GameScr.imgSkill2, GameScr.xSkill + GameScr.xS[i] - 1, GameScr.yS[i] - 1, 0);
								bool flag31 = GameCanvas.isTouch && !Main.isPC;
								if (flag31)
								{
									g.drawRegion(Mob.imgHP, 0, 12, 9, 6, 0, GameScr.xSkill + GameScr.xS[i] + 8, GameScr.yS[i] - 7, 0);
								}
							}
							skill.paint(GameScr.xSkill + GameScr.xS[i] + 13, GameScr.yS[i] + 13, g);
							bool flag32 = (i == this.selectedIndexSkill && !this.isPaintUI() && GameCanvas.gameTick % 10 > 5) || i == this.keyTouchSkill;
							if (flag32)
							{
								g.drawImage(ItemMap.imageFlare, GameScr.xSkill + GameScr.xS[i] + 13, GameScr.yS[i] + 14, 3);
							}
							long num7 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							bool flag33 = skill.template.id == 7;
							if (flag33)
							{
								GameScr.timehoichieubuff = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag34 = skill.template.id == 19;
							if (flag34)
							{
								GameScr.timehoikhien = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag35 = skill.template.id == 8;
							if (flag35)
							{
								GameScr.timehoiskill3 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag36 = skill.template.id == 1;
							if (flag36)
							{
								GameScr.ccc1 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag37 = skill.template.id == 3;
							if (flag37)
							{
								GameScr.ccc3 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag38 = skill.template.id == 5;
							if (flag38)
							{
								GameScr.ccc5 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag39 = skill.template.id == 14;
							if (flag39)
							{
								GameScr.timehoibom = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag40 = skill.template.id == 22;
							if (flag40)
							{
								GameScr.timehoithoimien = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag41 = skill.template.id == 24 || skill.template.id == 25 || skill.template.id == 26;
							if (flag41)
							{
								GameScr.timehoiskill9 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							mFont.tahoma_7b_white.drawString(g, (num7 > 0L) ? string.Concat(num7 / 1000L) : string.Empty, GameScr.xSkill + GameScr.xS[i] + 14, GameScr.yS[i] + 8, mFont.CENTER, mFont.tahoma_7b_red);
						}
					}
				}
				this.paintGamePad(g);
			}
		}
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00060DC0 File Offset: 0x0005EFC0
	public void paintOpen(mGraphics g)
	{
		bool flag = this.isstarOpen;
		if (flag)
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.fillRect(0, 0, GameCanvas.w, this.moveUp);
			g.setColor(10275899);
			g.fillRect(0, this.moveUp - 1, GameCanvas.w, 1);
			g.fillRect(0, this.moveDow + 1, GameCanvas.w, 1);
		}
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00060E3C File Offset: 0x0005F03C
	public static void startFlyText(string flyString, int x, int y, int dx, int dy, int color)
	{
		int num = -1;
		for (int i = 0; i < 5; i++)
		{
			bool flag = GameScr.flyTextState[i] == -1;
			if (flag)
			{
				num = i;
				break;
			}
		}
		bool flag2 = num == -1;
		if (!flag2)
		{
			GameScr.flyTextColor[num] = color;
			GameScr.flyTextString[num] = flyString;
			GameScr.flyTextX[num] = x;
			GameScr.flyTextY[num] = y;
			GameScr.flyTextDx[num] = dx;
			GameScr.flyTextDy[num] = ((dy >= 0) ? 5 : (-5));
			GameScr.flyTextState[num] = 0;
			GameScr.flyTime[num] = 0;
			GameScr.flyTextYTo[num] = 10;
			for (int j = 0; j < 5; j++)
			{
				bool flag3 = GameScr.flyTextState[j] != -1 && num != j && GameScr.flyTextDy[num] < 0 && Res.abs(GameScr.flyTextX[num] - GameScr.flyTextX[j]) <= 20 && GameScr.flyTextYTo[num] == GameScr.flyTextYTo[j];
				if (flag3)
				{
					GameScr.flyTextYTo[num] += 10;
				}
			}
		}
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x00060F4C File Offset: 0x0005F14C
	public static void updateFlyText()
	{
		for (int i = 0; i < 5; i++)
		{
			bool flag = GameScr.flyTextState[i] == -1;
			if (!flag)
			{
				bool flag2 = GameScr.flyTextState[i] > GameScr.flyTextYTo[i];
				if (flag2)
				{
					GameScr.flyTime[i]++;
					bool flag3 = GameScr.flyTime[i] == 25;
					if (flag3)
					{
						GameScr.flyTime[i] = 0;
						GameScr.flyTextState[i] = -1;
						GameScr.flyTextYTo[i] = 0;
						GameScr.flyTextDx[i] = 0;
						GameScr.flyTextX[i] = 0;
					}
				}
				else
				{
					GameScr.flyTextState[i] += Res.abs(GameScr.flyTextDy[i]);
					GameScr.flyTextX[i] += GameScr.flyTextDx[i];
					GameScr.flyTextY[i] += GameScr.flyTextDy[i];
				}
			}
		}
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00061034 File Offset: 0x0005F234
	public static void loadSplash()
	{
		bool flag = GameScr.imgSplash == null;
		if (flag)
		{
			GameScr.imgSplash = new Image[3];
			for (int i = 0; i < 3; i++)
			{
				GameScr.imgSplash[i] = GameCanvas.loadImage("/e/sp" + i.ToString() + ".png");
			}
		}
		GameScr.splashX = new int[2];
		GameScr.splashY = new int[2];
		GameScr.splashState = new int[2];
		GameScr.splashF = new int[2];
		GameScr.splashDir = new int[2];
		GameScr.splashState[0] = (GameScr.splashState[1] = -1);
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x000610D8 File Offset: 0x0005F2D8
	public static bool startSplash(int x, int y, int dir)
	{
		int num = ((GameScr.splashState[0] != -1) ? 1 : 0);
		bool flag = GameScr.splashState[num] != -1;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			GameScr.splashState[num] = 0;
			GameScr.splashDir[num] = dir;
			GameScr.splashX[num] = x;
			GameScr.splashY[num] = y;
			flag2 = true;
		}
		return flag2;
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x00061130 File Offset: 0x0005F330
	public static void updateSplash()
	{
		for (int i = 0; i < 2; i++)
		{
			bool flag = GameScr.splashState[i] != -1;
			if (flag)
			{
				GameScr.splashState[i]++;
				GameScr.splashX[i] += GameScr.splashDir[i] << 2;
				GameScr.splashY[i]--;
				bool flag2 = GameScr.splashState[i] >= 6;
				if (flag2)
				{
					GameScr.splashState[i] = -1;
				}
				else
				{
					GameScr.splashF[i] = (GameScr.splashState[i] >> 1) % 3;
				}
			}
		}
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x000611D4 File Offset: 0x0005F3D4
	public static void paintSplash(mGraphics g)
	{
		for (int i = 0; i < 2; i++)
		{
			bool flag = GameScr.splashState[i] != -1;
			if (flag)
			{
				bool flag2 = GameScr.splashDir[i] == 1;
				if (flag2)
				{
					g.drawImage(GameScr.imgSplash[GameScr.splashF[i]], GameScr.splashX[i], GameScr.splashY[i], 3);
				}
				else
				{
					g.drawRegion(GameScr.imgSplash[GameScr.splashF[i]], 0, 0, mGraphics.getImageWidth(GameScr.imgSplash[GameScr.splashF[i]]), mGraphics.getImageHeight(GameScr.imgSplash[GameScr.splashF[i]]), 2, GameScr.splashX[i], GameScr.splashY[i], 3);
				}
			}
		}
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00004E75 File Offset: 0x00003075
	private void loadInforBar()
	{
		this.imgScrW = 84;
		GameScr.hpBarW = 66L;
		GameScr.mpBarW = 59;
		GameScr.hpBarX = 52;
		GameScr.hpBarY = 10;
		GameScr.spBarW = 61;
		GameScr.expBarW = GameScr.gW - 61;
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00061294 File Offset: 0x0005F494
	public void updateSS()
	{
		bool flag = GameScr.indexMenu != -1;
		if (flag)
		{
			bool flag2 = GameScr.cmySK != GameScr.cmtoYSK;
			if (flag2)
			{
				GameScr.cmvySK = GameScr.cmtoYSK - GameScr.cmySK << 2;
				GameScr.cmdySK += GameScr.cmvySK;
				GameScr.cmySK += GameScr.cmdySK >> 4;
				GameScr.cmdySK &= 15;
			}
			bool flag3 = global::Math.abs(GameScr.cmtoYSK - GameScr.cmySK) < 15 && GameScr.cmySK < 0;
			if (flag3)
			{
				GameScr.cmtoYSK = 0;
			}
			bool flag4 = global::Math.abs(GameScr.cmtoYSK - GameScr.cmySK) < 15 && GameScr.cmySK > GameScr.cmyLimSK;
			if (flag4)
			{
				GameScr.cmtoYSK = GameScr.cmyLimSK;
			}
		}
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0006136C File Offset: 0x0005F56C
	public void updateKeyAlert()
	{
		bool flag = !GameScr.isPaintAlert || GameCanvas.currentDialog != null;
		if (!flag)
		{
			bool flag2 = false;
			bool flag3 = GameCanvas.keyPressed[Key.NUM8];
			if (flag3)
			{
				GameScr.indexRow++;
				bool flag4 = GameScr.indexRow >= this.texts.size();
				if (flag4)
				{
					GameScr.indexRow = 0;
				}
				flag2 = true;
			}
			else
			{
				bool flag5 = GameCanvas.keyPressed[Key.NUM2];
				if (flag5)
				{
					GameScr.indexRow--;
					bool flag6 = GameScr.indexRow < 0;
					if (flag6)
					{
						GameScr.indexRow = this.texts.size() - 1;
					}
					flag2 = true;
				}
			}
			bool flag7 = flag2;
			if (flag7)
			{
				GameScr.scrMain.moveTo(GameScr.indexRow * GameScr.scrMain.ITEM_SIZE);
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
			}
			bool isTouch = GameCanvas.isTouch;
			if (isTouch)
			{
				ScrollResult scrollResult = GameScr.scrMain.updateKey();
				bool flag8 = scrollResult.isDowning || scrollResult.isFinish;
				if (flag8)
				{
					GameScr.indexRow = scrollResult.selected;
					flag2 = true;
				}
			}
			bool flag9 = !flag2 || GameScr.indexRow < 0 || GameScr.indexRow >= this.texts.size();
			if (!flag9)
			{
				string text = (string)this.texts.elementAt(GameScr.indexRow);
				this.fnick = null;
				this.alertURL = null;
				this.center = null;
				ChatTextField.gI().center = null;
				int num;
				bool flag10 = (num = text.IndexOf("http://")) >= 0;
				if (flag10)
				{
					Cout.println("currentLine: " + text);
					this.alertURL = text.Substring(num);
					this.center = new Command(mResources.open_link, 12000);
					bool flag11 = !GameCanvas.isTouch;
					if (flag11)
					{
						ChatTextField.gI().center = new Command(mResources.open_link, null, 12000, null);
					}
				}
				else
				{
					bool flag12 = text.IndexOf("@") < 0;
					if (!flag12)
					{
						string text2 = text.Substring(2);
						text2 = text2.Trim();
						num = text2.IndexOf("@");
						string text3 = text2.Substring(num);
						int num2 = text3.IndexOf(" ");
						num2 = ((num2 > 0) ? (num2 + num) : (num + text3.Length));
						this.fnick = text2.Substring(num + 1, num2);
						bool flag13 = !this.fnick.Equals(string.Empty) && !this.fnick.Equals(global::Char.myCharz().cName);
						if (flag13)
						{
							this.center = new Command(mResources.SELECT, 12009, this.fnick);
							bool flag14 = !GameCanvas.isTouch;
							if (flag14)
							{
								ChatTextField.gI().center = new Command(mResources.SELECT, null, 12009, this.fnick);
							}
						}
						else
						{
							this.fnick = null;
							this.center = null;
						}
					}
				}
			}
		}
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x0006168C File Offset: 0x0005F88C
	public bool isPaintPopup()
	{
		return GameScr.isPaintItemInfo || GameScr.isPaintInfoMe || GameScr.isPaintStore || GameScr.isPaintWeapon || GameScr.isPaintNonNam || GameScr.isPaintNonNu || GameScr.isPaintAoNam || GameScr.isPaintAoNu || GameScr.isPaintGangTayNam || GameScr.isPaintGangTayNu || GameScr.isPaintQuanNam || GameScr.isPaintQuanNu || GameScr.isPaintGiayNam || GameScr.isPaintGiayNu || GameScr.isPaintLien || GameScr.isPaintNhan || GameScr.isPaintNgocBoi || GameScr.isPaintPhu || GameScr.isPaintStack || GameScr.isPaintStackLock || GameScr.isPaintGrocery || GameScr.isPaintGroceryLock || GameScr.isPaintUpGrade || GameScr.isPaintConvert || GameScr.isPaintSplit || GameScr.isPaintUpPearl || GameScr.isPaintBox || GameScr.isPaintTrade || GameScr.isPaintAlert || GameScr.isPaintZone || GameScr.isPaintTeam || GameScr.isPaintClan || GameScr.isPaintFindTeam || GameScr.isPaintTask || GameScr.isPaintFriend || GameScr.isPaintEnemies || GameScr.isPaintCharInMap || GameScr.isPaintMessage;
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x000617EC File Offset: 0x0005F9EC
	public bool isNotPaintTouchControl()
	{
		bool flag = !GameCanvas.isTouchControl && GameCanvas.currentScreen == GameScr.gI();
		bool flag2;
		if (flag)
		{
			flag2 = true;
		}
		else
		{
			bool flag3 = !GameCanvas.isTouch;
			if (flag3)
			{
				flag2 = true;
			}
			else
			{
				bool isShow = ChatTextField.gI().isShow;
				if (isShow)
				{
					flag2 = true;
				}
				else
				{
					bool isShow2 = InfoDlg.isShow;
					if (isShow2)
					{
						flag2 = true;
					}
					else
					{
						bool flag4 = GameCanvas.currentDialog != null || ChatPopup.currChatPopup != null || GameCanvas.menu.showMenu || GameCanvas.panel.isShow || this.isPaintPopup();
						flag2 = flag4;
					}
				}
			}
		}
		return flag2;
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0006188C File Offset: 0x0005FA8C
	public bool isPaintUI()
	{
		return GameScr.isPaintStore || GameScr.isPaintWeapon || GameScr.isPaintNonNam || GameScr.isPaintNonNu || GameScr.isPaintAoNam || GameScr.isPaintAoNu || GameScr.isPaintGangTayNam || GameScr.isPaintGangTayNu || GameScr.isPaintQuanNam || GameScr.isPaintQuanNu || GameScr.isPaintGiayNam || GameScr.isPaintGiayNu || GameScr.isPaintLien || GameScr.isPaintNhan || GameScr.isPaintNgocBoi || GameScr.isPaintPhu || GameScr.isPaintStack || GameScr.isPaintStackLock || GameScr.isPaintGrocery || GameScr.isPaintGroceryLock || GameScr.isPaintUpGrade || GameScr.isPaintConvert || GameScr.isPaintSplit || GameScr.isPaintUpPearl || GameScr.isPaintBox || GameScr.isPaintTrade;
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x00061974 File Offset: 0x0005FB74
	public bool isOpenUI()
	{
		return GameScr.isPaintItemInfo || GameScr.isPaintInfoMe || GameScr.isPaintStore || GameScr.isPaintNonNam || GameScr.isPaintNonNu || GameScr.isPaintAoNam || GameScr.isPaintAoNu || GameScr.isPaintGangTayNam || GameScr.isPaintGangTayNu || GameScr.isPaintQuanNam || GameScr.isPaintQuanNu || GameScr.isPaintGiayNam || GameScr.isPaintGiayNu || GameScr.isPaintLien || GameScr.isPaintNhan || GameScr.isPaintNgocBoi || GameScr.isPaintPhu || GameScr.isPaintWeapon || GameScr.isPaintStack || GameScr.isPaintStackLock || GameScr.isPaintGrocery || GameScr.isPaintGroceryLock || GameScr.isPaintUpGrade || GameScr.isPaintConvert || GameScr.isPaintUpPearl || GameScr.isPaintBox || GameScr.isPaintSplit || GameScr.isPaintTrade;
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00061A70 File Offset: 0x0005FC70
	public static void setPopupSize(int w, int h)
	{
		bool flag = GameCanvas.w == 128 || GameCanvas.h <= 208;
		if (flag)
		{
			w = 126;
			h = 160;
		}
		GameScr.indexTitle = 0;
		GameScr.popupW = w;
		GameScr.popupH = h;
		GameScr.popupX = GameScr.gW2 - w / 2;
		GameScr.popupY = GameScr.gH2 - h / 2;
		bool flag2 = GameCanvas.isTouch && !GameScr.isPaintZone && !GameScr.isPaintTeam && !GameScr.isPaintClan && !GameScr.isPaintCharInMap && !GameScr.isPaintFindTeam && !GameScr.isPaintFriend && !GameScr.isPaintEnemies && !GameScr.isPaintTask && !GameScr.isPaintMessage;
		if (flag2)
		{
			bool flag3 = GameCanvas.h <= 240;
			if (flag3)
			{
				GameScr.popupY -= 10;
			}
			bool flag4 = GameCanvas.isTouch && !GameCanvas.isTouchControlSmallScreen && GameCanvas.currentScreen is GameScr;
			if (flag4)
			{
				GameScr.popupW = 310;
				GameScr.popupX = GameScr.gW / 2 - GameScr.popupW / 2;
				bool flag5 = GameScr.isPaintInfoMe && GameScr.indexMenu > 0;
				if (flag5)
				{
					GameScr.popupW = w;
					GameScr.popupX = GameScr.gW2 - w / 2;
				}
			}
		}
		bool flag6 = GameScr.popupY < -10;
		if (flag6)
		{
			GameScr.popupY = -10;
		}
		bool flag7 = GameCanvas.h > 208 && GameScr.popupY < 0;
		if (flag7)
		{
			GameScr.popupY = 0;
		}
		bool flag8 = GameCanvas.h == 208 && GameScr.popupY < 10;
		if (flag8)
		{
			GameScr.popupY = 10;
		}
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00004EB1 File Offset: 0x000030B1
	public static void loadImg()
	{
		TileMap.loadTileImage();
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x00061C24 File Offset: 0x0005FE24
	public void paintTitle(mGraphics g, string title, bool arrow)
	{
		int num = GameScr.gW / 2;
		g.setColor(Paint.COLORDARK);
		g.fillRoundRect(num - mFont.tahoma_8b.getWidth(title) / 2 - 12, GameScr.popupY + 4, mFont.tahoma_8b.getWidth(title) + 22, 24, 6, 6);
		bool flag = (GameScr.indexTitle == 0 || GameCanvas.isTouch) && arrow;
		if (flag)
		{
			SmallImage.drawSmallImage(g, 989, num - mFont.tahoma_8b.getWidth(title) / 2 - 15 - 7 - ((GameCanvas.gameTick % 8 <= 3) ? 2 : 0), GameScr.popupY + 16, 2, StaticObj.VCENTER_HCENTER);
			SmallImage.drawSmallImage(g, 989, num + mFont.tahoma_8b.getWidth(title) / 2 + 15 + 5 + ((GameCanvas.gameTick % 8 <= 3) ? 2 : 0), GameScr.popupY + 16, 0, StaticObj.VCENTER_HCENTER);
		}
		bool flag2 = GameScr.indexTitle == 0;
		if (flag2)
		{
			g.setColor(Paint.COLORFOCUS);
		}
		else
		{
			g.setColor(Paint.COLORBORDER);
		}
		g.drawRoundRect(num - mFont.tahoma_8b.getWidth(title) / 2 - 12, GameScr.popupY + 4, mFont.tahoma_8b.getWidth(title) + 22, 24, 6, 6);
		mFont.tahoma_8b.drawString(g, title, num, GameScr.popupY + 9, 2);
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00061D80 File Offset: 0x0005FF80
	public static int getTaskMapId()
	{
		bool flag = global::Char.myCharz().taskMaint == null;
		int num;
		if (flag)
		{
			num = -1;
		}
		else
		{
			num = GameScr.mapTasks[global::Char.myCharz().taskMaint.index];
		}
		return num;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x00061DC0 File Offset: 0x0005FFC0
	public static sbyte getTaskNpcId()
	{
		sbyte b = 0;
		bool flag = global::Char.myCharz().taskMaint == null;
		if (flag)
		{
			b = -1;
		}
		else
		{
			bool flag2 = global::Char.myCharz().taskMaint.index <= GameScr.tasks.Length - 1;
			if (flag2)
			{
				b = (sbyte)GameScr.tasks[global::Char.myCharz().taskMaint.index];
			}
		}
		return b;
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x00003E4C File Offset: 0x0000204C
	public void refreshTeam()
	{
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x00061E28 File Offset: 0x00060028
	public void onChatFromMe(string text, string to)
	{
		Res.outz("CHAT");
		bool flag = !GameScr.isPaintMessage || GameCanvas.isTouch;
		if (flag)
		{
			ChatTextField.gI().isShow = false;
		}
		bool flag2 = to.Equals(mResources.chat_player);
		if (flag2)
		{
			bool flag3 = GameScr.info2.playerID != global::Char.myCharz().charID;
			if (flag3)
			{
				Service.gI().chatPlayer(text, GameScr.info2.playerID);
			}
		}
		else
		{
			bool flag4 = !text.Equals(string.Empty);
			if (flag4)
			{
				Service.gI().chat(text);
			}
		}
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00061ECC File Offset: 0x000600CC
	public void onCancelChat()
	{
		bool flag = GameScr.isPaintMessage;
		if (flag)
		{
			GameScr.isPaintMessage = false;
			ChatTextField.gI().center = null;
		}
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00061EF8 File Offset: 0x000600F8
	public void openWeb(string strLeft, string strRight, string url, string title, string str)
	{
		GameScr.isPaintAlert = true;
		this.isLockKey = true;
		GameScr.indexRow = 0;
		GameScr.setPopupSize(175, 200);
		this.textsTitle = title;
		this.texts = mFont.tahoma_7.splitFontVector(str, GameScr.popupW - 30);
		this.center = null;
		this.left = new Command(strLeft, 11068, url);
		this.right = new Command(strRight, 11069);
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00061F78 File Offset: 0x00060178
	public void sendSms(string strLeft, string strRight, short port, string syntax, string title, string str)
	{
		GameScr.isPaintAlert = true;
		this.isLockKey = true;
		GameScr.indexRow = 0;
		GameScr.setPopupSize(175, 200);
		this.textsTitle = title;
		this.texts = mFont.tahoma_7.splitFontVector(str, GameScr.popupW - 30);
		this.center = null;
		MyVector myVector = new MyVector();
		myVector.addElement(string.Empty + port.ToString());
		myVector.addElement(syntax);
		this.left = new Command(strLeft, 11074);
		this.right = new Command(strRight, 11075);
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00004EBA File Offset: 0x000030BA
	public void actMenu()
	{
		GameCanvas.panel.setTypeMain();
		GameCanvas.panel.show();
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0006201C File Offset: 0x0006021C
	public void openUIZone(Message message)
	{
		InfoDlg.hide();
		try
		{
			this.zones = new int[(int)message.reader().readByte()];
			this.pts = new int[this.zones.Length];
			this.numPlayer = new int[this.zones.Length];
			this.maxPlayer = new int[this.zones.Length];
			this.rank1 = new int[this.zones.Length];
			this.rankName1 = new string[this.zones.Length];
			this.rank2 = new int[this.zones.Length];
			this.rankName2 = new string[this.zones.Length];
			for (int i = 0; i < this.zones.Length; i++)
			{
				this.zones[i] = (int)message.reader().readByte();
				this.pts[i] = (int)message.reader().readByte();
				this.numPlayer[i] = (int)message.reader().readByte();
				this.maxPlayer[i] = (int)message.reader().readByte();
				sbyte b = message.reader().readByte();
				bool flag = b == 1;
				if (flag)
				{
					this.rankName1[i] = message.reader().readUTF();
					this.rank1[i] = message.reader().readInt();
					this.rankName2[i] = message.reader().readUTF();
					this.rank2[i] = message.reader().readInt();
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham OPEN UIZONE " + ex.ToString());
		}
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x000621D8 File Offset: 0x000603D8
	public void openUIZoneDemo(Message message)
	{
		InfoDlg.hide();
		try
		{
			this.zones = new int[(int)message.reader().readByte()];
			this.pts = new int[this.zones.Length];
			this.numPlayer = new int[this.zones.Length];
			this.maxPlayer = new int[this.zones.Length];
			this.rank1 = new int[this.zones.Length];
			this.rankName1 = new string[this.zones.Length];
			this.rank2 = new int[this.zones.Length];
			this.rankName2 = new string[this.zones.Length];
			for (int i = 0; i < this.zones.Length; i++)
			{
				this.zones[i] = (int)message.reader().readByte();
				this.pts[i] = (int)message.reader().readByte();
				this.numPlayer[i] = (int)message.reader().readByte();
				this.maxPlayer[i] = (int)message.reader().readByte();
				sbyte b = message.reader().readByte();
				bool flag = b == 1;
				if (flag)
				{
					this.rankName1[i] = message.reader().readUTF();
					this.rank1[i] = message.reader().readInt();
					this.rankName2[i] = message.reader().readUTF();
					this.rank2[i] = message.reader().readInt();
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham OPEN UIZONE " + ex.ToString());
		}
		GameCanvas.panel.setTypeZone();
		GameCanvas.panel.show();
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00004ED3 File Offset: 0x000030D3
	public void showViewInfo()
	{
		GameScr.indexMenu = 3;
		GameScr.isPaintInfoMe = true;
		GameScr.setPopupSize(175, 200);
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x000623A8 File Offset: 0x000605A8
	private void actDead()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command(mResources.DIES[1], 110381));
		myVector.addElement(new Command(mResources.DIES[2], 110382));
		myVector.addElement(new Command(mResources.DIES[3], 110383));
		GameCanvas.menu.startAt(myVector, 3);
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00004EF2 File Offset: 0x000030F2
	public void startYesNoPopUp(string info, Command cmdYes, Command cmdNo)
	{
		this.popUpYesNo = new PopUpYesNo();
		this.popUpYesNo.setPopUp(info, cmdYes, cmdNo);
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00062414 File Offset: 0x00060614
	public void player_vs_player(int playerId, int xu, string info, sbyte typePK)
	{
		global::Char @char = GameScr.findCharInMap(playerId);
		bool flag = @char != null;
		if (flag)
		{
			bool flag2 = typePK == 3;
			if (flag2)
			{
				this.startYesNoPopUp(info, new Command(mResources.OK, 2000, @char), new Command(mResources.CLOSE, 2009, @char));
			}
			bool flag3 = typePK == 4;
			if (flag3)
			{
				this.startYesNoPopUp(info, new Command(mResources.OK, 2005, @char), new Command(mResources.CLOSE, 2009, @char));
			}
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0006249C File Offset: 0x0006069C
	public void giaodich(int playerID)
	{
		global::Char @char = GameScr.findCharInMap(playerID);
		bool flag = @char != null;
		if (flag)
		{
			this.startYesNoPopUp(@char.cName + mResources.want_to_trade, new Command(mResources.YES, 11114, @char), new Command(mResources.NO, 2009, @char));
		}
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x000624F4 File Offset: 0x000606F4
	public void getFlagImage(int charID, sbyte cflag)
	{
		bool flag = GameScr.vFlag.size() == 0;
		if (flag)
		{
			Service.gI().getFlag(2, cflag);
			Res.outz("getFlag1");
		}
		else
		{
			bool flag2 = charID == global::Char.myCharz().charID;
			if (flag2)
			{
				Res.outz("my cflag: isme");
				bool flag3 = global::Char.myCharz().isGetFlagImage(cflag);
				if (flag3)
				{
					Res.outz("my cflag: true");
					for (int i = 0; i < GameScr.vFlag.size(); i++)
					{
						PKFlag pkflag = (PKFlag)GameScr.vFlag.elementAt(i);
						bool flag4 = pkflag != null && pkflag.cflag == cflag;
						if (flag4)
						{
							Res.outz("my cflag: cflag==");
							global::Char.myCharz().flagImage = pkflag.IDimageFlag;
						}
					}
				}
				else
				{
					bool flag5 = !global::Char.myCharz().isGetFlagImage(cflag);
					if (flag5)
					{
						Res.outz("my cflag: false");
						Service.gI().getFlag(2, cflag);
					}
				}
			}
			else
			{
				Res.outz("my cflag: not me");
				bool flag6 = GameScr.findCharInMap(charID) == null;
				if (!flag6)
				{
					bool flag7 = GameScr.findCharInMap(charID).isGetFlagImage(cflag);
					if (flag7)
					{
						Res.outz("my cflag: true");
						for (int j = 0; j < GameScr.vFlag.size(); j++)
						{
							PKFlag pkflag2 = (PKFlag)GameScr.vFlag.elementAt(j);
							bool flag8 = pkflag2 != null && pkflag2.cflag == cflag;
							if (flag8)
							{
								Res.outz("my cflag: cflag==");
								GameScr.findCharInMap(charID).flagImage = pkflag2.IDimageFlag;
							}
						}
					}
					else
					{
						bool flag9 = !GameScr.findCharInMap(charID).isGetFlagImage(cflag);
						if (flag9)
						{
							Res.outz("my cflag: false");
							Service.gI().getFlag(2, cflag);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x000626E0 File Offset: 0x000608E0
	public void actionPerform(int idAction, object p)
	{
		Cout.println("PERFORM WITH ID = " + idAction.ToString());
		bool flag = idAction == 999901;
		if (flag)
		{
			new Thread(new ThreadStart(AddSetDo.MacSet1)).Start();
		}
		bool flag2 = idAction == 999902;
		if (flag2)
		{
			new Thread(new ThreadStart(AddSetDo.MacSet2)).Start();
		}
		int num = idAction;
		int num2 = num;
		if (num2 <= 11059)
		{
			if (num2 <= 8002)
			{
				if (num2 <= 2)
				{
					if (num2 != 1)
					{
						if (num2 == 2)
						{
							GameCanvas.menu.showMenu = false;
						}
					}
					else
					{
						GameCanvas.endDlg();
					}
				}
				else
				{
					switch (num2)
					{
					case 2000:
					{
						this.popUpYesNo = null;
						GameCanvas.endDlg();
						bool flag3 = (global::Char)p == null;
						if (flag3)
						{
							Service.gI().player_vs_player(1, 3, -1);
						}
						else
						{
							Service.gI().player_vs_player(1, 3, ((global::Char)p).charID);
							Service.gI().charMove();
						}
						break;
					}
					case 2001:
						GameCanvas.endDlg();
						break;
					case 2002:
					case 2008:
						break;
					case 2003:
						GameCanvas.endDlg();
						InfoDlg.showWait();
						Service.gI().player_vs_player(0, 3, global::Char.myCharz().charFocus.charID);
						break;
					case 2004:
						GameCanvas.endDlg();
						Service.gI().player_vs_player(0, 4, global::Char.myCharz().charFocus.charID);
						break;
					case 2005:
					{
						GameCanvas.endDlg();
						this.popUpYesNo = null;
						bool flag4 = (global::Char)p == null;
						if (flag4)
						{
							Service.gI().player_vs_player(1, 4, -1);
						}
						else
						{
							Service.gI().player_vs_player(1, 4, ((global::Char)p).charID);
						}
						break;
					}
					case 2006:
						GameCanvas.endDlg();
						Service.gI().player_vs_player(2, 4, global::Char.myCharz().charFocus.charID);
						break;
					case 2007:
						GameCanvas.endDlg();
						GameMidlet.instance.exit();
						break;
					case 2009:
						this.popUpYesNo = null;
						break;
					default:
						if (num2 == 8002)
						{
							this.doFire(false, true);
							GameCanvas.clearKeyHold();
							GameCanvas.clearKeyPressed();
						}
						break;
					}
				}
			}
			else if (num2 <= 11038)
			{
				switch (num2)
				{
				case 11000:
					this.actMenu();
					break;
				case 11001:
					global::Char.myCharz().findNextFocusByKey();
					break;
				case 11002:
					GameCanvas.panel.hide();
					break;
				default:
					if (num2 == 11038)
					{
						this.actDead();
					}
					break;
				}
			}
			else if (num2 != 11057)
			{
				if (num2 == 11059)
				{
					Skill skill = GameScr.onScreenSkill[this.selectedIndexSkill];
					this.doUseSkill(skill, false);
					this.center = null;
				}
			}
			else
			{
				Effect2.vEffect2Outside.removeAllElements();
				Effect2.vEffect2.removeAllElements();
				Npc npc = (Npc)p;
				bool flag5 = npc.idItem == 0;
				if (flag5)
				{
					Service.gI().confirmMenu((short)npc.template.npcTemplateId, (sbyte)GameCanvas.menu.menuSelectedItem);
				}
				else
				{
					bool flag6 = GameCanvas.menu.menuSelectedItem == 0;
					if (flag6)
					{
						Service.gI().pickItem(npc.idItem);
					}
				}
			}
		}
		else if (num2 <= 110001)
		{
			if (num2 <= 11121)
			{
				if (num2 != 11067)
				{
					switch (num2)
					{
					case 11111:
					{
						bool flag7 = global::Char.myCharz().charFocus != null;
						if (flag7)
						{
							InfoDlg.showWait();
							bool flag8 = GameCanvas.panel.vPlayerMenu.size() <= 0;
							if (flag8)
							{
								this.playerMenu(global::Char.myCharz().charFocus);
							}
							GameCanvas.panel.setTypePlayerMenu(global::Char.myCharz().charFocus);
							GameCanvas.panel.show();
							Service.gI().getPlayerMenu(global::Char.myCharz().charFocus.charID);
							Service.gI().messagePlayerMenu(global::Char.myCharz().charFocus.charID);
						}
						break;
					}
					case 11112:
					{
						global::Char @char = (global::Char)p;
						Service.gI().friend(1, @char.charID);
						break;
					}
					case 11113:
					{
						global::Char char2 = (global::Char)p;
						bool flag9 = char2 != null;
						if (flag9)
						{
							Service.gI().giaodich(0, char2.charID, -1, -1);
						}
						break;
					}
					case 11114:
					{
						this.popUpYesNo = null;
						GameCanvas.endDlg();
						global::Char char3 = (global::Char)p;
						bool flag10 = char3 != null;
						if (flag10)
						{
							Service.gI().giaodich(1, char3.charID, -1, -1);
						}
						break;
					}
					case 11115:
					{
						bool flag11 = global::Char.myCharz().charFocus != null;
						if (flag11)
						{
							InfoDlg.showWait();
							Service.gI().playerMenuAction(global::Char.myCharz().charFocus.charID, (short)global::Char.myCharz().charFocus.menuSelect);
						}
						break;
					}
					case 11120:
					{
						object[] array = (object[])p;
						Skill skill2 = (Skill)array[0];
						int num3 = int.Parse((string)array[1]);
						for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
						{
							bool flag12 = GameScr.onScreenSkill[i] == skill2;
							if (flag12)
							{
								GameScr.onScreenSkill[i] = null;
							}
						}
						GameScr.onScreenSkill[num3] = skill2;
						this.saveonScreenSkillToRMS();
						break;
					}
					case 11121:
					{
						object[] array2 = (object[])p;
						Skill skill3 = (Skill)array2[0];
						int num4 = int.Parse((string)array2[1]);
						for (int j = 0; j < GameScr.keySkill.Length; j++)
						{
							bool flag13 = GameScr.keySkill[j] == skill3;
							if (flag13)
							{
								GameScr.keySkill[j] = null;
							}
						}
						GameScr.keySkill[num4] = skill3;
						this.saveKeySkillToRMS();
						break;
					}
					}
				}
				else
				{
					bool flag14 = TileMap.zoneID != GameScr.indexSelect;
					if (flag14)
					{
						Service.gI().requestChangeZone(GameScr.indexSelect, this.indexItemUse);
						InfoDlg.showWait();
					}
					else
					{
						GameScr.info1.addInfo(mResources.ZONE_HERE, 0);
					}
				}
			}
			else
			{
				switch (num2)
				{
				case 12000:
					Service.gI().getClan(1, -1, null);
					break;
				case 12001:
					GameCanvas.endDlg();
					break;
				case 12002:
				{
					GameCanvas.endDlg();
					ClanObject clanObject = (ClanObject)p;
					Service.gI().clanInvite(1, -1, clanObject.clanID, clanObject.code);
					this.popUpYesNo = null;
					break;
				}
				case 12003:
				{
					ClanObject clanObject2 = (ClanObject)p;
					GameCanvas.endDlg();
					Service.gI().clanInvite(2, -1, clanObject2.clanID, clanObject2.code);
					this.popUpYesNo = null;
					break;
				}
				case 12004:
				{
					Skill skill4 = (Skill)p;
					this.doUseSkill(skill4, true);
					global::Char.myCharz().saveLoadPreviousSkill();
					break;
				}
				case 12005:
				{
					bool flag15 = GameCanvas.serverScr == null;
					if (flag15)
					{
						GameCanvas.serverScr = new ServerScr();
					}
					GameCanvas.serverScr.switchToMe();
					GameCanvas.endDlg();
					break;
				}
				case 12006:
					GameMidlet.instance.exit();
					break;
				default:
					if (num2 == 110001)
					{
						GameCanvas.panel.setTypeMain();
						GameCanvas.panel.show();
					}
					break;
				}
			}
		}
		else if (num2 <= 110382)
		{
			if (num2 != 110004)
			{
				if (num2 == 110382)
				{
					Service.gI().returnTownFromDead();
				}
			}
			else
			{
				GameCanvas.menu.showMenu = false;
			}
		}
		else if (num2 != 110383)
		{
			if (num2 != 110391)
			{
				if (num2 == 888351)
				{
					Service.gI().petStatus(5);
					GameCanvas.endDlg();
				}
			}
			else
			{
				Service.gI().clanInvite(0, global::Char.myCharz().charFocus.charID, -1, -1);
			}
		}
		else
		{
			Service.gI().wakeUpFromDead();
		}
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x00062F70 File Offset: 0x00061170
	private static void setTouchBtn()
	{
		bool flag = GameScr.isAnalog != 0;
		if (flag)
		{
			GameScr.xTG = (GameScr.xF = GameCanvas.w - 45);
			bool isLargeGamePad = GameScr.gamePad.isLargeGamePad;
			if (isLargeGamePad)
			{
				GameScr.xSkill = GameScr.gamePad.wZone + 20;
				GameScr.wSkill = 35;
				GameScr.xHP = GameScr.xF - 45;
			}
			else
			{
				bool isMediumGamePad = GameScr.gamePad.isMediumGamePad;
				if (isMediumGamePad)
				{
					GameScr.xHP = GameScr.xF - 45;
				}
			}
			GameScr.yF = GameCanvas.h - 45;
			GameScr.yTG = GameScr.yF - 45;
		}
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00063014 File Offset: 0x00061214
	private void updateGamePad()
	{
		bool flag = GameScr.isAnalog == 0 || global::Char.myCharz().statusMe == 14;
		if (!flag)
		{
			bool flag2 = GameCanvas.isPointerHoldIn(GameScr.xF, GameScr.yF, 40, 40);
			if (flag2)
			{
				mScreen.keyTouch = 5;
				bool isPointerJustRelease = GameCanvas.isPointerJustRelease;
				if (isPointerJustRelease)
				{
					GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = true;
					GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
				}
			}
			GameScr.gamePad.update();
			bool flag3 = GameCanvas.isPointerHoldIn(GameScr.xTG, GameScr.yTG, 34, 34);
			if (flag3)
			{
				mScreen.keyTouch = 13;
				GameCanvas.isPointerJustDown = false;
				this.isPointerDowning = false;
				bool flag4 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
				if (flag4)
				{
					global::Char.myCharz().findNextFocusByKey();
					GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
				}
			}
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x00063100 File Offset: 0x00061300
	private void paintGamePad(mGraphics g)
	{
		bool flag = GameScr.isAnalog != 0 && global::Char.myCharz().statusMe != 14;
		if (flag)
		{
			g.drawImage((mScreen.keyTouch != 5 && mScreen.keyMouse != 5) ? GameScr.imgFire0 : GameScr.imgFire1, GameScr.xF + 20, GameScr.yF + 20, mGraphics.HCENTER | mGraphics.VCENTER);
			GameScr.gamePad.paint(g);
			g.drawImage((mScreen.keyTouch != 13) ? GameScr.imgFocus : GameScr.imgFocus2, GameScr.xTG + 20, GameScr.yTG + 20, mGraphics.HCENTER | mGraphics.VCENTER);
		}
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x000631B4 File Offset: 0x000613B4
	public void showWinNumber(string num, string finish)
	{
		this.winnumber = new int[num.Length];
		this.randomNumber = new int[num.Length];
		this.tMove = new int[num.Length];
		this.moveCount = new int[num.Length];
		this.delayMove = new int[num.Length];
		try
		{
			for (int i = 0; i < num.Length; i++)
			{
				this.winnumber[i] = (int)short.Parse(num[i].ToString());
				this.randomNumber[i] = Res.random(0, 11);
				this.tMove[i] = 1;
				this.delayMove[i] = 0;
			}
		}
		catch (Exception)
		{
		}
		this.tShow = 100;
		this.moveIndex = 0;
		this.strFinish = finish;
		GameScr.lastXS = (GameScr.currXS = mSystem.currentTimeMillis());
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x000632AC File Offset: 0x000614AC
	public void chatVip(string chatVip)
	{
		DovaBaoKhu.TbBoss = chatVip;
		bool flag = !this.startChat;
		if (flag)
		{
			this.currChatWidth = mFont.tahoma_7b_yellowSmall.getWidth(chatVip);
			this.xChatVip = GameCanvas.w;
			this.startChat = true;
		}
		bool flag2 = chatVip.StartsWith("!");
		if (flag2)
		{
			chatVip = chatVip.Substring(1, chatVip.Length);
			this.isFireWorks = true;
		}
		GameScr.vChatVip.addElement(chatVip);
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x00004F0F File Offset: 0x0000310F
	public void clearChatVip()
	{
		GameScr.vChatVip.removeAllElements();
		this.xChatVip = GameCanvas.w;
		this.startChat = false;
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x00063328 File Offset: 0x00061528
	public void paintChatVip(mGraphics g)
	{
		bool flag = GameScr.vChatVip.size() != 0 && GameScr.isPaintChatVip;
		if (flag)
		{
			g.setClip(0, GameCanvas.h - 13, GameCanvas.w, 15);
			g.fillRect(0, GameCanvas.h - 13, GameCanvas.w, 15, 0, 90);
			string text = (string)GameScr.vChatVip.elementAt(0);
			mFont.tahoma_7b_yellow.drawString(g, text, this.xChatVip, GameCanvas.h - 13, 0, mFont.tahoma_7b_dark);
		}
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x000633B4 File Offset: 0x000615B4
	public void updateChatVip()
	{
		bool flag = !this.startChat;
		if (!flag)
		{
			this.xChatVip -= 2;
			bool flag2 = this.xChatVip < -this.currChatWidth;
			if (flag2)
			{
				this.xChatVip = GameCanvas.w;
				GameScr.vChatVip.removeElementAt(0);
				bool flag3 = GameScr.vChatVip.size() == 0;
				if (flag3)
				{
					this.isFireWorks = false;
					this.startChat = false;
				}
				else
				{
					this.currChatWidth = mFont.tahoma_7b_white.getWidth((string)GameScr.vChatVip.elementAt(0));
				}
			}
		}
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x00004F2F File Offset: 0x0000312F
	public void showYourNumber(string strNum)
	{
		this.yourNumber = strNum;
		this.strPaint = mFont.tahoma_7.splitFontArray(this.yourNumber, 500);
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x00004F54 File Offset: 0x00003154
	public static void checkRemoveImage()
	{
		ImgByName.checkDelHash(ImgByName.hashImagePath, 10, false);
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x00063454 File Offset: 0x00061654
	public static void StartServerPopUp(string strMsg)
	{
		GameCanvas.endDlg();
		int num = 1139;
		ChatPopup.addBigMessage(strMsg, 100000, new Npc(-1, 0, 0, 0, 0, 0)
		{
			avatar = num
		});
		ChatPopup.serverChatPopUp.cmdMsg1 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null);
		ChatPopup.serverChatPopUp.cmdMsg1.x = GameCanvas.w / 2 - 35;
		ChatPopup.serverChatPopUp.cmdMsg1.y = GameCanvas.h - 35;
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x000634E0 File Offset: 0x000616E0
	public static bool ispaintPhubangBar()
	{
		return TileMap.mapPhuBang() && GameScr.phuban_Info.type_PB == 0;
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x00063514 File Offset: 0x00061714
	public void paintPhuBanBar(mGraphics g, int x, int y, int w)
	{
		bool flag = GameScr.phuban_Info == null || GameScr.isPaintOther || GameScr.isPaintRada != 1 || GameCanvas.panel.isShow || !GameScr.ispaintPhubangBar();
		if (!flag)
		{
			bool flag2 = w < GameScr.fra_PVE_Bar_1.frameWidth + GameScr.fra_PVE_Bar_0.frameWidth * 4;
			if (flag2)
			{
				w = GameScr.fra_PVE_Bar_1.frameWidth + GameScr.fra_PVE_Bar_0.frameWidth * 4;
			}
			bool flag3 = x > GameCanvas.w - w / 2;
			if (flag3)
			{
				x = GameCanvas.w - w / 2;
			}
			bool flag4 = x < mGraphics.getImageWidth(GameScr.imgKhung) + w / 2 + 10;
			if (flag4)
			{
				x = mGraphics.getImageWidth(GameScr.imgKhung) + w / 2 + 10;
			}
			int frameHeight = GameScr.fra_PVE_Bar_0.frameHeight;
			int num = y + frameHeight + mGraphics.getImageHeight(GameScr.imgBall) / 2 + 2;
			int frameWidth = GameScr.fra_PVE_Bar_1.frameWidth;
			int num2 = w / 2 - frameWidth / 2;
			int num3 = x - w / 2;
			int num4 = x + frameWidth / 2;
			int num5 = y + 3;
			int num6 = num2 - GameScr.fra_PVE_Bar_0.frameWidth;
			int num7 = num6 / GameScr.fra_PVE_Bar_0.frameWidth;
			bool flag5 = num6 % GameScr.fra_PVE_Bar_0.frameWidth > 0;
			if (flag5)
			{
				num7++;
			}
			for (int i = 0; i < num7; i++)
			{
				bool flag6 = i < num7 - 1;
				if (flag6)
				{
					GameScr.fra_PVE_Bar_0.drawFrame(1, num3 + GameScr.fra_PVE_Bar_0.frameWidth + i * GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
				}
				else
				{
					GameScr.fra_PVE_Bar_0.drawFrame(1, num3 + num6, num5, 0, 0, g);
				}
				bool flag7 = i < num7 - 1;
				if (flag7)
				{
					GameScr.fra_PVE_Bar_0.drawFrame(1, num4 + i * GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
				}
				else
				{
					GameScr.fra_PVE_Bar_0.drawFrame(1, num4 + num6 - GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
				}
			}
			GameScr.fra_PVE_Bar_0.drawFrame(0, num3, num5, 2, 0, g);
			GameScr.fra_PVE_Bar_0.drawFrame(0, num4 + num6, num5, 0, 0, g);
			bool flag8 = GameScr.phuban_Info.pointTeam1 > 0;
			if (flag8)
			{
				int num8 = 2;
				int num9 = 3;
				bool flag9 = GameScr.phuban_Info.color_1 == 4;
				if (flag9)
				{
					num8 = 4;
					num9 = 5;
				}
				int num10 = GameScr.phuban_Info.pointTeam1 * num2 / GameScr.phuban_Info.maxPoint;
				bool flag10 = num10 < 0;
				if (flag10)
				{
					num10 = 0;
				}
				bool flag11 = num10 > num2;
				if (flag11)
				{
					num10 = num2;
				}
				g.setClip(num3 + num2 - num10, num5, num10, frameHeight);
				for (int j = 0; j < num7; j++)
				{
					bool flag12 = j < num7 - 1;
					if (flag12)
					{
						GameScr.fra_PVE_Bar_0.drawFrame(num9, num3 + GameScr.fra_PVE_Bar_0.frameWidth + j * GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
					}
					else
					{
						GameScr.fra_PVE_Bar_0.drawFrame(num9, num3 + num6, num5, 0, 0, g);
					}
				}
				GameScr.fra_PVE_Bar_0.drawFrame(num8, num3, num5, 2, 0, g);
				GameCanvas.resetTrans(g);
			}
			bool flag13 = GameScr.phuban_Info.pointTeam2 > 0;
			if (flag13)
			{
				int num11 = 2;
				int num12 = 3;
				bool flag14 = GameScr.phuban_Info.color_2 == 4;
				if (flag14)
				{
					num11 = 4;
					num12 = 5;
				}
				int num13 = GameScr.phuban_Info.pointTeam2 * num2 / GameScr.phuban_Info.maxPoint;
				bool flag15 = num13 < 0;
				if (flag15)
				{
					num13 = 0;
				}
				bool flag16 = num13 > num2;
				if (flag16)
				{
					num13 = num2;
				}
				g.setClip(num4, num5, num13, frameHeight);
				for (int k = 0; k < num7; k++)
				{
					bool flag17 = k < num7 - 1;
					if (flag17)
					{
						GameScr.fra_PVE_Bar_0.drawFrame(num12, num4 + k * GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
					}
					else
					{
						GameScr.fra_PVE_Bar_0.drawFrame(num12, num4 + num6 - GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
					}
				}
				GameScr.fra_PVE_Bar_0.drawFrame(num11, num4 + num6, num5, 0, 0, g);
				GameCanvas.resetTrans(g);
			}
			GameScr.fra_PVE_Bar_1.drawFrame(0, x - frameWidth / 2, y, 0, 0, g);
			string timeCountDown = mSystem.getTimeCountDown(GameScr.phuban_Info.timeStart, (int)GameScr.phuban_Info.timeSecond, true, false);
			mFont.tahoma_7b_yellow.drawString(g, timeCountDown, x + 1, y + GameScr.fra_PVE_Bar_1.frameHeight / 2 - mFont.tahoma_7b_green2.getHeight() / 2, 2);
			Panel.setTextColor(GameScr.phuban_Info.color_1, 1).drawString(g, GameScr.phuban_Info.nameTeam1, x - 5, num + 5, 1);
			Panel.setTextColor(GameScr.phuban_Info.color_2, 1).drawString(g, GameScr.phuban_Info.nameTeam2, x + 5, num + 5, 0);
			bool flag18 = GameScr.phuban_Info.type_PB != 0;
			if (flag18)
			{
				int num14 = y + frameHeight / 2 - 2;
				mFont.bigNumber_While.drawString(g, string.Empty + GameScr.phuban_Info.pointTeam1.ToString(), num3 + num2 / 2, num14, 2);
				mFont.bigNumber_While.drawString(g, string.Empty + GameScr.phuban_Info.pointTeam2.ToString(), num4 + num2 / 2, num14, 2);
			}
			g.drawImage(GameScr.imgVS, x, y + GameScr.fra_PVE_Bar_1.frameHeight + 2, 3);
			bool flag19 = GameScr.phuban_Info.type_PB == 0;
			if (flag19)
			{
				GameScr.paintChienTruong_Life(g, GameScr.phuban_Info.maxLife, GameScr.phuban_Info.color_1, GameScr.phuban_Info.lifeTeam1, x - 13, GameScr.phuban_Info.color_2, GameScr.phuban_Info.lifeTeam2, x + 13, num);
			}
		}
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x00063B1C File Offset: 0x00061D1C
	public static void paintChienTruong_Life(mGraphics g, int maxLife, int cl1, int lifeTeam1, int x1, int cl2, int lifeTeam2, int x2, int y)
	{
		bool flag = GameScr.imgBall == null;
		if (!flag)
		{
			int num = mGraphics.getImageHeight(GameScr.imgBall) / 2;
			for (int i = 0; i < maxLife; i++)
			{
				int num2 = 0;
				bool flag2 = i < lifeTeam1;
				if (flag2)
				{
					num2 = 1;
				}
				g.drawRegion(GameScr.imgBall, 0, num2 * num, mGraphics.getImageWidth(GameScr.imgBall), num, 0, x1 - i * (num + 1), y, mGraphics.VCENTER | mGraphics.HCENTER);
			}
			for (int j = 0; j < maxLife; j++)
			{
				int num3 = 0;
				bool flag3 = j < lifeTeam2;
				if (flag3)
				{
					num3 = 1;
				}
				g.drawRegion(GameScr.imgBall, 0, num3 * num, mGraphics.getImageWidth(GameScr.imgBall), num, 0, x2 + j * (num + 1), y, mGraphics.VCENTER | mGraphics.HCENTER);
			}
		}
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x00063C00 File Offset: 0x00061E00
	public static void paintHPBar_NEW(mGraphics g, int x, int y, global::Char c)
	{
		g.drawImage(GameScr.imgKhung, x, y, 0);
		int num = x + 3;
		int num2 = y + 19;
		int width = GameScr.imgHP_NEW.getWidth();
		int num3 = GameScr.imgHP_NEW.getHeight() / 2;
		int num4 = (int)(c.cHP * (long)width / c.cHPFull);
		bool flag = num4 <= 0;
		if (flag)
		{
			num4 = 1;
		}
		else
		{
			bool flag2 = num4 > width;
			if (flag2)
			{
				num4 = width;
			}
		}
		g.drawRegion(GameScr.imgHP_NEW, 0, num3, num4, num3, 0, num, num2, 0);
		int num5 = (int)(c.cMP * (long)width / c.cMPFull);
		bool flag3 = num5 <= 0;
		if (flag3)
		{
			num5 = 1;
		}
		else
		{
			bool flag4 = num5 > width;
			if (flag4)
			{
				num5 = width;
			}
		}
		g.drawRegion(GameScr.imgHP_NEW, 0, 0, num5, num3, 0, num, num2 + 6, 0);
		int num6 = x + GameScr.imgKhung.getWidth() / 2 + 1;
		int num7 = num2 + 13;
		mFont.tahoma_7_green2.drawString(g, c.cName, num6, y + 4, 2);
		bool flag5 = c.mobFocus != null;
		if (flag5)
		{
			bool flag6 = c.mobFocus.getTemplate() != null;
			if (flag6)
			{
				mFont.tahoma_7_green2.drawString(g, c.mobFocus.getTemplate().name, num6, num7, 2);
			}
		}
		else
		{
			bool flag7 = c.npcFocus != null;
			if (flag7)
			{
				mFont.tahoma_7_green2.drawString(g, c.npcFocus.template.name, num6, num7, 2);
			}
			else
			{
				bool flag8 = c.charFocus != null;
				if (flag8)
				{
					mFont.tahoma_7_green2.drawString(g, c.charFocus.cName, num6, num7, 2);
				}
			}
		}
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x00063DB8 File Offset: 0x00061FB8
	public static void addEffectEnd(int type, int subtype, int typePaint, int x, int y, int levelPaint, int dir, short timeRemove, Point[] listObj)
	{
		Effect_End effect_End = new Effect_End(type, subtype, typePaint, x, y, levelPaint, dir, timeRemove, listObj);
		GameScr.addEffect2Vector(effect_End);
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x00063DE4 File Offset: 0x00061FE4
	public static void addEffectEnd_Target(int type, int subtype, int typePaint, global::Char charUse, Point target, int levelPaint, short timeRemove, short range)
	{
		Effect_End effect_End = new Effect_End(type, subtype, typePaint, charUse.clone(), target, levelPaint, timeRemove, range);
		GameScr.addEffect2Vector(effect_End);
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x00063E10 File Offset: 0x00062010
	public static void addEffect2Vector(Effect_End eff)
	{
		bool flag = eff.levelPaint == 0;
		if (flag)
		{
			EffectManager.addHiEffect(eff);
		}
		else
		{
			bool flag2 = eff.levelPaint == 1;
			if (flag2)
			{
				EffectManager.addMidEffects(eff);
			}
			else
			{
				bool flag3 = eff.levelPaint == 2;
				if (flag3)
				{
					EffectManager.addMid_2Effects(eff);
				}
				else
				{
					EffectManager.addLowEffect(eff);
				}
			}
		}
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x00063E70 File Offset: 0x00062070
	public static bool setIsInScreen(int x, int y, int wOne, int hOne)
	{
		bool flag = x < GameScr.cmx - wOne || x > GameScr.cmx + GameCanvas.w + wOne || y < GameScr.cmy - hOne || y > GameScr.cmy + GameCanvas.h + hOne * 3 / 2;
		return !flag;
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x00063EC8 File Offset: 0x000620C8
	public static bool isSmallScr()
	{
		return GameCanvas.w <= 320;
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x00063EF4 File Offset: 0x000620F4
	private void paint_xp_bar(mGraphics g)
	{
		g.setColor(8421504);
		g.fillRect(0, GameCanvas.h - 2, GameCanvas.w, 2);
		int num = (int)(global::Char.myCharz().cLevelPercent * (long)GameCanvas.w / 10000L);
		g.setColor(16777215);
		g.fillRect(0, GameCanvas.h - 2, num, 2);
		g.setColor(0);
		num = GameCanvas.w / 10;
		for (int i = 1; i < 10; i++)
		{
			g.fillRect(i * num, GameCanvas.h - 2, 1, 2);
		}
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x00063F94 File Offset: 0x00062194
	private void paint_ios_bg(mGraphics g)
	{
		bool flag = mSystem.clientType == 5;
		if (flag)
		{
			bool flag2 = GameScr.imgBgIOS != null;
			if (flag2)
			{
				g.setColor(16777215);
				g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
				g.drawImage(GameScr.imgBgIOS, GameCanvas.w / 2, GameCanvas.h / 2, mGraphics.VCENTER | mGraphics.HCENTER);
			}
			else
			{
				GameScr.imgBgIOS = GameCanvas.loadImage("/bg/bg_ios_" + ((TileMap.bgID % 2 != 0) ? 1 : 2).ToString() + ".png");
			}
		}
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x00064038 File Offset: 0x00062238
	public void paint_CT(mGraphics g, int x, int y, int w)
	{
		w = 194;
		w = 182;
		w = 170;
		int num = 66;
		int num2 = 11;
		bool flag = x > GameCanvas.w - w / 2;
		if (flag)
		{
			x = GameCanvas.w - w / 2;
		}
		bool flag2 = x < mGraphics.getImageWidth(GameScr.imgKhung) + w / 2 + 10;
		if (flag2)
		{
			x = mGraphics.getImageWidth(GameScr.imgKhung) + w / 2 + 10;
		}
		int frameHeight = GameScr.fra_PVE_Bar_0.frameHeight;
		int num3 = y + frameHeight + mGraphics.getImageHeight(GameScr.imgBall) / 2 + 2;
		int frameWidth = GameScr.fra_PVE_Bar_1.frameWidth;
		int num4 = w / 2 - frameWidth / 2;
		int num5 = x - w / 2 + 3;
		int num6 = x + frameWidth / 2;
		int num7 = y + 3;
		int num8 = num4 - GameScr.fra_PVE_Bar_0.frameWidth;
		int num9 = num8 / GameScr.fra_PVE_Bar_0.frameWidth;
		bool flag3 = num8 % GameScr.fra_PVE_Bar_0.frameWidth > 0;
		if (flag3)
		{
			num9++;
		}
		for (int i = 0; i < num9; i++)
		{
			bool flag4 = i < num9 - 1;
			if (flag4)
			{
				g.drawRegion(GameScr.img_ct_bar_0, 0, 15, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num5 + GameScr.fra_PVE_Bar_0.frameWidth + i * GameScr.fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, true);
			}
			else
			{
				g.drawRegion(GameScr.img_ct_bar_0, 0, 15, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num5 + num8, num7, mGraphics.TOP | mGraphics.LEFT, true);
			}
			bool flag5 = i < num9 - 1;
			if (flag5)
			{
				g.drawRegion(GameScr.img_ct_bar_0, 0, 15, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num6 + i * GameScr.fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, true);
			}
			else
			{
				g.drawRegion(GameScr.img_ct_bar_0, 0, 15, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num6 + num8 - GameScr.fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, true);
			}
		}
		GameScr.fra_PVE_Bar_0.drawFrame(0, num5, num7, 2, 0, g);
		GameScr.fra_PVE_Bar_0.drawFrame(0, num6 + num8, num7, 0, 0, g);
		int num10 = GameScr.nCT_TeamA * 100 / (GameScr.nCT_nBoyBaller / 2) * num / 100;
		bool flag6 = num10 > 0;
		if (flag6)
		{
			bool flag7 = num10 < 6;
			if (flag7)
			{
				num10 = 6;
			}
			g.setClip(num5, num7, num10, 15);
		}
		bool flag8 = GameScr.nCT_TeamA > 0;
		if (flag8)
		{
			for (int j = 0; j < num2; j++)
			{
				bool flag9 = j == 0;
				if (flag9)
				{
					g.drawRegion(GameScr.img_ct_bar_0, 0, 60, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num5, num7, mGraphics.TOP | mGraphics.LEFT, true);
				}
				else
				{
					g.drawRegion(GameScr.img_ct_bar_0, 0, 75, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num5 + j * 6, num7, mGraphics.TOP | mGraphics.LEFT, true);
				}
			}
		}
		GameCanvas.resetTrans(g);
		int num11 = GameScr.nCT_TeamB * 100 / (GameScr.nCT_nBoyBaller / 2) * num / 100;
		bool flag10 = num - (num - num11) > 0;
		if (flag10)
		{
			bool flag11 = num11 < 6;
			if (flag11)
			{
				num11 = 6;
			}
			g.setClip(num6 + num - num11, num7, num - (num - num11), 15);
		}
		bool flag12 = GameScr.nCT_TeamB > 0;
		if (flag12)
		{
			for (int k = 0; k < num2; k++)
			{
				bool flag13 = k == 0;
				if (flag13)
				{
					g.drawRegion(GameScr.img_ct_bar_0, 0, 30, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 0, num6 + num8, num7, mGraphics.TOP | mGraphics.LEFT, true);
				}
				else
				{
					g.drawRegion(GameScr.img_ct_bar_0, 0, 45, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 0, num6 + num8 - k * 6, num7, mGraphics.TOP | mGraphics.LEFT, true);
				}
			}
		}
		GameCanvas.resetTrans(g);
		GameScr.fra_PVE_Bar_1.drawFrame(0, x - frameWidth / 2 + 1, y, 0, 0, g);
		string text = NinjaUtil.getTime((int)((GameScr.nCT_timeBallte - mSystem.currentTimeMillis()) / 1000L)) + string.Empty;
		mFont.tahoma_7b_yellow.drawString(g, text, num5 + w / 2 - 2, y + 5, 2);
		mFont.tahoma_7_grey.drawString(g, "Tầng " + GameScr.nCT_floor.ToString(), num5 + w / 2 - 3, y + GameScr.fra_PVE_Bar_1.frameHeight, mFont.CENTER);
		int num12 = mFont.tahoma_7b_red.getWidth(GameScr.nCT_TeamA.ToString() + string.Empty);
		mFont.tahoma_7b_blue.drawString(g, GameScr.nCT_TeamA.ToString() + string.Empty, x - frameWidth / 2 - num12, num7 + GameScr.fra_PVE_Bar_1.frameHeight, 0);
		SmallImage.drawSmallImage(g, 2325, x - frameWidth / 2 - num12 - 15, num7 + GameScr.fra_PVE_Bar_1.frameHeight, 2, mGraphics.TOP | mGraphics.LEFT);
		num12 = mFont.tahoma_7b_red.getWidth(GameScr.nCT_TeamB.ToString() + string.Empty);
		mFont.tahoma_7b_red.drawString(g, GameScr.nCT_TeamB.ToString() + string.Empty, x + frameWidth / 2, num7 + GameScr.fra_PVE_Bar_1.frameHeight, 0);
		SmallImage.drawSmallImage(g, 2323, x + frameWidth / 2 + num12 + 3, num7 + GameScr.fra_PVE_Bar_1.frameHeight, 0, mGraphics.TOP | mGraphics.LEFT);
		this.paint_board_CT(g, GameCanvas.w - mFont.tahoma_7b_dark.getWidth("#01 AAAAAAAAAA"), 40);
		GameCanvas.resetTrans(g);
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x00064638 File Offset: 0x00062838
	private void paint_board_CT(mGraphics g, int x, int y)
	{
		bool flag = !GameScr.is_Paint_boardCT_Expand;
		if (flag)
		{
			string text = "#01 nnnnnnnnnnnn";
			int width = mFont.tahoma_7.getWidth(text);
			int num = GameCanvas.w - width - 20;
			for (int i = 0; i < GameScr.nTop; i++)
			{
				mFont mFont = mFont.tahoma_7_white;
				switch (i)
				{
				case 0:
					mFont = mFont.tahoma_7_red;
					break;
				case 1:
					mFont = mFont.tahoma_7_yellow;
					break;
				case 2:
					mFont = mFont.tahoma_7_blue;
					break;
				}
				bool flag2 = i == GameScr.nTop - 1;
				if (flag2)
				{
					mFont = mFont.tahoma_7_green;
				}
				string[] array = Res.split((string)GameScr.res_CT.elementAt(i), "|", 0);
				int[] array2 = new int[] { 0, 18 };
				for (int j = 0; j < 2; j++)
				{
					mFont.drawString(g, array[j], num + array2[j], y + i * mFont.tahoma_7.getHeight(), 0, mFont.tahoma_7);
				}
			}
			GameCanvas.resetTrans(g);
			GameScr.xRect = num;
			GameScr.yRect = y;
			GameScr.wRect = width + 10;
			GameScr.hRect = mFont.tahoma_7b_dark.getHeight() * 6;
		}
		else
		{
			string text2 = "#01 namec1000000 0001   00000";
			int[] array3 = new int[] { 0, 18, 80, 101 };
			int width2 = mFont.tahoma_7.getWidth(text2);
			int num2 = GameCanvas.w - width2 - 20;
			for (int k = 0; k < GameScr.nTop; k++)
			{
				string[] array4 = Res.split((string)GameScr.res_CT.elementAt(k), "|", 0);
				mFont mFont2 = mFont.tahoma_7_white;
				switch (k)
				{
				case 0:
					mFont2 = mFont.tahoma_7_red;
					break;
				case 1:
					mFont2 = mFont.tahoma_7_yellow;
					break;
				case 2:
					mFont2 = mFont.tahoma_7_blue;
					break;
				}
				bool flag3 = k == GameScr.nTop - 1;
				if (flag3)
				{
					mFont2 = mFont.tahoma_7_green;
				}
				int num3 = k * mFont.tahoma_7_white.getHeight() + y;
				for (int l = 0; l < array3.Length; l++)
				{
					mFont2.drawString(g, array4[l], num2 + array3[l], num3, 0, mFont.tahoma_7);
				}
			}
			GameScr.xRect = num2;
			GameScr.yRect = y;
			GameScr.wRect = width2 + 10;
			GameScr.hRect = mFont.tahoma_7b_dark.getHeight() * 6;
		}
		GameCanvas.resetTrans(g);
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x000648DC File Offset: 0x00062ADC
	private void paintHPCT(mGraphics g, int x, int y, global::Char c)
	{
		g.drawImage(GameScr.imgKhung, x, y, 0);
		int num = x + 3;
		int num2 = y + 19;
		int width = GameScr.imgHP_NEW.getWidth();
		int num3 = GameScr.imgHP_NEW.getHeight() / 2;
		int num4 = (int)(c.cHP * (long)width / c.cHPFull);
		bool flag = num4 <= 0;
		if (!flag)
		{
			bool flag2 = num4 > width;
			if (flag2)
			{
			}
		}
		g.drawRegion(GameScr.imgHP_NEW, 0, num3, 80, num3, 0, num, num2, 0);
		int num5 = (int)(c.cMP * (long)width / c.cMPFull);
		bool flag3 = num5 <= 0;
		if (!flag3)
		{
			bool flag4 = num5 > width;
			if (flag4)
			{
			}
		}
		g.drawRegion(GameScr.imgHP_NEW, 0, 0, 80, num3, 0, num, num2 + 6, 0);
	}

	// Token: 0x0400078F RID: 1935
	public bool isWaitingDoubleClick;

	// Token: 0x04000790 RID: 1936
	public long timeStartDblClick;

	// Token: 0x04000791 RID: 1937
	public long timeEndDblClick;

	// Token: 0x04000792 RID: 1938
	public static bool isPaintOther = false;

	// Token: 0x04000793 RID: 1939
	public static MyVector textTime = new MyVector(string.Empty);

	// Token: 0x04000794 RID: 1940
	public static bool isLoadAllData = false;

	// Token: 0x04000795 RID: 1941
	public static GameScr instance;

	// Token: 0x04000796 RID: 1942
	public static int gW;

	// Token: 0x04000797 RID: 1943
	public static int gH;

	// Token: 0x04000798 RID: 1944
	public static int gW2;

	// Token: 0x04000799 RID: 1945
	public static int gssw;

	// Token: 0x0400079A RID: 1946
	public static int gssh;

	// Token: 0x0400079B RID: 1947
	public static int gH34;

	// Token: 0x0400079C RID: 1948
	public static int gW3;

	// Token: 0x0400079D RID: 1949
	public static int gH3;

	// Token: 0x0400079E RID: 1950
	public static int gH23;

	// Token: 0x0400079F RID: 1951
	public static int gW23;

	// Token: 0x040007A0 RID: 1952
	public static int gH2;

	// Token: 0x040007A1 RID: 1953
	public static int csPadMaxH;

	// Token: 0x040007A2 RID: 1954
	public static int cmdBarH;

	// Token: 0x040007A3 RID: 1955
	public static int gW34;

	// Token: 0x040007A4 RID: 1956
	public static int gW6;

	// Token: 0x040007A5 RID: 1957
	public static int gH6;

	// Token: 0x040007A6 RID: 1958
	public static int cmx;

	// Token: 0x040007A7 RID: 1959
	public static int cmy;

	// Token: 0x040007A8 RID: 1960
	public static int cmdx;

	// Token: 0x040007A9 RID: 1961
	public static int cmdy;

	// Token: 0x040007AA RID: 1962
	public static int cmvx;

	// Token: 0x040007AB RID: 1963
	public static int cmvy;

	// Token: 0x040007AC RID: 1964
	public static int cmtoX;

	// Token: 0x040007AD RID: 1965
	public static int cmtoY;

	// Token: 0x040007AE RID: 1966
	public static int cmxLim;

	// Token: 0x040007AF RID: 1967
	public static int cmyLim;

	// Token: 0x040007B0 RID: 1968
	public static int gssx;

	// Token: 0x040007B1 RID: 1969
	public static int gssy;

	// Token: 0x040007B2 RID: 1970
	public static int gssxe;

	// Token: 0x040007B3 RID: 1971
	public static int gssye;

	// Token: 0x040007B4 RID: 1972
	public Command cmdback;

	// Token: 0x040007B5 RID: 1973
	public Command cmdBag;

	// Token: 0x040007B6 RID: 1974
	public Command cmdSkill;

	// Token: 0x040007B7 RID: 1975
	public Command cmdTiemnang;

	// Token: 0x040007B8 RID: 1976
	public Command cmdtrangbi;

	// Token: 0x040007B9 RID: 1977
	public Command cmdInfo;

	// Token: 0x040007BA RID: 1978
	public Command cmdFocus;

	// Token: 0x040007BB RID: 1979
	public Command cmdFire;

	// Token: 0x040007BC RID: 1980
	public static int d;

	// Token: 0x040007BD RID: 1981
	public static int hpPotion;

	// Token: 0x040007BE RID: 1982
	public static SkillPaint[] sks;

	// Token: 0x040007BF RID: 1983
	public static Arrowpaint[] arrs;

	// Token: 0x040007C0 RID: 1984
	public static DartInfo[] darts;

	// Token: 0x040007C1 RID: 1985
	public static Part[] parts;

	// Token: 0x040007C2 RID: 1986
	public static EffectCharPaint[] efs;

	// Token: 0x040007C3 RID: 1987
	public static int lockTick;

	// Token: 0x040007C4 RID: 1988
	private int moveUp;

	// Token: 0x040007C5 RID: 1989
	private int moveDow;

	// Token: 0x040007C6 RID: 1990
	private int idTypeTask;

	// Token: 0x040007C7 RID: 1991
	private bool isstarOpen;

	// Token: 0x040007C8 RID: 1992
	private bool isChangeSkill;

	// Token: 0x040007C9 RID: 1993
	public static MyVector vClan = new MyVector();

	// Token: 0x040007CA RID: 1994
	public static MyVector vPtMap = new MyVector();

	// Token: 0x040007CB RID: 1995
	public static MyVector vFriend = new MyVector();

	// Token: 0x040007CC RID: 1996
	public static MyVector vEnemies = new MyVector();

	// Token: 0x040007CD RID: 1997
	public static MyVector vCharInMap = new MyVector();

	// Token: 0x040007CE RID: 1998
	public static MyVector vItemMap = new MyVector();

	// Token: 0x040007CF RID: 1999
	public static MyVector vMobAttack = new MyVector();

	// Token: 0x040007D0 RID: 2000
	public static MyVector vSet = new MyVector();

	// Token: 0x040007D1 RID: 2001
	public static MyVector vMob = new MyVector();

	// Token: 0x040007D2 RID: 2002
	public static MyVector vNpc = new MyVector();

	// Token: 0x040007D3 RID: 2003
	public static MyVector vFlag = new MyVector();

	// Token: 0x040007D4 RID: 2004
	public static NClass[] nClasss;

	// Token: 0x040007D5 RID: 2005
	public static int indexSize = 28;

	// Token: 0x040007D6 RID: 2006
	public static int indexTitle = 0;

	// Token: 0x040007D7 RID: 2007
	public static int indexSelect = 0;

	// Token: 0x040007D8 RID: 2008
	public static int indexRow = -1;

	// Token: 0x040007D9 RID: 2009
	public static int indexRowMax;

	// Token: 0x040007DA RID: 2010
	public static int indexMenu = 0;

	// Token: 0x040007DB RID: 2011
	public Item itemFocus;

	// Token: 0x040007DC RID: 2012
	public ItemOptionTemplate[] iOptionTemplates;

	// Token: 0x040007DD RID: 2013
	public SkillOptionTemplate[] sOptionTemplates;

	// Token: 0x040007DE RID: 2014
	private static Scroll scrInfo = new Scroll();

	// Token: 0x040007DF RID: 2015
	public static Scroll scrMain = new Scroll();

	// Token: 0x040007E0 RID: 2016
	public static MyVector vItemUpGrade = new MyVector();

	// Token: 0x040007E1 RID: 2017
	public static bool isTypeXu;

	// Token: 0x040007E2 RID: 2018
	public static bool isViewNext;

	// Token: 0x040007E3 RID: 2019
	public static bool isViewClanMemOnline = false;

	// Token: 0x040007E4 RID: 2020
	public static bool isViewClanInvite = true;

	// Token: 0x040007E5 RID: 2021
	public static bool isChop;

	// Token: 0x040007E6 RID: 2022
	public static string titleInputText = string.Empty;

	// Token: 0x040007E7 RID: 2023
	public static int tickMove;

	// Token: 0x040007E8 RID: 2024
	public static bool isPaintAlert = false;

	// Token: 0x040007E9 RID: 2025
	public static bool isPaintTask = false;

	// Token: 0x040007EA RID: 2026
	public static bool isPaintTeam = false;

	// Token: 0x040007EB RID: 2027
	public static bool isPaintFindTeam = false;

	// Token: 0x040007EC RID: 2028
	public static bool isPaintFriend = false;

	// Token: 0x040007ED RID: 2029
	public static bool isPaintEnemies = false;

	// Token: 0x040007EE RID: 2030
	public static bool isPaintItemInfo = false;

	// Token: 0x040007EF RID: 2031
	public static bool isHaveSelectSkill = false;

	// Token: 0x040007F0 RID: 2032
	public static bool isPaintSkill = false;

	// Token: 0x040007F1 RID: 2033
	public static bool isPaintInfoMe = false;

	// Token: 0x040007F2 RID: 2034
	public static bool isPaintStore = false;

	// Token: 0x040007F3 RID: 2035
	public static bool isPaintNonNam = false;

	// Token: 0x040007F4 RID: 2036
	public static bool isPaintNonNu = false;

	// Token: 0x040007F5 RID: 2037
	public static bool isPaintAoNam = false;

	// Token: 0x040007F6 RID: 2038
	public static bool isPaintAoNu = false;

	// Token: 0x040007F7 RID: 2039
	public static bool isPaintGangTayNam = false;

	// Token: 0x040007F8 RID: 2040
	public static bool isPaintGangTayNu = false;

	// Token: 0x040007F9 RID: 2041
	public static bool isPaintQuanNam = false;

	// Token: 0x040007FA RID: 2042
	public static bool isPaintQuanNu = false;

	// Token: 0x040007FB RID: 2043
	public static bool isPaintGiayNam = false;

	// Token: 0x040007FC RID: 2044
	public static bool isPaintGiayNu = false;

	// Token: 0x040007FD RID: 2045
	public static bool isPaintLien = false;

	// Token: 0x040007FE RID: 2046
	public static bool isPaintNhan = false;

	// Token: 0x040007FF RID: 2047
	public static bool isPaintNgocBoi = false;

	// Token: 0x04000800 RID: 2048
	public static bool isPaintPhu = false;

	// Token: 0x04000801 RID: 2049
	public static bool isPaintWeapon = false;

	// Token: 0x04000802 RID: 2050
	public static bool isPaintStack = false;

	// Token: 0x04000803 RID: 2051
	public static bool isPaintStackLock = false;

	// Token: 0x04000804 RID: 2052
	public static bool isPaintGrocery = false;

	// Token: 0x04000805 RID: 2053
	public static bool isPaintGroceryLock = false;

	// Token: 0x04000806 RID: 2054
	public static bool isPaintUpGrade = false;

	// Token: 0x04000807 RID: 2055
	public static bool isPaintConvert = false;

	// Token: 0x04000808 RID: 2056
	public static bool isPaintUpGradeGold = false;

	// Token: 0x04000809 RID: 2057
	public static bool isPaintUpPearl = false;

	// Token: 0x0400080A RID: 2058
	public static bool isPaintBox = false;

	// Token: 0x0400080B RID: 2059
	public static bool isPaintSplit = false;

	// Token: 0x0400080C RID: 2060
	public static bool isPaintCharInMap = false;

	// Token: 0x0400080D RID: 2061
	public static bool isPaintTrade = false;

	// Token: 0x0400080E RID: 2062
	public static bool isPaintZone = false;

	// Token: 0x0400080F RID: 2063
	public static bool isPaintMessage = false;

	// Token: 0x04000810 RID: 2064
	public static bool isPaintClan = false;

	// Token: 0x04000811 RID: 2065
	public static bool isRequestMember = false;

	// Token: 0x04000812 RID: 2066
	public static global::Char currentCharViewInfo;

	// Token: 0x04000813 RID: 2067
	public static long[] exps;

	// Token: 0x04000814 RID: 2068
	public static int[] crystals;

	// Token: 0x04000815 RID: 2069
	public static int[] upClothe;

	// Token: 0x04000816 RID: 2070
	public static int[] upAdorn;

	// Token: 0x04000817 RID: 2071
	public static int[] upWeapon;

	// Token: 0x04000818 RID: 2072
	public static int[] coinUpCrystals;

	// Token: 0x04000819 RID: 2073
	public static int[] coinUpClothes;

	// Token: 0x0400081A RID: 2074
	public static int[] coinUpAdorns;

	// Token: 0x0400081B RID: 2075
	public static int[] coinUpWeapons;

	// Token: 0x0400081C RID: 2076
	public static int[] maxPercents;

	// Token: 0x0400081D RID: 2077
	public static int[] goldUps;

	// Token: 0x0400081E RID: 2078
	public int tMenuDelay;

	// Token: 0x0400081F RID: 2079
	public int zoneCol = 6;

	// Token: 0x04000820 RID: 2080
	public int[] zones;

	// Token: 0x04000821 RID: 2081
	public int[] pts;

	// Token: 0x04000822 RID: 2082
	public int[] numPlayer;

	// Token: 0x04000823 RID: 2083
	public int[] maxPlayer;

	// Token: 0x04000824 RID: 2084
	public int[] rank1;

	// Token: 0x04000825 RID: 2085
	public int[] rank2;

	// Token: 0x04000826 RID: 2086
	public string[] rankName1;

	// Token: 0x04000827 RID: 2087
	public string[] rankName2;

	// Token: 0x04000828 RID: 2088
	public int typeTrade;

	// Token: 0x04000829 RID: 2089
	public int typeTradeOrder;

	// Token: 0x0400082A RID: 2090
	public int coinTrade;

	// Token: 0x0400082B RID: 2091
	public int coinTradeOrder;

	// Token: 0x0400082C RID: 2092
	public int timeTrade;

	// Token: 0x0400082D RID: 2093
	public int indexItemUse = -1;

	// Token: 0x0400082E RID: 2094
	public int cLastFocusID = -1;

	// Token: 0x0400082F RID: 2095
	public int cPreFocusID = -1;

	// Token: 0x04000830 RID: 2096
	public bool isLockKey;

	// Token: 0x04000831 RID: 2097
	public static int[] tasks;

	// Token: 0x04000832 RID: 2098
	public static int[] mapTasks;

	// Token: 0x04000833 RID: 2099
	public static Image imgRoomStat;

	// Token: 0x04000834 RID: 2100
	public static Image frBarPow0;

	// Token: 0x04000835 RID: 2101
	public static Image frBarPow1;

	// Token: 0x04000836 RID: 2102
	public static Image frBarPow2;

	// Token: 0x04000837 RID: 2103
	public static Image frBarPow20;

	// Token: 0x04000838 RID: 2104
	public static Image frBarPow21;

	// Token: 0x04000839 RID: 2105
	public static Image frBarPow22;

	// Token: 0x0400083A RID: 2106
	public MyVector texts;

	// Token: 0x0400083B RID: 2107
	public string textsTitle;

	// Token: 0x0400083C RID: 2108
	public static sbyte vcData;

	// Token: 0x0400083D RID: 2109
	public static sbyte vcMap;

	// Token: 0x0400083E RID: 2110
	public static sbyte vcSkill;

	// Token: 0x0400083F RID: 2111
	public static sbyte vcItem;

	// Token: 0x04000840 RID: 2112
	public static sbyte vsData;

	// Token: 0x04000841 RID: 2113
	public static sbyte vsMap;

	// Token: 0x04000842 RID: 2114
	public static sbyte vsSkill;

	// Token: 0x04000843 RID: 2115
	public static sbyte vsItem;

	// Token: 0x04000844 RID: 2116
	public static sbyte vcTask;

	// Token: 0x04000845 RID: 2117
	public static Image imgArrow;

	// Token: 0x04000846 RID: 2118
	public static Image imgArrow2;

	// Token: 0x04000847 RID: 2119
	public static Image imgChat;

	// Token: 0x04000848 RID: 2120
	public static Image imgChat2;

	// Token: 0x04000849 RID: 2121
	public static Image imgMenu;

	// Token: 0x0400084A RID: 2122
	public static Image imgFocus;

	// Token: 0x0400084B RID: 2123
	public static Image imgFocus2;

	// Token: 0x0400084C RID: 2124
	public static Image imgSkill;

	// Token: 0x0400084D RID: 2125
	public static Image imgSkill2;

	// Token: 0x0400084E RID: 2126
	public static Image imgHP1;

	// Token: 0x0400084F RID: 2127
	public static Image imgHP2;

	// Token: 0x04000850 RID: 2128
	public static Image imgHP3;

	// Token: 0x04000851 RID: 2129
	public static Image imgHP4;

	// Token: 0x04000852 RID: 2130
	public static Image imgFire0;

	// Token: 0x04000853 RID: 2131
	public static Image imgFire1;

	// Token: 0x04000854 RID: 2132
	public static Image imgNR1;

	// Token: 0x04000855 RID: 2133
	public static Image imgNR2;

	// Token: 0x04000856 RID: 2134
	public static Image imgNR3;

	// Token: 0x04000857 RID: 2135
	public static Image imgNR4;

	// Token: 0x04000858 RID: 2136
	public static Image imgLbtn;

	// Token: 0x04000859 RID: 2137
	public static Image imgLbtnFocus;

	// Token: 0x0400085A RID: 2138
	public static Image imgLbtn2;

	// Token: 0x0400085B RID: 2139
	public static Image imgLbtnFocus2;

	// Token: 0x0400085C RID: 2140
	public static Image imgAnalog1;

	// Token: 0x0400085D RID: 2141
	public static Image imgAnalog2;

	// Token: 0x0400085E RID: 2142
	public string tradeName = string.Empty;

	// Token: 0x0400085F RID: 2143
	public string tradeItemName = string.Empty;

	// Token: 0x04000860 RID: 2144
	public int timeLengthMap;

	// Token: 0x04000861 RID: 2145
	public int timeStartMap;

	// Token: 0x04000862 RID: 2146
	public static sbyte typeViewInfo = 0;

	// Token: 0x04000863 RID: 2147
	public static sbyte typeActive = 0;

	// Token: 0x04000864 RID: 2148
	public static InfoMe info1 = new InfoMe();

	// Token: 0x04000865 RID: 2149
	public static InfoMe info2 = new InfoMe();

	// Token: 0x04000866 RID: 2150
	public static Image imgPanel;

	// Token: 0x04000867 RID: 2151
	public static Image imgPanel2;

	// Token: 0x04000868 RID: 2152
	public static Image imgHP;

	// Token: 0x04000869 RID: 2153
	public static Image imgMP;

	// Token: 0x0400086A RID: 2154
	public static Image imgSP;

	// Token: 0x0400086B RID: 2155
	public static Image imgHPLost;

	// Token: 0x0400086C RID: 2156
	public static Image imgMPLost;

	// Token: 0x0400086D RID: 2157
	public static Image imgHP_tm_do;

	// Token: 0x0400086E RID: 2158
	public static Image imgHP_tm_vang;

	// Token: 0x0400086F RID: 2159
	public static Image imgHP_tm_xam;

	// Token: 0x04000870 RID: 2160
	public static Image imgHP_tm_xanh;

	// Token: 0x04000871 RID: 2161
	public static Image imgHP_tm_xanhnuocbien;

	// Token: 0x04000872 RID: 2162
	public Mob mobCapcha;

	// Token: 0x04000873 RID: 2163
	public MagicTree magicTree;

	// Token: 0x04000874 RID: 2164
	private short l;

	// Token: 0x04000875 RID: 2165
	public static int countEff;

	// Token: 0x04000876 RID: 2166
	public static GamePad gamePad = new GamePad();

	// Token: 0x04000877 RID: 2167
	public static Image imgChatPC;

	// Token: 0x04000878 RID: 2168
	public static Image imgChatsPC2;

	// Token: 0x04000879 RID: 2169
	public static int isAnalog = 0;

	// Token: 0x0400087A RID: 2170
	public static Image img_ct_bar_0 = mSystem.loadImage("/mainImage/i_pve_bar_0.png");

	// Token: 0x0400087B RID: 2171
	public static Image img_ct_bar_1 = mSystem.loadImage("/mainImage/i_pve_bar_1.png");

	// Token: 0x0400087C RID: 2172
	public static bool isUseTouch;

	// Token: 0x0400087D RID: 2173
	public Command cmdDoiCo;

	// Token: 0x0400087E RID: 2174
	public Command cmdLogOut;

	// Token: 0x0400087F RID: 2175
	public Command cmdChatTheGioi;

	// Token: 0x04000880 RID: 2176
	public Command cmdshowInfo;

	// Token: 0x04000881 RID: 2177
	private static Command[] cmdTestLogin = null;

	// Token: 0x04000882 RID: 2178
	public const int numSkill = 10;

	// Token: 0x04000883 RID: 2179
	public const int numSkill_2 = 5;

	// Token: 0x04000884 RID: 2180
	public static Skill[] keySkill = new Skill[10];

	// Token: 0x04000885 RID: 2181
	public static Skill[] onScreenSkill = new Skill[10];

	// Token: 0x04000886 RID: 2182
	public Command cmdMenu;

	// Token: 0x04000887 RID: 2183
	public static int firstY;

	// Token: 0x04000888 RID: 2184
	public static int wSkill;

	// Token: 0x04000889 RID: 2185
	public static long deltaTime;

	// Token: 0x0400088A RID: 2186
	public bool isPointerDowning;

	// Token: 0x0400088B RID: 2187
	public bool isChangingCameraMode;

	// Token: 0x0400088C RID: 2188
	private int ptLastDownX;

	// Token: 0x0400088D RID: 2189
	private int ptLastDownY;

	// Token: 0x0400088E RID: 2190
	private int ptFirstDownX;

	// Token: 0x0400088F RID: 2191
	private int ptFirstDownY;

	// Token: 0x04000890 RID: 2192
	private int ptDownTime;

	// Token: 0x04000891 RID: 2193
	private bool disableSingleClick;

	// Token: 0x04000892 RID: 2194
	public long lastSingleClick;

	// Token: 0x04000893 RID: 2195
	public bool clickMoving;

	// Token: 0x04000894 RID: 2196
	public bool clickOnTileTop;

	// Token: 0x04000895 RID: 2197
	public bool clickMovingRed;

	// Token: 0x04000896 RID: 2198
	private int clickToX;

	// Token: 0x04000897 RID: 2199
	private int clickToY;

	// Token: 0x04000898 RID: 2200
	private int lastClickCMX;

	// Token: 0x04000899 RID: 2201
	private int lastClickCMY;

	// Token: 0x0400089A RID: 2202
	private int clickMovingP1;

	// Token: 0x0400089B RID: 2203
	private int clickMovingTimeOut;

	// Token: 0x0400089C RID: 2204
	private long lastMove;

	// Token: 0x0400089D RID: 2205
	public static bool isNewClanMessage;

	// Token: 0x0400089E RID: 2206
	private long lastFire;

	// Token: 0x0400089F RID: 2207
	private long lastUsePotion;

	// Token: 0x040008A0 RID: 2208
	public int auto;

	// Token: 0x040008A1 RID: 2209
	public int dem;

	// Token: 0x040008A2 RID: 2210
	private string strTam = string.Empty;

	// Token: 0x040008A3 RID: 2211
	private int a;

	// Token: 0x040008A4 RID: 2212
	public bool isFreez;

	// Token: 0x040008A5 RID: 2213
	public bool isUseFreez;

	// Token: 0x040008A6 RID: 2214
	public static Image imgTrans;

	// Token: 0x040008A7 RID: 2215
	public bool isRongThanXuatHien;

	// Token: 0x040008A8 RID: 2216
	public bool isRongNamek;

	// Token: 0x040008A9 RID: 2217
	public bool isSuperPower;

	// Token: 0x040008AA RID: 2218
	public int tPower;

	// Token: 0x040008AB RID: 2219
	public int xPower;

	// Token: 0x040008AC RID: 2220
	public int yPower;

	// Token: 0x040008AD RID: 2221
	public int dxPower;

	// Token: 0x040008AE RID: 2222
	public bool activeRongThan;

	// Token: 0x040008AF RID: 2223
	public bool isMeCallRongThan;

	// Token: 0x040008B0 RID: 2224
	public int mautroi;

	// Token: 0x040008B1 RID: 2225
	public int mapRID;

	// Token: 0x040008B2 RID: 2226
	public int zoneRID;

	// Token: 0x040008B3 RID: 2227
	public int bgRID = -1;

	// Token: 0x040008B4 RID: 2228
	public static int tam = 0;

	// Token: 0x040008B5 RID: 2229
	public static bool isAutoPlay;

	// Token: 0x040008B6 RID: 2230
	public static bool canAutoPlay;

	// Token: 0x040008B7 RID: 2231
	public static bool isChangeZone;

	// Token: 0x040008B8 RID: 2232
	private int timeSkill;

	// Token: 0x040008B9 RID: 2233
	private int nSkill;

	// Token: 0x040008BA RID: 2234
	private int selectedIndexSkill = -1;

	// Token: 0x040008BB RID: 2235
	private Skill lastSkill;

	// Token: 0x040008BC RID: 2236
	private bool doSeleckSkillFlag;

	// Token: 0x040008BD RID: 2237
	public string strCapcha;

	// Token: 0x040008BE RID: 2238
	private long longPress;

	// Token: 0x040008BF RID: 2239
	private int move;

	// Token: 0x040008C0 RID: 2240
	public bool flareFindFocus;

	// Token: 0x040008C1 RID: 2241
	private int flareTime;

	// Token: 0x040008C2 RID: 2242
	public int keyTouchSkill = -1;

	// Token: 0x040008C3 RID: 2243
	private long lastSendUpdatePostion;

	// Token: 0x040008C4 RID: 2244
	public static long lastTick;

	// Token: 0x040008C5 RID: 2245
	public static long currTick;

	// Token: 0x040008C6 RID: 2246
	private int timeAuto;

	// Token: 0x040008C7 RID: 2247
	public static long lastXS;

	// Token: 0x040008C8 RID: 2248
	public static long currXS;

	// Token: 0x040008C9 RID: 2249
	public static int secondXS;

	// Token: 0x040008CA RID: 2250
	public int runArrow;

	// Token: 0x040008CB RID: 2251
	public static int isPaintRada;

	// Token: 0x040008CC RID: 2252
	public static Image imgNut;

	// Token: 0x040008CD RID: 2253
	public static Image imgNutF;

	// Token: 0x040008CE RID: 2254
	public int[] keyCapcha;

	// Token: 0x040008CF RID: 2255
	public static Image imgCapcha;

	// Token: 0x040008D0 RID: 2256
	public string keyInput;

	// Token: 0x040008D1 RID: 2257
	public static int disXC;

	// Token: 0x040008D2 RID: 2258
	public static bool isPaint = true;

	// Token: 0x040008D3 RID: 2259
	public static int shock_scr;

	// Token: 0x040008D4 RID: 2260
	private static int[] shock_x = new int[] { 1, -1, 1, -1 };

	// Token: 0x040008D5 RID: 2261
	private static int[] shock_y = new int[] { 1, -1, -1, 1 };

	// Token: 0x040008D6 RID: 2262
	private int tDoubleDelay;

	// Token: 0x040008D7 RID: 2263
	public static Image arrow;

	// Token: 0x040008D8 RID: 2264
	private static int yTouchBar;

	// Token: 0x040008D9 RID: 2265
	private static int xC;

	// Token: 0x040008DA RID: 2266
	private static int yC;

	// Token: 0x040008DB RID: 2267
	private static int xL;

	// Token: 0x040008DC RID: 2268
	private static int yL;

	// Token: 0x040008DD RID: 2269
	public int xR;

	// Token: 0x040008DE RID: 2270
	public int yR;

	// Token: 0x040008DF RID: 2271
	private static int xU;

	// Token: 0x040008E0 RID: 2272
	private static int yU;

	// Token: 0x040008E1 RID: 2273
	private static int xF;

	// Token: 0x040008E2 RID: 2274
	private static int yF;

	// Token: 0x040008E3 RID: 2275
	public static int xHP;

	// Token: 0x040008E4 RID: 2276
	public static int yHP;

	// Token: 0x040008E5 RID: 2277
	private static int xTG;

	// Token: 0x040008E6 RID: 2278
	private static int yTG;

	// Token: 0x040008E7 RID: 2279
	public static int[] xS;

	// Token: 0x040008E8 RID: 2280
	public static int[] yS;

	// Token: 0x040008E9 RID: 2281
	public static int xSkill;

	// Token: 0x040008EA RID: 2282
	public static int ySkill;

	// Token: 0x040008EB RID: 2283
	public static int padSkill;

	// Token: 0x040008EC RID: 2284
	public long dMP;

	// Token: 0x040008ED RID: 2285
	public long twMp;

	// Token: 0x040008EE RID: 2286
	public bool isInjureMp;

	// Token: 0x040008EF RID: 2287
	public long dHP;

	// Token: 0x040008F0 RID: 2288
	public long twHp;

	// Token: 0x040008F1 RID: 2289
	public bool isInjureHp;

	// Token: 0x040008F2 RID: 2290
	private long curr;

	// Token: 0x040008F3 RID: 2291
	private long last;

	// Token: 0x040008F4 RID: 2292
	private int secondVS;

	// Token: 0x040008F5 RID: 2293
	private int[] idVS = new int[] { -1, -1 };

	// Token: 0x040008F6 RID: 2294
	public static string[] flyTextString;

	// Token: 0x040008F7 RID: 2295
	public static int[] flyTextX;

	// Token: 0x040008F8 RID: 2296
	public static int[] flyTextY;

	// Token: 0x040008F9 RID: 2297
	public static int[] flyTextYTo;

	// Token: 0x040008FA RID: 2298
	public static int[] flyTextDx;

	// Token: 0x040008FB RID: 2299
	public static int[] flyTextDy;

	// Token: 0x040008FC RID: 2300
	public static int[] flyTextState;

	// Token: 0x040008FD RID: 2301
	public static int[] flyTextColor;

	// Token: 0x040008FE RID: 2302
	public static int[] flyTime;

	// Token: 0x040008FF RID: 2303
	public static int[] splashX;

	// Token: 0x04000900 RID: 2304
	public static int[] splashY;

	// Token: 0x04000901 RID: 2305
	public static int[] splashState;

	// Token: 0x04000902 RID: 2306
	public static int[] splashF;

	// Token: 0x04000903 RID: 2307
	public static int[] splashDir;

	// Token: 0x04000904 RID: 2308
	public static Image[] imgSplash;

	// Token: 0x04000905 RID: 2309
	public static int cmdBarX;

	// Token: 0x04000906 RID: 2310
	public static int cmdBarY;

	// Token: 0x04000907 RID: 2311
	public static int cmdBarW;

	// Token: 0x04000908 RID: 2312
	public static int cmdBarLeftW;

	// Token: 0x04000909 RID: 2313
	public static int cmdBarRightW;

	// Token: 0x0400090A RID: 2314
	public static int cmdBarCenterW;

	// Token: 0x0400090B RID: 2315
	public static int hpBarX;

	// Token: 0x0400090C RID: 2316
	public static int hpBarY;

	// Token: 0x0400090D RID: 2317
	public static int spBarW;

	// Token: 0x0400090E RID: 2318
	public static int mpBarW;

	// Token: 0x0400090F RID: 2319
	public static int expBarW;

	// Token: 0x04000910 RID: 2320
	public static int lvPosX;

	// Token: 0x04000911 RID: 2321
	public static int moneyPosX;

	// Token: 0x04000912 RID: 2322
	public static int hpBarH;

	// Token: 0x04000913 RID: 2323
	public static int girlHPBarY;

	// Token: 0x04000914 RID: 2324
	public static long hpBarW;

	// Token: 0x04000915 RID: 2325
	public static Image[] imgCmdBar;

	// Token: 0x04000916 RID: 2326
	private int imgScrW;

	// Token: 0x04000917 RID: 2327
	public static int popupY;

	// Token: 0x04000918 RID: 2328
	public static int popupX;

	// Token: 0x04000919 RID: 2329
	public static int isborderIndex;

	// Token: 0x0400091A RID: 2330
	public static int isselectedRow;

	// Token: 0x0400091B RID: 2331
	private static Image imgNolearn;

	// Token: 0x0400091C RID: 2332
	public int cmxp;

	// Token: 0x0400091D RID: 2333
	public int cmvxp;

	// Token: 0x0400091E RID: 2334
	public int cmdxp;

	// Token: 0x0400091F RID: 2335
	public int cmxLimp;

	// Token: 0x04000920 RID: 2336
	public int cmyLimp;

	// Token: 0x04000921 RID: 2337
	public int cmyp;

	// Token: 0x04000922 RID: 2338
	public int cmvyp;

	// Token: 0x04000923 RID: 2339
	public int cmdyp;

	// Token: 0x04000924 RID: 2340
	private int indexTiemNang;

	// Token: 0x04000925 RID: 2341
	private string alertURL;

	// Token: 0x04000926 RID: 2342
	private string fnick;

	// Token: 0x04000927 RID: 2343
	public static int xstart;

	// Token: 0x04000928 RID: 2344
	public static int ystart;

	// Token: 0x04000929 RID: 2345
	public static int popupW = 140;

	// Token: 0x0400092A RID: 2346
	public static int popupH = 160;

	// Token: 0x0400092B RID: 2347
	public static int cmySK;

	// Token: 0x0400092C RID: 2348
	public static int cmtoYSK;

	// Token: 0x0400092D RID: 2349
	public static int cmdySK;

	// Token: 0x0400092E RID: 2350
	public static int cmvySK;

	// Token: 0x0400092F RID: 2351
	public static int cmyLimSK;

	// Token: 0x04000930 RID: 2352
	public static int columns = 6;

	// Token: 0x04000931 RID: 2353
	public static int rows;

	// Token: 0x04000932 RID: 2354
	private int totalRowInfo;

	// Token: 0x04000933 RID: 2355
	private int ypaintKill;

	// Token: 0x04000934 RID: 2356
	private int ylimUp;

	// Token: 0x04000935 RID: 2357
	private int ylimDow;

	// Token: 0x04000936 RID: 2358
	private int yPaint;

	// Token: 0x04000937 RID: 2359
	public static int indexEff = 0;

	// Token: 0x04000938 RID: 2360
	public static EffectCharPaint effUpok;

	// Token: 0x04000939 RID: 2361
	public static int inforX;

	// Token: 0x0400093A RID: 2362
	public static int inforY;

	// Token: 0x0400093B RID: 2363
	public static int inforW;

	// Token: 0x0400093C RID: 2364
	public static int inforH;

	// Token: 0x0400093D RID: 2365
	public Command cmdDead;

	// Token: 0x0400093E RID: 2366
	public static bool notPaint = false;

	// Token: 0x0400093F RID: 2367
	public static bool isPing = false;

	// Token: 0x04000940 RID: 2368
	public static int INFO = 0;

	// Token: 0x04000941 RID: 2369
	public static int STORE = 1;

	// Token: 0x04000942 RID: 2370
	public static int ZONE = 2;

	// Token: 0x04000943 RID: 2371
	public static int UPGRADE = 3;

	// Token: 0x04000944 RID: 2372
	private int Hitem = 30;

	// Token: 0x04000945 RID: 2373
	private int maxSizeRow = 5;

	// Token: 0x04000946 RID: 2374
	private int isTranKyNang;

	// Token: 0x04000947 RID: 2375
	private bool isTran;

	// Token: 0x04000948 RID: 2376
	private int cmY_Old;

	// Token: 0x04000949 RID: 2377
	private int cmX_Old;

	// Token: 0x0400094A RID: 2378
	public PopUpYesNo popUpYesNo;

	// Token: 0x0400094B RID: 2379
	public static MyVector vChatVip = new MyVector();

	// Token: 0x0400094C RID: 2380
	public static int vBig;

	// Token: 0x0400094D RID: 2381
	public bool isFireWorks;

	// Token: 0x0400094E RID: 2382
	public int[] winnumber;

	// Token: 0x0400094F RID: 2383
	public int[] randomNumber;

	// Token: 0x04000950 RID: 2384
	public int[] tMove;

	// Token: 0x04000951 RID: 2385
	public int[] moveCount;

	// Token: 0x04000952 RID: 2386
	public int[] delayMove;

	// Token: 0x04000953 RID: 2387
	public int moveIndex;

	// Token: 0x04000954 RID: 2388
	private bool isWin;

	// Token: 0x04000955 RID: 2389
	private string strFinish;

	// Token: 0x04000956 RID: 2390
	private int tShow;

	// Token: 0x04000957 RID: 2391
	private int xChatVip;

	// Token: 0x04000958 RID: 2392
	private int currChatWidth;

	// Token: 0x04000959 RID: 2393
	private bool startChat;

	// Token: 0x0400095A RID: 2394
	public sbyte percentMabu;

	// Token: 0x0400095B RID: 2395
	public bool mabuEff;

	// Token: 0x0400095C RID: 2396
	public int tMabuEff;

	// Token: 0x0400095D RID: 2397
	public static bool isPaintChatVip;

	// Token: 0x0400095E RID: 2398
	public static sbyte mabuPercent;

	// Token: 0x0400095F RID: 2399
	public static sbyte isNewMember;

	// Token: 0x04000960 RID: 2400
	private string yourNumber = string.Empty;

	// Token: 0x04000961 RID: 2401
	private string[] strPaint;

	// Token: 0x04000962 RID: 2402
	public static Image imgHP_NEW;

	// Token: 0x04000963 RID: 2403
	public static InfoPhuBan phuban_Info;

	// Token: 0x04000964 RID: 2404
	public static FrameImage fra_PVE_Bar_0;

	// Token: 0x04000965 RID: 2405
	public static FrameImage fra_PVE_Bar_1;

	// Token: 0x04000966 RID: 2406
	public static Image imgVS;

	// Token: 0x04000967 RID: 2407
	public static Image imgBall;

	// Token: 0x04000968 RID: 2408
	public static Image imgKhung;

	// Token: 0x04000969 RID: 2409
	public int countFrameSkill;

	// Token: 0x0400096A RID: 2410
	public static Image imgBgIOS;

	// Token: 0x0400096B RID: 2411
	public static int nCT_TeamB = 50;

	// Token: 0x0400096C RID: 2412
	public static int nCT_TeamA = 50;

	// Token: 0x0400096D RID: 2413
	public static long nCT_timeBallte;

	// Token: 0x0400096E RID: 2414
	public static string nCT_team;

	// Token: 0x0400096F RID: 2415
	public static int nCT_nBoyBaller = 100;

	// Token: 0x04000970 RID: 2416
	public static bool isPaint_CT;

	// Token: 0x04000971 RID: 2417
	public static sbyte nCT_floor;

	// Token: 0x04000972 RID: 2418
	public static bool is_Paint_boardCT_Expand;

	// Token: 0x04000973 RID: 2419
	private static int xRect;

	// Token: 0x04000974 RID: 2420
	private static int yRect;

	// Token: 0x04000975 RID: 2421
	private static int wRect;

	// Token: 0x04000976 RID: 2422
	private static int hRect;

	// Token: 0x04000977 RID: 2423
	public static MyVector res_CT = new MyVector();

	// Token: 0x04000978 RID: 2424
	public static int nTop = 1;

	// Token: 0x04000979 RID: 2425
	public static bool isPickNgocRong = false;

	// Token: 0x0400097A RID: 2426
	public static int nUSER_CT;

	// Token: 0x0400097B RID: 2427
	public static int nUSER_MAX_CT;

	// Token: 0x0400097C RID: 2428
	public static bool isudungCapsun;

	// Token: 0x0400097D RID: 2429
	public static bool isudungCapsun4;

	// Token: 0x0400097E RID: 2430
	public static bool isudungCapsun3;

	// Token: 0x0400097F RID: 2431
	public static long timehoichieubuff;

	// Token: 0x04000980 RID: 2432
	public static long timehoikhien;

	// Token: 0x04000981 RID: 2433
	public static long timehoiskill3;

	// Token: 0x04000982 RID: 2434
	public static long timehoiskill9;

	// Token: 0x04000983 RID: 2435
	public static long timehoibom;

	// Token: 0x04000984 RID: 2436
	public static long timehoithoimien;

	// Token: 0x04000985 RID: 2437
	public static long ccc1;

	// Token: 0x04000986 RID: 2438
	public static long ccc3;

	// Token: 0x04000987 RID: 2439
	public static long ccc5;
}
using System;
using AssemblyCSharp.Mod.PickMob;
using Mod.DungPham.KoiOctiiu957;

// Token: 0x02000018 RID: 24
internal class KsSupper
{
	// Token: 0x060000A0 RID: 160 RVA: 0x0000ADF8 File Offset: 0x00008FF8
	public static bool IsBoss()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x0000AE6C File Offset: 0x0000906C
	public static void FocusSuperBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHP > 0L)
			{
				if (global::Char.myCharz().charFocus != @char)
				{
					global::Char.myCharz().npcFocus = null;
					global::Char.myCharz().mobFocus = null;
					global::Char.myCharz().charFocus = @char;
					return;
				}
				if (Res.distance(@char.cx, @char.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) > 200 && @char.cx > 500 && @char.cHP > 2L)
				{
					KsSupper.Move(@char.cx - 100, @char.cy);
					return;
				}
				if (Res.distance(@char.cx, @char.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) > 200 && @char.cx < 500 && @char.cHP > 2L)
				{
					KsSupper.Move(@char.cx + 100, @char.cy);
					return;
				}
				if (Res.distance(@char.cx, @char.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) < 80 && @char.cx < 500 && @char.cHP > 2L)
				{
					KsSupper.Move(@char.cx + 100, @char.cy);
					return;
				}
				if (Res.distance(@char.cx, @char.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) < 80 && @char.cx > 500 && @char.cHP > 2L)
				{
					KsSupper.Move(@char.cx - 100, @char.cy);
					return;
				}
			}
		}
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x0000B080 File Offset: 0x00009280
	private static void Move(int x, int y)
	{
		global::Char @char = global::Char.myCharz();
		if (!Pk9rPickMob.IsVuotDiaHinh)
		{
			@char.currentMovePoint = new MovePoint(x, y);
			return;
		}
		int[] pointYsdMax = KsSupper.GetPointYsdMax(@char.cx, x);
		if (pointYsdMax[1] >= y || (pointYsdMax[1] >= @char.cy && (@char.statusMe == 2 || @char.statusMe == 1)))
		{
			pointYsdMax[0] = x;
			pointYsdMax[1] = y;
		}
		@char.currentMovePoint = new MovePoint(pointYsdMax[0], pointYsdMax[1]);
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x0000B100 File Offset: 0x00009300
	private static int GetYsd(int xsd)
	{
		global::Char @char = global::Char.myCharz();
		int num = TileMap.pxh;
		int num2 = -1;
		for (int i = 24; i < TileMap.pxh; i += 24)
		{
			if (TileMap.tileTypeAt(xsd, i, 2))
			{
				int num3 = Res.abs(i - @char.cy);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
		}
		return num2;
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x0000B154 File Offset: 0x00009354
	private static int[] GetPointYsdMax(int xStart, int xEnd)
	{
		int num = TileMap.pxh;
		int num2 = -1;
		if (xStart > xEnd)
		{
			for (int i = xEnd; i < xStart; i += 24)
			{
				int ysd = KsSupper.GetYsd(i);
				if (ysd < num)
				{
					num = ysd;
					num2 = i;
				}
			}
		}
		else
		{
			for (int j = xEnd; j > xStart; j -= 24)
			{
				int ysd2 = KsSupper.GetYsd(j);
				if (ysd2 < num)
				{
					num = ysd2;
					num2 = j;
				}
			}
		}
		return new int[] { num2, num };
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x0000B1C4 File Offset: 0x000093C4
	public static void TelePortTo(int x, int y)
	{
		global::Char.myCharz().cx = x;
		global::Char.myCharz().cy = y;
		Service.gI().charMove();
		if (!ItemTime.isExistItem(4387))
		{
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y + 1;
			Service.gI().charMove();
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y;
			Service.gI().charMove();
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x0000B240 File Offset: 0x00009440
	public static void Ks()
	{
		global::Char charFocus = global::Char.myCharz().charFocus;
		if (Res.distance(charFocus.cx, charFocus.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) > 100 && charFocus.cHP < 2L && charFocus.cHP > 0L)
		{
			KsSupper.TelePortTo(charFocus.cx, charFocus.cy);
		}
		if (Res.distance(charFocus.cx, charFocus.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) < 100 && charFocus.cHP < 2L && charFocus.cHP > 0L)
		{
			AutoSkill.AutoSendAttack();
		}
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x0000B2F4 File Offset: 0x000094F4
	public static void autoitem()
	{
		global::Char @char = global::Char.myCharz();
		for (int i = 0; i < GameScr.vItemMap.size(); i++)
		{
			ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
			if (itemMap != null && itemMap.playerId == @char.charID)
			{
				if (Res.abs(itemMap.xEnd - @char.cx) > 25)
				{
					KsSupper.Move(itemMap.xEnd, itemMap.yEnd);
				}
				Service.gI().pickItem(itemMap.itemMapID);
				return;
			}
		}
		for (int j = 0; j < GameScr.vItemMap.size(); j++)
		{
			ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(j);
			if (itemMap2 != null && itemMap2.playerId == -1)
			{
				if (Res.abs(itemMap2.xEnd - @char.cx) > 25)
				{
					KsSupper.Move(itemMap2.xEnd, itemMap2.yEnd);
				}
				Service.gI().pickItem(itemMap2.itemMapID);
				return;
			}
		}
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00004089 File Offset: 0x00002289
	public static void Update()
	{
		if (KsSupper.IsBoss())
		{
			KsSupper.FocusSuperBroly();
			if (DataAccount.Type == 3)
			{
				KsSupper.Ks();
			}
		}
	}
}
