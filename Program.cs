﻿using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
using NLog;
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
    var blogIdoption = Console.ReadLine();
    if (!int.TryParse(blogIdoption, out int blogId))
      {
        Console.WriteLine("Invalid Blog Id");
      }
    else if (int.TryParse(blogIdoption, out int blogId2) && !blogs.Any(b => b.BlogId == blogId2))
      {
        Console.WriteLine("There are no Blogs saved with that Id");
      }
    else if (int.TryParse(blogIdoption, out int blogId3) && blogs.Any(b => b.BlogId == blogId3))
        {
          Console.WriteLine("Enter the Post title");
          var PostTitle = Console.ReadLine();
      if (string.IsNullOrEmpty(PostTitle))
      {
        Console.WriteLine("Post title cannot be null");
      }
      else if (!string.IsNullOrEmpty(PostTitle))
            {
                Console.WriteLine("Enter the Post content");
                var PostContent = Console.ReadLine();
                var post = new Post { Title = PostTitle, Content = PostContent, BlogId = blogId3 };
                db.AddPost(post);
                logger.Info("Post added - {PostTitle}", PostTitle);
  
            }
        }

  }
  else if (choice == "4")
  {
    // Display all Posts from a Blog
    Console.WriteLine("Select the blog you would like to post to:");
    Console.WriteLine("0) Posts from all Blogs");
    var blogs = db.Blogs.OrderBy(b => b.Name).ToList();
    foreach (var item in blogs)
    {
      Console.WriteLine($"{item.BlogId}) {item.Name}");
    }
    var blogPostOption = Console.ReadLine();
    if (!int.TryParse(blogPostOption, out int blogId))
    {
      Console.WriteLine("Invalid Blog Id");
    }
    else if (int.TryParse(blogPostOption, out int blogId2) && !blogs.Any(b => b.BlogId == blogId2))
        {
            Console.WriteLine("There are no Blogs saved with that Id");
        }
    else if (blogPostOption == "0")
    {
      var posts = db.Posts.OrderBy(p => p.Title).ToList();
      Console.WriteLine($"{posts.Count} post(s) returned");

      foreach (var post in posts)
      {
        Console.WriteLine($"Blog: {post.Blog.Name}\nTitle: {post.Title}\nContent: {post.Content}");
      }
    }
  }
} while (choice == "1" || choice == "2" || choice == "3" || choice == "4");














// QUESTIONS
// example different from class demo
// ids out of order when displaying blogs
