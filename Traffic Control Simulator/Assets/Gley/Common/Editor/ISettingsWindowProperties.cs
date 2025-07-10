namespace Gley.Common
{
    public interface ISettingsWindowProperties
    {
        string versionFilePath { get; }
        string windowName { get;}
        int minWidth { get;}
        int minHeight { get;}

        string folderName { get; }
        string parentFolder { get; }
    }
}
