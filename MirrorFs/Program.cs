using FuseDotNet;
using FuseDotNet.Logging;

namespace MirrorFs;

public static class Program
{
    public static int Main(params string[] args)
    {
        try
        {
            Console.WriteLine($"Creating mirror file system with base path '{args[0]}'");
            using var operations = new MirrorFsOperations(args[0]);

            args = [.. args.Skip(1)];

            Console.WriteLine($"Starting fuse with arguments: '{string.Join("', '", args)}'");
            operations.Mount(args, new ConsoleLogger());

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
