using Microsoft.VisualStudio.TestTools.UnitTesting;
using NameParser;
using System;

namespace NameParserTest
{
    [TestClass]
    public class NameParserTests
    {
        [TestMethod]
        public void NullInput()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new HumanName(null));
        }

        [TestMethod]
        public void BlankInput()
        {
            var parsed = new HumanName(string.Empty);
            Assert.IsEmpty(parsed.First);
            Assert.IsEmpty(parsed.Middle);
            Assert.IsEmpty(parsed.Last);
            Assert.IsEmpty(parsed.Title);
            Assert.IsEmpty(parsed.Nickname);
            Assert.IsEmpty(parsed.Suffix);
        }

        [TestMethod]
        public void Jfk()
        {
            var jfk = new HumanName("president john 'jack' fitzgerald kennedy");

            Assert.AreEqual("president", jfk.Title);
            Assert.AreEqual("john", jfk.First);
            Assert.AreEqual("fitzgerald", jfk.Middle);
            Assert.AreEqual("kennedy", jfk.Last);
            Assert.IsEmpty(jfk.Suffix);
            Assert.AreEqual("jack", jfk.Nickname);
            Assert.AreEqual("president john fitzgerald kennedy", jfk.FullName);
            Assert.AreEqual("kennedy", jfk.LastBase);
            Assert.IsEmpty(jfk.LastPrefixes);

            jfk.Normalize();

            Assert.AreEqual("President", jfk.Title);
            Assert.AreEqual("John", jfk.First);
            Assert.AreEqual("Fitzgerald", jfk.Middle);
            Assert.AreEqual("Kennedy", jfk.Last);
            Assert.IsEmpty(jfk.Suffix);
            Assert.AreEqual("Jack", jfk.Nickname);
            Assert.AreEqual("President John Fitzgerald Kennedy", jfk.FullName);
            Assert.AreEqual("Kennedy", jfk.LastBase);
            Assert.IsEmpty(jfk.LastPrefixes);
        }

        [TestMethod]
        public void Nixon()
        {
            var nixon = new HumanName("mr president richard (dick) nixon");

            Assert.AreEqual("mr president", nixon.Title);
            Assert.AreEqual("richard", nixon.First);
            Assert.IsEmpty(nixon.Middle);
            Assert.AreEqual("nixon", nixon.Last);
            Assert.IsEmpty(nixon.Suffix);
            Assert.AreEqual("dick", nixon.Nickname);
            Assert.AreEqual("mr president richard nixon", nixon.FullName);
            Assert.AreEqual("nixon", nixon.LastBase);
            Assert.IsEmpty(nixon.LastPrefixes);

            nixon.Normalize();

            Assert.AreEqual("Mr President", nixon.Title);
            Assert.AreEqual("Richard", nixon.First);
            Assert.IsEmpty(nixon.Middle);
            Assert.AreEqual("Nixon", nixon.Last);
            Assert.IsEmpty(nixon.Suffix);
            Assert.AreEqual("Dick", nixon.Nickname);
            Assert.AreEqual("Mr President Richard Nixon", nixon.FullName);
            Assert.AreEqual("Nixon", nixon.LastBase);
            Assert.IsEmpty(nixon.LastPrefixes);
        }

        [TestMethod]
        public void TitleFirstOrLastName()
        {
            var mrJones = new HumanName("Mr. Jones");
            Assert.AreEqual("Mr.", mrJones.Title);
            Assert.IsEmpty(mrJones.First);
            Assert.IsEmpty(mrJones.Middle);
            Assert.AreEqual("Jones", mrJones.Last);
            Assert.IsEmpty(mrJones.Suffix);
            Assert.IsEmpty(mrJones.Nickname);
            Assert.AreEqual("Jones", mrJones.LastBase);
            Assert.IsEmpty(mrJones.LastPrefixes);

            var uncleAdam = new HumanName("Uncle Adam");
            Assert.AreEqual("Uncle", uncleAdam.Title);
            Assert.AreEqual("Adam", uncleAdam.First);
            Assert.IsEmpty(uncleAdam.Middle);
            Assert.IsEmpty(uncleAdam.Last);
            Assert.IsEmpty(uncleAdam.Suffix);
            Assert.IsEmpty(uncleAdam.Nickname);
            Assert.IsEmpty(uncleAdam.LastBase);
            Assert.IsEmpty(uncleAdam.LastPrefixes);
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

            Assert.AreEqual("ROBERT", parsed.First);
            Assert.AreEqual("HENRY", parsed.Middle);
            Assert.AreEqual("BUSH", parsed.Last);
            Assert.AreEqual("TREY", parsed.Nickname);
            Assert.AreEqual("III", parsed.Suffix);
        }

        [TestMethod]
        public void NicknameAtBeginning_SingleQuote()
        {
            var parsed = new HumanName("'TREY' ROBERT HENRY BUSH III");

            Assert.AreEqual("ROBERT", parsed.First);
            Assert.AreEqual("HENRY", parsed.Middle);
            Assert.AreEqual("BUSH", parsed.Last);
            Assert.AreEqual("TREY", parsed.Nickname);
            Assert.AreEqual("III", parsed.Suffix);
        }

        [TestMethod]
        public void LastBaseAndPrefixes()
        {
            var parsed = new HumanName("John Smith");
            Assert.AreEqual("Smith", parsed.Last);
            Assert.IsEmpty(parsed.LastPrefixes);
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

        [TestMethod]
        public void TwoNames_MacArthur()
        {
            HumanName.ParseMultipleNames = true;
            var parsed = new HumanName("John D. and Catherine T. MacArthur");

            Assert.AreEqual("John", parsed.First);
            Assert.AreEqual("D.", parsed.Middle);
            Assert.AreEqual("MacArthur", parsed.Last);

            Assert.IsNotNull(parsed.AdditionalName);

            Assert.AreEqual("Catherine", parsed.AdditionalName.First);
            Assert.AreEqual("T.", parsed.AdditionalName.Middle);
            Assert.AreEqual("MacArthur", parsed.AdditionalName.Last);

            Assert.IsNull(parsed.AdditionalName.AdditionalName);

            parsed = new HumanName("John D. & Catherine T. MacArthur");

            Assert.AreEqual("John", parsed.First);
            Assert.AreEqual("D.", parsed.Middle);
            Assert.AreEqual("MacArthur", parsed.Last);

            Assert.IsNotNull(parsed.AdditionalName);

            Assert.AreEqual("Catherine", parsed.AdditionalName.First);
            Assert.AreEqual("T.", parsed.AdditionalName.Middle);
            Assert.AreEqual("MacArthur", parsed.AdditionalName.Last);

            Assert.IsNull(parsed.AdditionalName.AdditionalName);
        }

        [TestMethod]
        public void TwoNames_TitleFirstInitialLast()
        {
            HumanName.ParseMultipleNames = true;
            var parsed = new HumanName("Mr S Bloggs and Miss L Jones");

            Assert.AreEqual("Mr", parsed.Title);
            Assert.AreEqual("S", parsed.First);
            Assert.IsEmpty(parsed.Middle);
            Assert.AreEqual("Bloggs", parsed.Last);

            Assert.IsNotNull(parsed.AdditionalName);

            Assert.AreEqual("Miss", parsed.AdditionalName.Title);
            Assert.AreEqual("L", parsed.AdditionalName.First);
            Assert.IsEmpty(parsed.AdditionalName.Middle);
            Assert.AreEqual("Jones", parsed.AdditionalName.Last);

            Assert.IsNull(parsed.AdditionalName.AdditionalName);
        }

        [TestMethod]
        public void TwoNames_TitleFirstInitialMiddleInitialLast()
        {
            HumanName.ParseMultipleNames = true;
            var parsed = new HumanName("Mr S R Bloggs and Miss L B Jones");

            Assert.AreEqual("Mr", parsed.Title);
            Assert.AreEqual("S", parsed.First);
            Assert.AreEqual("R", parsed.Middle);
            Assert.AreEqual("Bloggs", parsed.Last);

            Assert.IsNotNull(parsed.AdditionalName);

            Assert.AreEqual("Miss", parsed.AdditionalName.Title);
            Assert.AreEqual("L", parsed.AdditionalName.First);
            Assert.AreEqual("B", parsed.AdditionalName.Middle);
            Assert.AreEqual("Jones", parsed.AdditionalName.Last);

            Assert.IsNull(parsed.AdditionalName.AdditionalName);
        }

        [TestMethod]
        public void ThreeNames()
        {
            HumanName.ParseMultipleNames = true;
            var johnSmith = new HumanName("Mr John Smith and Mrs Jane Doe and President Abraham Lincoln");

            Assert.IsNotNull(johnSmith.AdditionalName);
            var janeDoe = johnSmith.AdditionalName;

            Assert.IsNotNull(janeDoe.AdditionalName);
            var abrahamLincoln = janeDoe.AdditionalName;

            Assert.AreEqual("Mr", johnSmith.Title);
            Assert.AreEqual("John", johnSmith.First);
            Assert.AreEqual("Smith", johnSmith.Last);

            Assert.AreEqual("Mrs", janeDoe.Title);
            Assert.AreEqual("Jane", janeDoe.First);
            Assert.AreEqual("Doe", janeDoe.Last);

            Assert.AreEqual("President", abrahamLincoln.Title);
            Assert.AreEqual("Abraham", abrahamLincoln.First);
            Assert.AreEqual("Lincoln", abrahamLincoln.Last);
        }

        [TestMethod]
        // https://github.com/aeshirey/NameParserSharp/issues/8
        public void Parens()
        {
            var johnSmith = new HumanName("(John Smith)");
            Assert.IsEmpty(johnSmith.First);
            Assert.IsEmpty(johnSmith.Last);
            Assert.AreEqual("John Smith", johnSmith.Nickname);
        }

        [TestMethod]
        public void FirstMiddleLastSuffix_NoCommas()
        {
            var john = new HumanName("John Quincy Smith III");
            Assert.AreEqual("John", john.First);
            Assert.AreEqual("Quincy", john.Middle);
            Assert.AreEqual("Smith", john.Last);
            Assert.AreEqual("III", john.Suffix);

            var robert = new HumanName("Robert Lee Elder III");
            Assert.AreEqual("Robert", robert.First);
            Assert.AreEqual("Lee", robert.Middle);
            Assert.AreEqual("Elder", robert.Last);
            Assert.AreEqual("III", robert.Suffix);
        }

        [TestMethod]
        public void TwoCommaWithMiddleName()
        {
            var parsed = new HumanName("Surname, John Middle, III");

            Assert.AreEqual("John", parsed.First);
            Assert.AreEqual("Middle", parsed.Middle);
            Assert.AreEqual("Surname", parsed.Last);
            Assert.AreEqual("III", parsed.Suffix);
        }

        [TestMethod]
        public void FirstLastPrefixesLastSuffix_NoCommas()
        {
            var valeriano = new HumanName("Valeriano De Leon JR.");

            Assert.AreEqual("Valeriano", valeriano.First);
            Assert.AreEqual("De", valeriano.LastPrefixes);
            Assert.AreEqual("De Leon", valeriano.Last);
            Assert.AreEqual("JR.", valeriano.Suffix);

            var quincy = new HumanName("Quincy De La Rosa Sr");
            Assert.AreEqual("Quincy", quincy.First);
            Assert.AreEqual("De La", quincy.LastPrefixes);
            Assert.AreEqual("De La Rosa", quincy.Last);
            Assert.AreEqual("Sr", quincy.Suffix);
        }

        [DataRow("VAN L JOHNSON", "VAN", "L", "JOHNSON")]
        [DataRow("VAN JOHNSON", "VAN", "", "JOHNSON")]
        [DataRow("JOHNSON, VAN L", "VAN", "L", "JOHNSON")]
        [TestMethod]
        // https://github.com/aeshirey/NameParserSharp/issues/15
        public void Prefix_AsFirstName(string full, string first, string middle, string last)
        {
            var sut = new HumanName(full);

            Assert.AreEqual(first, sut.First);
            Assert.AreEqual(middle, sut.Middle);
            Assert.AreEqual(last, sut.Last);
        }

        [TestMethod]
        public void Conjunctions()
        {
            Assert.IsNotNull(new HumanName("mrs and mrs mickey and minnie mouse"));
        }

        /// <summary>
        /// https://github.com/aeshirey/NameParserSharp/issues/18
        /// </summary>
        [TestMethod]
        public void AddToLists()
        {
            var parsed = new HumanName("Mr. John Smith 2nd");
            Assert.AreEqual("Mr.", parsed.Title);
            Assert.AreEqual("John", parsed.First);
            Assert.AreEqual("Smith", parsed.Middle);
            Assert.AreEqual("2nd", parsed.Last);
            Assert.IsEmpty(parsed.Suffix);

            HumanName.Suffixes.Add("2nd");
            var withSuffix = new HumanName("Mr. John Smith 2nd");
            Assert.AreEqual("Mr.", withSuffix.Title);
            Assert.AreEqual("John", withSuffix.First);
            Assert.AreEqual("Smith", withSuffix.Last);
            Assert.AreEqual("2nd", withSuffix.Suffix);
        }

        /// <summary>
        /// https://github.com/aeshirey/NameParserSharp/issues/20
        /// </summary>
        [TestMethod]
        public void FirstNameIsPrefix()
        {
            // Default behavior
            var parsedPrefix = new HumanName("Mr. Del Richards");
            Assert.AreEqual("Mr.", parsedPrefix.Title);
            Assert.IsEmpty(parsedPrefix.First);
            Assert.AreEqual("Del Richards", parsedPrefix.Last);
            Assert.AreEqual("Del", parsedPrefix.LastPrefixes);

            // A single prefix should be treated as a first name when no first exists
            var parsedFirst = new HumanName("Mr. Del Richards", Prefer.FirstOverPrefix);
            Assert.AreEqual("Mr.", parsedFirst.Title);
            Assert.AreEqual("Del", parsedFirst.First);
            Assert.AreEqual("Richards", parsedFirst.Last);
            Assert.IsEmpty(parsedFirst.LastPrefixes);
        }
    }
}
