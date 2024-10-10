namespace BeKindRewind;

using CustomerId = int;
using VideoId = int;

/*
    Philosophy:

    This is designed around the "D" (Dependency Inversion) in SOLID. This
    class responsible for organizing customers and videos should be aware of
    customers and videos, but customers and videos should not be aware of
    the System. Thus,
*/

public class System
{
    private class Database<T>
    {
        private readonly Dictionary<int, T> _map = [];
        private int _currentId;

        public IReadOnlyDictionary<int, T> Map => _map;

        public int Add(T thing)
        {
            _map[_currentId++] = thing;
            return _currentId;
        }
    }

    public class VideoNotFoundException : Exception
    {
        public VideoNotFoundException() { }
        public VideoNotFoundException(string message) : base(message) { }
        public VideoNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    private readonly Database<Customer> _customers = new();
    private readonly Database<Video> _videos = new();
    private readonly Dictionary<CustomerId, HashSet<VideoId>> _customerToRentedVideos = [];
    private readonly Dictionary<VideoId, CustomerId> _rentedVideoToCustomer = [];
    public ICollection<VideoId> RentedVideos => _rentedVideoToCustomer.Keys;

    private static List<int> Search<T_RECORD, T_QUERY>(Database<T_RECORD> database, Func<T_RECORD, T_QUERY, bool> queryDelegate, T_QUERY query)
    {
        List<int> matches = [];

        foreach (var kvp in database.Map)
        {
            int id = kvp.Key;

            if (queryDelegate(kvp.Value, query))
            {
                matches.Add(id);
            }
        }

        return matches;
    }

    public void AddCustomer(Customer customer) => _customers.Add(customer);
    public void AddVideo(Video video) => _videos.Add(video);

    public IEnumerable<CustomerId> SearchCustomers(string searchCustomersQuery)
        => Search(
            _customers,
            (customer, query) => customer.FullName.Contains(query) || customer.EmailAddress.Contains(query),
            searchCustomersQuery);
    public IEnumerable<VideoId> SearchVideos(string searchVideosQuery)
        => Search(
            _videos,
            (video, query) => video.Name.Contains(query),
            searchVideosQuery);
    public Customer GetCustomer(CustomerId customerId) => _customers.Map[customerId];
    public Video GetVideo(VideoId videoId) => _videos.Map[videoId];

    public void RentVideo(CustomerId customerId, VideoId videoId)
    {
        // Prefer a 'TryGetValue' call over a Dictionary indexer access guarded by a 'ContainsKey' check to avoid double lookup csharp(CA1854)
        if (!_customerToRentedVideos.TryGetValue(customerId, out HashSet<VideoId>? videoIds))
        {
            videoIds = ([]);
            _customerToRentedVideos[customerId] = videoIds;
        }

        videoIds.Add(videoId);
        _rentedVideoToCustomer[videoId] = customerId;
        _customerToRentedVideos[customerId].Add(videoId);
    }

    public void ReturnVideo(VideoId videoId)
    {
        try
        {
            CustomerId customerId = _rentedVideoToCustomer[videoId];

            _rentedVideoToCustomer.Remove(videoId);
            _customerToRentedVideos[customerId].Remove(videoId);
        }
        catch (InvalidOperationException)
        {
            throw new VideoNotFoundException("Video could not be returned because its provided ID was not found.");
        }
    }
}
