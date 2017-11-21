using DBFilesClient.NET;
using MeshViewer.Data.Structures;
using System.IO;
using System.Threading.Tasks;

namespace MeshViewer.Data
{
    public static class DBC
    {
        public static Storage<GameObjectDisplayInfoEntry> GameObjectDisplayInfo { get; private set; }

        public static async Task AsyncInitialize(string gameDirectory)
        {
            await Task.Run(() =>
            {
                GameObjectDisplayInfo = new Storage<GameObjectDisplayInfoEntry>(Path.Combine(gameDirectory, "dbc", "GameObjectDisplayInfo.dbc"));
            });
        }
    }
}
