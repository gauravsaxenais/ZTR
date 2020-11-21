namespace ZTR.Framework.DataAccess.Entities
{
    using System;

    public interface IEffectiveDateTime
    {
        DateTimeOffset EffectiveStartDateTime { get; set; }

        DateTimeOffset? EffectiveEndDateTime { get; set; }
    }
}
