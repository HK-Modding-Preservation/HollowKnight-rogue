using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using Modding;
using Newtonsoft.Json;
using Satchel;
using Steamworks;
using UnityEngine;

namespace rogue;
public static class Lang
{
    public static Dictionary<string, Dictionary<string, string>> texts = new();

    public const string default_dict = "en";

    public const string no_that_key = "no that key";

    public static void init()
    {
        var files = typeof(Rogue).Assembly.GetManifestResourceNames();
        foreach (var file in files)
        {

            Modding.Logger.Log(file);
            if (!file.EndsWith(".json")) continue;
            var bytes = Assembly.GetExecutingAssembly().GetBytesFromResources(file);
            var temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.Text.Encoding.UTF8.GetString(bytes));
            texts.Add(Path.GetFileNameWithoutExtension(file).Split('.').Last().ToLower(), temp);
        }




    }
    public static string Localize(this string name)
    {
        string now_lan = Language.Language.CurrentLanguage().ToString().ToLower();
        foreach (var item in texts)
        {
            if (now_lan.Contains(item.Key))
            {
                if (item.Value.ContainsKey(name))
                {
                    return item.Value[name];
                }
                else
                {
                    return "?" + name + "?";
                }
            }
        }
        if (texts.ContainsKey(default_dict))
        {
            if (texts[default_dict].ContainsKey(name))
            {
                return texts[default_dict][name];
            }
            else
            {
                return no_that_key;
            }
        }
        return no_that_key;
    }

}