using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace BitFlashGenericWCF
{
    [ServiceContract]
    public interface ILogic
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string TestRemoteExecution(int value, string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string DownloadScript(string scriptName, string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string UploadScript(string scriptName, string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string StoreBadge(string scriptName, string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string StoreHeader(string scriptName, string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string GetScriptHash(string scriptName, string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        List<string> LiveStats(string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        List<string> GetMostDownloadedScripts(string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        List<string> GetNewestScripts(string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        List<string> GetHarmfulScripts(string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        List<string> GetMostPopularScripts(string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        List<string> GetFeaturedScripts(string apiKey);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        List<string> GetUpdatedScripts(string apiKey);
    }
}