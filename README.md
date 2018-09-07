# HackerNewsScraper

In order to make it more fun to read for robots (and easier to integrate in our workflow) we would like to write a simple command line application that would output to STDOUT the top posts in JSON.

## Example/Instructions

```
  HackerNewsScraper.exe "numberOfPosts"  
```
Where number of posts is an integer stating how many posts you require from 1-100. This will then output the result to the console in this format:

```
{
  "title": "Medieval Fantasy City Generator",
  "uri": "https://watabou.itch.io/medieval-fantasy-city-generator",
  "author": "BerislavLopac",
  "points": 454,
  "comments": 41,
  "rank": 2
}
```
## Running the tests

There are some units tests which exist in the XUnitTestProject. These check for some tests such as having invalid inputs as well as receiving invalid data from the api. 
