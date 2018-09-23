using System;
using UnityEngine;

[Serializable]
public class SerializableEnum<T> where T : struct, IConvertible
{
    public T Value
    {
        get { 
            m_EnumValue = (T)Enum.Parse(m_EnumValue.GetType(), m_EnumValueAsString);
            return m_EnumValue; 
        }
        set { m_EnumValue = value; m_EnumValueAsString = m_EnumValue.ToString();}
    }

    [SerializeField]
    private string m_EnumValueAsString;
    [SerializeField]
    private T m_EnumValue;
}