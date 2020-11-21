namespace ZTR.Framework.Business.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ZTR.Framework.Business.Test.FixtureSetup;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Validators;
    using ZTR.Framework.Business.Test.FixtureSetup.Fakes;
    using ZTR.Framework.Test;
    using EnsureThat;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public sealed class ValidationHelperTest : IClassFixture<TestFixture>
    {
        private readonly TestFixture _testFixture;

        public ValidationHelperTest(TestFixture testFixture)
        {
            EnsureArg.IsNotNull(testFixture, nameof(testFixture));

            _testFixture = testFixture;
        }

        [Fact]
        public void IsPreciseToDay_Rule_ShouldFail()
        {
            var widgetFaker = _testFixture.ServiceProvider.GetRequiredService<WidgetFaker<WidgetCreateModel>>();
            var widgetCreateModel = widgetFaker.Generate();

            widgetCreateModel.SomeDateTimePropertyPreciseToTheDay = new DateTimeOffset(2019, 1, 2, 3, 4, 51, 600, TimeSpan.Zero);

            var widgetCreateValidator = _testFixture.ServiceProvider.GetRequiredService<WidgetCreateModelValidator>();

            var validationResult = widgetCreateValidator.Validate(widgetCreateModel);
            var errorRecord = new ErrorRecord<WidgetErrorCode>(0, validationResult);
            var errorRecords = new ErrorRecords<WidgetErrorCode>(new[] { errorRecord });

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.NotPreciseToDays);
        }

        [Fact]
        public void IsPreciseToDay_ForNullableDateTimeOffsetProperty_Rule_ShouldFail()
        {
            var widgetFaker = _testFixture.ServiceProvider.GetRequiredService<WidgetFaker<WidgetCreateModel>>();
            var widgetCreateModel = widgetFaker.Generate();

            widgetCreateModel.SomeNullableDateTimePropertyPreciseToTheDay = new DateTimeOffset(2019, 1, 2, 3, 4, 51, 600, TimeSpan.Zero);

            var widgetCreateValidator = _testFixture.ServiceProvider.GetRequiredService<WidgetCreateModelValidator>();

            var validationResult = widgetCreateValidator.Validate(widgetCreateModel);
            var errorRecord = new ErrorRecord<WidgetErrorCode>(0, validationResult);
            var errorRecords = new ErrorRecords<WidgetErrorCode>(new[] { errorRecord });

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.NotPreciseToDaysNullable);
        }

        [Fact]
        public void IsPreciseToHour_Rule_ShouldFail()
        {
            var widgetFaker = _testFixture.ServiceProvider.GetRequiredService<WidgetFaker<WidgetCreateModel>>();
            var widgetCreateModel = widgetFaker.Generate();

            widgetCreateModel.SomeDateTimePropertyPreciseToTheHour = new DateTimeOffset(2019, 1, 2, 3, 4, 51, 600, TimeSpan.Zero);

            var widgetCreateValidator = _testFixture.ServiceProvider.GetRequiredService<WidgetCreateModelValidator>();

            var validationResult = widgetCreateValidator.Validate(widgetCreateModel);
            var errorRecord = new ErrorRecord<WidgetErrorCode>(0, validationResult);
            var errorRecords = new ErrorRecords<WidgetErrorCode>(new[] { errorRecord });

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.NotPreciseToHours);
        }

        [Fact]
        public void IsPreciseToHour_ForNullableDateTimeOffsetProperty_Rule_ShouldFail()
        {
            var widgetFaker = _testFixture.ServiceProvider.GetRequiredService<WidgetFaker<WidgetCreateModel>>();
            var widgetCreateModel = widgetFaker.Generate();

            widgetCreateModel.SomeNullableDateTimePropertyPreciseToTheHour = new DateTimeOffset(2019, 1, 2, 3, 4, 51, 600, TimeSpan.Zero);

            var widgetCreateValidator = _testFixture.ServiceProvider.GetRequiredService<WidgetCreateModelValidator>();

            var validationResult = widgetCreateValidator.Validate(widgetCreateModel);
            var errorRecord = new ErrorRecord<WidgetErrorCode>(0, validationResult);
            var errorRecords = new ErrorRecords<WidgetErrorCode>(new[] { errorRecord });

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.NotPreciseToHoursNullable);
        }

        [Fact]
        public void IsPreciseToMinute_Rule_ShouldFail()
        {
            var widgetFaker = _testFixture.ServiceProvider.GetRequiredService<WidgetFaker<WidgetCreateModel>>();
            var widgetCreateModel = widgetFaker.Generate();

            widgetCreateModel.SomeDateTimePropertyPreciseToTheMinute = new DateTimeOffset(2019, 1, 2, 3, 4, 51, 600, TimeSpan.Zero);

            var widgetCreateValidator = _testFixture.ServiceProvider.GetRequiredService<WidgetCreateModelValidator>();

            var validationResult = widgetCreateValidator.Validate(widgetCreateModel);
            var errorRecord = new ErrorRecord<WidgetErrorCode>(0, validationResult);
            var errorRecords = new ErrorRecords<WidgetErrorCode>(new[] { errorRecord });

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.NotPreciseToMinute);
        }

        [Fact]
        public void IsPreciseToMinute_ForNullableDateTimeOffsetProperty_Rule_ShouldFail()
        {
            var widgetFaker = _testFixture.ServiceProvider.GetRequiredService<WidgetFaker<WidgetCreateModel>>();
            var widgetCreateModel = widgetFaker.Generate();

            widgetCreateModel.SomeNullableDateTimePropertyPreciseToTheMinute = new DateTimeOffset(2019, 1, 2, 3, 4, 51, 600, TimeSpan.Zero);

            var widgetCreateValidator = _testFixture.ServiceProvider.GetRequiredService<WidgetCreateModelValidator>();

            var validationResult = widgetCreateValidator.Validate(widgetCreateModel);
            var errorRecord = new ErrorRecord<WidgetErrorCode>(0, validationResult);
            var errorRecords = new ErrorRecords<WidgetErrorCode>(new[] { errorRecord });

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.NotPreciseToMinuteNullable);
        }

        [Fact]
        public void IsPreciseToMinute_And_IsPreciseToSecond_Rules_ShouldSucceed()
        {
            var widgetFaker = _testFixture.ServiceProvider.GetRequiredService<WidgetFaker<WidgetCreateModel>>();
            var widgetCreateModel = widgetFaker.Generate();

            var widgetCreateValidator = _testFixture.ServiceProvider.GetRequiredService<WidgetCreateModelValidator>();

            var validationResult = widgetCreateValidator.Validate(widgetCreateModel);
            var errorMessages = validationResult.ToErrorMessages<WidgetErrorCode>();

            Assert.False(errorMessages.Any());
        }

        [Fact]
        public void IsPreciseToSecond_Rule_ShouldFail()
        {
            var widgetFaker = _testFixture.ServiceProvider.GetRequiredService<WidgetFaker<WidgetCreateModel>>();
            var widgetCreateModel = widgetFaker.Generate();

            widgetCreateModel.SomeDateTimePropertyPreciseToTheSecond = new DateTimeOffset(2019, 1, 2, 3, 4, 51, 600, TimeSpan.Zero);

            var widgetCreateValidator = _testFixture.ServiceProvider.GetRequiredService<WidgetCreateModelValidator>();

            var validationResult = widgetCreateValidator.Validate(widgetCreateModel);
            var errorRecord = new ErrorRecord<WidgetErrorCode>(0, validationResult);
            var errorRecords = new ErrorRecords<WidgetErrorCode>(new[] { errorRecord });

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.NotPreciseToSecond);
        }

        [Fact]
        public void IsPreciseToSecond_ForNullableDateTimeOffsetProperty_Rule_ShouldFail()
        {
            var widgetFaker = _testFixture.ServiceProvider.GetRequiredService<WidgetFaker<WidgetCreateModel>>();
            var widgetCreateModel = widgetFaker.Generate();

            widgetCreateModel.SomeNullableDateTimePropertyPreciseToTheSecond = new DateTimeOffset(2019, 1, 2, 3, 4, 51, 600, TimeSpan.Zero);

            var widgetCreateValidator = _testFixture.ServiceProvider.GetRequiredService<WidgetCreateModelValidator>();

            var validationResult = widgetCreateValidator.Validate(widgetCreateModel);
            var errorRecord = new ErrorRecord<WidgetErrorCode>(0, validationResult);
            var errorRecords = new ErrorRecords<WidgetErrorCode>(new[] { errorRecord });

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.NotPreciseToSecondNullable);
        }

        [Fact]
        public void DuplicateValidationShouldWorkOnSimpleTypes()
        {
            var numbers = new List<int>() { 1, 2, 4, 2 };
            var indexedItems = numbers.ToIndexedItems().ToList();

            var errorRecords = ValidationHelpers.DuplicateValidation(
                indexedItems,
                x => x.Item,
                WidgetErrorCode.CodeNotUnique);

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.CodeNotUnique);
            AssertExtensions.ContainsOrdinalPosition(errorRecords, 3);
            AssertExtensions.NotContainsOrdinalPosition(errorRecords, new[] { 0, 1, 2 });
        }

        [Fact]
        public void DuplicateValidationShouldWorkOnAMultipleKeysOfSimpleTypes()
        {
            var theItems = new[]
            {
                new { Id = 1, Code = "A", OtherId = 2 },
                new { Id = 2, Code = "B", OtherId = 2 },
                new { Id = 3, Code = "A", OtherId = 2 },
            };

            var indexedItems = theItems.ToIndexedItems().ToList();

            var errorRecords = ValidationHelpers.DuplicateValidation(
                indexedItems,
                x => new { x.Item.Code, x.Item.OtherId },
                WidgetErrorCode.CodeNotUnique);

            AssertExtensions.ContainsErrorCode(errorRecords, WidgetErrorCode.CodeNotUnique);
            AssertExtensions.ContainsOrdinalPosition(errorRecords, new[] { 2 });
            AssertExtensions.NotContainsOrdinalPosition(errorRecords, new[] { 1 });
        }

        [Fact]
        public void DuplicateValidationShouldWorkOnAMultipleKeysOfSimpleTypesHappy()
        {
            var theItems = new[]
            {
                new { Id = 1, Code = "A", OtherId = 2 },
                new { Id = 2, Code = "B", OtherId = 2 },
                new { Id = 3, Code = "A", OtherId = 2 },
            };

            var indexedItems = theItems.ToIndexedItems().ToList();

            var errorRecords = ValidationHelpers.DuplicateValidation(
                indexedItems,
                x => new { x.Item.Id, x.Item.Code, x.Item.OtherId },
                WidgetErrorCode.CodeNotUnique);

            AssertExtensions.NotContainsErrorCode(errorRecords, WidgetErrorCode.CodeNotUnique);
            AssertExtensions.NotContainsOrdinalPosition(errorRecords, new[] { 0, 1, 2 });
        }
    }
}
