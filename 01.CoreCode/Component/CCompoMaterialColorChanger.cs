using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-02-17 오후 5:59:36
   Description : a
   Edit Log    : 
   ============================================ */

public class CCompoMaterialColorChanger : CObjectBase
{
    /* const & readonly declaration             */
    static private Dictionary<Material, List<SMaterialSearchInfo>> g_mapMaterialFinder = new Dictionary<Material, List<SMaterialSearchInfo>>();

    /* enum & struct declaration                */

    class SMaterialSearchInfo
    {
        private int _iIndex_Renderer;    public int p_iIndex_Renderer {  get { return _iIndex_Renderer; } }
        private int _iIndex_Material;    public int p_iIndex_Material { get { return _iIndex_Material; } }

        public SMaterialSearchInfo(int iIndex_Renderer, int iIndex_Material)
        {
            this._iIndex_Renderer = iIndex_Renderer; this._iIndex_Material = iIndex_Material;
        }
    }

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */
    [SerializeField]
    private List<Material> _listMaterialControl = null;

    private Renderer[] _arrRenderer;
    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoChangeMaterialColor(Color pColor)
    {
        for (int i = 0; i < _listMaterialControl.Count; i++)
        {
            List<SMaterialSearchInfo> listMaterial = g_mapMaterialFinder[_listMaterialControl[i]];
            for (int j = 0; j < listMaterial.Count; j++)
            {
                int iRendererIndex = listMaterial[j].p_iIndex_Renderer;
                int iMaterialIndex = listMaterial[j].p_iIndex_Material;
                _arrRenderer[iRendererIndex].materials[iMaterialIndex].color = pColor;
            }
        }
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    protected override void OnAwake()
    {
        base.OnAwake();
        _arrRenderer = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < _listMaterialControl.Count; i++)
        {
            if (g_mapMaterialFinder.ContainsKey(_listMaterialControl[i])) continue;

            for (int j = 0; j < _arrRenderer.Length; j++)
            {
                for (int k = 0; k < _arrRenderer[j].materials.Length; k++)
                {
                    if (CheckisSameMaterial(_arrRenderer[j].materials[k], _listMaterialControl[i]))
                    {
                        if (g_mapMaterialFinder.ContainsKey(_listMaterialControl[i]) == false)
                            g_mapMaterialFinder[_listMaterialControl[i]] = new List<SMaterialSearchInfo>();

                        g_mapMaterialFinder[_listMaterialControl[i]].Add(new SMaterialSearchInfo(j, k));
                    }
                }
            }
        }
    }

    // ========================================================================== //

    /* private - [Proc] Function             
           중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

    // Renderer의 Material만 얻어와도 유니티가 스스로
    // Material Instance가 동적으로 생성하므로, (instance)를 붙여 string 체크
    bool CheckisSameMaterial(Material pMaterialOne, Material pMaterialTwo)
    {
        return (pMaterialOne.name.ToString().CompareTo(pMaterialTwo.name.ToString() + " (Instance)")) == 0;
    }
}
