namespace FuseDotNet;

/// <summary>
/// Optional extension to <see cref="IFuseOperations"/> for high-level FUSE configuration.
/// </summary>
public interface IFuseInitWithConfig
{
    /// <summary>
    /// Initializes the filesystem instead of IFuseOperations.Init. The configuration
    /// contains libfuse's current defaults and command-line settings. References are
    /// valid only during this callback. Change only the settings the filesystem needs.
    /// </summary>
    void Init(ref FuseConnInfo fuse_conn_info, ref FuseConfig config);
}
