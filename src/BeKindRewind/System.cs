namespace BeKindRewind;

using CustomerId = int;
using VideoId = int;

/*
    Philosophy:

    This is designed around the "D" (Dependency Inversion) in SOLID. This
    class is responsible for organizing customers and videos should be aware of
    customers and videos, but customers and videos should not be aware of
    the System.

    I chose not to give videos an IsRented field simply because this would
    lock me into doing a linear search (even though I do it anyway in places
    where I don't have to in this learning assignment), so I opted to use
    multiple dictionaries instead (this is basically standard and encouraged
    practice when making SQL lookup tables!) but from an organizational and
    learning standpoint, it seems to have been the thing to do. Simultaneously,
    the video being aware of whether or not its rented seems to betray the "D"
    in SOLID a bit because it telegraphs some sort of small awareness of how it
    should be used (VHS tapes in real life can exist independently of a rental
    store POS system).

    Thus, both the Video and the Customer structs are POD, and multiple, albeit
    intentionally redundant dictionaries are used.
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
    private readonly Dictionary<CustomerId, HashSet<VideoId>> _customerRentalHistory = [];
    public ICollection<VideoId> RentedVideos => _rentedVideoToCustomer.Keys;
    public IReadOnlyDictionary<CustomerId, HashSet<VideoId>> RentalHistory => _customerRentalHistory;

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

        if (!_customerRentalHistory.TryGetValue(customerId, out HashSet<VideoId>? historicVideoIds))
        {
            historicVideoIds = ([]);
            _customerRentalHistory[customerId] = historicVideoIds;
        }

        videoIds.Add(videoId);
        _rentedVideoToCustomer[videoId] = customerId;
        _customerToRentedVideos[customerId].Add(videoId);
        _customerRentalHistory[customerId].Add(videoId);
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
