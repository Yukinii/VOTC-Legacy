using System.Windows.Forms;

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
namespace VOTCClient.Core.Extensions
{
    internal class KonamiSequence
    {
        readonly Keys[] _code = { Keys.Up, Keys.Up, Keys.Down, Keys.Down, Keys.Left, Keys.Right, Keys.Left, Keys.Right, Keys.B, Keys.A };

        private int _offset;
        private readonly int _length, _target;

        public KonamiSequence()
        {
            _length = _code.Length - 1;
            _target = _code.Length;
        }

        public bool IsCompletedBy(Keys key)
        {
            _offset %= _target;

            if (key == _code[_offset]) _offset++;
            else if (key == _code[0]) _offset = 2;  // repeat index

            return _offset > _length;
        }
    }
}
