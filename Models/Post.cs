#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace CSharpBelt.Models;

public class Post
{
    [Key]
    public int PostId {get;set;}

    [Required]
    public string Image {get;set;}

    [Required]
    public string Title {get;set;}

    [Required]
    public string Medium {get;set;}

    [Required]
    public bool ForSale {get;set;}

    public int UserId {get;set;}

    public User? Creator {get;set;}

    public List<Like> LikesP {get;set;} = new List<Like>();

    public DateTime CreatedAt {get;set;} = DateTime.Now;

    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public int Liked(int uid)
    {
        foreach (Like l in LikesP)
        {
            if(l.UserId == uid)
            {
                return l.LikeId;
            }
        }
        return 0;
    }
}