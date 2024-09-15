using System.Diagnostics;

using Ardalis.GuardClauses;

namespace ToggleNetwork.Adapters;

class InternetAdapters
{
	public static void ToggleInternetService()
	{
		List<string> adapters = GetNetworkInterfaceList();

		if(adapters.Count == 0)
		{
			Console.WriteLine("No network adapters found.");
			return;
		}

		foreach(var adapter in adapters)
		{
			ToggleAdapter(adapter);
		}
	}

	static void ToggleAdapter(string adapterName)
	{
		bool isEnabled = IsNetworkAdapterEnabled(adapterName);

		string command = isEnabled ? "Disable-NetAdapter" : "Enable-NetAdapter";
		SetNetworkAdapterStatus(adapterName, command);

		Console.WriteLine($"Toggled network adapter: {adapterName} to {command}");
	}

	static bool IsNetworkAdapterEnabled(string adapterName)
	{
		var checkStatusPsi = new ProcessStartInfo("powershell", $"-Command \"Get-NetAdapter -Name '{adapterName}'\"")
		{
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		RunProcessWithOutput(checkStatusPsi, out string output);
		return output.Contains("Up");
	}

	static void SetNetworkAdapterStatus(string adapterName, string command)
	{
		var psi = new ProcessStartInfo("powershell", $"-Command \"{command} -Name '{adapterName}' -Confirm:$false\"")
		{
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		RunProcess(psi);
	}

	static void RunProcessWithOutput(ProcessStartInfo psi, out string output)
	{
		Process? process = RunProcess(psi);
		output = process.StandardOutput.ReadToEnd();
	}

	static Process RunProcess(ProcessStartInfo psi)
	{
		var process = Process.Start(psi);
		Guard.Against.Null(process);
		process.WaitForExit();
		return process;
	}

	static List<string> GetNetworkInterfaceList()
	{
		return [NetworkAdapters.Ethernet.ToString()];
	}
}
