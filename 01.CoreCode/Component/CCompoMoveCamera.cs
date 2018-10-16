// Just add this script to your camera. It doesn't need any configuration.

using UnityEngine;

public class CCompoMoveCamera : CObjectBase
{
    [SerializeField]
    private bool bRotateEnable = false;
    //[SerializeField]
    //private float fSpeedMove = 1f;
    [SerializeField]
    private Vector3 vecPosLimitMin = Vector3.zero;
    [SerializeField]
    private Vector3 vecPosLimitMax =Vector3.zero;
    //[SerializeField]
    //private float fSpeedZoom = 1f;
    [SerializeField]
    private float fZoomLimitMin = 30f;
    [SerializeField]
    private float fZoomLimitMax = 90f;

    private Camera _pCam;

    Vector2?[] oldTouchPositions = { null, null };
	Vector2 oldTouchVector;
	float oldTouchDistance;

    // ========================== [ Division ] ========================== //

    protected override void OnAwake()
    {
        base.OnAwake();

        _pCam = GetComponent<Camera>();
    }

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate();
        bCheckUpdateCount = true;

        int iTouchCount = 0;// UICamera.CountInputSources();
        if (iTouchCount == 0)
        {
			oldTouchPositions[0] = null;
			oldTouchPositions[1] = null;
		}
		else if (iTouchCount == 1)
        {
			if (oldTouchPositions[0] == null || oldTouchPositions[1] != null) {
				//oldTouchPositions[0] = UICamera.lastEventPosition;
				oldTouchPositions[1] = null;
			}
			else
            {
				//Vector2 newTouchPosition = UICamera.lastEventPosition;
    //            Vector3 vecConvert = _pTransformCached.TransformDirection((Vector3)((oldTouchPositions[0] - newTouchPosition) * _pCam.orthographicSize / _pCam.pixelHeight * 2f));
    //            vecConvert.z = vecConvert.y;
    //            vecConvert.y = 0f;
    //            _pTransformCached.position += vecConvert;
				//oldTouchPositions[0] = newTouchPosition;
			}
		}
		else
        {
			if (oldTouchPositions[1] == null) {
				oldTouchPositions[0] = Input.GetTouch(0).position;
                oldTouchPositions[1] = Input.GetTouch(1).position;
                oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
				oldTouchDistance = oldTouchVector.magnitude;
			}
			else
            {
				Vector2 screen = new Vector2(_pCam.pixelWidth, _pCam.pixelHeight);
				Vector2[] newTouchPositions = {
                    Input.GetTouch(0).position,
                    Input.GetTouch(1).position,
                };

				Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
				float newTouchDistance = newTouchVector.magnitude;

                Vector3 vecConvert = transform.TransformDirection((Vector3)((oldTouchPositions[0] + oldTouchPositions[1] - screen) * _pCam.orthographicSize / screen.y));
                vecConvert.z = vecConvert.y;
                vecConvert.y = 0f;

                transform.position += vecConvert;
                if(bRotateEnable)
                    transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, Mathf.Asin(Mathf.Clamp((oldTouchVector.y * newTouchVector.x - oldTouchVector.x * newTouchVector.y) / oldTouchDistance / newTouchDistance, -1f, 1f)) / 0.0174532924f));
                _pCam.fieldOfView *= oldTouchDistance / newTouchDistance;

                vecConvert = transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen) * _pCam.orthographicSize / screen.y);
                vecConvert.z = vecConvert.y;
                vecConvert.y = 0f;
                transform.position -= vecConvert;

				oldTouchPositions[0] = newTouchPositions[0];
				oldTouchPositions[1] = newTouchPositions[1];
				oldTouchVector = newTouchVector;
				oldTouchDistance = newTouchDistance;
			}
		}

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            _pCam.fieldOfView--;
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            _pCam.fieldOfView++;


        Vector3 vecPos = transform.position;
        if (vecPos.x < vecPosLimitMin.x) vecPos.x = vecPosLimitMin.x;
        if (vecPos.x > vecPosLimitMax.x) vecPos.x = vecPosLimitMax.x;
        if (vecPos.z < vecPosLimitMin.z) vecPos.z = vecPosLimitMin.z;
        if (vecPos.z > vecPosLimitMax.z) vecPos.z = vecPosLimitMax.z;
        transform.position = vecPos;

        float fFOV = _pCam.fieldOfView;
        if (fFOV < fZoomLimitMin) fFOV = fZoomLimitMin;
        if (fFOV > fZoomLimitMax) fFOV = fZoomLimitMax;
        _pCam.fieldOfView = fFOV;
    }
}
