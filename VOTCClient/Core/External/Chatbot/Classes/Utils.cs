using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml.XPath;

/*
    ChatterBotAPI
    Copyright (C) 2011 pierredavidbelanger@gmail.com
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace VOTCClient.Core.External.Chatbot.Classes {
	
	static class Utils {
		
		public static string ParametersToWwwFormUrlEncoded(IDictionary<string, string> parameters) {
			string wwwFormUrlEncoded = null;
			foreach (var parameterKey in parameters.Keys) {
				var parameterValue = parameters[parameterKey];
				var parameter = string.Format("{0}={1}", HttpUtility.UrlEncode(parameterKey), HttpUtility.UrlEncode(parameterValue));
				if (wwwFormUrlEncoded == null) {
					wwwFormUrlEncoded = parameter;
				} else {
					wwwFormUrlEncoded = string.Format("{0}&{1}", wwwFormUrlEncoded, parameter);
				}
			}
			return wwwFormUrlEncoded;
		}
		
		public static string Md5(string input) {
			return FormsAuthentication.HashPasswordForStoringInConfigFile(input, "MD5");
		}
		
		public static string Post(string url, IDictionary<string, string> parameters) {
			var postData = ParametersToWwwFormUrlEncoded(parameters);
			var postDataBytes = Encoding.ASCII.GetBytes(postData);
			
			var webRequest = WebRequest.Create(url);
			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ContentLength = postDataBytes.Length;
			
			var outputStream = webRequest.GetRequestStream();
			outputStream.Write(postDataBytes, 0, postDataBytes.Length);
			outputStream.Close();
			
			var webResponse = webRequest.GetResponse();
			var responseStreamReader = new StreamReader(webResponse.GetResponseStream());
			return responseStreamReader.ReadToEnd().Trim();
		}
		
		public static string XPathSearch(string input, string expression) {
			var document = new XPathDocument(new MemoryStream(Encoding.ASCII.GetBytes(input)));
			var navigator = document.CreateNavigator();
			return navigator.SelectSingleNode(expression).Value.Trim();
		}
		
		public static string StringAtIndex(string[] strings, int index) {
			if (index >= strings.Length) return "";
			return strings[index];
		}
	}
}
