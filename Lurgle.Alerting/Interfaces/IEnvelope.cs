using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lurgle.Alerting
{
    public interface IEnvelope : IHideObjectMembers
    {
        IEnvelope To(string toAddress, string toName = null, AddressType addressType = AddressType.Email);
        IEnvelope To(string[] emailList);
        IEnvelope To(List<string> emailList);
        IEnvelope To(Dictionary<string, string> emailList);
        IEnvelope Cc(string ccAddress, string ccName = null, AddressType addressType = AddressType.Email);
        IEnvelope Cc(string[] emailList);
        IEnvelope Cc(List<string> emailList);
        IEnvelope Cc(Dictionary<string, string> emailList);
        IEnvelope Bcc(string bccAddress, string bccName = null, AddressType addressType = AddressType.Email);
        IEnvelope Bcc(string[] emailList);
        IEnvelope Bcc(List<string> emailList);
        IEnvelope Bcc(Dictionary<string, string> emailList);
        IEnvelope ReplyTo(string replyToAddress = null, string replyToName = null, AddressType addressType = AddressType.Email);
        IEnvelope Subject(string subjectText = null, params object[] args);
        IEnvelope Priority(AlertLevel alertLevel);
        IEnvelope Attach(string filePath, string contentType = null);
        IEnvelope Attach(string[] fileList);
        IEnvelope Attach(List<string> fileList);
        bool Send(string msg, params object[] args);
        Task<bool> SendAsync(string msg, params object[] args);
        bool SendTemplate<T>(string template, T templateModel, bool isHtml = true);
        Task<bool> SendTemplateAsync<T>(string template, T templateModel, bool isHtml = true);
        bool SendTemplateFile<T>(string templateConfig, T templateModel, bool isHtml = true);
        Task<bool> SendTemplateFileAsync<T>(string templateConfig, T templateModel, bool isHtml = true);
    }
}
