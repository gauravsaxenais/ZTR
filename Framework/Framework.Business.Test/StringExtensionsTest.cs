namespace ZTR.Framework.Business.Test
{
    using System;
    using Xunit;

    public class StringExtensionsTest
    {
        [Fact]
        public void ToGuid_ShouldGenerateAGuid_Successfully()
        {
            const string test = "Hello, World!";

            // Note: Used an online generator to get the expected GUID format from here: http://www.miraclesalad.com/webtools/md5.php
            var expectedGuid = new Guid("65a8e27d8879283831b664bd8b7f0ad4");

            var result = test.ToGuid();

            Assert.Equal(expectedGuid, result);
        }

        [Fact]
        public void FirstLetter_ShouldReturnASmallLetter_Successfully()
        {
            const string test = "HELLO World!";

            var expectedResult = "hELLO World!";

            var result = test.ToLowerFirstCharacter();

            Assert.Equal(expectedResult, result);
        }
    }
}
