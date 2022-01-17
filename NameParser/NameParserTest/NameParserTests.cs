using Microsoft.VisualStudio.TestTools.UnitTesting;
using NameParser;
using System;

namespace NameParseTest
{
    [TestClass]
    public class NameParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullInput()
        {
            var parsed = new HumanName(null);
        }


        [TestMethod]
        public void BlankInput()
        {
            var parsed = new HumanName(string.Empty);
            Assert.AreEqual(string.Empty, parsed.First);
            Assert.AreEqual(string.Empty, parsed.Middle);
            Assert.AreEqual(string.Empty, parsed.Last);
            Assert.AreEqual(string.Empty, parsed.Title);
            Assert.AreEqual(string.Empty, parsed.Nickname);
            Assert.AreEqual(string.Empty, parsed.Suffix);
        }

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

        [TestMethod]
        public void TwoNames_MacAthur()
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
            Assert.AreEqual("", parsed.Middle);
            Assert.AreEqual("Bloggs", parsed.Last);

            Assert.IsNotNull(parsed.AdditionalName);

            Assert.AreEqual("Miss", parsed.AdditionalName.Title);
            Assert.AreEqual("L", parsed.AdditionalName.First);
            Assert.AreEqual("", parsed.AdditionalName.Middle);
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
            Assert.AreEqual(string.Empty, johnSmith.First);
            Assert.AreEqual(string.Empty, johnSmith.Last);
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

            Assert.AreEqual(parsed.First, "John");
            Assert.AreEqual(parsed.Middle, "Middle");
            Assert.AreEqual(parsed.Last, "Surname");
            Assert.AreEqual(parsed.Suffix, "III");
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
            var mice = new HumanName("mrs and mrs mickey and minnie mouse");
        }


        /// <summary>
        /// https://github.com/aeshirey/NameParserSharp/issues/18
        /// </summary>
        [TestMethod]
        public void AddToLists()
        {
            var as_is = new HumanName("Mr. John Smith 2nd");
            Assert.AreEqual("Mr.", as_is.Title);
            Assert.AreEqual("John", as_is.First);
            Assert.AreEqual("Smith", as_is.Middle);
            Assert.AreEqual("2nd", as_is.Last);
            Assert.AreEqual("", as_is.Suffix);


            HumanName.Suffixes.Add("2nd");
            var with_2nd = new HumanName("Mr. John Smith 2nd");
            Assert.AreEqual("Mr.", with_2nd.Title);
            Assert.AreEqual("John", with_2nd.First);
            Assert.AreEqual("Smith", with_2nd.Last);
            Assert.AreEqual("2nd", with_2nd.Suffix);
        }


        /// <summary>
        /// https://github.com/aeshirey/NameParserSharp/issues/20
        /// </summary>
        //[TestMethod]
        public void FirstNameIsPrefix()
        {
            var parsed = new HumanName("Mr. Del Richards");
            Assert.AreEqual(parsed.Title, "Mr.");
            Assert.AreEqual(parsed.First, "Del");
            Assert.AreEqual(parsed.Last, "Richards");
            Assert.AreEqual(parsed.LastPrefixes, "");
        }
    }
}
