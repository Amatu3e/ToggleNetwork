using System.Runtime.Versioning;

using ToggleNetwork.Adapters;
using ToggleNetwork.Users;

namespace ToggleNetwork;

[SupportedOSPlatform("Windows")]
class Program
{
	static void Main(string[] args)
	{
		if (!User.IsAdministrator())
		{
			User.RestartAsAdmin();
			return;
		}
		InternetAdapters.ToggleInternetService();
	}	
}
