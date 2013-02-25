An implementation of the <a href="http://en.wikipedia.org/wiki/Travelling_salesman_problem">Travelling salesman problem</a> based on <a href="http://en.wikipedia.org/wiki/Genetic_algorithm">genetic algorithms</a>.

Starting with a random route, the algorithm applies mutations and permutations on the best current routes to generate new generations. By basing new routes on the previous generation's best routes, the algorithm gradually improves its results.

Algorithm progression
===========

Here's a few samples of what a progression typically looks like.

First, we start with something completely random (generation #0):<br />
<img src="http://frankkvam.com/travellingsalesman/Generation%200%20-%20total%20length%20is%2022445158459.small.png" alt="Random route" />

After a few hundred generations, we get a slightly more organized route (generation #387):<br />
<img src="http://frankkvam.com/travellingsalesman/Generation%20387%20-%20tota%20length%20is%209242054964.small.png" alt="More organized" />

After a few hundred generations more, it's even more organized (generation #1358):<br />
<img src="http://frankkvam.com/travellingsalesman/Generation%201358%20-%20total%20length%20is%205934799063.small.png" alt="Even more organized" />

The state becomes more and more stable (generation #5451):<br />
<img src="http://frankkvam.com/travellingsalesman/Generation%205451%20-%20total%20length%20is%203416072266.small.png" alt="Slowing down" />

Eventually, things will enter an equilibrium, where the route no longer improves (generation #23801):<br />
<img src="http://frankkvam.com/travellingsalesman/Generation%2023801%20-%20total%20length%20is%202044366888.small.png" alt="Equilibrium" />

Perhaps not a great solution on its own, but combining this technique with traditional approaches can improve the results further.
