using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   ============================================ */

public class CRandomSector2D : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	public enum ESectorAroundCheckOption
	{
		None,
		//Side,
		//Up_And_Down,
		FourWay,
		EightWay,
	}


	public struct SSectorIndex
	{
		public int iX;
		public int iY;

		public SSectorIndex(int iX, int iY)
		{
			this.iX = iX;
			this.iY = iY;
		}

		public bool CheckIsEqual(SSectorIndex pTargetSectorIndex)
		{
			return (iX == pTargetSectorIndex.iX && iY == pTargetSectorIndex.iY);
		}
	}

	/* public - Variable declaration            */

	[Header( "섹터 구간 설정" )]
	[SerializeField]
	private Transform _pTrans_LeftDown = null;
	[SerializeField]
	private Transform _pTrans_RightUp = null;

	[Header( "섹터를 얼마나 나눌것인지" )]
	[SerializeField]
	private int _iSectorDivision_X = 2;
	[SerializeField]
	private int _iSectorDivision_Y = 2;

	[Header( "주변 섹터에 생성 금지 옵션" )]
	[SerializeField]
	private ESectorAroundCheckOption eCheckOption = ESectorAroundCheckOption.None;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<int, HashSet<SSectorIndex>> _mapUseSector = new Dictionary<int, HashSet<SSectorIndex>>();

	private float _fSectorTotalPos_Right;
	private float _fSectorTotalPos_Down;

	private float _fSectorTotalPos_Left;
	private float _fSectorTotalPos_Up;

	private float _fSectorUnit_X;
	private float _fSectorUnit_Y;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public Vector2 GetRandomPosition_UseCheckOption( )
	{
        return ProcCheckEmptySector(GetInstanceID(), true);
	}

	public Vector2 GetRandomPosition_UseCheckOption(MonoBehaviour pTarget)
	{
		return ProcCheckEmptySector( pTarget.GetInstanceID(), false);
	}

	public Vector2 GetRandomPosition_UseCheckOption( int iObjectID )
	{
		return ProcCheckEmptySector( iObjectID, false);
	}

	public void DoRemoveUseSector( MonoBehaviour pTarget )
	{
		_mapUseSector.Remove( pTarget.GetInstanceID() );
	}

	public void DoRemoveUseSector(int iKey)
	{
		_mapUseSector.Remove(iKey);
	}

	public Vector2 GetRandomPosition()
	{
		float fRandX = Random.Range(_fSectorTotalPos_Left, _fSectorTotalPos_Right);
		float fRandY = Random.Range(_fSectorTotalPos_Down, _fSectorTotalPos_Up);

		return new Vector2(fRandX, fRandY);
	}

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _fSectorTotalPos_Up = _pTrans_RightUp.position.y;
        _fSectorTotalPos_Down = _pTrans_LeftDown.position.y;

        _fSectorTotalPos_Left = _pTrans_LeftDown.position.x;
        _fSectorTotalPos_Right = _pTrans_RightUp.position.x;

        _fSectorUnit_X = Mathf.Abs(_fSectorTotalPos_Right - _fSectorTotalPos_Left) / _iSectorDivision_X;
        _fSectorUnit_Y = Mathf.Abs(_fSectorTotalPos_Up - _fSectorTotalPos_Down) / _iSectorDivision_Y;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
	{
        EventOnAwake_Force();
        Gizmos.color = Color.red;

		float fDrawPosX = _fSectorTotalPos_Left;
		float fDrawPos_StartY = _fSectorTotalPos_Up;
		float fDrawPos_DestY = _fSectorTotalPos_Down;

		for (int i = -1; i < _iSectorDivision_X; i++)
		{
			Gizmos.DrawLine( new Vector2( fDrawPosX, fDrawPos_StartY ), new Vector2( fDrawPosX, fDrawPos_DestY ));
			fDrawPosX += _fSectorUnit_X;
		}

		float fDrawPosY = _fSectorTotalPos_Down;
		float fDrawPos_StartX = _fSectorTotalPos_Left;
		float fDrawPos_DestX = _fSectorTotalPos_Right;

		for (int i = -1; i < _iSectorDivision_Y; i++)
		{
			Gizmos.DrawLine( new Vector2( fDrawPos_StartX, fDrawPosY ), new Vector2( fDrawPos_DestX, fDrawPosY ) );
			fDrawPosY += _fSectorUnit_Y;
		}
	}
#endif

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

    private Vector2 ProcCheckEmptySector(int iObjectID, bool bCheckOnlyOnce)
	{
        // 총길이(300) _arrPosition[(int)EVector2.X].x - 랜덤좌표(-115)

        if (_mapUseSector.ContainsKey(iObjectID) == false)
            _mapUseSector.Add(iObjectID, new HashSet<SSectorIndex>());

        HashSet<SSectorIndex> setCheckSector = _mapUseSector[iObjectID];

        int iCount = 0;
		while (iCount < 100)
		{
			Vector2 v2RandPos = GetRandomPosition();
			SSectorIndex sSectorIndex;
			sSectorIndex.iX = (int)(Mathf.Abs(_fSectorTotalPos_Right - v2RandPos.x) / _fSectorUnit_X);
			sSectorIndex.iY = (int)(Mathf.Abs(_fSectorTotalPos_Up - v2RandPos.y) / _fSectorUnit_Y);

            if(setCheckSector.Contains(sSectorIndex) == false)
            {
                if (eCheckOption == ESectorAroundCheckOption.None || CheckIs_AlreadyUse_SectorAround(setCheckSector, sSectorIndex) == false)
                {
                    if (bCheckOnlyOnce)
                        setCheckSector.Clear();

                    setCheckSector.Add(sSectorIndex);
                    AddCheckSector(setCheckSector, sSectorIndex);

                    return v2RandPos;
                }
            }

            iCount++;
		}

		if (iCount >= 100)
			Debug.Log( "ProcCheckEmptySector Loop Count > 100" );

		return Vector2.zero;
	}

	private bool CheckIs_AlreadyUse_SectorAround(HashSet<SSectorIndex> setCheckSector, SSectorIndex pSectorIndex)
	{
		if(eCheckOption == ESectorAroundCheckOption.FourWay)
		{
			for (int i = 0; i < 3; i++)
			{
                int iIndexX = pSectorIndex.iX - 1 + i;
				if (setCheckSector.Contains( new SSectorIndex(iIndexX, pSectorIndex.iY ) ))
					return true;
			}

			for (int i = 0; i < 3; i++)
			{
                int iIndexY = pSectorIndex.iY - 1 + i;
				if (setCheckSector.Contains(new SSectorIndex(pSectorIndex.iX, iIndexY)))
					return true;
			}
		}
		else if(eCheckOption == ESectorAroundCheckOption.EightWay)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
                    int iIndexX = pSectorIndex.iX - 1 + i;
                    int iIndexY = pSectorIndex.iY - 1 + j;

                    if (setCheckSector.Contains(new SSectorIndex(iIndexX, iIndexY)))
						return true;
				}
			}
		}

		return false;
	}

    private void AddCheckSector(HashSet<SSectorIndex> setCheckSector, SSectorIndex pSectorIndex)
    {
        if (eCheckOption == ESectorAroundCheckOption.FourWay)
        {
            for (int i = 0; i < 3; i++)
            {
                int iIndexX = pSectorIndex.iX - 1 + i;
                setCheckSector.Add(new SSectorIndex(iIndexX, pSectorIndex.iY));
            }

            for (int i = 0; i < 3; i++)
            {
                int iIndexY = pSectorIndex.iY - 1 + i;
                setCheckSector.Add(new SSectorIndex(pSectorIndex.iX, iIndexY));
            }
        }
        else if (eCheckOption == ESectorAroundCheckOption.EightWay)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int iIndexX = pSectorIndex.iX - 1 + i;
                    int iIndexY = pSectorIndex.iY - 1 + j;
                    setCheckSector.Add(new SSectorIndex(iIndexX, iIndexY));
                }
            }
        }
    }
}

#region Test
#if UNITY_EDITOR

public class Test_CRandomSector2D
{

}

#endif
#endregion