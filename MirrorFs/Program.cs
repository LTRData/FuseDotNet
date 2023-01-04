using FuseDotNet;

namespace MirrorFs;

internal class Program
{
    public static int Main(string[] args)
    {
        try
        {
            Console.WriteLine($"Creating mirror file system with base path '{args[0]}'");
            var operations = new MirrorFsOperations(args[0]);

            args = args.Skip(1).ToArray();

            Console.WriteLine($"Starting fuse with arguments: '{string.Join("', '", args)}'");
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
