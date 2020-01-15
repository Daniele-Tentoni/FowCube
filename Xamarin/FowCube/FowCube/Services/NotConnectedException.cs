namespace FowCube.Services
{
    using System;

    public class NotConnectedException : Exception
    {
        public NotConnectedException(string message, Exception inner) : base(message, inner) { }
        public NotConnectedException(string message) : this(message, null) { }
        public NotConnectedException() : this("") { }
    }
}
