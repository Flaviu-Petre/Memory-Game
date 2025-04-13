using MemoryGame.Utilities;
using MemoryGame.Views;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace MemoryGame
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AvatarSetupUtility.EnsureAvatarImages();
        }
    }
}
