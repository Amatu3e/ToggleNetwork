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
		var checkStatusPsi = CreatePowershellCommand($"-Command \"Get-NetAdapter -Name '{adapterName}'\"");
		RunProcessWithOutput(checkStatusPsi, out string output);
		return output.Contains("Up");
	}

	static void SetNetworkAdapterStatus(string adapterName, string command)
	{
		RunProcess(CreatePowershellCommand($"-Command \"{command} -Name '{adapterName}' -Confirm:$false\""));
	}

	static void RunProcessWithOutput(ProcessStartInfo psi, out string output)
	{
		var process = RunProcess(psi);
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
		List<string> adapterNames = [];

		var checkStatusPsi = CreatePowershellCommand("-Command \"Get-NetAdapter | Select-Object -ExpandProperty Name\"");

		using(var process = RunProcess(checkStatusPsi))
		{
			using var reader = process.StandardOutput;
			
			string line;
			while((line = reader.ReadLine()!) != null)
			{
				adapterNames.Add(line);
			}
		}
		return adapterNames;
	}

	static ProcessStartInfo CreatePowershellCommand(string command)
	{
		return new ProcessStartInfo("powershell", command)
		{
			RedirectStandardOutput = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};
	}
}
