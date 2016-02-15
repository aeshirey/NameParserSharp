namespace NameParseTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NameParser;

    [TestClass]
    public class NameParserTests
    {
        [TestMethod]
        public void JFK()
        {
            var jfk = new HumanName("president john 'jack' fitzgerald kennedy");

            Assert.AreEqual("president", jfk.Title);
            Assert.AreEqual("john", jfk.First);
            Assert.AreEqual("fitzgerald", jfk.Middle);
            Assert.AreEqual("kennedy", jfk.Last);
            Assert.AreEqual(string.Empty, jfk.Suffix);
            Assert.AreEqual("jack", jfk.Nickname);
            Assert.AreEqual("president john fitzgerald kennedy", jfk.FullName);
            Assert.AreEqual("kennedy", jfk.LastBase);
            Assert.AreEqual(string.Empty, jfk.LastPrefixes);

            jfk.Normalize();

            Assert.AreEqual("President", jfk.Title);
            Assert.AreEqual("John", jfk.First);
            Assert.AreEqual("Fitzgerald", jfk.Middle);
            Assert.AreEqual("Kennedy", jfk.Last);
            Assert.AreEqual(string.Empty, jfk.Suffix);
            Assert.AreEqual("Jack", jfk.Nickname);
            Assert.AreEqual("President John Fitzgerald Kennedy", jfk.FullName);
            Assert.AreEqual("Kennedy", jfk.LastBase);
            Assert.AreEqual(string.Empty, jfk.LastPrefixes);
        }

        [TestMethod]
        public void Nixon()
        {
            var nixon = new HumanName("mr president richard (dick) nixon");

            Assert.AreEqual("mr president", nixon.Title);
            Assert.AreEqual("richard", nixon.First);
            Assert.AreEqual(string.Empty, nixon.Middle);
            Assert.AreEqual("nixon", nixon.Last);
            Assert.AreEqual(string.Empty, nixon.Suffix);
            Assert.AreEqual("dick", nixon.Nickname);
            Assert.AreEqual("mr president richard nixon", nixon.FullName);
            Assert.AreEqual("nixon", nixon.LastBase);
            Assert.AreEqual(string.Empty, nixon.LastPrefixes);

            nixon.Normalize();

            Assert.AreEqual("Mr President", nixon.Title);
            Assert.AreEqual("Richard", nixon.First);
            Assert.AreEqual(string.Empty, nixon.Middle);
            Assert.AreEqual("Nixon", nixon.Last);
            Assert.AreEqual(string.Empty, nixon.Suffix);
            Assert.AreEqual("Dick", nixon.Nickname);
            Assert.AreEqual("Mr President Richard Nixon", nixon.FullName);
            Assert.AreEqual("Nixon", nixon.LastBase);
            Assert.AreEqual(string.Empty, nixon.LastPrefixes);
        }

        [TestMethod]
        public void TitleFirstOrLastName()
        {
            var mrJones = new HumanName("Mr. Jones");
            Assert.AreEqual("Mr.", mrJones.Title);
            Assert.AreEqual(string.Empty, mrJones.First);
            Assert.AreEqual(string.Empty, mrJones.Middle);
            Assert.AreEqual("Jones", mrJones.Last);
            Assert.AreEqual(string.Empty, mrJones.Suffix);
            Assert.AreEqual(string.Empty, mrJones.Nickname);
            Assert.AreEqual("Jones", mrJones.LastBase);
            Assert.AreEqual(string.Empty, mrJones.LastPrefixes);


            var uncleAdam = new HumanName("Uncle Adam");
            Assert.AreEqual("Uncle", uncleAdam.Title);
            Assert.AreEqual("Adam", uncleAdam.First);
            Assert.AreEqual(string.Empty, uncleAdam.Middle);
            Assert.AreEqual(string.Empty, uncleAdam.Last);
            Assert.AreEqual(string.Empty, uncleAdam.Suffix);
            Assert.AreEqual(string.Empty, uncleAdam.Nickname);
            Assert.AreEqual(string.Empty, uncleAdam.LastBase);
            Assert.AreEqual(string.Empty, uncleAdam.LastPrefixes);
        }

        [TestMethod]
        public void DifferentInputsSameValues()
        {
            var fml = new HumanName("john x smith");
            var lfm = new HumanName("smith, john x");

            Assert.IsTrue(fml == lfm);
        }

        [TestMethod]
        public void NicknameAtBeginning_DoubleQuote()
        {
            var parsed = new HumanName("\"TREY\" ROBERT HENRY BUSH III");

            Assert.AreEqual(parsed.First, "ROBERT");
            Assert.AreEqual(parsed.Middle, "HENRY");
            Assert.AreEqual(parsed.Last, "BUSH");
            Assert.AreEqual(parsed.Nickname, "TREY");
            Assert.AreEqual(parsed.Suffix, "III");
        }
        [TestMethod]

        public void NicknameAtBeginning_SingleQuote()
        {
            var parsed = new HumanName("'TREY' ROBERT HENRY BUSH III");

            Assert.AreEqual(parsed.First, "ROBERT");
            Assert.AreEqual(parsed.Middle, "HENRY");
            Assert.AreEqual(parsed.Last, "BUSH");
            Assert.AreEqual(parsed.Nickname, "TREY");
            Assert.AreEqual(parsed.Suffix, "III");
        }

        [TestMethod]
        public void LastBaseAndPrefixes()
        {
            var parsed = new HumanName("John Smith");
            Assert.AreEqual("Smith", parsed.Last);
            Assert.AreEqual(string.Empty, parsed.LastPrefixes);
            Assert.AreEqual("Smith", parsed.LastBase);

            parsed = new HumanName("johannes van der waals");
            Assert.AreEqual("johannes", parsed.First);
            Assert.AreEqual("van der", parsed.LastPrefixes); // specifically, the prefixes to the last name
            Assert.AreEqual("waals", parsed.LastBase); // only the base component of the last name
            Assert.AreEqual("van der waals", parsed.Last); // the full last name, combined
            
            parsed.Normalize();
            Assert.AreEqual("Johannes", parsed.First);
            Assert.AreEqual("van der", parsed.LastPrefixes);
            Assert.AreEqual("Waals", parsed.LastBase);
            Assert.AreEqual("van der Waals", parsed.Last);
        }
    }
}
