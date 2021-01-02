using System;

namespace WebApiWithEFCore.XUnit
{
    public class TestDisposable : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}