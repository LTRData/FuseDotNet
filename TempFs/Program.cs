using FuseDotNet;
using System;

namespace TempFs;

public static class Program
{
    public static int Main(params string[] args)
    {
        try
        {
            var operations = new TempFsOperations();

            operations.Mount(args);

            Console.WriteLine($"Fuse exit");

            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(ex);
            Console.ResetColor();
            return ex is PosixException pex ? (int)pex.NativeErrorCode : ex.HResult;
        }
    }
}
