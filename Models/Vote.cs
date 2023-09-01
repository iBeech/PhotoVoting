namespace PhotoVotingApp.Models
{
    public class Vote
    {
        public int Id { get; set; } = -1;
        public string Fingerprint { get; set; }

        public bool HasVoted => Id > -1;
        public bool FingerprintMatch(string fingerprint)
        {
            return Fingerprint == fingerprint;
        }
    }
}
