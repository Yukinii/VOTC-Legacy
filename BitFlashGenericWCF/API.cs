using System.Collections.Generic;
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
    public static class API
    {
        private static readonly List<string> APIKeys = new List<string>(10000); 
        public static bool Validate(string apiKey)
        {
            return APIKeys.Contains(apiKey);
        }

        public static void Initialize()
        {
            APIKeys.Add("6c793695171e793d7d0080ad7700a2bc50256912cef2492c201e8ecc54b24ab5");
        }
    }
}
