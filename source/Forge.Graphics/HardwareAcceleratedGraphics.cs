using System.Runtime.InteropServices;

namespace Forge.Graphics;

public static class Graphics
{
	public static void ForceHardwareAcceleratedRendering()
	{
		try
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				RunOnWindows();
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				//TODO: RunOnLinux();
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				//TODO: RunOnMacOS();
			}
		}
		catch (Exception e)
		{
			Console.Error.WriteLine("Error happend when enable dedicated graphics!");
			Console.Error.WriteLine(e.ToString());
		}
	}

	// Solution - https://stackoverflow.com/questions/17270429/forcing-hardware-accelerated-rendering
	private static void RunOnWindows()
	{
		if (Environment.Is64BitProcess)
			NativeLibrary.Load("nvapi64.dll");
		else
			NativeLibrary.Load("nvapi32.dll");
	}
}
