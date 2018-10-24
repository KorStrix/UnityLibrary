#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CCameraToSpriteRenderer : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	public int p_iRenderTexture_Width = 512;
	public int p_iRenderTexture_Height = 512;

	public float p_fUpdateDelaySec = 1f;

	public SpriteRenderer p_pSpriteRendererTarget;

	/* protected - Field declaration         */

	/* private - Field declaration           */

	[Header("For Debug")]
	[SerializeField]
	private RenderTexture _pRenderTexture;
	[SerializeField]
	private Texture2D _pTextureCopy;

	private Camera _pCamera;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	#region Protected

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		_pCamera = GetComponent<Camera>();
		_pRenderTexture = new RenderTexture( p_iRenderTexture_Width, p_iRenderTexture_Height, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear );
		System.IntPtr pTexturePointer = _pRenderTexture.GetNativeTexturePtr();

		_pTextureCopy = Texture2D.CreateExternalTexture( _pRenderTexture.width, _pRenderTexture.height, TextureFormat.ARGB32, false, false, pTexturePointer );

		p_pSpriteRendererTarget.sprite = Sprite.Create( _pTextureCopy, new Rect( 0f, 0f, p_iRenderTexture_Width, p_iRenderTexture_Height ), new Vector3( 0.5f, 0.5f ) );
		_pCamera.targetTexture = _pRenderTexture;
	}

	protected override void OnEnableObject()
	{
		base.OnEnableObject();

		StartCoroutine( CoUpdateTexture() );
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	private IEnumerator CoUpdateTexture()
	{
		while(true)
		{
			yield return new WaitForEndOfFrame();
			System.IntPtr pTexturePointer = _pRenderTexture.GetNativeTexturePtr();
			_pTextureCopy.UpdateExternalTexture( pTexturePointer );

			yield return SCManagerYield.GetWaitForSecond( p_fUpdateDelaySec );
		}
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
