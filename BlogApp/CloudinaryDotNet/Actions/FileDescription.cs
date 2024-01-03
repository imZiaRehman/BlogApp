using System.IO;

namespace CloudinaryDotNet.Actions
{
    internal class FileDescription : CloudinaryDotNet.FileDescription
    {
        public FileDescription(string name, Stream stream) : base(name, stream)
        {
        }
    }
}