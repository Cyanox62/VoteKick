using System;
using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using System.IO;

namespace VoteKick
{
	[PluginDetails(
	author = "Cyanox",
	name = "Vote Kick",
	description = "A vote kick plugin.",
	id = "cyan.votekick",
	version = "1.0.0",
	SmodMajor = 3,
	SmodMinor = 0,
	SmodRevision = 0
	)]

	public class VoteKick : Plugin
	{
		public static Plugin instance;

		public static int cMinVotes;
		public static int cCooldown;
		public static float cTimeout;
		public static int cPassPercent;
		public static int cPassCooldown;
		public static int cFailCooldown;
		public static int cVoteLevel;
		public static List<string> cVoteRanks;
		public static List<string> cImmuneRanks;

		public static bool isPlayerXP = PluginManager.Manager.Plugins.Where(p => p.Details.id == "cyan.playerxp").Count() > 0;

		public static string ConfigFolerFilePath = FileManager.GetAppFolder() + "VoteKick";
		public static string BanFolderFilePath = FileManager.GetAppFolder() + "VoteKick" + Path.DirectorySeparatorChar + "Bans";

		public override void OnDisable() {}

		public override void OnEnable()
		{
			if (!Directory.Exists(ConfigFolerFilePath))
			{
				Directory.CreateDirectory(ConfigFolerFilePath);
			}
			if (!Directory.Exists(BanFolderFilePath))
			{
				Directory.CreateDirectory(BanFolderFilePath);
			}
		}

		public override void Register()
		{
			instance = this;

			AddEventHandlers(new EventHandler());

			AddCommands(new[] { "vkban", "votekickban" }, new BanPlayerCommand());
			AddCommands(new[] { "vkunban", "votekickunban" }, new UnbanPlayerCommand());

			AddConfig(new Smod2.Config.ConfigSetting("vk_minimum_votes", 2, false, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_timeout", 30f, false, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_pass_percent", 60, false, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_pass_cooldown", 300, false, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_fail_cooldown", 300, false, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_vote_ranks", new string[] {}, false, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_immune_ranks", new[]
			{
				"moderator",
				"admin",
				"owner"
			}, false, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_vote_level", 1, false, true, ""));
		}

		public static string[] StringToStringArray(string input)
		{
			List<string> data = new List<string>();
			if (input.Length > 0)
			{
				string[] a = input.Split(' ');
				for (int i = 0; i < a.Count(); i++)
				{
					data.Add(a[i]);
				}
			}
			return data.ToArray();
		}

		public static Player GetPlayer(string steamid)
		{
			return PluginManager.Manager.Server.GetPlayers().FirstOrDefault(x => x.SteamId == steamid);
		}

		public static int LevenshteinDistance(string s, string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			for (int i = 0; i <= n; d[i, 0] = i++)
			{
			}

			for (int j = 0; j <= m; d[0, j] = j++)
			{
			}

			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= m; j++)
				{
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
			return d[n, m];
		}

		public static Player GetPlayer(string args, out Player playerOut)
		{
			int maxNameLength = 31, LastnameDifference = 31;
			Player plyer = null;
			string str1 = args.ToLower();
			foreach (Player pl in PluginManager.Manager.Server.GetPlayers(str1))
			{
				if (!pl.Name.ToLower().Contains(args.ToLower())) { goto NoPlayer; }
				if (str1.Length < maxNameLength)
				{
					int x = maxNameLength - str1.Length;
					int y = maxNameLength - pl.Name.Length;
					string str2 = pl.Name;
					for (int i = 0; i < x; i++)
					{
						str1 += "z";
					}
					for (int i = 0; i < y; i++)
					{
						str2 += "z";
					}
					int nameDifference = LevenshteinDistance(str1, str2);
					if (nameDifference < LastnameDifference)
					{
						LastnameDifference = nameDifference;
						plyer = pl;
					}
				}
				NoPlayer:;
			}
			playerOut = plyer;
			return playerOut;
		}

		public static bool isPlayerBanned(Player player)
		{
			if (Directory.Exists(BanFolderFilePath))
			{
				foreach (string file in Directory.GetFiles(BanFolderFilePath))
				{
					if (file.Replace($"{BanFolderFilePath}{Path.DirectorySeparatorChar}", "").Replace(".txt", "").Trim() == player.SteamId)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
