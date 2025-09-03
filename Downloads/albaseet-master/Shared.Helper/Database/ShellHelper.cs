using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Shared.Helper.Database
{
	public static class ShellHelper
	{
		public static async Task ExecuteShellCommandAsync(string arguments)
		{
			var processInfo = new ProcessStartInfo("cmd.exe", arguments)
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (var process = new Process { StartInfo = processInfo })
			{
				process.Start();

				string output = await process.StandardOutput.ReadToEndAsync();
				string error = await process.StandardError.ReadToEndAsync();
				await process.WaitForExitAsync();

				if (process.ExitCode != 0)
				{
					throw new Exception($"Command failed with error: {error}");
				}
			}
		}


		/// <summary>
		/// Executes a command on a remote host via SSH.
		/// Throws if the exit status is non‑zero.
		/// </summary>
		public static Task<string> ExecuteOverSshAsync(string host, int port, string username, string password, string commandText) => Task.Run(() =>
		{
			using var client = new SshClient(host, port, username, password);
			client.Connect();

			var cmd = client.RunCommand(commandText);
			client.Disconnect();

			if (cmd.ExitStatus != 0)
				throw new Exception($"Remote command failed (exit {cmd.ExitStatus}): {cmd.Error}");

			return cmd.Result;
		});


		public static bool RemoteFileExists(string host, int port, string username, string password, string remoteFilePath)
		{
			// Escape backslashes for PowerShell
			var escaped = remoteFilePath.Replace("\\", "\\\\");

			// Build a one-liner. Exit code 0 = exists, 1 = not exists.
			var psCmd =
				$"powershell -NoProfile -Command " +
				$"\"if (Test-Path '{escaped}') {{ exit 0 }} else {{ exit 1 }}\"";

			using var client = new SshClient(host, port, username, password);
			client.Connect();

			var cmd = client.CreateCommand(psCmd);
			cmd.Execute();

			client.Disconnect();
			return cmd.ExitStatus == 0;
		}
	}
}
