using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
/*
    This file is part of VOTC.

    VOTC is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    VOTC is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with VOTC.  If not, see <http://www.gnu.org/licenses/>.
*/
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