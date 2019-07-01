#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-06 오전 10:36:46
 *	기능 : Component에 종속되지 않는 Shake 매니져
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CManagerShake : CSingletonNotMonoBase<CManagerShake>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EShakePos
    {
        All,

        X,
        XY,
        XZ,
        Y,
        YZ,
        Z
    }

    public struct SShakeData : IDictionaryItem<Transform>
    {
        Transform pTransform;
        Vector3 vecOriginPos;

        EShakePos eShakePosType;

        float _fRemainShakePow;
        float _fShakeMinusDelta;

        public SShakeData(Transform pTransform, EShakePos eShakePos, float fShakePower, float fShakeMinusDelta)
        {
            this.pTransform = pTransform;
            this.vecOriginPos = pTransform.position;

            eShakePosType = eShakePos;
            _fRemainShakePow = fShakePower;
            _fShakeMinusDelta = fShakeMinusDelta;
        }

        public void Shake(out bool bIsFinish)
        {
            if (_fRemainShakePow > 0f)
            {
                Vector3 vecShakePos = PrimitiveHelper.RandomRange(vecOriginPos.AddFloat(-_fRemainShakePow), vecOriginPos.AddFloat(_fRemainShakePow));
                if (eShakePosType != EShakePos.All)
                {
                    if (eShakePosType == EShakePos.Y || eShakePosType == EShakePos.YZ || eShakePosType == EShakePos.Z)
                        vecShakePos.x = vecOriginPos.x;

                    if (eShakePosType == EShakePos.X || eShakePosType == EShakePos.XZ || eShakePosType == EShakePos.Z)
                        vecShakePos.y = vecOriginPos.y;

                    if (eShakePosType == EShakePos.X || eShakePosType == EShakePos.XY || eShakePosType == EShakePos.Y)
                        vecShakePos.z = vecOriginPos.z;
                }

                pTransform.position = vecShakePos;
                _fRemainShakePow -= _fShakeMinusDelta;
                bIsFinish = false;
            }
            else
            {
                pTransform.position = vecOriginPos;
                bIsFinish = true;
            }
        }

        public Transform IDictionaryItem_GetKey()
        {
            return pTransform;
        }
    }

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    List<SShakeData> _listShakeData = new List<SShakeData>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoShakeObject(Transform pTransform, EShakePos eShakePos, float fShakePower, float fShakeMinusDelta)
    {
        _listShakeData.Add(new SShakeData(pTransform, eShakePos, fShakePower, fShakeMinusDelta));
    }

    public Vector3 DoCalculate_ShakePosition(Vector3 vecOriginPos, EShakePos eShakePosType, float fShakePower)
    {
        Vector3 vecShakePos = PrimitiveHelper.RandomRange(vecOriginPos.AddFloat(-fShakePower), vecOriginPos.AddFloat(fShakePower));
        if (eShakePosType != EShakePos.All)
        {
            if (eShakePosType == EShakePos.Y || eShakePosType == EShakePos.YZ || eShakePosType == EShakePos.Z)
                vecShakePos.x = vecOriginPos.x;

            if (eShakePosType == EShakePos.X || eShakePosType == EShakePos.XZ || eShakePosType == EShakePos.Z)
                vecShakePos.y = vecOriginPos.y;

            if (eShakePosType == EShakePos.X || eShakePosType == EShakePos.XY || eShakePosType == EShakePos.Y)
                vecShakePos.z = vecOriginPos.z;
        }

        return vecShakePos;
    }

    public void DoRemove_ShakeObject()
    {

    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnMakeSingleton(out bool bIsGenearteGameObject, out bool bIsUpdateAble)
    {
        base.OnMakeSingleton(out bIsGenearteGameObject, out bIsUpdateAble);

        bIsGenearteGameObject = true;
        bIsUpdateAble = true;
    }

    public override void OnUpdate(float fTimeScale_Individual)
    {
        base.OnUpdate(fTimeScale_Individual);

        for (int i = 0; i < _listShakeData.Count; i++)
        {
            bool bIsFinish;
            SShakeData sShakeData = _listShakeData[i];
            sShakeData.Shake(out bIsFinish);

            if (bIsFinish)
            {
                _listShakeData.RemoveAt(i);
                i--;
            }
            else
                _listShakeData[i] = sShakeData;
        }

        gameObject.name = string.Format("Shake/오브젝트개수:{0}", _listShakeData.Count);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}