using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;

using Ardalis.GuardClauses;

namespace ToggleNetwork.Users
{
	[SupportedOSPlatform("Windows")]
	sealed class User
	{
		public static bool IsAdministrator()
		{
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		public static void RestartAsAdmin()
		{
			var exeName = Environment.ProcessPath;

			Guard.Against.NullOrEmpty(exeName);

			var startInfo = new ProcessStartInfo(exeName)
			{
				UseShellExecute = true,
				Verb = "runas"
			};
			Process.Start(startInfo);
		}
	}
}
