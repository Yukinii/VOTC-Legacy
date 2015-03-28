using System; //VOTC LEGACY
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
	
	class Pandorabots: IChatterBot {
		private readonly string _botid;
		
		public Pandorabots(string botid) {
			_botid = botid;
		}
		
		public IChatterBotSession CreateSession() {
			return new PandorabotsSession(_botid);
		}
	}
	
	class PandorabotsSession: IChatterBotSession {
		private readonly IDictionary<string, string> _vars;
		
		public PandorabotsSession(string botid) {
			_vars = new Dictionary<string, string>();
			_vars["botid"] = botid;
			_vars["custid"] = Guid.NewGuid().ToString();
		}
		
		public ChatterBotThought Think(ChatterBotThought thought) {
			_vars["input"] = thought.Text;
			
			var response = Utils.Post("http://www.pandorabots.com/pandora/talk-xml", _vars);
			
			var responseThought = new ChatterBotThought();
			responseThought.Text = Utils.XPathSearch(response, "//result/that/text()");
			
			return responseThought;
		}
		
		public string Think(string text) {
			return Think(new ChatterBotThought { Text = text }).Text;
		}
	}
}