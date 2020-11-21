namespace ZTR.Framework.Business.Test
{
    using System.Linq;
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget;
    using Xunit;

    public sealed class ErrorRecordsTest
    {
        public ErrorRecordsTest()
        {
        }

        [Fact]
        public void Merge_Error_Records()
        {
            var firstSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(6, WidgetErrorCode.CodeNotUnique, "1 for position 6"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.CodeNotUnique, "1a for position 0"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.DescriptionTooLong, "1b for position 0"),
                new ErrorRecord<WidgetErrorCode>(1, WidgetErrorCode.CodeNotUnique, "1 for position 1"),
            };

            var secondSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(4, WidgetErrorCode.NameRequired, "2 for position 4"),
                new ErrorRecord<WidgetErrorCode>(2, WidgetErrorCode.NameRequired, "2 for position 2"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.NameRequired, "2 for position 0"),
                new ErrorRecord<WidgetErrorCode>(6, WidgetErrorCode.NameRequired, "2 for position 6"),
            };

            var result = firstSet.Merge(secondSet).ToList();

            Assert.Equal(5, result.Count);
            Assert.Equal(0, result.First().OrdinalPosition);
            Assert.Equal(3, result.First().Errors.Count());
            Assert.Equal(2, result.First(x => x.OrdinalPosition == 6).Errors.Count());
        }

        [Fact]
        public void Merge_With_Empty_Ordinal()
        {
            var firstSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(WidgetErrorCode.CodeNotUnique, "1 for position 6"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.CodeNotUnique, "1a for position 0"),
                new ErrorRecord<WidgetErrorCode>(WidgetErrorCode.NameRequired, "1b for position 0"),
                new ErrorRecord<WidgetErrorCode>(1, WidgetErrorCode.CodeNotUnique, "1 for position 1"),
            };

            var secondSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(1, WidgetErrorCode.NameRequired, "2 for position 4"),
                new ErrorRecord<WidgetErrorCode>(WidgetErrorCode.DescriptionTooLong, "2 for position 2"),
            };

            var result = firstSet.Merge(secondSet).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(0, result.First().OrdinalPosition);
            Assert.Equal(4, result.First().Errors.Count());
            Assert.Equal(2, result.First(x => x.OrdinalPosition == 1).Errors.Count());
        }

        [Fact]
        public void Merge_Empty_First_Set()
        {
            var firstSet = new ErrorRecords<WidgetErrorCode>()
            {
            };

            var secondSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(13, WidgetErrorCode.NameRequired, "2 for position 4"),
                new ErrorRecord<WidgetErrorCode>(13, WidgetErrorCode.DescriptionTooLong, "2 for position 2"),
            };

            var result = firstSet.Merge(secondSet).ToList();

            Assert.Single(result);
            Assert.Equal(13, result.First().OrdinalPosition);
            Assert.Equal(2, result.First().Errors.Count());
        }

        [Fact]
        public void Merge_Empty_Second_Set()
        {
            var firstSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(99, WidgetErrorCode.NameRequired, "2 for position 4"),
                new ErrorRecord<WidgetErrorCode>(99, WidgetErrorCode.DescriptionTooLong, "2 for position 2"),
            };

            var secondSet = new ErrorRecords<WidgetErrorCode>()
            {
            };

            var result = firstSet.Merge(secondSet).ToList();

            Assert.Single(result);
            Assert.Equal(99, result.First().OrdinalPosition);
            Assert.Equal(2, result.First().Errors.Count());
        }

        [Fact]
        public void Chain_Multiple_Merges()
        {
            var firstSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(6, WidgetErrorCode.CodeNotUnique, "1 for position 6"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.CodeNotUnique, "1a for position 0"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.DescriptionTooLong, "1b for position 0"),
                new ErrorRecord<WidgetErrorCode>(1, WidgetErrorCode.CodeNotUnique, "1 for position 1"),
            };

            var secondSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(4, WidgetErrorCode.NameRequired, "2 for position 4"),
                new ErrorRecord<WidgetErrorCode>(2, WidgetErrorCode.NameRequired, "2 for position 2"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.NameRequired, "2 for position 0"),
                new ErrorRecord<WidgetErrorCode>(6, WidgetErrorCode.NameRequired, "2 for position 6"),
            };

            var thirdSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(6, WidgetErrorCode.CodeNotUnique, "1 for position 6"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.CodeNotUnique, "1a for position 0"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.DescriptionTooLong, "1b for position 0"),
                new ErrorRecord<WidgetErrorCode>(1, WidgetErrorCode.CodeNotUnique, "1 for position 1"),
            };

            var fourthSet = new ErrorRecords<WidgetErrorCode>()
            {
                new ErrorRecord<WidgetErrorCode>(4, WidgetErrorCode.NameRequired, "2 for position 4"),
                new ErrorRecord<WidgetErrorCode>(2, WidgetErrorCode.NameRequired, "2 for position 2"),
                new ErrorRecord<WidgetErrorCode>(0, WidgetErrorCode.NameRequired, "2 for position 0"),
                new ErrorRecord<WidgetErrorCode>(6, WidgetErrorCode.NameRequired, "2 for position 6"),
            };

            var result = firstSet.Concat(secondSet).Concat(thirdSet).Merge(fourthSet).ToList();

            Assert.Equal(5, result.Count);
            Assert.Equal(0, result.First().OrdinalPosition);
            Assert.Equal(6, result.First().Errors.Count());
            Assert.Equal(4, result.First(x => x.OrdinalPosition == 6).Errors.Count());
        }
    }
}
