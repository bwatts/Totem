Totem is a set of development techniques lovingly prepared in C# and spiced with the experience of coding for those who code. Software is exploding with cross-pollination of ideas, lending new eyes to old problems. This is an attempt to catch some of that lightning in a bottle.

# Introduction

Totem has been brewing for a long time, in various [projects](https://github.com/bwatts/Grasp), [utility libraries](https://github.com/bwatts/Cloak), [gists](https://gist.github.com/bwatts/762afe2f884cb787044b), and other places you might find attempts to smooth the development experience. The concepts are battle-hardened and ready to move beyond the proving grounds.

# Documentation

Head over to the [wiki](https://github.com/bwatts/Totem/wiki) to learn more about Totem and its nifty tricks.

Some highlights:

* [Text](https://github.com/bwatts/Totem/wiki/Text): A unified medium for working with strings
* [Expectations](https://github.com/bwatts/Totem/wiki/Expectations): State exactly what is expected of runtime values
* [Tagging](https://github.com/bwatts/Totem/wiki/Tagging): Attach structured metadata to objects

# Community

At the moment, I ([Bryan](https://github.com/bwatts)) am the founder and sole contributor. I am always looking for like-minded individuals who share my enthusiasm for collaboration.

I can usually be found in the [DDD/CQRS/ES](https://jabbr.net/#/rooms/DDD-CQRS-ES) room on [JabbR](https://jabbr.net/) and am always up for a discussion.

I am also [@deftcode](https://twitter.com/deftcode) on Twitter.

### License

Totem is under the [MIT License](http://www.opensource.org/licenses/MIT) - like knowledge, it wants to be free.

# Influences

Totem draws influence from a wide range of knowledge work concepts. The dynamics of [collaboration](https://twitter.com/deftcode/status/522233594186829824), [decisions](https://twitter.com/deftcode/status/496708216034963456), [time](https://twitter.com/deftcode/status/503890347848916993), and [intent](http://www.executableintent.com/about/) form the core of its worldview.

### Tactical design

Knowledge work is a rich landscape of problems, ideas, and perspectives. These implementation paradigms (and more) express solutions and play important roles in Totem:

| Style | Primary focus
| ----- | -------------
| [Object-oriented](http://en.wikipedia.org/wiki/Object-oriented_programming) | Pair data with the code using it
| [Functional](http://en.wikipedia.org/wiki/Functional_programming) | Separate data from the code using it
| [Static](http://en.wikipedia.org/wiki/Type_system#Static_type-checking) | Resolve symbols at [compile time](http://en.wikipedia.org/wiki/Compile_time)
| [Dynamic](http://en.wikipedia.org/wiki/Type_system#Dynamic_type-checking_and_runtime_type_information) | Resolve symbols at [run time](http://en.wikipedia.org/wiki/Run_time_%28program_lifecycle_phase%29)
| [Reactive](http://en.wikipedia.org/wiki/Reactive_programming) | Resolve program flow at [run time](http://en.wikipedia.org/wiki/Run_time_%28program_lifecycle_phase%29)
| [Asynchronous](http://en.wikipedia.org/wiki/Asynchrony) | Separate work from workers across time

### Strategic design

Deciding what work _to do_ is an art all its own; much effort lies at the feet of this foe. These modeling paradigms (and more) help understand where and how to apply tactical design:

| Approach | Meaning | Primary focus
| -------- | ------- | -------------
| [DDD](http://en.wikipedia.org/wiki/Domain-driven_design) | Domain-Driven Design | Model knowledge work as a series of decisions: the data informing them and the data they yield
| [CQRS](http://martinfowler.com/bliki/CQRS.html) | Command/Query Responsibility Segregation | Model writes and reads independently
| [ES](http://martinfowler.com/eaaDev/EventSourcing.html) | Event Sourcing | Model the domain as a timeline
