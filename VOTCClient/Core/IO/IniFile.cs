using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace VOTCClient.Core.IO
{
    public class IniFile
    {
        #region "Declarations"

        // *** Lock for thread-safe access to file and local cache ***
        private readonly object _lock = new object();

        // *** File name ***
        private string _fileName;

        // *** Lazy loading flag ***
        private bool _lazy;

        // *** Automatic flushing flag ***
        private bool _autoFlush;

        // *** Local cache ***
        public readonly Dictionary<string, Dictionary<string, string>> Sections = new Dictionary<string, Dictionary<string, string>>();
        private readonly Dictionary<string, Dictionary<string, string>> _modified = new Dictionary<string, Dictionary<string, string>>();

        // *** Local cache modified flag ***
        private bool _cacheModified;

        #endregion

        #region "Methods"

        // *** Constructor ***
        public IniFile(string fileName)
        {
            Initialize(fileName, false);
        }

        // *** Initialization ***
        private void Initialize(string fileName, bool lazy)
        {
            _fileName = fileName;
            _lazy = lazy;
            _autoFlush = true;
            if (!_lazy) Refresh();
        }

        // *** Parse section name ***
        private static string ParseSectionName(string line)
        {
            if (!line.StartsWith("[")) return null;
            if (!line.EndsWith("]")) return null;
            return line.Length < 3 ? null : line.Substring(1, line.Length - 2);
        }

        // *** Parse key+value pair ***
        private static bool ParseKeyValuePair(string line, ref string key, ref string value)
        {
            // *** Check for key+value pair ***
            int I;
            if ((I = line.IndexOf('=')) <= 0) return false;

            var j = line.Length - I - 1;
            key = line.Substring(0, I).Trim();
            if (key.Length <= 0) return false;

            value = (j > 0) ? (line.Substring(I + 1, j).Trim()) : ("");
            return true;
        }

        // *** Read file contents into local cache ***
        private void Refresh()
        {
            lock (_lock)
            {
                StreamReader sr = null;
                try
                {
                    // *** Clear local cache ***
                    Sections.Clear();
                    _modified.Clear();

                    // *** Open the INI file ***
                    try
                    {
                        sr = !File.Exists(_fileName) ? new StreamReader(File.Create(_fileName)) : new StreamReader(_fileName);
                    }
                    catch
                    {
                        return;
                    }

                    // *** Read up the file content ***
                    Dictionary<string, string> currentSection = null;
                    string s;
                    string key = null;
                    string value = null;
                    while ((s = sr.ReadLine()) != null)
                    {
                        s = s.Trim();

                        // *** Check for section names ***
                        var sectionName = ParseSectionName(s);
                        if (sectionName != null)
                        {
                            // *** Only first occurrence of a section is loaded ***
                            if (Sections.ContainsKey(sectionName))
                            {
                                currentSection = null;
                            }
                            else
                            {
                                currentSection = new Dictionary<string, string>();
                                Sections.Add(sectionName, currentSection);
                            }
                        }
                        else if (currentSection != null)
                        {
                            // *** Check for key+value pair ***
                            if (!ParseKeyValuePair(s, ref key, ref value)) continue;
                            // *** Only first occurrence of a key is loaded ***
                            if (!currentSection.ContainsKey(key))
                            {
                                currentSection.Add(key, value);
                            }
                        }
                    }
                }
                finally
                {
                    // *** Cleanup: close file ***
                    sr?.Close();
                }
            }
        }

        // *** Flush local cache content ***
        public void Flush()
        {
            lock (_lock)
            {
                PerformFlush();
            }
        }

        private void PerformFlush()
        {
            try
            {
                // *** If local cache was not modified, exit ***
                if (!_cacheModified) return;
                _cacheModified = false;

                // *** Check if original file exists ***
                var originalFileExists = File.Exists(_fileName);

                // *** Get temporary file name ***
                var tmpFileName = Path.ChangeExtension(_fileName, "$n$");

                // *** Copy content of original file to temporary file, replace modified values ***

                // *** Create the temporary file ***

                while (File.Exists(tmpFileName))
                {
                    try
                    {
                        File.Delete(tmpFileName);
                    }
                    catch
                    {
                        //Kernel.UI.DisplayCmd("Fail.");
                    }
                }
                var sw = new StreamWriter(tmpFileName) { AutoFlush = true };

                try
                {
                    Dictionary<string, string> currentSection = null;
                    if (originalFileExists)
                    {
                        StreamReader sr = null;
                        try
                        {
                            // *** Open the original file ***
                            sr = new StreamReader(_fileName);

                            // *** Read the file original content, replace changes with local cache values ***
                            string key = null;
                            string value = null;
                            var reading = true;
                            while (reading)
                            {
                                var s = sr.ReadLine();
                                reading = (s != null);

                                // *** Check for end of file ***
                                bool unmodified;
                                string sectionName;
                                if (reading)
                                {
                                    unmodified = true;
                                    s = s.Trim();
                                    sectionName = ParseSectionName(s);
                                }
                                else
                                {
                                    unmodified = false;
                                    sectionName = null;
                                }

                                // *** Check for section names ***
                                if ((sectionName != null) || (!reading))
                                {
                                    // *** Write all remaining modified values before leaving a section ****
                                    if (currentSection?.Count > 0)
                                    {
                                        var section = currentSection;
                                        foreach (var fkey in currentSection.Keys.Where(fkey => section.TryGetValue(fkey, out value)))
                                        {
                                            sw.Write(fkey);
                                            sw.Write('=');
                                            sw.WriteLine(value);
                                        }
                                        //sw.WriteLine();
                                        currentSection.Clear();
                                    }

                                    if (reading)
                                    {
                                        // *** Check if current section is in local modified cache ***
                                        if (!_modified.TryGetValue(sectionName, out currentSection))
                                        {
                                        }
                                    }
                                }
                                else if (currentSection != null)
                                {
                                    // *** Check for key+value pair ***
                                    if (ParseKeyValuePair(s, ref key, ref value))
                                    {
                                        if (currentSection.TryGetValue(key, out value))
                                        {
                                            // *** Write modified value to temporary file ***
                                            unmodified = false;
                                            currentSection.Remove(key);

                                            sw.Write(key);
                                            sw.Write('=');
                                            sw.WriteLine(value);
                                        }
                                    }
                                }

                                // *** Write unmodified lines from the original file ***
                                if (unmodified)
                                {
                                    sw.WriteLine(s);
                                }
                            }

                            // *** Close the original file ***
                            sr.Close();
                            sr = null;
                        }
                        finally
                        {
                            // *** Cleanup: close files ***                  
                            sr?.Close();
                        }
                    }

                    // *** Cycle on all remaining modified values ***
                    foreach (var sectionPair in _modified)
                    {
                        currentSection = sectionPair.Value;
                        if (currentSection.Count <= 0) continue;
                        //sw.WriteLine();

                        // *** Write the section name ***
                        sw.Write('[');
                        sw.Write(sectionPair.Key);
                        sw.WriteLine(']');

                        // *** Cycle on all key+value pairs in the section ***
                        foreach (var valuePair in currentSection)
                        {
                            // *** Write the key+value pair ***
                            sw.Write(valuePair.Key);
                            sw.Write('=');
                            sw.WriteLine(valuePair.Value);
                        }
                        currentSection.Clear();
                    }
                    _modified.Clear();

                    // *** Close the temporary file ***
                    sw.Close();
                    sw = null;

                    // *** Rename the temporary file ***
                    File.Copy(tmpFileName, _fileName, true);

                    // *** Delete the temporary file ***
                    File.Delete(tmpFileName);
                }
                finally
                {
                    // *** Cleanup: close files ***                  
                    sw?.Close();
                }
            }
            catch (Exception e)
            {
                Kernel.UI.DisplayCmd(e.StackTrace);
            }

        }

        // *** Read a value from local cache ***
        public string ReadString(string sectionName, string key, string defaultValue)
        {
            // *** Lazy loading ***
            if (_lazy)
            {
                _lazy = false;
                Refresh();
            }

            lock (_lock)
            {
                // *** Check if the section exists ***
                Dictionary<string, string> section;
                if (!Sections.TryGetValue(sectionName, out section)) return defaultValue;

                // *** Check if the key exists ***
                string value;
                if (section.TryGetValue(key, out value))
                    return value;
                return defaultValue;

                // *** Return the found value ***
            }
        }
        public string ReadString(string sectionName, string key)
        {
            Refresh();
            // *** Lazy loading ***
            if (_lazy)
            {
                _lazy = false;
                Refresh();
            }

            lock (_lock)
            {
                // *** Check if the section exists ***
                Dictionary<string, string> section;
                if (!Sections.TryGetValue(sectionName, out section)) return "";

                // *** Check if the key exists ***
                string value;
                return !section.TryGetValue(key, out value) ? "nil" : value;

                // *** Return the found value ***
            }
        }

        // *** Insert or modify a value in local cache ***
        public void Write(string sectionName, string key, object value)
        {
            // *** Lazy loading ***
            if (_lazy)
            {
                _lazy = false;
                Refresh();
            }

            lock (_lock)
            {
                // *** Flag local cache modification ***
                _cacheModified = true;

                // *** Check if the section exists ***
                Dictionary<string, string> section;
                if (!Sections.TryGetValue(sectionName, out section))
                {
                    // *** If it doesn't, add it ***
                    section = new Dictionary<string, string>();
                    Sections.Add(sectionName, section);
                }

                // *** Modify the value ***
                if (section.ContainsKey(key)) section.Remove(key);
                section.Add(key, Convert.ToString(value));

                // *** Add the modified value to local modified values cache ***
                if (!_modified.TryGetValue(sectionName, out section))
                {
                    section = new Dictionary<string, string>();
                    _modified.Add(sectionName, section);
                }

                if (section.ContainsKey(key)) section.Remove(key);
                section.Add(key, Convert.ToString(value));

                // *** Automatic flushing : immediately write any modification to the file ***
                if (_autoFlush)
                    PerformFlush();
            }
        }

        // *** Encode byte array ***

        // *** Decode byte array ***

        // *** Getters for various types ***
        public bool GetValue(string sectionName, string key, bool defaultValue)
        {
            var stringValue = ReadString(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            int value;
            if (int.TryParse(stringValue, out value)) return (value != 0);
            return defaultValue;
        }

        public int ReadInt(string sectionName, string key, int defaultValue)
        {
            var stringValue = ReadString(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            int value;
            return int.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value) ? value : defaultValue;
        }
        public byte ReadByte(string sectionName, string key, byte defaultValue)
        {
            var stringValue = ReadString(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            byte value;
            return byte.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value) ? value : defaultValue;
        }
        public ushort ReadShort(string sectionName, string key, ushort defaultValue)
        {
            var stringValue = ReadString(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            ushort value;
            return ushort.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value) ? value : defaultValue;
        }

        public DateTime GetValue(string sectionName, string key, DateTime defaultValue)
        {
            var stringValue = ReadString(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            DateTime value;
            return DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out value) ? value : defaultValue;
        }
        public bool GetBool(string sectionName, string key, bool defaultValue)
        {
            var stringValue = ReadString(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            return (stringValue == "true");
        }

        // *** Setters for various types ***
        public void SetValue(string sectionName, string key, bool value)
        {
            Write(sectionName, key, (value) ? ("true") : ("false"));
        }

        public void SetValue(string sectionName, string key, string value)
        {
            Write(sectionName, key, value);
        }

        public void SetValue(string sectionName, string key, int value)
        {
            Write(sectionName, key, value.ToString(CultureInfo.InvariantCulture));
        }

        public void SetValue(string sectionName, string key, DateTime value)
        {
            Write(sectionName, key, value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        public double ReadDouble(string sectionName, string key, double defaultValue = 0)
        {
            var stringValue = ReadString(sectionName, key, defaultValue.ToString(CultureInfo.InvariantCulture));
            double value;
            return double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value) ? value : defaultValue;

        }
    }
}
