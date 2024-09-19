using System.Collections.Generic;

namespace Deps_CleanArchitecture.Web.Base
{
    public class PagedResponse<T>
    {
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
    }
}
