// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using HighScore;
using BeKindRewind;

LearningActivity5_1();
LearningActivity5_2();

void LearningActivity5_1()
{
    Random scoreRandom = new();
    HighScore.DescendingOrderTree<(int Score, Initials Initials)> highScores = new();
    GameSelection? choice = null;

    while (choice != GameSelection.Exit)
    {
        choice = ChooseOption();

        switch (choice)
        {
        case GameSelection.Play:
            int score = scoreRandom.Next(100, 1000000);

            Console.Write($"A score of {score} was achieved. Enter your initials: ");

            while (true)
            {
                if (!Initials.TryParse(Console.ReadLine(), out Initials initials))
                {
                    Console.Write("Invalid initials! Enter your initials: ");
                    continue;
                }

                highScores.Add((score, initials));
                break;
            }

            break;
        case GameSelection.SeeHighScore:
            highScores.Traverse(pair => Console.WriteLine($"{pair.Initials}: {pair.Score}"));
            break;
        }
    }
}

GameSelection ChooseOption()
{
    GameSelection selection;

    while (true)
    {
        Console.WriteLine("Would you like to:\n\t 1: Play again\n\t 2: See the list of high scores\n\t 3: Exit the game?");

        if (!Enum.TryParse(Console.ReadLine(), out selection) || !Enum.IsDefined(selection))
        {
            Console.WriteLine("Invalid selection!");
            continue;
        }

        break;
    }

    return selection;
}

// Be kind, rewind.
void LearningActivity5_2()
{
    BeKindRewind.System system = new();

    system.AddCustomer(new Customer
    {
        EmailAddress = "johndoe1983@bomb.com",
        FullName = "John Doe",
    });

    system.AddCustomer(new Customer
    {
        EmailAddress = "janedoe1982@tucowsmail.ca",
        FullName = "Jane Doe",
    });

    system.AddVideo(new Video
    {
        Duration = new TimeSpan(1, 30, 0),
        Genre = "Action",
        Name = "Rumble in the Bronx",
        ReleaseDate = new DateTime(1995, 1, 21),
    });

    system.AddVideo(new Video
    {
        Duration = new TimeSpan(1, 35, 0),
        Genre = "Comedy Drama",
        Name = "When Harry Met Sally...",
        ReleaseDate = new DateTime(1989, 7, 14),
    });

    system.AddVideo(new Video
    {
        Duration = new TimeSpan(1, 21, 0),
        Genre = "Horror",
        Name = "The Blair Witch Project",
        ReleaseDate = new DateTime(1999, 1, 23),
    });

    system.AddVideo(new Video
    {
        Duration = new TimeSpan(1, 17, 0),
        Genre = "Science Fiction",
        Name = "Primer",
        ReleaseDate = new DateTime(2004, 10, 8),
    });

    system.AddVideo(new Video
    {
        Duration = new TimeSpan(2, 27, 0),
        Genre = "War",
        Name = "All Quiet on the Western Front",
        ReleaseDate = new DateTime(2022, 9, 12),
    });

    system.AddVideo(new Video
    {
        Duration = new TimeSpan(1, 32, 0),
        Genre = "War",
        Name = "All Quiet on the Western Front",
        ReleaseDate = new DateTime(1930, 4, 21),
    });

    IEnumerable<int> searchResults = system.SearchCustomers("John");

    if (!searchResults.Any())
    {
        Console.WriteLine("No customers named John!");
        return;
    }

    int john = searchResults.First();

    searchResults = system.SearchVideos("All Quiet").Where(videoId => system.GetVideo(videoId).ReleaseDate.Year == 1930);

    if (!searchResults.Any())
    {
        Console.WriteLine("No videos matching 'All Quiet'!");
        return;
    }

    int allQuiet1930 = searchResults.First();

    system.RentVideo(john, allQuiet1930);
    Console.WriteLine("Rented out videos so far for John: ");

    foreach (var rentedVideoId in system.RentedVideos)
    {
        var video = system.GetVideo(rentedVideoId);
        Console.WriteLine($"{video.Name} ({video.ReleaseDate.Year})");
    }

    Console.WriteLine("Returning All Quiet 1930...");
    system.ReturnVideo(allQuiet1930);
    Debug.Assert(system.RentedVideos.Count == 0);

    Console.WriteLine("Rental history for John: ");

    foreach (var videoId in system.RentalHistory[john])
    {
        var video = system.GetVideo(videoId);
        Console.WriteLine($"{video.Name} ({video.ReleaseDate.Year})");
    }
}

enum GameSelection
{
    Play = 1,
    SeeHighScore,
    Exit,
}
