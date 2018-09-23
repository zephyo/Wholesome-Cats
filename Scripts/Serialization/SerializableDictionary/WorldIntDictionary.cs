using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.Serialization;
using System.Linq;

[Serializable]
public class WorldIntDictionary : SerializableDictionary<WorldType, WorldLevel>
{

    public WorldIntDictionary() : base() { }
    public WorldIntDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

}
[Serializable]
public class WorldLevel
{
    //levels tried
    public int hasPlayed;
    //levels completed
    public int level;
    //array; index 0 = daily plays of level 0
    public ushort[] starRating;
    public uint[] dailyPlays;
    public WorldLevel(int level, int played, uint[] dailyPlays, ushort[] starRating)
    {
        this.level = level;
        this.hasPlayed = played;
        this.dailyPlays = dailyPlays;
        this.starRating = starRating;
    }
    public override bool Equals(object obj)
    {
        if (!(obj is WorldLevel))
            return false;

        WorldLevel mys = (WorldLevel)obj;
        // compare elements here
        return this.hasPlayed == mys.hasPlayed &&
        this.level == mys.level &&
        this.starRating.SequenceEqual(mys.starRating) &&
        this.dailyPlays.SequenceEqual(mys.dailyPlays);
    }


}

// [Serializable]
// public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> {}

// [Serializable]
// public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> {}

// [Serializable]
// public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> {}

// [Serializable]
// public class MyClass
// {
//     public int i;
//     public string str;
// }

// [Serializable]
// public class QuaternionMyClassDictionary : SerializableDictionary<Quaternion, MyClass> {}