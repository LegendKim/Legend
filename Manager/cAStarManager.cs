using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cAStarManager : cSingleTon<cAStarManager>
{
    enum eDirection
    {
        LEFT = 1 << 0,
        RIGHT = 1 << 1,
        UP = 1 << 2,
        DOWN = 1 << 3,
    };

    public List<cBlock> m_setOpenList;
    public List<cBlock> m_vecBlock;

    public int m_nTileRow;


    protected override void Awake()
    {
        base.Awake();
        m_vecBlock = new List<cBlock>();
    }

    public void AStarSetting()
    {
        m_vecBlock.Clear();
        m_nTileRow = cMapManager.GetInstance.m_nMapRow;

        for(int i = 0; i< cMapManager.GetInstance.m_blockList.Count; ++i)
        {
            cBlock block = cMapManager.GetInstance.m_blockList[i];
            m_vecBlock.Add(block);
        }
    }

    public Vector3 TargetBlockPos(Vector3 tarGetPos)
    {
        cBlock block = GetNearestNodeIndex(tarGetPos);

        return block.transform.position;

    }


    public void AStarRoute(Vector3 vStart, Vector3 vDest, ref List<Vector3> vecRoute)
    {
        m_setOpenList = new List<cBlock>();

        cBlock pStartNode = GetNearestNodeIndex(vStart); // 몬스터와 가장 가까운 블록(출발지) 찾기
        cBlock pDestNode = GetNearestNodeIndex(vDest); // 플레이어와 가장 가까운 블록(목적지) 찾기
        cBlock block = null;

        if (pStartNode != null && pDestNode != null) // 출발 노드와 목적 노드가 있을 시
        {
            InsertOpenList(ref pStartNode, ref pDestNode, ref block, 0); // 최초 탐색시에 OpenList 첫 노드 넣기

            while (true)
            {
                cBlock pExtNode = FindMinFNode(); // OpenList에서 가장 F값이 작은 노드 찾기 
                if (pExtNode == null) // 길 없음
                    break;

                InsertCloseList(ref pExtNode); // 탐색할 이유가 없는 노드 CloseList에 넣기

                Extend(ref pExtNode, ref pDestNode); // 주변 8개 방향으로 확장하여 탐색

                if (pDestNode.m_blockNum == pExtNode.m_blockNum) // 탐색한 노드와 목적지 노드가 같으면 탐색 종료
                    break;
            }

            List<cBlock> vecPath = new List<cBlock>(); // 경로 블록들을 담을 리스트

            cBlock pTemp = pDestNode;

            while (pTemp) // 마지막 노드의 부모 노드를 따라가며 리스트에 추가
            {
                vecPath.Add(pTemp);
                pTemp = pTemp.m_Parent;
            }

            for (int i = vecPath.Count - 1; i >= 0; --i) // 경로의 마지막 부터 넣어야 올바른 경로완성
            {
                Vector3 pos = vecPath[i].gameObject.transform.position; // 경로의 위치값
                pos.y = 0.5f;
                vecRoute.Add(pos); // 몬스터의 경로에 저장
            }

            for (int i = 0; i < m_vecBlock.Count; ++i)
            {
                if (m_vecBlock[i].m_eBlockState != cBlock.eBlockState.WALL)
                {
                    m_vecBlock[i].m_eBlockState = cBlock.eBlockState.NONE;
                }
                m_vecBlock[i].m_Parent = null;
                m_vecBlock[i].m_fF = 0.0f;
                m_vecBlock[i].m_fG = 0.0f;
                m_vecBlock[i].m_fH = 0.0f;
            }

            m_setOpenList.Clear();
        }
    }


    void InsertOpenList(ref cBlock pNode, ref cBlock pDestNode, ref cBlock pParentNode, float fG)
    {
        pNode.m_fG = fG;
        pNode.m_fH = CalcHeuristic(ref pNode, ref pDestNode);
        pNode.m_fF = pNode.m_fG + pNode.m_fH;
        pNode.m_eBlockState = cBlock.eBlockState.OPEN;

        pNode.m_Parent = pParentNode;
       
       
        m_setOpenList.Add(pNode);
    }

    void InsertCloseList(ref cBlock pNode)
    {
        m_setOpenList.Remove(pNode);
        pNode.m_eBlockState = cBlock.eBlockState.CLOSE;
    }

    float CalcHeuristic(ref cBlock pNode, ref cBlock pDestNode)
    {
        return Mathf.Abs(pNode.m_nRow - pDestNode.m_nRow) +
        Mathf.Abs(pNode.m_nCol - pDestNode.m_nCol);
    }

    cBlock FindMinFNode()
    {
        cBlock pNode = null;
        float fMinF = 99999999.9f;

        for (int i=0; i< m_setOpenList.Count; ++i)
        {
            if (m_setOpenList[i].m_fF < fMinF)
            {
                fMinF = m_setOpenList[i].m_fF;
                pNode = m_setOpenList[i];
            }
        }

        return pNode;
    }

    void Extend(ref cBlock pExtNode, ref cBlock pDestNode)
    {
        eDirection[] aDir = new eDirection[]
        {
        eDirection.LEFT,
        eDirection.RIGHT,
        eDirection.UP,
        eDirection.DOWN,
        eDirection.LEFT | eDirection.UP,
        eDirection.LEFT | eDirection.DOWN,
        eDirection.RIGHT |eDirection.UP,
        eDirection.RIGHT |eDirection.DOWN
        };


        for (int i = 0; i < 8; i++)
        {
            cBlock pAdjNode = GetAdjNode(ref pExtNode, (int)aDir[i]);

            if (pAdjNode == null) continue;

            float fG = pExtNode.m_fG;
            fG += (i < 4 ? 1.0f : Mathf.Sqrt(2.0f));


            bool isFirstOpen = true;
            
            for (int s = 0; s< m_setOpenList.Count; ++s)
            {
                if(m_setOpenList[s].m_blockNum == pAdjNode.m_blockNum)
                {
                    isFirstOpen = false;
                }
            }

            if (isFirstOpen)
            {
                // 탐색에 처음 참여
                InsertOpenList(ref pAdjNode, ref pDestNode, ref pExtNode, fG);
            }
            else
            {
                // 이미 탐색에 참여. 인접 노드가 오픈리스트에 있음.
                if (pAdjNode.m_fG > fG)
                {
                    pAdjNode.m_fG = fG;
                    pAdjNode.m_fF = pAdjNode.m_fG + pAdjNode.m_fH;
                    pAdjNode.m_Parent = pExtNode;
                }
            }
        }
    }

    cBlock GetAdjNode(ref cBlock pExtNode, int nDir)
    {
        int nAdjRow = pExtNode.m_nRow;
        int nAdjCol = pExtNode.m_nCol;

        int left = nDir & (int)(eDirection.LEFT);
        int right = nDir & (int)(eDirection.RIGHT);
        int up = nDir & (int)(eDirection.UP);
        int down = nDir & (int)(eDirection.DOWN);

        if (left >= 1)
        {
            if (pExtNode.m_nCol == 0) return null;
            int index = pExtNode.m_nRow * m_nTileRow + pExtNode.m_nCol - 1;
            if (m_vecBlock[index].m_eBlockType == cBlock.eBlockType.WALL ||
            m_vecBlock[index].m_eBlockType == cBlock.eBlockType.RIVER) return null;
            nAdjCol--;
        }
        else if (right >= 1)
        {
            if (pExtNode.m_nCol == m_nTileRow - 1) return null;
            int index = pExtNode.m_nRow * m_nTileRow + pExtNode.m_nCol + 1;
            if (m_vecBlock[index].m_eBlockType == cBlock.eBlockType.WALL ||
            m_vecBlock[index].m_eBlockType == cBlock.eBlockType.RIVER) return null;
            nAdjCol++;
        }

        if (up >= 1)
        {
            if (pExtNode.m_nRow == 0) return null;
            int index = (pExtNode.m_nRow - 1) * m_nTileRow + pExtNode.m_nCol;
            if (m_vecBlock[index].m_eBlockType == cBlock.eBlockType.WALL ||
            m_vecBlock[index].m_eBlockType == cBlock.eBlockType.RIVER) return null;
            nAdjRow--;
        }
        else if (down >= 1)
        {
            if (pExtNode.m_nRow == m_nTileRow - 1) return null;
            int index = (pExtNode.m_nRow + 1) * m_nTileRow + pExtNode.m_nCol;
            if (m_vecBlock[index].m_eBlockType == cBlock.eBlockType.WALL ||
            m_vecBlock[index].m_eBlockType == cBlock.eBlockType.RIVER) return null;
            nAdjRow++;
        }

        int nIndex = nAdjRow * m_nTileRow + nAdjCol;
        if (m_vecBlock[nIndex].m_eBlockType == cBlock.eBlockType.WALL || 
            m_vecBlock[nIndex].m_eBlockType == cBlock.eBlockType.RIVER) return null;

        if (m_vecBlock[nIndex].m_eBlockState == cBlock.eBlockState.CLOSE) return null;

        return m_vecBlock[nIndex];
    }

    cBlock GetNearestNodeIndex(Vector3 vPos)
    {
        int index = 0;
        float fMinDist = 999999.9f;
        cBlock pNode = null;
        for (int i=0; i< m_vecBlock.Count; ++i)
        {
            Vector3 v = m_vecBlock[i].gameObject.transform.position - vPos;
            float d = Vector3.SqrMagnitude(v);
            if (d < fMinDist && 
                m_vecBlock[i].m_eBlockType != cBlock.eBlockType.WALL && m_vecBlock[i].m_eBlockType != cBlock.eBlockType.RIVER)
            {
                pNode = m_vecBlock[i];
                index = i;
                fMinDist = d;
            }
        }


        return pNode;
    }
}
