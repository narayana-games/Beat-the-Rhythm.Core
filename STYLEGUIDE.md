# Beat the Rhythm - Core: Styleguide

This styleguide documents how code is written in Beat-the-Rhythm.Core. 
Most of the C# coding style is written down in the [EditorConfig](https://editorconfig.org/)
definitions, in a format that is automatically used by Visual Studio and JetBrains Rider (and
probably other IDEs as well). We do use compact indentation style, so opening curly braces 
are in the same line as the statement that begins the block, not in the next line.

More importantly, this styleguide also documents a few quirks that may need a bit of
explanation (so this basically documents the rationale behind certain unusual decisions
that may require explanation):

## Being Inside or Outside Unity

Many of the classes in `Package-Maps/com.narayana-games.btr.maps` are used both as data
structures within Unity projects as well as in server-side Web apps. In the latter, they
are used to store data persistently in a MongoDB.

To conveniently distinguish those two cases for the purpose of conditional compilation,
we use one symbol that we know will always be available in Unity but not outside Unity.
As we'll never use a version of Unity older than 2017.4 (we're currently on 2019.4),
we simply use `UNITY_2017_4_OR_NEWER`. So, one construct that you'll see quite often
is:

    [Serializable]
    public class MapContainer {
    #if !UNITY_2017_4_OR_NEWER // used to see if we are OUTSIDE Unity
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public MongoDB.Bson.ObjectId Id { get; set; }
    #endif
        ...
    }
