using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour 
{
    const int rows = 4;
    [SerializeField]
    GameObject TilePrototype;
    
    [SerializeField]
    RectTransform gameTransform;

    [SerializeField]
    Text scoreTxt;
    [SerializeField]
    Text hiScoreTxt;

    [SerializeField]
    Button backButton;

    Tile[] tiles = new Tile[rows * rows];
    int[] history = new int[rows * rows];
    public int Score = 0;

    int preScore = 0;

    Vector3 lastMousePos = Vector2.zero;

    bool backAvailable = false;
	// Use this for initialization
	void Start () {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        if (Input.touchSupported)
        {
            Cursor.visible = false;
        }
        lastMousePos = Input.mousePosition;

        iniHistory();
        Tile[] tempTiles;
        if(!SaveLoad.Load(out tempTiles, out Score))
        {
            createTiles();
            fillTiles();
        }
        else
        {
            preScore = Score;
            for (int i = 0; i < tiles.Length; i++)
			{
                instantiateTile(i, tempTiles[i].Position, tempTiles[i].Value);
			}
        }
	}
	
	void Update() 
    {
        input();
        scoreTxt.text = "Score: " + Score.ToString("D8");
        if (Score > SaveLoad.highScore)
        {
            SaveLoad.highScore = Score;
            hiScoreTxt.text = "High Score: " + Score.ToString("D8");
        }
        else
        {
            hiScoreTxt.text = "High Score: " + SaveLoad.highScore.ToString("D8");
        }
        if (Input.mousePosition != lastMousePos)
        {
            CancelInvoke("hideCursor");
            lastMousePos = Input.mousePosition;
            Cursor.visible = true;
            Invoke("hideCursor", 2);
        }
        backButton.interactable = backAvailable;
	}

    public void Reset()
    {
        for (int i = 0; i < gameTransform.childCount; i++)
        {
            Destroy(gameTransform.GetChild(i).gameObject);
        }
        Score = 0;
        backAvailable = false;
        iniHistory();
        createTiles();
        fillTiles();
        SaveLoad.Save(null, 0);
    }

    public void Quit()
    {
        SaveLoad.Save(tiles, Score);
        Application.Quit();
    }


    public void Back()
    {
        if(backAvailable && history != null)
        {
            for (int i = 0; i < rows * rows; i++)
            {
                tiles[i].Value = history[i];
            }
            Score = preScore;
            backAvailable = false;
            history = null;
        }
        else
        {
            backAvailable = false;
        }
    }

    private void createTiles()
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            instantiateTile(i, i, 0);
        }
    }

    private void fillTiles()
    {
        int i1 = Random.Range(0, tiles.Length);
        int i2 = Random.Range(0, tiles.Length);
        while(i1 == i2)
        {
            i2 = Random.Range(0, tiles.Length);
        }
        tiles[i1].Value = randomPOT();
        tiles[i2].Value = randomPOT();
    }
    private void oneNew()
    {
        int i = Random.Range(0, tiles.Length);
        while (tiles[i].Value != 0)
        {
            i = Random.Range(0, tiles.Length);
        }
        tiles[i].Value = randomPOT();
    }

    private bool checkFull()
    {
        foreach(var t in tiles)
        {
            if(t.Value == 0)
            {
                return false;
            }
        }
        return true;
    }

    private int randomPOT()
    {
        if(Random.value < 0.6f)
        {
            return 2;
        }
        else
        {
            return 4;
        }
    }


    /*
     * 
     * Move indices:
     * 0: Left
     * 1: Up
     * 2: Right
     * 3: Down
     * 
     */

    /*
     * 
     * 0  1  2  3 
     * 4  5  6  7
     * 8  9  10 11
     * 12 13 14 15
     * 
     */
    private void makeMove(int moveIndex)
    {
        int[] temp = new int[rows * rows];
        saveHistory(out temp);
        bool moved = false;
        moved = addTiles(moveIndex);
        moved = moveTiles(moveIndex) || moved;
        if (!checkFull())
        {
            if (moved)
            {
                oneNew();
                SaveLoad.Save(tiles, Score);
            }
        }
        if (moved)
        {
            backAvailable = true;
        }
        else
        {
            revertHistory(temp);
        }
    }

    private void input()
    {
        int moveIndex = -1;

        if(Input.GetKeyDown(KeyCode.LeftArrow) || TouchHelper.getTouchInput() == TouchHelper.Direction.left)
        {
            moveIndex = 0;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow) || TouchHelper.getTouchInput() == TouchHelper.Direction.right)
        {
            moveIndex = 2;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow) || TouchHelper.getTouchInput() == TouchHelper.Direction.up)
        {
            moveIndex = 1;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow) || TouchHelper.getTouchInput() == TouchHelper.Direction.down)
        {
            moveIndex = 3;
        }

        if (moveIndex != -1)
        {
            makeMove(moveIndex);
        }
    }

    private bool addTiles(int moveIndex)
    {
        bool r = false;

        switch (moveIndex)
        {
            #region left
            case 0:
                for (int j = 0; j < rows; j++)
                {
                    for (int i = 0; i < rows - 1; i++)
                    {
                        for (int w = i + 1; w < rows; w++ )
                        {
                            if(tiles[w + rows * j].Value != 0)
                            {
                                if(tiles[w + rows * j].Value ==  tiles[i + rows * j].Value)
                                {
                                    tiles[i + rows * j].Value *= 2;
                                    tiles[w + rows * j].Value = 0;
                                    Score += tiles[i + rows * j].Value;
                                    r = true;
                                    i = w;
                                }
                                break;
                            }
                        }
                    }
                }
                break;
            #endregion
            #region up
            case 1:
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < rows - 1; j++)
                    {
                        for (int w = j + 1; w < rows; w++)
                        {
                            if (tiles[i + rows * w].Value != 0)
                            {
                                if (tiles[i + rows * w].Value == tiles[i + rows * j].Value)
                                {
                                    tiles[i + rows * j].Value *= 2;
                                    tiles[i + rows * w].Value = 0;
                                    Score += tiles[i + rows * j].Value;
                                    r = true;
                                    j = w;
                                }
                                break;
                            }
                        }
                    }
                }
                break;
            #endregion
            #region right
            case 2:
                for (int j = 0; j < rows; j++)
                {
                    for (int i = rows - 1; i > 0; i--)
                    {
                        for (int w = i - 1; w >= 0; w--)
                        {
                            if(tiles[w + rows * j].Value != 0)
                            {
                                if(tiles[w + rows * j].Value == tiles[i + rows * j].Value)
                                {
                                    tiles[i + rows * j].Value *= 2;
                                    tiles[w + rows * j].Value = 0;
                                    Score += tiles[i + rows * j].Value;
                                    r = true;
                                    i = w;
                                }
                                break;
                            }

                        }
                    }
                }
                break;
            #endregion
            #region down
            case 3:
                for (int i = 0; i < rows; i++)
                {
                    for (int j = rows - 1; j > 0; j--)
                    {
                        for (int w = j - 1; w >= 0; w--)
                        {
                            if (tiles[i + rows * w].Value != 0)
                            { 
                                if(tiles[i + rows * w].Value == tiles[i + rows * j].Value)
                                {
                                    tiles[i + rows * j].Value *= 2;
                                    tiles[i + rows * w].Value = 0;
                                    Score += tiles[i + rows * j].Value;
                                    r = true;
                                    j = w;
                                }
                                break;
                            }
                        }
                    }
                }
                break;
            #endregion
        }

        return r;
    }

    private void hideCursor()
    {
        Cursor.visible = false;
    }

    private void iniHistory()
    {
        history = null;
    }

    private void saveHistory(out int[] temp)
    {
        preScore = Score;
        if (history == null)
        {
            temp = null;
            history = new int[rows * rows];
            for (int i = 0; i < rows * rows; i++)
            {
                history[i] = tiles[i].Value;
            }
            
            return;
        }
        temp = new int[rows * rows];
        for (int i = 0; i < rows * rows; i++)
        {
            temp[i] = history[i];
            history[i] = tiles[i].Value;
        }
    }

    private void revertHistory(int[] temp)
    {
        for (int i = 0; i < rows * rows; i++)
        {
            history[i] = temp[i];
        }
    }

    private bool moveTiles(int moveIndex)
    {
        bool r = false;

        switch (moveIndex)
        {
            case 0:
                for (int j = 0; j < rows; j++)
                {
                    for (int i = 0; i < rows - 1; i++)
                    {
                        if(tiles[i + j*rows].Value == 0)
                        {
                            for(int w = i + 1; w < rows; w++)
                            {
                                if(tiles[w + j * rows].Value != 0)
                                {
                                    tiles[i + j * rows].Value = tiles[w + j * rows].Value;
                                    tiles[w + j * rows].Value = 0;
                                    r = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            case 1:
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < rows - 1; j++)
                    {
                        if(tiles[i + rows * j].Value == 0)
                        {
                            for(int w = j + 1; w < rows; w++)
                            {
                                if(tiles[i + rows * w].Value != 0)
                                {
                                    tiles[i + rows * j].Value = tiles[i + rows * w].Value;
                                    tiles[i + rows * w].Value = 0;
                                    r = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            case 2:
                for (int j = 0; j < rows; j++)
                {
                    for (int i = rows - 1; i > 0; i--)
                    {
                        if(tiles[i + rows * j].Value == 0)
                        {
                            for(int w = i - 1; w >= 0; w--)
                            {
                                if(tiles[w + rows * j].Value != 0)
                                {
                                    tiles[i + rows * j].Value = tiles[w + rows * j].Value;
                                    tiles[w + rows * j].Value = 0;
                                    r = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            case 3:
                for (int i = 0; i < rows; i++)
                {
                    for (int j = rows - 1; j > 0; j--)
                    {
                        if(tiles[i + j * rows].Value == 0)
                        {
                            for(int w = j - 1; w >= 0; w--)
                            {
                                if(tiles[i + w * rows].Value != 0)
                                {
                                    tiles[i + j * rows].Value = tiles[i + w * rows].Value;
                                    tiles[i + w * rows].Value = 0;
                                    r = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
        }

        return r;
    }

    private void instantiateTile(int i, int position, int _value)
    {
        GameObject go = (GameObject)Instantiate(TilePrototype);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.parent = gameTransform;
        rt.localScale = new Vector3(1, 1, 1);
        tiles[i] = go.GetComponent<Tile>();
        tiles[i].Value = _value;
        tiles[i].Position = position;
    }
}
