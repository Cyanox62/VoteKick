using System.IO;
using Smod2.API;
using Smod2.Commands;

namespace VoteKick
{
	class BanPlayerCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Bans a player from using votekick.";
		}

		public string GetUsage()
		{
			return "(VKBAN / VOTEKICKBAN) (PLAYER / STEAMID)";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
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

				File.Create($"{VoteKick.BanFolderFilePath}{Path.DirectorySeparatorChar}{steamid}.txt");
				return new string[]
				{
					$"{VoteKick.GetPlayer(steamid).Name} has been banned."
				};
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
