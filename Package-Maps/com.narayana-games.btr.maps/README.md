TODO:

- Rename _Access_ to _Permissions_
- Rename _NoteEvent_ to _RhythmicEvent_

# Universal Music Mapping Format (UMMF or Oomph!)

UMMF (pronounce: _oomph_) is the native beatmap format for the 
VR rhythm games Holodance, Beat the Rhythm, and the standalone 
beatmap editor Beatographer. It is the core of the BAM-initiative: 
_Beatmap all music!_ BAM is a project where people from all over the
world collaborate to make all music playable in the most fun ways 
in rhythm games.

## Who is this for?

We primarily open-sourced this format so that anyone who wants to
mod either our own games, or other games, or wants to build or 
consume content for rhythm games, can use a common language that
can be used universally to build rhythm gameplay, or converted
into the proprietary gameplay languages for other games. 

The format is currently used in Unity games, and stored in a MongoDB,
so that means JSON, C# and Unity. But you should be able to use 
other game engines or even implementation languages (in that case,
the classes in this repository act primarily as reference and
documentation).

## What's the idea behind it?

The design rationale behind this format is to support (almost)
any kind of music, with (almost) any kind of rhythm game mechanic.
The primary focus, naturally, are Virtual Reality rhythm games. But
the concepts translate very well to Augmented Reality rhythm games,
and to a certain degree even to 2D rhythm games.

This format is workflow-oriented and designed to be able to re-use
as much of the authoring as possible when optimizing maps for
different rhythm game mechanics, games, or even platforms (in 
the sense of VR, AR, flatscreen and mobile being the four
major platforms, and flatscreen being both, desktop computers
and consoles).

So this format is also designed to facilitate collaboration
and "modding": For any given song (or more precisely: _audio
recording_), one person, possibly even one of the original artists,
could begin by providing the musical _song structure_. The 
_song structure_ is how the song is built of sections, like
intro, breakdown, buildup, drop, or intro, verse, chorus, solo.
The _song structure_ also includes the tempo of the song, or 
even tempo changes, and the meter signature (usually 4/4 but
could also be 3/4 or something esoteric like 7/8).

Another person could then add interesting rhythmic patterns for each
section, and yet another person could turn these rhythmic patterns into 
fun gameplay at a specific difficulty by assigning each _rhythmic event_ a 
location and information how that _rhythmic event_ needs to be handled
by the player (e.g. catch, punch or slice a drop, or shoot a drone).
And yet another person might adapt the gameplay for another difficulty,
or even another mechanic or game.

Finally, the format is designed to be easy to be shared by programs,
in particular, servers that host the maps, and clients producing or
consuming the maps, which also takes us to the next section:

## Where do I go from here?

Have a look at `MapContainer.cs`, which can store any part of a given
map. This could be one simple song, with structure and one map,
but it could also be just a generic building block with one section
and a single sequence; or it could include all maps for all
game mechanics for a given audio recording. This approach simplifies 
the API/interfaces that clients use to interact with the data storage server.

Most importantly, this has a `SongStructure.cs`, which mainly consists of
a list of `Section.cs` but also has meta data like the song title or 
artist name. Sections can be further divided into `Phrase.cs` (both 
implement the abstract class `SongSegment.cs`, because in some cases,
sections and phrases can be used interchangeably).

The actual rhythmic patterns are stored in a list of `Track.cs`. Each
track is for the whole length of the song but could represent different
difficulties, different game mechanics, or different gameplay for
different players in collaborative (or competitive) multiplayer.

Each track has a list of `Sequence.cs`, with each element in that
list providing the rhythmic data for a specific phrase from the
song structure (in many cases, sections consist of a single phrase
but we need to cover the more complex case of phrases changing 
tempo and/or meter signature, so the mapping is sequence-phrase,
not sequence-section).

