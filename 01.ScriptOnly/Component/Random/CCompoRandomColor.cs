using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CCompoRandomColor : CCompoEventTrigger
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	public Color _pColorRandom_Min = Color.green;
	public Color _pColorRandom_Max = Color.white;

	public bool _bChange_Children = false;

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

#if NGUI
	private UIWidget _pWidget;
#endif
	private UnityEngine.UI.Image _pImage;
	private MeshRenderer _pRenderer_Mesh;
	private SpriteRenderer _pRenderer_Sprite;

	private SpriteRenderer[] _arrRenderer_Sprite;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	public Color GetRandomColor()
	{
		float fRandomR = Random.Range( _pColorRandom_Min.r, _pColorRandom_Max.r );
		float fRandomG = Random.Range( _pColorRandom_Min.g, _pColorRandom_Max.g );
		float fRandomB = Random.Range( _pColorRandom_Min.b, _pColorRandom_Max.b );
		float fRandomA = Random.Range( _pColorRandom_Min.a, _pColorRandom_Max.a );

		return new Color( fRandomR, fRandomG, fRandomB, fRandomA );
	}

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

#if NGUI
		_pWidget = GetComponentInChildren<UIWidget>( true );
#endif
		_pImage = GetComponentInChildren<UnityEngine.UI.Image>( true );
		_pRenderer_Sprite = GetComponentInChildren<SpriteRenderer>(true);
		_pRenderer_Mesh = GetComponentInChildren<MeshRenderer>(true);

		if(_bChange_Children)
			_arrRenderer_Sprite = GetComponentsInChildren<SpriteRenderer>( true );
	}

	protected override void OnPlayEvent()
	{
		base.OnPlayEvent();
		
		Color pColorRandom = GetRandomColor();
		if (_pRenderer_Sprite != null)
			_pRenderer_Sprite.color = pColorRandom;

		if (_pRenderer_Mesh != null)
			_pRenderer_Mesh.material.color = pColorRandom;

		if(_pImage != null)
			_pImage.color = pColorRandom;

		if (_bChange_Children)
		{
			if(_arrRenderer_Sprite != null)
			{
				for (int i = 0; i < _arrRenderer_Sprite.Length; i++)
					_arrRenderer_Sprite[i].color = pColorRandom;
			}
		}

#if NGUI
		if (_pWidget != null)
			_pWidget.color = pColorRandom;
#endif
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
