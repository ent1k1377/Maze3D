using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPath : MonoBehaviour
{
    [SerializeField] List<Transform> _points = new List<Transform>();

    [SerializeField] [Range(0.01f, 1f)] float _pointerSpeed;
    float _step = 0f;

    Coroutine _movePointer;


    void Update()
    {
        if (_movePointer == null)
        {
            _movePointer = StartCoroutine(MovePointer());
        }

        //transform.position = GetPoint(_points, Mathf.SmoothStep(0f, 1f, _step));
        //_step += _pointerSpeed * Time.deltaTime;

        //if (_step > 1f)
        //{
        //    _step = _step - 1f;
        //}
    }

    IEnumerator MovePointer()
    {
        while (_step < 1f)
        {
            transform.position = GetPoint(_points, Mathf.SmoothStep(0f, 1f, _step));
            _step += _pointerSpeed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        _step = 0f;

        _movePointer = null;
    }

    Vector3 GetPoint(List<Transform> points, float step) 
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Transform point in points)
        {
            positions.Add(point.position);
        }
        
        while (positions.Count > 1)
        {
            List<Vector3> tempPositions = new List<Vector3>(positions);
            positions.Clear();
            for (int i = 0; i < tempPositions.Count - 1; i++)
            {
                Vector3 pos = Vector3.Lerp(tempPositions[i], tempPositions[i + 1], step);
                positions.Add(pos);
            }
        }

        return positions[0];
    }

    private void OnDrawGizmos() 
    {
        if (_points.Count > 0)
        {
            int sigmentsNumber = 50;
            Vector3 preveousePoint = _points[0].position;

            for (int i = 0; i < sigmentsNumber + 1; i++) 
            {
                float paremeter = (float)i / sigmentsNumber;
                Vector3 point = GetPoint(_points, paremeter);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(preveousePoint, point);
                preveousePoint = point;
            }
        }

    }

}
