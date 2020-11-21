namespace ZTR.Framework.Service
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RawRequestBodyAttribute : ModelBinderAttribute
    {
        public RawRequestBodyAttribute()
            : base(typeof(RawRequestBodyModelBinder))
        {
        }

        public override BindingSource BindingSource
        {
            get => BindingSource.Body;
        }
    }
}
