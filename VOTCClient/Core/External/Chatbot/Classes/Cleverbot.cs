using System.Collections.Generic;

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
	
	class Cleverbot: IChatterBot {
		private readonly string _url;
		private readonly int _endIndex;
		
		public Cleverbot(string url, int endIndex) {
			_url = url;
			_endIndex = endIndex;
		}
		
		public IChatterBotSession CreateSession() {
			return new CleverbotSession(_url, _endIndex);
		}
	}
	
	class CleverbotSession: IChatterBotSession {
		private readonly string _url;
		private readonly int _endIndex;
		private readonly IDictionary<string, string> _vars;
		
		public CleverbotSession(string url, int endIndex) {
			_url = url;
			_endIndex = endIndex;
			_vars = new Dictionary<string, string>();
			_vars["start"] = "y";
			_vars["icognoid"] = "wsf";
			_vars["fno"] = "0";
			_vars["sub"] = "Say";
			_vars["islearning"] = "1";
			_vars["cleanslate"] = "false";
		}
		
		public ChatterBotThought Think(ChatterBotThought thought) {
			_vars["stimulus"] = thought.Text;
			
			var formData = Utils.ParametersToWwwFormUrlEncoded(_vars);
			var formDataToDigest = formData.Substring(9, _endIndex);
			var formDataDigest = Utils.Md5(formDataToDigest);
			_vars["icognocheck"] = formDataDigest;
			
			var response = Utils.Post(_url, _vars);
			
			var responseValues = response.Split('\r');
			
			//vars[""] = Utils.StringAtIndex(responseValues, 0); ??
			_vars["sessionid"] = Utils.StringAtIndex(responseValues, 1);
			_vars["logurl"] = Utils.StringAtIndex(responseValues, 2);
			_vars["vText8"] = Utils.StringAtIndex(responseValues, 3);
			_vars["vText7"] = Utils.StringAtIndex(responseValues, 4);
			_vars["vText6"] = Utils.StringAtIndex(responseValues, 5);
			_vars["vText5"] = Utils.StringAtIndex(responseValues, 6);
			_vars["vText4"] = Utils.StringAtIndex(responseValues, 7);
			_vars["vText3"] = Utils.StringAtIndex(responseValues, 8);
			_vars["vText2"] = Utils.StringAtIndex(responseValues, 9);
			_vars["prevref"] = Utils.StringAtIndex(responseValues, 10);
			//vars[""] = Utils.StringAtIndex(responseValues, 11); ??
			_vars["emotionalhistory"] = Utils.StringAtIndex(responseValues, 12);
			_vars["ttsLocMP3"] = Utils.StringAtIndex(responseValues, 13);
			_vars["ttsLocTXT"] = Utils.StringAtIndex(responseValues, 14);
			_vars["ttsLocTXT3"] = Utils.StringAtIndex(responseValues, 15);
			_vars["ttsText"] = Utils.StringAtIndex(responseValues, 16);
			_vars["lineRef"] = Utils.StringAtIndex(responseValues, 17);
			_vars["lineURL"] = Utils.StringAtIndex(responseValues, 18);
			_vars["linePOST"] = Utils.StringAtIndex(responseValues, 19);
			_vars["lineChoices"] = Utils.StringAtIndex(responseValues, 20);
			_vars["lineChoicesAbbrev"] = Utils.StringAtIndex(responseValues, 21);
			_vars["typingData"] = Utils.StringAtIndex(responseValues, 22);
			_vars["divert"] = Utils.StringAtIndex(responseValues, 23);
			
			var responseThought = new ChatterBotThought();
			
			responseThought.Text = Utils.StringAtIndex(responseValues, 16);
			
			return responseThought;
		}
		
		public string Think(string text) {
			return Think(new ChatterBotThought { Text = text }).Text;
		}
	}
}