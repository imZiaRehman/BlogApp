using BlogApp.Data;
using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BlogApp.ViewModels;

namespace BlogApp.Repositories
{
    public class BlogRepository
    {
        private readonly BlogDbContext _context;
        public BlogRepository(BlogDbContext context)
        {
            _context = context;
        }

        public List<Post> GetPosts()
        {
            return _context.Posts.Include(p => p.Comments).ToList();
        }

        public PostViewModel GetPostById(int postId)
        {

            var postViewModel = _context.Posts
             .Include(p => p.user)
            .Include(p => p.Comments)
            .Include(p => p.Attachments)
            .Include(p => p.Likes)
            .Select(p => new PostViewModel
            {
                PostId = p.PostId,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                UserName = p.user.FirstName + p.user.LastName,
                Attachments = p.Attachments,
                Likes = p.Likes,
                UserHasLiked = p.Likes.Any(l => l.UserId == p.UserId && l.PostId == p.PostId),

                // Map comments for the post
                Comments = p.Comments.Select(c => new CommentViewModel
                {
                    CommentId = c.CommentId,
                    CommentText = c.Content, 
                    CreatedAt = c.CreatedAt,
                    UserName = p.user.FirstName + p.user.LastName 
                }).ToList()
            })
            .FirstOrDefault(p => p.PostId == postId);

            return postViewModel;

        }


        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public List<PostViewModel> GetAllPosts()
        {

            var postViewModels = _context.Posts
            .Include(p => p.user)
            .Select(p => new PostViewModel{
                PostId = p.PostId,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                UserName = p.user.FirstName + p.user.LastName, 
                                    
                })
                .ToList();

            return postViewModels;

        }

        public void AddLike(int userId, int postId)
        {
            var like = new Like
            {
                UserId = userId,
                PostId = postId
            };

            _context.Likes.Add(like);
            _context.SaveChanges();
        }

        public bool HasUserLikedPost(int postId, int userId)
        {
            var hasLiked = _context.Likes.Any(l => l.PostId == postId && l.UserId == userId);
            return hasLiked;
        }

        public void UnlikePost(int postId, int userId)
        {
            var likeToRemove = _context.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            if (likeToRemove != null)
            {
                _context.Likes.Remove(likeToRemove);
                _context.SaveChanges();
            }
        }


    }



}
