using Microsoft.AspNetCore.Mvc;

namespace Deps_CleanArchitecture.Web.Base
{
    public class PagedRequest
    {
        [FromQuery]
        public int Page { get; set; }

        [FromQuery]
        public int Size { get; set; }
    }
}
