using ClassLibrary;
using HackerNewsScraper;
using System;
using Xunit;


namespace XUnitTestProject
{
    public class UnitTest1
    { 
        [Fact]
        public void AreThreeIdsReturned_True()
        {
            string[] y = { "3" };
            Program.Main(y);
            Assert.Equal(3, Program.ListOfIds.Length);
        }

        [Fact]
        public void Are100IdsReturned_True()
        {
            string[] y = { "100" };
            Program.Main(y);
            Assert.Equal(100, Program.ListOfIds.Length);
        }

        [Fact]
        public void ValidNumberInputRuns_True()
        {
            string[] y = { "4" };
            Program.Main(y);
            Assert.Equal(4, Program.ListOfIds.Length);
        }

        [Fact]
        public void InvalidStringInputThrowsException_True()
        {
            string[] y = { "STRING NOT INT" };
            try
            {
                Program.Main(y);
            }
            catch (Exception ex)
            {
                Assert.Equal("Input string was not in a correct format.", ex.Message.ToString());
            }

        }

        [Fact]
        public void CheckValidRootObjects_True()
        {
            RootObject x = new RootObject()
            {
                by = "Louis",
                descendants = 100,
                id = 200,
                score = 111,
                title = "How to write an article",
                url = "https://watabou.itch.io/medieval-fantasy-city-generator"
            };

            Program.ValidateAndArrangeArticleObject(x);
            Assert.Equal("Louis", Program.HackerNews.author);
        }

        //Check for invalid items
        [Fact]
        public void CheckInvalidUsernameRootObjects_True()
        {
            RootObject x = new RootObject()
            {
                //empty username
                by = "",
                descendants = 100,
                id = 200,
                score = 111,
                title = "How to write an article",
                url = "https://watabou.itch.io/medieval-fantasy-city-generator"
            };

            Program.ValidateAndArrangeArticleObject(x);
            Assert.Null(Program.HackerNews);
        }

        [Fact]
        public void CheckInvalidTitleRootObjects_True()
        {
            RootObject x = new RootObject()
            {
                by = "Louis",
                descendants = 100,
                id = 200,
                score = 111,
                //Too long title 257 characters
                title = "Lorem ipsum 1dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis,.",
                url = "http://www.test.co"
            };

            Program.ValidateAndArrangeArticleObject(x);
            Assert.Null(Program.HackerNews);
        }

        [Fact]
        //Using Uri.IsWellFormedUriString(jsonObject.url, UriKind.Absolute)
        public void CheckInvalidURIRootObjects_True()
        {
            RootObject x = new RootObject()
            {
                by = "Louis",
                descendants = 100,
                id = 200,
                score = 111,
                title = "Normal length title",
                url = "http://www.test.co\\m/ asdasd asd asd"
            };

            Program.ValidateAndArrangeArticleObject(x);
            Assert.Null(Program.HackerNews);
        }

        [Fact]
        public void CheckInvalidCommentsMinusValueRootObjects_True()
        {
            RootObject x = new RootObject()
            {
                by = "Louis",
                descendants = -1,
                id = 200,
                score = 111,
                title = "How to write an article",
                url = "https://watabou.itch.io/medieval-fantasy-city-generator"
            };

            Program.ValidateAndArrangeArticleObject(x);
            Assert.Null(Program.HackerNews);
        }

        [Fact]
        public void CheckInvalidPointsMinusValueRootObjects_True()
        {
            RootObject x = new RootObject()
            {
                by = "Louis",
                descendants = 100,
                id = 200,
                score = -100,
                title = "How to write an article",
                url = "https://watabou.itch.io/medieval-fantasy-city-generator"
            };

            Program.ValidateAndArrangeArticleObject(x);
            Assert.Null(Program.HackerNews);
        }







    }
}
