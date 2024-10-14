using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class JAnimation_AutoSetBoneMatchData : ScriptableObject
{
    [System.Serializable]
    public class OneData
    {
        public JAnimationData.BoneType boneType = JAnimationData.BoneType.unknowType;
        public List<string> keyWords = new List<string>();

        public OneData(JAnimationData.BoneType _boneType)
        {
            boneType = _boneType;
            keyWords = new List<string>();
        }
        public OneData(JAnimationData.BoneType _boneType, List<string> _keyWords)
        {
            boneType = _boneType;
            keyWords = _keyWords;
        }
    }

    public List<OneData> DataList = new List<OneData>();

    public JAnimationData.BoneType GetBoneType(string _text)
    {
        JAnimationData.BoneType matchedType = JAnimationData.BoneType.unknowType;
        string lastKey = "";
        for (int i = DataList.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < DataList[i].keyWords.Count; j++)
            {
                if (_text.Contains(DataList[i].keyWords[j]) == true)
                {
                    if(matchedType == JAnimationData.BoneType.unknowType)
                    {
                        matchedType = DataList[i].boneType;
                        lastKey = DataList[i].keyWords[j];
                    }
                    else
                    { 
                        if(DataList[i].keyWords[j].Length > lastKey.Length)
                        {
                            matchedType = DataList[i].boneType;
                        }
                    } 
                }
            }
        }
        return matchedType;
    }

    public List<string> GetKeyWords(JAnimationData.BoneType _boneType)
    {
        for (int i = 0; i < DataList.Count; i++)
        {
            
            //Debug.Log("DataList.count: " + DataList.Count);
            if (DataList[i].boneType == _boneType)
            {
                
                List<string> newKeywords = new List<string>();//use a new list.
                for(int k=0; k< DataList[i].keyWords.Count; k++)
                {
                    newKeywords.Add(DataList[i].keyWords[k]);
                    //Debug.Log("keywords" + i + " " + k + "name: " + DataList[i].keyWords[k]);
                }
                return newKeywords;
            }
        }
        return new List<string>();
    }

    public bool CanTextMatchOneKeyWord(string _text, List<string> _keyWords)
    {
        for (int i = 0; i < _keyWords.Count; i++)
        {
            if(_text.Length < _keyWords[i].Length)
            {
                continue;
            }
            if (_text.Contains(_keyWords[i]))
            {
                return true;
            }
        }
        return false;
    }

    private int CompareKeyWords(List<string> keyWordsX, List<string> keyWordsY)
    {
        for (int i = 0; i < keyWordsX.Count; i++)
        {
            for (int j = 0; j < keyWordsY.Count; j++)
            {
                if (keyWordsX[i].Contains(keyWordsY[j]) == true)
                {
                    return 1;
                }
                else if (keyWordsY[j].Contains(keyWordsX[i]) == true)
                {
                    return -1;
                }else
                {
                    int C_String = CompareString(keyWordsX[i], keyWordsY[j]);
                    if (C_String != 0)
                    {
                        return C_String;
                    }
                }
            }
        }
        return 0;
    }

    public void CompleteTheLowerAndUpperKeyWords()
    {
        for (int i = 0; i < DataList.Count; i++)
        {
            for (int j = 0; j < DataList[i].keyWords.Count; j++)
            {
                string newString = DataList[i].keyWords[j];
                if(string.IsNullOrEmpty(newString) == false)
                {
                    char char0 = newString[0] ;
                    char[] chars = newString.ToCharArray();
                    chars[0] = char.ToLower(char0);
                    string newLowerString = new string(chars);
                    if (DataList[i].keyWords.Contains(newLowerString) == false)
                        DataList[i].keyWords.Add(newLowerString);
                    chars[0] = char.ToUpper(char0);
                    string newUpperString = new string(chars);
                    if (DataList[i].keyWords.Contains(newUpperString) == false)
                        DataList[i].keyWords.Add(newUpperString);
                }
            }

        }
    }
    public void CompleteTheLowerAndUpperKeyWords_TypeName()
    {
        for (int i = 0; i < DataList.Count; i++)
        {
            string newString = DataList[i].boneType.ToString();
            if (string.IsNullOrEmpty(newString) == false)
            {
                char char0 = newString[0];
                char[] chars = newString.ToCharArray();
                chars[0] = char.ToLower(char0);
                string newLowerString = new string(chars);
                if (DataList[i].keyWords.Contains(newLowerString) == false)
                    DataList[i].keyWords.Add(newLowerString);
                chars[0] = char.ToUpper(char0);
                string newUpperString = new string(chars);
                if (DataList[i].keyWords.Contains(newUpperString) == false)
                    DataList[i].keyWords.Add(newUpperString);
            }

        }
    }
    public int CompareString(string x, string y)
    {
        int L = y.Length;
        if (y.Length < x.Length)
        {
            L = x.Length;
        }
      
        int i_x = 0, i_y = 0;

        for (int i = 0; i < L; i++)
        {
            if (i < x.Length) i_x = i;
            if (i < y.Length) i_y = i;
            if (x[i_x] > y[i_y]) return 1;
            if (x[i_x] < y[i_y]) return -1;
        }
        return 0;
    }

    public void SortDataList()
    {
       
        for (int i = DataList.Count - 1; i >= 0; i--)
        {
            if(DataList[i].keyWords.Count == 0)
            {
                Debug.Log("删除了一个没有成员的项目：" + DataList[i].boneType);
                DataList.RemoveAt(i); 
            }
        }
        for (int i = 0; i < DataList.Count; i++)
        {
            for(int j= DataList.Count - 1; j>i; j--)
            {
                if(DataList[j].boneType == DataList[i].boneType)
                {
                    Debug.Log("删除了一个重复的项目: " + DataList[j].boneType);
                    DataList.RemoveAt(j);
                }
            } 
        }

        for (int i = 0; i < DataList.Count; i++)
        {
            DataList[i].keyWords.Sort();
        }
        DataList.Sort(delegate (OneData x, OneData y)
        {
            return CompareKeyWords(x.keyWords, y.keyWords);
        });

        Debug.Log("Sorted!");
    }

}

