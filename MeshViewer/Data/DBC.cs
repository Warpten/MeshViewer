using DBFilesClient.NET;
using MeshViewer.Data.Structures;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MeshViewer.Data
{
    public static class DBC
    {
        public static Storage<GameObjectDisplayInfoRec_C> GameObjectDisplayInfo { get; private set; }

        public static Storage<SpellRec_C> Spell { get; private set; }
        public static Storage<SpellAuraOptionsRec_C> SpellAuraOptions { get; private set; }

        public static async void AsyncInitialize(string gameDirectory)
        {
            await Task.Run(() =>
            {
                Spell = new Storage<SpellRec_C>(Path.Combine(gameDirectory, "dbc", "Spell.dbc"));
                SpellAuraOptions = new Storage<SpellAuraOptionsRec_C>(Path.Combine(gameDirectory, "dbc", "SpellAuraOptions.dbc"));

                GameObjectDisplayInfo = new Storage<GameObjectDisplayInfoRec_C>(Path.Combine(gameDirectory, "dbc", "GameObjectDisplayInfo.dbc"));
            });
        }

        public static Storage<T> TryGetStorage<T>() where T : class, new()
        {
            foreach (var memberInfo in typeof(DBC).GetProperties(BindingFlags.Public | BindingFlags.Static))
                if (memberInfo.PropertyType.GetGenericArguments()[0] == typeof(T))
                    return memberInfo.GetValue(null) as Storage<T>;

            return default(Storage<T>);
        }
    }
}
