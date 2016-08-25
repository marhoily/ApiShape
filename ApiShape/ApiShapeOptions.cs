using System;

namespace ApiShape
{
    /// <summary> Options of GetShape </summary>
    [Flags]
    public enum ApiShapeOptions
    {
        /// <summary>Not an option </summary>
        None,
        /// <summary>Shows assembly name as a header, even though
        ///     it contains version that changes and destroys regression</summary>
        ShowAssemblyName = 0x1,


    }
}