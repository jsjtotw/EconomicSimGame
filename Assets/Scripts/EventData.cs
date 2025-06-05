using System;
using UnityEngine;

[Serializable]
public class EventData
{
    public string id, title, description, effect;
    public float minValue, maxValue;
    public string industry, company;

    // This is *not* loaded from JSON — it's set at runtime
    [NonSerialized]
    public float value;

    public override string ToString()
    {
        return $"Event: {title} ({effect})";
    }
}
