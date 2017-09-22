using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Synergy.Lerman.Realm.Books
{
    public class Word
    {
        public Word()
        {
        }

        public Word(string en, string pl, Category category)
        {
            this.Polish = pl.Trim();
            this.Phrases = this.CalculateEnglishPhrases(en.Trim());
            this.Category = category;
        }

        [JsonIgnore]
        public Category Category { get; private set; }

        [JsonProperty(PropertyName = "pl")]
        public string Polish { get; private set; }

        [JsonProperty(PropertyName = "en")]
        private List<Phrase> Phrases { get; set; }

        private List<Phrase> CalculateEnglishPhrases(string en)
        {
            List<Phrase> phrases = new List<Phrase>();
            var versions = en.Split('|');
            foreach (var version in versions)
            {
                Regex regex = new Regex(@"\w+\s*\/\s*\w+");
                MatchCollection matches = regex.Matches(version);
                foreach (var match in matches)
                {
                    var optionalPart = ((Match) match).Value;
                    var options = optionalPart.Split('/');
                    foreach (var option in options)
                    {
                        var text = version.Replace(optionalPart, option.Trim());
                        phrases.Add(new Phrase(text));
                    }
                }

                if (matches.Count == 0)
                    phrases.Add(new Phrase(version));
            }

            return phrases;
        }

        public void Edit(string polish, List<string> phrases)
        {
            this.Polish = polish;
            this.Phrases = phrases.ConvertAll(p => new Phrase(p));
            this.TryToFindPronunciations();
        }

        public string GetPolishPhrase()
        {
            return this.Polish.Replace("/", " / ").Replace("  ", " ");
        }

        public List<string> GetEnglishTexts()
        {
            return this.Phrases.ConvertAll(phrase => phrase.Text);
        }

        public List<Phrase> GetEnglishPhrases()
        {
            return this.Phrases;
        }

        public string GetEnglishPhrase(int index)
        {
            var en = this.GetEnglishTexts();
            if (index >= en.Count)
                return null;

            return en[index];
        }

        public void TryToFindPronunciations()
        {
            this.Phrases.ForEach(phrase => phrase.TryToFindPronunciation());
        }

        public class Phrase
        {
            [NotNull]
            [JsonProperty(PropertyName = "text")]
            public string Text { get; private set; }

            [CanBeNull]
            [JsonProperty(PropertyName = "sound")]
            public string PronunciationUrl { get; private set; }

            public Phrase(string text)
            {
                Text = text;
            }

            [JsonConstructor]
            public Phrase(string text, string sound)
            {
                Text = text;
                PronunciationUrl = sound;
            }

            public void TryToFindPronunciation()
            {
                var url = "https://www.diki.pl/images-common/en/mp3/"+ this.Text.Replace(" ", "_") + ".mp3";
                if (this.RemoteFileExists(url))
                    this.PronunciationUrl = url;
            }


            private bool RemoteFileExists(string url)
            {
                try
                {
                    //Creating the HttpWebRequest
                    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                    //Setting the Request method HEAD, you can also use GET too.
                    request.Method = "HEAD";
                    //Getting the Web Response.
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    //Returns TRUE if the Status code == 200
                    response.Close();
                    return (response.StatusCode == HttpStatusCode.OK);
                }
                catch
                {
                    //Any exception will returns false.
                    return false;
                }
            }
        }

        public void BelongsTo(Category category)
        {
            this.Category = category;
        }
    }
}