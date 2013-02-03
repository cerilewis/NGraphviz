namespace NGraphviz
{
    using System;

    using NGraphviz.Native;

    public class Graph : IDisposable
    {
        private readonly IntPtr _graph;

        private bool _disposed; // to detect redundant calls

        internal Graph(IntPtr graph)
        {
            this._graph = graph;
        }

        ~Graph()
        {
            this.Dispose(false);
        }

        public IntPtr GraphPtr
        {
            get
            {
                return this._graph;
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
                    NGraphvizNative.agclose(this._graph);
                }

                this._disposed = true;
            }
        }
    }
}