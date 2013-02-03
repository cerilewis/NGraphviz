namespace NGraphviz.Native
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
    public static class NGraphvizNative
    {
        public const string LibGvc = "gvc.dll";

        public const string LibGraph = "graph.dll";
        public const int Success = 0;

        /// <summary> 
        /// Creates a new Graphviz context. 
        /// </summary> 
        [DllImport(LibGvc, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gvContext();

        /// <summary> 
        /// Releases a context's resources. 
        /// </summary> 
        [DllImport(LibGvc, CallingConvention = CallingConvention.Cdecl)]
        public static extern int gvFreeContext(IntPtr gvc);

        /// <summary> 
        /// Reads a graph from a string. 
        /// </summary> 
        [DllImport(LibGraph)]
        public static extern IntPtr agmemread(string data);

        /// <summary> 
        /// Releases the resources used by a graph. 
        /// </summary> 
        [DllImport(LibGraph)]
        public static extern void agclose(IntPtr g);

        /// <summary> 
        /// Applies a layout to a graph using the given engine. 
        /// </summary> 
        [DllImport(LibGvc)]
        public static extern int gvLayout(IntPtr gvc, IntPtr g, string engine);

        /// <summary> 
        /// Releases the resources used by a layout. 
        /// </summary> 
        [DllImport(LibGvc)]
        public static extern int gvFreeLayout(IntPtr gvc, IntPtr g);

        /// <summary> 
        /// Renders a graph to a file. 
        /// </summary> 
        [DllImport(LibGvc)]
        public static extern int gvRenderFilename(IntPtr gvc, IntPtr g, string format, string fileName);

        /// <summary> 
        /// Renders a graph in memory. 
        /// </summary> 
        [DllImport(LibGvc)]
        public static extern int gvRenderData(IntPtr gvc, IntPtr g, string format, out IntPtr result, out int length);

        [DllImport("msvcrt.dll", SetLastError = true)]
        public static extern void free(IntPtr pointer);
    }
}