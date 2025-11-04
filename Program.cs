﻿using NLog;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

string? choice;

var db = new DataContext();

do
{
  // display choices to user
  Console.WriteLine("Enter your selection:");
  Console.WriteLine("1) Display all Blogs");
  Console.WriteLine("2) Add Blog");
  Console.WriteLine("3) Create Post");
  Console.WriteLine("4) Display Posts");
  Console.WriteLine("Enter q to quit");

  // input selection
  choice = Console.ReadLine();
  logger.Info("User choice: {Choice}", choice);

  if (choice == "1")
  {
    // Display all Blogs from the database
    var query = db.Blogs.OrderBy(b => b.Name);

    Console.WriteLine($"{db.Blogs.Count()} Blogs returned");
    foreach (var item in query)
    {
      Console.WriteLine(item.Name);
    }

    logger.Info("Program ended");
  }
  else if (choice == "2")
  {
    // Create and save a new Blog
    Console.Write("Enter a name for a new Blog: ");
    var name = Console.ReadLine();
    if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Blog name cannot be null");
        }
    else if (!string.IsNullOrEmpty(name))
        {
          var blog = new Blog { Name = name };

          db.AddBlog(blog);
          logger.Info("Blog added - {name}", name);
        }
  }
  else if (choice == "3")
  {
    // Create and save a new Post
    Console.WriteLine("Select the blog you would like to post to:");
    var blogs = db.Blogs.OrderBy(b => b.Name).ToList();
    foreach (var item in blogs)
    {
      Console.WriteLine($"{item.BlogId}) {item.Name}");
    }    

  }
  else if (choice == "4")
  {
    // Display all Posts from a Blog
  }
} while (choice == "1" || choice == "2" || choice == "3" || choice == "4");


