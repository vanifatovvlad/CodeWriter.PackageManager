using System;

namespace CodeWriter.PackageManager
{
    [Serializable]
    public class CodePackage
    {
        public string name;
        public string guid;
        public string[] references = new string[0];
    }
}