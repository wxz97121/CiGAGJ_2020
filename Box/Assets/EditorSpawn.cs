using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.WSA;

[ExecuteInEditMode]
public class EditorSpawn : MonoBehaviour
{
    public int Horizontal = 7, Vertical = 7;
    private int row, line;
    private Block[,,] block_map;
    public GameObject BlockPrefab;
    public float BlockWidth = 50, BlockHeight = 50;
    public int WinX = 5, WinY = 5;
    public void Respawn()
    {

        row = Vertical;
        line = Horizontal;
        block_map = new Block[row, line, 2];
        foreach (var m_block in FindObjectsOfType<Block>())
        {
            block_map[m_block.x, m_block.y, m_block.z] = m_block;
            m_block.ChangeToType(m_block.type);
        }
        for (int k = 0; k < 2; k++)
        {
            Transform e;
            if (k == 0) e = transform.Find("up");
            else e = transform.Find("down");
            for (int i = 0; i < row; i++)
                for (int j = 0; j < line; j++)
                {
                    if (!block_map[i, j, k])
                    {
                        block_map[i, j, k] = Instantiate(BlockPrefab).GetComponent<Block>();
                        block_map[i, j, k].gameObject.transform.parent = e;
                        block_map[i, j, k].Init(i, j, k, 0);
                        block_map[i, j, k].transform.position = new Vector3(BlockWidth * j, -1 * BlockHeight * i, k * 0.5f);
                    }

                }
        }
    }
    private void Update()
    {
        if (UnityEngine.Application.isPlaying) return;
        row = Vertical;
        line = Horizontal;
        block_map = new Block[row, line, 2];
        foreach (var m_block in FindObjectsOfType<Block>())
        {
            block_map[m_block.x, m_block.y, m_block.z] = m_block;
            m_block.ChangeToType(m_block.type);
        }

    }

}
