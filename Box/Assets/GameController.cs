using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object _singletonLock = new object();

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_singletonLock)
                {
                    T[] singletonInstances = FindObjectsOfType(typeof(T)) as T[];
                    if (singletonInstances.Length == 0) return null;

                    if (singletonInstances.Length > 1)
                    {
                        if (Application.isEditor)
                            Debug.LogWarning(
                                "MonoSingleton<T>.Instance: Only 1 singleton instance can exist in the scene. Null will be returned.");
                        return null;
                    }
                    _instance = singletonInstances[0];
                }
            }
            return _instance;
        }
    }
}

public class GameController : SingletonBase<GameController>
{
    public Block[,,] block_map;
    private int human_x, human_y, row, line;
    [HideInInspector]
    public int win_x, win_y;
    [HideInInspector]
    public int Horizontal = 7, Vertical = 7;
    Block Human;
    public Sprite[] UpSprite, DownSprite;
    public bool UseWinSprite;
    public Sprite WinSprite;
    private Text WinText;
    bool isWin = false;
    private List<int[,,]> OldMap = new List<int[,,]>();
    private void Start()
    {
        WinText = GameObject.FindGameObjectWithTag("WinText").GetComponent<Text>();
        WinText.color = Color.clear;
        Horizontal = GameObject.FindObjectOfType<EditorSpawn>().Horizontal;
        Vertical = GameObject.FindObjectOfType<EditorSpawn>().Vertical;
        win_x = GameObject.FindObjectOfType<EditorSpawn>().WinX;
        win_y = GameObject.FindObjectOfType<EditorSpawn>().WinY;
        block_map = new Block[Vertical, Horizontal, 2];
        //block_map = new Block[row, line, 2];
        UpdateHuman();
    }
    void UpdateHuman()
    {
        foreach (var m_block in FindObjectsOfType<Block>())
        {
            block_map[m_block.x, m_block.y, m_block.z] = m_block;
            if (m_block.type == 3)
            {
                human_x = m_block.x;
                human_y = m_block.y;
                Human = m_block;
            }
        }
    }
    void Undo()
    {
        if (OldMap.Count == 0) return;
        print("UNDO");
        //block_map = OldMap[OldMap.Count - 1];

        for (int i = 0; i < Vertical; i++)
            for (int j = 0; j < Horizontal; j++)
                for (int k = 0; k < 2; k++)
                {
                    //if (block_map[i, j, k].NeedToBeUpdate()) print("Debug "+i.ToString()+" "+j.ToString());
                    block_map[i, j, k].ChangeToType(OldMap[OldMap.Count - 1][i, j, k]);
                }
        OldMap.RemoveAt(OldMap.Count - 1);
        UpdateHuman();
    }
    private void Update()
    {
        if (UseWinSprite)
        {

            if (block_map[win_x, win_y, 0].type == 0)
            {

                //print("Debug " + win_x.ToString() + " " + win_y.ToString());
                //print(block_map[win_x, win_y, 0].name);
                block_map[win_x, win_y, 0].gameObject.GetComponent<SpriteRenderer>().sprite = WinSprite;
                block_map[win_x, win_y, 0].gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        if (isWin) return;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            Move(0);
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            Move(1);
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Move(2);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Move(3);
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (Input.GetKeyDown(KeyCode.Z))
            Undo();
    }
    bool BFSMove(int Dir)
    {
        //Block[,,] tmpMove = block_map;
        bool flag = true;
        int num = 1;
        Queue<int> qx = new Queue<int>(), qy = new Queue<int>();
        List<Block> BlockToMove = new List<Block>();
        HashSet<Tuple<int, int>> hashmap = new HashSet<Tuple<int, int>>();
        hashmap.Add(new Tuple<int, int>(human_x, human_y));
        qx.Enqueue(human_x);
        qy.Enqueue(human_y);
        while (qx.Count != 0)
        {
            int nowx = qx.Peek();
            int nowy = qy.Peek();
            //print("Now Pro " + nowx.ToString() + " " + nowy.ToString());
            qx.Dequeue();
            qy.Dequeue();
            if (block_map[nowx, nowy, Human.z].type != 0) BlockToMove.Add(block_map[nowx, nowy, Human.z]);
            int new_x = nowx + dx[Dir];
            int new_y = nowy + dy[Dir];
            if (new_x < 0 || new_x >= Vertical || new_y < 0 || new_y >= Horizontal)
            {
                flag = false;
                break;
            }
            int t = block_map[new_x, new_y, Human.z].type;

            if (hashmap.Contains(new Tuple<int, int>(new_x, new_y))) continue;
            hashmap.Add(new Tuple<int, int>(new_x, new_y));
            if (t == 0)
            {
                continue;
            }
            if (t == 4)
            {
                flag = false; break;
            }
            else
            {
                num++;
                //print(t);
                qx.Enqueue(new_x);
                qy.Enqueue(new_y);
                if (t >= 5 && t <= 10)
                {
                    for (int i = 0; i < Vertical; i++)
                        for (int j = 0; j < Horizontal; j++)
                            for (int k = 0; k < 2; k++)
                            {
                                if (block_map[i, j, k].type == t)
                                    if (!hashmap.Contains(new Tuple<int, int>(i, j)))
                                    {
                                        qx.Enqueue(i);
                                        qy.Enqueue(j);
                                        hashmap.Add(new Tuple<int, int>(i, j));
                                    }
                            }
                }
            }
        }
        if (num > 2) flag = false;
        if (Human.z == 0)
            if ((block_map[human_x + dx[Dir], human_y + dy[Dir], 1].type == 0))
                flag = false;

        if (flag)
        {
            OldMap.Add(new int[Vertical, Horizontal, 2]);
            for (int i = 0; i < Vertical; i++)
                for (int j = 0; j < Horizontal; j++)
                    for (int k = 0; k < 2; k++)
                    {
                        OldMap[OldMap.Count - 1][i, j, k] = block_map[i, j, k].type;
                        //if (block_map[i, j, k].NeedToBeUpdate()) print("Debug "+i.ToString()+" "+j.ToString());
                        //block_map[i, j, k].ChangeToType(OldMap[OldMap.Count - 1][i, j, k]);
                    }
            BlockToMove.Sort((a, b) => ((b.x * dx[Dir]) + (b.y * dy[Dir])).CompareTo((a.x * dx[Dir]) + (a.y * dy[Dir])));
            foreach (var m_block in BlockToMove)
            {
                int new_x = m_block.x + dx[Dir];
                int new_y = m_block.y + dy[Dir];
                //print("queue " + m_block.x + " " + m_block.y);
                if (!(new_x >= 0 && new_x < Vertical && new_y >= 0 && new_y < Horizontal)) continue;
                block_map[new_x, new_y, Human.z].ChangeToType(m_block.type);
                m_block.ChangeToType(0);

            }
        }
        return flag;

    }

    //0 up 1 down 2 left 3 right
    private int[] dx = { -1, 1, 0, 0 };
    private int[] dy = { 0, 0, -1, 1 };
    private void Move(int Dir)
    {
        int oldx = human_x;
        int oldy = human_y;
        int oldz = Human.z;
        if (BFSMove(Dir))
        {
            UpdateHuman();
            if (human_x == win_x && human_y == win_y && Human.z == 0)
            {
                StartCoroutine(Win());
                print("WIN!");
            }

            int t = block_map[oldx, oldy, 1].type;
            if (t == 2 && oldz == 0)
                block_map[oldx, oldy, 1].ChangeToType(0);

            if (t >= 5 && t <= 7 && oldz == 0 && block_map[human_x, human_y, 1].type != t)
                foreach (var m_block in GetFriend(block_map[oldx, oldy, 1]))
                    m_block.ChangeToType(0);
            //Move Audio
        }
        else
        {
            //Not Move Audio;
        }
        CheckDown();

        /*
        int new_x = human_x + dx[Dir];
        int new_y = human_y + dy[Dir];
        if (new_x >= 0 && new_x < Vertical && new_y >= 0 && new_y < Horizontal)
            if ()*/
    }
    List<Block> GetFriend(Block m_block)
    {
        List<Block> l = new List<Block>();
        for (int i = 0; i < Vertical; i++)
            for (int j = 0; j < Horizontal; j++)
                for (int k = 0; k < 2; k++)
                {
                    if (block_map[i, j, k].type == m_block.type)
                        l.Add(block_map[i, j, k]);
                }
        return l;
    }
    IEnumerator Win()
    {
        isWin = true;
        WinText.DOFade(1, 1f);
        yield return new WaitForSeconds(1);
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void CheckDown()
    {
        for (int i = 0; i < Vertical; i++)
            for (int j = 0; j < Horizontal; j++)
            {
                int t = block_map[i, j, 0].type;
                if (block_map[i, j, 1].type != 0) continue;
                if (t == 1 || t == 2 || t == 3)
                {
                    block_map[i, j, 1].ChangeToType(t);
                    block_map[i, j, 0].ChangeToType(0);
                }
                else if (t >= 5 && t <= 10)
                {
                    bool flag = true;
                    var m_block = block_map[i, j, 0];
                    var friend = GetFriend(m_block);
                    foreach (var temp in friend)
                    {
                        if (block_map[temp.x, temp.y, 1].type != 0)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        int name = m_block.type;
                        foreach (var temp in friend)
                        {
                            block_map[temp.x, temp.y, 1].ChangeToType(name);
                            block_map[temp.x, temp.y, 0].ChangeToType(0);
                        }
                    }

                }
            }
    }
}
