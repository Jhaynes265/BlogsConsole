﻿using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.DataAnnotations;
using NLog;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

string? choice;

do
{
  // display choices to user
  Console.WriteLine("\nEnter your selection:");
  Console.WriteLine("1) Display all Blogs");
  Console.WriteLine("2) Add Blog");
  Console.WriteLine("3) Create Post");
  Console.WriteLine("4) Display Posts");
  Console.WriteLine("5) Delete Blog");
  Console.WriteLine("6) Edit Blog");
  Console.WriteLine("Enter q to quit\n");

  // input selection
  choice = Console.ReadLine();
  logger.Info("User choice: {Choice}", choice);

  if (choice == "1")
  {
    var db = new DataContext();
    // Display all Blogs from the database
    var query = db.Blogs.OrderBy(b => b.BlogId);

    Console.WriteLine($"{db.Blogs.Count()} Blogs returned");
    foreach (var item in query)
    {
      Console.WriteLine(item.Name);
    }
  }
  else if (choice == "2")
  {
    // Create and save a new Blog
    var db = new DataContext();
    Blog? blog = InputBlog(db, logger);
    if (blog != null)
    {
      //blog.BlogId = BlogId;
      db.AddBlog(blog);
      logger.Info("Blog added - {name}", blog.Name);
    }
  }
  else if (choice == "3")
  {
    var db = new DataContext();
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
    var db = new DataContext();
    // Display all Posts from a Blog
    Console.WriteLine("Select the blog you would like to post to:");
    Console.WriteLine("0) Posts from all Blogs");
    var blogs = db.Blogs.OrderBy(b => b.BlogId).ToList();
    foreach (var item in blogs)
    {
      Console.WriteLine($"{item.BlogId}) {item.Name}");
    }
    var blogPostOption = Console.ReadLine();

    if (blogPostOption == "0")
    {
      var posts = db.Posts.OrderBy(p => p.Title).ToList();
      Console.WriteLine($"{posts.Count} post(s) returned");

      foreach (var post in posts)
      {
        Console.WriteLine($"\nBlog: {post.Blog.Name}\nTitle: {post.Title}\nContent: {post.Content}");
      }
    }
    else if (!int.TryParse(blogPostOption, out int blogId))
    {
      Console.WriteLine("Invalid Blog Id");
    }
    else if (int.TryParse(blogPostOption, out int blogId2) && !blogs.Any(b => b.BlogId == blogId2))
    {
      Console.WriteLine("There are no Blogs saved with that Id");
    }
    else if (int.TryParse(blogPostOption, out int blogId3) && blogs.Any(b => b.BlogId == blogId3))
        {
            var posts = db.Posts.Where(p => p.BlogId == blogId3).OrderBy(p => p.Title).ToList();
            Console.WriteLine($"{posts.Count} post(s) returned");

            foreach (var post in posts)
            {
                Console.WriteLine($"\nBlog: {post.Blog.Name}\nTitle: {post.Title}\nContent: {post.Content}");
            }
        }
  }

  else if (choice == "5")
  {
    // delete blog
    Console.WriteLine("Choose the blog to delete:");
        var db = new DataContext();
    var blog = GetBlog(db);
    if (blog != null)
    {
      // delete blog
      db.DeleteBlog(blog);
      logger.Info($"Blog (id: {blog.BlogId}) deleted");
    }
    else
    {
      logger.Error("Blog is null");
    }
  }
    else if (choice == "6")
  {
    // edit blog
    Console.WriteLine("Choose the blog to edit:");
    var db = new DataContext();
    var blog = GetBlog(db);
    if (blog != null)
    {
      // TODO: input blog
    }
  }
} while (choice == "1" || choice == "2" || choice == "3" || choice == "4" || choice == "5" || choice == "6");



static Blog? GetBlog(DataContext db)
{
  // display all blogs
  var blogs = db.Blogs.OrderBy(b => b.BlogId);
  foreach (Blog b in blogs)
  {
    Console.WriteLine($"{b.BlogId}: {b.Name}");
  }
  if (int.TryParse(Console.ReadLine(), out int BlogId))
  {
    Blog blog = db.Blogs.FirstOrDefault(b => b.BlogId == BlogId)!;
    return blog;
  }
  return null;
}

static Blog? InputBlog(DataContext db, NLog.Logger logger)
{
  Blog blog = new();
  Console.WriteLine("Enter the Blog name");
  blog.Name = Console.ReadLine();

  ValidationContext context = new(blog, null, null);
  List<ValidationResult> results = [];

  var isValid = Validator.TryValidateObject(blog, context, results, true);
  if (isValid)
  {
    // check for unique name
    if (db.Blogs.Any(b => b.Name == blog.Name))
    {
      // generate validation error
      isValid = false;
      results.Add(new ValidationResult("Blog name exists", ["Name"]));
    }
    else
    {
      logger.Info("Validation passed");
    }
  }
  if (!isValid)
  {
    foreach (var result in results)
    {
      logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
    }
    return null;
  }
  return blog;
}