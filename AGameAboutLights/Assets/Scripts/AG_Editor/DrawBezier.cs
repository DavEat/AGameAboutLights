using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawBezier : MonoBehaviour {

    #region Var
    public int weightGap = 10;
    public Transform[] points;
    private float weight;
    private Vector3[] pos;
    #endregion
    #region MonoFunction
    BezierUp bezier;
    private void Start()
    {
        weight = 1 / (float)weightGap;
    }
    private void Update()
    {
        BezierCurve();
    }
    #endregion
    #region Function
    private void BezierCurve()
    {
        List<Vector3> posObj = new List<Vector3>();
        for (int i = 0; i < points.Length; i++)
            posObj.Add(points[i].position);
        bezier = new BezierUp(posObj.ToArray());
        bezier.CalculatePoints(weightGap);
        pos = bezier.outPoints;
        for (int i = 1; i < weightGap; i++)
        {
            Debug.DrawLine(pos[i - 1], pos[i], Color.blue, 0);
        }
    }
    #endregion
}

/*[System.Serializable]
public class Bezier : System.Object
{

    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;

    public float length = 0;

    public Vector3[] points;

    // Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point
    // handle1 = v0 + v1
    // handle2 = v3 + v2
    public Bezier(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, int _calculatePoints = 0)
    {
        this.p0 = v0;
        this.p1 = v1;
        this.p2 = v2;
        this.p3 = v3;

        if (_calculatePoints > 0) CalculatePoints(_calculatePoints);
    }

    // 0.0 >= t <= 1.0 her be magic and dragons
    public Vector3 GetPointAtTime(float t)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; //first term
        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term

        return p;
    }

    //where _num is the desired output of points and _precision is how good we want matching to be
    public void CalculatePoints(int _num, int _precision = 100)
    {
        if (_num > _precision) Debug.LogError("_num must be less than _precision");

        //calculate the length using _precision to give a rough estimate, save lengths in array
        length = 0;
        //store the lengths between PointsAtTime in an array
        float[] arcLengths = new float[_precision];

        Vector3 oldPoint = GetPointAtTime(0);

        for (int p = 1; p < arcLengths.Length; p++)
        {
            Vector3 newPoint = GetPointAtTime((float)p / _precision); //get next point
            arcLengths[p] = Vector3.Distance(oldPoint, newPoint); //find distance to old point
            length += arcLengths[p]; //add it to the bezier's length
            oldPoint = newPoint; //new is old for next loop
        }

        //create our points array
        points = new Vector3[_num];
        //target length for spacing
        float segmentLength = length / _num;

        //arc index is where we got up to in the array to avoid the Shlemiel error http://www.joelonsoftware.com/articles/fog0000000319.html
        int arcIndex = 0;

        float walkLength = 0; //how far along the path we've walked
        oldPoint = GetPointAtTime(0);

        //iterate through points and set them
        for (int i = 0; i < points.Length; i++)
        {
            float iSegLength = i * segmentLength; //what the total length of the walkLength must equal to be valid
                                                  //run through the arcLengths until past it
            while (walkLength < iSegLength)
            {
                walkLength += arcLengths[arcIndex]; //add the next arcLength to the walk
                arcIndex++; //go to next arcLength
            }
            //walkLength has exceeded target, so lets find where between 0 and 1 it is
            points[i] = GetPointAtTime((float)arcIndex / arcLengths.Length);

        }


    }
}*/

[System.Serializable]
public class BezierUp : System.Object
{

    public Vector3[] inPoints;

    public float length = 0;

    public Vector3[] outPoints;

    // Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point
    // handle1 = v0 + v1
    // handle2 = v3 + v2
    public BezierUp(Vector3[] points, int _calculatePoints = 0)
    {
        inPoints = points;

        if (_calculatePoints > 0) CalculatePoints(_calculatePoints);
    }

    // 0.0 >= t <= 1.0
    public Vector3 GetPointAtTime(float t)
    {
        float u = 1 - t;
        float tt = t;
        Vector3 p = Mathf.Pow(u, inPoints.Length - 1) * inPoints[0];
        for (int i = 1; i < inPoints.Length; i++)
        {
            Vector3 value = (Mathf.Pow(u, inPoints.Length - (i + 1)) * Mathf.Pow(t, i)) * inPoints[i];
            p += value * Multiplicator(inPoints.Length - 1, i);
        }

        return p;
    }

    // x 6 152015 6
    //   5 1010 5
    //   4 6 4
    //   3 3
    //   2
    //   1          y
    private int Multiplicator(int x, int y)
    {
        if (x == 0 || y == 0)
            return 1;
        else if (x == 1)
            return y;
        else
        {
            int[][] array = new int[x + 1][];
            for (int i = 0; i < array.Length; i++)
                array[i] = new int[y + 1];

            for (int i = 0; i < array.Length; i++)
                array[i][0] = 1;

            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    if (i != 0 && j != 0) 
                        array[i][j] = array[i - 1][j - 1] + array[i - 1][j];
                }
            }

            // Debug.Log(array[0][0] + " " + array[0][1] + " " + array[0][2] + " " + array[0][3] + " " + array[0][4]
            //  + "\n" + array[0][0] + " " + array[1][1] + " " + array[1][2] + " " + array[1][3] + " " + array[1][4]
            //  + "\n" + array[0][0] + " " + array[2][1] + " " + array[2][2] + " " + array[2][3] + " " + array[2][4]
            //  + "\n" + array[0][0] + " " + array[3][1] + " " + array[3][2] + " " + array[3][3] + " " + array[3][4]);

            return array[x][y];
        }
    }

    //where _num is the desired output of points and _precision is how good we want matching to be
    public void CalculatePoints(int _num, int _precision = 100)
    {
        if (_num > _precision) Debug.LogError("_num must be less than _precision");

        //calculate the length using _precision to give a rough estimate, save lengths in array
        length = 0;
        //store the lengths between PointsAtTime in an array
        float[] arcLengths = new float[_precision];

        Vector3 oldPoint = GetPointAtTime(0);

        for (int p = 1; p < arcLengths.Length; p++)
        {
            Vector3 newPoint = GetPointAtTime((float)p / _precision); //get next point
            arcLengths[p] = Vector3.Distance(oldPoint, newPoint); //find distance to old point
            length += arcLengths[p]; //add it to the bezier's length
            oldPoint = newPoint; //new is old for next loop
        }

        //create our points array
        outPoints = new Vector3[_num];
        //target length for spacing
        float segmentLength = length / _num;

        //arc index is where we got up to in the array to avoid the Shlemiel error http://www.joelonsoftware.com/articles/fog0000000319.html
        int arcIndex = 0;

        float walkLength = 0; //how far along the path we've walked
        oldPoint = GetPointAtTime(0);

        //iterate through points and set them
        for (int i = 0; i < outPoints.Length; i++)
        {
            float iSegLength = i * segmentLength; //what the total length of the walkLength must equal to be valid
                                                  //run through the arcLengths until past it
            while (walkLength < iSegLength)
            {
                walkLength += arcLengths[arcIndex]; //add the next arcLength to the walk
                arcIndex++; //go to next arcLength
            }
            //walkLength has exceeded target, so lets find where between 0 and 1 it is
            outPoints[i] = GetPointAtTime((float)arcIndex / arcLengths.Length);
        }
    }
}