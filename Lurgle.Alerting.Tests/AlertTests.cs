using System.IO;
using System.Reflection;
using FluentEmail.Core.Models;
using Xunit;
using Xunit.Abstractions;

namespace Lurgle.Alerting.Tests
{
    public class AlertTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public AlertTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            Alerting.SetConfig(new AlertConfig(appName: "TestMaster Prime", mailRenderer: RendererType.Liquid,
                mailSender: SenderType.MailKit, mailHost: "mail", mailPort: 25, mailFrom: "bob@builder.com",
                mailTo: "wendy@builder.com", mailDebug: "scoop@builder.com", mailSubject: "Can you fix it?",
                mailTemplatePath: Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
        }

        /// <summary>
        ///     Validate that the ToAddresses collection is correctly populated
        /// </summary>
        [Fact]
        public void CreateEmailTo()
        {
            var alert = Alert.To();
            Assert.Contains(new Address(Alerting.Config.MailTo), alert.ToAddresses);
            Assert.True(Equals(alert.FromAddress, new Address(Alerting.Config.MailFrom)));
        }

        /// <summary>
        ///     Validate that the From address is correctly populated, and no ToAddresses exist
        /// </summary>
        [Fact]
        public void CreateEmailFrom()
        {
            var alert = Alert.From();
            Assert.True(Equals(alert.FromAddress, new Address(Alerting.Config.MailFrom)));
            Assert.True(alert.ToAddresses.Count == 0);
        }

        /// <summary>
        ///     Validate that a subject is defined
        /// </summary>
        [Fact]
        public void CreateWithSubject()
        {
            var alert = Alert.From().To(Alerting.Config.MailTo).Subject("Test");
            Assert.False(alert.AlertSubject == Alerting.Config.MailSubject);
        }

        /// <summary>
        ///     Validate attaching a file
        /// </summary>
        [Fact]
        public void CreateWithAttachment()
        {
            var alert = Alert.From().Attach(Assembly.GetExecutingAssembly().Location);
            Assert.True(alert.Attachments.Count > 0);
        }

        /// <summary>
        ///     Validate CC and BCC
        /// </summary>
        [Fact]
        public void CreateCarbonCopies()
        {
            var alert = Alert.From().To(Alerting.Config.MailTo).Cc("scoop@builder.com").Bcc("rolly@builder.com");
            Assert.Contains(new Address("scoop@builder.com"), alert.CcAddresses);
            Assert.Contains(new Address("rolly@builder.com"), alert.BccAddresses);
        }

        /// <summary>
        ///     Validate Priority
        /// </summary>
        [Fact]
        public void SetPriority()
        {
            var alert = Alert.From().To(Alerting.Config.MailTo).Priority(AlertLevel.High);
            Assert.True(alert.AlertPriority == AlertLevel.High);
        }

        /// <summary>
        ///     Validate the template render and send processes
        /// </summary>
        [Fact]
        public void TemplateSendTest()
        {
            var alert = Alert.To().Subject("Test Liquid Template").SendTemplateFile("Liquid", new
            {
                Alerting.Config.AppName,
                Alerting.Config.AppVersion,
                MailRenderer = Alerting.Config.MailRenderer.ToString(),
                MailSender = Alerting.Config.MailSender.ToString(),
                Alerting.Config.MailTemplatePath,
                Alerting.Config.MailHost,
                MailTestTimeout = Alerting.Config.MailTestTimeout / 1000,
                Alerting.Config.MailPort,
                Alerting.Config.MailUseAuthentication,
                Alerting.Config.MailUsername,
                Alerting.Config.MailPassword,
                Alerting.Config.MailUseTls,
                MailTimeout = Alerting.Config.MailTimeout / 1000,
                Alerting.Config.MailFrom,
                Alerting.Config.MailTo,
                Alerting.Config.MailDebug,
                Alerting.Config.MailSubject
            });

            foreach (var error in alert.ErrorMessages)
                testOutputHelper.WriteLine("Error Output: {0}", error);
            Assert.True(!alert.Successful);
        }
    }
}