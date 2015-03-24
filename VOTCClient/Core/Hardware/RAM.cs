using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
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
namespace VOTCClient.Core.Hardware
{
    public static class RAM
    {
        public static IEnumerable<float?> GetValues(SensorType type)
        {
            return from sensor in HardwareInterface.RAMSensors where sensor.SensorType == type select sensor.Value;
        }

        public static async Task<int> GetFreeMemoryAsync()
        {
            return await Task.Run(() => (from value in GetValues(SensorType.Load) where value != null select (int)value.Value).FirstOrDefault());
        }
        public static async Task<int> GetUsedMemoryAsync()
        {
            return await Task.Run(() => (from value in GetValues(SensorType.Load) where value != null select (int)value.Value).FirstOrDefault());
        }
    }
}
