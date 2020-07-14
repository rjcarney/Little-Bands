**Little Bands Version 1.0**

**Purpose**

This project is for the class COSC 481w software engineering. Little Bands is a software developed by Justin Reed, Michael O&#39;Connor, Ryan Carney, Jayse Benson, and Harris Larson. Built for Josh Grekin for the school that he runs in Ann Arbor. With the goal being to get an app that helps kids learn how to be recording artists.

The goal of the project is to get a functional prototype working. Our definition of working is having a functioning user interface, being able to pick a song, being able to choose a student instrument to record, and then have that recording remembered between play sessions.

For the project we decided on using the unity engine. Our decision came from many factors. The first is that some people in the group were familiar with the software. Secondly it&#39;s free to use and develop, and unity won&#39;t charge you unless the product you make with them makes at least 100k. The last reason was how easy it is to port to different platforms. With just a click of a button you can export to web, windows, and ios.

**Meetings with client/ Sprint plans**

Throughout this process we have been in constant contact with the client we&#39;re building this product for, Josh. Over the semester we have had two in person meetings at the school he runs. We met after each Sprint, and these meetings shaped our sprint processes.

Our first sprint cycle was a lot less organized then it could have been, due to the fact we were all new to the process and we were also getting used to using github and unity. Our goals for the first sprint was to develop a functioning user interface, and to be able to record. Which we did complete on time. Our first sprint was a success, and we met with Josh in person shortly after.

Our first meeting happened in February after sprint one. Our main topic of the meeting was talking about user interface design. Josh asked for a hand full of changes. The changes he asked for included: the instrument selection page should be removed, instrument buttons should select the instrument on the first click. Every subsequent click will toggle the instrument&#39;s associated audio clip between original audio, recorded audio and muted. Sheet music should appear while recording. The Play button always plays the user&#39;s progress.

This shaped our sprint number two. Our goal for sprint two was to incorporate The changes we were asked to implement, and to mix and store our audio. With storing our audio we were split between using a persistent data path, or a built in unity function known as serialization. The persistent data path proved to be a superior data storage method, so that&#39;s what the group decided to go with. By the end of sprint two we were able to save and hold data, with UI implementation.

We again met up with Josh after sprint two was complete. Just like the end of sprint one Josh was impressed with what he saw. He had a few more ideas of things we should change on our UI. The changes he recommended included removing the instrument buttons from the recording view and the tracks should be defaulted to the student&#39;s recorded tracks. He also wanted to add a metronome functionality. These changes drove our third sprint.

We are currently nearing the end of phase three. Phase three has been drastically different due to the COVID-19 pandemic. We as a team haven&#39;t met up since mid-march, though we are making great progress on finishing up the project. Our current goal for phase three is to finalize the UI and we are constantly testing and pusinging updates to the github.

**Teacher Manual**

The teacher view of the app is we want them to be able to upload new songs, and keep an eye on students&#39; progress. Though for simplicity of the project and the massive time restraint of the semester we were not able to focus on this aspect, nor is it part of our completion criteria.

**Student Manual**

The student will start the program by choosing an avatar. Once the avatar has been selected the user may choose a song to learn. The next thing they need to do is select an instrument that they will be recording. They can choose to look at sheet music, watch a video tutorial of how to play it, listen to the song, or play along and record. The student may also choose to save the recording.

After the student chooses the song he or she wishes to record they can choose the instrumentation of the play back track. By default the instruments will all be set to the teacher recordings though they can tap an instrument to select the instrument or tap again to mute it. Once the students record a track, in the playback screen their recording becomes the default though on a second tap it goes back to the teacher recording and on the third tap will mute the track.

**System Documentation**

![](RackMultipart20200714-4-ep47l0_html_e1a702d3edc25dda.png)

This is a user diagram Created by Justin Reed. This is to demonstrate how this app is supposed to be used

**Platforms Little Bands works on**

Using the unity engine Little Bands version 1.0 can run on almost any major platforms on the market today. Though currently exporting to IOS is a little difficult due to some of the add ons we are using, like the code we use ro record and mix the audio.

**User Stories**

For this project we stuck to 11 user stories.

1. As a student, I want to be able to use a microphone and headphones such that I can listen to and record audio.

2. As a teacher for the music app I want my students to be able to complete their music without any app errors such that they can efficiently produce music without frustration.

3. As a teacher, I want the layout of the app to be intuitive, such that it is easy to use for an 8-year old student.

4. As a student, I want to learn how to play a song using the piano, the guitar, the bass, the drums, and my voice, such that I am my own band.

5. As a student, I want to listen to tracks and see sheet music, such that I can remind myself about how to play those tracks.

6. As a student, I want to record tracks and replace tracks, such that I can hear myself playing every instrument in the song.

7. As a teacher, I want the app to be able to teach at least 1 complete song, such that the student can learn and play it on their own.

8. As a music student, I want to have a recording of myself playing a song such that I can share it with my teachers and family.

9. As a teacher, I want to use recording technology in my curriculum such that I am able to teach my class how to be recording artists as well as musicians.

10. As a music student, I want to hear my teacher playing a song along with me while I am recording music outside of the classroom such that I am guided in the process.

11. As a music student, I want to use a menu to choose which songs and instruments I wish to work on first such that I am learning on my own.

**Future Work**

The biggest thing that needs to be finished up is on the teacher side of the app. We have no current plans to get this up and running for this version, so our current documentation is key to support this potential update. If it were to be implemented, we would need a server to send student data to so that a teacher may access students progress anywhere. This would also be useful so a teacher can upload a song and then it would be on all the students&#39; devices.

Another potential future task for the project is to get other songs up and running on the app. Part of our completion criteria is having just one song working. Though for the time/resources we had, we are proud of the progress we have made.