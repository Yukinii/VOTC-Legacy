using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using VOTCClient.Core.Network.WCF;

namespace VOTCClient.Core.Network
{
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public class LogicClient : ClientBase<ILogic>, ILogic
    {
        public LogicClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
        {
        }
        public string TestRemoteExecution(int value, string apiKey)
        {
            return Channel.TestRemoteExecution(value, apiKey);
        }
    
        public Task<string> TestRemoteExecutionAsync(int value, string apiKey)
        {
            return Channel.TestRemoteExecutionAsync(value, apiKey);
        }
    
        public string DownloadScript(string scriptName, string apiKey)
        {
            return Channel.DownloadScript(scriptName, apiKey);
        }
    
        public Task<string> DownloadScriptAsync(string scriptName, string apiKey)
        {
            return Channel.DownloadScriptAsync(scriptName, apiKey);
        }
    
        public string UploadScript(string scriptName, string apiKey)
        {
            return Channel.UploadScript(scriptName, apiKey);
        }
    
        public Task<string> UploadScriptAsync(string scriptName, string apiKey)
        {
            return Channel.UploadScriptAsync(scriptName, apiKey);
        }
    
        public string StoreBadge(string scriptName, string apiKey)
        {
            return Channel.StoreBadge(scriptName, apiKey);
        }
    
        public Task<string> StoreBadgeAsync(string scriptName, string apiKey)
        {
            return Channel.StoreBadgeAsync(scriptName, apiKey);
        }
    
        public string StoreHeader(string scriptName, string apiKey)
        {
            return Channel.StoreHeader(scriptName, apiKey);
        }
    
        public Task<string> StoreHeaderAsync(string scriptName, string apiKey)
        {
            return Channel.StoreHeaderAsync(scriptName, apiKey);
        }
    
        public string GetScriptHash(string scriptName, string apiKey)
        {
            return Channel.GetScriptHash(scriptName, apiKey);
        }
    
        public Task<string> GetScriptHashAsync(string scriptName, string apiKey)
        {
            return Channel.GetScriptHashAsync(scriptName, apiKey);
        }
    
        public List<string> LiveStats(string apiKey)
        {
            return Channel.LiveStats(apiKey);
        }
    
        public Task<List<string>> LiveStatsAsync(string apiKey)
        {
            return Channel.LiveStatsAsync(apiKey);
        }
    
        public List<string> GetMostDownloadedScripts(string apiKey)
        {
            return Channel.GetMostDownloadedScripts(apiKey);
        }
    
        public Task<List<string>> GetMostDownloadedScriptsAsync(string apiKey)
        {
            return Channel.GetMostDownloadedScriptsAsync(apiKey);
        }
    
        public List<string> GetNewestScripts(string apiKey)
        {
            return Channel.GetNewestScripts(apiKey);
        }
    
        public Task<List<string>> GetNewestScriptsAsync(string apiKey)
        {
            return Channel.GetNewestScriptsAsync(apiKey);
        }
    
        public List<string> GetHarmfulScripts(string apiKey)
        {
            return Channel.GetHarmfulScripts(apiKey);
        }
    
        public Task<List<string>> GetHarmfulScriptsAsync(string apiKey)
        {
            return Channel.GetHarmfulScriptsAsync(apiKey);
        }
    
        public List<string> GetMostPopularScripts(string apiKey)
        {
            return Channel.GetMostPopularScripts(apiKey);
        }
    
        public Task<List<string>> GetMostPopularScriptsAsync(string apiKey)
        {
            return Channel.GetMostPopularScriptsAsync(apiKey);
        }
    
        public List<string> GetFeaturedScripts(string apiKey)
        {
            return Channel.GetFeaturedScripts(apiKey);
        }
    
        public Task<List<string>> GetFeaturedScriptsAsync(string apiKey)
        {
            return Channel.GetFeaturedScriptsAsync(apiKey);
        }
    
        public List<string> GetUpdatedScripts(string apiKey)
        {
            return Channel.GetUpdatedScripts(apiKey);
        }
    
        public Task<List<string>> GetUpdatedScriptsAsync(string apiKey)
        {
            return Channel.GetUpdatedScriptsAsync(apiKey);
        }
        public void PostChatMessage(string json)
        {
            Channel.PostChatMessage(json);
        }

        public Task PostChatMessageAsync(string json)
        {
            return Channel.PostChatMessageAsync(json);
        }

        public string GetQuote()
        {
            return Channel.GetQuote();
        }

        public Task<string> GetQuoteAsync()
        {
            return Channel.GetQuoteAsync();
        }
    }
}