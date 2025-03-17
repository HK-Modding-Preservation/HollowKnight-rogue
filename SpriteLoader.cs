using System.Reflection;
using System.IO;

namespace rogue;
internal static class SpriteLoader
{
    private static Sprite LoadSprite(string name, float PPU = 64)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        MemoryStream memoryStream = new((int)stream.Length);
        stream.CopyTo(memoryStream);
        stream.Close();
        var bytes = memoryStream.ToArray();
        memoryStream.Close();
        var texture2D = new Texture2D(0, 0);
        texture2D.LoadImage(bytes, true);
        return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), Vector2.one / 2, PPU);
    }
    internal static Dictionary<string, Sprite> resource_sprites = new();
    internal static void Init()
    {
        var files = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        foreach (var file in files)
        {

            Modding.Logger.Log(file);
            if (!file.EndsWith(".png")) continue;
            resource_sprites.Add(Path.GetFileNameWithoutExtension(file).Split('.').Last().ToLower(), LoadSprite(file));
            Modding.Logger.Log(file + "is completed");
        }
        resource_sprites.Add("hunter_mask", (Sprite)Resources.InstanceIDToObject(8948));
        resource_sprites.Add("love_key", (Sprite)Resources.InstanceIDToObject(8248));
        resource_sprites.Add("map", (Sprite)Resources.InstanceIDToObject(8232));
        resource_sprites.Add("map_with_quill", (Sprite)Resources.InstanceIDToObject(12928));
        resource_sprites.Add("trinket1", (Sprite)Resources.InstanceIDToObject(8230));
        resource_sprites.Add("trinket2", (Sprite)Resources.InstanceIDToObject(8420));
        resource_sprites.Add("trinket3", (Sprite)Resources.InstanceIDToObject(8282));
        resource_sprites.Add("trinket4", (Sprite)Resources.InstanceIDToObject(8242));
    }
    internal static Sprite GetSprite(string name)
    {
        if (resource_sprites.ContainsKey(name)) return resource_sprites[name];
        return null;
    }
}
