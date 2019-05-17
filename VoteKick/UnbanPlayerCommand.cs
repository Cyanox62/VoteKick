using System;
using System.IO;
using Smod2.API;
using Smod2.Commands;

namespace VoteKick
{
	class UnbanPlayerCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Unbans a player from using the report system.";
		}

		public string GetUsage()
		{
			return "(VKUNBAN / VOTEKICKUNBAN) (STEAMID)";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (Directory.Exists(VoteKick.BanFolderFilePath))
			{
				if (Directory.Exists(VoteKick.BanFolderFilePath))
				{
					string steamid;
					Player myPlayer = VoteKick.GetPlayer(args[0], out myPlayer);
					if (myPlayer != null)
					{
						steamid = myPlayer.SteamId;
					}
					else if (ulong.TryParse(args[0], out ulong a))
					{
						steamid = a.ToString();
					}
					else
					{
						return new string[] { "Error: invalid player." };
					}

					string file = $"{VoteKick.BanFolderFilePath}{Path.DirectorySeparatorChar}{steamid}.txt";
					if (File.Exists(file))
					{
						File.Delete(file);
						return new string[]
						{
							$"{VoteKick.GetPlayer(steamid).Name} has been unbanned."
						};
					}
					else
					{
						return new string[]
						{
							"Player is not banned."
						};
					}
				}
				else
				{
					return new string[]
					{
						"Error parsing SteamID."
					};
				}
			}
			else
			{
				return new string[]
				{
					"Error locating config folder."
				};
			}
		}
	}
}
