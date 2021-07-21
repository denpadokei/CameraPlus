using System.IO;

namespace CameraPlus.Configuration
{
    public class RootConfig
    {
        public string FilePath { get; }

        public bool ProfileSceneChange = false;
        public string MenuProfile = "";
        public string GameProfile = "";
        public string RotateProfile = "";
        public string MultiplayerProfile = "";
        public bool ProfileLoadCopyMethod = false;
        public int CameraQuadLayer = 0;
        public bool ScreenFillBlack = false;

        public RootConfig(string filePath)
        {
            FilePath = filePath;

            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            if (File.Exists(FilePath))
                ConfigSerializer.LoadConfig(this, FilePath);

            PluginConfig.Instance.ProfileSceneChange = ProfileSceneChange;
            PluginConfig.Instance.MenuProfile = MenuProfile;
            PluginConfig.Instance.GameProfile = GameProfile;
            PluginConfig.Instance.RotateProfile = RotateProfile;
            PluginConfig.Instance.MultiplayerProfile = MultiplayerProfile;
            PluginConfig.Instance.ProfileLoadCopyMethod = ProfileLoadCopyMethod;
            PluginConfig.Instance.CameraQuadLayer = CameraQuadLayer;
            PluginConfig.Instance.ScreenFillBlack = ScreenFillBlack;
        }
    }
}
