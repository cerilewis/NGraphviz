namespace NGraphviz
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using NGraphviz.Native;

    public class GraphvizContext : IDisposable
    {
        private static readonly object LockObject = new object();

        private static GraphvizContext singletonContext;

        private readonly IntPtr _context;

        private bool _disposed; // to detect redundant calls

        internal GraphvizContext(IntPtr context)
        {
            this._context = context;
        }

        ~GraphvizContext()
        {
            Trace.WriteLine("~GraphvizContext() called");
            this.Dispose(false);
        }

        public static bool IsInitialised
        {
            get
            {
                lock (LockObject)
                {
                    return singletonContext != null;
                }
            }
        }

        public static GraphvizContext Initialise()
        {
            lock (LockObject)
            {
                if (singletonContext != null)
                {
                    throw new InvalidOperationException("Context already created");
                }

                var contextPtr = NGraphvizNative.gvContext();
                if (contextPtr == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Failed to initialise Graphviz context");
                }

                singletonContext = new GraphvizContext(contextPtr);
            }

            return singletonContext;
        }

        public Graph CreateGraph(string dot)
        {
            var graphPtr = NGraphvizNative.agmemread(dot);
            if (graphPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to generate graph from dot source");
            }

            return new Graph(graphPtr);
        }

        public byte[] RenderGraph(Graph graph, string layout, string format)
        {
            var layoutSucceeded = false;
            try
            {
                if (NGraphvizNative.gvLayout(this._context, graph.GraphPtr, layout) != NGraphvizNative.Success)
                {
                    throw new InvalidOperationException("Graphviz layout failed");
                }
                
                layoutSucceeded = true;

                IntPtr output;
                int length;
                if (NGraphvizNative.gvRenderData(this._context, graph.GraphPtr, format, out output, out length)
                    != NGraphvizNative.Success)
                {
                    throw new InvalidOperationException("Graphviz render data failed");
                }

                var data = new byte[length];
                Marshal.Copy(output, data, 0, length);

                //// TODO Free memory

                return data;
            }
            finally
            {
                if (layoutSucceeded)
                {
                    NGraphvizNative.gvFreeLayout(this._context, graph.GraphPtr);
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (!this._disposed)
                {
                    Trace.WriteLine("GraphvizContext.Dispose() called");
                    NGraphvizNative.gvFreeContext(this._context);
                    ContextDisposed();
                }

                this._disposed = true;
            }
        }

        private static void ContextDisposed()
        {
            lock (LockObject)
            {
                singletonContext = null;
            }
        }
    }
}