namespace NameParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Parse a person's name into individual components.
    /// Instantiation assigns to "fullName", and assignment to "fullName"
    /// triggers parseFullName. After parsing the name, these instance 
    /// attributes are available.
    /// </summary>
    public partial class HumanName
    {
        #region Properties
        /// <summary>
        /// Indicates whether any values were parsed out of the provided <see cref="FullName"/>
        /// </summary>
        public bool IsUnparsable { get; private set; }

        public static bool ParseMultipleNames { get; set; }

        /// <summary>
        /// The full name without nickname
        /// </summary>
        public string FullName
        {
            get { return _FullName; }
            private set
            {
                _OriginalName = value;
                _FullName = _OriginalName;

                _TitleList = new List<string>();
                _FirstList = new List<string>();
                _MiddleList = new List<string>();
                _LastList = new List<string>();
                _SuffixList = new List<string>();
                _NicknameList = new List<string>();
                _LastBaseList = new List<string>();
                _LastPrefixList = new List<string>();

                if (!string.IsNullOrEmpty(value))
                {
                    ParseFullName();
                }
            }
        }

        public string Title
        {
            get { return string.Join(" ", _TitleList); }
        }

        public string First
        {
            get { return string.Join(" ", _FirstList); }
        }

        public string Middle
        {
            get { return string.Join(" ", _MiddleList); }
        }

        public string Last
        {
            get { return string.Join(" ", _LastList); }
        }

        public string Suffix
        {
            get { return string.Join(" ", _SuffixList); }
        }

        public string Nickname
        {
            get { return string.Join(" ", _NicknameList); }
        }

        /// <summary>
        /// If <see cref="ParseMultipleNames"/> is true and the input contains "&" or "and", the additional
        /// name will be parsed out and put into a second <see cref="HumanName"/> record. For example,
        /// "John D. and Catherine T. MacArthur" should be parsed as {John, D, MacArthur} with an AdditionalName
        /// set to the parsed value {Catherine, T, MacAthur}.
        /// </summary>
        public HumanName AdditionalName { get; private set; }

        //public string LastBase { get; private set; }
        public string LastBase
        {
            get { return string.Join(" ", _LastBaseList); }
        }

        //public string LastPrefixes { get; private set; }
        public string LastPrefixes
        {
            get { return string.Join(" ", _LastPrefixList); }
        }
        #endregion

        private string _FullName, _OriginalName;

        private IList<string> _TitleList;
        private IList<string> _FirstList;
        private IList<string> _MiddleList;
        private IList<string> _LastList;
        private IList<string> _SuffixList;
        private IList<string> _NicknameList;
        private IList<string> _LastBaseList;
        private IList<string> _LastPrefixList;

        public HumanName(string fullName)
        {
            if (fullName == null)
            {
                throw new ArgumentNullException("fullName");
            }

            FullName = fullName;
        }

        public static bool operator ==(HumanName left, HumanName right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return left.Title == right.Title
                && left.First == right.First
                && left.Middle == right.Middle
                && left.Last == right.Last
                && left.Suffix == right.Suffix
                &&
                (string.IsNullOrEmpty(left.Nickname) || string.IsNullOrEmpty(right.Nickname) ||
                left.Nickname == right.Nickname);
        }

        public static bool operator !=(HumanName left, HumanName right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Return the parsed name as a dictionary of its attributes.
        /// </summary>
        /// <param name="includeEmpty">Include keys in the dictionary for empty name attributes.</param>
        /// <returns></returns>
        public Dictionary<string, string> AsDictionary(bool includeEmpty = true)
        {
            var d = new Dictionary<string, string>();

            if (includeEmpty || !string.IsNullOrEmpty(Title))
            {
                d["title"] = Title;
            }

            if (includeEmpty || !string.IsNullOrEmpty(First))
            {
                d["first"] = First;
            }

            if (includeEmpty || !string.IsNullOrEmpty(Middle))
            {
                d["middle"] = Middle;
            }

            if (includeEmpty || !string.IsNullOrEmpty(Last))
            {
                d["last"] = Last;
            }

            if (includeEmpty || !string.IsNullOrEmpty(LastBase))
            {
                d["lastbase"] = LastBase;
            }

            if (includeEmpty || !string.IsNullOrEmpty(LastPrefixes))
            {
                d["lastprefixes"] = LastPrefixes;
            }

            if (includeEmpty || !string.IsNullOrEmpty(Suffix))
            {
                d["suffix"] = Suffix;
            }

            if (includeEmpty || !string.IsNullOrEmpty(Nickname))
            {
                d["nickname"] = Nickname;
            }

            return d;
        }

        #region  Parse helpers
        private static bool IsTitle(string value)
        {
            return Titles.Contains(value.ToLower().Replace(".", string.Empty));
        }

        private static bool IsConjunction(string piece)
        {
            return Conjunctions.Contains(piece.ToLower().Replace(".", string.Empty)) && !IsAnInitial(piece);
        }

        private static bool IsPrefix(string piece)
        {
            return Prefixes.Contains(piece.ToLower().Replace(".", string.Empty)) && !IsAnInitial(piece);
        }

        private static bool IsSuffix(string piece)
        {
            return Suffixes.Contains(piece.Replace(".", string.Empty).ToLower()) && !IsAnInitial(piece);
        }

        private static bool AreSuffixes(IEnumerable<string> pieces)
        {
            return pieces.Any() && pieces.All(IsSuffix);
        }

        /// <summary>
        /// Determines whether <see cref="piece"/> is a given name component as opposed to an affix, initial or title.
        /// </summary>
        /// <param name="piece">A single word from a name</param>
        /// <returns>False if <see cref="piece"/> is a prefix (de, abu, bin), suffix (jr, iv, cpa), title (mr, pope), or initial (x, e.); true otherwise</returns>
        private static bool IsRootname(string piece)
        {
            var lcPiece = piece.ToLower().Replace(".", string.Empty);

            return !Suffixes.Contains(lcPiece)
                && !Prefixes.Contains(lcPiece)
                && !Titles.Contains(lcPiece)
                && !IsAnInitial(piece);
        }

        /// <summary>
        /// Words with a single period at the end, or a single uppercase letter.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True iff <see cref="value"/> matches the regex "^[A-Za-z].?$"</returns>
        private static bool IsAnInitial(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 2)
            {
                return false;
            }

            return char.IsLetter(value[0]) && (value.Length == 1 || value[1] == '.');
        }
        #endregion

        #region full name parser
        /// <summary>
        /// If there are only two parts and one is a title, assume it's a last name
        /// instead of a first name. e.g. Mr. Johnson. Unless it's a special title
        /// like "Sir", then when it's followed by a single name that name is always
        /// a first name.
        /// </summary>
        private void PostProcessFirstnames()
        {
            if (!string.IsNullOrEmpty(Title)
                && !FirstNameTitles.Contains(Title.ToLower().Replace(".", string.Empty))
                && 1 == _FirstList.Count + _LastList.Count)
            {
                if (_FirstList.Any())
                {
                    _LastList = _FirstList;
                    _FirstList = new List<string>();
                }
                else
                {
                    _FirstList = _LastList;
                    _LastList = new List<string>();
                }
            }
        }

        /// <summary>
        /// Parse out the last name components into prefixes and a base last name
        /// in order to allow sorting. Prefixes are those in <see cref="Prefixes"/>,
        /// start off <see cref="Last"/> and are contiguous. See <seealso cref="https://en.wikipedia.org/wiki/Tussenvoegsel"/>
        /// </summary>
        private void PostProcessLastname()
        {
            // parse out 'words' from the last name
            var words = _LastList
                .SelectMany(part => part.Split(' '))
                .ToList();

            var prefixCount = 0;
            while (prefixCount < words.Count && IsPrefix(words[prefixCount]))
            {
                prefixCount++;
            }

            _LastPrefixList = words.Take(prefixCount).ToList();
            _LastBaseList = words.Skip(prefixCount).ToList();
        }

        private void PostProcessAdditionalName()
        {
            if (!ParseMultipleNames || AdditionalName == null)
            {
                return;
            }

            // Often, the secondary in a pair of names will contain the last name but not the primary.
            // (eg, John D. and Catherine T. MacArthur). In this case, we should be able to infer
            // the primary's last name from the secondary.
            if (string.IsNullOrEmpty(Last))
            {
                _LastList = AdditionalName._LastList;
            }
            else
            {
                // for names like "Smith, John And Jane", we'd have to propagate the name backward (possibly through multiple names)
                var next = AdditionalName;
                while (next != null && string.IsNullOrEmpty(next.Last))
                {
                    next._LastList = _LastList;
                    next = next.AdditionalName;
                }
            }
        }

        /// <summary>
        /// The main parse method for the parser. This method is run upon assignment to the
        /// fullName attribute or instantiation.
        /// 
        /// Basic flow is to hand off to `pre_process` to handle nicknames. It
        /// then splits on commas and chooses a code path depending on the number of commas.
        /// `parsePieces` then splits those parts on spaces and
        /// `joinOnConjunctions` joins any pieces next to conjunctions. 
        /// </summary>
        private void ParseFullName()
        {
            if (ParseMultipleNames)
            {
                if (_FullName.Contains('&'))
                {
                    var split = _FullName.IndexOf('&');

                    var primary = _FullName.Substring(0, split);

                    var secondary = _FullName.Substring(split + 1);
                    AdditionalName = new HumanName(secondary);

                    _FullName = primary;
                }
                else if (_FullName.ToLowerInvariant().Contains(" and "))
                {
                    var split = _FullName.IndexOf(" and ", StringComparison.InvariantCultureIgnoreCase);

                    var primary = _FullName.Substring(0, split);

                    var secondary = _FullName.Substring(split + 5 /* length of " and " */);
                    AdditionalName = new HumanName(secondary);

                    _FullName = primary;
                }
            }

            ParseNicknames(ref _FullName, out _NicknameList);

            // break up fullName by commas
            var parts = _FullName
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .ToList();

            if (parts.Count == 0)
            {
                // Edge case where the input was all in parens and has become the nickname
                // See https://github.com/aeshirey/NameParserSharp/issues/8
            }
            else if (parts.Count == 1)
            {
                // no commas, title first middle middle middle last suffix
                //            part[0]

                var pieces = ParsePieces(parts);

                for (var i = 0; i < pieces.Length; i++)
                {
                    var piece = pieces[i];
                    var nxt = i == pieces.Length - 1 ? string.Empty : pieces[i + 1];

                    // title must have a next piece, unless it's just a title
                    if (IsTitle(piece) && (!string.IsNullOrEmpty(nxt) || pieces.Length == 1))
                    {
                        // some last names appear as titles (https://github.com/aeshirey/NameParserSharp/issues/9)
                        // if we've already parsed out titles, first, or middle names, something appearing as a title may in fact be a last name
                        if (_FirstList.Count > 0 || _MiddleList.Count > 0)
                        {
                            _LastList.Add(piece);
                        }
                        else
                        {
                            _TitleList.Add(piece);
                        }
                    }
                    else if (string.IsNullOrEmpty(First))
                    {
                        _FirstList.Add(piece);
                    }
                    else if (AreSuffixes(pieces.Skip(i + 1)))
                    {
                        _LastList.Add(piece);
                        _SuffixList = _SuffixList.Concat(pieces.Skip(i + 1)).ToList();
                        break;
                    }
                    else if (!string.IsNullOrEmpty(nxt))
                    {
                        // another component exists, so this is likely a middle name
                        _MiddleList.Add(piece);
                    }
                    else if (!ParseMultipleNames || AdditionalName == null)
                    {
                        // no additional name. some last names can appear to be suffixes. try to figure this out
                        if (_LastList.Count > 0 && IsSuffix(piece))
                        {
                            _SuffixList.Add(piece);
                        }
                        else
                        {
                            _LastList.Add(piece);
                        }
                    }
                    else if (AdditionalName._LastList.Any() && IsAnInitial(piece))
                    {
                        // the additional name has a last, and this one looks like a middle. we'll save as a middle and later will copy the last name.
                        _MiddleList.Add(piece);
                    }
                    else
                    {
                        _LastList.Add(piece);
                    }
                }
            }
            else if (AreSuffixes(parts[1].Split(' ')))
            {
                // suffix comma: title first middle last [suffix], suffix [suffix] [, suffix]
                //               parts[0],                         parts[1:...]
                _SuffixList = _SuffixList.Concat(parts.Skip(1)).ToList();
                var pieces = ParsePieces(parts[0].Split(new[] { ' ' }));

                for (var i = 0; i < pieces.Length; i++)
                {
                    var piece = pieces[i];
                    var nxt = i == pieces.Length - 1 ? string.Empty : pieces[i + 1];

                    if (IsTitle(piece) && (!string.IsNullOrEmpty(nxt) || pieces.Length == 1))
                    {
                        _TitleList.Add(piece);
                        continue;
                    }

                    if (string.IsNullOrEmpty(First))
                    {
                        _FirstList.Add(piece);
                        continue;
                    }

                    if (AreSuffixes(pieces.Skip(i + 1)))
                    {
                        _LastList.Add(piece);
                        _SuffixList = pieces.Skip(i + 1).Concat(_SuffixList).ToList();
                        break;
                    }

                    // correct for when we have "John D" with an AdditionalName={Catherine, T, MacArthur}.
                    // We should not see this as being First=John, Last=D; rather, First=John, Middle=D, Last=<AdditionalName.Last>
                    if (!string.IsNullOrEmpty(nxt))
                    {
                        // another component exists, so this is likely a middle name
                        _MiddleList.Add(piece);
                    }
                    else if (!ParseMultipleNames || AdditionalName == null)
                    {
                        // no additional name, so treat this as a last
                        _LastList.Add(piece);
                    }
                    else if (AdditionalName._LastList.Any() && IsAnInitial(piece))
                    {
                        // the additional name has a last, and this one looks like a middle. we'll save as a middle and later will copy the last name.
                        _MiddleList.Add(piece);
                    }
                    else
                    {
                        _LastList.Add(piece);
                    }
                }
            }
            else
            {
                // lastname comma: last [suffix], title first middles[,] suffix [,suffix]
                //                 parts[0],      parts[1],              parts[2:...]
                var pieces = ParsePieces(parts[1].Split(new[] { ' ' }), 1);

                // lastname part may have suffixes in it
                var lastnamePieces = ParsePieces(parts[0].Split(new[] { ' ' }), 1);

                foreach (var piece in lastnamePieces)
                {

                    // the first one is always a last name, even if it look like a suffix
                    if (IsSuffix(piece) && _LastList.Any())
                    {
                        _SuffixList.Add(piece);
                    }
                    else
                    {
                        _LastList.Add(piece);
                    }
                }

                for (var i = 0; i < pieces.Length; i++)
                {
                    var piece = pieces[i];
                    var nxt = i == pieces.Length - 1 ? string.Empty : pieces[i + 1];

                    if (IsTitle(piece) && (!string.IsNullOrEmpty(nxt) || pieces.Length == 1))
                    {
                        _TitleList.Add(piece);
                    }
                    else if (string.IsNullOrEmpty(First))
                    {
                        _FirstList.Add(piece);
                    }
                    else if (IsSuffix(piece))
                    {
                        _SuffixList.Add(piece);
                    }
                    else
                    {
                        _MiddleList.Add(piece);
                    }
                }
                if (parts.Count() >= 3 && !string.IsNullOrEmpty(parts[2]))
                {
                    _SuffixList = _SuffixList.Concat(parts.Skip(2)).ToList();
                }
            }

            IsUnparsable = !_TitleList.Any()
                         && !_FirstList.Any()
                         && !_MiddleList.Any()
                         && !_LastList.Any()
                         && !_SuffixList.Any()
                         && !_NicknameList.Any();

            PostProcessFirstnames();
            PostProcessLastname();
            PostProcessAdditionalName();
        }

        private static void ParseNicknames(ref string fullName, out IList<string> nicknameList)
        {
            // this regex is an improvement upon the original in that it adds apostrophes and appropriately captures 
            // the nicknames in "john 'jack' kennedy", "richard (dick) nixon" and @"william ""bill"" clinton".
            // it also doesn't try to parse out improperly matched inputs that the python version would have such as 
            // @"john (j"" jones", @"samuel (sammy"" samsonite" 

            // https://code.google.com/p/python-nameparser/issues/detail?id=33
            var nicknameRegex = new Regex(@"\s*(?:\((.+?)\))|(?:([""'])(.+?)\2)");
            nicknameList = new List<string>();

            var match = nicknameRegex.Match(fullName);

            var nicknameFound = false;
            while (match.Success && match.Groups[0].Value.Length > 0)
            {
                nicknameFound = true;

                // remove from the full name the nickname plus its identifying boundary (parens or quotes)
                fullName = fullName.Replace(match.Groups[0].Value, string.Empty);

                // keep only the nickname part
                var matchGroup = match.Groups[0].Value.TrimStart().StartsWith("(") ? 1 : 3; // which regex group was used: 1 is for parens; 3 is single- or double-quoted nicknames
                nicknameList.Add(match.Groups[matchGroup].Value);

                match = nicknameRegex.Match(fullName);
            }

            // normalize whitespace
            if (nicknameFound)
            {
                fullName = string.Join(" ", fullName.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        /// <summary>
        /// Split parts on spaces and remove commas, join on conjunctions and lastname prefixes.
        /// </summary>
        /// <param name="parts">name part strings from the comma split</param>
        /// <param name="additionalPartsCount"></param>
        /// <returns>pieces split on spaces and joined on conjunctions</returns>
        protected static string[] ParsePieces(IEnumerable<string> parts, int additionalPartsCount = 0)
        {
            var tmp = new List<string>();
            foreach (var part in parts)
            {
                tmp.AddRange(part.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim(',')));
            }

            return joinOnConjunctions(tmp, additionalPartsCount);
        }

        /// <summary>
        /// join conjunctions to surrounding pieces, e.g.:
        ///    ['Mr. and Mrs.'], ['King of the Hill'], ['Jack and Jill'], ['Velasquez y Garcia']
        /// </summary>
        /// <param name="pieces">name pieces strings after split on spaces</param>
        /// <param name="additionalPartsCount"></param>
        /// <returns>new list with piece next to conjunctions merged into one piece with spaces in it.</returns>
        internal static string[] joinOnConjunctions(List<string> pieces, int additionalPartsCount = 0)
        {
            var length = pieces.Count() + additionalPartsCount;

            // don't join on conjuctions if there are only 2 parts
            if (length < 3)
            {
                return pieces.ToArray();
            }

            foreach (var conj in pieces.Where(IsConjunction).Reverse())
            {
                // loop through the pieces backwards, starting at the end of the list.
                // Join conjunctions to the pieces on either side of them.
                if (conj.Length == 1 && pieces.Count(IsRootname) < 4)
                {
                    // if there are only 3 total parts (minus known titles, suffixes and prefixes) 
                    // and this conjunction is a single letter, prefer treating it as an initial
                    // rather than a conjunction.
                    // http://code.google.com/p/python-nameparser/issues/detail?id=11
                    continue;
                }

                var index = pieces.IndexOf(conj);

                if (index == -1)
                {
                    continue;
                }

                if (index < pieces.Count() - 1)
                {
                    // if this is not the last piece
                    string newPiece;
                    if (index == 0)
                    {
                        // if this is the first piece and it's a conjunction
                        var nxt = pieces[index + 1];

                        var cons = IsTitle(nxt) ? Conjunctions : Titles;
                        newPiece = string.Join(" ", pieces.Take(2));
                        cons.Add(newPiece);
                        pieces[index] = newPiece;
                        pieces.RemoveAt(index + 1);
                        continue;
                    }

                    if (IsConjunction(pieces[index - 1]))
                    {
                        // if the piece in front of this one is a conjunction too,
                        // add new_piece (this conjuction and the following piece) 
                        // to the conjuctions constant so that it is recognized
                        // as a conjunction in the next loop. 
                        // e.g. for ["Lord","of","the Universe"], put "the Universe"
                        // into the conjunctions constant.

                        newPiece = string.Join(" ", pieces.Skip(index).Take(2));
                        Conjunctions.Add(newPiece);
                        pieces[index] = newPiece;
                        pieces.RemoveAt(index + 1);
                        continue;
                    }

                    newPiece = string.Join(" ", pieces.Skip(index - 1).Take(3));

                    if (IsTitle(pieces[index - 1]))
                    {
                        // if the second name is a title, assume the first one is too and add the 
                        // two titles with the conjunction between them to the titles constant 
                        // so the combo we just created gets parsed as a title. 
                        // e.g. "Mr. and Mrs." becomes a title.
                        Titles.Add(newPiece);
                    }

                    pieces[index - 1] = newPiece;
                    pieces.RemoveAt(index);
                    pieces.RemoveAt(index);
                }
            }

            // join prefixes to following lastnames: ['de la Vega'], ['van Buren']
            // skip first part to avoid counting it as a prefix, e.g. "van" is either a first name or a preposition depending on its position
            var prefixes = pieces.Skip(1).Where(IsPrefix).ToArray();
            if (prefixes.Length > 0)
            {
                var i = pieces.IndexOf(prefixes[0]);
                // join everything after the prefix until the next suffix
                var nextSuffix = pieces.Skip(i).Where(IsSuffix).ToArray();

                if (nextSuffix.Length > 0)
                {
                    var j = pieces.IndexOf(nextSuffix[0]);
                    var newPiece = string.Join(" ", pieces.Skip(i).Take(j - i));

                    pieces = pieces
                        .Take(i)
                        .Concat(new[] { newPiece })
                        .Concat(pieces.Skip(j))
                        .ToList();
                }
                else
                {
                    var newPiece = string.Join(" ", pieces.Skip(i));
                    pieces = pieces.Take(i).ToList();
                    pieces.Add(newPiece);
                }
            }

            return pieces.ToArray();
        }
        #endregion

        #region Capitalization Support

        /// <summary>
        /// Capitalize a single word in a context-sensitive manner. Values such as "and", "der" and "bin" are unmodified, but "smith" -> "Smith", and "phd" -> "Ph.D."
        /// </summary>
        private static string CapitalizeWord(string word)
        {
            var wordLower = word.ToLower().Replace(".", string.Empty);
            if (IsPrefix(word) || IsConjunction(word))
            {
                return wordLower;
            }

            // "phd" => "Ph.D."; "ii" => "II"
            var exception = CapitalizationExceptions.FirstOrDefault(tup => tup.Item1 == wordLower);

            if (exception != null)
            {
                return exception.Item2;
            }

            // special case: "macbeth" should be "MacBeth"; "mcbride" -> "McBride"
            var macMatch = RegexMac.Match(word);
            if (macMatch.Success)
            {
                return ToTitleCase(macMatch.Groups[1].Value) + ToTitleCase(macMatch.Groups[2].Value);
            }

            return ToTitleCase(word);
        }

        private static string ToTitleCase(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            return s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
        }

        private static string CapitalizePiece(string piece)
        {
            return string.Join(" ", piece.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(CapitalizeWord));
        }

        /// <summary>
        /// Attempt to normalize the input values in a human-readable way. For example, "juan de garcia" would normalize to "Juan de Garcia"
        /// </summary>
        public void Normalize()
        {
            _TitleList = _TitleList.Select(CapitalizePiece).ToList();
            _FirstList = _FirstList.Select(CapitalizePiece).ToList();
            _MiddleList = _MiddleList.Select(CapitalizePiece).ToList();
            _LastList = _LastList.Select(CapitalizePiece).ToList(); // CapitalizePiece recognizes prefixes, so its okay to normalize "van der waals" like this
            _SuffixList = _SuffixList.Select(CapitalizePiece).ToList();
            _NicknameList = _NicknameList.Select(CapitalizePiece).ToList();
            _LastBaseList = _LastBaseList.Select(CapitalizePiece).ToList();
            // normalizing _LastPrefixList would effectively be a no-op, so don't bother calling it

            var fullNamePieces = _FullName
                .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(CapitalizePiece);

            _FullName = string.Join(" ", fullNamePieces);
        }
        #endregion
    }
}
