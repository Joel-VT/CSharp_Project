using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CSharpBelt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CSharpBelt.Controllers;

public class PostController : Controller
{
    private readonly ILogger<PostController> _logger;
    private MyContext _context;

    public PostController(ILogger<PostController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [SessionCheck]
    [HttpGet("posts")]
    public IActionResult Posts()
    {
        List<Post> EveryPosting = _context.Posts.OrderByDescending(p => p.CreatedAt).Include(p => p.Creator).Include(w => w.LikesP).ToList();
        ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
        return View(EveryPosting);
    }

    [SessionCheck]
    [HttpGet("posts/new")]
    public IActionResult NewPost()
    {
        return View();
    }

    [SessionCheck]
    [HttpPost("posts/create")]
    public IActionResult CreatePost(Post newPost)
    {
        if (ModelState.IsValid)
        {
            _context.Add(newPost);
            _context.SaveChanges();
            return Redirect($"/posts/{newPost.PostId}");
        }
        else
        {
            return View("NewPost");
        }
    }

    [SessionCheck]
    [HttpGet("posts/{id}")]
    public IActionResult OnePost(int id)
    {
        Post? one = _context.Posts.Include(p => p.Creator).Include(p => p.LikesP).FirstOrDefault(p => p.PostId == id);
        ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
        return View(one);
    }

    [SessionCheck]
    [HttpPost("posts/{id}/destroy")]
    public IActionResult DestroyPost(int id)
    {
        Post? PostToDelete = _context.Posts.SingleOrDefault(p => p.PostId == id);
        _context.Posts.Remove(PostToDelete);
        _context.SaveChanges();
        return RedirectToAction("Posts");
    }

    [SessionCheck]
    [HttpPost("posts/likes/{id}/destroy")]
    public IActionResult DestroyLike(int id)
    {
        Like? LikeToDelete = _context.Likes.SingleOrDefault(l => l.LikeId == id);
        _context.Likes.Remove(LikeToDelete);
        _context.SaveChanges();
        return RedirectToAction("Posts");
    }

    [SessionCheck]
    [HttpPost("likes/{id}/{pid}/destroy")]
    public IActionResult DestroyPostLike(int id,int pid)
    {
        Like? LikeToDelete = _context.Likes.SingleOrDefault(l => l.LikeId == id);
        _context.Likes.Remove(LikeToDelete);
        _context.SaveChanges();
        return Redirect($"/posts/{pid}");
    }

    [SessionCheck]
    [HttpPost("posts/likes/create")]
    public IActionResult CreateLike(Like newLike)
    {
        if (ModelState.IsValid)
        {
            _context.Add(newLike);
            _context.SaveChanges();
            return RedirectToAction("Posts");
        }
        else
        {
            List<Post> EveryPosting = _context.Posts.OrderByDescending(p => p.CreatedAt).Include(w => w.LikesP).ToList();
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View("Posts", EveryPosting);
        }
    }
    
    [SessionCheck]
    [HttpPost("likes/{pid}/create")]
    public IActionResult CreatePostLike(Like newLike, int pid)
    {
        if (ModelState.IsValid)
        {
            _context.Add(newLike);
            _context.SaveChanges();
            return Redirect($"/posts/{pid}");
        }
        else
        {
            List<Post> EveryPosting = _context.Posts.OrderByDescending(p => p.CreatedAt).Include(w => w.LikesP).ToList();
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View("Posts", EveryPosting);
        }
    }

    [SessionCheck]
    [HttpGet("posts/{id}/edit")]
    public IActionResult EditPost(int id)
    {
        Post? PostToEdit = _context.Posts.FirstOrDefault(p => p.PostId == id);
        return View(PostToEdit);
    }

    [SessionCheck]
    [HttpPost("posts/{id}/update")]
    public IActionResult UpdatePost(int id, Post UpdatedPost)
    {
        Post? PostToUpdate = _context.Posts.FirstOrDefault(d => d.PostId == id);
        if(PostToUpdate == null)
        {
            return RedirectToAction("Posts");
        }
        if(ModelState.IsValid)
        {
            PostToUpdate.Image = UpdatedPost.Image;
            PostToUpdate.Title = UpdatedPost.Title;
            PostToUpdate.Medium = UpdatedPost.Medium;
            PostToUpdate.ForSale = UpdatedPost.ForSale;
            PostToUpdate.UserId = UpdatedPost.UserId;
            PostToUpdate.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return Redirect($"/posts/{id}");
        }
        else
        {
            return View("EditPost", PostToUpdate);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? UserId = context.HttpContext.Session.GetInt32("UserId");
        if (UserId == null)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}