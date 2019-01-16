using Smod2.Commands;

namespace VoteKick
{
	class CommandHandler : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Cancells a vote.";
		}

		public string GetUsage()
		{
			return "(VOTEKICK) (CANCEL)";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length > 0)
			{
				switch (args[0].ToLower())
				{
					case "cancel":
						{
							// unfinished
							break;
						}
				}
			}
			return new string[] { GetUsage() };
		}
	}
}
