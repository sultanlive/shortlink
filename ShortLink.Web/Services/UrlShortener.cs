using System.Text;

namespace ShortLink.Web.Services
{
    public class UrlShortener : IUrlShortener
    {
        public static readonly string Alphabet = "LaPD8K6b5xZOsEQjYwUMyN7qvmHtJudo4cki2I39lFXSneg0RGAVpBChfWTz1r";
        public static readonly int Base = Alphabet.Length;

        public int Decode(string shortLink)
        {
            var i = 0;

            foreach (var c in shortLink)
            {
                i = (i * Base) + Alphabet.IndexOf(c);
            }

            return i;
        }

        public string Encode(int id)
        {
            if (id == 0) return Alphabet[0].ToString();

            var s = new StringBuilder();

            while (id > 0)
            {
                s.Insert(0, Alphabet[id % Base]);
                id = id / Base;
            }

            return s.ToString();
        }
    }
}
