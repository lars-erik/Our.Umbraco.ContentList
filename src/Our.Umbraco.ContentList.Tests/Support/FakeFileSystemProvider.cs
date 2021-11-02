using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public class FakeFileSystemProvider : IFileProvider
    {
        private readonly DirectoryInfo webDir;

        public FakeFileSystemProvider(string webFolderName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            
            var dir = new DirectoryInfo(basePath);
            while (dir.GetFiles("*.sln").Length == 0 && dir.Parent != null)
            {
                dir = dir.Parent;
            }

            if (dir.GetFiles(".sln").Length == 0 && dir.Parent == null)
            {
                throw new Exception("No solution folder found outside test domain base directory.");
            }
            var solutionDir = dir;

            webDir = solutionDir.GetDirectories(webFolderName).FirstOrDefault();
            if (webDir == null)
            {
                throw new Exception($"Web directory {webFolderName} not found under {solutionDir.FullName}");
            }
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var directPath = Path.Combine(webDir.FullName, subpath.TrimStart('/'));
            if (File.Exists(directPath))
            { 
                var fileInfo = new PhysicalFileInfo(new FileInfo(directPath));
                return fileInfo;
            }

            return null;
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
    }
}