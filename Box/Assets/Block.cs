using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{
    //0 空格子 1 箱子 2 易碎 3 人 4 墙
    [HideInInspector]
    public int x, y, z;
    public int type = 0;
    private int _type;
    [HideInInspector]
    public List<Block> friend;

    public bool NeedToBeUpdate()
    {
        return type != _type;
    }
    public void Init(int _x = 0, int _y = 0, int _z = 0, int _t = 0)
    {
        x = _x;
        y = _y;
        z = _z;
        ChangeToType(_t);
    }
    public void ChangeToType(int target)
    {
        //print(x.ToString());
        //print(y.ToString());
        //print(target);
        //print("Change!");
        friend.Clear();
        _type = target;
        type = target;
        if (type >= 5 && type <= 10)
        {
            for (int k = 0; k < 2; k++)
            {
                for (int i = 0; i < GameController.Instance.Vertical; i++)
                    for (int j = 0; j < GameController.Instance.Horizontal; j++)
                    {
                        if (GameController.Instance.block_map[i, j, k].type == type)
                        {
                            friend.Add(GameController.Instance.block_map[i, j, k]);
                            GameController.Instance.block_map[i, j, k].friend.Add(this);
                        }


                    }
            }
        }
        //print(z);
        if (z == 1)
            GetComponent<SpriteRenderer>().sprite = GameController.Instance.DownSprite[_type];
        if (z == 0)
            GetComponent<SpriteRenderer>().sprite = GameController.Instance.UpSprite[_type];
        //TODO：更新Sprite

        //if ()
        /*
        switch (target)
        {
            case (0):

        }*/
    }
    private int[] dx = { -1, 1, 0, 0 };
    private int[] dy = { 0, 0, -1, 1 };
    // -1 不行 0 正常 1 会下落一格
    /*
    public bool TestMove(int Dir,bool first)
    {
        int new_x = x + dx[Dir];
        int new_y = y + dy[Dir];
        if (!(new_x >= 0 && new_x < GameController.Instance.Vertical && new_y >= 0 && new_y < GameController.Instance.Vertical)) return false;
        if (type == 0 || type == 5)
            return true;
        if (type == 1 || type == 2 || type == 3)
        {
            //if (type==3 && )
            return GameController.Instance.block_map[new_x, new_y, z].TestMove(Dir,true);
        }
        if (type == 4)
            return false;
        if ((type >= 5 && type <= 10) && first)
        {
            if (first)
            {
                bool flag = true;
                for (int i = 0; i < GameController.Instance.Vertical; i++)
                    for (int j = 0; j < GameController.Instance.Horizontal; j++)
                        for (int k = 0; k < 2; k++)
                        {
                            if (!GameController.Instance.block_map[i, j, k].type == type)
                                TestMove()
    
                    }
            }
            else
            {

            }
        }
      

        return false;

    }
    */
    public int Move(int Dir, bool force = false)
    {
        int new_x = x + dx[Dir];
        int new_y = y + dy[Dir];
        if (!(new_x >= 0 && new_x < GameController.Instance.Vertical && new_y >= 0 && new_y < GameController.Instance.Vertical)) return -1;
        if (type == 0 || type == 5)
            return 1;
        if (type == 1 || type == 2 || type == 3 || (type >= 5 && type <= 10 && force))
        {
            GameController.Instance.block_map[new_x, new_y, z].Move(Dir);
            GameController.Instance.block_map[new_x, new_y, z].ChangeToType(type);
            GameController.Instance.block_map[x, y, z].ChangeToType(0);
        }
        if (type >= 5 && type <= 10 && (!force))
        {
            friend.Sort((a, b) => (a.x * dx[Dir]) + (a.y * dy[Dir]).CompareTo((b.x * dx[Dir]) + (b.y * dy[Dir])));
            foreach (var m_block in friend)
            {

                int tx = m_block.x + dx[Dir];
                int ty = m_block.y + dy[Dir];
                GameController.Instance.block_map[tx, ty, z].Move(Dir);
                //GameController.Instance.block_map[tx, ty, z].ChangeToType(type);
                GameController.Instance.block_map[m_block.x, m_block.y, z].ChangeToType(0);
            }
        }
        return 0;
    }
}
