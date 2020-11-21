namespace ZTR.Framework.Service
{
    using Microsoft.AspNetCore.Mvc;

    public class JobRouteAttribute : RouteAttribute
    {
        private const string TemplateBase = "api/job/";

        public JobRouteAttribute()
            : base(TemplateBase + "[controller]")
        {
        }

        public JobRouteAttribute(string templateSuffix)
            : base(TemplateBase + templateSuffix)
        {
        }
    }
}
