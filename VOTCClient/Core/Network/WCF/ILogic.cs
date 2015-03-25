using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace VOTCClient.Core.Network.WCF
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(ConfigurationName="ILogic")]
    public interface ILogic
    {
    
        [OperationContract(Action="http://tempuri.org/ILogic/TestRemoteExecution", ReplyAction="http://tempuri.org/ILogic/TestRemoteExecutionResponse")]
        string TestRemoteExecution(int value, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/TestRemoteExecution", ReplyAction="http://tempuri.org/ILogic/TestRemoteExecutionResponse")]
        Task<string> TestRemoteExecutionAsync(int value, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/DownloadScript", ReplyAction="http://tempuri.org/ILogic/DownloadScriptResponse")]
        string DownloadScript(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/DownloadScript", ReplyAction="http://tempuri.org/ILogic/DownloadScriptResponse")]
        Task<string> DownloadScriptAsync(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/UploadScript", ReplyAction="http://tempuri.org/ILogic/UploadScriptResponse")]
        string UploadScript(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/UploadScript", ReplyAction="http://tempuri.org/ILogic/UploadScriptResponse")]
        Task<string> UploadScriptAsync(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/StoreBadge", ReplyAction="http://tempuri.org/ILogic/StoreBadgeResponse")]
        string StoreBadge(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/StoreBadge", ReplyAction="http://tempuri.org/ILogic/StoreBadgeResponse")]
        Task<string> StoreBadgeAsync(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/StoreHeader", ReplyAction="http://tempuri.org/ILogic/StoreHeaderResponse")]
        string StoreHeader(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/StoreHeader", ReplyAction="http://tempuri.org/ILogic/StoreHeaderResponse")]
        Task<string> StoreHeaderAsync(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetScriptHash", ReplyAction="http://tempuri.org/ILogic/GetScriptHashResponse")]
        string GetScriptHash(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetScriptHash", ReplyAction="http://tempuri.org/ILogic/GetScriptHashResponse")]
        Task<string> GetScriptHashAsync(string scriptName, string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/LiveStats", ReplyAction="http://tempuri.org/ILogic/LiveStatsResponse")]
        List<string> LiveStats(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/LiveStats", ReplyAction="http://tempuri.org/ILogic/LiveStatsResponse")]
        Task<List<string>> LiveStatsAsync(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetMostDownloadedScripts", ReplyAction="http://tempuri.org/ILogic/GetMostDownloadedScriptsResponse")]
        List<string> GetMostDownloadedScripts(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetMostDownloadedScripts", ReplyAction="http://tempuri.org/ILogic/GetMostDownloadedScriptsResponse")]
        Task<List<string>> GetMostDownloadedScriptsAsync(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetNewestScripts", ReplyAction="http://tempuri.org/ILogic/GetNewestScriptsResponse")]
        List<string> GetNewestScripts(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetNewestScripts", ReplyAction="http://tempuri.org/ILogic/GetNewestScriptsResponse")]
        Task<List<string>> GetNewestScriptsAsync(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetHarmfulScripts", ReplyAction="http://tempuri.org/ILogic/GetHarmfulScriptsResponse")]
        List<string> GetHarmfulScripts(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetHarmfulScripts", ReplyAction="http://tempuri.org/ILogic/GetHarmfulScriptsResponse")]
        Task<List<string>> GetHarmfulScriptsAsync(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetMostPopularScripts", ReplyAction="http://tempuri.org/ILogic/GetMostPopularScriptsResponse")]
        List<string> GetMostPopularScripts(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetMostPopularScripts", ReplyAction="http://tempuri.org/ILogic/GetMostPopularScriptsResponse")]
        Task<List<string>> GetMostPopularScriptsAsync(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetFeaturedScripts", ReplyAction="http://tempuri.org/ILogic/GetFeaturedScriptsResponse")]
        List<string> GetFeaturedScripts(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetFeaturedScripts", ReplyAction="http://tempuri.org/ILogic/GetFeaturedScriptsResponse")]
        Task<List<string>> GetFeaturedScriptsAsync(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetUpdatedScripts", ReplyAction="http://tempuri.org/ILogic/GetUpdatedScriptsResponse")]
        List<string> GetUpdatedScripts(string apiKey);
    
        [OperationContract(Action="http://tempuri.org/ILogic/GetUpdatedScripts", ReplyAction="http://tempuri.org/ILogic/GetUpdatedScriptsResponse")]
        Task<List<string>> GetUpdatedScriptsAsync(string apiKey);

        [OperationContractAttribute(Action = "http://tempuri.org/ILogic/PostChatMessage", ReplyAction = "http://tempuri.org/ILogic/PostChatMessageResponse")]
        void PostChatMessage(string json);

        [OperationContractAttribute(Action = "http://tempuri.org/ILogic/PostChatMessage", ReplyAction = "http://tempuri.org/ILogic/PostChatMessageResponse")]
        Task PostChatMessageAsync(string json);

        [OperationContractAttribute(Action = "http://tempuri.org/ILogic/GetQuote", ReplyAction = "http://tempuri.org/ILogic/GetQuoteResponse")]
        string GetQuote();

        [OperationContractAttribute(Action = "http://tempuri.org/ILogic/GetQuote", ReplyAction = "http://tempuri.org/ILogic/GetQuoteResponse")]
        Task<string> GetQuoteAsync();
    }
}