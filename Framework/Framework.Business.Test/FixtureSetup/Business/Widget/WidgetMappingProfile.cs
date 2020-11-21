namespace ZTR.Framework.Business.Test.FixtureSetup.Business.Widget
{
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models;
    using ZTR.Framework.Business.Test.FixtureSetup.DataAccess.Entities;
    using AutoMapper;

    public class WidgetMappingProfile : Profile
    {
        public WidgetMappingProfile()
        {
            CreateMap<Widget, WidgetReadModel>()
                .ForMember(x => x.SomeDateTimePropertyPreciseToTheMinute, opt => opt.Ignore())
                .ForMember(x => x.SomeDateTimePropertyPreciseToTheSecond, opt => opt.Ignore())
                .ForMember(x => x.SomeNullableDateTimePropertyPreciseToTheMinute, opt => opt.Ignore())
                .ForMember(x => x.SomeNullableDateTimePropertyPreciseToTheSecond, opt => opt.Ignore())
                .ForMember(x => x.SomeDateTimePropertyPreciseToTheHour, opt => opt.Ignore())
                .ForMember(x => x.SomeNullableDateTimePropertyPreciseToTheHour, opt => opt.Ignore())
                .ForMember(x => x.SomeDateTimePropertyPreciseToTheDay, opt => opt.Ignore())
                .ForMember(x => x.SomeNullableDateTimePropertyPreciseToTheDay, opt => opt.Ignore());

            CreateMap<WidgetCreateModel, Widget>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.DateCreated, opt => opt.Ignore())
                .ForMember(x => x.DateModified, opt => opt.Ignore());

            CreateMap<WidgetUpdateModel, Widget>()
                .ForMember(x => x.DateCreated, opt => opt.Ignore())
                .ForMember(x => x.DateModified, opt => opt.Ignore());
        }
    }
}
