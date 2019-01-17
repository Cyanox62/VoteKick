﻿using System;
using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using scp4aiur;

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
		public static List<string> cVoteRanks;
		public static List<string> cImmuneRanks;

		public override void OnDisable() {}

		public override void OnEnable() {}

		public override void Register()
		{
			instance = this;

			Timing.Init(this);

			AddEventHandlers(new EventHandler());

			AddConfig(new Smod2.Config.ConfigSetting("vk_minimum_votes", 2, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_timeout", 30f, Smod2.Config.SettingType.FLOAT, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_pass_percent", 60, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_pass_cooldown", 300, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_fail_cooldown", 300, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_vote_ranks", new string[] {"moderator"}, Smod2.Config.SettingType.LIST, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("vk_immune_ranks", new[]
{
				"moderator",
				"admin",
				"owner"
			}, Smod2.Config.SettingType.LIST, true, ""));
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
			foreach (Player player in PluginManager.Manager.Server.GetPlayers())
			{
				if (player.SteamId == steamid)
				{
					return player;
				}
			}
			return null;
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
	}
}
