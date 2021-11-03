using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFieldOfView : MonoBehaviour
{
    public MeshFilter FOWMeshFilter;
    Mesh _FOWMesh;
    MeshRenderer _FOWRenderer;

    [HideInInspector] public FOWMaterial FOWMode;
    [HideInInspector] public Transform VisiblePlayer;
    [HideInInspector] public Vector3 LastPlayerPosition;
    [HideInInspector] public Vector3 LastPlayerDirection;

    [Space(10)]
    [SerializeField] float _meshResolution;
    [SerializeField] int _edgeResolveIterations;
	[SerializeField] float _edgeDstThreshold;

    [Space(10)]
    [SerializeField] LayerMask _targetMask;
    [SerializeField] LayerMask _obstacleMask;

    [Space(10)]
    [SerializeField] Material _calmFOWMaterial;
    [SerializeField] Material _attentionFOWMaterial;
    [SerializeField] Material _alarmFOWMaterial;

    [Space(10)]
    [SerializeField] BookDeactivating _bookDeactivator;

    List<Vector3> _viewPoints = new List<Vector3>();
    //Collider[] targetsInRadius = new Collider[10];


    float _rangeOfView;
    float _aroundAreaOfView;
    float _angleOfView;
    float _leftoverAngleOfView;

    void Awake()
    {
        _bookDeactivator.OnBookDissapearing += SetEscapeSettings;
    }

    void Start()
    {
        _FOWMesh = new Mesh();
        FOWMeshFilter.name = "FOW Mesh";
        FOWMeshFilter.mesh = _FOWMesh;

        _FOWRenderer = FOWMeshFilter.GetComponent<MeshRenderer>();

        _rangeOfView = AIOptions.RangeOfView;
        _aroundAreaOfView = AIOptions.AroundAreaOfView;
        _angleOfView = AIOptions.AngleOfView;
        _leftoverAngleOfView = 360 - _angleOfView;
    }

    void LateUpdate()
    {
            VisiblePlayer = null;      
            FindVisibleTargets(_angleOfView, _rangeOfView, true);
            FindVisibleTargets(_leftoverAngleOfView, _aroundAreaOfView, false);
        
            DrawFieldOfView();
            SetFOWColor();
    }

    void SetFOWColor()
    {
        if (_FOWRenderer)
        {
            switch (FOWMode)
            {
                case FOWMaterial.calm :
                {
                    if (_FOWRenderer.material != _calmFOWMaterial)
                        _FOWRenderer.material = _calmFOWMaterial;
                    break;
                }
                case FOWMaterial.attention :
                {
                    if (_FOWRenderer.material != _attentionFOWMaterial)
                        _FOWRenderer.material = _attentionFOWMaterial;
                    break;
                }
                case FOWMaterial.alarm :
                {
                    if (_FOWRenderer.material != _alarmFOWMaterial)
                        _FOWRenderer.material = _alarmFOWMaterial;
                    break;
                }
            }
        }
    }

    void SetEscapeSettings(BookDeactivating bookDeactivator)
    {
        _rangeOfView = AIOptions.RangeOfViewEscape;
        _aroundAreaOfView = AIOptions.AroundAreaOfViewEscape;
        _angleOfView = AIOptions.AngleOfViewEscape;
        _leftoverAngleOfView = 360 - _angleOfView;
    }

    void FindVisibleTargets(float viewAngle, float viewRadius, bool inFrontView)
    {
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position + Vector3.up, viewRadius, _targetMask);

        foreach (Collider target in targetsInRadius)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            
            float angleToTarget;

            if (inFrontView)
                angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            else
                angleToTarget = Vector3.Angle(-transform.forward, directionToTarget);

            if (angleToTarget < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    if (target.GetComponent<UnitInfo>().isPlayer)
                    {
                        VisiblePlayer = target.transform;
                        LastPlayerPosition = VisiblePlayer.position;
                        LastPlayerDirection = VisiblePlayer.forward;
                    }
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(_angleOfView * _meshResolution);
        float stepAngleSize = _angleOfView / stepCount;
        _viewPoints.Clear();
        ViewCastInfo oldViewCast = new ViewCastInfo ();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - _angleOfView / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle, _rangeOfView);

            if (i > 0) 
            {
				bool edgeDstThresholdExceeded = Mathf.Abs (oldViewCast.Distance - newViewCast.Distance) > _edgeDstThreshold;
				if (oldViewCast.Hitted != newViewCast.Hitted || (oldViewCast.Hitted && newViewCast.Hitted && edgeDstThresholdExceeded)) 
                {
					EdgeInfo edge = FindEdge (oldViewCast, newViewCast, _rangeOfView);
					if (edge.PointA != Vector3.zero) _viewPoints.Add (edge.PointA);
					if (edge.PointB != Vector3.zero) _viewPoints.Add (edge.PointB);
				}
			}

            _viewPoints.Add(newViewCast.Point);
            oldViewCast = newViewCast;
        }

        oldViewCast = new ViewCastInfo ();
        stepCount = Mathf.RoundToInt(_leftoverAngleOfView * _meshResolution);
        stepAngleSize = _leftoverAngleOfView / stepCount;
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = (transform.eulerAngles.y + 180f) - _leftoverAngleOfView / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle, _aroundAreaOfView);

            if (i > 0) 
            {
				bool edgeDstThresholdExceeded = Mathf.Abs (oldViewCast.Distance - newViewCast.Distance) > _edgeDstThreshold;
				if (oldViewCast.Hitted != newViewCast.Hitted || (oldViewCast.Hitted && newViewCast.Hitted && edgeDstThresholdExceeded)) 
                {
					EdgeInfo edge = FindEdge (oldViewCast, newViewCast, _aroundAreaOfView);
					if (edge.PointA != Vector3.zero) _viewPoints.Add (edge.PointA);
					if (edge.PointB != Vector3.zero) _viewPoints.Add (edge.PointB);
				}
			}

            _viewPoints.Add(newViewCast.Point);
            oldViewCast = newViewCast;
        }
        int vertexCount = _viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(_viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _FOWMesh.Clear();
        _FOWMesh.vertices = vertices;
        _FOWMesh.triangles = triangles;
        _FOWMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float range) 
    {
		float minAngle = minViewCast.Angle;
		float maxAngle = maxViewCast.Angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < _edgeResolveIterations; i++) 
        {
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast (angle, range);

			bool edgeDstThresholdExceeded = Mathf.Abs (minViewCast.Distance - newViewCast.Distance) > _edgeDstThreshold;
			if (newViewCast.Hitted == minViewCast.Hitted && !edgeDstThresholdExceeded) 
            {
				minAngle = angle;
				minPoint = newViewCast.Point;
			} 
            else 
            {
				maxAngle = angle;
				maxPoint = newViewCast.Point;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);
	}

    public Vector3 GetDirFromAngle(float angle, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    ViewCastInfo ViewCast(float globalAngle, float viewRadius)
    {
        Vector3 direction = GetDirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, viewRadius, _obstacleMask))
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        else
            return new ViewCastInfo(false, transform.position + direction * viewRadius, viewRadius, globalAngle);
    }

    public struct ViewCastInfo
    {
        public bool Hitted;
        public Vector3 Point;
        public float Distance;
        public float Angle;

        public ViewCastInfo(bool hitted, Vector3 point, float distance, float angle)
        {
            Hitted = hitted;
            Point = point;
            Distance = distance;
            Angle = angle;
        }
    }

    public struct EdgeInfo 
    {
		public Vector3 PointA;
		public Vector3 PointB;

		public EdgeInfo(Vector3 pointA, Vector3 pointB) {
			PointA = pointA;
			PointB = pointB;
		}
	}
}

public enum FOWMaterial
{
    calm,
    attention,
    alarm
}
