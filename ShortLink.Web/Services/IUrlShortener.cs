namespace ShortLink.Web.Services
{
    public interface IUrlShortener
    {
        string Encode(int id);
        int Decode(string shortLink);
    }
}
