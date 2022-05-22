using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum Status {
    Menu,
    Play,
    Win,
    Lose,
}
enum SubStatus
{
    TileMoving,
    TileFalling,
}
enum TileType
{
    Cube,
    Cylinder,
    Triangle,
    Pentagone,
}

public class level : MonoBehaviour
{
    private static string MAXLEVELNUMBER = "MAXLEVELNUMBER";
    private static string MONEY = "MONEY";
    private Status status = Status.Menu;
    private SubStatus subStatus;
    private int actualLevel;
    public GameObject camera_go;
    public GameObject canvas;
    public GameObject title_go;
    public GameObject money_go;
    public GameObject score_go;
    public GameObject playButton;
    public GameObject floor;
    public GameObject cube;
    public GameObject cylinder;
    public GameObject triangle;
    public GameObject pentagone;
    public GameObject win_go;
    public GameObject lose_go;
    public GameObject placeTile_go;
    private List<GameObject> gosList = new List<GameObject>();
    public List<GameObject> levelsButton = new List<GameObject>();
    public List<GameObject> achivements = new List<GameObject>();
    public List<Material> materials = new List<Material>();
    private Material tileMaterial;
    public Material defaultMaterial;
    public float speed = 0.5f;
    public float cameraSpeed = 0.2f;
    private float horizontalSpeed;
    public float tileHeight = 0.1f;
    public float errorOnXPositionPlaced = 0.5f;
    public float tooFar = 1f;
    public float spawnDistance = 1.8f;
    private int totalGold;
    private int score = 0;
    private float defaultYCameraPosition = 1;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt(MAXLEVELNUMBER, PlayerPrefs.GetInt(MAXLEVELNUMBER, 1));
        SetTileMaterial(PlayerPrefs.GetInt("MATERIAL", -1));
        int gold = PlayerPrefs.GetInt(MONEY, 0);
        this.totalGold = gold;
        SaveGold(0);
        checkUnlockLevels();
        camera_go.transform.position = new Vector3(0, defaultYCameraPosition, -3);
        this.UpdateAchievements();
    }

    GameObject Triangle()
    {
        var obj = new GameObject("Triangle");
        Vector3[] vertices = {
             //devant
             new Vector3(-0.5f, -0.5f, 0), // 0
             new Vector3(0.5f, -0.5f, 0), // 1
             new Vector3(0f, 0.5f, 0), // 2
             //derrière
             new Vector3(-0.5f, -0.5f, 0.1f), // 3
             new Vector3(0.5f, -0.5f, 0.1f), // 4
             new Vector3(0f, 0.5f, 0.1f), // 5
             //droite 1
             new Vector3(0.5f, -0.5f, 0.1f), // 4
             new Vector3(0.5f, -0.5f, 0), // 1
             new Vector3(0f, 0.5f, 0), // 2
             //droite 2
             new Vector3(0.5f, -0.5f, 0.1f), // 4
             new Vector3(0f, 0.5f, 0), // 2
             new Vector3(0f, 0.5f, 0.1f), // 5
             //gauche 1
             new Vector3(-0.5f, -0.5f, 0), // 0
             new Vector3(-0.5f, -0.5f, 0.1f), // 3
             new Vector3(0f, 0.5f, 0.1f), // 5
             //gauche 2
             new Vector3(-0.5f, -0.5f, 0), // 0
             new Vector3(0f, 0.5f, 0.1f), // 5
             new Vector3(0f, 0.5f, 0), // 2
             //bas 1
             new Vector3(0.5f, -0.5f, 0.1f), // 4
             new Vector3(-0.5f, -0.5f, 0.1f), // 3
             new Vector3(-0.5f, -0.5f, 0), // 0
             //bas 2
             new Vector3(0.5f, -0.5f, 0.1f), // 4
             new Vector3(-0.5f, -0.5f, 0), // 0
             new Vector3(0.5f, -0.5f, 0), // 1
         };

        Vector2[] uv = {
            //devant
             new Vector2(0, 0),
             new Vector2(1, 0),
             new Vector2(0.5f, 1),
             //derrière
             new Vector2(0, 0),
             new Vector2(1, 0),
             new Vector2(0.5f, 1),
             //droite
             new Vector2(0, 0.1f),
             new Vector2(0, 0),
             new Vector2(1, 0),
             new Vector2(0, 0.1f),
             new Vector2(1, 0),
             new Vector2(1, 0.1f),
             //gauche
             new Vector2(0, 0.1f),
             new Vector2(0, 0),
             new Vector2(1, 0),
             new Vector2(0, 0.1f),
             new Vector2(1, 0),
             new Vector2(1, 0.1f),
             //bas
             new Vector2(0, 0),
             new Vector2(1, 0),
             new Vector2(1, 0.1f),
             new Vector2(0, 0),
             new Vector2(1, 0.1f), 
             new Vector2(0, 0.1f),
         };

        int[] triangles = {
            1, 0, 2,
            3, 4, 5,
            6, 7, 8,
            9, 10, 11,
            12, 13, 14,
            15, 16, 17,
            18, 19, 20,
            21, 22, 23,
        };

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        var filter = obj.AddComponent<MeshFilter>();
        var renderer = obj.AddComponent<MeshRenderer>();
        var collider = obj.AddComponent<MeshCollider>();

        filter.sharedMesh = mesh;
        collider.sharedMesh = mesh;
        renderer.sharedMaterial = defaultMaterial;
        return obj;
    }

    public void StartLevel(int level)
    {
        this.actualLevel = (level == -1 ? PlayerPrefs.GetInt(MAXLEVELNUMBER) : level);
        Reset();
        this.status = Status.Play;
        this.subStatus = SubStatus.TileMoving;
        this.playButton.SetActive(true);
        YsoCorp.GameUtils.YCManager.instance.OnGameStarted(this.actualLevel);
        AddTile();
    }

    public void SetTileMaterial(int materialIndex)
    {
        if (materialIndex >= 0)
        {
            if (materials.Count > materialIndex)
            {
                this.tileMaterial = materials[materialIndex];
                this.floor.GetComponent<MeshRenderer>().material = this.tileMaterial;
                for (int i = 0; i < gosList.Count; i++) {
                    MeshRenderer mr = gosList[i].GetComponent<MeshRenderer>();
                    if (mr)
                    {
                        gosList[i].GetComponent<MeshRenderer>().material = this.tileMaterial;
                    } else if (gosList[i].transform.GetChild(0) && gosList[i].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>())
                    {
                        gosList[i].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = this.tileMaterial;
                    }
                }
                PlayerPrefs.SetInt("MATERIAL", PlayerPrefs.GetInt("MATERIAL", materialIndex));
            }
        }
    }

    void Reset()
    {
        foreach (var gameObject in gosList)
        {
            Destroy(gameObject);
        }
        gosList.Clear();
        status = Status.Menu;
        this.horizontalSpeed = speed * (1f + ((float)this.actualLevel / 5f));
        this.score = 0;
        SaveScore(0);
        camera_go.transform.position = new Vector3(0, defaultYCameraPosition, -3);
    }

    void AddTile()
    {
        TileType tileType = (this.actualLevel <= 3 ? TileType.Cube : (this.actualLevel <= 6 ? TileType.Triangle : TileType.Cylinder));
        if (this.actualLevel == 10)
        {
            if (this.gosList.Count % 150 < 50)
            {
                tileType = TileType.Cube;
            } else if (this.gosList.Count % 150 < 100)
            {
                tileType = TileType.Triangle;
            }
            else
            {
                tileType = TileType.Cylinder;
            }
        }
        GameObject gameobject;
        float x = (this.gosList.Count % 2 == 0 ? 1 : -1) * this.spawnDistance;
        float y = (gosList.Count + 1) * tileHeight;
        switch (tileType)
        {
            case TileType.Cube:
            default:
                gameobject = Instantiate(cube, new Vector3(x, y, 0), Quaternion.identity);
                gameobject.transform.Rotate(new Vector3(0, 45, 0), Space.Self);
                break;
            case TileType.Cylinder:
                gameobject = Instantiate(cylinder, new Vector3(x, y, 0), Quaternion.identity);
                gameobject.GetComponent<CapsuleCollider>().enabled = false;
                gameobject.AddComponent(typeof(BoxCollider));
                break;
            case TileType.Triangle:
                GameObject child = Triangle();
                child.transform.Rotate(90.0f, 180.0f, 0.0f, Space.Self);
                child.transform.position = new Vector3(x, y + 0.05f, -0.2f);
                gameobject = new GameObject("Triangle");
                gameobject.transform.position = new Vector3(x, y, 0);
                child.transform.parent = gameobject.transform;
                child.GetComponent<MeshCollider>().convex = true;
                break;
            case TileType.Pentagone:
                gameobject = Instantiate(pentagone, new Vector3(x, y, 0), Quaternion.identity);
                break;
        }
        if (this.tileMaterial)
        {
            switch (tileType)
            {
                case TileType.Cube:
                case TileType.Cylinder:
                default:
                    gameobject.GetComponent<MeshRenderer>().material = this.tileMaterial;
                    break;
                case TileType.Pentagone:
                case TileType.Triangle:
                    gameobject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = this.tileMaterial;
                    break;
            }
        }
        Rigidbody rb = gameobject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        rb.isKinematic = true;
        gosList.Add(gameobject);
        // set speed for endless mod
        if (this.actualLevel >= 10)
        {
            this.horizontalSpeed = this.speed * (1f + ((float)this.gosList.Count * 0.0133f));
            // max speed
            if (this.horizontalSpeed > this.speed * 4)
            {
                this.horizontalSpeed = this.speed * 4;
            }
        }
    }

    bool LevelIsFinished()
    {
        if (this.actualLevel < 10)
        {
            if (this.gosList.Count >= this.actualLevel * 5 + 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        } else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (status)
        {
            case Status.Menu:
                break;
            case Status.Play:
                UpdatePlay();
                break;
            case Status.Win:
                break;
            case Status.Lose:
                break;
        }
    }

    void UpdatePlay()
    {
        // if tile is moving
        if (this.subStatus == SubStatus.TileMoving)
        {
            if (this.gosList.Count > 0)
            {
                this.gosList[this.gosList.Count - 1].transform.position += new Vector3((this.gosList.Count % 2 == 0 ? 1 : -1) * this.horizontalSpeed * Time.deltaTime, 0, 0);
                // if pair
                if (this.gosList.Count % 2 == 0)
                {
                    // if going too far
                    if (this.gosList[this.gosList.Count - 1].transform.position.x > this.tooFar)
                    {
                        OnLose();
                    }
                }
                else
                {
                    // if going too far
                    if (this.gosList[this.gosList.Count - 1].transform.position.x < this.tooFar * -1)
                    {
                        OnLose();
                    }
                }
            }
        }
        // if tile is falling
        if (this.subStatus == SubStatus.TileFalling)
        {
            // tile falling
            float goToY = this.gosList[this.gosList.Count - 1].transform.position.y + -1 * this.speed * Time.deltaTime;
            float hitBoxY = (this.gosList.Count > 1) ? this.gosList[this.gosList.Count - 2].transform.position.y + this.tileHeight : this.tileHeight;
            this.gosList[this.gosList.Count - 1].transform.position += new Vector3(0, -1 * this.speed * Time.deltaTime, 0);
            // check if it dit not hit the floor
            if (goToY > hitBoxY)
            {
                // set y position
                this.gosList[this.gosList.Count - 1].transform.position = new Vector3(this.gosList[this.gosList.Count - 1].transform.position.x, goToY, this.gosList[this.gosList.Count - 1].transform.position.z);
            } else // hit the floor
            {
                // set y position
                this.gosList[this.gosList.Count - 1].transform.position = new Vector3(this.gosList[this.gosList.Count - 1].transform.position.x, hitBoxY, this.gosList[this.gosList.Count - 1].transform.position.z);
                // check if well place
                // not too far right
                if (this.gosList[this.gosList.Count - 1].transform.position.x < errorOnXPositionPlaced)
                {
                    // not too far left
                    if (this.gosList[this.gosList.Count - 1].transform.position.x > errorOnXPositionPlaced * -1)
                    {
                        // well placed
                        // add 1 to score
                        SaveScore(1);
                        // calc gold
                        float distanceFromCenter = (this.gosList[this.gosList.Count - 1].transform.position.x < 0 ? this.gosList[this.gosList.Count - 1].transform.position.x * -1 : this.gosList[this.gosList.Count - 1].transform.position.x);
                        if (distanceFromCenter > this.errorOnXPositionPlaced / 100 * 50)
                        {
                            SaveGold(1);
                        } else if (distanceFromCenter > this.errorOnXPositionPlaced / 100 * 25)
                        {
                            SaveGold(2);
                        } else if(distanceFromCenter > this.errorOnXPositionPlaced / 100 * 10)
                        {
                            SaveGold(3);
                        } else
                        {
                            SaveGold(5);
                        }
                        // is game finished
                        if (LevelIsFinished())
                        {
                            OnWin();
                        } else
                        {
                            this.subStatus = SubStatus.TileMoving;
                            this.placeTile_go.SetActive(false);
                            this.placeTile_go.SetActive(true);
                            AddTile();
                        }
                    }
                    else
                    {
                        // lose
                        OnLose();
                    }
                }
                else
                {
                    // lose
                    OnLose();
                }
            }
        }
        // update camera position
        if (gosList.Count > 9)
        {
            float posY = camera_go.transform.position.y;
            float maxYPosition = (gosList.Count - 9) * tileHeight;
            if (posY - this.defaultYCameraPosition < maxYPosition)
            {
                float newYPos = posY - this.defaultYCameraPosition + (this.cameraSpeed * Time.deltaTime);
                newYPos = (newYPos > maxYPosition ? maxYPosition : newYPos);
                camera_go.transform.position = new Vector3(camera_go.transform.position.x, newYPos + this.defaultYCameraPosition, camera_go.transform.position.z);
            }
        }
    }

    void OnWin()
    {
        this.win_go.SetActive(false);
        this.win_go.SetActive(true);
        this.playButton.SetActive(false);
        this.status = Status.Win;
        Text text = title_go.GetComponent<Text>();
        text.text = "Level finished !";
        text.color = new Color(69f / 255f, 1f, 34f / 255f);
        int maxLevel = PlayerPrefs.GetInt(MAXLEVELNUMBER);
        PlayerPrefs.SetInt(MAXLEVELNUMBER, (maxLevel <= actualLevel ? actualLevel + 1 : maxLevel));
        YsoCorp.GameUtils.YCManager.instance.OnGameFinished(true);
        canvas.SetActive(true);
        if (this.actualLevel == 1)
        {
            PlayerPrefs.SetInt("ACHIEVEMENTS" + 1, 1);
        } else if (this.actualLevel == 3)
        {
            PlayerPrefs.SetInt("ACHIEVEMENTS" + 2, 1);
        }
        else if (this.actualLevel == 6)
        {
            PlayerPrefs.SetInt("ACHIEVEMENTS" + 3, 1);
        }
        else if (this.actualLevel == 9)
        {
            PlayerPrefs.SetInt("ACHIEVEMENTS" + 4, 1);
        }
        this.UpdateAchievements();
        checkUnlockLevels();
    }

    void OnLose()
    {
        this.playButton.SetActive(false);
        this.status = Status.Lose;
        this.subStatus = SubStatus.TileMoving;
        if (this.actualLevel < 10)
        {
            Text text = title_go.GetComponent<Text>();
            text.text = "Try again.";
            text.color = new Color(1f, 1f, 0f);
            YsoCorp.GameUtils.YCManager.instance.OnGameFinished(false);
            this.lose_go.SetActive(false);
            this.lose_go.SetActive(true);
        } else
        {
            this.win_go.SetActive(false);
            this.win_go.SetActive(true);
            if (this.score >= 1)
            {
                PlayerPrefs.SetInt("ACHIEVEMENTS" + 5, 1);
            }
            if (this.score >= 2)
            {
                PlayerPrefs.SetInt("ACHIEVEMENTS" + 6, 1);
            }
            if (this.score >= 3)
            {
                PlayerPrefs.SetInt("ACHIEVEMENTS" + 7, 1);
            }
            if (this.score >= 700)
            {
                PlayerPrefs.SetInt("ACHIEVEMENTS" + 8, 1);
            }
            if (this.score >= 1000)
            {
                PlayerPrefs.SetInt("ACHIEVEMENTS" + 9, 1);
            }
            Text text = title_go.GetComponent<Text>();
            text.text = "BuildiStack";
            text.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
            YsoCorp.GameUtils.YCManager.instance.OnGameFinished(true);
        }
        this.UpdateAchievements();
        canvas.SetActive(true);
        foreach (var gameObject in this.gosList)
        {
            // make all tiles falling
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
            }
        }
    }

    public void UpdateAchievements()
    {
        if (PlayerPrefs.GetInt("ACHIEVEMENTS" + 1, 0) == 1)
        {
            this.achivements[0].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        if (PlayerPrefs.GetInt("ACHIEVEMENTS" + 2, 0) == 1)
        {
            this.achivements[1].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        if (PlayerPrefs.GetInt("ACHIEVEMENTS" + 3, 0) == 1)
        {
            this.achivements[2].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        if (PlayerPrefs.GetInt("ACHIEVEMENTS" + 4, 0) == 1)
        {
            this.achivements[3].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        if (PlayerPrefs.GetInt("ACHIEVEMENTS" + 5, 0) == 1)
        {
            this.achivements[4].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        if (PlayerPrefs.GetInt("ACHIEVEMENTS" + 6, 0) == 1)
        {
            this.achivements[5].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        if (PlayerPrefs.GetInt("ACHIEVEMENTS" + 7, 0) == 1)
        {
            this.achivements[6].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        if (PlayerPrefs.GetInt("ACHIEVEMENTS" + 8, 0) == 1)
        {
            this.achivements[7].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        if (PlayerPrefs.GetInt("ACHIEVEMENTS" + 9, 0) == 1)
        {
            this.achivements[8].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
    }

    // ----------------- PLAY BUTTON -----------------

    public void PlayButton()
    {
        if (this.subStatus == SubStatus.TileMoving)
        {
            this.subStatus = SubStatus.TileFalling;
        }
    }

    // ----------------- LEVELS MENU -----------------

    void checkUnlockLevels()
    {
        int level = 1;
        int numbersOfUnlockLevels = PlayerPrefs.GetInt(MAXLEVELNUMBER);
        foreach (var gameObject in levelsButton)
        {
            if (level <= numbersOfUnlockLevels)
            {
                gameObject.GetComponent<Button>().interactable = true;
                GameObject child = gameObject.transform.GetChild(1).gameObject;
                child.SetActive(false);
            }
            level += 1;
        }
    }

    void SaveGold(int goldToAdd)
    {
        this.totalGold += goldToAdd;
        PlayerPrefs.SetInt(MONEY, this.totalGold);
        Text text = this.money_go.GetComponent<Text>();
        text.text = this.totalGold.ToString();
    }

    void SaveScore(int scoreToAdd)
    {
        this.score += scoreToAdd;
        Text text = this.score_go.GetComponent<Text>();
        text.text = this.score.ToString();
    }
}
