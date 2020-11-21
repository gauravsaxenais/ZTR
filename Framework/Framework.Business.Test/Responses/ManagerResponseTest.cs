namespace ZTR.Framework.Business.Test.Responses
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget;
    using ZTR.Framework.Business.Test.FixtureSetup.Business.Widget.Models;
    using ZTR.Framework.Business.Test.FixtureSetup.Fakes;
    using ZTR.Framework.Test;
    using Newtonsoft.Json;
    using Xunit;

    public sealed class ManagerResponseTest
    {
        [Fact]
        public void ManagerResponseWithIdsShouldSerialize()
        {
            var widgetFaker = new WidgetFaker<WidgetReadModel>();
            var models = widgetFaker.Generate(5);

            var managerResponse = new ManagerResponse<WidgetErrorCode>(models.Select(x => x.Id));

            var newtonSerialized = JsonConvert.SerializeObject(managerResponse);
            var newtonDeserialized = JsonConvert.DeserializeObject<ManagerResponse<WidgetErrorCode>>(newtonSerialized);

            var xmlSerializer = new XmlSerializer(typeof(ManagerResponse<WidgetErrorCode>));
            var stringBuilder = new StringBuilder();

            using (TextWriter writer = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(writer, managerResponse);
            }

            var xmlSerialized = stringBuilder.ToString();
            object output = null;
            using (TextReader reader = new StringReader(xmlSerialized))
            {
                output = xmlSerializer.Deserialize(reader);
            }

            var xmlDeserialized = (ManagerResponse<WidgetErrorCode>)output;

            AssertExtensions.Equivalent(managerResponse, newtonDeserialized);
            AssertExtensions.Equivalent(managerResponse, xmlDeserialized);
        }

        [Fact]
        public void ManagerResponseWithErrorsShouldSerialize()
        {
            var errorRecords = new List<ErrorRecord<WidgetErrorCode>>
            {
                new ErrorRecord<WidgetErrorCode>(new NotImplementedException()),
                new ErrorRecord<WidgetErrorCode>(12, WidgetErrorCode.CodeTooLong, "A custom message here."),
                new ErrorRecord<WidgetErrorCode>(89, new ErrorMessage<WidgetErrorCode>(WidgetErrorCode.IdDoesNotExist, new InvalidCastException())),
                new ErrorRecord<WidgetErrorCode>(WidgetErrorCode.NameTooLong, new OperationCanceledException()),
                new ErrorRecord<WidgetErrorCode>(WidgetErrorCode.DescriptionRequired, "Another message."),
                new ErrorRecord<WidgetErrorCode>(45, 344, new ErrorMessage<WidgetErrorCode>(WidgetErrorCode.IdDoesNotExist, "yup yup")),
                new ErrorRecord<WidgetErrorCode>(55654, 6546546, WidgetErrorCode.CategoryNotFound, "sdsdsddsdsdsewre")
            };

            var managerResponse = new ManagerResponse<WidgetErrorCode>(new ErrorRecords<WidgetErrorCode>(errorRecords));

            var newtonSerialized = JsonConvert.SerializeObject(managerResponse);
            var newtonDeserialized = JsonConvert.DeserializeObject<ManagerResponse<WidgetErrorCode>>(newtonSerialized);

            var xmlSerializer = new XmlSerializer(typeof(ManagerResponse<WidgetErrorCode>));
            var stringBuilder = new StringBuilder();

            using (TextWriter writer = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(writer, managerResponse);
            }

            var xmlSerialized = stringBuilder.ToString();
            object output = null;
            using (TextReader reader = new StringReader(xmlSerialized))
            {
                output = xmlSerializer.Deserialize(reader);
            }

            var xmlDeserialized = (ManagerResponse<WidgetErrorCode>)output;

            AssertExtensions.Equivalent(managerResponse, newtonDeserialized);
            AssertExtensions.Equivalent(managerResponse, xmlDeserialized);
        }
    }
}
