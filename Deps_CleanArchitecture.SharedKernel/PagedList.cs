using System.Collections.Generic;

namespace Deps_CleanArchitecture.SharedKernel
{
    public class PagedList<T>
    {
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
    }
}
