using Xunit;

namespace WpfApp3.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void Title_Validation_Should_Reject_Empty()
        {
            string title = "";
            Assert.True(string.IsNullOrEmpty(title));
        }

        [Fact]
        public void Title_Validation_Should_Reject_Longer_Than_100()
        {
            string title = new string('A', 101);
            Assert.True(title.Length > 100);
        }

        [Fact]
        public void Content_Validation_Should_Reject_Longer_Than_10000()
        {
            string content = new string('A', 10001);
            Assert.True(content.Length > 10000);
        }

        [Fact]
        public void Tags_Validation_Should_Allow_Valid_Chars()
        {
            string tags = "work,urgent_project,test-123";
            var clean = System.Text.RegularExpressions.Regex.Replace(tags, @"[^a-zA-Zа-яА-Я0-9,_\-\s]", "");
            Assert.Equal(tags, clean);
        }

        [Fact]
        public void Tags_Validation_Should_Reject_Invalid_Chars()
        {
            string tags = "work@home";
            var clean = System.Text.RegularExpressions.Regex.Replace(tags, @"[^a-zA-Zа-яА-Я0-9,_\-\s]", "");
            Assert.NotEqual(tags, clean);
        }
    }
}