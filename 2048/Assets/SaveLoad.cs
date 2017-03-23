using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
#if NETFX_CORE
using StreamReader = WinRTLegacy.IO.StreamReader;
using StreamWriter = WinRTLegacy.IO.StreamWriter;
#else
using StreamReader = System.IO.StreamReader;
using StreamWriter = System.IO.StreamWriter;
#endif

public class SaveLoad 
{
    public static int highScore = 0;
    public static void Save(Tile[] tiles, int score)
    {
        if(tiles != null)
        {
            try
            {
                TilesCollection TS = new TilesCollection();
                TS.tsd = new TileSaveData[tiles.Length];
                for (int i = 0; i < tiles.Length; i++)
                {
                    TS.tsd[i] = new TileSaveData();
                    TS.tsd[i].i = i;
                    TS.tsd[i].value = tiles[i].Value;   
                }
                TS.score = score;
                HighScoreData hsd = new HighScoreData();
                hsd.highScore = score > highScore ? score : highScore;
                string saveJSON = UnityEngine.JsonUtility.ToJson(TS, true);
                string highJSON = UnityEngine.JsonUtility.ToJson(hsd);
                if (!File.Exists(Application.persistentDataPath + "/save.json"))
                {
                    File.Create(Application.persistentDataPath + "/save.json");
                }
                if (!File.Exists(Application.persistentDataPath + "/score.json"))
                {
                    File.Create(Application.persistentDataPath + "/score.json");
                }
                StreamWriter sr;
                if(score > highScore)
                {
                    sr = new StreamWriter(Application.persistentDataPath + "/score.json");
                    sr.Write(highJSON);
                    sr.Close();
                } 
                sr = new StreamWriter(Application.persistentDataPath + "/save.json");
                sr.Write(saveJSON);
                sr.Close();
            }
            catch
            {
                return;
            }
        }
        else
        {
            try
            {
                File.Delete(Application.persistentDataPath + "/save.json");
            }
            catch
            {
                
            }
        }
    }

    public static bool Load(out Tile[] tiles, out int score)
    {
        try
        {
            loadHighScore();
            tiles = null;
            score = 0;
            if (File.Exists(Application.persistentDataPath + "/save.json"))
            {
                StreamReader sr = new StreamReader(Application.persistentDataPath + "/save.json");
                string loadJSON = sr.ReadToEnd();
                sr.Close();
                if(loadJSON == "")
                {
                    File.Delete(Application.persistentDataPath + "/save.json");
                    return false;
                }
                TilesCollection TS = UnityEngine.JsonUtility.FromJson<TilesCollection>(loadJSON);
                tiles = new Tile[TS.tsd.Length];
                for (int i = 0; i < TS.tsd.Length; i++)
                {
                    tiles[i] = new Tile();
                    tiles[i].Value = TS.tsd[i].value;
                    tiles[i].Position = TS.tsd[i].i;
                }
                score = TS.score;
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
           score = 0;
           tiles = null;
           return false;
        }
    }

    private static void loadHighScore()
    {
        try
        {
            if(File.Exists(Application.persistentDataPath + "/score.json"))
            {
                StreamReader sr = new StreamReader(Application.persistentDataPath + "/score.json");
                string loadJSON = sr.ReadToEnd();
                sr.Close();
                highScore = UnityEngine.JsonUtility.FromJson<HighScoreData>(loadJSON).highScore;
            }
        }
        catch
        {
            highScore = 0;
            return;
        }
    }
}

[Serializable]
public struct TileSaveData
{
    public int i;
    public int value;
}

[Serializable]
public struct TilesCollection
{
    public TileSaveData[] tsd;
    public int score;
}

[Serializable]
public struct HighScoreData
{
    public int highScore;
}
