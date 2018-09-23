using System;

[Serializable]
public class CatTypeEnumClass : SerializableEnum<CatType> { }

// [Serializable]
// public class WorldTypeEnumClass : SerializableEnum<WorldType> { }
[System.Serializable]
public enum WorldType
{
    house,
    computer,
    fields,
    city,
    ocean,
    woods,
    sky,
    space
}
[System.Serializable]
public enum AttackType
{
    Melee,
    Ranged,
    None,
}
[System.Serializable]
public enum FactionType
{
    Life,

    Manmade,

    Earth
}
[System.Serializable]
public enum SecondaryType
{
    //melee
    Paw,
    Bite,
    Lick,
    //ranged
    Star,
    Nebula,
    Mouse,
    Yarn,
    Flower,
    Heart,
    Fish,
    Leaf,
    Bubbles,
    Butterfly,
    Candy,
    Gem,
    Arrow,
    Music,
    Rainbow,
    Rose,
    Snowflake,
    Sprinkles,
    Berry,
    Skull,
    Bat


}
[System.Serializable]
public enum ActionType
{
    Magic,
    Meow, //play meow sound
    Joke,
    Teach,
    Bork,
    Boop,
    Hug, //love particles
    Story,
    Comfort,//love particles
    Deal,
    Fly,
    Love,//love particles
    Feed,
    Think, // reaction - gives clue to solution
    Ghost,
    Swim,

}


[Serializable]
public enum CatType
{
    anime,
    artist,
    black,
    blep,
    blueeyes,
    bot,
    bread,
    brown,
    business,
    choco,
    cloud,
    cream,
    dog,
    donutchoc,
    donutpink,
    doot,
    eyes3,
    fire,
    gem,
    ghost,
    grass,
    grey,
    grumpy,
    head3,
    ice,
    lantern,
    music,
    mustache,
    nerd,
    niceeyes,
    night,
    old,
    orange,
    oxo,
    persian,
    phat,
    pink,
    pixel,
    plush,
    pocky,
    rainbow,
    scottish,
    shadow,
    sleepy,
    sphinx,
    sprout,
    star,
    sushi,
    tabby,
    unicorn,
    uwu,
    water,
    white,
    wood,
    none,
}






