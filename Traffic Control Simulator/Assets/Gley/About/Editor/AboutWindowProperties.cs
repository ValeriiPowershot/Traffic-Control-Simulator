using Gley.Common;
namespace Gley.About
{
    public class AboutWindowProperties : ISettingsWindowProperties
    {
        public const string menuItem = "Tools/Gley/About";

        public string versionFilePath => "/Scripts/Version.txt";

        public string windowName => "About - v.";

        public int minWidth => 600;

        public int minHeight => 520;

        public string folderName => "About";

        public string parentFolder => "Gley";
    }
}