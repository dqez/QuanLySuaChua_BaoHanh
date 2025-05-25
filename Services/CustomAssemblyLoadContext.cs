using System.Reflection;
using System.Runtime.Loader;

namespace QuanLySuaChua_BaoHanh.Services
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
            => LoadUnmanagedDllFromPath(absolutePath);

        protected override Assembly Load(AssemblyName assemblyName) => null;
    }

}
