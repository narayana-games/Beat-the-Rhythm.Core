# Beat the Rhythm - Maps Format

## About SongSegments, Sections and Phrases

Unlike most other rhythm games, Beat the Rhythm uses the song structure as a central element
in the mapping process. While this does add a little bit of meta-authoring, it also provides
important benefits during both mapping and playing.

However, for these benefits to become available, a little bit of musical understanding is 
necessary:

### Sections

Each song consists of several **sections**. In popular music, typical sections are the verses,
chorus, bridge, often an introduction, and outro or fade out. Various genres have different
typical structures: EDM in general, or Dubstep/Gamestep in particular, usually has an
intro, breakdown, buildup and, most importantly, the drop. 

So, in a nutshell, most songs only have a few sections (roughly 5-10), usually lasting 8 
to 32 bars, and when the section changes, there is usually a noticeable change in the 
atmosphere of the song.

More specifically, the duration of a section in bars will usually be 4 or 8, but 12, 
16 or even 32 is also possible.

One thing you should keep in mind when mapping the song structure is that the primary
purpose of that song structure is to guide the gameplay mapping process. There is a 
natural flow and progression through each individual song, and getting the sections 
right will help you or others who create gameplay for that song to be in tune with
that flow.

For many songs, just mapping out the sections will be sufficient and you can safely ignore
that phrases even exist. But often, splitting sections, especially longer ones, into
phrases will still be helpful:

### Phrases

One way to understand phrases in music is to view sections as paragraphs, and phrases
as sentences. From a rhythm game perspective, phrases are best seen as comparatively
short segments, maybe two or four bars, that encapsulates one idea in terms of gameplay.
More specifically, the duration of a section in bars will usually be 4 or 8, but 1, 12, 
16 or more is also possible

As said in the section on *Sections*, it's often enough or even best to have phrases 
and sections be one and the same. But if you find noticeable changes within a section, 
or if a section has segments that repeat, splitting the section into several phrases 
can be preferable, in particular if those noticeable changes, or repetitions are 
relevant for gameplay.

One technical reason why you may need phrases, or even every short phrases with just a 
single bar, is when the tempo or time signature changes:

### Tempo

Tempo is usually given in beats per minute (BPM). In almost all popular music, one
beat is a quarter note in a bar with four quarter notes. That means that 120 BPM
has one beat every half second (60 seconds divided into 120 beats), and one bar 
lasts two seconds (four beats at half a second each is two seconds). Also, in
a lot of popular music, the tempo does not change throughout the song.

There are, however, exceptions: Using dubstep as a convenient example, drops will 
often be halftime or double time, depending on your perspective of the actual tempo 
of the song.

While Dubstep usually has 140 BPM, due to its use of halftime, the actual tempo is 
usually only 70 BPM. This is very important because we may have visualizations
that are based on the tempo, and when we say the song runs at 140 BPM but it actually
feels like 70 BPM, those will feel wrong.

Both, sections and phrases can define a tempo, and our system currently only supports
setting a new tempo at the beginning of a new bar. You can change the tempo within
a single section by adding phrases each time the tempo changes.

### Time Signatures

As said before: Almost all songs are just 4/4 - one bar consists of four quarter
notes. But there are exceptions, some songs, e.g. waltz are in 3/4, and some songs
might even change the time signature. This is handled just like tempo, so any
new phrase can also define a new time signature.

### Recommended Reading

There is a really good tutorial that outlines some of the basics of songwriting;
it's third part is about deconstructing the form of a song:
[Beginner’s Guide To Songwriting – Part 3](https://music.tutsplus.com/tutorials/beginners-guide-to-songwriting-part-3--audio-4107).

More details on the different types of sections used in traditional pop music and
modern EDM can be found in the first part of:
[Song Structure workshop](http://dsmootz.blogspot.com/p/song-structure-workshop.html).
The other three parts of that workshop are examples.

If you're looking for more examples, you'll find the song structures of several songs visualized in: 
[http://www.ethanhein.com/wp/2012/song-structures/](http://www.ethanhein.com/wp/2012/song-structures/).

