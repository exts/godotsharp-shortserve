using Godot;
using Godot.Collections;
using Newtonsoft.Json;

namespace ShortServe.App.Core
{
    public class Highscore
    {
        private const string A = "08";
        private const string B = "12";
        private const string C = "20";
        private const string D = "18";
        public const string DataFile = "user://gamedata.bin";
        
        public void CreateFile(string key, int score)
        {
            var file = new File();
            var error = file.OpenEncryptedWithPass(DataFile, File.ModeFlags.Write, key);
            if(error == Error.Ok)
            {
                var data = new System.Collections.Generic.Dictionary<string, int>
                {
                    {"highscore", score}
                };

                var json = JsonConvert.SerializeObject(data);
                file.StoreString(json);
            }
            file.Close();
        }

        public void SaveHighscore(int score)
        {
            var f = A + B + C + D;
            CreateFile(f, score);
        }
        
        public int LoadHighscore()
        {
            var e = A + B + C + D;
            
            var gamedata = new File();
            if(!gamedata.FileExists(DataFile))
            {
                CreateFile(e, 0);
                return 0;
            }

            var highscore = 0;
            var error = gamedata.OpenEncryptedWithPass(DataFile, File.ModeFlags.Read, e);
            if(error == Error.Ok)
            {
                var json = gamedata.GetAsText();
                var data = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);

                if(data.ContainsKey("highscore"))
                {
                    highscore = data["highscore"];
                }
            }
            
            gamedata.Close();

            return highscore;
        }
    }
}
