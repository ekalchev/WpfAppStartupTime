using System.Diagnostics;
using System;
using System.Runtime.InteropServices.ComTypes;

public static class ProcessStartupInfo
{
    public static TimeSpan TimeSinceProcessStart
    {
        get
        {
            TimeSpan result = TimeSpan.Zero;

            try
            {
                var now = DateTime.UtcNow;
                FILETIME ftCreation, ftExit, ftKernel, ftUser;
                if (GetProcessTimes(Process.GetCurrentProcess().Handle, out ftCreation, out ftExit, out ftKernel, out ftUser) == true)
                {
                    DateTime startTimeUtc = FileTimeToDateTime(ftCreation);
                    result = now - startTimeUtc;
                }
            }
            catch (Exception)
            {
                // do not propagate exceptions
            }

            return result;
        }
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool GetProcessTimes(IntPtr hProcess, out FILETIME lpCreationTime, out FILETIME lpExitTime, out FILETIME lpKernelTime, out FILETIME lpUserTime);

    private static DateTime FileTimeToDateTime(FILETIME fileTime)
    {
        // uint conversion must be done on both fields before ulong conversion
        ulong hFT2 = unchecked((((ulong)(uint)fileTime.dwHighDateTime) << 32) | (uint)fileTime.dwLowDateTime);
        return DateTime.FromFileTimeUtc((long)hFT2);
    }
}