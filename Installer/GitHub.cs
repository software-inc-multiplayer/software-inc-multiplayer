using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Installer
{
    public class ReleaseTag
    {
        public string DownloadURL { get; set; }
        public ReleaseTag() { }
        public string Tag { get; set; }
        public bool PreRelease { get; set; }
        public bool Latest { get; set; }
        public int DownloadCount { get; set; }
    }
    public class GitHub
    {
        public static readonly string LocalLowPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Software Inc Multiplayer Mod/";
        public static Dictionary<string, ReleaseTag> allowedReleases = new Dictionary<string, ReleaseTag>() { };
        public static WebClient client = new WebClient();
        public static async Task GetReleases()
        {
            var client = new GitHubClient(new ProductHeaderValue(new Random().Next().ToString()));
            IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("cal3432", "software-inc-multiplayer");
            foreach (Release release in releases)
            {
                if (release.Name.ToLower().Contains("installer")) continue;
                foreach (ReleaseAsset asset in release.Assets)
                {
                    if (asset.Name != "installer-binaries.zip") continue;
                    ReleaseTag temp = new ReleaseTag();
                    temp.DownloadURL = asset.BrowserDownloadUrl;
                    temp.PreRelease = release.Prerelease;
                    temp.Tag = release.TagName;
                    temp.DownloadCount = asset.DownloadCount;
                    allowedReleases.Add(release.TagName, temp);
                }
            }
        }
        public static async Task InstallReleaseAsync(ReleaseTag release)
        {
            if (!Directory.Exists(LocalLowPath + release.Tag))
            {
                client.Headers.Add("user-agent", "Anything");
                if (!Directory.Exists(LocalLowPath))
                {
                    Directory.CreateDirectory(LocalLowPath);
                }
                await client.DownloadFileTaskAsync(new Uri(release.DownloadURL), LocalLowPath + "download.zip");
                ZipFile.ExtractToDirectory(LocalLowPath + "download.zip", LocalLowPath + release.Tag);
                DirectoryCopy(LocalLowPath + release.Tag + "/mf", Properties.Settings.Default.installDir + "/DLLMods/Multiplayer/", true);
                DirectoryCopy(LocalLowPath + release.Tag + "/manage", Properties.Settings.Default.installDir + "/Software Inc_Data/Managed", true);
                File.Delete(LocalLowPath + "download.zip");
                Directory.Delete(LocalLowPath + release.Tag, true);
            }
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (File.Exists(temppath))
                {
                    File.Delete(temppath);
                }
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
