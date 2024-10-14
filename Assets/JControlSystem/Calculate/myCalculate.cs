using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myCalculate 
{



    public static Quaternion Rotation_BversusA(Quaternion B, Quaternion A)
    {
        return Quaternion.Inverse(A) * B;
    }

    public static Vector3 GetRight(Quaternion worldRotation)
    {
        return worldRotation * Vector3.right ;
    }
    public static Vector3 GetUp(Quaternion worldRotation)
    {
        return worldRotation * Vector3.up;
    }
    public static Vector3 GetForward(Quaternion worldRotation)
    {
        return worldRotation * Vector3.forward;
    }

    public static Vector3 Pos_BversusA(Vector3 Bpos, Transform A)
    {
        Vector3 v1 = Bpos - A.position;
        Vector3 v_right =
            ProjectVectorOnPlane(ProjectVectorOnPlane(v1, A.up), A.forward);
        Vector3 v_up =
            ProjectVectorOnPlane(ProjectVectorOnPlane(v1, A.forward), A.right);
        Vector3 v_forward =
            ProjectVectorOnPlane(ProjectVectorOnPlane(v1, A.up), A.right);

        Vector3 V_Offset = new Vector3(v_right.magnitude, v_up.magnitude, v_forward.magnitude);
        if (Vector3.Dot(A.right, v_right) < 0f)
        {
            V_Offset.x = -V_Offset.x;
        }
        if (Vector3.Dot(A.up, v_up) < 0f)
        {
            V_Offset.y = -V_Offset.y;
        }
        if (Vector3.Dot(A.forward, v_forward) < 0f)
        {
            V_Offset.z = -V_Offset.z;
        }

        return V_Offset;
    }

    /// <summary>
	/// 投影一个向量到某个平面
	/// </summary>
	/// <returns>The vector on plane.</returns>
	/// <param name="v">要被投影的向量</param>
	/// <param name="planeNormal">投影的平面</param>
	public static Vector3 ProjectVectorOnPlane(Vector3 v, Vector3 planeNormal)
    {
        planeNormal.Normalize();
        float distance = -Vector3.Dot(planeNormal.normalized, v);
        return v + planeNormal * distance;
    }


    public static float GetNearFloat(float _value)
    {
        string st1 = _value.ToString();
        if (string.IsNullOrEmpty(st1))
        {
            return 0;
        }
        else
        {
            string st2 = "";
            if (_value > 0f && _value < 1f)
            {
                for (int i = 0; i < st1.Length; i++)
                {
                    if (st1[i] != '-' && st1[i] != '.' && st1[i] != '0')
                    {
                        if (st1[i] >= '7' && st1[i] <= '9')//+1
                        {
                            int j = i;
                            j--;
                            if (j >= 0)
                            {
                                if (st1[j] != '.')
                                {
                                    st2 = st2.Substring(0, j) + '1';
                                }
                                else
                                {
                                    st2 = "1";
                                }
                            }
                            else
                            {
                                //不可能
                            }
                            break;
                        }
                        else if (st1[i] >= '4' && st1[i] <= '6')
                        {
                            st2 = st2 + '5';
                            break;
                        }
                        else
                        {
                            st2 = st2 + '1';
                            break;
                        }
                    }
                    else
                    {
                        st2 = st2 + st1[i];
                    }
                }
            }
            else if (_value >= 1f)
            {
                int i = 0;
                if (st1[i] >= '4' && st1[i] <= '6')
                {
                    st2 = st2 + '5';
                }
                else if (st1[i] >= '7' && st1[i] <= '9')//+1
                {
                    st2 = "10";
                }
                else
                {
                    st2 = "1";
                }
                i++;
                for (; i < st1.Length; i++)
                {
                    if (st1[i] == '.') break;
                    st2 = st2 + '0';
                }
            }
            else if (_value < 0f && _value > -1f)
            {
                for (int i = 0; i < st1.Length; i++)
                {
                    if (st1[i] != '-' && st1[i] != '.' && st1[i] != '0')
                    {
                        if (st1[i] >= '7' && st1[i] <= '9')//+1
                        {
                            int j = i;
                            j--;
                            if (j >= 0)
                            {
                                if (st1[j] != '.')
                                {
                                    st2 = st2.Substring(0, j) + '1';
                                }
                                else
                                {
                                    st2 = "-1";
                                }
                            }
                            else
                            {
                                //不可能
                            }
                            break;
                        }
                        else if (st1[i] >= '4' && st1[i] <= '6')
                        {
                            st2 = st2 + '5';
                            break;
                        }
                        else
                        {
                            st2 = st2 + '1';
                            break;
                        }
                    }
                    else
                    {
                        st2 = st2 + st1[i];
                    }
                }
            }
            else if (_value <= -1f)
            {
                int i = 1;
                if (st1[i] >= '4' && st1[i] <= '6')
                {
                    st2 = "-5";
                }
                else if (st1[i] >= '7' && st1[i] <= '9')//+1
                {
                    st2 = "-10";
                }
                else
                {
                    st2 = "-1";
                }
                i++;
                for (; i < st1.Length; i++)
                {
                    if (st1[i] == '.') break;
                    st2 = st2 + '0';
                }
            }
            else
            {
                st2 = "0";
            }

            return float.Parse(st2);
        }
    }
}
